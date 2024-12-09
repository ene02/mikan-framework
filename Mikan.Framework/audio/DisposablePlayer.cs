using ManagedBass;
using ManagedBass.Fx;
using System.Diagnostics;

namespace Sprout.Audio;

/// <summary>
/// A class designed for single-instance audio playback, where each new playback operation stops any currently playing audio. This ensures that only
/// one audio stream is active at a time, with new streams dynamically created for each playback and old streams cleared and removed.
/// </summary>
public class DisposablePlayer : AudioProcessor
{
    private readonly static string DEBUG_TITLE = $"{DateTime.Today} || [AudioPlayer]:";

    private int _streamHandle;

    public override int GetHandler()
    {
        return _streamHandle;
    }

    public DisposablePlayer(int bufferLenghts = 100, int updatePeriods = 10)
    {
        CheckInit(bufferLenghts, updatePeriods);
    }

    public override void Play()
    {
        Debug.WriteLine($"{DEBUG_TITLE} This audio processor doesn't save any audio data by default, so i got nothing to play.");
    }

    public override void Play(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            throw new ArgumentException($"{DEBUG_TITLE} Invalid file path", nameof(filePath));

        try
        {
            if (_streamHandle != 0)
            {
                Stop();
            }

            // create a stream for the file
            _streamHandle = Bass.CreateStream(filePath, 0, 0, BassFlags.Decode);

            // wrap in tempo stream,
            _streamHandle = BassFx.TempoCreate(_streamHandle, BassFlags.Default);

            if (_streamHandle == 0)
                throw new InvalidOperationException($"{DEBUG_TITLE} Failed to create stream: " + Bass.LastError);

            // start playback
            if (!Bass.ChannelPlay(_streamHandle))
                throw new InvalidOperationException($"{DEBUG_TITLE} Failed to play stream: " + Bass.LastError);

            _isPlaying = true;

            // playback ended event.
            Bass.ChannelSetSync(_streamHandle, SyncFlags.End, 0, (handle, channel, data, user) =>
            {
                PlaybackEnded?.Invoke(this, EventArgs.Empty);

                Bass.StreamFree(_streamHandle); // free the stream
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
            if (_streamHandle != 0)
            {
                Stop();
            }

            // create a stream for the file
            _streamHandle = Bass.CreateStream(buffer, 0, buffer.Length, BassFlags.Decode);

            // wrap in tempo stream,
            _streamHandle = BassFx.TempoCreate(_streamHandle, BassFlags.Default);

            if (_streamHandle == 0)
                throw new InvalidOperationException($"{DEBUG_TITLE} Failed to create stream: " + Bass.LastError);

            // start playback
            if (!Bass.ChannelPlay(_streamHandle))
                throw new InvalidOperationException($"{DEBUG_TITLE} Failed to play stream: " + Bass.LastError);

            _isPlaying = true;

            // playback ended event.
            Bass.ChannelSetSync(_streamHandle, SyncFlags.End, 0, (handle, channel, data, user) =>
            {
                PlaybackEnded?.Invoke(this, EventArgs.Empty);

                Bass.StreamFree(_streamHandle); // free the stream
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
        Bass.StreamFree(_streamHandle);

        _streamHandle = 0;

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
}