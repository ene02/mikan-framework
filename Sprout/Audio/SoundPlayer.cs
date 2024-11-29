using ManagedBass;
using ManagedBass.Fx;
using System.Diagnostics;

namespace Sprout.Audio;

/// <summary>
/// A basic sound player class based on the BASS library.
/// </summary>
public class SoundPlayer : IDisposable
{
    private bool _isDisposed, _isPlaying; // States.
    private int _streamHandle = -1; // Handle.
    private float _volume = 1, _panning = 0, _speed = 0, _pitch = 0; // Channel attributes.
    private int? _highPassFilter = null, _lowPassFilter = null; // Hz pass.
    private bool _isSettingsGlobal = true;

    /// <summary>
    /// Current volume.
    /// </summary>
    public float Volume => _volume;

    /// <summary>
    /// Current panning.
    /// </summary>
    public float Panning => _panning;

    /// <summary>
    /// Current speed.
    /// </summary>
    public float Speed => _speed;

    /// <summary>
    /// Current pitch.
    /// </summary>
    public float Pitch => _pitch;

    /// <summary>
    /// Returns a true or false depending if the SoundPlayer was disposed.
    /// </summary>
    public bool IsDisposed => _isDisposed;

    /// <summary>
    /// Current playback state.
    /// </summary>
    public bool IsPlaying => _isPlaying;

    /// <summary>
    /// Sets if new files should keep using the given attributes or if should they start with new ones.
    /// </summary>
    public bool IsSettingsGlobal
    {
        get
        {
            return _isSettingsGlobal;
        }

        set
        {
            _isSettingsGlobal = value;
        }
    }

    /// <summary>
    /// Event that triggers when playback ends.
    /// </summary>
    public EventHandler? PlaybackEnded;

    /// <summary>
    /// Returns the audio file time lenght in seconds.
    /// </summary>
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
            _streamHandle = Bass.CreateStream(filePath, 0, 0, BassFlags.Decode);

            // wrap the stream into a tempo stream for better managment.
            _streamHandle = BassFx.TempoCreate(_streamHandle, BassFlags.Default);

            if (_streamHandle == 0)
                throw new InvalidOperationException("Failed to create stream: " + Bass.LastError);

            // start playback
            if (!Bass.ChannelPlay(_streamHandle))
                throw new InvalidOperationException("Failed to play stream: " + Bass.LastError);

            _isPlaying = true;

            if (_isSettingsGlobal)
            {
                SetGlobalAttributes();
            }

            // playback ended event.
            Bass.ChannelSetSync(_streamHandle, SyncFlags.End, 0, (handle, channel, data, user) =>
            {
                PlaybackEnded?.Invoke(this, EventArgs.Empty);

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

    public void SetVolume(float volume) => SetAttribute(_streamHandle, ChannelAttribute.Volume, volume);

    public void SetPanning(float panning) => SetAttribute(_streamHandle, ChannelAttribute.Pan, panning);

    public void SetSpeed(float speed) => SetAttribute(_streamHandle, ChannelAttribute.Tempo, speed);

    public void SetPitch(float pitch) => SetAttribute(_streamHandle, ChannelAttribute.Pitch, pitch);

    private void SetAttribute(int handle, ChannelAttribute attribute, float value)
    {
        if (_streamHandle != 0)
            return;

        Bass.ChannelSetAttribute(_streamHandle, attribute, value);

        if (attribute == ChannelAttribute.Volume)
        {
            _volume = value;
        }
        else if (attribute == ChannelAttribute.Pan)
        {
            _panning = value;
        }
        else if (attribute == ChannelAttribute.Tempo)
        {
            _speed = value;
        }
        else if (attribute == ChannelAttribute.Pitch)
        {
            _pitch = value;
        }

    }

    private void SetGlobalAttributes()
    {
        SetVolume(_volume);
        SetPanning(_panning);
        SetSpeed(_speed);
        SetPitch(_pitch);
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