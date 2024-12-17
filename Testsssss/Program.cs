using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Mikan.Toolkit.Audio;
using Mikan.Toolkit.Handlers;
using Mikan.Toolkit.Window;

namespace Testsssss
{
    public class Program
    {
        private static PersistentPlayer _qplayer = new(AudioProcessor.Preset.Default);

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        static void Main(string[] args)
        {
            foreach (string arg in args)
            {
                if (arg.Contains("--show-console") || arg.Contains("-sc"))
                {
                    AllocConsole();
                }
            }

            _qplayer.AudioData = File.ReadAllBytes(@"C:\Users\kiwawa\Desktop\notch-tick.wav"); // File will always exist, this is a test

            Window window = new();

            window.MouseButtonDown += Window_MouseButtonDown;
            window.Quit += Window_Quit;

            window.ShowWindow("Hallo :D", 500, 500, SDL2.SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN | SDL2.SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE);
        }

        private static void Window_MouseButtonDown(int arg1, int arg2, int arg3)
        {
            throw new NotImplementedException();
        }

        private static void Window_Quit()
        {
            InitHandler.EndEverything(); // Dramatic.
        }
    }
}
