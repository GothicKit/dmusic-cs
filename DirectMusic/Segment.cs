using System;

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

		public void Download(Loader loader)
		{
			Native.DmSegment_download(Handle, loader.Handle).Check();
		}
	}
}