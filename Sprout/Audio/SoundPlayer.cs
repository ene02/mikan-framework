using ManagedBass;
using ManagedBass.Fx;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Channels;

namespace Sprout.Audio;

/// <summary>
/// A basic sound player class based on the BASS library.
/// </summary>
public class SoundPlayer : IDisposable
{
    public enum PassFilter
    {
        High,
        Low,
        None,
    }

    private bool _isDisposed, _isPlaying; // States.
    private int _streamHandle = -1, _filterHandle; // Handles.
    private float _volume = 1, _panning = 0, _speed = 0, _pitch = 0; // Channel attributes.
    private double _filterPercentage = 0;
    private PassFilter _savedFilter = PassFilter.None;
    private SampleInfo _sampleInfo = new();
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
            if (_streamHandle == 0)
                return 0;

            // get lenght
            long lengthInBytes = Bass.ChannelGetLength(_streamHandle);

            // convert byte length to seconds
            double lengthInSeconds = Bass.ChannelBytes2Seconds(_streamHandle, lengthInBytes);

            return lengthInSeconds;
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

        if (_filterHandle != 0) // remove previous effects.
        {
            RemoveFilter();
        }

        try
        {
            // create stream from filepath.
            _streamHandle = Bass.CreateStream(filePath, 0, 0, BassFlags.Decode);

            // wrap the stream into a tempo stream for better managment.
            _streamHandle = BassFx.TempoCreate(_streamHandle, BassFlags.Default);

            if (_streamHandle == 0)
                throw new InvalidOperationException("Failed to create stream: " + Bass.LastError);

            // get audio info and save it.
            Bass.SampleGetInfo(_streamHandle, _sampleInfo);

            // if global settings are enabled, then the new audio will have the previous settings.
            if (_isSettingsGlobal)
            {
                SetGlobalAttributes();
                SetHzPassFilter(_savedFilter, _filterPercentage);
                Debug.WriteLine($"Filters applied at playback: f:{_savedFilter} p:{_filterPercentage}");
            }

            // start playback
            if (!Bass.ChannelPlay(_streamHandle))
                throw new InvalidOperationException("Failed to play stream: " + Bass.LastError);

            _isPlaying = true;

            // playback ended event.
            Bass.ChannelSetSync(_streamHandle, SyncFlags.End, 0, (handle, channel, data, user) =>
            {
                PlaybackEnded?.Invoke(this, EventArgs.Empty);

                _isPlaying = false;

                RemoveFilter();
                Bass.StreamFree(_streamHandle); // free the stream
                _streamHandle = 0; // reset the stream handle

                Debug.WriteLine($"[SoundPlayer] Playback ended");
            });

            Debug.WriteLine($"[SoundPlayer] Playing sound: {filePath}");
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
        if (_streamHandle == 0)
            return;

        Bass.ChannelStop(_streamHandle);
        Debug.WriteLine("[SoundPlayer] Sound stopped.");
    }

    /// <summary>
    /// Pauses playback.
    /// </summary>
    public void Pause()
    {
        if (_streamHandle == 0)
            return;

        Bass.ChannelPause(_streamHandle);
        Debug.WriteLine("[SoundPlayer] Sound paused.");
    }

    /// <summary>
    /// Change the position of the audio playback in miliseconds.
    /// </summary>
    /// <param name="miliseconds"></param>
    public void SetPosition(int miliseconds)
    {
        if (_streamHandle == 0)
            return;

        long newPos = Bass.ChannelSeconds2Bytes(_streamHandle, miliseconds / 1000);
        Bass.ChannelSetPosition(_streamHandle, newPos);
    }

    public void SetVolume(float volume) => SetAttribute(_streamHandle, ChannelAttribute.Volume, volume);

    public void SetPanning(float panning) => SetAttribute(_streamHandle, ChannelAttribute.Pan, panning);

    public void SetSpeed(float speed) => SetAttribute(_streamHandle, ChannelAttribute.Tempo, speed);

