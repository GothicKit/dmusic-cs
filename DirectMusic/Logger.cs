using System;
using System.Runtime.InteropServices;
using DirectMusic.Util;

namespace DirectMusic
{
	public enum LogLevel
	{
		/// <summary>
		/// The log message indicates a fatal error.
		/// </summary>
		Fatal = 10,
		
		/// <summary>
		/// The log message indicates an error.
		/// </summary>
		Error = 20,
		
		/// <summary>
		/// The log message indicates a warning.
		/// </summary>
		Warning = 30,
		
		/// <summary>
		/// The log message is informational.
		/// </summary>
		Info = 40,
		
		/// <summary>
		/// The log message is a debug message.
		/// </summary>
		Debug = 50,
		
		/// <summary>
		/// The log message is a tracing message.
		/// </summary>
		Trace = 60
	}

	public static class Logger
	{
		public delegate void Callback(LogLevel level, string message);

		private static GCHandle? _handler;
		private static readonly Native.DmLogHandler NativeHandler = _nativeCallbackHandler;

		/// <summary>
		/// <b>Set a callback to send log messages to.</b> Registers the given <paramref name="callback"/> function to
		/// be called whenever a log message is issued by the library. If <paramref name="callback"/> is set to <c>null</c>,
		/// any existing log callback function is removed and logging is disabled.
		/// </summary>
		/// <param name="lvl">The log level to set for the library.</param>
		/// <param name="callback">The callback function to invoke whenever log message is generated or <c>null</c></param>
		/// <seealso cref="SetDefault"/>
		/// <seealso cref="SetLevel"/>
		public static void Set(LogLevel lvl, Callback? callback)
		{
			var handler = GCHandle.Alloc(callback);
			Native.Dm_setLogger(lvl, NativeHandler, GCHandle.ToIntPtr(handler));

			_handler?.Free();
			_handler = handler;
		}

		/// <summary>
		/// <b>Set a default logging function.</b> Registers a default log handler which outputs all log messages at
		/// or above the given level to the standard error stream (<c>stderr</c>).
		/// </summary>
		/// <param name="level">The log level to set for the library.</param>
		/// <seealso cref="Set"/>
		/// <seealso cref="SetLevel"/>
		public static void SetDefault(LogLevel level)
		{
			Native.Dm_setLoggerDefault(level);
		}

		/// <summary>
		/// <b>Set the log level of the library.</b>
		/// </summary>
		/// <param name="level">The log level to set.</param>
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