using ManagedBass.Fx;
using ManagedBass;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;

namespace Sprout.Audio;

/// <summary>
/// A class for playing a predefined audio file or buffer repeatedly without creating new streams for each playback. The playback
/// operates on a single reusable stream, allowing consistent audio output unless manually reset.
/// </summary>
public class StaticPlayer : AudioProcessor
{
    private readonly static string DEBUG_TITLE = $"{DateTime.Today} || [AudioPlayer]:";

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

    public override int GetHandler()
    {
        return _streamHandle;
    }

    public StaticPlayer(int bufferLenghts = 100, int updatePeriods = 10)
    {
        CheckInit(bufferLenghts, updatePeriods);
    }

    public override void Play()
    {
        try
        {
            if (_streamHandle == 0)
            {
                // create a stream for the file
                _streamHandle = Bass.CreateStream(_data, 0, _data.Length, BassFlags.Decode);

                // wrap in tempo stream,
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
        catch (Exception ex)
        {
            Debug.WriteLine($"{DEBUG_TITLE} {ex}");
        }
    }

    public override void Play(string filePath)
    {
        try
        {
            byte[] buffer = File.ReadAllBytes(filePath);
            _data = buffer;

            if (_streamHandle == 0)
            {
                // create a stream for the file
                _streamHandle = Bass.CreateStream(buffer, 0, buffer.Length, BassFlags.Decode);

                // wrap in tempo stream,
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
        catch (Exception ex)
        {
            Debug.WriteLine($"{DEBUG_TITLE} {ex}");
        }
    }

    public override void Play(byte[] buffer)
    {
        try
        {
            _data = buffer;

            if (_streamHandle == 0)
            {
                // create a stream for the file
                _streamHandle = Bass.CreateStream(buffer, 0, buffer.Length, BassFlags.Decode);

                // wrap in tempo stream,
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
        catch (Exception ex)
        {
            Debug.WriteLine($"{DEBUG_TITLE} {ex}");
        }
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
    /// Clears the SamplePlayer.
    /// </summary>
    public void Clear()
    {
        if (_streamHandle == 0)
            return;

        Stop();

        Bass.StreamFree(_streamHandle);

        _data = Array.Empty<byte>();
    }
}
