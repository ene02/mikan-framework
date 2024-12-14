using ManagedBass.Fx;
using ManagedBass;
using System.Diagnostics;

namespace Mikan.Toolkit.Audio;

/// <summary>
/// Extension methods for effects of AudioProcessors.
/// </summary>
public static class SoundEffects
{
    private readonly static string DEBUG_TITLE = $"[ManagedBass]:";

    /// <summary>
    /// Applies a Biquad filter to the audio, select a filter type and a frequency.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="percentage"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public static void SetBiquadFilter(this AudioProcessor audioProcessor, BQFType type, float hz)
    {
        int handler = audioProcessor.GetHandler();

        if (handler == 0)
            return;

        hz = Math.Clamp(hz, 0, float.MaxValue);

        BQFParameters parameters = new()
        {
            fCenter = hz, // cut-off frequency for LowPass or HighPass.
            lFilter = type
        };

        if (hz < 5) // Very small thresh holds are unnoticeable and for some reason they cause some weird behaviour, so its better to try to avoid them.
        {
            if (audioProcessor.GetFXHandler(EffectType.BQF) == 0)
                return;

            Bass.ChannelRemoveFX(handler, audioProcessor.GetFXHandler(EffectType.BQF));
            audioProcessor.SetFXHandler(EffectType.BQF, 0);
            Debug.WriteLine($"{DEBUG_TITLE} Filter removed.");
            return;
        }

        if (audioProcessor.GetFXHandler(EffectType.BQF) == 0) // create filter if it doesnt exist.
        {
            Debug.WriteLine($"{DEBUG_TITLE} Filter didnt exist, creating...");

            // make new fx handler.
            int newFxHandler = Bass.ChannelSetFX(handler, EffectType.BQF, 0);

            // pass to audio processor.
            audioProcessor.SetFXHandler(EffectType.BQF, newFxHandler);

            if (newFxHandler == 0)
                throw new InvalidOperationException($"{DEBUG_TITLE} Failed to set {type.GetType().FullName} filter: {Bass.LastError}");
        }

        // apply the filter parameters
        if (!Bass.FXSetParameters(audioProcessor.GetFXHandler(EffectType.BQF), parameters))
            throw new InvalidOperationException($"{DEBUG_TITLE} Failed to apply parameters for {type} filter: {Bass.LastError}");
    }

    public static void SetEchoEffect(this AudioProcessor audioProcessor, float delay, float feedback, float wetMix, float dryMix)
    {
        int handler = audioProcessor.GetHandler();

        if (handler == 0)
            return;

        delay = Math.Clamp(delay, 0, float.MaxValue);
        feedback = Math.Clamp(feedback, 0, 1);
        wetMix = Math.Clamp(wetMix, 0, 1);
        dryMix = Math.Clamp(dryMix, 0, 1);

        EchoParameters parameters = new()
        {
            fDelay = delay,           // Delay in milliseconds
            fFeedback = feedback,     // Feedback level (0 to 1)
            fWetMix = wetMix,    // Wet/Dry mix (0 to 1)
            fDryMix = dryMix
        };

        if ((Math.Abs(delay) < 1f || feedback <= 0f || wetMix <= 0f))
        {
            if (audioProcessor.GetFXHandler(EffectType.Echo) == 0)
                return;

            Bass.ChannelRemoveFX(handler, audioProcessor.GetFXHandler(EffectType.Echo));
            audioProcessor.SetFXHandler(EffectType.Echo, 0);
            Debug.WriteLine($"{DEBUG_TITLE} Echo effect removed due to negligible or invalid parameters.");
            return;
        }

        if (audioProcessor.GetFXHandler(EffectType.Echo) == 0) // Create effect if it doesn't exist.
        {
            Debug.WriteLine($"{DEBUG_TITLE} Echo effect didn't exist, creating...");

            // Create a new FX handler for the Echo effect.
            int newFxHandler = Bass.ChannelSetFX(handler, EffectType.Echo, 0);

            // Pass to the audio processor.
            audioProcessor.SetFXHandler(EffectType.Echo, newFxHandler);

            if (newFxHandler == 0)
                throw new InvalidOperationException($"{DEBUG_TITLE} Failed to create Echo effect: {Bass.LastError}");
        }

        // Apply the echo effect parameters.
        if (!Bass.FXSetParameters(audioProcessor.GetFXHandler(EffectType.Echo), parameters))
            throw new InvalidOperationException($"{DEBUG_TITLE} Failed to apply parameters for Echo effect: {Bass.LastError}");
    }

