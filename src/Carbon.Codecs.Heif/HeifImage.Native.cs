﻿// Copyright (c) Carbon and contributors.
// Licensed under the MIT License.

using System;
using System.Runtime.InteropServices;

namespace Carbon.Codecs.Heif
{
    /// <content />
    public sealed partial class HeifImage
    {
        private readonly NativeHeifImage nativeInstance;

        private static class NativeMethods
        {
            [DllImport(NativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
            public static extern int HeifImage_Height(IntPtr instance);

            [DllImport(NativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
            public static extern int HeifImage_Width(IntPtr instance);

            [DllImport(NativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr HeifImage_Create(IntPtr context);

            [DllImport(NativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
            public static extern void HeifImage_Dispose(IntPtr instance);

            [DllImport(NativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr HeifImage_GetPlane(IntPtr instance, UIntPtr channel, out UIntPtr stride);
        }

        private sealed class NativeHeifImage : NativeInstance
        {
            public NativeHeifImage(HeifImageHandle handle)
            {
                var instance = NativeMethods.HeifImage_Create(HeifImageHandle.GetInstance(handle));
                if (instance == IntPtr.Zero)
                {
                    throw new HeifException("Unable to decode heif image.");
                }

                this.Instance = instance;
            }

            public int Height => NativeMethods.HeifImage_Height(this.Instance);

            public int Width => NativeMethods.HeifImage_Width(this.Instance);

            protected override string TypeName => nameof(HeifImage);

            public IntPtr GetPlane(HeifChannel channel, out int stride)
            {
                var data = NativeMethods.HeifImage_GetPlane(this.Instance, (UIntPtr)channel, out UIntPtr stridePtr);

                stride = (int)stridePtr.ToUInt32();
                return data;
            }

            protected override void Dispose(IntPtr instance)
            {
                if (instance != IntPtr.Zero)
                {
                    NativeMethods.HeifImage_Dispose(instance);
                }
            }
        }
    }
}
