using System;
using System.Runtime.InteropServices;
using DirectMusic.Util;

namespace DirectMusic
{
	internal enum DmResult
	{
		Success = 0,
		InvalidArgument = 1,
		InvalidState = 2,
		MemoryExhausted = 3,
		NotFound = 4,
		FileCorrupt = 5,
		InternalError = 6
	}

	[Flags]
	internal enum DmRenderOptions
	{
		Short = 1 << 0,
		Float = 1 << 1,
		Stereo = 1 << 2
	}

	internal static class DmResultExtension
	{
		public static void Check(this DmResult result)
		{
			if (result == DmResult.Success) return;
			throw new NativeAccessError(result);
		}
	}

	internal static class Native
	{
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IntPtr DmLoaderResolverCallback(IntPtr ctx, string name, out ulong size);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void DmLogHandler(IntPtr ctx, LogLevel lvl, string message);

		private const string DllName = "dmusic";

		[DllImport(DllName)]
		public static extern void Dm_setLogger(LogLevel lvl, DmLogHandler log, IntPtr ctx);

		[DllImport(DllName)]
		public static extern void Dm_setLoggerDefault(LogLevel lvl);

		[DllImport(DllName)]
		public static extern void Dm_setLoggerLevel(LogLevel lvl);

		[DllImport(DllName)]
		public static extern DmResult DmLoader_create(out IntPtr slf, LoaderOptions opt);

		[DllImport(DllName)]
		public static extern IntPtr DmLoader_retain(IntPtr slf);

		[DllImport(DllName)]
		public static extern void DmLoader_release(IntPtr slf);

		[DllImport(DllName)]
		public static extern DmResult DmLoader_addResolver(IntPtr slf, DmLoaderResolverCallback resolve, IntPtr ctx);

		[DllImport(DllName)]
		public static extern DmResult DmLoader_getSegment(IntPtr slf, string name, out IntPtr segment);

		[DllImport(DllName)]
		public static extern IntPtr DmSegment_retain(IntPtr slf);

		[DllImport(DllName)]
		public static extern void DmSegment_release(IntPtr slf);

		[DllImport(DllName)]
		public static extern DmResult DmSegment_download(IntPtr slf, IntPtr loader);

		[DllImport(DllName)]
		public static extern DmResult DmPerformance_create(out IntPtr slf);

		[DllImport(DllName)]
		public static extern IntPtr DmPerformance_retain(IntPtr slf);

		[DllImport(DllName)]
		public static extern void DmPerformance_release(IntPtr slf);

		[DllImport(DllName)]
		public static extern DmResult DmPerformance_playSegment(IntPtr slf, IntPtr sgt, PlaybackFlags flags);

		[DllImport(DllName)]
		public static extern DmResult DmPerformance_playTransition(IntPtr slf, IntPtr sgt, Embellishment embellishment,
			PlaybackFlags flags);

		[DllImport(DllName)]
		public static extern DmResult DmPerformance_renderPcm(IntPtr slf, short[] buf, ulong len, DmRenderOptions opts);

		[DllImport(DllName)]
		public static extern DmResult DmPerformance_renderPcm(IntPtr slf, float[] buf, ulong len, DmRenderOptions opts);

		public class Structs
		{
		}
	}
}