    public void SetPitch(float pitch) => SetAttribute(_streamHandle, ChannelAttribute.Pitch, pitch);

    private void SetAttribute(int handle, ChannelAttribute attribute, float value)
    {
        if (_streamHandle == 0)
            return;

        Bass.ChannelSetAttribute(_streamHandle, attribute, value);

        switch (attribute)
        {
            case ChannelAttribute.Volume:
                _volume = value;
                break;
            case ChannelAttribute.Pan:
                _panning = value;
                break;
            case ChannelAttribute.Tempo:
                _speed = value;
                break;
            case ChannelAttribute.Pitch:
                _pitch = value;
                break;
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
        if (_streamHandle == 0)
            return 0;

        long posInBytes = Bass.ChannelGetPosition(_streamHandle);
        double posInSec = Bass.ChannelBytes2Seconds(_streamHandle, posInBytes);

        return posInSec;
    }

    /// <summary>
    /// Removes all FX filters applied.
    /// </summary>
    private void RemoveFilter()
    {
        if (_streamHandle == 0)
            return;

        Bass.ChannelRemoveFX(_streamHandle, _filterHandle);
        _filterHandle = 0;

        Debug.WriteLine($"[SoundPlayer] Removing filters");
    }

    private float CalculateFrequency(double maxFreq, double percentage)
    {
        return (float)(maxFreq * percentage / 100); // rule of three based on the max sample rate from the file when an usuable percentage is given.
    }

    private BQFType GetBQFType(PassFilter filter)
    {
        return filter switch
        {
            PassFilter.Low => BQFType.LowPass,
            PassFilter.High => BQFType.HighPass,
            _ => throw new ArgumentOutOfRangeException(nameof(filter), "Invalid filter type")
        };
    }

    public void SetHzPassFilter(PassFilter type, double percentage = 0)
    {
        if (_streamHandle == 0)
            return;

        // keep percentage on a valid range
        percentage = Math.Clamp(percentage, 0, 99);

        // save values given.
        _filterPercentage = percentage;
        _savedFilter = type;

        // get the maximum Hz availible from sample rate divided by 2.
        double maxFreq = _sampleInfo.Frequency / 2;

        // if no filter is selected, then just remove it.
        if (_savedFilter == PassFilter.None)
        {
            RemoveFilter();
            return;
        }

        // TODO: for some reason, 1Hz should be "the default", but it bugs the audio, so for now we will just remove effects when 0% is selected.
        if (percentage <= 0)
        {
            RemoveFilter();
            return;
        }

        float freq = CalculateFrequency(maxFreq, percentage);

        BQFParameters parameters = new()
        {
            fCenter = freq, // cut-off frequency for LowPass or HighPass.
            lFilter = GetBQFType(type)
        };

        if (_filterHandle == 0) // create filter if it doesnt exist.
        {
            Debug.WriteLine($"[SoundPlayer] Filter didnt exist, creating...");
            _filterHandle = Bass.ChannelSetFX(_streamHandle, EffectType.BQF, 0);

            if (_filterHandle == 0)
                throw new InvalidOperationException($"Failed to set {type} filter: {Bass.LastError}");
        }

        // apply the filter parameters
        if (!Bass.FXSetParameters(_filterHandle, parameters))
            throw new InvalidOperationException($"Failed to apply parameters for {type} filter: {Bass.LastError}");

        Debug.WriteLine($"[SoundPlayer] {type} filter applied: Frequency={freq}");
    }

    /// <summary>
    /// Releases all resources used by SoundPlayer.
    /// </summary>
    public void Dispose()
    {
        if (_streamHandle == 0)
            return;

        if (!_isDisposed)
        {
            RemoveFilter();

            Bass.ChannelStop(_streamHandle); // Stop the stream if its playing
            Bass.Free(); // Free all BASS resources
            _streamHandle = 0; // Clear the stream handle

            _isDisposed = true;

            Debug.WriteLine("[SoundPlayer] BASS resources freed.");
        }
    }
}