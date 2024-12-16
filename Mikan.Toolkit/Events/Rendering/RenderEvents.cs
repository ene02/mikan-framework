using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mikan.Toolkit.Handlers;
using static SDL2.SDL;

namespace Mikan.Toolkit.Events.Rendering;

/// <summary>
/// Provides events related to render state changes, such as when render targets or devices are reset.
/// </summary>
public static class RenderEvents
{
    /// <summary>
    /// Triggered when render targets are reset and their contents need to be updated.
    /// </summary>
    public static event Action RenderTargetsReset;

    /// <summary>
    /// Triggered when the render device is reset, requiring recreation of textures.
    /// </summary>
    public static event Action RenderDeviceReset;

    private static bool _isRunning;

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

                // Handle Render Targets Reset Event
                if (e.type == SDL_EventType.SDL_RENDER_TARGETS_RESET)
                {
                    RenderTargetsReset?.Invoke();
                }
                // Handle Render Device Reset Event
                else if (e.type == SDL_EventType.SDL_RENDER_DEVICE_RESET)
                {
                    RenderDeviceReset?.Invoke();
                }
            }
        }
    }

    public static void StopPolling()
    {
        _isRunning = false;
    }

}
