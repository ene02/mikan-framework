using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Mikan.Toolkit.Handlers;
using SDL2;
using static SDL2.SDL;

namespace Mikan.Toolkit.Window;

/// <summary>
/// Represents a window in an SDL-based application, providing functionality for creating, resizing, and managing window properties. <br/>
/// The class supports event handling for various user interactions and system events, such as mouse movements, keyboard input, and window resizing.<br/>
/// <br/>
/// This class encapsulates the creation and management of a window using SDL and provides various methods for adjusting window properties <br/>
/// like title, size, and fullscreen mode. It also exposes events for user input, lifecycle changes, and system notifications.<br/>
/// <br/>
/// Unlike traditional SDL event handling, which uses a central event loop and a large set of event types (e.g., SDL_Event), this class leverages <br/>
/// C# style events with `Action` delegates for more concise, type-safe, and flexible event handling.<br/> <br/> By using events such as `FingerDown`,
/// `KeyDown`, and `WindowResized`, you can easily subscribe and handle specific events without needing to manually check event types <br/>
/// and process event data. This modern approach reduces the complexity and boilerplate code typically associated with SDL event loops.
/// </summary>
public class Window
{
    /// <summary>
    /// Triggered when a finger touches the screen, including the following details:
    /// - The ID of the finger (a unique identifier for each touch).
    /// - The X coordinate of the touch on the screen.
    /// - The Y coordinate of the touch on the screen.
    /// - The pressure of the touch, ranging from 0 to 1.
    /// </summary>
    public event Action<long, float, float, float> FingerDown;


    /// <summary>
    /// Triggered when a finger is lifted off the screen, providing the following details:
    /// - The ID of the finger (a unique identifier for each touch).
    /// - The X coordinate of the touch release.
    /// - The Y coordinate of the touch release.
    /// - The pressure of the touch, ranging from 0 to 1.
    /// </summary>
    public event Action<long, float, float, float> FingerUp;

    /// <summary>
    /// Triggered when a finger moves across the screen, including the following details:
    /// - The ID of the finger (a unique identifier for each touch).
    /// - The X coordinate of the touch movement.
    /// - The Y coordinate of the touch movement.
    /// - The pressure of the touch, ranging from 0 to 1.
    /// </summary>
    public event Action<long, float, float, float> FingerMotion;


    /// <summary>
    /// Triggered when the application receives a quit request from the user or system.
    /// </summary>
    public event Action Quit;

    /// <summary>
    /// Triggered when the application is being terminated by the operating system.
    /// <para>This event is primarily relevant on mobile platforms like iOS and Android.</para>
    /// <para>On iOS, this corresponds to <c>applicationWillTerminate()</c>.</para>
    /// <para>On Android, this corresponds to <c>onDestroy()</c>.</para>
    /// </summary>
    public event Action AppTerminating;

    /// <summary>
    /// Triggered when the application is running low on memory.
    /// <para>Use this event to release unused memory or cache to prevent crashes.</para>
    /// <para>On iOS, this corresponds to <c>applicationDidReceiveMemoryWarning()</c>.</para>
    /// <para>On Android, this corresponds to <c>onLowMemory()</c>.</para>
    /// </summary>
    public event Action AppLowMemory;

    /// <summary>
    /// Triggered when the application is about to enter the background.
    /// <para>This event can be used to save the application state.</para>
    /// <para>On iOS, this corresponds to <c>applicationWillResignActive()</c>.</para>
    /// <para>On Android, this corresponds to <c>onPause()</c>.</para>
    /// </summary>
    public event Action AppWillEnterBackground;

    /// <summary>
    /// Triggered when the application has entered the background.
    /// <para>The application may not receive CPU time while in the background.</para>
    /// <para>On iOS, this corresponds to <c>applicationDidEnterBackground()</c>.</para>
    /// <para>On Android, this corresponds to <c>onPause()</c>.</para>
    /// </summary>
    public event Action AppDidEnterBackground;

    /// <summary>
    /// Triggered when the application is about to enter the foreground.
    /// <para>This event can be used to prepare the application for user interaction.</para>
    /// <para>On iOS, this corresponds to <c>applicationWillEnterForeground()</c>.</para>
    /// <para>On Android, this corresponds to <c>onResume()</c>.</para>
    /// </summary>
    public event Action AppWillEnterForeground;

