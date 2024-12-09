using ManagedBass;
using ManagedBass.Fx;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata;

namespace Mikan.Audio;

/// <summary>
/// A class that combines multiple StaticPlayer instances, enabling simultaneous or individual playback of
/// preloaded audio samples. It supports customization of audio properties like effects and volume for each sample,
/// facilitating dynamic audio compositions.
/// </summary>
public class SampleMixer
{
    private readonly static string DEBUG_TITLE = $"{DateTime.Today} || [Mixer]:";

    // List of streams in the mixer, gets updated when a streams starts or finishes.
    private List<SingleStreamPlayer> _staticPlayers = new();

    /// <summary>
    /// A read only list of the current sample players, ues this for effects or whatever you want, using these sample players to play other files WILL replace the files saved in the buffer.
    /// </summary>
    public IReadOnlyList<SingleStreamPlayer> StaticPlayers => _staticPlayers;

    /// <summary>
    /// Loads audio buffers into memory for quick playback.
    /// </summary>
    /// <param name="buffers"></param>
    public void LoadBuffers(params byte[][] buffers)
    {
        foreach (byte[] byteArray in buffers)
        {
            SingleStreamPlayer player = new()
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
        for (int i = 0; i < _staticPlayers.Count; i++)
        {
            for (int x = 0; i < buffers.Length; i++)
            {
                if (_staticPlayers[i].AudioData == buffers[x])
                {
                    _staticPlayers.RemoveAt(i);
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
        foreach (SingleStreamPlayer player in _staticPlayers)
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

        foreach (SingleStreamPlayer player in _staticPlayers)
        {
            player.Clear();
        }

        _staticPlayers.Clear();

        Debug.WriteLine($"{DEBUG_TITLE} All players and buffers disposed.");
    }
}