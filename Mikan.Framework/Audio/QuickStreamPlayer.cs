using ManagedBass;
using ManagedBass.Fx;
using System.Diagnostics;

namespace Mikan.Audio;

/// <summary>
/// A class designed for quick change of audio playback, where each new playback operation stops any currently playing audio. This ensures that only
/// one audio stream is active at a time, with new streams dynamically created for each playback and old streams cleared and removed.
/// </summary>
public class QuickStreamPlayer : AudioProcessor
{
    private readonly static string DEBUG_TITLE = $"[ManagedBass]:";

    private int _streamHandle;

    public QuickStreamPlayer(int bufferLenghts = 100, int updatePeriods = 10)
    {
        CheckInit(bufferLenghts, updatePeriods);
    }

    /// <summary>
    /// Returns the handler being used.
    /// </summary>
    /// <returns></returns>
    public override int GetHandler()
    {
        return _streamHandle;
    }

    /// <summary>
    /// Plays the given filepath.
    /// </summary>
    /// <param name="filePath"></param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public void Play(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            throw new ArgumentException($"{DEBUG_TITLE} Invalid file path", nameof(filePath));

        if (_streamHandle != 0)
        {
            Stop();
        }

        // create a stream for the file
        _streamHandle = Bass.CreateStream(filePath, 0, 0, BassFlags.Decode);

        // wrap in tempo stream
        _streamHandle = BassFx.TempoCreate(_streamHandle, BassFlags.Default);

        if (_streamHandle == 0)
            throw new InvalidOperationException($"{DEBUG_TITLE} Failed to create stream: " + Bass.LastError);

        // start playback
        if (!Bass.ChannelPlay(_streamHandle))
            throw new InvalidOperationException($"{DEBUG_TITLE} Failed to play stream: " + Bass.LastError);

        _isPlaying = true;

        // playback ended event
        Bass.ChannelSetSync(_streamHandle, SyncFlags.End, 0, (handle, channel, data, user) =>
        {
            PlaybackEnded?.Invoke(this, EventArgs.Empty);
            Bass.StreamFree(_streamHandle); // free the stream
        });
    }

    /// <summary>
    /// Plays the given audio buffer.
    /// </summary>
    /// <param name="buffer"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void Play(byte[] buffer)
    {
        if (_streamHandle != 0)
        {
            Stop();
        }

        // create a stream for the buffer
        _streamHandle = Bass.CreateStream(buffer, 0, buffer.Length, BassFlags.Decode);

        // wrap in tempo stream
        _streamHandle = BassFx.TempoCreate(_streamHandle, BassFlags.Default);

        if (_streamHandle == 0)
            throw new InvalidOperationException($"{DEBUG_TITLE} Failed to create stream: " + Bass.LastError);

        // start playback
        if (!Bass.ChannelPlay(_streamHandle))
            throw new InvalidOperationException($"{DEBUG_TITLE} Failed to play stream: " + Bass.LastError);

        _isPlaying = true;

        // playback ended event
        Bass.ChannelSetSync(_streamHandle, SyncFlags.End, 0, (handle, channel, data, user) =>
        {
            PlaybackEnded?.Invoke(this, EventArgs.Empty);
            Bass.StreamFree(_streamHandle); // free the stream
        });
    }

    /// <summary>
    /// Stops playback and releases the stream.
    /// </summary>
    public override void Stop()
    {
        if (_streamHandle == 0)
            return;

        Bass.ChannelStop(_streamHandle);
        Bass.StreamFree(_streamHandle);

        _streamHandle = 0;

        Debug.WriteLine($"{DEBUG_TITLE} Stream sound stopped.");
    }

    /// <summary>
    /// Pauses playback.
    /// </summary>
    public override void Pause()
    {
        if (_streamHandle == 0)
            return;

        Bass.ChannelPause(_streamHandle);
        Debug.WriteLine($"{DEBUG_TITLE} Sound paused.");
    }

    /// <summary>
    /// Resumes playback.
    /// </summary>
    public override void Resume()
    {
        if (_streamHandle == 0)
            return;

        Bass.ChannelPlay(_streamHandle);
        Debug.WriteLine($"{DEBUG_TITLE} Sound resumed.");
    }
}
