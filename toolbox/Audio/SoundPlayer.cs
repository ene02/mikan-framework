using ManagedBass;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;

namespace Toolbox.Audio;

/// <summary>
/// A basic single SoundPlayer based on BASS.Net
/// </summary>
public class SoundPlayer : IDisposable
{
    private bool _isDisposed, _isPlaying;
    private int _streamHandle = -1;
    private double _volume = 1;

    public double Volume => _volume;
    public bool IsDisposed => _isDisposed;
    public bool IsPlaying => _isPlaying;

    public EventHandler PlaybackEnded;

    public double AudioLenght
    {
        get
        {
            if (_streamHandle != 0)
            {
                // get lenght
                long lengthInBytes = Bass.ChannelGetLength(_streamHandle);

                // convert byte length to seconds
                double lengthInSeconds = Bass.ChannelBytes2Seconds(_streamHandle, lengthInBytes);

                return lengthInSeconds;
            }
            else
            {
                return 0;
            }
        }
    }

    public SoundPlayer()
    {
        if (!Bass.Init())
            throw new InvalidOperationException("Failed to initialize BASS.");

        Debug.WriteLine("[SoundPlayer] BASS initialized successfully");
    }

    /// <summary>
    /// Play an audio file from a file path.
    /// </summary>
    /// <param name="filePath"></param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public void Play(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            throw new ArgumentException("Invalid file path", nameof(filePath));

        if (_streamHandle != 0) // stop previous stream in case its still going, to prevent audio overlap.
        {
            Bass.ChannelStop(_streamHandle);
            Bass.StreamFree(_streamHandle);
        }

        try
        {
            // create stream from filepath.
            _streamHandle = Bass.CreateStream(filePath);

            if (_streamHandle == 0)
                throw new InvalidOperationException("Failed to create stream: " + Bass.LastError);

            // start playback
            if (!Bass.ChannelPlay(_streamHandle))
                throw new InvalidOperationException("Failed to play stream: " + Bass.LastError);

            _isPlaying = true;

            SetVolume(_volume);

            // playback ended event.
            Bass.ChannelSetSync(_streamHandle, SyncFlags.End, 0, (handle, channel, data, user) =>
            {
                PlaybackEnded.Invoke(this, EventArgs.Empty);

                _isPlaying = false;

                Debug.WriteLine($"[SoundPlayer] Playback ended for: {filePath}");
                Bass.StreamFree(_streamHandle); // free the stream
                _streamHandle = 0; // reset the stream handle
            });

            Debug.WriteLine("[SoundPlayer] Playing sound");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    /// <summary>
    /// Stops plaback.
    /// </summary>
    public void Stop()
    {
        if (_streamHandle != 0)
        {
            Bass.ChannelStop(_streamHandle);
            Debug.WriteLine("[SoundPlayer] Sound stopped.");
        }
    }

    /// <summary>
    /// Pauses playback.
    /// </summary>
    public void Pause()
    {
        if (_streamHandle != 0)
        {
            Bass.ChannelPause(_streamHandle);
            Debug.WriteLine("[SoundPlayer] Sound paused.");
        }
    }

    /// <summary>
    /// Sets the volume of the current sound player.
    /// </summary>
    /// <param name="volume"></param>
    public void SetVolume(double volume)
    {
        if (_streamHandle != 0)
            Bass.ChannelSetAttribute(_streamHandle, ChannelAttribute.Volume, volume);

        _volume = volume;
    }

    /// <summary>
    /// Change the position of the audio playback in miliseconds.
    /// </summary>
    /// <param name="miliseconds"></param>
    public void SetPosition(int miliseconds)
    {
        if (_streamHandle != 0)
        {
            long newPos = Bass.ChannelSeconds2Bytes(_streamHandle, miliseconds / 1000);
            Bass.ChannelSetPosition(_streamHandle, newPos);
        }
    }

    /// <summary>
    /// Gives the current position of the track in seconds.
    /// </summary>
    /// <returns></returns>
    public double GetPosition()
    {
        if (_streamHandle != 0)
        {
            long posInBytes = Bass.ChannelGetPosition(_streamHandle);
            double posInSec = Bass.ChannelBytes2Seconds(_streamHandle, posInBytes);

            return posInSec;
        }

        return 0;
    }

    /// <summary>
    /// Releases all resources used by SoundPlayer.
    /// </summary>
    public void Dispose()
    {
        if (!_isDisposed && _streamHandle != 0)
        {
            Bass.ChannelStop(_streamHandle); // Stop the stream if its playing
            Bass.Free(); // Free all BASS resources
            _streamHandle = 0; // Clear the stream handle

            _isDisposed = true;
            Debug.WriteLine("[SoundPlayer] BASS resources freed.");
        }
    }
}