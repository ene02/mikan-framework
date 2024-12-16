using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mikan.Toolkit.Handlers;
using SDL2;
using static SDL2.SDL;

namespace Mikan.Toolkit.Events.Program;

/// <summary>
/// Provides application-level events, including lifecycle events
/// and notifications about memory or locale changes.
/// </summary>
public static class AppEvents
{
    /// <summary>
    /// Triggered when the application receives a quit request from the user or system.
    /// </summary>
    public static event Action Quit;

    /// <summary>
    /// Triggered when the application is being terminated by the operating system.
    /// <para>This event is primarily relevant on mobile platforms like iOS and Android.</para>
    /// <para>On iOS, this corresponds to <c>applicationWillTerminate()</c>.</para>
    /// <para>On Android, this corresponds to <c>onDestroy()</c>.</para>
    /// </summary>
    public static event Action AppTerminating;

    /// <summary>
    /// Triggered when the application is running low on memory.
    /// <para>Use this event to release unused memory or cache to prevent crashes.</para>
    /// <para>On iOS, this corresponds to <c>applicationDidReceiveMemoryWarning()</c>.</para>
    /// <para>On Android, this corresponds to <c>onLowMemory()</c>.</para>
    /// </summary>
    public static event Action AppLowMemory;

    /// <summary>
    /// Triggered when the application is about to enter the background.
    /// <para>This event can be used to save the application state.</para>
    /// <para>On iOS, this corresponds to <c>applicationWillResignActive()</c>.</para>
    /// <para>On Android, this corresponds to <c>onPause()</c>.</para>
    /// </summary>
    public static event Action AppWillEnterBackground;

    /// <summary>
    /// Triggered when the application has entered the background.
    /// <para>The application may not receive CPU time while in the background.</para>
    /// <para>On iOS, this corresponds to <c>applicationDidEnterBackground()</c>.</para>
    /// <para>On Android, this corresponds to <c>onPause()</c>.</para>
    /// </summary>
    public static event Action AppDidEnterBackground;

    /// <summary>
    /// Triggered when the application is about to enter the foreground.
    /// <para>This event can be used to prepare the application for user interaction.</para>
    /// <para>On iOS, this corresponds to <c>applicationWillEnterForeground()</c>.</para>
    /// <para>On Android, this corresponds to <c>onResume()</c>.</para>
    /// </summary>
    public static event Action AppWillEnterForeground;

    /// <summary>
    /// Triggered when the application has entered the foreground and is now interactive.
    /// <para>On iOS, this corresponds to <c>applicationDidBecomeActive()</c>.</para>
    /// <para>On Android, this corresponds to <c>onResume()</c>.</para>
    /// </summary>
    public static event Action AppDidEnterForeground;

    /// <summary>
    /// Triggered when the user's locale preferences have changed.
    /// <para>For example, this could happen if the user changes the system language or region settings.</para>
    /// </summary>
    public static event Action LocaleChanged;

    private static bool _isRunning = false;

    public static void StopPolling()
    {
        _isRunning = false;
    }

    public static void StartPolling()
    {
        SDLHandler.CheckSDLInit();

        if (!_isRunning)
            _isRunning = true;

        while (_isRunning) // Keep running until we decide to stop
        {
            while (SDL_WaitEvent(out SDL_Event e) != 0)
            {
                if (!_isRunning)
                {
                    break;
                }

                // Handle Application Events
                if (e.type == SDL_EventType.SDL_APP_TERMINATING)
                {
                    AppTerminating?.Invoke();
                }
                else if (e.type == SDL_EventType.SDL_APP_LOWMEMORY)
                {
                    AppLowMemory?.Invoke();
                }
                else if (e.type == SDL_EventType.SDL_APP_WILLENTERBACKGROUND)
                {
                    AppWillEnterBackground?.Invoke();
                }
                else if (e.type == SDL_EventType.SDL_APP_DIDENTERBACKGROUND)
                {
                    AppDidEnterBackground?.Invoke();
                }
                else if (e.type == SDL_EventType.SDL_APP_WILLENTERFOREGROUND)
                {
                    AppWillEnterForeground?.Invoke();
                }
                else if (e.type == SDL_EventType.SDL_APP_DIDENTERFOREGROUND)
                {
                    AppDidEnterForeground?.Invoke();
                }
                else if (e.type == SDL_EventType.SDL_LOCALECHANGED)
                {
                    LocaleChanged?.Invoke();
                }
            }
        }

    }
}

