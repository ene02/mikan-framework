using ManagedBass;
using ManagedBass.Fx;
using System.Diagnostics;

namespace Mikan.Audio;

/// <summary>
/// QuickPlayer is designed for one-at-a-time audio playback. It dynamically creates a new stream for each playback request, ensuring that only one stream<br />
/// is active at any given time. Existing streams are automatically stopped and cleared before initiating new playback. This makes QuickPlayer ideal for scenarios<br />
/// where an audio source is not needed for long times, just load whatever file, play it and forget about it.
/// <para>Since new files given are played on a new stream, all efects and attributes are reset by default on each new playback.</para>
/// </summary>
public class QuickPlayer : AudioProcessor
{
    private readonly static string DEBUG_TITLE = $"[ManagedBass]:";

    private int _streamHandle;

    public QuickPlayer(Preset preset)
    {
        CheckInit(preset);
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
    public override void Play(object data = null)
    {
        byte[] audioData = null;

        if (data is string)
        {
            if (string.IsNullOrWhiteSpace((string)data) || !File.Exists((string)data))
                throw new ArgumentException($"{DEBUG_TITLE} Invalid file path", (string)data);

            audioData = File.ReadAllBytes((string)data);
        }
        else if (data is byte[])
        {
            audioData = (byte[])data;
        }
        else if (data is null)
        {
            Debug.WriteLine("[ManagedBass] QuickPlayer doesn't save audio data and no data has been given for playback");
            return;
        }

        if (_streamHandle != 0)
        {
            Stop();
        }

        // create a stream for the file
        _streamHandle = Bass.CreateStream(audioData, 0, audioData.Length, BassFlags.Decode);

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

            SoundEffects.RemoveAllFx(this);
            Bass.StreamFree(_streamHandle); // free the stream
            _streamHandle = 0;
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
        SoundEffects.RemoveAllFx(this);
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