    /// <summary>
    /// Triggered when the application has entered the foreground and is now interactive.
    /// <para>On iOS, this corresponds to <c>applicationDidBecomeActive()</c>.</para>
    /// <para>On Android, this corresponds to <c>onResume()</c>.</para>
    /// </summary>
    public event Action AppDidEnterForeground;

    /// <summary>
    /// Triggered when the user's locale preferences have changed.
    /// <para>For example, this could happen if the user changes the system language or region settings.</para>
    /// </summary>
    public event Action LocaleChanged;

    /// <summary>
    /// Triggered when a new audio device is added, providing the following details:
    /// - The ID of the newly added audio device.
    /// </summary>
    public event Action<uint> AudioDeviceAdded;

    /// <summary>
    /// Triggered when an audio device is removed, providing the following details:
    /// - The ID of the removed audio device.
    /// </summary>
    public event Action<uint> AudioDeviceRemoved;


    /// <summary>
    /// Triggered when the clipboard content is updated.
    /// </summary>
    public event Action ClipboardUpdated;

    /// <summary>
    /// Triggered when a file is dropped, providing the following details:
    /// - The path of the file that was dropped.
    /// </summary>
    public event Action<string> FileDropped;

    /// <summary>
    /// Triggered when a drag-and-drop operation begins.
    /// </summary>
    public event Action DropBegin;

    /// <summary>
    /// Triggered when a drag-and-drop operation is complete.
    /// </summary>
    public event Action DropComplete;

    /// <summary>
    /// Triggered when a game controller axis moves, providing the following details:
    /// - The ID of the game controller.
    /// - The value of the axis movement (e.g., -1 to 1 for a typical joystick axis).
    /// </summary>
    public event Action<int, int> ControllerAxisMotion;

    /// <summary>
    /// Triggered when a game controller button is pressed, providing the following details:
    /// - The ID of the game controller.
    /// - The index of the button pressed.
    /// </summary>
    public event Action<int, int> ControllerButtonDown;

    /// <summary>
    /// Triggered when a game controller button is released, providing the following details:
    /// - The ID of the game controller.
    /// - The index of the button released.
    /// </summary>
    public event Action<int, int> ControllerButtonUp;

    /// <summary>
    /// Triggered when a game controller is added to the system, providing the following details:
    /// - The ID of the newly added game controller.
    /// </summary>
    public event Action<int> ControllerDeviceAdded;

    /// <summary>
    /// Triggered when a game controller is removed from the system, providing the following details:
    /// - The ID of the removed game controller.
    /// </summary>
    public event Action<int> ControllerDeviceRemoved;

    /// <summary>
    /// Triggered when a game controller's mapping is updated, providing the following details:
    /// - The ID of the game controller whose mapping was updated.
    /// </summary>
    public event Action<int> ControllerDeviceRemapped;

    /// <summary>
    /// Triggered when a dollar gesture is detected, providing the following details:
    /// - The ID of the detected gesture.
    /// - The X coordinate of the gesture's origin.
    /// - The Y coordinate of the gesture's origin.
    /// - The distance of the gesture's movement.
    /// </summary>
    public event Action<float, float, float, uint, long> DollarGesture;

    /// <summary>
    /// Triggered when a multi-gesture is detected (such as pinch, rotate, etc.), providing the following details:
    /// - The X coordinate of the gesture.
    /// - The Y coordinate of the gesture.
    /// - The rotation angle of the gesture.
    /// - The distance of the gesture's movement.
    /// </summary>
    public event Action<float, float, float, float> MultiGesture;

    /// <summary>
    /// Triggered when a joystick axis moves, providing the following details:
    /// - The ID of the joystick.
    /// - The value of the axis movement (e.g., -1 to 1 for a typical joystick axis).
    /// </summary>
    public event Action<int, int> JoystickAxisMotion;

    /// <summary>
    /// Triggered when a joystick button is pressed, providing the following details:
    /// - The ID of the joystick.
    /// - The index of the button pressed.
    /// </summary>
    public event Action<int, int> JoystickButtonDown;

