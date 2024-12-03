using ManagedBass.Fx;
using ManagedBass;
using System.Diagnostics;

namespace Sprout.Audio;

public static class SoundEffects
{
    private readonly static string DEBUG_TITLE = $"{DateTime.Today} || [SoundEffects]:";

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

        BQFParameters parameters = new()
        {
            fCenter = hz, // cut-off frequency for LowPass or HighPass.
            lFilter = type
        };

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

        Debug.WriteLine($"{DEBUG_TITLE} All FX removed");
    }
}
