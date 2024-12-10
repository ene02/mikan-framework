using ManagedBass.Fx;
using ManagedBass;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;

namespace Mikan.Audio;

/// <summary>
/// A class designed for playing a unique audio buffer that doesnt get removed automatically, perfect for samples or any kind of audio that needs to be replayed multiple times.
/// </summary>
public class SingleStreamPlayer : AudioProcessor
{
    private readonly static string DEBUG_TITLE = $"[ManagedBass]:";

    private int _streamHandle;
    private byte[] _data = Array.Empty<byte>();

    public byte[] AudioData
    {
        get
        {
            return _data;
        }

        set
        {
            _data = value;
        }
    }

    public SingleStreamPlayer(Preset preset)
    {
        CheckInit(preset);
    }

    /// <summary>
    /// Plays the buffered audio data.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public void Play()
    {
        if (_streamHandle == 0)
        {
            // create a stream for the file
            _streamHandle = Bass.CreateStream(_data, 0, _data.Length, BassFlags.Decode | BassFlags.Float);

            // wrap in tempo stream
            _streamHandle = BassFx.TempoCreate(_streamHandle, BassFlags.Default);

            if (_streamHandle == 0)
                throw new InvalidOperationException($"{DEBUG_TITLE} Failed to create stream: " + Bass.LastError);
        }

        // start playback
        if (!Bass.ChannelPlay(_streamHandle, true))
            throw new InvalidOperationException($"{DEBUG_TITLE} Failed to play stream: " + Bass.LastError);

        _isPlaying = true;

        // playback ended event.
        Bass.ChannelSetSync(_streamHandle, SyncFlags.End, 0, (handle, channel, data, user) =>
        {
            PlaybackEnded?.Invoke(this, EventArgs.Empty);
        });
    }

    public override void Stop()
    {
        if (_streamHandle == 0)
            return;

        Bass.ChannelStop(_streamHandle);
        Debug.WriteLine($"{DEBUG_TITLE} Stream sound stopped.");
    }

    public override void Pause()
    {
        if (_streamHandle == 0)
            return;

        Bass.ChannelPause(_streamHandle);
        Debug.WriteLine($"{DEBUG_TITLE} Sound paused.");
    }

    public override void Resume()
    {
        if (_streamHandle == 0)
            return;

        Bass.ChannelPlay(_streamHandle);
        Debug.WriteLine($"{DEBUG_TITLE} Sound resumed.");
    }

    /// <summary>
    /// Clears the stream.
    /// </summary>
    public void Clear()
    {
        if (_streamHandle == 0)
            return;

        Stop();
        SoundEffects.RemoveAllFx(this);
        Bass.StreamFree(_streamHandle);

        _data = Array.Empty<byte>();
    }

    // Helper Methods
    public override int GetHandler()
    {
        return _streamHandle;
    }
}
