using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mikan.Toolkit.Handlers;
using static SDL2.SDL;

namespace Mikan.Toolkit.Events.Audio;

/// <summary>
/// Provides events related to audio device hotplugging, such as when audio devices are added or removed.
/// </summary>
public static class AudioHotplugEvents
{
    /// <summary>
    /// Triggered when a new audio device is added.
    /// </summary>
    /// <param name="deviceId">The ID of the newly added audio device.</param>
    public static event Action<uint> AudioDeviceAdded;

    /// <summary>
    /// Triggered when an audio device is removed.
    /// </summary>
    /// <param name="deviceId">The ID of the removed audio device.</param>
    public static event Action<uint> AudioDeviceRemoved;

    private static bool _isRunning;

    public static void StopPolling()
    {
        _isRunning = false;
    }

    public static void StartPolling()
    {
        SDLHandler.CheckSDLInit();

        if (_isRunning)
            return;

        _isRunning = true;

        while (_isRunning) // Keep running until we decide to stop
        {
            while (SDL_WaitEvent(out SDL_Event e) != 0)
            {
                if (!_isRunning)
                {
                    break;
                }

                // Handle Audio Hotplug Events
                if (e.type == SDL_EventType.SDL_AUDIODEVICEADDED)
                {
                    AudioDeviceAdded?.Invoke(e.adevice.which);  // Trigger the event while giving the device ID at the same time.
                }
                else if (e.type == SDL_EventType.SDL_AUDIODEVICEREMOVED)
                {
                    AudioDeviceRemoved?.Invoke(e.adevice.which);  // Same as above.
                }
            }
        }
    }

}
