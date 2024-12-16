using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mikan.Toolkit.Handlers;
using static SDL2.SDL;

namespace Mikan.Toolkit.Events.Input;

/// <summary>
/// Provides events related to mouse input, including motion, button presses, and wheel scrolling.
/// </summary>
public static class MouseEvents
{
    /// <summary>
    /// Triggered when the mouse is moved.
    /// </summary>
    /// <param name="x">The X-coordinate of the mouse position.</param>
    /// <param name="y">The Y-coordinate of the mouse position.</param>
    public static event Action<int, int> MouseMotion;

    /// <summary>
    /// Triggered when a mouse button is pressed.
    /// </summary>
    /// <param name="button">The button that was pressed (e.g., left, right, or middle).</param>
    /// <param name="x">The X-coordinate of the mouse position at the time of the press.</param>
    /// <param name="y">The Y-coordinate of the mouse position at the time of the press.</param>
    public static event Action<int, int, int> MouseButtonDown;

    /// <summary>
    /// Triggered when a mouse button is released.
    /// </summary>
    /// <param name="button">The button that was released (e.g., left, right, or middle).</param>
    /// <param name="x">The X-coordinate of the mouse position at the time of the release.</param>
    /// <param name="y">The Y-coordinate of the mouse position at the time of the release.</param>
    public static event Action<int, int, int> MouseButtonUp;

    /// <summary>
    /// Triggered when the mouse wheel is scrolled.
    /// </summary>
    /// <param name="x">The horizontal scroll amount (positive for right, negative for left).</param>
    /// <param name="y">The vertical scroll amount (positive for up, negative for down).</param>
    public static event Action<int, int> MouseWheel;

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

                // Handle Mouse Events
                if (e.type == SDL_EventType.SDL_MOUSEMOTION)
                {
                    int X = e.motion.x; // Getting the X coordinate of the mouse
                    int Y = e.motion.y; // Getting the Y coordinate of the mouse
                    MouseMotion?.Invoke(X, Y); // Trigger the MouseMotion event
                }
                else if (e.type == SDL_EventType.SDL_MOUSEBUTTONDOWN)
                {
                    int Button = e.button.button; // Getting the mouse button that was pressed
                    int X = e.button.x; // Getting the X coordinate at the time of the press
                    int Y = e.button.y; // Getting the Y coordinate at the time of the press
                    MouseButtonDown?.Invoke(Button, X, Y); // Trigger the MouseButtonDown event
                }
                else if (e.type == SDL_EventType.SDL_MOUSEBUTTONUP)
                {
                    int Button = e.button.button; // Getting the mouse button that was released
                    int X = e.button.x; // Getting the X coordinate at the time of the release
                    int Y = e.button.y; // Getting the Y coordinate at the time of the release
                    MouseButtonUp?.Invoke(Button, X, Y); // Trigger the MouseButtonUp event
                }
                else if (e.type == SDL_EventType.SDL_MOUSEWHEEL)
                {
                    int X = e.wheel.x; // Getting the horizontal scroll amount
                    int Y = e.wheel.y; // Getting the vertical scroll amount
                    MouseWheel?.Invoke(X, Y); // Trigger the MouseWheel event
                }
            }
        }
    }

    public static void StopPolling()
    {
        _isRunning = false;
    }

}
