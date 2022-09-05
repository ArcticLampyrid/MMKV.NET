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
typedef void* (__stdcall* MMKVBytesAccessor)(void* data, size_t length);
typedef void* (__stdcall* MMKVStringBoxAccessorU8)(const char* data, size_t length);
typedef void (__stdcall* MMKVLogHandlerU8)(MMKVLogLevel level, const char* file, int line, const char* function, const char* message);
typedef void (__stdcall* MMKVErrorHandlerU8)(const char* mmapID, MMKVErrorType errorType, MMKVRecoverStrategic* recoverStrategic);
typedef void (__stdcall* MMKVContentChangedHandlerU8)(const char* mmapID);