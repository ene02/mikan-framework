using ManagedBass;
using ManagedBass.Fx;
using ManagedBass.Mix;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Channels;

namespace Sprout.Audio;

/// <summary>
/// A basic sound player class based on the BASS library.
/// </summary>
public class SoundPlayer : IDisposable
{
    public const float DEFAULT_VOLUME = 1, DEFAULT_PANNING = 0, DEFAULT_SPEED = 0, DEFAULT_PITCH = 0;

    private bool _isDisposed, _isPlaying; // States.
    private int _filterHandle, _mixerHandle, _tempoMixerHandle, _lastStreamHandle; // Handles.
    private List<int> _streamHandlers = new();
    private float _volume = 1, _panning = 0, _speed = 0, _pitch = 0; // Channel attributes.
    private double _filterPercentage = 0; // saved percentage for the hz pass filters
    private PassFilter _savedFilter = PassFilter.None; // last filter used.

    public enum PassFilter
    {
        High,
        Low,
        None,
    }

    /// <summary>
    /// Gives the handler of the last audio stream added.
    /// </summary>
    public int LastStreamAdded => _lastStreamHandle;

    /// <summary>
    /// Current volume of the mixer.
    /// </summary>
    public float Volume => _volume;

    /// <summary>
    /// Current panning of the mixer.
    /// </summary>
    public float Panning => _panning;

    /// <summary>
    /// Current speed of the mixer.
    /// </summary>
    public float Speed => _speed;

    /// <summary>
    /// Current pitch of the mixer.
    /// </summary>
    public float Pitch => _pitch;

    /// <summary>
    /// Returns a true or false depending if the SoundPlayer was disposed.
    /// </summary>
    public bool IsDisposed => _isDisposed;

    /// <summary>
    /// Current playback state of the mixer.
    /// </summary>
    public bool IsPlaying => _isPlaying;

    /// <summary>
    /// Event that triggers when mixer playback ends.
    /// </summary>
    public EventHandler? PlaybackEnded;

    /// <summary>
    /// Event that triggers when a song ends.
    /// </summary>
    public EventHandler? SongEnded;

