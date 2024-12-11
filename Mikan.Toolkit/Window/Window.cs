using System.Diagnostics;
using SDL2;
using static SDL2.SDL;

namespace Mikan.Toolkit.Window
{
    public class Window
    {
        private IntPtr _windowHandler = 0;
        private bool _isWindowRunning;

        // SDL Event: Window Close
        /// <summary>
        /// Triggered when the window is closed (SDL_QUIT).
        /// </summary>
        public static event Action OnWindowClosed;

        // SDL Event: Mouse Button Down
        /// <summary>
        /// Triggered when a mouse button is pressed (SDL_MOUSEBUTTONDOWN).
        /// </summary>
        public static event Action<int, int> OnMouseButtonDown;

        // SDL Event: Mouse Button Up
        /// <summary>
        /// Triggered when a mouse button is released (SDL_MOUSEBUTTONUP).
        /// </summary>
        public static event Action<int, int> OnMouseButtonUp;

        // SDL Event: Mouse Motion
        /// <summary>
        /// Triggered when the mouse is moved (SDL_MOUSEMOTION).
        /// </summary>
        public static event Action<int, int> OnMouseMotion;

        // SDL Event: Key Pressed
        /// <summary>
        /// Triggered when a key is pressed down (SDL_KEYDOWN).
        /// </summary>
        public static event Action<SDL.SDL_Keycode> OnKeyDown;

        // SDL Event: Key Released
        /// <summary>
        /// Triggered when a key is released (SDL_KEYUP).
        /// </summary>
        public static event Action<SDL.SDL_Keycode> OnKeyUp;

        // SDL Event: Window Resize
        /// <summary>
        /// Triggered when the window is resized (SDL_WINDOWEVENT).
        /// </summary>
        public static event Action<int, int> OnWindowResize;

        // SDL Event: Window Focus Gain
        /// <summary>
        /// Triggered when the window gains focus (SDL_WINDOWEVENT_FOCUS_GAINED).
        /// </summary>
        public static event Action OnWindowFocusGained;

        // SDL Event: Window Focus Lost
        /// <summary>
        /// Triggered when the window loses focus (SDL_WINDOWEVENT_FOCUS_LOST).
        /// </summary>
        public static event Action OnWindowFocusLost;

        // SDL Event: Drop File
        /// <summary>
        /// Triggered when a file is dropped onto the window (SDL_DROPFILE).
        /// </summary>
        public static event Action<string> OnFileDropped;

        // SDL Event: App Terminating
        /// <summary>
        /// Triggered when the application is about to terminate (SDL_APP_TERMINATING).
        /// </summary>
        public static event Action OnAppTerminating;

        public Window()
        {
            // Initialize SDL.
            if (SDL_Init(SDL_INIT_VIDEO) < 0)
            {
                Debug.WriteLine($"[SDL] Could not initialize! SDL_Error: {SDL_GetError()}");
                return;
            }

            Debug.WriteLine($"[SDL] Initiated!");
        }

        public void ShowWindow(SDL_WindowFlags windowFlags, string title, Int32 width = 200, Int32 height = 200)
        {
            if (_windowHandler != 0)
                return;

            SDL_Event e;

            Debug.WriteLine($"[SDL] Making window with the next params: Title={title}, W={width}, H={height}");

            // Create a window.
            IntPtr window = SDL_CreateWindow(
                title,
                SDL_WINDOWPOS_CENTERED,
                SDL_WINDOWPOS_CENTERED,
                width,
                height,
                windowFlags
            );

            if (window == IntPtr.Zero)
            {
                Debug.WriteLine($"[SDL] Window could not be created! SDL_Error: {SDL_GetError()}");
                SDL_Quit();
                return;
            }

            Debug.WriteLine("[SDL] SDL window was created sucessfully, event loop started!");
            _isWindowRunning = true;

            while (_isWindowRunning) // Keep running until we decide to stop
            {
                // Poll events
                while (SDL_PollEvent(out e) != 0) // Continuously process events
                {
                    switch (e.type)
                    {
                        case SDL_EventType.SDL_MOUSEBUTTONDOWN:
                            Console.WriteLine($"Mouse button pressed at: {e.button.x}, {e.button.y}");
                            _isWindowRunning = false; // Stop the loop
                            break;
                        default:
                            break;
                    }
                }
            }

            // Clean up and quit SDL
            SDL.SDL_DestroyWindow(window);
            SDL.SDL_Quit();
        }
    }
}
