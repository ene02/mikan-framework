using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using ManagedBass;

namespace Mikan.Audio;

/// <summary>
/// Defines methods and values for audio playback classes, allowing Play, Pause, Stop and a method to get the its handler.
/// </summary>
public abstract class AudioProcessor
{
    // Audio states
    protected bool _isDisposed, _isPlaying;

    /// <summary>
    /// Returns a true or false depending if the SoundPlayer was disposed.
    /// </summary>
    public bool IsDisposed => _isDisposed;

    /// <summary>
    /// Current playback state of the mixer.
    /// </summary>
    public bool IsPlaying => _isPlaying;

    // Audio attributes
    protected float _volume = SoundAttributes.DEFAULT_VOLUME, _panning = SoundAttributes.DEFAULT_PANNING, _speed = SoundAttributes.DEFAULT_SPEED, _pitch = SoundAttributes.DEFAULT_PITCH;

    // Handlers for each EffectType in BassFx.
    // good or bad, i dont care what you think about this.
    protected int[] _fxHandlers = new int[23]; // damn.
    // 0 - DXChorus
    // 1 - DXDistortion
    // 2 - DXEcho
    // 3 - DXFlanger
    // 4 - DXCompressor
    // 5 - DXGargle
    // 6 - DX_I3DL2Reverb
    // 7 - DXParamEQ
    // 8 - DXReverb
    // 9 - Rotate
    // 10 - Volume
    // 11 - PeakEQ
    // 12 - Mix
    // 13 - Damp
    // 14 - AutoWah
    // 15 - Phaser
    // 16 - VolumeEnvelope
    // 17 - BQF
    // 18 - PitchShift
    // 19 - Freeverb

    /// <summary>
    /// Checks if BASS was initialized.
    /// </summary>
    /// <param name="bufferLenghts"></param>
    /// <param name="updatePeriods"></param>
    /// 

    public enum Preset
    {
        /// <summary>
        /// Default preset, good for simple audio playback.
        /// </summary>
        Default,

        /// <summary>
        /// Preset for low latency audio, perfect for effects and other stuff.
        /// </summary>
        LowLatency,
    }
    protected void CheckInit(Preset preset)
    {
        if (Bass.Init())
        {
            Debug.WriteLine($"[ManagedBass] BASS initialized successfully");

            switch (preset)
            {
                case Preset.Default:
                    Bass.Configure(Configuration.DeviceBufferLength, 200);
                    Bass.Configure(Configuration.DevicePeriod, 25);
                    Bass.Configure(Configuration.UpdatePeriod, 25);
                    Bass.Configure(Configuration.PlaybackBufferLength, 400);
                    break;
                case Preset.LowLatency:
                    Bass.Configure(Configuration.DeviceBufferLength, 50);
                    Bass.Configure(Configuration.DevicePeriod, 5);
                    Bass.Configure(Configuration.UpdatePeriod, 5);
                    Bass.Configure(Configuration.PlaybackBufferLength, 100);
                    break;
                default:
                    break;
            }
        }
        else
        {
            if (Bass.LastError == Errors.Already)
            {
                Debug.WriteLine($"[ManagedBass] BASS already initialized");
            }
        }
    }

    /// <summary>
    /// Event that triggers when mixer playback ends.
    /// </summary>
    public EventHandler PlaybackEnded;

    /// <summary>
    /// Stops the stream playback.
    /// </summary>
    public abstract void Stop();

    /// <summary>
    /// Pauses the stream in its current position.
    /// </summary>
    public abstract void Pause();

    /// <summary>
    /// Resumes the playback from where it was.
    /// </summary>
    public abstract void Resume();

    /// <summary>
    /// Returns the handler being used.
    /// </summary>
    /// <returns></returns>
    public abstract int GetHandler();

