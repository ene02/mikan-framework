using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagedBass;
using SDL2;
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
            throw new Exception($"[SDL] Could not initialize! SDL_Error: {SDL_GetError()}");
        }

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
    }

    public static void CheckBassInit()
    {
        if (!Bass.Init())
        {
            if (Bass.LastError == Errors.Already)
            {
                return;
            }

            throw new Exception($"[ManagedBass] Failed to initialize BASS: {Bass.LastError}");
        }

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
    }

    public static void EndEverything()
    {
        EndBass();
        EndSDL();
    }
}