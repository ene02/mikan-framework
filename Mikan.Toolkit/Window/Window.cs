using System.Diagnostics;
using System.Runtime.InteropServices;
using SDL2;
using static Mikan.Toolkit.Window.Window;
using static SDL2.SDL;

namespace Mikan.Toolkit.Window;

public class Window
{
    public event Action None;
    public event Action Shown;
    public event Action Hidden;
    public event Action<int, int> Moved;
    public event Action Exposed;
    public event Action<int, int> Resized;
    public event Action<int, int> SizeChanged;
    public event Action Minimized;
    public event Action Maximized;
    public event Action Restored;
    public event Action Enter;
    public event Action Leave;
    public event Action FocusGained;
    public event Action FocusLost;
    public event Action Closed;
    public event Action TakeFocus;
    public event Action HitTest;

    private nint _windowHandler = 0;
    private string _title;
    private bool _isWindowRunning = false, _resizable = false, _isMaximized = false, _isMinimized = false, _hasFocus = false;
    private int _maxHeight = int.MaxValue, _minHeight = 1, _maxWidth = int.MaxValue, _minWidth = 1, _currentHeight = 0, _currentWidth = 0, _xPos = 0, _yPos = 0;
    private float _currentOpacity = 1.0f;
    private Mode _currentWindowMode = Mode.Windowed;
    private nint _image = nint.Zero;

    public bool IsWindowCreated { get { return _isWindowRunning; } }

    public bool IsMinimized { get { return _isMinimized; } }

    public bool IsMaximized { get { return _isMaximized; } }

    public bool IsFocused { get { return _hasFocus; } }

    public Mode CurrentMode { get { return _currentWindowMode; } }

    public int Height { get { return _currentHeight; } }

    public int Width { get { return _currentWidth; } }

    public float Opacity { get { return _currentOpacity; } }

    public string Title { get { return _title; } }

    public int XPosition { get { return _xPos; } }

    public int YPosition { get { return _yPos; } }

    public nint ImageData { get { return _image; } }

    public enum Mode
    {
        Fullscreen,
        Borderless,
        Windowed
    }

    public enum State
    {
        Minimized,
        Maximized,
        Normal,
    }

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
        SDL.SDL_DestroyWindow(_windowHandler);
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

        maxWidth = Math.Clamp(maxWidth, 1, Int32.MaxValue);
        maxHeight = Math.Clamp(maxHeight, 1, Int32.MaxValue);

        _maxWidth = maxWidth;
        _maxHeight = maxHeight;

        SDL_SetWindowMaximumSize(_windowHandler, maxWidth, maxHeight);
        ChangeSize(_currentWidth, _currentHeight);
    }

    public void SetMinimumSize(int minWidth, int minHeight)
    {
        if (_windowHandler == 0)
            return;

        minWidth = Math.Clamp(minWidth, 1, Int32.MaxValue);
        minHeight = Math.Clamp(minHeight, 1, Int32.MaxValue);

        _minHeight = minHeight;
        _minWidth = minWidth;

        _currentHeight = Math.Clamp(_currentHeight, _minHeight, _maxHeight);
        _currentWidth = Math.Clamp(_currentWidth, _minWidth, _maxWidth);

        SDL_SetWindowMinimumSize(_windowHandler, minWidth, minHeight);
        ChangeSize(_currentWidth, _currentHeight);
    }

    public void ShowWindow(string title, Int32 width, Int32 height, SDL_WindowFlags windowFlags)
    {
        if (_windowHandler != 0)
            return;

        width = Math.Clamp(width, 1, Int32.MaxValue);
        height = Math.Clamp(height, 1, Int32.MaxValue);

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

        if (_windowHandler == IntPtr.Zero)
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
            while (SDL.SDL_WaitEvent(out SDL_Event e) != 0)
            {
                if (e.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_NONE)
                {
                    None?.Invoke();
                }
                else if (e.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_SHOWN)
                {
                    Shown?.Invoke();
                }
                else if (e.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_HIDDEN)
                {
                    Hidden?.Invoke();
                }
                else if (e.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_MOVED)
                {
                    Moved?.Invoke(e.window.data1, e.window.data2);
                }
                else if (e.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_EXPOSED)
                {
                    Exposed?.Invoke();
                }
                else if (e.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED)
                {
                    Resized?.Invoke(e.window.data1, e.window.data2);
                }
                else if (e.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_SIZE_CHANGED)
                {
                    SDL.SDL_GetWindowSize(_windowHandler, out int w, out int h);
                    _currentWidth = w;
                    _currentHeight = h;
                    SizeChanged?.Invoke(w, h);
                }
                else if (e.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_MINIMIZED)
                {
                    _isMinimized = true;
                    Minimized?.Invoke();
                }
                else if (e.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_MAXIMIZED)
                {
                    _isMaximized = true;
                    Maximized?.Invoke();
                }
                else if (e.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_RESTORED)
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
                else if (e.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_ENTER)
                {
                    Enter?.Invoke();
                }
                else if (e.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_LEAVE)
                {
                    Leave?.Invoke();
                }
                else if (e.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_GAINED)
                {
                    _hasFocus = true;
                    FocusGained?.Invoke();
                }
                else if (e.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_LOST)
                {
                    _hasFocus = false;
                    FocusLost?.Invoke();
                }
                else if (e.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_CLOSE)
                {
                    Closed?.Invoke();
                }
                else if (e.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_TAKE_FOCUS)
                {
                    TakeFocus?.Invoke();
                }
                else if (e.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_HIT_TEST)
                {
                    HitTest?.Invoke();
                }
            }
        }
    }
}
