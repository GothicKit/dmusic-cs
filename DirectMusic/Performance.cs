using System;

namespace DirectMusic
{
	[Flags]
	public enum Timing
	{
		Instant = 1,
		Grid = 2,
		Beat = 3,
		Measure = 4,
	}

	public enum Embellishment
	{
		None = 0,
		Groove = 1,
		Fill = 2,
		Intro = 3,
		Break = 4,
		End = 5,
		EndAndIntro = 6,
	}

	public class Performance
	{
		private readonly IntPtr _handle;

		private Performance(IntPtr handle)
		{
			_handle = handle;
		}

		~Performance()
		{
			Native.DmPerformance_release(_handle);
		}

		public void PlaySegment(Segment segment, Timing timing)
		{
			Native.DmPerformance_playSegment(_handle, segment.Handle, timing).Check();
		}

		public void PlayTransition(Segment segment, Embellishment embellishment, Timing timing)
		{
			Native.DmPerformance_playTransition(_handle, segment.Handle, embellishment, timing).Check();
		}

		public void StopSegment(Timing timing)
		{
			Native.DmPerformance_playSegment(_handle, IntPtr.Zero, timing).Check();
		}

		public void SetVolume(float volume)
		{
			Native.DmPerformance_setVolume(_handle, volume);
		}

		public void RenderPcm(short[] pcm, bool stereo)
		{
			var opts = DmRenderOptions.Short;
			if (stereo) opts |= DmRenderOptions.Stereo;

			Native.DmPerformance_renderPcm(_handle, pcm, (ulong)pcm.Length, opts).Check();
		}

		public void RenderPcm(float[] pcm, bool stereo)
		{
			var opts = DmRenderOptions.Float;
			if (stereo) opts |= DmRenderOptions.Stereo;

			Native.DmPerformance_renderPcm(_handle, pcm, (ulong)pcm.Length, opts).Check();
		}

		public static Performance Create()
		{
			Native.DmPerformance_create(out var handle).Check();
			return new Performance(handle);
		}
	}
}