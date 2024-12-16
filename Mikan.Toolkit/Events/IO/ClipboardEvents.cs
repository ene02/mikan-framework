using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mikan.Toolkit.Handlers;
using static SDL2.SDL;

namespace Mikan.Toolkit.Events.IO;

/// <summary>
/// Provides events related to clipboard changes, such as when the clipboard content is updated.
/// </summary>
public static class ClipboardEvents
{
    /// <summary>
    /// Triggered when the clipboard content is updated.
    /// </summary>
    public static event Action ClipboardUpdated;

    private static bool _isRunning;
    private static string _previousClipboardContent = string.Empty;

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

                // Handle Clipboard Update Events
                if (e.type == SDL_EventType.SDL_CLIPBOARDUPDATE)
                {
                    string clipboardContent = SDL_GetClipboardText(); // Retrieve the current clipboard content
                    if (clipboardContent != _previousClipboardContent)
                    {
                        ClipboardUpdated?.Invoke(); // Trigger the ClipboardUpdated event
                        _previousClipboardContent = clipboardContent; // Update previous clipboard content
                    }
                }
            }
        }
    }

    public static void StopPolling()
    {
        _isRunning = false;
    }

}
