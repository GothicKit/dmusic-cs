using System;
using DirectMusic.Util;

namespace DirectMusic
{
	public enum Timing
	{
		/// <summary>
		/// Timing flag indicating start at the next possible tick.
		/// </summary>
		Instant = 1,

		/// <summary>
		/// Timing flag indicating start at the next possible grid boundary.
		/// </summary>
		Grid = 2,

		/// <summary>
		/// Timing flag indicating start at the next possible beat boundary.
		/// </summary>
		Beat = 3,

		/// <summary>
		/// Timing flag indicating start at the next possible measure boundary.
		/// </summary>
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

		/// <summary>
		/// <b>Schedule a new segment to be played by the performance.</b> The segment is played at the next
		/// timing boundary provided with <paramref name="timing"/>. This function simply stops the currently playing
		/// segment and starts playing the next one. To play a transition between the two segments, use
		/// <see cref="PlayTransition"/>.
		/// </summary>
		/// <remarks>
		/// The segment will always start playing strictly after the last call to <see cref="RenderPcm(short[],bool)"/>
		/// since that function advances the internal clock. This means, if you have already rendered ten seconds
		/// worth of PCM using <see cref="RenderPcm(short[],bool)"/>, the transition can only audibly be heard after
		/// these ten seconds of PCM have been played.
		/// </remarks>
		/// <param name="segment">The segment to play or <c>null</c> to simply stop the playing segment.</param>
		/// <param name="timing">The timing bounding to start playing the segment at.</param>
		/// <exception cref="NativeAccessError">If the operation fails in any way.</exception>
		public void PlaySegment(Segment? segment, Timing timing)
		{
			Native.DmPerformance_playSegment(_handle, segment?.Handle ?? IntPtr.Zero, timing).Check();
		}

		/// <summary>
		/// <b>Schedule a new segment to play by the performance with a transition.</b> Schedules a new
		/// transitional segment to be played, which first plays a transitional pattern from the currently playing
		/// segment's style and then starts playing the given segment. This can be used to smoothly transition from
		/// one segment to another. The transitional pattern is selected by its <paramref name="embellishment"/> type
		/// provided when calling the function. Only embellishments matching the current groove level are considered.
		/// </summary>
		/// <param name="segment">The segment to transition to or <c>null</c> to transition to silence.</param>
		/// <param name="embellishment">The embellishment type to use for the transition.</param>
		/// <param name="timing">The timing bounding to start playing the transition at.</param>
		public void PlayTransition(Segment? segment, Embellishment embellishment, Timing timing)
		{
			Native.DmPerformance_playTransition(_handle, segment?.Handle ?? IntPtr.Zero, embellishment, timing).Check();
		}

		/// <summary>
		/// <b>Set the playback volume of a performance</b>
		/// </summary>
		/// <param name="volume">The new volume to set (between 0 and 1).</param>
		public void SetVolume(float volume)
		{
			Native.DmPerformance_setVolume(_handle, volume);
		}

		/// <summary>
		/// <b>Render short PCM samples from the performance.</b>
		/// Since the performance is played "on demand", calling this function will advance the internal clock and
		/// perform all musical operation for the rendered timeframe. If no segment is currently playing, the output
		/// will be set to zero samples.
		/// </summary>
		/// <param name="pcm">A short array to output the PCM into.</param>
		/// <param name="stereo">Set to <c>true</c> to render stereo samples. Renders mono samples otherwise.</param>
		/// <seealso cref="RenderPcm(float[],bool)"/>
		public void RenderPcm(short[] pcm, bool stereo)
		{
			var opts = DmRenderOptions.Short;
			if (stereo) opts |= DmRenderOptions.Stereo;

			Native.DmPerformance_renderPcm(_handle, pcm, (ulong)pcm.Length, opts).Check();
		}

		/// <summary>
		/// <b>Render float PCM samples from the performance.</b>
		/// Since the performance is played "on demand", calling this function will advance the internal clock and
		/// perform all musical operation for the rendered timeframe. If no segment is currently playing, the output
		/// will be set to zero samples.
		/// </summary>
		/// <param name="pcm">A float array to output the PCM into.</param>
		/// <param name="stereo">Set to <c>true</c> to render stereo samples. Renders mono samples otherwise.</param>
		/// <seealso cref="RenderPcm(short[],bool)"/>
		public void RenderPcm(float[] pcm, bool stereo)
		{
			var opts = DmRenderOptions.Float;
			if (stereo) opts |= DmRenderOptions.Stereo;

			Native.DmPerformance_renderPcm(_handle, pcm, (ulong)pcm.Length, opts).Check();
		}

		/// <summary>
		/// <b>Create a new DirectMusic Performance object.</b>
		/// </summary>
		/// <param name="rate">The sample rate for the synthesizer. Provide 0 to use the default (44100 Hz).</param>
		/// <returns>The newly created performance</returns>
		/// <exception cref="NativeAccessError">If the operation fails in any way.</exception>
		public static Performance Create(int rate)
		{
			Native.DmPerformance_create(out var handle, (uint)rate).Check();
			return new Performance(handle);
		}
	}
}