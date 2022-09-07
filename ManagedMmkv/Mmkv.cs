using Alampy.ManagedMmkv.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Alampy.ManagedMmkv
{
    public class Mmkv : IDisposable
    {
        public delegate void LogHandler(MmkvLogLevel level, string file, int line, string function, string message);
        public delegate void ErrorHandler(string mmapID, MmkvErrorType errorType, ref MmkvRecoverStrategic recoverStrategic);
        public delegate void ContentChangedHandler(string mmapID);

        public static event LogHandler OnLog;
        public static event ErrorHandler OnError;
        public static event ContentChangedHandler OnContentChanged;

        private static readonly NativeMethods.MmkvLogHandlerU8 MyNativeLogHandler = HandleLogFromNative;
        private static void HandleLogFromNative(MmkvLogLevel level, string file, int line, string function, string message)
        {
            OnLog?.Invoke(level, file, line, function, message);
        }
        private static readonly NativeMethods.MmkvErrorHandlerU8 MyNativeErrorHandler = HandleErrorFromNative;
        private static void HandleErrorFromNative(string mmapID, MmkvErrorType errorType, ref MmkvRecoverStrategic recoverStrategic)
        {
            OnError?.Invoke(mmapID, errorType, ref recoverStrategic);
        }
        private static readonly NativeMethods.MmkvContentChangedHandlerU8 MyNativeContentChangedHandler = HandleContentChangedFromNative;
        private static void HandleContentChangedFromNative(string mmapID)
        {
            OnContentChanged?.Invoke(mmapID);
        }

        private static readonly object _init_lock = new object();
        private static volatile bool init = false;
        public static MmkvLogLevel LogLevel
        {
            set
            {
                EnsureMmkvInited(value);
                NativeMethods.mmkvSetLogLevel(value);
            }
        }
        public static void Init(string rootPath, MmkvLogLevel logLevel = MmkvLogLevel.Info)
        {
            lock (_init_lock)
            {
                NativeMethods.mmkvInit(rootPath, logLevel);
                NativeMethods.mmkvSetLogHandlerU8(MyNativeLogHandler);
                NativeMethods.mmkvSetErrorHandlerU8(MyNativeErrorHandler);
                NativeMethods.mmkvSetContentChangedHandlerU8(MyNativeContentChangedHandler);
                init = true;
            }
        }
        public static void Init(MmkvLogLevel logLevel = MmkvLogLevel.Info)
        {
            Init(Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "mmkv"), logLevel);
        }
        public static void EnsureMmkvInited(MmkvLogLevel initialLevel = MmkvLogLevel.Info)
        {
            if (init) // Check without lock first
            {
                return;
            }
            lock (_init_lock)
            {
                if (!init)
                {
                    Init(initialLevel);
                }
            }
        }
        static Mmkv()
        {
            OnLog += (level, file, line, function, message) =>
            {
                Debug.WriteLine($"[{level}] <{file}:{line}::{function}> {message}", "mmkv");
            };
        }

        public static string VersionName => NativeUtils.FinalizeStringBox(NativeMethods.mmkvVersion());

        public static Mmkv WithID(string mmapID, MmkvMode mode, string cryptKey = null, string rootPath = null)
        {
            EnsureMmkvInited();
            var kv = NativeMethods.mmkvWithID(mmapID, mode, cryptKey, rootPath);
            if (kv == IntPtr.Zero)
            {
                throw new Exception("Failed to create mmkv with id");
            }
            return new Mmkv(kv);
        }

        public static Mmkv Default(MmkvMode mode, string cryptKey = null)
        {
            EnsureMmkvInited();
            var kv = NativeMethods.mmkvDefault(mode, cryptKey);
            if (kv == IntPtr.Zero)
            {
                throw new Exception("Failed to create default mmkv");
            }
            return new Mmkv(kv);
        }

        public static bool BackupOneToDirectory(string mmapID, string dstPath, string srcPath = null)
        {
            EnsureMmkvInited();
            return NativeMethods.mmkvBackupOneToDirectory(mmapID, dstPath, srcPath);
        }

        public static UIntPtr BackupAllToDirectory(string dstPath, string srcPath = null)
        {
            EnsureMmkvInited();
            return NativeMethods.mmkvBackupAllToDirectory(dstPath, srcPath);
        }

        public static bool RestoreOneFromDirectory(string mmapID, string srcPath, string dstPath = null)
        {
            EnsureMmkvInited();
            return NativeMethods.mmkvRestoreOneFromDirectory(mmapID, srcPath, dstPath);
        }

        public static UIntPtr RestoreAllFromDirectory(string srcPath, string dstPath = null)
        {
            EnsureMmkvInited();
            return NativeMethods.mmkvRestoreAllFromDirectory(srcPath, dstPath);
        }

        private IntPtr kv;

        private Mmkv(IntPtr kv)
        {
            this.kv = kv;
        }
        #region Set
        /// <exception cref="InvalidOperationException"></exception>
        public void Set(string key, bool value)
        {
            var result = NativeMethods.mmkvSetBool(kv, key, value);
            if (!result)
            {
                throw new InvalidOperationException($"Failed to set {key} to {value}");
            }
        }
        /// <exception cref="InvalidOperationException"></exception>
        public void Set(string key, int value)
        {
            var result = NativeMethods.mmkvSetInt32(kv, key, value);
            if (!result)
            {
                throw new InvalidOperationException($"Failed to set {key} to {value}");
            }
        }
        /// <exception cref="InvalidOperationException"></exception>
        public void Set(string key, long value)
        {
            var result = NativeMethods.mmkvSetInt64(kv, key, value);
            if (!result)
            {
                throw new InvalidOperationException($"Failed to set {key} to {value}");
            }
        }
        /// <exception cref="InvalidOperationException"></exception>
        public void Set(string key, uint value)
        {
            var result = NativeMethods.mmkvSetUInt32(kv, key, value);
            if (!result)
            {
                throw new InvalidOperationException($"Failed to set {key} to {value}");
            }
        }
        /// <exception cref="InvalidOperationException"></exception>
        public void Set(string key, ulong value)
        {
            var result = NativeMethods.mmkvSetUInt64(kv, key, value);
            if (!result)
            {
                throw new InvalidOperationException($"Failed to set {key} to {value}");
            }
        }
        /// <exception cref="InvalidOperationException"></exception>
        public void Set(string key, float value)
        {
            var result = NativeMethods.mmkvSetFloat(kv, key, value);
            if (!result)
            {
                throw new InvalidOperationException($"Failed to set {key} to {value}");
            }
        }
        /// <exception cref="InvalidOperationException"></exception>
        public void Set(string key, double value)
        {
            var result = NativeMethods.mmkvSetDouble(kv, key, value);
            if (!result)
            {
                throw new InvalidOperationException($"Failed to set {key} to {value}");
            }
        }
        /// <exception cref="InvalidOperationException"></exception>
        public void Set(string key, string value)
        {
            var result = NativeMethods.mmkvSetString(kv, key, value);
            if (!result)
            {
                throw new InvalidOperationException($"Failed to set {key} to {value}");
            }
        }
        /// <exception cref="InvalidOperationException"></exception>
        public void Set(string key, byte[] value)
        {
            var result = NativeMethods.mmkvSetBytes(kv, key, value, checked((UIntPtr)value.LongLength));
            if (!result)
            {
                throw new InvalidOperationException($"Failed to set {key} to {value}");
            }
        }
        /// <exception cref="InvalidOperationException"></exception>
        public void Set(string key, string[] value)
        {
            var result = NativeMethods.mmkvSetSringArray(kv, key, value, checked((UIntPtr)(value?.LongLength ?? 0)));
            if (!result)
            {
                throw new InvalidOperationException($"Failed to set {key} to {value}");
            }
        }
        #endregion

        #region TrySet
        public bool TrySet(string key, bool value) => NativeMethods.mmkvSetBool(kv, key, value);
        public bool TrySet(string key, int value) => NativeMethods.mmkvSetInt32(kv, key, value);
        public bool TrySet(string key, long value) => NativeMethods.mmkvSetInt64(kv, key, value);
        public bool TrySet(string key, uint value) => NativeMethods.mmkvSetUInt32(kv, key, value);
        public bool TrySet(string key, ulong value) => NativeMethods.mmkvSetUInt64(kv, key, value);
        public bool TrySet(string key, float value) => NativeMethods.mmkvSetFloat(kv, key, value);
        public bool TrySet(string key, double value) => NativeMethods.mmkvSetDouble(kv, key, value);
        public bool TrySet(string key, string value) => NativeMethods.mmkvSetString(kv, key, value);
        public bool TrySet(string key, byte[] value) => NativeMethods.mmkvSetBytes(kv, key, value, checked((UIntPtr)value.LongLength));
        public bool TrySet(string key, string[] value) => NativeMethods.mmkvSetSringArray(kv, key, value, checked((UIntPtr)(value?.LongLength ?? 0)));
        #endregion

        #region Get
        /// <exception cref="KeyNotFoundException"></exception>
        public bool GetBoolean(string key)
        {
            var result = NativeMethods.mmkvGetBool(kv, key, default, out var hasValue);
            if (!hasValue)
            {
                throw new Exception($"Failed to get key ${key}");
            }
            return result;
        }
        /// <exception cref="KeyNotFoundException"></exception>
        public int GetInt32(string key)
        {
            var result = NativeMethods.mmkvGetInt32(kv, key, default, out var hasValue);
            if (!hasValue)
            {
                throw new Exception($"Failed to get key ${key}");
            }
            return result;
        }
        /// <exception cref="KeyNotFoundException"></exception>
        public long GetInt64(string key)
        {
            var result = NativeMethods.mmkvGetInt64(kv, key, default, out var hasValue);
            if (!hasValue)
            {
                throw new Exception($"Failed to get key ${key}");
            }
            return result;
        }
        /// <exception cref="KeyNotFoundException"></exception>
        public uint GetUInt32(string key)
        {
            var result = NativeMethods.mmkvGetUInt32(kv, key, default, out var hasValue);
            if (!hasValue)
            {
                throw new Exception($"Failed to get key ${key}");
            }
            return result;
        }
        /// <exception cref="KeyNotFoundException"></exception>
        public ulong GetUInt64(string key)
        {
            var result = NativeMethods.mmkvGetUInt64(kv, key, default, out var hasValue);
            if (!hasValue)
            {
                throw new Exception($"Failed to get key ${key}");
            }
            return result;
        }
        /// <exception cref="KeyNotFoundException"></exception>
        public float GetFloat(string key)
        {
            var result = NativeMethods.mmkvGetFloat(kv, key, default, out var hasValue);
            if (!hasValue)
            {
                throw new Exception($"Failed to get key ${key}");
            }
            return result;
        }
        /// <exception cref="KeyNotFoundException"></exception>
        public double GetDouble(string key)
        {
            var result = NativeMethods.mmkvGetDouble(kv, key, default, out var hasValue);
            if (!hasValue)
            {
                throw new Exception($"Failed to get key ${key}");
            }
            return result;
        }
        /// <exception cref="KeyNotFoundException"></exception>
        public string GetString(string key)
        {
            var result = NativeMethods.mmkvGetString(kv, key, default, out var hasValue);
            if (!hasValue)
            {
                throw new Exception($"Failed to get key ${key}");
            }
            return NativeUtils.FinalizeStringBox(result);
        }
        /// <exception cref="KeyNotFoundException"></exception>
        public byte[] GetBytes(string key)
        {
            var result = NativeUtils.mmkvGetBytes(kv, key, default, out var hasValue);
            if (!hasValue)
            {
                throw new Exception($"Failed to get key ${key}");
            }
            return result;
        }
        /// <exception cref="KeyNotFoundException"></exception>
        public string[] GetStringArray(string key)
        {
            var result = NativeUtils.mmkvGetStringArray(kv, key, default, out var hasValue);
            if (!hasValue)
            {
                throw new Exception($"Failed to get key ${key}");
            }
            return result;
        }
        #endregion

        #region GetOrDefault
        public bool GetBooleanOrDefault(string key, bool defaultValue = default) => NativeMethods.mmkvGetBool(kv, key, defaultValue, IntPtr.Zero);
        public int GetInt32OrDefault(string key, int defaultValue = default) => NativeMethods.mmkvGetInt32(kv, key, defaultValue, IntPtr.Zero);
        public long GetInt64OrDefault(string key, long defaultValue = default) => NativeMethods.mmkvGetInt64(kv, key, defaultValue, IntPtr.Zero);
        public uint GetUInt32OrDefault(string key, uint defaultValue = default) => NativeMethods.mmkvGetUInt32(kv, key, defaultValue, IntPtr.Zero);
        public ulong GetUInt64OrDefault(string key, ulong defaultValue = default) => NativeMethods.mmkvGetUInt64(kv, key, defaultValue, IntPtr.Zero);
        public float GetFloatOrDefault(string key, float defaultValue = default) => NativeMethods.mmkvGetFloat(kv, key, defaultValue, IntPtr.Zero);
        public double GetDoubleOrDefault(string key, double defaultValue = default) => NativeMethods.mmkvGetDouble(kv, key, defaultValue, IntPtr.Zero);
        public string GetStringOrDefault(string key, string defaultValue = default)
        {
            var result = NativeMethods.mmkvGetString(kv, key, null, out var hasValue);
            if (!hasValue)
            {
                return defaultValue;
            }
            return NativeUtils.FinalizeStringBox(result);
        }
        public byte[] GetBytesOrDefault(string key, byte[] defaultValue = default) => NativeUtils.mmkvGetBytes(kv, key, defaultValue, out _);
        public string[] GetStringArrayOrDefault(string key, string[] defaultValue = default) => NativeUtils.mmkvGetStringArray(kv, key, defaultValue, out _);
        #endregion

        #region TryGet
        public bool TryGetBoolean(string key, out bool value)
        {
            value = NativeMethods.mmkvGetBool(kv, key, default, out var hasValue);
            return hasValue;
        }
        public bool TryGetInt32(string key, out int value)
        {
            value = NativeMethods.mmkvGetInt32(kv, key, default, out var hasValue);
            return hasValue;
        }
        public bool TryGetInt64(string key, out long value)
        {
            value = NativeMethods.mmkvGetInt64(kv, key, default, out var hasValue);
            return hasValue;
        }
        public bool TryGetUInt32(string key, out uint value)
        {
            value = NativeMethods.mmkvGetUInt32(kv, key, default, out var hasValue);
            return hasValue;
        }
        public bool TryGetUInt64(string key, out ulong value)
        {
            value = NativeMethods.mmkvGetUInt64(kv, key, default, out var hasValue);
            return hasValue;
        }
        public bool TryGetFloat(string key, out float value)
        {
            value = NativeMethods.mmkvGetFloat(kv, key, default, out var hasValue);
            return hasValue;
        }
        public bool TryGetDouble(string key, out double value)
        {
            value = NativeMethods.mmkvGetDouble(kv, key, default, out var hasValue);
            return hasValue;
        }
        public bool TryGetString(string key, out string value)
        {
            value = NativeUtils.FinalizeStringBox(NativeMethods.mmkvGetString(kv, key, default, out var hasValue));
            return hasValue;
        }
        public bool TryGetBytes(string key, out byte[] value)
        {
            value = NativeUtils.mmkvGetBytes(kv, key, default, out var hasValue);
            return hasValue;
        }
        public bool TryGetStringArray(string key, out string[] value)
        {
            value = NativeUtils.mmkvGetStringArray(kv, key, default, out var hasValue);
            return hasValue;
        }
        #endregion

        public bool ContainsKey(string key)
        {
            return NativeMethods.mmkvContainsKey(kv, key);
        }

        public string MmapID => NativeUtils.FinalizeStringBox(NativeMethods.mmkvMmapID(kv));

        /// <exception cref="OverflowException"/>
        public int Count => checked((int)NativeCount);
        public long LongCount => checked((long)NativeCount);
        public UIntPtr NativeCount => NativeMethods.mmkvCount(kv);

        public UIntPtr TotalSize => NativeMethods.mmkvTotalSize(kv);
        public UIntPtr ActualSize => NativeMethods.mmkvActualSize(kv);


        public void Remove(string key)
        {
            NativeMethods.mmkvRemoveValueForKey(kv, key);
        }

        public void RemoveAll(string[] key)
        {
            if ((key?.LongLength ?? 0) == 0) return;
            NativeMethods.mmkvRemoveValuesForKeys(kv, key, (UIntPtr)key.LongLength);
        }

        public void RemoveAll(IEnumerable<string> keys)
        {
            RemoveAll(keys?.ToArray());
        }

        public void TrimExcess()
        {
            NativeMethods.mmkvTrim(kv);
        }

        public void Clear()
        {
            NativeMethods.mmkvClearAll(kv);
        }

        public void Flush(MmkvSyncFlag flag)
        {
            NativeMethods.mmkvSync(kv, flag);
        }

        public void ClearMemoryCache()
        {
            NativeMethods.mmkvClearMemoryCache(kv);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (kv != IntPtr.Zero)
            {
                if (disposing)
                {
                    // Do nothing
                }
                NativeMethods.mmkvClose(kv);
                kv = IntPtr.Zero;
            }
        }

        ~Mmkv()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
