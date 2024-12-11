using System.Diagnostics;

namespace Mikan.Toolkit.Audio;

/// <summary>
/// AudioMixer is an utility for managing and playing multiple audio samples simultaneously. It allows efficient playback<br />
/// of preloaded audio buffers, providing features such as playback control, buffer management, and cleanup. Designed for real-time audio applications<br />
/// it ensures low-latency performance and seamless integration with other audio components.
/// </summary>
public class AudioMixer
{
    private readonly static string DEBUG_TITLE = $"[AudioMixer]:";

    // List of streams in the mixer, gets updated when a streams starts or finishes.
    private List<PersistentPlayer> _staticPlayers = new();

    /// <summary>
    /// A read only list of the current sample players, ues this for effects or whatever you want, using these sample players to play other files WILL replace the files saved in the buffer.
    /// </summary>
    public IReadOnlyList<PersistentPlayer> StaticPlayers => _staticPlayers;

    /// <summary>
    /// Loads audio buffers into memory for quick playback.
    /// </summary>
    /// <param name="buffers"></param>
    public void LoadBuffers(params byte[][] buffers)
    {
        foreach (byte[] byteArray in buffers)
        {
            PersistentPlayer player = new(AudioProcessor.Preset.Realtime)
            {
                AudioData = byteArray,
            };

            _staticPlayers.Add(player);
        }
    }

    /// <summary>
    /// Remove audio buffers.
    /// </summary>
    /// <param name="buffers"></param>
    public void RemoveBuffers(params byte[][] buffers)
    {
        for (int i = 0; i < _staticPlayers.Count; i++) // Go through each PersistentPlayer.
        {
            for (int x = 0; i < buffers.Length; i++) // Go through each audio buffer
            {
                if (_staticPlayers[i].AudioData == buffers[x]) // If the current PersistenPlayer has the same data as the buffer selected by the loop.
                {
                    _staticPlayers.RemoveAt(i); // Remove it.
                }
            }
        }
    }

    /// <summary>
    /// Plays the selected audio player with the corresponding audio buffer.
    /// </summary>
    /// <param name="index"></param>
    public void PlayBuffer(int index)
    {
        _staticPlayers[index].Play();
    }

    /// <summary>
    /// Stops all playbacks.
    /// </summary>
    public void StopAll()
    {
        foreach (PersistentPlayer player in _staticPlayers)
        {
            player.Stop();
        }
    }

    /// <summary>
    /// Pause an audio stream.
    /// </summary>
    /// <param name="index"></param>
    public void PauseBuffer(int index)
    {
        _staticPlayers[index].Pause();
    }

    /// <summary>
    /// Resumes an audio stream.
    /// </summary>
    /// <param name="index"></param>
    public void ResumeBuffer(int index)
    {
        _staticPlayers[index].Resume();
    }

    /// <summary>
    /// Disposes all AudioPlayers and their buffers.
    /// </summary>
    public void ClearMixer()
    {
        StopAll();

        foreach (PersistentPlayer player in _staticPlayers)
        {
            player.Clear();
        }

        _staticPlayers.Clear();

        Debug.WriteLine($"{DEBUG_TITLE} All players and buffers were disposed.");
    }
}