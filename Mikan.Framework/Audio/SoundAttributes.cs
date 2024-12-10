using ManagedBass;
using System.Diagnostics;

namespace Mikan.Audio;

/// <summary>
/// Extension methods for attributes of AudioProcessors.
/// </summary>
public static class SoundAttributes
{
    private readonly static string DEBUG_TITLE = $"[ManagedBass]:";

    public const float DEFAULT_VOLUME = 1;
    public const float DEFAULT_PANNING = 0;
    public const float DEFAULT_SPEED = 0;
    public const float DEFAULT_PITCH = 0;

    /// <summary>
    /// Returns the audio file time lenght in seconds.
    /// </summary>
    public static double GetAudioLenght(this AudioProcessor audioProcessor)
    {
        int handler = audioProcessor.GetHandler();

        if (handler == 0)
            return 0;

        // get lenght
        long lengthInBytes = Bass.ChannelGetLength(handler);

        // convert byte length to seconds
        double lengthInSeconds = Bass.ChannelBytes2Seconds(handler, lengthInBytes);

        return lengthInSeconds;
    }

    /// <summary>
    /// Retrieves the volume attribute.
    /// </summary>
    /// <param name="audioProcessor"></param>
    /// <returns></returns>
    public static float GetVolume(this AudioProcessor audioProcessor)
    {
        int handler = audioProcessor.GetHandler();

        if (handler == 0)
            return 0;

        Bass.ChannelGetAttribute(handler, ChannelAttribute.Volume, out float value);
        return value;
    }

    /// <summary>
    /// Retrieves the panning attribute.
    /// </summary>
    /// <param name="audioProcessor"></param>
    /// <returns></returns>
    public static float GetPanning(this AudioProcessor audioProcessor)
    {
        int handler = audioProcessor.GetHandler();

        if (handler == 0)
            return 0;

        Bass.ChannelGetAttribute(handler, ChannelAttribute.Pan, out float value);
        return value;
    }

    /// <summary>
    /// Retrieves the speed attribute.
    /// </summary>
    /// <param name="audioProcessor"></param>
    /// <returns></returns>
    public static float GetSpeed(this AudioProcessor audioProcessor)
    {
        int handler = audioProcessor.GetHandler();

        if (handler == 0)
            return 0;

        Bass.ChannelGetAttribute(handler, ChannelAttribute.Tempo, out float value);
        return value;
    }

    /// <summary>
    /// Retrieves the pitch attribute.
    /// </summary>
    /// <param name="audioProcessor"></param>
    /// <returns></returns>
    public static float GetPitch(this AudioProcessor audioProcessor)
    {
        int handler = audioProcessor.GetHandler();

        if (handler == 0)
            return 0;

        Bass.ChannelGetAttribute(handler, ChannelAttribute.Pitch, out float value);
        return value;
    }

    /// <summary>
    /// Gives the current position of the track in seconds.
    /// </summary>
    /// <returns></returns>
    public static double GetPosition(this AudioProcessor audioProcessor)
    {
        int handler = audioProcessor.GetHandler();

        if (handler == 0)
            return 0;

        long posInBytes = Bass.ChannelGetPosition(handler);
        double posInSec = Bass.ChannelBytes2Seconds(handler, posInBytes);

        return posInSec;
    }

    /// <summary>
    /// Change volume.
    /// </summary>
    /// <param name="volume"></param>
    public static void SetVolume(this AudioProcessor audioProcessor, float value)
    {
        int handler = audioProcessor.GetHandler();

        if (handler == 0)
            return;

        Bass.ChannelSetAttribute(handler, ChannelAttribute.Volume, value);
    }

    /// <summary>
    /// Set the volume for the left and right channels (less than 0 decreases the right channel, higher than 0 decreases the left channel).
    /// </summary>
    /// <param name="panning"></param>
    public static void SetPanning(this AudioProcessor audioProcessor, float value)
    {
        int handler = audioProcessor.GetHandler();

        if (handler == 0)
            return;

        Bass.ChannelSetAttribute(handler, ChannelAttribute.Pan, value);
    }

    /// <summary>
    /// Changes the playback speed.
    /// </summary>
    /// <param name="speed"></param>
    public static void SetSpeed(this AudioProcessor audioProcessor, float value)
    {
        int handler = audioProcessor.GetHandler();

        if (handler == 0)
            return;

        Bass.ChannelSetAttribute(handler, ChannelAttribute.Tempo, value);
    }

    /// <summary>
    /// Changes the pitch.
    /// </summary>
    /// <param name="pitch"></param>
    public static void SetPitch(this AudioProcessor audioProcessor, float value)
    {
        int handler = audioProcessor.GetHandler();

        if (handler == 0)
            return;

        Bass.ChannelSetAttribute(handler, ChannelAttribute.Pitch, value);
    }

    /// <summary>
    /// Sets all attributes to the ones used last time.
    /// </summary>
    public static void SetAttributesToDefault(this AudioProcessor audioProcessor)
    {
        audioProcessor.SetVolume(DEFAULT_VOLUME);
        audioProcessor.SetPanning(DEFAULT_PANNING);
        audioProcessor.SetSpeed(DEFAULT_SPEED);
        audioProcessor.SetPitch(DEFAULT_PITCH);

        Debug.WriteLine($"{DEBUG_TITLE} All attiributes were reset to their default values.");
    }
}