#include <MMKV.h>
#include <cmmkv.h>
#include <string>
#include <locale>
#include <codecvt>
#include <memory>

template<class Facet>
struct deletable_facet : Facet
{
    template<class... Args>
    deletable_facet(Args&&... args) : Facet(std::forward<Args>(args)...) {}
    ~deletable_facet() {}
};

inline std::string strToU8(const char16_t* str)
{
    if (!str)
    {
        return "";
    }
    return std::wstring_convert<deletable_facet<std::codecvt<char16_t, char, std::mbstate_t>>, char16_t>().to_bytes(str);
}

inline MMKVPath_t strToPath(const char16_t* str)
{
#ifdef _WIN32
    if (!str)
    {
        return L"";
    }
    return std::wstring(reinterpret_cast<const wchar_t*>(str));
#else
    return string2MMKVPath_t(strToU8(str));
#endif
}

inline MMKVStringBox* mmkvStringBoxNewInternal(const std::string& str)
{
    return reinterpret_cast<MMKVStringBox*>(new std::string(str));
}

inline MMKVStringBox* mmkvStringBoxNewInternal(std::string&& str)
{
    return reinterpret_cast<MMKVStringBox*>(new std::string(std::move(str)));
}

inline std::vector<std::string> stringArrayToVector(const char16_t** strings, size_t length)
{
    std::vector<std::string> result;
    result.reserve(length);
    for (size_t i = 0; i < length; i++)
    {
        result.push_back(strToU8(strings[i]));
    }
    return result;
}

