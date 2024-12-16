using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mikan.Toolkit.Handlers;
using static SDL2.SDL;

namespace Mikan.Toolkit.Events.Input;

/// <summary>
/// Provides events related to gesture input, including dollar and multi gestures.
/// </summary>
public static class GestureEvents
{
    /// <summary>
    /// Triggered when a dollar gesture is detected.
    /// </summary>
    /// <param name="gestureId">The ID of the detected gesture.</param>
    /// <param name="x">The X coordinate of the gesture's origin.</param>
    /// <param name="y">The Y coordinate of the gesture's origin.</param>
    /// <param name="distance">The distance of the gesture's movement.</param>
    public static event Action<float, float, float, uint, long> DollarGesture;

    /// <summary>
    /// Triggered when a multi-gesture is detected (such as pinch, rotate, etc.).
    /// </summary>
    /// <param name="x">The X coordinate of the gesture.</param>
    /// <param name="y">The Y coordinate of the gesture.</param>
    /// <param name="rotation">The rotation angle of the gesture.</param>
    /// <param name="distance">The distance of the gesture's movement.</param>
    public static event Action<float, float, float, float> MultiGesture;

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

                // Handle Gesture Events
                if (e.type == SDL_EventType.SDL_DOLLARGESTURE)
                {
                    float GestureId = e.dgesture.gestureId;  // Getting the gesture ID from the event
                    float X = e.dgesture.x;                  // Getting the X coordinate of the gesture
                    float Y = e.dgesture.y;                  // Getting the Y coordinate of the gesture
                    uint Fingers = e.dgesture.numFingers;  // Getting the number of fingers involved in the gesture
                    long Timestamp = e.dgesture.timestamp;   // Getting the timestamp of the gesture

                    DollarGesture?.Invoke(GestureId, X, Y, Fingers, Timestamp);  // Trigger the DollarGesture event
                }
                else if (e.type == SDL_EventType.SDL_MULTIGESTURE)
                {
                    float X = e.mgesture.x;           // Getting the X coordinate of the gesture
                    float Y = e.mgesture.y;           // Getting the Y coordinate of the gesture
                    float Distance = e.mgesture.dDist;   // Getting the distance change of the gesture
                    float Rotation = e.mgesture.dTheta; // Getting the angle change (rotation) of the gesture

                    MultiGesture?.Invoke(X, Y, Distance, Rotation); // Trigger the MultiGesture event
                }

                // Handle other event types (you can add more here as needed)
            }
        }
    }

}
