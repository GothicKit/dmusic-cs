using System;

namespace DirectMusic.Util
{
	public class NativeAccessError : Exception
	{
		internal NativeAccessError() : base("Access to a native function failed")
		{
		}

		internal NativeAccessError(string message) : base("Access to a native function failed: " + message)
		{
		}

		internal NativeAccessError(DmResult message) : base("Access to a native function failed: " + message)
		{
		}

		internal NativeAccessError(string message, Exception innerException) : base(
			"Access to a native property failed: " + message, innerException)
		{
		}

		internal static void ThrowResult(DmResult result)
		{
			if (result == DmResult.Success) return;
			throw new NativeAccessError(result);
		}
	}
}