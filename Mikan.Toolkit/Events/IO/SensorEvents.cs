using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Mikan.Toolkit.Handlers;
using static SDL2.SDL;

namespace Mikan.Toolkit.Events.IO;

/// <summary>
/// Provides events related to sensor updates, such as when a sensor value is changed.
/// </summary>
public static class SensorEvents
{
    /// <summary>
    /// Triggered when a sensor value is updated.
    /// </summary>
    /// <param name="sensorId">The ID of the updated sensor.</param>
    /// <param name="data">The updated data from the sensor (e.g., temperature, motion, etc.).</param>
    public static event Action<int, float[]> SensorUpdated;

    private static bool _isRunning;

    /// <summary>
    /// This does nothing.
    /// </summary>
    public static void StartPolling()
    {
        // === This doesnt work properly because sensors can have different data lenghts, so i just decided to implement nothing at all because who needs sensors anyways. ===

        //SDLHandler.CheckSDLInit();

        //if (_isRunning)
        //    return;  // Prevent multiple polling loops from running at the same time

        //_isRunning = true;

        //while (_isRunning) // Keep running until we decide to stop
        //{
        //    while (SDL_WaitEvent(out SDL_Event e) != 0)
        //    {
        //        if (!_isRunning)
        //        {
        //            break;
        //        }

        //        // Handle Sensor Events
        //        if (e.type == SDL_EventType.SDL_SENSORUPDATE)
        //        {
        //            int sensorId = e.sensor.which;  // Get the sensor ID from the event

        //            int sensorDataLength = ???; // Sensor data lenght needed for the array limit.

        //            // Create a managed float array with the length of the sensor data
        //            float[] sensorData = new float[sensorDataLength];

        //            unsafe
        //            {
        //                // Copy the unmanaged float* data to the managed float[] using Marshal.Copy
        //                Marshal.Copy((IntPtr)e.sensor.data, sensorData, 0, sensorDataLength);
        //            }

        //            // Trigger the event
        //            SensorUpdated?.Invoke(sensorId, sensorData);
        //        }
        //    }
        //}
    }

    public static void StopPolling()
    {
        _isRunning = false;  // Stop the polling loop
    }
}
