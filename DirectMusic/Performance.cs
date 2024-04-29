using System;

namespace DirectMusic
{
	[Flags]
	public enum PlaybackFlags
	{
		Secondary = 1 << 7,
		Queue = 1 << 8,
		Control = 1 << 9,
		AfterPrepareTime = 1 << 10,
		Grid = 1 << 11,
		Beat = 1 << 12,
		Measure = 1 << 13,
		Default = 1 << 14,
		NoInvalidate = 1 << 15,
		Align = 1 << 16,
		ValidStartBeat = 1 << 17,
		ValidStartGrid = 1 << 18,
		ValidStartTick = 1 << 19,
		AutoTransition = 1 << 20,
		AfterQueueTime = 1 << 21,
		AfterLatencyTime = 1 << 22,
		SegmentEnd = 1 << 23,
		Marker = 1 << 24,
		TimeSigAlways = 1 << 25,
		UseAudiopath = 1 << 26,
		ValidStartMeasure = 1 << 27,
		InvalidatePri = 1 << 28
	}

	public enum Embellishment
	{
		None = 0,
		Fill = 1,
		Intro = 2,
		Break = 3,
		End = 4
	}

	public class Performance
	{
		private readonly IntPtr _handle;

		public Performance(IntPtr handle)
		{
			_handle = handle;
		}

		~Performance()
		{
			Native.DmPerformance_release(_handle);
		}

		public void PlaySegment(Segment segment, PlaybackFlags flags)
		{
			Native.DmPerformance_playSegment(_handle, segment.Handle, flags);
		}

		public void PlayTransition(Segment segment, Embellishment embellishment, PlaybackFlags flags)
		{
			Native.DmPerformance_playTransition(_handle, segment.Handle, embellishment, flags);
		}

		public void RenderPcm(short[] pcm, bool stereo)
		{
			var opts = DmRenderOptions.Short;
			if (stereo) opts |= DmRenderOptions.Stereo;

			Native.DmPerformance_renderPcm(_handle, pcm, (ulong)pcm.Length, opts);
		}

		public void RenderPcm(float[] pcm, bool stereo)
		{
			var opts = DmRenderOptions.Float;
			if (stereo) opts |= DmRenderOptions.Stereo;

			Native.DmPerformance_renderPcm(_handle, pcm, (ulong)pcm.Length, opts);
		}

		public static Performance Create()
		{
			Native.DmPerformance_create(out var handle).Check();
			return new Performance(handle);
		}
	}
}