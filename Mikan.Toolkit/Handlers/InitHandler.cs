using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagedBass;
using static SDL2.SDL;

namespace Mikan.Toolkit.Handlers;

public static class InitHandler
{
    private static bool _isBASSInitialized = false;

    private static bool _isSDLInitialized = false;

    public static bool IsBASSInitialized => _isBASSInitialized;

    public static bool IsSDLInitialized => _isSDLInitialized;

    public static void CheckSDLInit()
    {
        if (_isSDLInitialized)
            return;

        // Initialize SDL.
        if (SDL_Init(SDL_INIT_VIDEO | SDL_INIT_EVENTS) < 0)
        {
            Debug.WriteLine($"[SDL] Could not initialize! SDL_Error: {SDL_GetError()}");
            return;
        }

        Debug.WriteLine($"[SDL] Initiated!");
        Debug.WriteLine($"[SDL] Flags: SDL_INIT_VIDEO,  SDL_INIT_EVENTS!");
        _isSDLInitialized = true;

        AppDomain.CurrentDomain.ProcessExit += (s, e) =>
        {
            EndEverything();
        };
    }

    public static void EndSDL()
    {
        if (!_isSDLInitialized)
            return;

        SDL_Quit();
        _isSDLInitialized = false;

        Debug.WriteLine($"[SDL] Bye bye!");
    }

    public static void CheckBassInit()
    {
        if (!Bass.Init())
        {
            if (Bass.LastError == Errors.Already)
            {
                Debug.WriteLine($"[ManagedBass] BASS already initialized");
                return;
            }

            Debug.WriteLine($"[ManagedBass] Failed to initialize BASS: {Bass.LastError}");
        }

        Debug.WriteLine($"[ManagedBass] BASS initialized successfully");
        _isBASSInitialized = true;

        AppDomain.CurrentDomain.ProcessExit += (s, e) =>
        {
            EndEverything();
        };
    }

    public static void EndBass()
    {
        if (!_isBASSInitialized)
            return;

        Bass.Free();
        _isBASSInitialized = false;
        Debug.WriteLine($"[ManagedBass] BASS resources were freed");
    }

    public static void EndEverything()
    {
        EndBass();
        EndSDL();
    }
}