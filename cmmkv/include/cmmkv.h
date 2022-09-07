#pragma once
#ifndef MMKV_MMKV_H
enum MMKVMode : uint32_t {
    MMKV_SINGLE_PROCESS = 1 << 0,
    MMKV_MULTI_PROCESS = 1 << 1,
};
enum MMKVLogLevel : int {
    MMKVLogDebug = 0,
    MMKVLogInfo = 1,
    MMKVLogWarning,
    MMKVLogError,
    MMKVLogNone,
};
enum MMKVRecoverStrategic : int {
    OnErrorDiscard = 0,
    OnErrorRecover,
};
enum MMKVErrorType : int {
    MMKVCRCCheckFail = 0,
    MMKVFileLength,
};
#endif
struct MMKVStringBox;
class MMKV;
#if (defined(WIN32) || defined(_WIN32) || defined(__WIN32__) || defined(__NT__)) && !defined(_NI_mswin16_)
#define MMKVCALL __stdcall
#endif
#ifndef MMKVCALL
#define MMKVCALL 
#endif
typedef void* (MMKVCALL* MMKVBytesAccessor)(void* data, size_t length);
typedef void* (MMKVCALL* MMKVStringBoxAccessorU8)(const char* data, size_t length);
typedef void* (MMKVCALL* MMKVStringArrayAccessorU8)(const char** data, size_t length);
typedef void (MMKVCALL* MMKVLogHandlerU8)(MMKVLogLevel level, const char* file, int line, const char* function, const char* message);
typedef void (MMKVCALL* MMKVErrorHandlerU8)(const char* mmapID, MMKVErrorType errorType, MMKVRecoverStrategic* recoverStrategic);
typedef void (MMKVCALL* MMKVContentChangedHandlerU8)(const char* mmapID);