using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mikan.Toolkit.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mikan.Toolkit.Audio.Tests
{
    [TestClass()]
    public class PersistentPlayerTests
    {
        [TestMethod()]
        public void PlayTest()
        {
            PersistentPlayer player = new(AudioProcessor.Preset.Default);

            player.Play("the-builder.mp3");

            Thread.Sleep(3000);

            Assert.IsTrue(player.IsPlaying);

            player.Clear();
        }

        [TestMethod()]
        public void StopTest()
        {
            PersistentPlayer player = new(AudioProcessor.Preset.Default);

            player.Play("the-builder.mp3");

            Thread.Sleep(1000);

            player.Stop();

            Thread.Sleep(1000);

            Assert.IsTrue(!player.IsPlaying);
        }

        [TestMethod()]
        public void PauseTest()
        {
            PersistentPlayer player = new(AudioProcessor.Preset.Default);

            player.Play("the-builder.mp3");

            Thread.Sleep(1000);

            player.Pause();

            Thread.Sleep(1000);

            Assert.IsTrue(!player.IsPlaying);
        }

        [TestMethod()]
        public void ResumeTest()
        {
            PersistentPlayer player = new(AudioProcessor.Preset.Default);

            player.Play("the-builder.mp3");

            Thread.Sleep(1000);

            player.Pause();

            Thread.Sleep(1000);

            player.Resume();

            Thread.Sleep(1000);

            Assert.IsTrue(player.IsPlaying);
        }

        [TestMethod()]
        public void ClearTest()
        {
            PersistentPlayer player = new(AudioProcessor.Preset.Default);

            player.Play("the-builder.mp3");

            Thread.Sleep(1000);

            player.Clear();

            Assert.IsTrue(player.AudioData == Array.Empty<byte>());
        }

        [TestMethod()]
        public void GetHandlerTest()
        {
            PersistentPlayer player = new(AudioProcessor.Preset.Default);

            player.Play("the-builder.mp3");

            Thread.Sleep(1000);

            Assert.IsTrue(player.GetHandler() != 0);

            player.Clear();

            Assert.IsTrue(player.GetHandler() == 0);
        }
    }
}