using System.Diagnostics;
using System.Runtime.InteropServices;
using Mikan.Toolkit.Handlers;
using SDL2;
using static Mikan.Toolkit.Window.Window;
using static SDL2.SDL;

namespace Mikan.Toolkit.Window;

public class Window
{
    /// <summary>
    /// Triggered when the window is in an idle state (no event has occurred).
    /// </summary>
    public event Action None;

    /// <summary>
    /// Triggered when the window is shown (made visible).
    /// </summary>
    public event Action Shown;

    /// <summary>
    /// Triggered when the window is hidden (made invisible).
    /// </summary>
    public event Action Hidden;

    /// <summary>
    /// Triggered when the window is moved to a new position.
    /// </summary>
    /// <param name="x">The new X-coordinate of the window.</param>
    /// <param name="y">The new Y-coordinate of the window.</param>
    public event Action<int, int> Moved;

    /// <summary>
    /// Triggered when the window is exposed (restored from being covered by another window).
    /// </summary>
    public event Action Exposed;

    /// <summary>
    /// Triggered when the window is resized.
    /// </summary>
    /// <param name="width">The new width of the window.</param>
    /// <param name="height">The new height of the window.</param>
    public event Action<int, int> Resized;

    /// <summary>
    /// Triggered when the size of the window changes, which may be either a resize or other size-related event.
    /// </summary>
    /// <param name="width">The new width of the window.</param>
    /// <param name="height">The new height of the window.</param>
    public event Action<int, int> SizeChanged;

    /// <summary>
    /// Triggered when the window is minimized (typically by the user or the system).
    /// </summary>
    public event Action Minimized;

    /// <summary>
    /// Triggered when the window is maximized (typically by the user or the system).
    /// </summary>
    public event Action Maximized;

    /// <summary>
    /// Triggered when the window is restored from a minimized or maximized state.
    /// </summary>
    public event Action Restored;

    /// <summary>
    /// Triggered when the cursor enters the window area.
    /// </summary>
    public event Action Enter;

    /// <summary>
    /// Triggered when the cursor leaves the window area.
    /// </summary>
    public event Action Leave;

    /// <summary>
    /// Triggered when the window gains focus (becomes the active window).
    /// </summary>
    public event Action FocusGained;

    /// <summary>
    /// Triggered when the window loses focus (becomes inactive).
    /// </summary>
    public event Action FocusLost;

    /// <summary>
    /// Triggered when the window is closed (either by the user or system).
    /// </summary>
    public event Action Closed;

    /// <summary>
    /// Triggered when the window is ready to take focus, but it hasn't yet.
    /// </summary>
    public event Action TakeFocus;

    /// <summary>
    /// Triggered when a hit-test event occurs on the window (e.g., checking if a specific point in the window is interactable).
    /// </summary>
    public event Action HitTest;

    // Private fields for the window state
    private nint _windowHandler = 0;
    private string _title;
    private bool _isWindowRunning = false, _resizable = false, _isMaximized = false, _isMinimized = false, _hasFocus = false;
    private int _maxHeight = int.MaxValue, _minHeight = 1, _maxWidth = int.MaxValue, _minWidth = 1, _currentHeight = 0, _currentWidth = 0, _xPos = 0, _yPos = 0;
    private float _currentOpacity = 1.0f;
    private Mode _currentWindowMode = Mode.Windowed;
    private nint _image = nint.Zero;

    // Public properties
    /// <summary>
    /// Gets whether the window is created and running.
    /// </summary>
    public bool IsWindowCreated { get { return _isWindowRunning; } }

    /// <summary>
    /// Gets whether the window is minimized.
    /// </summary>
    public bool IsMinimized { get { return _isMinimized; } }

    /// <summary>
    /// Gets whether the window is maximized.
    /// </summary>
    public bool IsMaximized { get { return _isMaximized; } }

    /// <summary>
    /// Gets whether the window currently has focus.
    /// </summary>
    public bool IsFocused { get { return _hasFocus; } }

    /// <summary>
    /// Gets the current window mode (Fullscreen, Borderless, Windowed).
    /// </summary>
    public Mode CurrentMode { get { return _currentWindowMode; } }

    /// <summary>
    /// Gets the current height of the window.
    /// </summary>
    public int Height { get { return _currentHeight; } }

    /// <summary>
    /// Gets the current width of the window.
    /// </summary>
    public int Width { get { return _currentWidth; } }

    /// <summary>
    /// Gets the current opacity of the window (between 0 and 1).
    /// </summary>
    public float Opacity { get { return _currentOpacity; } }

