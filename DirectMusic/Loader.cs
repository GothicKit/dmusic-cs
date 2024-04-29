using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DirectMusic.Util;

namespace DirectMusic
{
	[Flags]
	public enum LoaderOptions
	{
		Download = 1 << 0,
		Default = 0
	}

	public class Loader
	{
		public delegate byte[]? ResolveCallback(string name);

		private readonly List<GCHandle> _callbacks = new List<GCHandle>();
		internal readonly IntPtr Handle;

		public Loader(IntPtr handle)
		{
			Handle = handle;
		}

		~Loader()
		{
			_callbacks.ForEach(v => v.Free());
			Native.DmLoader_release(Handle);
		}

		public void AddResolver(ResolveCallback resolve)
		{
			var cb = GCHandle.Alloc(resolve);
			Native.DmLoader_addResolver(Handle, _nativeCallbackHandler, GCHandle.ToIntPtr(cb)).Check();
			_callbacks.Add(cb);
		}

		public Segment GetSegment(string name)
		{
			Native.DmLoader_getSegment(Handle, name, out var segment).Check();
			return new Segment(segment);
		}

		public static Loader Create(LoaderOptions opt)
		{
			Native.DmLoader_create(out var handle, opt).Check();
			return new Loader(handle);
		}

		[MonoPInvokeCallback]
		private static IntPtr _nativeCallbackHandler(IntPtr ctx, string name, out ulong size)
		{
			var gcHandle = GCHandle.FromIntPtr(ctx);
			var cb = (ResolveCallback)gcHandle.Target;

			var bytes = cb(name);
			if (bytes == null)
			{
				size = 0;
				return IntPtr.Zero;
			}

			var data = Marshal.AllocHGlobal(bytes.Length);
			Marshal.Copy(bytes, 0, data, bytes.Length);

			size = (ulong)bytes.Length;
			return data;
		}
	}
}