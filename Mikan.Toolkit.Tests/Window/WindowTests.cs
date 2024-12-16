using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mikan.Toolkit.Window.Tests
{
    [TestClass()]
    public class WindowTests
    {
        [TestMethod()]
        public void NegativeSizeShowWindowTest()
        {
            Window window = new();

            async void MakeWindow()
            {
                await Task.Run(() =>
                {
                    window.ShowWindow("Goodbye to MSTest", -100, -100, SDL2.SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
                });
            }

            MakeWindow();

            Thread.Sleep(2000);

            Assert.IsTrue(window.IsWindowCreated);

            window.Close();
        }

        [TestMethod()]
        public void ChangeWindowModeTest()
        {
            Window window = new();

            async void MakeWindow()
            {
                await Task.Run(() =>
                {
                    window.ShowWindow("Welcome to MSTest", 400, 400, SDL2.SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN | SDL2.SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE);
                });
            }

            MakeWindow();

            Thread.Sleep(1000);

            window.ChangeWindowMode(Window.Mode.Fullscreen);

            Thread.Sleep(1000);
            Assert.AreEqual(Window.Mode.Fullscreen, window.CurrentMode);

            window.ChangeWindowMode(Window.Mode.Borderless);

            Thread.Sleep(1000);
            Assert.AreEqual(Window.Mode.Borderless, window.CurrentMode);

            window.ChangeWindowMode(Window.Mode.Windowed);

            Thread.Sleep(1000);
            Assert.AreEqual(Window.Mode.Windowed, window.CurrentMode);

            window.Close();
        }

        [TestMethod()]
        public void UnboundWindowTest()
        {
            Window window = new();

            async void MakeWindow()
            {
                await Task.Run(() =>
                {
                    window.ShowWindow("Welcome to MSTest", 400, 400, SDL2.SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
                });
            }

            MakeWindow();

            Thread.Sleep(1000);

            window.SetMaximumSize(400, 400);

            window.UnboundWindow();

            window.ChangeSize(500, 500);

            Thread.Sleep(1000);

            Assert.AreEqual(500, window.Height);
            Assert.AreEqual(500, window.Width);

            window.Close();
        }

        [TestMethod()]
        public void MinimizeTest()
        {
            Window window = new();

            async void MakeWindow()
            {
                await Task.Run(() =>
                {
                    window.ShowWindow("Welcome to MSTest", 400, 400, SDL2.SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
                });
            }

            MakeWindow();

            Thread.Sleep(1000);

            window.Minimize();

            Thread.Sleep(1000);

            Assert.IsTrue(window.IsMinimized);

            window.Close();
        }

        [TestMethod()]
        public void MaximizeTest()
        {
            Window window = new();

            async void MakeWindow()
            {
                await Task.Run(() =>
                {
                    window.ShowWindow("Welcome to MSTest", 400, 400, SDL2.SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
                });
            }

            MakeWindow();

            Thread.Sleep(1000);

            window.Maximize();

            Thread.Sleep(1000);

            Assert.IsTrue(window.IsMaximized);

            window.Close();
        }

        [TestMethod()]
        public void SetMaximumSizeTest()
        {
            Window window = new();

            async void MakeWindow()
            {
                await Task.Run(() =>
                {
                    window.ShowWindow("Welcome to MSTest", 400, 400, SDL2.SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE | SDL2.SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
                });
            }

            MakeWindow();

            Thread.Sleep(1000);

            window.SetMaximumSize(400, 400);

            Thread.Sleep(1000);

            window.ChangeSize(600, 600);

            Thread.Sleep(1000);

            Assert.AreEqual(400, window.Height);
            Assert.AreEqual(400, window.Width);

            window.Close();

            Thread.Sleep(1000);

            // Set size before maximum.
            Window window2 = new();

            async void MakeWindow2()
            {
                await Task.Run(() =>
                {
                    window2.ShowWindow("Welcome to MSTest part 2 electric boogaloo", 400, 400, SDL2.SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE | SDL2.SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
                });
            }

            MakeWindow2();

            Thread.Sleep(1000);

            window2.ChangeSize(1000, 1000);

            Thread.Sleep(1000);

            Assert.AreEqual(1000, window2.Height);
            Assert.AreEqual(1000, window2.Width);

            Thread.Sleep(1000);

            window2.SetMaximumSize(300, 300);

            Thread.Sleep(1000);

            Assert.AreEqual(300, window2.Height);
            Assert.AreEqual(300, window2.Width);

            window2.Close();
        }

        [TestMethod()]
        public void ChangeTitleTest()
        {
            Window window = new();

            async void MakeWindow()
            {
                await Task.Run(() =>
                {
                    window.ShowWindow("Welcome to MSTest", 400, 400, SDL2.SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
                });
            }

            MakeWindow();

            Thread.Sleep(1000);

            window.ChangeTitle("This is another title");

            Thread.Sleep(1000);

            Assert.AreEqual("This is another title", window.Title);

            window.Close();
        }

        [TestMethod()]
        public void SetMinimumSizeTest()
        {
            Window window = new();

            async void MakeWindow()
            {
                await Task.Run(() =>
                {
                    window.ShowWindow("Welcome to MSTest", 500, 500, SDL2.SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE | SDL2.SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
                });
            }

            MakeWindow();

            Thread.Sleep(1000);

            window.SetMinimumSize(400, 400);

            Thread.Sleep(1000);

            window.ChangeSize(300, 300);

            Thread.Sleep(1000);

            Assert.AreEqual(400, window.Height);
            Assert.AreEqual(400, window.Width);

            Thread.Sleep(1000);

            window.Close();

            // Set size before minimum.
            Window window2 = new();

            async void MakeWindow2()
            {
                await Task.Run(() =>
                {
                    window2.ShowWindow("Welcome to MSTest", 400, 400, SDL2.SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE | SDL2.SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
                });
            }

            MakeWindow2();

            Thread.Sleep(1000);

            window2.ChangeSize(300, 300);

            Thread.Sleep(1000);

            Assert.AreEqual(300, window2.Height);
            Assert.AreEqual(300, window2.Width);

            Thread.Sleep(1000);

            window2.SetMinimumSize(400, 400);

            Thread.Sleep(1000);

            Assert.AreEqual(400, window2.Height);
            Assert.AreEqual(400, window2.Width);

            window2.Close();
        }

        [TestMethod()]
        public void ChangeSizeTest()
        {
            Window window = new();

            async void MakeWindow()
            {
                await Task.Run(() =>
                {
                    window.ShowWindow("Welcome to MSTest", 400, 400, SDL2.SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE);
                });
            }

            MakeWindow();

            Thread.Sleep(1000);

            window.ChangeSize(1000, 1000);

            Thread.Sleep(1000);

            Assert.AreEqual(1000, window.Height);
            Assert.AreEqual(1000, window.Width);

            Thread.Sleep(1000);

            window.ChangeSize(-1, -1);

            Thread.Sleep(1000);

            Assert.AreEqual(1, window.Height);
            Assert.AreEqual(1, window.Width);

            Thread.Sleep(1000);

            window.Close();
        }

        [TestMethod()]
        public void ChangeOpacityTest()
        {
            Window window = new();

            async void MakeWindow()
            {
                await Task.Run(() =>
                {
                    window.ShowWindow("Welcome to MSTest", 400, 400, SDL2.SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE);
                });
            }

            MakeWindow();

            Thread.Sleep(1000);

            window.ChangeOpacity(0.5f);

            Thread.Sleep(1000);

            Assert.AreEqual(0.5f, window.Opacity);

            Thread.Sleep(1000);

            window.ChangeOpacity(11111f);

            Thread.Sleep(1000);

            Assert.AreEqual(1f, window.Opacity);

            Thread.Sleep(1000);

            window.ChangeOpacity(-1f);

            Thread.Sleep(1000);

            Assert.AreEqual(0, window.Opacity);

            window.Close();
        }

        [TestMethod()]
        public void ChangePositionTest()
        {
            Window window = new();

            async void MakeWindow()
            {
                await Task.Run(() =>
                {
                    window.ShowWindow("Welcome to MSTest", 400, 400, SDL2.SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE);
                });
            }

            MakeWindow();

            Thread.Sleep(1000);

            window.ChangePosition(10, 10);

            Thread.Sleep(1000);

            Assert.AreEqual(10, window.XPosition);
            Assert.AreEqual(10, window.YPosition);

            Thread.Sleep(1000);

            window.ChangePosition(-1, -1);

            Thread.Sleep(1000);

            Assert.AreEqual(0, window.XPosition);
            Assert.AreEqual(0, window.YPosition);

            window.Close();
        }

        [TestMethod()]
        public void SetResizableTest()
        {
            Window window = new();

            async void MakeWindow()
            {
                await Task.Run(() =>
                {
                    window.ShowWindow("Welcome to MSTest", 400, 400, SDL2.SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN); // Not resizable.
                });
            }

            MakeWindow();

            Thread.Sleep(1000);

            window.SetResizable(true);

            window.ChangeSize(600, 500);

            Thread.Sleep(1000);

            Assert.AreEqual(600, window.Width);
            Assert.AreEqual(500, window.Height);

            window.Close();
        }

        [TestMethod()]
        public void RestoreTest()
        {
            Window window = new();
            bool restored = false;

            window.OnWindowRestored += Window_OnWindowRestored;

            void Window_OnWindowRestored()
            {
                restored = true;
            }

            async void MakeWindow()
            {
                await Task.Run(() =>
                {
                    window.ShowWindow("Welcome to MSTest", 400, 400, SDL2.SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
                });
            }

            MakeWindow();

            Thread.Sleep(1000);

            window.Minimize();

            Thread.Sleep(1000);

            window.Restore();

            Thread.Sleep(1000);

            Assert.IsTrue(restored);

            window.Close();
        }

        [TestMethod()]
        public void ChangeIconTest()
        {
            Window window = new();

            async void MakeWindow()
            {
                await Task.Run(() =>
                {
                    window.ShowWindow("Welcome to MSTest", 400, 400, SDL2.SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
                });
            }

            MakeWindow();

            Thread.Sleep(1000);

            window.ChangeIcon(@"little-sampling-ico.bmp");

            Thread.Sleep(1000);

            Assert.AreNotEqual(0, window.ImageData);

            window.Close();
        }

        [TestMethod()]
        public void ShowWindowTest()
        {
            Window window = new();

            async void MakeWindow()
            {
                await Task.Run(() =>
                {
                    window.ShowWindow("Welcome to MSTest", 400, 400, SDL2.SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
                });
            }

            MakeWindow();

            Thread.Sleep(2000);

            Assert.IsTrue(window.IsWindowCreated);

            window.Close();
        }
    }
}