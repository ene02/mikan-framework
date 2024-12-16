using System.Text;
using Mikan.Toolkit.Handlers;
using SDL2;
using static SDL2.SDL;

namespace Mikan.Toolkit.Events.Input;

/// <summary>
/// Provides events related to keyboard input, including key presses, text composition, and keymap changes.
/// </summary>
public static class KeyboardEvents
{
    /// <summary>
    /// Triggered when a key is pressed down.
    /// </summary>
    /// <param name="keycode">The key code of the key that was pressed.</param>
    public static event Action<SDL.SDL_Keycode> KeyDown;

    /// <summary>
    /// Triggered when a key is released.
    /// </summary>
    /// <param name="keycode">The key code of the key that was released.</param>
    public static event Action<SDL.SDL_Keycode> KeyUp;

    /// <summary>
    /// Triggered when text is being composed, such as during input method editor (IME) usage.
    /// <para>This event provides the current composition text while typing, but not the final input.</para>
    /// </summary>
    /// <param name="composedText">The text currently being composed.</param>
    public static event Action<string> TextEditing;

    /// <summary>
    /// Triggered when finalized text input is received.
    /// <para>Use this event to handle user-typed text for forms or chat systems.</para>
    /// </summary>
    /// <param name="inputText">The finalized input text.</param>
    public static event Action<string> TextInput;

    /// <summary>
    /// Triggered when the keyboard layout or keymap changes due to a system event.
    /// <para>This event may occur when the user changes the input language or keyboard layout.</para>
    /// </summary>
    public static event Action KeymapChanged;

    private static bool _isRunning;

    public static void StartPolling()
    {
        SDLHandler.CheckSDLInit();

        if (_isRunning)
            return;

        _isRunning = true;

        while (_isRunning) // Keep running until we decide to stop
        {
            while (SDL_WaitEvent(out SDL_Event e) != 0)
            {
                if (!_isRunning)
                {
                    break;
                }

                // Handle Keyboard Events
                if (e.type == SDL_EventType.SDL_KEYDOWN)
                {
                    SDL.SDL_Keycode Keycode = e.key.keysym.sym;  // Getting the keycode from the event
                    KeyDown?.Invoke(Keycode);  // Trigger the event
                }
                else if (e.type == SDL_EventType.SDL_KEYUP)
                {
                    SDL.SDL_Keycode Keycode = e.key.keysym.sym;  // Getting the keycode from the event
                    KeyUp?.Invoke(Keycode);  // Trigger the event
                }
                else if (e.type == SDL_EventType.SDL_TEXTEDITING)
                {
                    // Convert the byte* to string
                    unsafe
                    {
                        string ComposedText = Encoding.UTF8.GetString(e.edit.text, e.edit.length);
                        TextEditing?.Invoke(ComposedText);  // Trigger the event with the composed text
                    }
                }
                else if (e.type == SDL_EventType.SDL_TEXTINPUT)
                {
                    unsafe
                    {
                        // Get the pointer to the text (byte* type)
                        byte* textPtr = e.text.text;

                        // Calculate the length of the byte array by looking for the null terminator
                        int length = 0;
                        while (textPtr[length] != 0) // Look for the null terminator
                        {
                            length++;
                        }

                        // Convert the byte* to a string
                        string inputText = Encoding.UTF8.GetString(textPtr, length);

                        // Trigger the event with the final input text
                        TextInput?.Invoke(inputText);
                    }
                }
                else if (e.type == SDL_EventType.SDL_KEYMAPCHANGED)
                {
                    KeymapChanged?.Invoke();  // Trigger the event (no parameters)
                }
            }
        }
    }

    public static void StopPolling()
    {
        _isRunning = false;
    }

}
