using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mikan.Toolkit.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Mikan.Toolkit.Audio.Tests
{
    [TestClass()]
    public class QuickPlayerTests
    {
        [TestMethod()]
        public void GetHandlerTest()
        {
            QuickPlayer player = new(AudioProcessor.Preset.Default);

            player.Play("the-builder.mp3");

            Thread.Sleep(1000);

            Assert.AreNotEqual(0, player.GetHandler());

            player.Stop();

            Assert.AreSame(0, player.GetHandler());
        }

        [TestMethod()]
        public void PlayTest()
        {
            QuickPlayer player = new(AudioProcessor.Preset.Default);

            player.Play("the-builder.mp3");

            Thread.Sleep(3000);

            Assert.IsTrue(player.IsPlaying);

            player.Stop();
        }

        [TestMethod()]
        public void StopTest()
        {
            QuickPlayer player = new(AudioProcessor.Preset.Default);

            player.Play("the-builder.mp3");

            Thread.Sleep(1000);

            player.Stop();

            Thread.Sleep(1000);

            Assert.IsFalse(player.IsPlaying);
        }

        [TestMethod()]
        public void PauseTest()
        {
            QuickPlayer player = new(AudioProcessor.Preset.Default);

            player.Play("the-builder.mp3");

            Thread.Sleep(1000);

            player.Pause();

            Thread.Sleep(1000);

            Assert.IsFalse(player.IsPlaying);

            player.Stop();
        }

        [TestMethod()]
        public void ResumeTest()
        {
            QuickPlayer player = new(AudioProcessor.Preset.Default);

            player.Play("the-builder.mp3");

            Thread.Sleep(1000);

            player.Pause();

            Thread.Sleep(1000);

            player.Resume();

            Thread.Sleep(1000);

            Assert.IsTrue(player.IsPlaying);

            player.Stop();
        }
    }
}