    /// <summary>
    /// Triggered when a joystick button is released, providing the following details:
    /// - The ID of the joystick.
    /// - The index of the button released.
    /// </summary>
    public event Action<int, int> JoystickButtonUp;

    /// <summary>
    /// Triggered when a joystick is added to the system, providing the following details:
    /// - The ID of the newly added joystick.
    /// </summary>
    public event Action<int> JoystickDeviceAdded;

    /// <summary>
    /// Triggered when a joystick is removed from the system, providing the following details:
    /// - The ID of the removed joystick.
    /// </summary>
    public event Action<int> JoystickDeviceRemoved;

    /// <summary>
    /// Triggered when a key is pressed down, providing the following details:
    /// - The key code of the key that was pressed.
    /// </summary>
    public event Action<SDL.SDL_Keycode> KeyDown;

    /// <summary>
    /// Triggered when a key is released, providing the following details:
    /// - The key code of the key that was released.
    /// </summary>
    public event Action<SDL.SDL_Keycode> KeyUp;

    /// <summary>
    /// Triggered when text is being composed, such as during input method editor (IME) usage, providing the following details:
    /// - The text currently being composed.
    /// </summary>
    public event Action<string> TextEditing;

    /// <summary>
    /// Triggered when finalized text input is received, providing the following details:
    /// - The finalized input text.
    /// </summary>
    public event Action<string> TextInput;


    /// <summary>
    /// Triggered when the keyboard layout or keymap changes due to a system event.
    /// <para>This event may occur when the user changes the input language or keyboard layout.</para>
    /// </summary>
    public event Action KeymapChanged;

    /// <summary>
    /// Triggered when the mouse is moved, providing the following details:
    /// - The X-coordinate of the mouse position.
    /// - The Y-coordinate of the mouse position.
    /// </summary>
    public event Action<int, int> MouseMotion;

    /// <summary>
    /// Triggered when a mouse button is pressed, providing the following details:
    /// - The button that was pressed (e.g., left, right, or middle).
    /// - The X-coordinate of the mouse position at the time of the press.
    /// - The Y-coordinate of the mouse position at the time of the press.
    /// </summary>
    public event Action<int, int, int> MouseButtonDown;

    /// <summary>
    /// Triggered when a mouse button is released, providing the following details:
    /// - The button that was released (e.g., left, right, or middle).
    /// - The X-coordinate of the mouse position at the time of the release.
    /// - The Y-coordinate of the mouse position at the time of the release.
    /// </summary>
    public event Action<int, int, int> MouseButtonUp;

    /// <summary>
    /// Triggered when the mouse wheel is scrolled, providing the following details:
    /// - The horizontal scroll amount (positive for right, negative for left).
    /// - The vertical scroll amount (positive for up, negative for down).
    /// </summary>
    public event Action<int, int> MouseWheel;


    /// <summary>
    /// Triggered when render targets are reset and their contents need to be updated.
    /// </summary>
    public event Action RenderTargetsReset;

    /// <summary>
    /// Triggered when the render device is reset, requiring recreation of textures.
    /// </summary>
    public event Action RenderDeviceReset;


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
    /// Triggered when the window is moved to a new position, providing the following details:
    /// - The new X-coordinate of the window.
    /// - The new Y-coordinate of the window.
    /// </summary>
    public event Action<int, int> Moved;

    /// <summary>
    /// Triggered when the window is exposed (restored from being covered by another window).
    /// </summary>
    public event Action Exposed;

    /// <summary>
    /// Triggered when the window is resized, providing the following details:
    /// - The new width of the window.
    /// - The new height of the window.
    /// </summary>
    public event Action<int, int> Resized;

