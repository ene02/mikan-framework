using ManagedBass.Fx;
using ManagedBass;
using System.Diagnostics;

namespace Mikan.Toolkit.Audio;

/// <summary>
/// PersistentPlayer provides functionality to play audio from a reusable, persistent buffer. Unlike QuickPlayer, this class retains the audio data and stream<br />
/// allowing the same audio to be replayed multiple times without reloading the data. This makes it ideal for playing sound samples, loops, or any audio requiring repeated<br />
/// playback with minimal overhead.
/// </summary>
public class PersistentPlayer : AudioProcessor
{
    private readonly static string DEBUG_TITLE = $"[ManagedBass]:";

    private int _streamHandle;
    private byte[] _data = Array.Empty<byte>();

    /// <summary>
    /// Audio buffer used for quick playback.
    /// </summary>
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

    public PersistentPlayer(Preset preset)
    {
        CheckInit(preset);
    }

    /// <summary>
    /// Plays the buffered audio data or replaces the AudioData in case a file or buffer is given.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public override void Play(object data = null)
    {
        if (data is string)
        {
            if (string.IsNullOrWhiteSpace((string)data) || !File.Exists((string)data))
                throw new ArgumentException($"{DEBUG_TITLE} Invalid file path", (string)data);

            AudioData = File.ReadAllBytes((string)data);
        }
        else if (data is byte[])
        {
            AudioData = (byte[])data;
        }

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
        this.RemoveAllFx();
        Bass.StreamFree(_streamHandle);
        _streamHandle = 0;

        _data = Array.Empty<byte>();
    }

    // Helper Methods
    public override int GetHandler()
    {
        return _streamHandle;
    }
}
