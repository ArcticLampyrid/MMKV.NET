using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Alampy.ManagedMmkv.Interop
{
    internal class NativeUtils
    {
        private static IntPtr InternalAccessStringBox(IntPtr ptr, UIntPtr length)
        {
            var str = Marshal.PtrToStringUTF8(ptr, checked((int)length));
            return (IntPtr)GCHandle.Alloc(str);
        }
        private static readonly NativeMethods.MmkvStringBoxAccessorU8 stringBoxAccessor = new NativeMethods.MmkvStringBoxAccessorU8(InternalAccessStringBox);

        public static string FinalizeStringBox(IntPtr str)
        {
            var handlePtr = NativeMethods.mmkvStringBoxAccessU8(str, stringBoxAccessor);
            NativeMethods.mmkvStringBoxDelete(str);
            if (handlePtr == IntPtr.Zero)
            {
                return null;
            }
            var handle = GCHandle.FromIntPtr(handlePtr);
            try
            {
                return (string)handle.Target;
            }
            finally
            {
                handle.Free();
            }
        }

        private static IntPtr InternalAccessBytes(IntPtr ptr, UIntPtr length)
        {
            var intLength = checked((int)length);
            var data = new byte[intLength];
            Marshal.Copy(ptr, data, 0, intLength);
            return (IntPtr)GCHandle.Alloc(data);
        }
        private static readonly NativeMethods.MmkvBytesAccessor bytesAccessor = new NativeMethods.MmkvBytesAccessor(InternalAccessBytes);

#pragma warning disable IDE1006 // 命名样式
        public static byte[] mmkvGetBytes(IntPtr kv, string key, byte[] defaultValue, out bool hasValue)
#pragma warning restore IDE1006 // 命名样式
        {
            var handlePtr = NativeMethods.mmkvAccessBytes(kv, key, out hasValue, bytesAccessor);
            if (handlePtr == IntPtr.Zero)
            {
                if (!hasValue)
                {
                    return defaultValue;
                }
                return null;
            }
            var handle = GCHandle.FromIntPtr(handlePtr);
            try
            {
                return (byte[])handle.Target;
            }
            finally
            {
                handle.Free();
            }
        }

        private static IntPtr InternalAccessStringArray(IntPtr[] ptrs, UIntPtr length)
        {
            var intLength = checked((int)length);
            var data = new string[intLength];
            for (var i = 0; i < intLength; i++)
            {
                data[i] = Marshal.PtrToStringUTF8(ptrs[i]);
            }
            return (IntPtr)GCHandle.Alloc(data);
        }
        private static readonly NativeMethods.MmkvStringArrayAccessorU8 stringArrayAccessor = new NativeMethods.MmkvStringArrayAccessorU8(InternalAccessStringArray);

#pragma warning disable IDE1006 // 命名样式
        public static string[] mmkvGetStringArray(IntPtr kv, string key, string[] defaultValue, out bool hasValue)
#pragma warning restore IDE1006 // 命名样式
        {
            var handlePtr = NativeMethods.mmkvAccessSringArray(kv, key, out hasValue, stringArrayAccessor);
            if (handlePtr == IntPtr.Zero)
            {
                if (!hasValue)
                {
                    return defaultValue;
                }
                return null;
            }
            var handle = GCHandle.FromIntPtr(handlePtr);
            try
            {
                return (string[])handle.Target;
            }
            finally
            {
                handle.Free();
            }
        }
    }
}
