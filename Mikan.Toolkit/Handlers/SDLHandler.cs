using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;

namespace Mikan.Toolkit.Handlers;

public static class SDLHandler
{
    private static bool _isSDLInitialized = false;

    public static bool IsSDLInitialized => _isSDLInitialized;

    public static void CheckSDLInit()
    {
        if (_isSDLInitialized)
            return;

        // Initialize SDL.
        if (SDL_Init(SDL_INIT_VIDEO) < 0)
        {
            Debug.WriteLine($"[SDL] Could not initialize! SDL_Error: {SDL_GetError()}");
            return;
        }

        Debug.WriteLine($"[SDL] Initiated!");
        _isSDLInitialized = true;

        AppDomain.CurrentDomain.ProcessExit += (s, e) =>
        {
            EndSDL();
        };
    }

    public static void EndSDL()
    {
        if (_isSDLInitialized)
            SDL_Quit();
    }
}