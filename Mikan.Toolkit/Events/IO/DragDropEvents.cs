using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Mikan.Toolkit.Handlers;
using static SDL2.SDL;

namespace Mikan.Toolkit.Events.IO;

/// <summary>
/// Provides events related to drag-and-drop operations, including file and text drops.
/// </summary>
public static class DragAndDropEvents
{
    /// <summary>
    /// Triggered when a file is dropped.
    /// </summary>
    /// <param name="filePath">The path of the file that was dropped.</param>
    public static event Action<string> FileDropped;

    /// <summary>
    /// Triggered when text is dropped.
    /// </summary>
    /// <param name="text">The dropped text.</param>
    public static event Action<string> TextDropped;

    /// <summary>
    /// Triggered when a drag-and-drop operation begins.
    /// </summary>
    public static event Action DropBegin;

    /// <summary>
    /// Triggered when a drag-and-drop operation is complete.
    /// </summary>
    public static event Action DropComplete;

    private static bool _isRunning;

    public static void StartPolling()
    {
        SDLHandler.CheckSDLInit();

        if (_isRunning)
            return;

        _isRunning = true;

        while (_isRunning)
        {
            while (SDL_WaitEvent(out SDL_Event e) != 0)
            {
                if (!_isRunning)
                {
                    break;
                }

                // Handle Drop Begin Event
                if (e.type == SDL_EventType.SDL_DROPBEGIN)
                {
                    DropBegin?.Invoke();  // Trigger the DropBegin event
                }

                // Handle Drop Complete Event
                else if (e.type == SDL_EventType.SDL_DROPCOMPLETE)
                {
                    DropComplete?.Invoke();  // Trigger the DropComplete event
                }

                // Handle File Drop Event
                else if (e.type == SDL_EventType.SDL_DROPFILE)
                {
                    string filePath = Marshal.PtrToStringAnsi(e.drop.file);  // Get the file path from the event
                    FileDropped?.Invoke(filePath);  // Trigger the FileDropped event
                }
            }
        }
    }

    public static void StopPolling()
    {
        _isRunning = false;
    }
}
