using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using static Alampy.ManagedMmkv.Mmkv;

namespace Alampy.ManagedMmkv.Interop
{
    internal static class NativeMethods
    {
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate IntPtr MmkvStringBoxAccessorU8(IntPtr ptr, UIntPtr length);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate IntPtr MmkvBytesAccessor(IntPtr ptr, UIntPtr length);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate IntPtr MmkvStringArrayAccessorU8([MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPUTF8Str, SizeParamIndex = 1)] string[] data, UIntPtr length);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate void MmkvLogHandlerU8(MmkvLogLevel level, [MarshalAs(UnmanagedType.LPUTF8Str)] string file, int line, [MarshalAs(UnmanagedType.LPUTF8Str)] string function, [MarshalAs(UnmanagedType.LPUTF8Str)] string message);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate void MmkvErrorHandlerU8([MarshalAs(UnmanagedType.LPUTF8Str)] string mmapID, MmkvErrorType errorType, ref MmkvRecoverStrategic recoverStrategic);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate void MmkvContentChangedHandlerU8([MarshalAs(UnmanagedType.LPUTF8Str)] string mmapID);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr mmkvInit(string rootPath, MmkvLogLevel logLevel);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern void mmkvStringBoxDelete(IntPtr str);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr mmkvStringBoxNew(string str);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr mmkvStringBoxAccessU8(IntPtr str, MmkvStringBoxAccessorU8 accessor);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr mmkvWithID(string mmapID, MmkvMode mode, string cryptKey, string rootPath);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr mmkvDefault(MmkvMode mode, string cryptKey);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr mmkvMmapID(IntPtr kv);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool mmkvSetBool(IntPtr kv, string key, [MarshalAs(UnmanagedType.I1)] bool value);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool mmkvGetBool(IntPtr kv, string key, [MarshalAs(UnmanagedType.I1)] bool defaultValue, [MarshalAs(UnmanagedType.I1)] out bool hasValue);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool mmkvGetBool(IntPtr kv, string key, [MarshalAs(UnmanagedType.I1)] bool defaultValue, IntPtr hasValue);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool mmkvSetInt32(IntPtr kv, string key, int value);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern int mmkvGetInt32(IntPtr kv, string key, int defaultValue, [MarshalAs(UnmanagedType.I1)] out bool hasValue);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern int mmkvGetInt32(IntPtr kv, string key, int defaultValue, IntPtr hasValue);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool mmkvSetInt64(IntPtr kv, string key, long value);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern long mmkvGetInt64(IntPtr kv, string key, long defaultValue, [MarshalAs(UnmanagedType.I1)] out bool hasValue);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern long mmkvGetInt64(IntPtr kv, string key, long defaultValue, IntPtr hasValue);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool mmkvSetUInt32(IntPtr kv, string key, uint value);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern uint mmkvGetUInt32(IntPtr kv, string key, uint defaultValue, [MarshalAs(UnmanagedType.I1)] out bool hasValue);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern uint mmkvGetUInt32(IntPtr kv, string key, uint defaultValue, IntPtr hasValue);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool mmkvSetUInt64(IntPtr kv, string key, ulong value);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern ulong mmkvGetUInt64(IntPtr kv, string key, ulong defaultValue, [MarshalAs(UnmanagedType.I1)] out bool hasValue);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern ulong mmkvGetUInt64(IntPtr kv, string key, ulong defaultValue, IntPtr hasValue);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool mmkvSetFloat(IntPtr kv, string key, float value);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern float mmkvGetFloat(IntPtr kv, string key, float defaultValue, [MarshalAs(UnmanagedType.I1)] out bool hasValue);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern float mmkvGetFloat(IntPtr kv, string key, float defaultValue, IntPtr hasValue);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool mmkvSetDouble(IntPtr kv, string key, double value);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern double mmkvGetDouble(IntPtr kv, string key, double defaultValue, [MarshalAs(UnmanagedType.I1)] out bool hasValue);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern double mmkvGetDouble(IntPtr kv, string key, double defaultValue, IntPtr hasValue);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool mmkvSetString(IntPtr kv, string key, string value);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr mmkvGetString(IntPtr kv, string key, string defaultValue, [MarshalAs(UnmanagedType.I1)] out bool hasValue);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr mmkvGetString(IntPtr kv, string key, string defaultValue, IntPtr hasValue);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool mmkvSetBytes(IntPtr kv, string key, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] byte[] data, UIntPtr length);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr mmkvAccessBytes(IntPtr kv, string key, [MarshalAs(UnmanagedType.I1)] out bool hasValue, MmkvBytesAccessor accessor);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr mmkvAccessBytes(IntPtr kv, string key, IntPtr hasValue, MmkvBytesAccessor accessor);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool mmkvSetSringArray(IntPtr kv, string key, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPUTF8Str, SizeParamIndex = 3)] string[] data, UIntPtr length);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr mmkvAccessSringArray(IntPtr kv, string key, [MarshalAs(UnmanagedType.I1)] out bool hasValue, MmkvStringArrayAccessorU8 accessor);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr mmkvAccessSringArray(IntPtr kv, string key, IntPtr hasValue, MmkvStringArrayAccessorU8 accessor);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool mmkvContainsKey(IntPtr kv, string key);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern UIntPtr mmkvCount(IntPtr kv);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern UIntPtr mmkvTotalSize(IntPtr kv);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern UIntPtr mmkvActualSize(IntPtr kv);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern void mmkvRemoveValueForKey(IntPtr kv, string key);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern bool mmkvRemoveValuesForKeys(IntPtr kv, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] string[] keys, UIntPtr length);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern void mmkvClearAll(IntPtr kv);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern void mmkvSync(IntPtr kv, MmkvSyncFlag flag);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern void mmkvIsFileValid(MmkvMode mode, string mmapID, string rootPath);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern void mmkvLock(IntPtr kv);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern void mmkvUnlock(IntPtr kv);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern void mmkvClearMemoryCache(IntPtr kv);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool mmkvTryLock(IntPtr kv);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr mmkvVersion();

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern void mmkvTrim(IntPtr kv);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern void mmkvClose(IntPtr kv);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern void mmkvSetLogLevel(MmkvLogLevel level);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern void mmkvSetLogHandlerU8(MmkvLogHandlerU8 kv);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern void mmkvSetErrorHandlerU8(MmkvErrorHandlerU8 kv);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern void mmkvSetContentChangedHandlerU8(MmkvContentChangedHandlerU8 kv);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool mmkvBackupOneToDirectory(string mmapID, string dstPath, string srcPath);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern UIntPtr mmkvBackupAllToDirectory(string dstPath, string srcPath);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool mmkvRestoreOneFromDirectory(string mmapID, string srcPath, string dstPath);

        [DllImport("cmmkv", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern UIntPtr mmkvRestoreAllFromDirectory(string srcPath, string dstPath);
    }
}
