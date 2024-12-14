using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mikan.Toolkit.Window;
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

            Assert.IsTrue(!window.IsWindowCreated);

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
            Assert.IsTrue(window.CurrentMode == Window.Mode.Fullscreen);

            window.ChangeWindowMode(Window.Mode.Borderless);

            Thread.Sleep(1000);
            Assert.IsTrue(window.CurrentMode == Window.Mode.Borderless);

            window.ChangeWindowMode(Window.Mode.Windowed);

            Thread.Sleep(1000);
            Assert.IsTrue(window.CurrentMode == Window.Mode.Windowed);

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

            Assert.IsTrue(window.Height == 500 && window.Width == 500);

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
                    window.ShowWindow("Welcome to MSTest", 400, 400, SDL2.SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
                });
            }

            MakeWindow();

            Thread.Sleep(1000);

            window.SetMaximumSize(400, 400);

            Thread.Sleep(1000);

            window.ChangeSize(500, 500);

            Thread.Sleep(1000);

            Assert.IsTrue(window.Height == 400 && window.Width == 400);

            window.Close();
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

            Assert.IsTrue(window.Title == "This is another title");

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
                    window.ShowWindow("Welcome to MSTest", 400, 400, SDL2.SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
                });
            }

            MakeWindow();

            Thread.Sleep(1000);

            window.SetMinimumSize(400, 400);

            Thread.Sleep(1000);

            window.ChangeSize(300, 300);

            Thread.Sleep(1000);

            Assert.IsTrue(window.Height == 400 && window.Width == 400);

            window.Close();
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

            Assert.IsTrue(window.Height == 1000 && window.Width == 1000);

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

            Assert.IsTrue(window.Opacity == 0.5f);

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

            Assert.IsTrue(window.XPosition == 10 && window.YPosition == 10);

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

            Assert.IsTrue(window.Width == 600 && window.Height == 500);

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

            Assert.IsTrue(window.ImageData != 0);

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