extern "C"
{
    void MMKVCALL mmkvInit(const char16_t* rootDir, MMKVLogLevel logLevel)
    {
        MMKV::initializeMMKV(strToPath(rootDir), logLevel);
    }

    void MMKVCALL mmkvStringBoxDelete(MMKVStringBox* str)
    {
        if (str)
        {
            delete reinterpret_cast<std::string*>(str);
        }
    }

    MMKVStringBox* MMKVCALL mmkvStringBoxNew(const char16_t* str)
    {
        if (str)
        {
            return mmkvStringBoxNewInternal(strToU8(str));
        }
        return nullptr;
    }

    void* MMKVCALL mmkvStringBoxAccessU8(MMKVStringBox* str, MMKVStringBoxAccessorU8 accessor)
    {
        if (str)
        {
            auto native = reinterpret_cast<std::string*>(str);
            return accessor(native->c_str(), native->length());
        }
        return nullptr;
    }

    MMKV* MMKVCALL mmkvWithID(
        const char16_t* mmapID, 
        MMKVMode mode,
        const char16_t* cryptKey,
        const char16_t* rootPath)
    {
        MMKV* kv = nullptr;
        if (!mmapID) {
            return kv;
        }
        auto str = strToU8(mmapID);

        bool done = false;
        if (cryptKey)
        {
            auto crypt = strToU8(cryptKey);
            if (crypt.length() > 0) {
                if (rootPath) 
                {
                    auto path = strToPath(rootPath);
                    kv = MMKV::mmkvWithID(str,  mode, &crypt, &path);
                }
                else 
                {
                    kv = MMKV::mmkvWithID(str,  mode, &crypt, nullptr);
                }
                done = true;
            }
        }
        if (!done)
        {
            if (rootPath)
            {
                auto path = strToPath(rootPath);
                kv = MMKV::mmkvWithID(str,  mode, nullptr, &path);
            } 
            else
            {
                kv = MMKV::mmkvWithID(str,  mode, nullptr, nullptr);
            }
        }
        return kv;
    }

    MMKV* MMKVCALL mmkvDefault(
        MMKVMode mode,
        const char16_t* cryptKey)
    {
        MMKV* kv = nullptr;
        if (cryptKey) 
        {
            auto crypt = strToU8(cryptKey);
            if (crypt.length() > 0)
            {
                kv = MMKV::defaultMMKV(mode, &crypt);
            }
        }
        if (!kv) 
        {
            kv = MMKV::defaultMMKV(mode, nullptr);
        }
        return kv;
    }

    MMKVStringBox* MMKVCALL mmkvMmapID(MMKV* kv)
    {
        if (kv)
        {
            return mmkvStringBoxNewInternal(kv->mmapID());
        }
        return nullptr;
    }

    bool MMKVCALL mmkvIsExpirationEnabled(MMKV* kv)
    {
        if (kv)
        {
            return kv->isExpirationEnabled();
        }
        return false;
    }

    bool MMKVCALL mmkvEnableAutoKeyExpire(MMKV* kv, uint32_t defaultExpireDuration)
    {
        if (kv)
        {
            return kv->enableAutoKeyExpire(defaultExpireDuration);
        }
        return false;
    }

    bool MMKVCALL mmkvDisableAutoKeyExpire(MMKV* kv)
    {
        if (kv)
        {
            return kv->disableAutoKeyExpire();
        }
        return false;
    }

    void* MMKVCALL mmkvAccessAllKeys(MMKV* kv, bool filterExpire, MMKVStringArrayAccessorU8 accessor)
    {
        if (kv)
        {
            auto keys = kv->allKeys(filterExpire);
            size_t length = keys.size();
            if (length == 0)
            {
                return nullptr;
            }
            // Skip out-of-bounds checking for performance
            auto pBegin = keys.data();
            std::unique_ptr<const char*[]> lpArray = std::make_unique<const char*[]>(length);
            for (size_t i = 0; i < length; i++)
            {
                lpArray[i] = pBegin[i].c_str();
            }
            return accessor(lpArray.get(), length);
        }
        return nullptr;
    }

    bool MMKVCALL mmkvSetBool(MMKV* kv, const char16_t* key, bool value)
    {
        if (kv)
        {
            return kv->set(value, strToU8(key));
        }
        return false;
    }

    bool MMKVCALL mmkvSetBoolExpirable(MMKV* kv, const char16_t* key, bool value, uint32_t expireDuration)
    {
        if (kv && kv->isExpirationEnabled())
        {
            return kv->set(value, strToU8(key), expireDuration);
        }
        return false;
    }

    bool MMKVCALL mmkvGetBool(MMKV* kv, const char16_t* key, bool defaultValue, bool *hasValue)
    {
        if (kv)
        {
            auto result = kv->getBool(strToU8(key), defaultValue, hasValue);
            return result;
        }
        else
        {
            if (hasValue)
            {
                *hasValue = false;
            }
            return defaultValue;
        }
    }

    bool MMKVCALL mmkvSetInt32(MMKV* kv, const char16_t* key, int32_t value)
    {
        if (kv)
        {
            return kv->set(value, strToU8(key));
        }
        return false;
    }

    bool MMKVCALL mmkvSetInt32Expirable(MMKV* kv, const char16_t* key, int32_t value, uint32_t expireDuration)
    {
        if (kv && kv->isExpirationEnabled())
        {
            return kv->set(value, strToU8(key), expireDuration);
        }
        return false;
    }

    int32_t MMKVCALL mmkvGetInt32(MMKV* kv, const char16_t* key, int32_t defaultValue, bool* hasValue)
    {
        if (kv)
        {
            auto result = kv->getInt32(strToU8(key), defaultValue, hasValue);
            return result;
        }
        else
        {
            if (hasValue)
            {
                *hasValue = false;
            }
            return defaultValue;
        }
    }

    bool MMKVCALL mmkvSetInt64(MMKV* kv, const char16_t* key, int64_t value)
    {
        if (kv)
        {
            return kv->set(value, strToU8(key));
        }
        return false;
    }

    bool MMKVCALL mmkvSetInt64Expirable(MMKV* kv, const char16_t* key, int64_t value, uint32_t expireDuration)
    {
        if (kv && kv->isExpirationEnabled())
        {
            return kv->set(value, strToU8(key), expireDuration);
        }
        return false;
    }

    int64_t MMKVCALL mmkvGetInt64(MMKV* kv, const char16_t* key, int64_t defaultValue, bool* hasValue)
    {
        if (kv)
        {
            auto result = kv->getInt64(strToU8(key), defaultValue, hasValue);
            return result;
        }
        else
        {
            if (hasValue)
            {
                *hasValue = false;
            }
            return defaultValue;
        }
    }

    bool MMKVCALL mmkvSetUInt32(MMKV* kv, const char16_t* key, uint32_t value)
    {
        if (kv)
        {
            return kv->set(value, strToU8(key));
        }
        return false;
    }

    bool MMKVCALL mmkvSetUInt32Expirable(MMKV* kv, const char16_t* key, uint32_t value, uint32_t expireDuration)
    {
        if (kv && kv->isExpirationEnabled())
        {
            return kv->set(value, strToU8(key), expireDuration);
        }
        return false;
    }

    uint32_t MMKVCALL mmkvGetUInt32(MMKV* kv, const char16_t* key, uint32_t defaultValue, bool* hasValue)
    {
        if (kv)
        {
            auto result = kv->getUInt32(strToU8(key), defaultValue, hasValue);
            return result;
        }
        else
        {
            if (hasValue)
            {
                *hasValue = false;
            }
            return defaultValue;
        }
    }

    bool MMKVCALL mmkvSetUInt64(MMKV* kv, const char16_t* key, uint64_t value)
    {
        if (kv)
        {
            return kv->set(value, strToU8(key));
        }
        return false;
    }

    bool MMKVCALL mmkvSetUInt64Expirable(MMKV* kv, const char16_t* key, uint64_t value, uint32_t expireDuration)
    {
        if (kv && kv->isExpirationEnabled())
        {
            return kv->set(value, strToU8(key), expireDuration);
        }
        return false;
    }

    uint64_t MMKVCALL mmkvGetUInt64(MMKV* kv, const char16_t* key, uint64_t defaultValue, bool* hasValue)
    {
        if (kv)
        {
            auto result = kv->getUInt64(strToU8(key), defaultValue, hasValue);
            return result;
        }
        else
        {
            if (hasValue)
            {
                *hasValue = false;
            }
            return defaultValue;
        }
    }

    bool MMKVCALL mmkvSetFloat(MMKV* kv, const char16_t* key, float value)
    {
        if (kv)
        {
            return kv->set(value, strToU8(key));
        }
        return false;
    }

    bool MMKVCALL mmkvSetFloatExpirable(MMKV* kv, const char16_t* key, float value, uint32_t expireDuration)
    {
        if (kv && kv->isExpirationEnabled())
        {
            return kv->set(value, strToU8(key), expireDuration);
        }
        return false;
    }

    float MMKVCALL mmkvGetFloat(MMKV* kv, const char16_t* key, float defaultValue, bool* hasValue)
    {
        if (kv)
        {
            auto result = kv->getFloat(strToU8(key), defaultValue, hasValue);
            return result;
        }
        else
        {
            if (hasValue)
            {
                *hasValue = false;
            }
            return defaultValue;
        }
    }

    bool MMKVCALL mmkvSetDouble(MMKV* kv, const char16_t* key, double value)
    {
        if (kv)
        {
            return kv->set(value, strToU8(key));
        }
        return false;
    }

    bool MMKVCALL mmkvSetDoubleExpirable(MMKV* kv, const char16_t* key, double value, uint32_t expireDuration)
    {
        if (kv && kv->isExpirationEnabled())
        {
            return kv->set(value, strToU8(key), expireDuration);
        }
        return false;
    }

    double MMKVCALL mmkvGetDouble(MMKV* kv, const char16_t* key, double defaultValue, bool* hasValue)
    {
        if (kv)
        {
            auto result = kv->getDouble(strToU8(key), defaultValue, hasValue);
            return result;
        }
        else
        {
            if (hasValue)
            {
                *hasValue = false;
            }
            return defaultValue;
        }
    }

    bool MMKVCALL mmkvSetString(MMKV* kv, const char16_t* key, const char16_t* value)
    {
        if (kv)
        {
            return kv->set(strToU8(value), strToU8(key));
        }
        return false;
    }

    bool MMKVCALL mmkvSetStringExpirable(MMKV* kv, const char16_t* key, const char16_t* value, uint32_t expireDuration)
    {
        if (kv && kv->isExpirationEnabled())
        {
            return kv->set(strToU8(value), strToU8(key), expireDuration);
        }
        return false;
    }

    MMKVStringBox* MMKVCALL mmkvGetString(MMKV* kv, const char16_t* key, const char16_t* defaultValue, bool* hasValue)
    {
        if (kv)
        {
            std::string result;
            bool bhasValue = kv->getString(strToU8(key), result);
            if (hasValue)
            {
                *hasValue = bhasValue;
            }
            if (bhasValue)
            {
                return mmkvStringBoxNewInternal(result);
            }
            return mmkvStringBoxNew(defaultValue);
        }
        else
        {
            if (hasValue)
            {
                *hasValue = false;
            }
            return mmkvStringBoxNew(defaultValue);
        }
    }

    bool MMKVCALL mmkvSetBytes(MMKV* kv, const char16_t* key, void* data, size_t length)
    {
        if (kv)
        {
            return kv->set(mmkv::MMBuffer(data, length), strToU8(key));
        }
        return false;
    }

    bool MMKVCALL mmkvSetBytesExpirable(MMKV* kv, const char16_t* key, void* data, size_t length, uint32_t expireDuration)
    {
        if (kv && kv->isExpirationEnabled())
        {
            return kv->set(mmkv::MMBuffer(data, length), strToU8(key), expireDuration);
        }
        return false;
    }

    void* MMKVCALL mmkvAccessBytes(MMKV* kv, const char16_t* key, bool* hasValue, MMKVBytesAccessor accessor)
    {
        if (kv)
        {
            auto nativeKey = strToU8(key);
            if (kv->containsKey(nativeKey)) 
            {
                mmkv::MMBuffer value = kv->getBytes(nativeKey);
                if (hasValue)
                {
                    *hasValue = true;
                }
                return accessor(value.getPtr(), value.length());
            }
        }
        if (hasValue)
        {
            *hasValue = false;
        }
        return nullptr;
    }

    bool MMKVCALL mmkvSetStringArray(MMKV* kv, const char16_t* key, const char16_t** strings, size_t length)
    {
        if (kv)
        {
            return kv->set(stringArrayToVector(strings, length), strToU8(key));
        }
        return false;
    }

    bool MMKVCALL mmkvSetStringArrayExpirable(MMKV* kv, const char16_t* key, const char16_t** strings, size_t length, uint32_t expireDuration)
    {
        if (kv && kv->isExpirationEnabled())
        {
            return kv->set(stringArrayToVector(strings, length), strToU8(key), expireDuration);
        }
        return false;
    }

    void* MMKVCALL mmkvAccessStringArray(MMKV* kv, const char16_t* key, bool* hasValue, MMKVStringArrayAccessorU8 accessor)
    {
        if (kv)
        {
            std::vector<std::string> result;
            if (kv->getVector(strToU8(key), result)) 
            {
                if (hasValue)
                {
                    *hasValue = true;
                }
                size_t length = result.size();
                if (length == 0)
                {
                    return accessor(nullptr, length);
                }
                // Skip out-of-bounds checking for performance
                auto pBegin = result.data();
                std::unique_ptr<const char*[]> lpArray = std::make_unique<const char*[]>(length);
                for (size_t i = 0; i < length; i++)
                {
                    lpArray[i] = pBegin[i].c_str();
                }
                return accessor(lpArray.get(), length);
            }
        }
        if (hasValue)
        {
            *hasValue = false;
        }
        return nullptr;
    }

    bool MMKVCALL mmkvContainsKey(MMKV* kv, const char16_t* key)
    {
        if (kv)
        {
            return kv->containsKey(strToU8(key));
        }
        return false;
    }

    size_t MMKVCALL mmkvCount(MMKV* kv)
    {
        if (kv)
        {
            return kv->count();
        }
        return 0;
    }

    size_t MMKVCALL mmkvCountNonExpired(MMKV* kv)
    {
        if (kv)
        {
            return kv->count(true);
        }
        return 0;
    }

    size_t MMKVCALL mmkvTotalSize(MMKV* kv)
    {
        if (kv)
        {
            return kv->totalSize();
        }
        return 0;
    }

    size_t MMKVCALL mmkvActualSize(MMKV* kv)
    {
        if (kv)
        {
            return kv->actualSize();
        }
        return 0;
    }

    void MMKVCALL mmkvRemoveValueForKey(MMKV* kv, const char16_t* key)
    {
        if (kv)
        {
            kv->removeValueForKey(strToU8(key));
        }
    }

    void MMKVCALL mmkvRemoveValuesForKeys(MMKV* kv, const char16_t** keys, size_t length)
    {
        if (kv)
        {
            kv->removeValuesForKeys(stringArrayToVector(keys, length));
        }
    }

    void MMKVCALL mmkvClearAll(MMKV* kv)
    {
        if (kv)
        {
            kv->clearAll();
        }
    }

    void MMKVCALL mmkvSync(MMKV* kv, int32_t flag)
    {
        if (kv)
        {
            kv->sync((SyncFlag)flag);
        }
    }

    bool MMKVCALL mmkvIsFileValid(
        MMKVMode mode,
        const char16_t* mmapID,
        const char16_t* rootPath)
    {
        if (mmapID) 
        {
            if (!rootPath) 
            {
                return MMKV::isFileValid(strToU8(mmapID), nullptr);
            }
            else 
            {
                auto root = strToPath(rootPath);
                return MMKV::isFileValid(strToU8(mmapID), &root);
            }
        }
        return false;
    }

    void MMKVCALL mmkvLock(MMKV* kv)
    {
        if (kv)
        {
            kv->lock();
        }
    }

    void MMKVCALL mmkvUnlock(MMKV* kv)
    {
        if (kv)
        {
            kv->unlock();
        }
    }

    void MMKVCALL mmkvClearMemoryCache(MMKV* kv)
    {
        if (kv)
        {
            kv->clearMemoryCache();
        }
    }

    bool MMKVCALL mmkvTryLock(MMKV* kv)
    {
        if (kv) 
        {
            return kv->try_lock();
        }
        return false;
    }

    MMKVStringBox* MMKVCALL mmkvVersion()
    {
        return mmkvStringBoxNewInternal(MMKV_VERSION);
    }

    void MMKVCALL mmkvTrim(MMKV* kv)
    {
        if (kv)
        {
            kv->trim();
        }
    }

    void MMKVCALL mmkvClose(MMKV* kv)
    {
        if (kv)
        {
            kv->close();
        }
    }

    void MMKVCALL mmkvSetLogLevel(MMKVLogLevel level) 
    {
        MMKV::setLogLevel(level);
    }

    void MMKVCALL mmkvSetErrorHandler(mmkv::ErrorHandler handler)
    {
        if (handler)
        {
            MMKV::registerErrorHandler(handler);
        }
        else
        {
            MMKV::unRegisterErrorHandler();
        }
    }

    // Multi-thread unsafe
    inline MMKVLogHandlerU8 myLogHandlerU8;
    inline MMKVErrorHandlerU8 myErrorHandlerU8;
    inline MMKVContentChangedHandlerU8 myContentChangedHandlerU8;
    inline void handleLog(MMKVLogLevel level, const char* file, int line, const char* function, MMKVLog_t message)
    {
        myLogHandlerU8(level, file, line, function, message.c_str());
    }
    inline MMKVRecoverStrategic handleError(const std::string& mmapID, MMKVErrorType errorType)
    {
        MMKVRecoverStrategic recoverStrategic = OnErrorDiscard;
        myErrorHandlerU8(mmapID.c_str(), errorType, &recoverStrategic);
        return recoverStrategic;
    }
    inline void handleContentChanged(const std::string& mmapID)
    {
        myContentChangedHandlerU8(mmapID.c_str());
    }

    void MMKVCALL mmkvSetLogHandlerU8(MMKVLogHandlerU8 handler)
    {
        if (handler)
        {
            myLogHandlerU8 = handler;
            MMKV::registerLogHandler(&handleLog);
        }
        else
        {
            MMKV::unRegisterLogHandler();
        }
    }
    void MMKVCALL mmkvSetErrorHandlerU8(MMKVErrorHandlerU8 handler)
    {
        if (handler)
        {
            myErrorHandlerU8 = handler;
            MMKV::registerErrorHandler(&handleError);
        }
        else
        {
            MMKV::unRegisterErrorHandler();
        }
    }
    void MMKVCALL mmkvSetContentChangedHandlerU8(MMKVContentChangedHandlerU8 handler)
    {
        if (handler)
        {
            myContentChangedHandlerU8 = handler;
            MMKV::registerContentChangeHandler(&handleContentChanged);
        }
        else
        {
            MMKV::unRegisterContentChangeHandler();
        }
    }

    bool MMKVCALL mmkvBackupOneToDirectory(const char16_t* mmapID, const char16_t* dstPath, const char16_t* srcPath)
    {
        if (srcPath)
        {
            auto nativeSrcPath = strToPath(srcPath);
            return MMKV::backupOneToDirectory(strToU8(mmapID), strToPath(dstPath), &nativeSrcPath);
        } 
        else 
        {
            return MMKV::backupOneToDirectory(strToU8(mmapID), strToPath(dstPath), nullptr);
        }
    }

    size_t MMKVCALL mmkvBackupAllToDirectory(const char16_t* dstPath, const char16_t* srcPath)
    {
        if (srcPath)
        {
            auto nativeSrcPath = strToPath(srcPath);
            return MMKV::backupAllToDirectory(strToPath(dstPath), &nativeSrcPath);
        } 
        else 
        {
            return MMKV::backupAllToDirectory(strToPath(dstPath), nullptr);
        }
    }

    bool MMKVCALL mmkvRestoreOneFromDirectory(const char16_t* mmapID, const char16_t* srcDir, const char16_t* dstPath)
    {
        if (dstPath)
        {
            auto nativeDstPath = strToPath(dstPath);
            return MMKV::restoreOneFromDirectory(strToU8(mmapID), strToPath(srcDir), &nativeDstPath);
        } 
        else 
        {
            return MMKV::restoreOneFromDirectory(strToU8(mmapID), strToPath(srcDir), nullptr);
        }
    }

    size_t MMKVCALL mmkvRestoreAllFromDirectory(const char16_t* srcDir, const char16_t* dstPath)
    {
        if (dstPath)
        {
            auto nativeDstPath = strToPath(dstPath);
            return MMKV::restoreAllFromDirectory(strToPath(srcDir), &nativeDstPath);
        } 
        else 
        {
            return MMKV::restoreAllFromDirectory(strToPath(srcDir), nullptr);
        }
    }

    void MMKVCALL mmkvInitWithLogHandler(const char16_t* rootDir, MMKVLogLevel logLevel, MMKVLogHandlerU8 handler)
    {
        myLogHandlerU8 = handler;
        MMKV::initializeMMKV(strToPath(rootDir), logLevel, &handleLog);
    }
}