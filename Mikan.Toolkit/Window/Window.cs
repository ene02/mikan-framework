using System.Diagnostics;
using System.Runtime.InteropServices;
using SDL2;
using static SDL2.SDL;

namespace Mikan.Toolkit.Window;

public class Window
{
    private IntPtr _windowHandler = 0;
    private bool _isWindowRunning;

    // SDL Event: Window Close
    /// <summary>
    /// Triggered when the window is closed (SDL_QUIT).
    /// </summary>
    public event Action OnWindowClosed;

    // SDL Event: Mouse Button Down
    /// <summary>
    /// Triggered when a mouse button is pressed (SDL_MOUSEBUTTONDOWN).
    /// </summary>
    public EventHandler OnMouseButtonDown;

    // SDL Event: Mouse Button Up
    /// <summary>
    /// Triggered when a mouse button is released (SDL_MOUSEBUTTONUP).
    /// </summary>
    public event Action<int, int> OnMouseButtonUp;

    // SDL Event: Mouse Motion
    /// <summary>
    /// Triggered when the mouse is moved (SDL_MOUSEMOTION).
    /// </summary>
    public event Action<int, int> OnMouseMotion;

    // SDL Event: Key Pressed
    /// <summary>
    /// Triggered when a key is pressed down (SDL_KEYDOWN).
    /// </summary>
    public event Action<SDL.SDL_Keycode> OnKeyDown;

    // SDL Event: Key Released
    /// <summary>
    /// Triggered when a key is released (SDL_KEYUP).
    /// </summary>
    public event Action<SDL.SDL_Keycode> OnKeyUp;

    // SDL Event: Window Resize
    /// <summary>
    /// Triggered when the window is resized (SDL_WINDOWEVENT).
    /// </summary>
    public event Action<int, int> OnWindowResize;

    // SDL Event: Window Focus Gain
    /// <summary>
    /// Triggered when the window gains focus (SDL_WINDOWEVENT_FOCUS_GAINED).
    /// </summary>
    public event Action OnWindowFocusGained;

    // SDL Event: Window Focus Lost
    /// <summary>
    /// Triggered when the window loses focus (SDL_WINDOWEVENT_FOCUS_LOST).
    /// </summary>
    public event Action OnWindowFocusLost;

    // SDL Event: Drop File
    /// <summary>
    /// Triggered when a file is dropped onto the window (SDL_DROPFILE).
    /// </summary>
    public event Action<string> OnFileDropped;

    // SDL Event: App Terminating
    /// <summary>
    /// Triggered when the application is about to terminate (SDL_APP_TERMINATING).
    /// </summary>
    public event Action OnAppTerminating;

    public Window()
    {
        // Initialize SDL.
        if (SDL_Init(SDL_INIT_VIDEO) < 0)
        {
            Debug.WriteLine($"[SDL] Could not initialize! SDL_Error: {SDL_GetError()}");
            return;
        }

        Debug.WriteLine($"[SDL] Initiated!");

        AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
    }

    private void CurrentDomain_ProcessExit(object sender, EventArgs e)
    {
        if (_isWindowRunning)
            SDL.SDL_DestroyWindow(_windowHandler);

        // Quit SDL
        SDL.SDL_Quit();
    }

    public void ShowWindow(string title, Int32 width, Int32 height, SDL_WindowFlags windowFlags)
    {
        if (_windowHandler != 0)
            return;

        Debug.WriteLine($"[SDL] Making window with the next params: Title={title}, W={width}, H={height}");

        // Create a window.
        _windowHandler = SDL_CreateWindow(
            title,
            SDL_WINDOWPOS_CENTERED,
            SDL_WINDOWPOS_CENTERED,
            width,
            height,
            windowFlags
        );

        if (_windowHandler == IntPtr.Zero)
        {
            Debug.WriteLine($"[SDL] Window could not be created! SDL_Error: {SDL_GetError()}");
            SDL_Quit();
            return;
        }

        Debug.WriteLine("[SDL] SDL window was created sucessfully, event loop started!");
        _isWindowRunning = true;

        uint myWindowID = SDL.SDL_GetWindowID(_windowHandler);

        SDL_Event e;

        while (_isWindowRunning) // Keep running until we decide to stop
        {
            while (SDL.SDL_WaitEvent(out e) != 0)
            {
                if (e.type == SDL.SDL_EventType.SDL_QUIT)
                {
                    // Trigger the window close event
                    SDL.SDL_DestroyWindow(_windowHandler);
                    _isWindowRunning = false;
                    OnWindowClosed?.Invoke();
                    break;
                }
                else if (e.type == SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN)
                {
                    // Trigger mouse button down event
                    OnMouseButtonDown?.Invoke(this, EventArgs.Empty);
                    Debug.WriteLine("wawa");
                }
                else if (e.type == SDL.SDL_EventType.SDL_MOUSEBUTTONUP)
                {
                    // Trigger mouse button up event
                    OnMouseButtonUp?.Invoke(e.button.x, e.button.y);
                }
                else if (e.type == SDL.SDL_EventType.SDL_MOUSEMOTION)
                {
                    // Trigger mouse motion event
                    OnMouseMotion?.Invoke(e.motion.x, e.motion.y);
                }
                else if (e.type == SDL.SDL_EventType.SDL_KEYDOWN)
                {
                    // Trigger key down event
                    OnKeyDown?.Invoke(e.key.keysym.sym);
                }
                else if (e.type == SDL.SDL_EventType.SDL_KEYUP)
                {
                    // Trigger key up event
                    OnKeyUp?.Invoke(e.key.keysym.sym);
                }
                else if (e.type == SDL.SDL_EventType.SDL_WINDOWEVENT)
                {
                    // Handle window events
                    if (e.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_SIZE_CHANGED)
                    {
                        OnWindowResize?.Invoke(e.window.data1, e.window.data2);
                    }
                    else if (e.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_GAINED)
                    {
                        OnWindowFocusGained?.Invoke();
                    }
                    else if (e.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_LOST)
                    {
                        OnWindowFocusLost?.Invoke();
                    }
                }
                else if (e.type == SDL.SDL_EventType.SDL_DROPFILE)
                {
                    // Trigger file drop event
                    OnFileDropped?.Invoke(Marshal.PtrToStringAnsi(e.drop.file));
                }
                else if (e.type == SDL.SDL_EventType.SDL_APP_TERMINATING)
                {
                    // Trigger app termination event
                    OnAppTerminating?.Invoke();
                }
            }
        }
    }
}