    /// <summary>
    /// Triggered when the size of the window changes, which may be either a resize or other size-related event, providing the following details:
    /// - The new width of the window.
    /// - The new height of the window.
    /// </summary>
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
        InitHandler.CheckSDLInit();
    }

    /// <summary>
    /// Minimizes the window, making it invisible and reducing it to the taskbar.<br/>
    /// </summary>
    public void Minimize()
    {
        if (_windowHandler == 0)
            return;

        SDL_MinimizeWindow(_windowHandler);
        _isMinimized = true;
    }

    /// <summary>
    /// Maximizes the window, expanding it to fill the entire screen.<br/>
    /// </summary>
    public void Maximize()
    {
        if (_windowHandler == 0)
            return;

        SDL_MaximizeWindow(_windowHandler);
        _isMaximized = true;
    }

    /// <summary>
    /// Focuses the window, bringing it to the front and giving it input focus.<br/>
    /// </summary>
    public void Focus()
    {
        if (_windowHandler == 0)
            return;

        SDL_RaiseWindow(_windowHandler);
    }

    /// <summary>
    /// Restores the window to its normal state, undoing any minimization or maximization.<br/>
    /// </summary>
    public void Restore()
    {
        if (_windowHandler == 0)
            return;

        SDL_RestoreWindow(_windowHandler);
    }

    /// <summary>
    /// Sets whether the window can be resized by the user. <br/>If <paramref name="canResize"/> is true,
    /// the window will be resizable. Otherwise, it will be non-resizable.
    /// </summary>
    /// <param name="canResize">Indicates whether the window should be resizable. True to make it resizable, false to make it non-resizable.</param>
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

    /// <summary>
    /// Temporarily sets whether the window can be resized by the user.
    /// If <paramref name="canResize"/> is true, the window will be resizable<br/> but this setting may be reverted later.
    /// If <paramref name="canResize"/> is false, the window will not be resizable.
    /// </summary>
    /// <param name="canResize">Indicates whether the window should be temporarily resizable. True to make it resizable, false to make it non-resizable.</param>
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

    /// <summary>
    /// Closes the window, invokes the <see cref="Closed"/> event, and destroys the window handler.
    /// </summary>
    public void Close()
    {
        if (_windowHandler == 0)
            return;

        Closed?.Invoke();
        SDL_DestroyWindow(_windowHandler);
        _isWindowRunning = false;
    }

    /// <summary>
    /// Changes the position of the window to the specified X and Y coordinates.<br/>
    /// The X and Y values are clamped to a valid range (0 to <see cref="int.MaxValue"/>).
    /// </summary>
    /// <param name="x">The new X-coordinate for the window.</param>
    /// <param name="y">The new Y-coordinate for the window.</param>
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

    /// <summary>
    /// Changes the opacity of the window, with the <paramref name="alpha"/> value clamped between 0 and 1.<br/>
    /// A value of 0 makes the window fully transparent, and a value of 1 makes it fully opaque.<br/>
    /// </summary>
    /// <param name="alpha">The opacity of the window (0.0 to 1.0).</param>
    public void ChangeOpacity(float alpha)
    {
        if (_windowHandler == 0)
            return;

        alpha = Math.Clamp(alpha, 0, 1.0f); // Check for sillyness :p

        _currentOpacity = alpha;

        _ = SDL_SetWindowOpacity(_windowHandler, alpha);
    }

    /// <summary>
    /// Removes any size limitations on the window, allowing it to be resized to any size.<br/>
    /// This method sets the maximum window size to the desktop resolution and the minimum size to 0x0.
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

    /// <summary>
    /// Changes the window mode to the specified <paramref name="mode"/>. 
    /// It can switch between fullscreen, borderless, or windowed modes.<br/>
    /// </summary>
    /// <param name="mode">The desired window mode (e.g., fullscreen, borderless, or windowed).</param>
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

    /// <summary>
    /// Changes the window icon to the image located at the specified <paramref name="file"/> path.<br/>
    /// If the file does not exist the operation is ignored.
    /// </summary>
    /// <param name="file">The path to the image file to be used as the window icon.</param>
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

    /// <summary>
    /// Changes the size of the window to the specified <paramref name="width"/> and <paramref name="height"/>.<br/>
    /// The values are clamped to the current minimum and maximum size limits.
    /// </summary>
    /// <param name="width">The new width of the window.</param>
    /// <param name="height">The new height of the window.</param>
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

    /// <summary>
    /// Changes the window's title to the specified <paramref name="title"/>.
    /// </summary>
    /// <param name="title">The new title to be set for the window.</param>
    public void ChangeTitle(string title)
    {
        if (_windowHandler == 0)
            return;

        _title = title;

        SDL_SetWindowTitle(_windowHandler, title);
    }

    /// <summary>
    /// Sets the maximum size for the window.<br/> The window size will be clamped to this maximum when resized.
    /// </summary>
    /// <param name="maxWidth">The maximum width of the window.</param>
    /// <param name="maxHeight">The maximum height of the window.</param>
    public void SetMaximumSize(int maxWidth, int maxHeight)
    {
        maxWidth = Math.Clamp(maxWidth, 1, int.MaxValue);
        maxHeight = Math.Clamp(maxHeight, 1, int.MaxValue);

        _maxWidth = maxWidth;
        _maxHeight = maxHeight;

        SDL_SetWindowMaximumSize(_windowHandler, maxWidth, maxHeight);
        ChangeSize(_currentWidth, _currentHeight);
    }

    /// <summary>
    /// Sets the minimum size for the window.<br/> The window size will be clamped to this minimum when resized.
    /// </summary>
    /// <param name="minWidth">The minimum width of the window.</param>
    /// <param name="minHeight">The minimum height of the window.</param>
    public void SetMinimumSize(int minWidth, int minHeight)
    {
        minWidth = Math.Clamp(minWidth, 1, int.MaxValue);
        minHeight = Math.Clamp(minHeight, 1, int.MaxValue);

        _minHeight = minHeight;
        _minWidth = minWidth;

        _currentHeight = Math.Clamp(_currentHeight, _minHeight, _maxHeight);
        _currentWidth = Math.Clamp(_currentWidth, _minWidth, _maxWidth);

        SDL_SetWindowMinimumSize(_windowHandler, minWidth, minHeight);
        ChangeSize(_currentWidth, _currentHeight);
    }

    /// <summary>
    /// Initializes and shows the window with the specified parameters.<br/> The window will be created, and its event loop will start running until the window is closed.
    /// </summary>
    /// <param name="title">The title of the window.</param>
    /// <param name="width">The width of the window in pixels.</param>
    /// <param name="height">The height of the window in pixels.</param>
    /// <param name="windowFlags">Flags that define the window's behavior, such as resizable, fullscreen, etc.</param>
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
                if (!_isWindowRunning || e.type == SDL_EventType.SDL_QUIT)
                {
                    _isWindowRunning = false;
                    break;
                }

                if (e.type != SDL_EventType.SDL_WINDOWEVENT)
                {
                    switch (e.type)
                    {
                        // Touch events
                        case SDL_EventType.SDL_FINGERDOWN:
                            FingerDown?.Invoke(e.tfinger.fingerId, e.tfinger.x, e.tfinger.y, e.tfinger.pressure);
                            break;
                        case SDL_EventType.SDL_FINGERUP:
                            FingerUp?.Invoke(e.tfinger.fingerId, e.tfinger.x, e.tfinger.y, e.tfinger.pressure);
                            break;
                        case SDL_EventType.SDL_FINGERMOTION:
                            FingerMotion?.Invoke(e.tfinger.fingerId, e.tfinger.x, e.tfinger.y, e.tfinger.pressure);
                            break;

                        // App lifecycle events
                        case SDL_EventType.SDL_APP_TERMINATING:
                            AppTerminating?.Invoke();
                            break;
                        case SDL_EventType.SDL_APP_LOWMEMORY:
                            AppLowMemory?.Invoke();
                            break;
                        case SDL_EventType.SDL_APP_WILLENTERBACKGROUND:
                            AppWillEnterBackground?.Invoke();
                            break;
                        case SDL_EventType.SDL_APP_DIDENTERBACKGROUND:
                            AppDidEnterBackground?.Invoke();
                            break;
                        case SDL_EventType.SDL_APP_WILLENTERFOREGROUND:
                            AppWillEnterForeground?.Invoke();
                            break;
                        case SDL_EventType.SDL_APP_DIDENTERFOREGROUND:
                            AppDidEnterForeground?.Invoke();
                            break;

                        // Locale event
                        case SDL_EventType.SDL_LOCALECHANGED:
                            LocaleChanged?.Invoke();
                            break;

                        // Audio device events
                        case SDL_EventType.SDL_AUDIODEVICEADDED:
                            AudioDeviceAdded?.Invoke(e.adevice.which);
                            break;
                        case SDL_EventType.SDL_AUDIODEVICEREMOVED:
                            AudioDeviceRemoved?.Invoke(e.adevice.which);
                            break;

                        // Clipboard events
                        case SDL_EventType.SDL_CLIPBOARDUPDATE:
                            ClipboardUpdated?.Invoke();
                            break;

                        // File drop events
                        case SDL_EventType.SDL_DROPBEGIN:
                            DropBegin?.Invoke();
                            break;
                        case SDL_EventType.SDL_DROPCOMPLETE:
                            DropComplete?.Invoke();
                            break;
                        case SDL_EventType.SDL_DROPFILE:
                            string filePath = Marshal.PtrToStringAnsi(e.drop.file);
                            FileDropped?.Invoke(filePath);
                            break;

                        // Controller events
                        case SDL_EventType.SDL_CONTROLLERAXISMOTION:
                            ControllerAxisMotion?.Invoke(e.caxis.which, e.caxis.axisValue);
                            break;
                        case SDL_EventType.SDL_CONTROLLERBUTTONDOWN:
                            ControllerButtonDown?.Invoke(e.cbutton.which, e.cbutton.button);
                            break;
                        case SDL_EventType.SDL_CONTROLLERBUTTONUP:
                            ControllerButtonUp?.Invoke(e.cbutton.which, e.cbutton.button);
                            break;
                        case SDL_EventType.SDL_CONTROLLERDEVICEADDED:
                            ControllerDeviceAdded?.Invoke(e.cdevice.which);
                            break;
                        case SDL_EventType.SDL_CONTROLLERDEVICEREMOVED:
                            ControllerDeviceRemoved?.Invoke(e.cdevice.which);
                            break;
                        case SDL_EventType.SDL_CONTROLLERDEVICEREMAPPED:
                            ControllerDeviceRemapped?.Invoke(e.cdevice.which);
                            break;

                        // Gesture events
                        case SDL_EventType.SDL_DOLLARGESTURE:
                            DollarGesture?.Invoke(
                                e.dgesture.gestureId,
                                e.dgesture.x,
                                e.dgesture.y,
                                e.dgesture.numFingers,
                                e.dgesture.timestamp
                            );
                            break;
                        case SDL_EventType.SDL_MULTIGESTURE:
                            MultiGesture?.Invoke(
                                e.mgesture.x,
                                e.mgesture.y,
                                e.mgesture.dDist,
                                e.mgesture.dTheta
                            );
                            break;

                        // Joystick events
                        case SDL_EventType.SDL_JOYAXISMOTION:
                            JoystickAxisMotion?.Invoke(e.caxis.which, e.caxis.axisValue);
                            break;
                        case SDL_EventType.SDL_JOYBUTTONDOWN:
                            JoystickButtonDown?.Invoke(e.cbutton.which, e.cbutton.button);
                            break;
                        case SDL_EventType.SDL_JOYBUTTONUP:
                            JoystickButtonUp?.Invoke(e.cbutton.which, e.cbutton.button);
                            break;
                        case SDL_EventType.SDL_JOYDEVICEADDED:
                            JoystickDeviceAdded?.Invoke(e.cdevice.which);
                            break;
                        case SDL_EventType.SDL_JOYDEVICEREMOVED:
                            JoystickDeviceRemoved?.Invoke(e.cdevice.which);
                            break;

                        // Keyboard events
                        case SDL_EventType.SDL_KEYDOWN:
                            KeyDown?.Invoke(e.key.keysym.sym);
                            break;
                        case SDL_EventType.SDL_KEYUP:
                            KeyUp?.Invoke(e.key.keysym.sym);
                            break;

                        // Text events
                        case SDL_EventType.SDL_TEXTEDITING:
                            unsafe
                            {
                                string composedText = Encoding.UTF8.GetString(e.edit.text, e.edit.length);
                                TextEditing?.Invoke(composedText);
                            }
                            break;
                        case SDL_EventType.SDL_TEXTINPUT:
                            unsafe
                            {
                                byte* textPtr = e.text.text;
                                int length = 0;
                                while (textPtr[length] != 0) length++;
                                string inputText = Encoding.UTF8.GetString(textPtr, length);
                                TextInput?.Invoke(inputText);
                            }
                            break;

                        // Keymap change
                        case SDL_EventType.SDL_KEYMAPCHANGED:
                            KeymapChanged?.Invoke();
                            break;

                        // Render events
                        case SDL_EventType.SDL_RENDER_TARGETS_RESET:
                            RenderTargetsReset?.Invoke();
                            break;
                        case SDL_EventType.SDL_RENDER_DEVICE_RESET:
                            RenderDeviceReset?.Invoke();
                            break;

                        // Mouse events.
                        case SDL_EventType.SDL_MOUSEMOTION:
                            MouseMotion?.Invoke(e.motion.x, e.motion.y);
                            break;

                        case SDL_EventType.SDL_MOUSEBUTTONDOWN:
                            MouseButtonDown?.Invoke(e.button.button, e.button.x, e.button.y);
                            break;

                        case SDL_EventType.SDL_MOUSEBUTTONUP:
                            MouseButtonUp?.Invoke(e.button.button, e.button.x, e.button.y);
                            break;

                        case SDL_EventType.SDL_MOUSEWHEEL:
                            MouseWheel?.Invoke(e.wheel.x, e.wheel.y);
                            break;
                    }
                }
                else
                {
                    switch (e.window.windowEvent)
                    {
                        case SDL_WindowEventID.SDL_WINDOWEVENT_NONE:
                            None?.Invoke();
                            break;

                        case SDL_WindowEventID.SDL_WINDOWEVENT_SHOWN:
                            Shown?.Invoke();
                            break;

                        case SDL_WindowEventID.SDL_WINDOWEVENT_HIDDEN:
                            Hidden?.Invoke();
                            break;

                        case SDL_WindowEventID.SDL_WINDOWEVENT_MOVED:
                            Moved?.Invoke(e.window.data1, e.window.data2);
                            break;

                        case SDL_WindowEventID.SDL_WINDOWEVENT_EXPOSED:
                            Exposed?.Invoke();
                            break;

                        case SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED:
                            Resized?.Invoke(e.window.data1, e.window.data2);
                            break;

                        case SDL_WindowEventID.SDL_WINDOWEVENT_SIZE_CHANGED:
                            SDL_GetWindowSize(_windowHandler, out int w, out int h);
                            _currentWidth = w;
                            _currentHeight = h;
                            SizeChanged?.Invoke(w, h);
                            break;

                        case SDL_WindowEventID.SDL_WINDOWEVENT_MINIMIZED:
                            _isMinimized = true;
                            Minimized?.Invoke();
                            break;

                        case SDL_WindowEventID.SDL_WINDOWEVENT_MAXIMIZED:
                            _isMaximized = true;
                            Maximized?.Invoke();
                            break;

                        case SDL_WindowEventID.SDL_WINDOWEVENT_RESTORED:
                            if (_isMinimized)
                            {
                                _isMinimized = false;
                            }
                            else if (_isMaximized)
                            {
                                _isMaximized = false;
                            }
                            Restored?.Invoke();
                            break;

                        case SDL_WindowEventID.SDL_WINDOWEVENT_ENTER:
                            Enter?.Invoke();
                            break;

                        case SDL_WindowEventID.SDL_WINDOWEVENT_LEAVE:
                            Leave?.Invoke();
                            break;

                        case SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_GAINED:
                            _hasFocus = true;
                            FocusGained?.Invoke();
                            break;

                        case SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_LOST:
                            _hasFocus = false;
                            FocusLost?.Invoke();
                            break;

                        case SDL_WindowEventID.SDL_WINDOWEVENT_CLOSE:
                            Closed?.Invoke();
                            if (_isWindowRunning)
                                SDL_DestroyWindow(_windowHandler);
                            break;

                        case SDL_WindowEventID.SDL_WINDOWEVENT_TAKE_FOCUS:
                            TakeFocus?.Invoke();
                            break;

                        case SDL_WindowEventID.SDL_WINDOWEVENT_HIT_TEST:
                            HitTest?.Invoke();
                            break;

                        // Default case if no match found
                        default:
                            Console.WriteLine($"Unhandled window event: {e.window.windowEvent}");
                            break;
                    }
                }
            }
        }

        Console.WriteLine("Loop ended");
    }
}
