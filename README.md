# DirectMusic for C#

Very experimental wrapper for the also very experimental [dmusic](https://github.com/GothicKit/dmusic) C-library.

## Example

Here's how to use it, very simply. Refer to the C-library's docs for more details 
[dmusic](https://github.com/GothicKit/dmusic).

```csharp
using DirectMusic;

const string root = "/path/to/your/music/folder/";

Logger.SetDefault(LogLevel.Info);

// 1. Create a new loader. The loader is responsible for loading and caching DirectMusic files using a
//    user-defined callback function called a "resolver". You really only ever need one for your application.
var loader = Loader.Create(LoaderOptions.Download);

// 2. Register a resolver with the loader. A resolver is simply a function which gets a filename and returns a
//    memory buffer. You can return null from a resolver to indicate that the file was not found.
loader.AddResolver(name =>
{
	try
	{
		return File.ReadAllBytes(Path.Join(root, name));
	}
	catch (Exception e)
	{
		return null;
	}
});

// 3. Use the loader to obtain a segment. The loader will call your resolvers in order to read in the
//    file, and it will then perform some internal magic to load the segment. Since we set the
//    LoaderOptions.Download option when constructing the loader, we don't need to call Segment.Download
//    afterward. Otherwise, you do have to call it.
var segment = loader.GetSegment("YourSegment.sgt");

// 4. Create a new performance. The performance represents your main playback device. It handles all
//    the DirectMusic magic needed to produce music from your segments. You typically only need one
//    performance for your application. The first parameter here is the sample rate, defaulted to
//    44100 Hz.
var performance = Performance.Create(44100);

// 5. Instruct the performance to play a segment. This will set up the performance's internals so that
//    the following call to Performance.RenderPcm will start producing music. The performance renders
//    music on-demand, so as long as you don't call Performance.RenderPcm, you can consider playback to
//    be paused. To stop playing music, you can pass null as the first parameter.
//
//    The second parameter here is the timing. It tells the performance at which boundary to start playing
//    the new segment as to not interrupt the flow of music. The options are "instant", which ignores all
//    that and immediately plays the segment, "grid" which plays the segment at the next possible beat
//    subdivision, "beat" which plays the segment at the next beat and "measure" which plays it at the next
//    measure boundary.
//
//    The performance also supports transitions. To play those, use Performance.PlayTransition and see
//    its inline documentation for more information.
performance.PlaySegment(segment, Timing.Measure);

// 6. Finally, render some PCM! This will instruct the performance to start processing the underlying
//    DirectMusic messages and render the resulting PCM to the output buffer. In this case it will
//    render 1000000 stereo samples which is 500000 samples per channel.
//
//    This will advance the internal clock for as many ticks as required to render the requested number
//    of samples. No more, no less.
var pcm = new float[1_000_000];
performance.RenderPcm(pcm, true);

// 6.1. Write out the PCM data to some place where we can access it later. This could also just be some
//      audio output device or another library.
var bytes = new byte[pcm.Length * 4];
Buffer.BlockCopy(pcm, 0, bytes, 0, bytes.Length);
File.WriteAllBytes("output.pcm", bytes);
```

## Contact

If you have any questions, or you just want to say hi, you can reach me via e-mail ([`me@lmichaelis.de`](mailto:me@lmichaelis.de))
or on Discord either via DM but preferably in the Gothic VR and GMC Discords (`@lmichaelis`).