    /// <summary>
    /// Gets the title of the window.
    /// </summary>
    public string Title { get { return _title; } }

    /// <summary>
    /// Gets the current X-position of the window.
    /// </summary>
    public int XPosition { get { return _xPos; } }

    /// <summary>
    /// Gets the current Y-position of the window.
    /// </summary>
    public int YPosition { get { return _yPos; } }

    /// <summary>
    /// Gets the image data associated with the window.
    /// </summary>
    public nint ImageData { get { return _image; } }

    // Enum to represent window modes
    public enum Mode
    {
        /// <summary>Full-screen window mode.</summary>
        Fullscreen,

        /// <summary>Borderless window mode.</summary>
        Borderless,

        /// <summary>Normal windowed mode.</summary>
        Windowed
    }

    // Enum to represent window states
    public enum State
    {
        /// <summary>Window is minimized.</summary>
        Minimized,

        /// <summary>Window is maximized.</summary>
        Maximized,

        /// <summary>Window is in its normal state.</summary>
        Normal,
    }


    public Window()
    {
        SDLHandler.CheckSDLInit();
    }

    public void Minimize()
    {
        if (_windowHandler == 0)
            return;

        SDL_MinimizeWindow(_windowHandler);
        _isMinimized = true;
    }

    public void Maximize()
    {
        if (_windowHandler == 0)
            return;

        SDL_MaximizeWindow(_windowHandler);
        _isMaximized = true;
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

    public void SetResizable(bool canResize)
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

    public void Close()
    {
        if (_windowHandler == 0)
            return;

        Closed?.Invoke();
        SDL_DestroyWindow(_windowHandler);
        _isWindowRunning = false;
    }

    public void ChangePosition(int x, int y)
    {
        if (_windowHandler == 0)
            return;

        x = Math.Clamp(x, 0, int.MaxValue);
        y = Math.Clamp(y, 0, int.MaxValue);

        _xPos = x;
        _yPos = y;

        SDL_SetWindowPosition(_windowHandler, x, y);
    }

    public void ChangeOpacity(float alpha)
    {
        if (_windowHandler == 0)
            return;

        alpha = Math.Clamp(alpha, 0, 1.0f); // Check for sillyness :p

        _currentOpacity = alpha;

        _ = SDL_SetWindowOpacity(_windowHandler, alpha);
    }

    /// <summary>
    /// Makes the window have no size limits.
    /// </summary>
    public void UnboundWindow()
    {
        if (SDL_GetDesktopDisplayMode(0, out SDL_DisplayMode desktopMode) != 0)
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
                    break;
                };
                _currentWindowMode = mode;
                break;
            case Mode.Borderless:
                UnboundWindow();

                if (!_resizable)
                    TempResizable(true);

                if (SDL_SetWindowFullscreen(_windowHandler, 0x00001000) != 0)
                {
                    Debug.WriteLine($"[SDL] Could not set window to borderless!: {SDL_GetError()}");
                    break;
                };
                _currentWindowMode = mode;
                break;
            case Mode.Windowed:
                SetMaximumSize(_maxWidth, _maxHeight);
                SetMinimumSize(_minWidth, _minHeight);

                if (SDL_SetWindowFullscreen(_windowHandler, 0) != 0)
                {
                    Debug.WriteLine($"[SDL] Could not set window to windowed!: {SDL_GetError()}");
                    break;
                };

                _currentWindowMode = mode;

                SetResizable(_resizable);

                ChangeSize(_currentWidth, _currentHeight);
                break;
            default:
                break;
        }
    }

    public void ChangeIcon(string file)
    {
        if (_windowHandler == 0 || !File.Exists(file))
            return;

        _image = SDL_LoadBMP(file);

        if (_image == 0)
        {
            Debug.WriteLine($"[SDL] Failed to load image!: {SDL_GetError()}");
        }

        SDL_SetWindowIcon(_windowHandler, _image);
    }

    public void ChangeSize(int width, int height)
    {
        if (_windowHandler == 0)
            return;

        width = Math.Clamp(width, _minWidth, _maxWidth);
        height = Math.Clamp(height, _minHeight, _maxHeight);

        _currentHeight = height;
        _currentWidth = width;

        Debug.WriteLine($"{height}, {width}");

        SDL_SetWindowSize(_windowHandler, width, height);
    }

    public void ChangeTitle(string title)
    {
        if (_windowHandler == 0)
            return;

        _title = title;

        SDL_SetWindowTitle(_windowHandler, title);
    }

    public void SetMaximumSize(int maxWidth, int maxHeight)
    {
        if (_windowHandler == 0)
            return;

        maxWidth = Math.Clamp(maxWidth, 1, int.MaxValue);
        maxHeight = Math.Clamp(maxHeight, 1, int.MaxValue);

        _maxWidth = maxWidth;
        _maxHeight = maxHeight;

        SDL_SetWindowMaximumSize(_windowHandler, maxWidth, maxHeight);
        ChangeSize(_currentWidth, _currentHeight);
    }

    public void SetMinimumSize(int minWidth, int minHeight)
    {
        if (_windowHandler == 0)
            return;

        minWidth = Math.Clamp(minWidth, 1, int.MaxValue);
        minHeight = Math.Clamp(minHeight, 1, int.MaxValue);

        _minHeight = minHeight;
        _minWidth = minWidth;

        _currentHeight = Math.Clamp(_currentHeight, _minHeight, _maxHeight);
        _currentWidth = Math.Clamp(_currentWidth, _minWidth, _maxWidth);

        SDL_SetWindowMinimumSize(_windowHandler, minWidth, minHeight);
        ChangeSize(_currentWidth, _currentHeight);
    }

    public void ShowWindow(string title, int width, int height, SDL_WindowFlags windowFlags)
    {
        if (_windowHandler != 0)
            return;

        width = Math.Clamp(width, 1, int.MaxValue);
        height = Math.Clamp(height, 1, int.MaxValue);

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

        if (windowFlags.HasFlag(SDL_WindowFlags.SDL_WINDOW_FULLSCREEN))
        {
            _currentWindowMode = Mode.Fullscreen;
        }
        else if (windowFlags.HasFlag(SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP))
        {
            _currentWindowMode = Mode.Borderless;
        }
        else
        {
            _currentWindowMode = Mode.Windowed;
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

        if (_windowHandler == nint.Zero)
        {
            Debug.WriteLine($"[SDL] Window could not be created! SDL_Error: {SDL_GetError()}");
            SDL_Quit();
            return;
        }

        _currentHeight = height;
        _currentWidth = width;

        SDL_GetWindowPosition(_windowHandler, out int x, out int y);

        _xPos = x;
        _yPos = y;

        Debug.WriteLine("[SDL] SDL window was created sucessfully, event loop started!");
        _isWindowRunning = true;

        while (_isWindowRunning) // Keep running until we decide to stop
        {
            while (SDL_WaitEvent(out SDL_Event e) != 0)
            {
                if (e.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_NONE)
                {
                    None?.Invoke();
                }
                else if (e.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_SHOWN)
                {
                    Shown?.Invoke();
                }
                else if (e.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_HIDDEN)
                {
                    Hidden?.Invoke();
                }
                else if (e.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_MOVED)
                {
                    Moved?.Invoke(e.window.data1, e.window.data2);
                }
                else if (e.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_EXPOSED)
                {
                    Exposed?.Invoke();
                }
                else if (e.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED)
                {
                    Resized?.Invoke(e.window.data1, e.window.data2);
                }
                else if (e.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_SIZE_CHANGED)
                {
                    SDL_GetWindowSize(_windowHandler, out int w, out int h);
                    _currentWidth = w;
                    _currentHeight = h;
                    SizeChanged?.Invoke(w, h);
                }
                else if (e.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_MINIMIZED)
                {
                    _isMinimized = true;
                    Minimized?.Invoke();
                }
                else if (e.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_MAXIMIZED)
                {
                    _isMaximized = true;
                    Maximized?.Invoke();
                }
                else if (e.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_RESTORED)
                {
                    if (_isMinimized)
                    {
                        _isMinimized = false;
                    }
                    else if (_isMaximized)
                    {
                        _isMaximized = false;
                    }
                    Restored?.Invoke();
                }
                else if (e.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_ENTER)
                {
                    Enter?.Invoke();
                }
                else if (e.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_LEAVE)
                {
                    Leave?.Invoke();
                }
                else if (e.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_GAINED)
                {
                    _hasFocus = true;
                    FocusGained?.Invoke();
                }
                else if (e.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_LOST)
                {
                    _hasFocus = false;
                    FocusLost?.Invoke();
                }
                else if (e.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_CLOSE)
                {
                    Closed?.Invoke();

                    if (_isWindowRunning)
                        SDL_DestroyWindow(_windowHandler);
                }
                else if (e.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_TAKE_FOCUS)
                {
                    TakeFocus?.Invoke();
                }
                else if (e.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_HIT_TEST)
                {
                    HitTest?.Invoke();
                }
            }
        }
    }
}