    /// <summary>
    /// Returns the audio file time lenght in seconds.
    /// </summary>
    public double AudioLenght(int stream)
    {
        if (_tempoMixerHandle == 0)
            return 0;

        // get lenght
        long lengthInBytes = Bass.ChannelGetLength(stream);

        // convert byte length to seconds
        double lengthInSeconds = Bass.ChannelBytes2Seconds(stream, lengthInBytes);

        return lengthInSeconds;
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

        try
        {
            if (_tempoMixerHandle == 0)
            {
                // create the mixer stream.
                _mixerHandle = BassMix.CreateMixerStream(44100, 2, BassFlags.Decode | BassFlags.FxFreeSource);

                if (_mixerHandle == 0)
                    throw new InvalidOperationException("Failed to create mixer stream: " + Bass.LastError);

                // wrap into tempo stream.
                _tempoMixerHandle = BassFx.TempoCreate(_mixerHandle, BassFlags.Default);

                if (_tempoMixerHandle == 0)
                    throw new InvalidOperationException("Failed to create tempo stream for mixer: " + Bass.LastError);

                // mixer playback ended event.
                Bass.ChannelSetSync(_tempoMixerHandle, SyncFlags.End, 0, (handle, channel, data, user) =>
                {
                    PlaybackEnded?.Invoke(this, EventArgs.Empty);

                    _isPlaying = false;

                    Debug.WriteLine($"[SoundPlayer] Mixer playback ended");
                });
            }

            // create a stream for the file (decoded so it can be mixed)
            int streamHandle = Bass.CreateStream(filePath, 0, 0, BassFlags.Decode);

            if (streamHandle == 0)
                throw new InvalidOperationException("Failed to create stream: " + Bass.LastError);

            _streamHandlers.Add(streamHandle);
            _lastStreamHandle = streamHandle;

            // add the stream to the mixer
            if (!BassMix.MixerAddChannel(_mixerHandle, streamHandle, BassFlags.Default))
                throw new InvalidOperationException("Failed to add stream to mixer: " + Bass.LastError);

            // start playback
            if (!Bass.ChannelPlay(_tempoMixerHandle))
                throw new InvalidOperationException("Failed to play mixer: " + Bass.LastError);

            _isPlaying = true;

            // playback ended event.
            Bass.ChannelSetSync(streamHandle, SyncFlags.End, 0, (handle, channel, data, user) =>
            {
                SongEnded?.Invoke(this, EventArgs.Empty);

                Bass.StreamFree(streamHandle); // free the stream
                _streamHandlers.Remove(streamHandle);

                Debug.WriteLine($"[SoundPlayer] Playback ended for stream {streamHandle}");
            });

            Debug.WriteLine($"[SoundPlayer] Playing sound: {filePath}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    /// <summary>
    /// Completly stops mixer playback and releases all streams.
    /// </summary>
    public void Stop()
    {
        if (_tempoMixerHandle == 0)
            return;

        foreach (int stream in _streamHandlers)
        {
            Bass.StreamFree(stream);
        }

        Debug.WriteLine("[SoundPlayer] Mixer sound stopped.");
    }

    /// <summary>
    /// Pauses playback for entire mixer.
    /// </summary>
    public void Pause()
    {
        if (_tempoMixerHandle == 0)
            return;

        Bass.ChannelPause(_tempoMixerHandle);
        Debug.WriteLine("[SoundPlayer] Sound paused.");
    }

    /// <summary>
    /// Pauses playback for a stream.
    /// </summary>
    public void Pause(int stream)
    {
        if (_tempoMixerHandle == 0)
            return;

        if (!_streamHandlers.Contains(stream))
            return;

        Bass.ChannelPause(stream);
        Debug.WriteLine("[SoundPlayer] Sound paused.");
    }

    /// <summary>
    /// Change the position of the audio playback in miliseconds.
    /// </summary>
    /// <param name="miliseconds"></param>
    public void SetPosition(int stream, int miliseconds)
    {
        if (_tempoMixerHandle == 0)
            return;

        if (!_streamHandlers.Contains(stream))
            return;

        long newPos = Bass.ChannelSeconds2Bytes(stream, miliseconds / 1000);
        Bass.ChannelSetPosition(stream, newPos);
    }

    /// <summary>
    /// Change volume.
    /// </summary>
    /// <param name="volume"></param>
    public void SetVolume(float volume) => SetAttribute(_tempoMixerHandle, ChannelAttribute.Volume, volume);

    /// <summary>
    /// Change volume.
    /// </summary>
    /// <param name="volume"></param>
    public void SetVolume(float volume, int stream) => SetAttribute(stream, ChannelAttribute.Volume, volume);

    /// <summary>
    /// Set the volume for the left and right channels (less than 0 decreases the right channel, higher than 0 decreases the left channel).
    /// </summary>
    /// <param name="panning"></param>
    public void SetPanning(float panning) => SetAttribute(_tempoMixerHandle, ChannelAttribute.Pan, panning);

    /// <summary>
    /// Set the volume for the left and right channels (less than 0 decreases the right channel, higher than 0 decreases the left channel).
    /// </summary>
    /// <param name="panning"></param>
    public void SetPanning(float panning, int stream) => SetAttribute(stream, ChannelAttribute.Pan, panning);

    /// <summary>
    /// Changes the playback speed.
    /// </summary>
    /// <param name="speed"></param>
    public void SetSpeed(float speed) => SetAttribute(_tempoMixerHandle, ChannelAttribute.Tempo, speed);

    /// <summary>
    /// Changes the playback speed.
    /// </summary>
    /// <param name="speed"></param>
    public void SetSpeed(float speed, int stream) => SetAttribute(stream, ChannelAttribute.Tempo, speed);

    /// <summary>
    /// Changes the pitch.
    /// </summary>
    /// <param name="pitch"></param>
    public void SetPitch(float pitch) => SetAttribute(_tempoMixerHandle, ChannelAttribute.Pitch, pitch);

    /// <summary>
    /// Changes the pitch.
    /// </summary>
    /// <param name="pitch"></param>
    public void SetPitch(float pitch, int stream) => SetAttribute(stream, ChannelAttribute.Pitch, pitch);

    /// <summary>
    /// "Global" method used to set different attributes of a channel.
    /// </summary>
    /// <param name="handle"></param>
    /// <param name="attribute"></param>
    /// <param name="value"></param>
    private void SetAttribute(int handle, ChannelAttribute attribute, float value)
    {
        if (_tempoMixerHandle == 0)
            return;

        if (!handle.Equals(_tempoMixerHandle) || _streamHandlers.Contains(handle))
            return;

        Bass.ChannelSetAttribute(handle, attribute, value);

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

    /// <summary>
    /// Sets all attributes to the ones used last time.
    /// </summary>
    public void SetAttributesToDefault()
    {
        SetVolume(DEFAULT_VOLUME);
        SetPanning(DEFAULT_PANNING);
        SetSpeed(DEFAULT_SPEED);
        SetPitch(DEFAULT_PITCH);
    }

    /// <summary>
    /// Gives the current position of the track in seconds.
    /// </summary>
    /// <returns></returns>
    public double GetPosition(int stream)
    {
        if (_tempoMixerHandle == 0)
            return 0;

        if (!_streamHandlers.Contains(stream))
            return 0;

        long posInBytes = Bass.ChannelGetPosition(stream);
        double posInSec = Bass.ChannelBytes2Seconds(stream, posInBytes);

        return posInSec;
    }

    /// <summary>
    /// Removes all effect filters applied to the streams (High/Low pass filters).
    /// </summary>
    public void RemoveAllFx()
    {
        if (_tempoMixerHandle == 0)
            return;

        Bass.ChannelRemoveFX(_tempoMixerHandle, _filterHandle);
        _filterHandle = 0;

        Debug.WriteLine($"[SoundPlayer] Removing filters");
    }

    /// <summary>
    /// Calculates the percentage of a certain value.
    /// </summary>
    /// <param name="maxFreq"></param>
    /// <param name="percentage"></param>
    /// <returns></returns>
    private float CalculatePercentage(double value, double percentage)
    {
        return (float)(value * percentage / 100); // classic rule of three.
    }

    /// <summary>
    /// Directly returns the BiquadFilterType depending on the PassFilter enum.
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private BQFType GetBQFType(PassFilter filter)
    {
        return filter switch
        {
            PassFilter.Low => BQFType.LowPass,
            PassFilter.High => BQFType.HighPass,
            _ => throw new ArgumentOutOfRangeException(nameof(filter), "Invalid filter type")
        };
    }

    /// <summary>
    /// Applies a High/Low pass filter to the mixer, select the range of Hz by percentage.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="percentage"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void SetHzPassFilter(PassFilter type, double percentage = 0)
    {
        if (_tempoMixerHandle == 0)
            return;

        // keep percentage on a valid range
        percentage = Math.Clamp(percentage, 0, 99);

        // save values given.
        _filterPercentage = percentage;
        _savedFilter = type;

        // get the maximum Hz availible from sample rate divided by 2.
        double maxFreq = 44100 / 2;

        // if no filter is selected, then just remove it.
        if (_savedFilter == PassFilter.None)
        {
            RemoveAllFx();
            return;
        }

        // TODO: for some reason, 1Hz should be "the default", but it bugs the audio, so for now we will just remove effects when 0% is selected.
        if (percentage <= 0)
        {
            RemoveAllFx();
            return;
        }

        float freq = CalculatePercentage(maxFreq, percentage);

        BQFParameters parameters = new()
        {
            fCenter = freq, // cut-off frequency for LowPass or HighPass.
            lFilter = GetBQFType(type)
        };

        if (_filterHandle == 0) // create filter if it doesnt exist.
        {
            Debug.WriteLine($"[SoundPlayer] Filter didnt exist, creating...");
            _filterHandle = Bass.ChannelSetFX(_tempoMixerHandle, EffectType.BQF, 0);

            if (_filterHandle == 0)
                throw new InvalidOperationException($"Failed to set {type} filter: {Bass.LastError}");
        }

        // apply the filter parameters
        if (!Bass.FXSetParameters(_filterHandle, parameters))
            throw new InvalidOperationException($"Failed to apply parameters for {type} filter: {Bass.LastError}");

        Debug.WriteLine($"[SoundPlayer] {type} filter applied: Frequency={freq}");
    }

    /// <summary>
    /// Applies a High/Low pass filter to the audio, select the range of Hz by percentage.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="percentage"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void SetHzPassFilter(PassFilter type, int stream, double percentage = 0)
    {
        if (_tempoMixerHandle == 0)
            return;

        if (!_streamHandlers.Contains(stream))
            return;

        // keep percentage on a valid range
        percentage = Math.Clamp(percentage, 0, 99);

        // save values given.
        _filterPercentage = percentage;
        _savedFilter = type;

        SampleInfo sampleInfo = new();

        Bass.SampleGetInfo(stream, sampleInfo);

        // get the maximum Hz availible from sample rate divided by 2.
        double maxFreq = sampleInfo.Frequency / 2;

        // if no filter is selected, then just remove it.
        if (_savedFilter == PassFilter.None)
        {
            RemoveAllFx();
            return;
        }

        // TODO: for some reason, 1Hz should be "the default", but it bugs the audio, so for now we will just remove effects when 0% is selected.
        if (percentage <= 0)
        {
            RemoveAllFx();
            return;
        }

        float freq = CalculatePercentage(maxFreq, percentage);

        BQFParameters parameters = new()
        {
            fCenter = freq, // cut-off frequency for LowPass or HighPass.
            lFilter = GetBQFType(type)
        };

        if (_filterHandle == 0) // create filter if it doesnt exist.
        {
            Debug.WriteLine($"[SoundPlayer] Filter didnt exist, creating...");
            _filterHandle = Bass.ChannelSetFX(stream, EffectType.BQF, 0);

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
        if (_tempoMixerHandle == 0)
            return;

        if (!_isDisposed)
        {
            RemoveAllFx();

            Stop();
            Bass.ChannelStop(_tempoMixerHandle); // Stop the stream if its playing
            Bass.ChannelStop(_mixerHandle);
            Bass.Free(); // Free all BASS resources

            _isDisposed = true;

            Debug.WriteLine("[SoundPlayer] BASS resources freed.");
        }
    }
}