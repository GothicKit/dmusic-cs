using System;
using System.Runtime.InteropServices;
using DirectMusic.Util;

namespace DirectMusic
{
	public enum LogLevel
	{
		Fatal = 10,
		Error = 20,
		Warning = 30,
		Info = 40,
		Debug = 50,
		Trace = 60
	}

	public static class Logger
	{
		public delegate void Callback(LogLevel level, string message);

		private static GCHandle? _handler;
		private static readonly Native.DmLogHandler NativeHandler = _nativeCallbackHandler;

		public static void Set(LogLevel lvl, Callback callback)
		{
			var handler = GCHandle.Alloc(callback);
			Native.Dm_setLogger(lvl, NativeHandler, GCHandle.ToIntPtr(handler));

			_handler?.Free();
			_handler = handler;
		}

		public static void SetDefault(LogLevel level)
		{
			Native.Dm_setLoggerDefault(level);
		}

		public static void SetLevel(LogLevel level)
		{
			Native.Dm_setLoggerLevel(level);
		}

		[MonoPInvokeCallback]
		private static void _nativeCallbackHandler(IntPtr ctx, LogLevel level, string message)
		{
			var gcHandle = GCHandle.FromIntPtr(ctx);
			var cb = (Callback)gcHandle.Target;
			cb(level, message);
		}
	}
}