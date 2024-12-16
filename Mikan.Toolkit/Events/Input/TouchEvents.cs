using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mikan.Toolkit.Handlers;
using static SDL2.SDL;

namespace Mikan.Toolkit.Events.Input;

/// <summary>
/// Provides events related to touch input, including finger down, up, and motion tracking.
/// </summary>
public static class TouchEvents
{
    /// <summary>
    /// Triggered when a finger touches the screen.
    /// </summary>
    /// <param name="fingerId">The ID of the finger (unique identifier for each touch).</param>
    /// <param name="x">The X coordinate of the touch.</param>
    /// <param name="y">The Y coordinate of the touch.</param>
    /// <param name="pressure">The pressure of the touch (range from 0 to 1).</param>
    public static event Action<long, float, float, float> FingerDown;

    /// <summary>
    /// Triggered when a finger is lifted off the screen.
    /// </summary>
    /// <param name="fingerId">The ID of the finger (unique identifier for each touch).</param>
    /// <param name="x">The X coordinate of the touch release.</param>
    /// <param name="y">The Y coordinate of the touch release.</param>
    /// <param name="pressure">The pressure of the touch (range from 0 to 1).</param>
    public static event Action<long, float, float, float> FingerUp;

    /// <summary>
    /// Triggered when a finger moves across the screen.
    /// </summary>
    /// <param name="fingerId">The ID of the finger (unique identifier for each touch).</param>
    /// <param name="x">The X coordinate of the touch movement.</param>
    /// <param name="y">The Y coordinate of the touch movement.</param>
    /// <param name="pressure">The pressure of the touch (range from 0 to 1).</param>
    public static event Action<long, float, float, float> FingerMotion;

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

                // Handle Finger Touch Events
                if (e.type == SDL_EventType.SDL_FINGERDOWN)
                {
                    long FingerId = e.tfinger.fingerId; // Getting the finger ID
                    float X = e.tfinger.x; // Getting the X coordinate of the touch
                    float Y = e.tfinger.y; // Getting the Y coordinate of the touch
                    float Pressure = e.tfinger.pressure; // Getting the pressure of the touch
                    FingerDown?.Invoke(FingerId, X, Y, Pressure); // Trigger the FingerDown event
                }
                else if (e.type == SDL_EventType.SDL_FINGERUP)
                {
                    long FingerId = e.tfinger.fingerId; // Getting the finger ID
                    float X = e.tfinger.x; // Getting the X coordinate of the touch release
                    float Y = e.tfinger.y; // Getting the Y coordinate of the touch release
                    float Pressure = e.tfinger.pressure; // Getting the pressure of the touch
                    FingerUp?.Invoke(FingerId, X, Y, Pressure); // Trigger the FingerUp event
                }
                else if (e.type == SDL_EventType.SDL_FINGERMOTION)
                {
                    long FingerId = e.tfinger.fingerId; // Getting the finger ID
                    float X = e.tfinger.x; // Getting the X coordinate of the touch movement
                    float Y = e.tfinger.y; // Getting the Y coordinate of the touch movement
                    float Pressure = e.tfinger.pressure; // Getting the pressure of the touch
                    FingerMotion?.Invoke(FingerId, X, Y, Pressure); // Trigger the FingerMotion event
                }
            }
        }
    }

    public static void StopPolling()
    {
        _isRunning = false;
    }

}
