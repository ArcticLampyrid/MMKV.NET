#include <MMKV.h>
#include <cmmkv.h>
#include <string>
#include <locale>
#include <codecvt>

inline std::string strToU8(const char16_t* str)
{
    if (!str)
    {
        return "";
    }
    return std::wstring_convert<std::codecvt<char16_t, char, std::mbstate_t>, char16_t>().to_bytes(str);
}

inline MMKVPath_t strToPath(const char16_t* str)
{
    if constexpr ((sizeof(wchar_t) == sizeof(char16_t)) && std::is_same_v<MMKVPath_t, std::wstring>)
    {
        if (!str)
        {
            return L"";
        }
        return std::wstring(reinterpret_cast<const wchar_t*>(str));
    }
    else
    {
        return string2MMKVPath_t(strToU8(str));
    }
}

inline MMKVStringBox* mmkvStringBoxNewInternal(const std::string& str)
{
    return reinterpret_cast<MMKVStringBox*>(new std::string(str));
}

inline MMKVStringBox* mmkvStringBoxNewInternal(std::string&& str)
{
    return reinterpret_cast<MMKVStringBox*>(new std::string(std::move(str)));
}

extern "C"
{
    void __stdcall mmkvInit(const char16_t* rootDir, MMKVLogLevel logLevel)
    {
        MMKV::initializeMMKV(strToPath(rootDir), logLevel);
    }

    void __stdcall mmkvStringBoxDelete(MMKVStringBox* str)
    {
        if (str)
        {
            delete reinterpret_cast<std::string*>(str);
        }
    }

    MMKVStringBox* __stdcall mmkvStringBoxNew(const char16_t* str)
    {
        if (str)
        {
            return mmkvStringBoxNewInternal(strToU8(str));
        }
        return nullptr;
    }

    void* __stdcall mmkvStringBoxAccessU8(MMKVStringBox* str, MMKVStringBoxAccessorU8 accessor)
    {
        if (str)
        {
            auto native = reinterpret_cast<std::string*>(str);
            return accessor(native->c_str(), native->length());
        }
        return nullptr;
    }

    MMKV* __stdcall mmkvWithID(
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

    MMKV* __stdcall mmkvDefault(
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

    MMKVStringBox* __stdcall mmkvMmapID(MMKV* kv)
    {
        if (kv)
        {
            return mmkvStringBoxNewInternal(kv->mmapID());
        }
        return nullptr;
    }

    bool __stdcall mmkvSetBool(MMKV* kv, const char16_t* key, bool value)
    {
        if (kv)
        {
            return kv->set(value, strToU8(key));
        }
        return false;
    }

    bool __stdcall mmkvGetBool(MMKV* kv, const char16_t* key, bool defaultValue, bool *hasValue)
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

    bool __stdcall mmkvSetInt32(MMKV* kv, const char16_t* key, int32_t value)
    {
        if (kv)
        {
            return kv->set(value, strToU8(key));
        }
        return false;
    }

    int32_t __stdcall mmkvGetInt32(MMKV* kv, const char16_t* key, int32_t defaultValue, bool* hasValue)
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

    bool __stdcall mmkvSetInt64(MMKV* kv, const char16_t* key, int64_t value)
    {
        if (kv)
        {
            return kv->set(value, strToU8(key));
        }
        return false;
    }

    int64_t __stdcall mmkvGetInt64(MMKV* kv, const char16_t* key, int64_t defaultValue, bool* hasValue)
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

    bool __stdcall mmkvSetUInt32(MMKV* kv, const char16_t* key, uint32_t value)
    {
        if (kv)
        {
            return kv->set(value, strToU8(key));
        }
        return false;
    }

    uint32_t __stdcall mmkvGetUInt32(MMKV* kv, const char16_t* key, uint32_t defaultValue, bool* hasValue)
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

    bool __stdcall mmkvSetUInt64(MMKV* kv, const char16_t* key, uint64_t value)
    {
        if (kv)
        {
            return kv->set(value, strToU8(key));
        }
        return false;
    }

    uint64_t __stdcall mmkvGetUInt64(MMKV* kv, const char16_t* key, uint64_t defaultValue, bool* hasValue)
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

    bool __stdcall mmkvSetFloat(MMKV* kv, const char16_t* key, float value)
    {
        if (kv)
        {
            return kv->set(value, strToU8(key));
        }
        return false;
    }

    float __stdcall mmkvGetFloat(MMKV* kv, const char16_t* key, float defaultValue, bool* hasValue)
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

    bool __stdcall mmkvSetDouble(MMKV* kv, const char16_t* key, double value)
    {
        if (kv)
        {
            return kv->set(value, strToU8(key));
        }
        return false;
    }

    double __stdcall mmkvGetDouble(MMKV* kv, const char16_t* key, double defaultValue, bool* hasValue)
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

    bool __stdcall mmkvSetString(MMKV* kv, const char16_t* key, const char16_t* value)
    {
        if (kv)
        {
            return kv->set(strToU8(value), strToU8(key));
        }
        return false;
    }

    MMKVStringBox* __stdcall mmkvGetString(MMKV* kv, const char16_t* key, const char16_t* defaultValue, bool* hasValue)
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

    bool __stdcall mmkvSetBytes(MMKV* kv, const char16_t* key, void* data, size_t length)
    {
        if (kv)
        {
            return kv->set(mmkv::MMBuffer(data, length), strToU8(key));
        }
        return false;
    }

    void* __stdcall mmkvAccessBytes(MMKV* kv, const char16_t* key, bool* hasValue, MMKVBytesAccessor accessor)
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

    bool __stdcall mmkvContainsKey(MMKV* kv, const char16_t* key)
    {
        if (kv)
        {
            return kv->containsKey(strToU8(key));
        }
        return false;
    }

    size_t __stdcall mmkvCount(MMKV* kv)
    {
        if (kv)
        {
            return kv->count();
        }
        return 0;
    }

    size_t __stdcall mmkvTotalSize(MMKV* kv)
    {
        if (kv)
        {
            return kv->totalSize();
        }
        return 0;
    }

    size_t __stdcall mmkvActualSize(MMKV* kv)
    {
        if (kv)
        {
            return kv->actualSize();
        }
        return 0;
    }

    void __stdcall mmkvRemoveValueForKey(MMKV* kv, const char16_t* key)
    {
        if (kv)
        {
            kv->removeValueForKey(strToU8(key));
        }
    }

    void __stdcall mmkvClearAll(MMKV* kv)
    {
        if (kv)
        {
            kv->clearAll();
        }
    }

    void __stdcall mmkvSync(MMKV* kv, int32_t flag)
    {
        if (kv)
        {
            kv->sync((SyncFlag)flag);
        }
    }

    bool __stdcall mmkvIsFileValid(
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

    void __stdcall mmkvLock(MMKV* kv)
    {
        if (kv)
        {
            kv->lock();
        }
    }

    void __stdcall mmkvUnlock(MMKV* kv)
    {
        if (kv)
        {
            kv->unlock();
        }
    }

    void __stdcall mmkvClearMemoryCache(MMKV* kv)
    {
        if (kv)
        {
            kv->clearMemoryCache();
        }
    }

    bool __stdcall mmkvTryLock(MMKV* kv)
    {
        if (kv) 
        {
            return kv->try_lock();
        }
        return false;
    }

    MMKVStringBox* __stdcall mmkvVersion()
    {
        return mmkvStringBoxNewInternal(MMKV_VERSION);
    }

    void __stdcall mmkvTrim(MMKV* kv)
    {
        if (kv)
        {
            kv->trim();
        }
    }

    void __stdcall mmkvClose(MMKV* kv)
    {
        if (kv)
        {
            kv->close();
        }
    }

    void __stdcall mmkvSetLogLevel(MMKVLogLevel level) 
    {
        MMKV::setLogLevel(level);
    }

    void __stdcall mmkvSetErrorHandler(mmkv::ErrorHandler handler)
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

    void mmkvSetLogHandlerU8(MMKVLogHandlerU8 handler)
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
    void mmkvSetErrorHandlerU8(MMKVErrorHandlerU8 handler)
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
    void mmkvSetContentChangedHandlerU8(MMKVContentChangedHandlerU8 handler)
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
}