    /// <summary>
    /// Returns the FX handler of the current stream.
    /// </summary>
    /// <returns></returns>
    public int GetFXHandler(EffectType type)
    {
        switch (type)
        {
            case EffectType.DXChorus:
                return _fxHandlers[0];
            case EffectType.DXDistortion:
                return _fxHandlers[1];
            case EffectType.DXEcho:
                return _fxHandlers[2];
            case EffectType.DXFlanger:
                return _fxHandlers[3];
            case EffectType.DXCompressor:
                return _fxHandlers[4];
            case EffectType.DXGargle:
                return _fxHandlers[5];
            case EffectType.DX_I3DL2Reverb:
                return _fxHandlers[6];
            case EffectType.DXParamEQ:
                return _fxHandlers[7];
            case EffectType.DXReverb:
                return _fxHandlers[8];
            case EffectType.Rotate:
                return _fxHandlers[9];
            case EffectType.Volume:
                return _fxHandlers[10];
            case EffectType.PeakEQ:
                return _fxHandlers[11];
            case EffectType.Mix:
                return _fxHandlers[12];
            case EffectType.Damp:
                return _fxHandlers[13];
            case EffectType.AutoWah:
                return _fxHandlers[14];
            case EffectType.Phaser:
                return _fxHandlers[15];
            case EffectType.Chorus: // Added this effect
                return _fxHandlers[16];
            case EffectType.Distortion: // Added this effect
                return _fxHandlers[17];
            case EffectType.VolumeEnvelope:
                return _fxHandlers[18];
            case EffectType.BQF:
                return _fxHandlers[19];
            case EffectType.PitchShift:
                return _fxHandlers[20];
            case EffectType.Freeverb:
                return _fxHandlers[21];
            case EffectType.Echo:
                return _fxHandlers[22];
            default:
                return -1; // Return -1 if the effect type is not recognized
        }

    }

    /// <summary>
    /// Sets a handler value for an effect (DO NOT USEE THIS, thi iS FOR SoundEffects BUT I DONT KNOW HOW TO DO this XDDD), using this will cause many errors because of empty effect handlers
    /// </summary>
    /// <returns></returns>
    public void SetFXHandler(EffectType type, int handler)
    {
        switch (type)
        {
            case EffectType.DXChorus:
                _fxHandlers[0] = handler;
                break;
            case EffectType.DXDistortion:
                _fxHandlers[1] = handler;
                break;
            case EffectType.DXEcho:
                _fxHandlers[2] = handler;
                break;
            case EffectType.DXFlanger:
                _fxHandlers[3] = handler;
                break;
            case EffectType.DXCompressor:
                _fxHandlers[4] = handler;
                break;
            case EffectType.DXGargle:
                _fxHandlers[5] = handler;
                break;
            case EffectType.DX_I3DL2Reverb:
                _fxHandlers[6] = handler;
                break;
            case EffectType.DXParamEQ:
                _fxHandlers[7] = handler;
                break;
            case EffectType.DXReverb:
                _fxHandlers[8] = handler;
                break;
            case EffectType.Rotate:
                _fxHandlers[9] = handler;
                break;
            case EffectType.Volume:
                _fxHandlers[10] = handler;
                break;
            case EffectType.PeakEQ:
                _fxHandlers[11] = handler;
                break;
            case EffectType.Mix:
                _fxHandlers[12] = handler;
                break;
            case EffectType.Damp:
                _fxHandlers[13] = handler;
                break;
            case EffectType.AutoWah:
                _fxHandlers[14] = handler;
                break;
            case EffectType.Phaser:
                _fxHandlers[15] = handler;
                break;
            case EffectType.Chorus: // Added this effect
                _fxHandlers[16] = handler;
                break;
            case EffectType.Distortion: // Added this effect
                _fxHandlers[17] = handler;
                break;
            case EffectType.VolumeEnvelope:
                _fxHandlers[18] = handler;
                break;
            case EffectType.BQF:
                _fxHandlers[19] = handler;
                break;
            case EffectType.PitchShift:
                _fxHandlers[20] = handler;
                break;
            case EffectType.Freeverb:
                _fxHandlers[21] = handler;
                break;
            case EffectType.Echo:
                _fxHandlers[22] = handler;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), $"[ManagedBass] Effect type {type} is not recognized.");
        }

    }
}