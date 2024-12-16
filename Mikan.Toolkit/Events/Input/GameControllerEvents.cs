using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mikan.Toolkit.Handlers;
using static SDL2.SDL;

namespace Mikan.Toolkit.Events.Input;

/// <summary>
/// Provides events related to game controller input, including axis movements, button presses, and device additions/removals.
/// </summary>
public static class GameControllerEvents
{
    /// <summary>
    /// Triggered when a game controller axis moves.
    /// </summary>
    /// <param name="controllerId">The ID of the game controller.</param>
    /// <param name="axisValue">The value of the axis movement (e.g., -1 to 1 for a typical joystick axis).</param>
    public static event Action<int, int> ControllerAxisMotion;

    /// <summary>
    /// Triggered when a game controller button is pressed.
    /// </summary>
    /// <param name="controllerId">The ID of the game controller.</param>
    /// <param name="buttonIndex">The index of the button pressed.</param>
    public static event Action<int, int> ControllerButtonDown;

    /// <summary>
    /// Triggered when a game controller button is released.
    /// </summary>
    /// <param name="controllerId">The ID of the game controller.</param>
    /// <param name="buttonIndex">The index of the button released.</param>
    public static event Action<int, int> ControllerButtonUp;

    /// <summary>
    /// Triggered when a game controller is added to the system.
    /// </summary>
    /// <param name="controllerId">The ID of the newly added game controller.</param>
    public static event Action<int> ControllerDeviceAdded;

    /// <summary>
    /// Triggered when a game controller is removed from the system.
    /// </summary>
    /// <param name="controllerId">The ID of the removed game controller.</param>
    public static event Action<int> ControllerDeviceRemoved;

    /// <summary>
    /// Triggered when a game controller's mapping is updated.
    /// </summary>
    /// <param name="controllerId">The ID of the game controller whose mapping was updated.</param>
    public static event Action<int> ControllerDeviceRemapped;

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

        while (_isRunning)
        {
            while (SDL_WaitEvent(out SDL_Event e) != 0)
            {
                if (!_isRunning)
                {
                    break;
                }

                if (e.type == SDL_EventType.SDL_CONTROLLERAXISMOTION)
                {
                    int ControllerId = e.caxis.which;        // Getting the controller ID from the event
                    int AxisValue = e.caxis.axisValue;       // Getting the axis value from the event
                    ControllerAxisMotion?.Invoke(ControllerId, AxisValue);  // Trigger the event
                }
                else if (e.type == SDL_EventType.SDL_CONTROLLERBUTTONDOWN)
                {
                    int ControllerId = e.cbutton.which;      // Getting the controller ID from the event
                    int ButtonIndex = e.cbutton.button;      // Getting the button index from the event
                    ControllerButtonDown?.Invoke(ControllerId, ButtonIndex);  // Trigger the event
                }
                else if (e.type == SDL_EventType.SDL_CONTROLLERBUTTONUP)
                {
                    int ControllerId = e.cbutton.which;      // Getting the controller ID from the event
                    int ButtonIndex = e.cbutton.button;      // Getting the button index from the event
                    ControllerButtonUp?.Invoke(ControllerId, ButtonIndex);  // Trigger the event
                }
                else if (e.type == SDL_EventType.SDL_CONTROLLERDEVICEADDED)
                {
                    int ControllerId = e.cdevice.which;      // Getting the controller ID from the event
                    ControllerDeviceAdded?.Invoke(ControllerId);  // Trigger the event
                }
                else if (e.type == SDL_EventType.SDL_CONTROLLERDEVICEREMOVED)
                {
                    int ControllerId = e.cdevice.which;      // Getting the controller ID from the event
                    ControllerDeviceRemoved?.Invoke(ControllerId);  // Trigger the event
                }
                else if (e.type == SDL_EventType.SDL_CONTROLLERDEVICEREMAPPED)
                {
                    int ControllerId = e.cdevice.which;      // Getting the controller ID from the event
                    ControllerDeviceRemapped?.Invoke(ControllerId);  // Trigger the event
                }
            }
        }
    }

}
