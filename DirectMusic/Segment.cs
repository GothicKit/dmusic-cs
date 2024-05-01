using System;
using DirectMusic.Util;

namespace DirectMusic
{
	public class Segment
	{
		internal readonly IntPtr Handle;

		internal Segment(IntPtr handle)
		{
			Handle = handle;
		}

		~Segment()
		{
			Native.DmSegment_release(Handle);
		}

		/// <summary>
		/// <b>Download all resources needed by the segment.</b> In order to play a segment, its internal resources,
		/// like references to styles and bands need to be resolved and downloaded. This is done by either calling
		/// <see cref="Download"/> manually or by setting the <see cref="LoaderOptions.Download"/> flag when creating
		/// the loader.
		/// </summary>
		/// <param name="loader">The loader to use for downloading resources.</param>
		/// <exception cref="NativeAccessError">If the operation fails in any way.</exception>
		public void Download(Loader loader)
		{
			Native.DmSegment_download(Handle, loader.Handle).Check();
		}
	}
}