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
		/// <summary>
		/// <b>A function used to look up and read in DirectMusic objects by file name.</b> When called, a function
		/// implementing this interface should look up a DirectMusic data file corresponding to the given file
		/// <paramref name="name"/> and return the data contained within as a memory buffer. When the function fails to
		/// find an appropriate file, it should return <c>null</c>.
		/// </summary>
		/// <param name="name">The name of the file to look up.</param>
		/// <returns>A memory buffer containing the file data or <c>null</c> if the lookup failed.</returns>
		public delegate byte[]? ResolveCallback(string name);

		private readonly List<GCHandle> _callbacks = new List<GCHandle>();
		internal readonly IntPtr Handle;

		private Loader(IntPtr handle)
		{
			Handle = handle;
		}

		~Loader()
		{
			_callbacks.ForEach(v => v.Free());
			Native.DmLoader_release(Handle);
		}

		/// <summary>
		/// <b>Add a resolver to the loader.</b> Resolvers are used to locate stored DirectMusic object by file name.
		/// Whenever the loader needs to look up an object, it calls all resolvers in sequential order until one
		/// returns a match. If no match is found, an error is issued and the object is not loaded.
		/// </summary>
		/// <param name="resolve">The callback function used to resolve a file using the new resolver.</param>
		/// <exception cref="NativeAccessError">If the operation fails in any way.</exception>
		public void AddResolver(ResolveCallback resolve)
		{
			var cb = GCHandle.Alloc(resolve);
			Native.DmLoader_addResolver(Handle, _nativeCallbackHandler, GCHandle.ToIntPtr(cb)).Check();
			_callbacks.Add(cb);
		}

		/// <summary>
		/// <b>Get a segment from the loader's cache or load it by file <paramref name="name"/></b> Gets a segment from
		/// the loader's cache or loads the segment using the resolvers added to the loader. If the requested segment
		/// is found in neither the loader, nor by any resolver, an <exception cref="NativeAccessError"></exception>
		/// is thrown.
		/// <p>
		/// If the loader was created using the <see cref="LoaderOptions.Download"/> option, this function
		/// automatically downloads the segment by calling <see cref="Segment.Download"/>.
		/// </p>
		/// </summary>
		/// <param name="name">The file name of the segment to load.</param>
		/// <returns>The loaded segment.</returns>
		/// <exception cref="NativeAccessError">If the operation fails in any way.</exception>
		public Segment GetSegment(string name)
		{
			Native.DmLoader_getSegment(Handle, name, out var segment).Check();
			return new Segment(segment);
		}

		/// <summary>
		/// <b>Create a new DirectMusic Loader object.</b> If the <see cref="LoaderOptions.Download"/> option is
		/// specified, all references for objects retrieved for the loader are automatically resolved and downloaded.
		/// </summary>
		/// <param name="opt">A bitfield containing loader configuration flags.</param>
		/// <returns>The newly created loader.</returns>
		/// <exception cref="NativeAccessError">If the operation fails in any way.</exception>
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