    /// <summary>
    /// Removes all effect filters applied to the streams.
    /// </summary>
    public static void RemoveAllFx(this AudioProcessor audioProcessor)
    {
        int handler = audioProcessor.GetHandler();

        if (handler == 0)
            return;

        if (audioProcessor.GetFXHandler(EffectType.DXChorus) != 0)
        {
            Bass.ChannelRemoveFX(handler, audioProcessor.GetFXHandler(EffectType.DXChorus));
            audioProcessor.SetFXHandler(EffectType.DXChorus, 0);
        }

        if (audioProcessor.GetFXHandler(EffectType.DXDistortion) != 0)
        {
            Bass.ChannelRemoveFX(handler, audioProcessor.GetFXHandler(EffectType.DXDistortion));
            audioProcessor.SetFXHandler(EffectType.DXDistortion, 0);
        }

        if (audioProcessor.GetFXHandler(EffectType.DXEcho) != 0)
        {
            Bass.ChannelRemoveFX(handler, audioProcessor.GetFXHandler(EffectType.DXEcho));
            audioProcessor.SetFXHandler(EffectType.DXEcho, 0);
        }

        if (audioProcessor.GetFXHandler(EffectType.DXFlanger) != 0)
        {
            Bass.ChannelRemoveFX(handler, audioProcessor.GetFXHandler(EffectType.DXFlanger));
            audioProcessor.SetFXHandler(EffectType.DXFlanger, 0);
        }

        if (audioProcessor.GetFXHandler(EffectType.DXCompressor) != 0)
        {
            Bass.ChannelRemoveFX(handler, audioProcessor.GetFXHandler(EffectType.DXCompressor));
            audioProcessor.SetFXHandler(EffectType.DXCompressor, 0);
        }

        if (audioProcessor.GetFXHandler(EffectType.DXGargle) != 0)
        {
            Bass.ChannelRemoveFX(handler, audioProcessor.GetFXHandler(EffectType.DXGargle));
            audioProcessor.SetFXHandler(EffectType.DXGargle, 0);
        }

        if (audioProcessor.GetFXHandler(EffectType.DX_I3DL2Reverb) != 0)
        {
            Bass.ChannelRemoveFX(handler, audioProcessor.GetFXHandler(EffectType.DX_I3DL2Reverb));
            audioProcessor.SetFXHandler(EffectType.DX_I3DL2Reverb, 0);
        }

        if (audioProcessor.GetFXHandler(EffectType.DXParamEQ) != 0)
        {
            Bass.ChannelRemoveFX(handler, audioProcessor.GetFXHandler(EffectType.DXParamEQ));
            audioProcessor.SetFXHandler(EffectType.DXParamEQ, 0);
        }

        if (audioProcessor.GetFXHandler(EffectType.DXReverb) != 0)
        {
            Bass.ChannelRemoveFX(handler, audioProcessor.GetFXHandler(EffectType.DXReverb));
            audioProcessor.SetFXHandler(EffectType.DXReverb, 0);
        }

        if (audioProcessor.GetFXHandler(EffectType.Rotate) != 0)
        {
            Bass.ChannelRemoveFX(handler, audioProcessor.GetFXHandler(EffectType.Rotate));
            audioProcessor.SetFXHandler(EffectType.Rotate, 0);
        }

        if (audioProcessor.GetFXHandler(EffectType.Volume) != 0)
        {
            Bass.ChannelRemoveFX(handler, audioProcessor.GetFXHandler(EffectType.Volume));
            audioProcessor.SetFXHandler(EffectType.Volume, 0);
        }

        if (audioProcessor.GetFXHandler(EffectType.PeakEQ) != 0)
        {
            Bass.ChannelRemoveFX(handler, audioProcessor.GetFXHandler(EffectType.PeakEQ));
            audioProcessor.SetFXHandler(EffectType.PeakEQ, 0);
        }

        if (audioProcessor.GetFXHandler(EffectType.Mix) != 0)
        {
            Bass.ChannelRemoveFX(handler, audioProcessor.GetFXHandler(EffectType.Mix));
            audioProcessor.SetFXHandler(EffectType.Mix, 0);
        }

        if (audioProcessor.GetFXHandler(EffectType.Damp) != 0)
        {
            Bass.ChannelRemoveFX(handler, audioProcessor.GetFXHandler(EffectType.Damp));
            audioProcessor.SetFXHandler(EffectType.Damp, 0);
        }

        if (audioProcessor.GetFXHandler(EffectType.AutoWah) != 0)
        {
            Bass.ChannelRemoveFX(handler, audioProcessor.GetFXHandler(EffectType.AutoWah));
            audioProcessor.SetFXHandler(EffectType.AutoWah, 0);
        }

        if (audioProcessor.GetFXHandler(EffectType.Phaser) != 0)
        {
            Bass.ChannelRemoveFX(handler, audioProcessor.GetFXHandler(EffectType.Phaser));
            audioProcessor.SetFXHandler(EffectType.Phaser, 0);
        }

        if (audioProcessor.GetFXHandler(EffectType.VolumeEnvelope) != 0)
        {
            Bass.ChannelRemoveFX(handler, audioProcessor.GetFXHandler(EffectType.VolumeEnvelope));
            audioProcessor.SetFXHandler(EffectType.VolumeEnvelope, 0);
        }

        if (audioProcessor.GetFXHandler(EffectType.BQF) != 0)
        {
            Bass.ChannelRemoveFX(handler, audioProcessor.GetFXHandler(EffectType.BQF));
            audioProcessor.SetFXHandler(EffectType.BQF, 0);
        }

        if (audioProcessor.GetFXHandler(EffectType.PitchShift) != 0)
        {
            Bass.ChannelRemoveFX(handler, audioProcessor.GetFXHandler(EffectType.PitchShift));
            audioProcessor.SetFXHandler(EffectType.PitchShift, 0);
        }

        if (audioProcessor.GetFXHandler(EffectType.Freeverb) != 0)
        {
            Bass.ChannelRemoveFX(handler, audioProcessor.GetFXHandler(EffectType.Freeverb));
            audioProcessor.SetFXHandler(EffectType.Freeverb, 0);
        }

        if (audioProcessor.GetFXHandler(EffectType.Echo) != 0)
        {
            Bass.ChannelRemoveFX(handler, audioProcessor.GetFXHandler(EffectType.Echo));
            audioProcessor.SetFXHandler(EffectType.Echo, 0);
        }

        if (audioProcessor.GetFXHandler(EffectType.Distortion) != 0)
        {
            Bass.ChannelRemoveFX(handler, audioProcessor.GetFXHandler(EffectType.Distortion));
            audioProcessor.SetFXHandler(EffectType.Distortion, 0);
        }

        if (audioProcessor.GetFXHandler(EffectType.Chorus) != 0)
        {
            Bass.ChannelRemoveFX(handler, audioProcessor.GetFXHandler(EffectType.Chorus));
            audioProcessor.SetFXHandler(EffectType.Chorus, 0);
        }


        Debug.WriteLine($"{DEBUG_TITLE} All FX removed");
    }
}
