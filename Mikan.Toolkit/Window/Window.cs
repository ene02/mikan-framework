using System.Diagnostics;
using System.Runtime.InteropServices;
using SDL2;
using static SDL2.SDL;

namespace Mikan.Toolkit.Window;

public class Window
{
    private IntPtr _windowHandler = 0;
    private bool _isWindowRunning;
    private int _maxHeight = -1, _minHeight = -1, _maxWidth = -1, _minWidth = -1;
    private int _currentHeight = -1, _currentWidth = -1;
    private bool _resizable = false;

    public enum Mode
    {
        Fullscreen,
        Borderless,
        Windowed
    }

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
        if (SDLChecker.IsSDLInitialized)
            return;

        // Initialize SDL.
        if (SDL_Init(SDL_INIT_VIDEO) < 0)
        {
            Debug.WriteLine($"[SDL] Could not initialize! SDL_Error: {SDL_GetError()}");
            return;
        }

        Debug.WriteLine($"[SDL] Initiated!");
        SDLChecker.IsSDLInitialized = true;
        AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
    }

    private void CurrentDomain_ProcessExit(object sender, EventArgs e)
    {
        if (_isWindowRunning)
            SDL.SDL_DestroyWindow(_windowHandler);

        // Quit SDL
        SDL.SDL_Quit();
    }

    public void Minimize()
    {
        if (_windowHandler == 0)
            return;

        SDL_MinimizeWindow(_windowHandler);
    }

    public void Maximize()
    {
        if (_windowHandler == 0)
            return;

        SDL_MaximizeWindow(_windowHandler);
    }

    public void Focus()
    {
        if (_windowHandler == 0)
            return;

        SDL_RaiseWindow(_windowHandler);
    }

    public void Restore()
    {
        if (_windowHandler == 0)
            return;

        SDL_RestoreWindow(_windowHandler);
    }

    public void Resizable(bool canResize)
    {
        if (_windowHandler == 0)
            return;

        if (canResize)
        {
            SDL_SetWindowResizable(_windowHandler, SDL_bool.SDL_TRUE);
            _resizable = true;
            Debug.WriteLine("[SDL] Window is permanently resizable");
        }
        else
        {
            SDL_SetWindowResizable(_windowHandler, SDL_bool.SDL_FALSE);
            _resizable = false;
            Debug.WriteLine("[SDL] Window is permanently not resizable");
        }
    }

    private void TempResizable(bool canResize)
    {
        if (_windowHandler == 0)
            return;

        if (canResize)
        {
            SDL_SetWindowResizable(_windowHandler, SDL_bool.SDL_TRUE);
            Debug.WriteLine("[SDL] Window is temporarily resizable");
        }
        else
        {
            SDL_SetWindowResizable(_windowHandler, SDL_bool.SDL_FALSE);
            Debug.WriteLine("[SDL] Window is temporarily not resizable");
        }
    }

    public void ChangePosition(int x, int y)
    {
        if (_windowHandler == 0)
            return;

        SDL_SetWindowPosition(_windowHandler, x, y);
    }

    public void ChangeOpacity(float alpha)
    {
        if (_windowHandler == 0 && alpha < 0)
            return;

        if (alpha > 1.0f) // Check for silyness.
            alpha = 1.0f;

        SDL_SetWindowOpacity(_windowHandler, alpha);
    }

    /// <summary>
    /// Makes the window have no size limits.
    /// </summary>
    public void UnboundWindow()
    {
        SDL_DisplayMode desktopMode;

        if (SDL_GetDesktopDisplayMode(0, out desktopMode) != 0)
        {
            Debug.WriteLine($"[SDL] Could not get desktop resolution!: {SDL_GetError()}");
            return;
        }

        SetMaximumSize(desktopMode.w, desktopMode.h);
        SetMinimumSize(0, 0);
    }

    public void ChangeWindowMode(Mode mode)
    {
        if (_windowHandler == 0)
            return;

        switch (mode)
        {
            case Mode.Fullscreen:
                UnboundWindow();

                if (!_resizable)
                    TempResizable(true);

                if (SDL_SetWindowFullscreen(_windowHandler, 0x00000001) != 0)
                {
                    Debug.WriteLine($"[SDL] Could not set window to fullscreen!: {SDL_GetError()}");
                };
                break;
            case Mode.Borderless:
                UnboundWindow();

                if (!_resizable)
                    TempResizable(true);

                if (SDL_SetWindowFullscreen(_windowHandler, 0x00001000) != 0)
                {
                    Debug.WriteLine($"[SDL] Could not set window to borderless!: {SDL_GetError()}");
                };
                break;
            case Mode.Windowed:
                if (_maxWidth != -1 && _maxHeight != -1)
                {
                    SetMaximumSize(_maxWidth, _maxHeight);
                }

                if (_minWidth != -1 && _minHeight != -1)
                {
                    SetMinimumSize(_minWidth, _minHeight);
                }

                if (SDL_SetWindowFullscreen(_windowHandler, 0) != 0)
                {
                    Debug.WriteLine($"[SDL] Could not set window to windowed!: {SDL_GetError()}");
                };

                Resizable(_resizable);

                ChangeSize(_currentWidth, _currentHeight);
                break;
            default:
                break;
        }
    }

    public void ChangeIcon(string file)
    {
        if (_windowHandler == 0)
            return;

        if (SDL_LoadBMP(file) == 0)
        {
            Debug.WriteLine($"[SDL] Failed to load image!: {SDL_GetError()}");
        }

        SDL_SetWindowIcon(_windowHandler, SDL_LoadBMP(file));
    }

    public void ChangeSize(int width, int height)
    {
        if (_windowHandler == 0 && width > 0 && height > 0)
            return;

        _currentHeight = height;
        _currentWidth = width;

        SDL_SetWindowSize(_windowHandler, width, height);
    }

    public void ChangeTitle(string title)
    {
        SDL_SetWindowTitle(_windowHandler, title);
    }

    public void SetMaximumSize(int maxWidth, int maxHeight)
    {
        if (_windowHandler == 0 && maxWidth > 0 && maxHeight > 0)
            return;

        SDL_SetWindowMaximumSize(_windowHandler, maxWidth, maxHeight);
    }

    public void SetMinimumSize(int minWidth, int minHeight)
    {
        if (_windowHandler == 0 && minWidth > 0 && minHeight > 0)
            return;

        SDL_SetWindowMinimumSize(_windowHandler, minWidth, minHeight);
    }

    public void ShowWindow(string title, Int32 width, Int32 height, SDL_WindowFlags windowFlags)
    {
        if (_windowHandler != 0)
            return;

        Debug.WriteLine($"[SDL] Making window with the next params: Title={title}, W={width}, H={height}, Flags={windowFlags}");

        if (windowFlags.HasFlag(SDL_WindowFlags.SDL_WINDOW_RESIZABLE))
        {
            _resizable = true;
            Debug.WriteLine("[SDL] Window is initiatited as resizable");
        }
        else
        {
            Debug.WriteLine("[SDL] Window is initiatited as not resizable");
        }

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

        _currentHeight = height;
        _currentWidth = width;

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
