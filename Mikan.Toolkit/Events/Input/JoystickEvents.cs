using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mikan.Toolkit.Handlers;
using static SDL2.SDL;

namespace Mikan.Toolkit.Events.Input;

/// <summary>
/// Provides events related to joystick input, including axis movements, button presses, device additions/removals, and battery updates.
/// </summary>
public static class JoystickEvents
{
    /// <summary>
    /// Triggered when a joystick axis moves.
    /// </summary>
    /// <param name="joystickId">The ID of the joystick.</param>
    /// <param name="axisValue">The value of the axis movement (e.g., -1 to 1 for a typical joystick axis).</param>
    public static event Action<int, int> JoystickAxisMotion;

    /// <summary>
    /// Triggered when a joystick button is pressed.
    /// </summary>
    /// <param name="joystickId">The ID of the joystick.</param>
    /// <param name="buttonIndex">The index of the button pressed.</param>
    public static event Action<int, int> JoystickButtonDown;

    /// <summary>
    /// Triggered when a joystick button is released.
    /// </summary>
    /// <param name="joystickId">The ID of the joystick.</param>
    /// <param name="buttonIndex">The index of the button released.</param>
    public static event Action<int, int> JoystickButtonUp;

    /// <summary>
    /// Triggered when a joystick is added to the system.
    /// </summary>
    /// <param name="joystickId">The ID of the newly added joystick.</param>
    public static event Action<int> JoystickDeviceAdded;

    /// <summary>
    /// Triggered when a joystick is removed from the system.
    /// </summary>
    /// <param name="joystickId">The ID of the removed joystick.</param>
    public static event Action<int> JoystickDeviceRemoved;

    /// <summary>
    /// Triggered when a joystick's battery level is updated.
    /// </summary>
    /// <param name="joystickId">The ID of the joystick.</param>
    /// <param name="batteryLevel">The current battery level of the joystick, expressed as a float between 0 and 1.</param>
    public static event Action<int, float> JoystickBatteryUpdated;

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

                // Handle Joystick Events
                if (e.type == SDL_EventType.SDL_JOYAXISMOTION)
                {
                    int JoystickId = e.caxis.which;  // Getting the joystick ID from the event
                    int AxisValue = e.caxis.axisValue;  // Getting the axis value from the event
                    JoystickAxisMotion?.Invoke(JoystickId, AxisValue);  // Trigger the event
                }
                else if (e.type == SDL_EventType.SDL_JOYBUTTONDOWN)
                {
                    int JoystickId = e.cbutton.which;  // Getting the joystick ID from the event
                    int ButtonIndex = e.cbutton.button;  // Getting the button index from the event
                    JoystickButtonDown?.Invoke(JoystickId, ButtonIndex);  // Trigger the event
                }
                else if (e.type == SDL_EventType.SDL_JOYBUTTONUP)
                {
                    int JoystickId = e.cbutton.which;  // Getting the joystick ID from the event
                    int ButtonIndex = e.cbutton.button;  // Getting the button index from the event
                    JoystickButtonUp?.Invoke(JoystickId, ButtonIndex);  // Trigger the event
                }
                else if (e.type == SDL_EventType.SDL_JOYDEVICEADDED)
                {
                    int JoystickId = e.cdevice.which;  // Getting the joystick ID from the event
                    JoystickDeviceAdded?.Invoke(JoystickId);  // Trigger the event
                }
                else if (e.type == SDL_EventType.SDL_JOYDEVICEREMOVED)
                {
                    int JoystickId = e.cdevice.which;  // Getting the joystick ID from the event
                    JoystickDeviceRemoved?.Invoke(JoystickId);  // Trigger the event
                }
            }
        }
    }

    public static void StopPolling()
    {
        _isRunning = false;
    }
}
