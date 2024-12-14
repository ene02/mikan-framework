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
    public class SoundAttributesTests
    {
        [TestMethod()]
        public void GetAudioLenghtTest()
        {
            PersistentPlayer player = new(AudioProcessor.Preset.Default);

            player.Play("the-builder.mp3");

            Thread.Sleep(1000);

            Assert.AreEqual(118, Math.Round(player.GetAudioLenght()));

            player.Clear();
        }

        [TestMethod()]
        public void GetVolumeTest()
        {
            PersistentPlayer player = new(AudioProcessor.Preset.Default);

            player.Play("the-builder.mp3");

            Thread.Sleep(1000);

            Assert.AreEqual(1, player.GetVolume());

            player.Clear();
        }

        [TestMethod()]
        public void GetPanningTest()
        {
            PersistentPlayer player = new(AudioProcessor.Preset.Default);

            player.Play("the-builder.mp3");

            Thread.Sleep(1000);

            Assert.AreEqual(0, player.GetPanning());

            player.Clear();
        }

        [TestMethod()]
        public void GetSpeedTest()
        {
            PersistentPlayer player = new(AudioProcessor.Preset.Default);

            player.Play("the-builder.mp3");

            Thread.Sleep(1000);

            Assert.AreEqual(0, player.GetSpeed());

            player.Clear();
        }

        [TestMethod()]
        public void GetPitchTest()
        {
            PersistentPlayer player = new(AudioProcessor.Preset.Default);

            player.Play("the-builder.mp3");

            Thread.Sleep(1000);

            Assert.AreEqual(0, player.GetPitch());

            player.Clear();
        }

        [TestMethod()]
        public void GetPositionTest()
        {
            PersistentPlayer player = new(AudioProcessor.Preset.Default);

            player.Play("the-builder.mp3");

            Thread.Sleep(1000);

            Assert.AreNotEqual(0, player.GetPosition());

            player.Clear();
        }

        [TestMethod()]
        public void SetPositionInBytesTest()
        {
            PersistentPlayer player = new(AudioProcessor.Preset.Default);

            player.Play("the-builder.mp3");

            Thread.Sleep(1000);

            player.Pause();

            player.SetPositionInBytes(6.0f);

            Assert.AreEqual(6.0f, player.GetPosition());

            player.Clear();
        }

        [TestMethod()]
        public void SetVolumeTest()
        {
            PersistentPlayer player = new(AudioProcessor.Preset.Default);

            player.Play("the-builder.mp3");

            Thread.Sleep(1000);

            player.SetVolume(0.5f);

            Thread.Sleep(1000);

            Assert.AreEqual(0.5f, player.GetVolume());

            player.Clear();
        }

        [TestMethod()]
        public void SetPanningTest()
        {
            PersistentPlayer player = new(AudioProcessor.Preset.Default);

            player.Play("the-builder.mp3");

            Thread.Sleep(1000);

            player.SetPanning(1.0f);

            Thread.Sleep(1000);

            Assert.AreEqual(1.0f, player.GetPanning());

            player.Clear();
        }

        [TestMethod()]
        public void SetSpeedTest()
        {
            PersistentPlayer player = new(AudioProcessor.Preset.Default);

            player.Play("the-builder.mp3");

            Thread.Sleep(1000);

            player.SetSpeed(100f);

            Thread.Sleep(1000);

            Assert.AreEqual(100f, player.GetSpeed());

            player.Clear();
        }

        [TestMethod()]
        public void SetPitchTest()
        {
            PersistentPlayer player = new(AudioProcessor.Preset.Default);

            player.Play("the-builder.mp3");

            Thread.Sleep(1000);

            player.SetPitch(6f);

            Thread.Sleep(1000);

            Assert.AreEqual(6f, player.GetPitch());

            player.Clear();
        }

        [TestMethod()]
        public void SetAttributesToDefaultTest()
        {
            PersistentPlayer player = new(AudioProcessor.Preset.Default);

            player.Play("the-builder.mp3");

            Thread.Sleep(1000);

            player.SetVolume(0.5f);
            player.SetPanning(1.0f);
            player.SetSpeed(100f);
            player.SetPitch(6f);

            Thread.Sleep(1000);

            player.SetAttributesToDefault();

            Thread.Sleep(1000);

            Assert.AreEqual(1f, player.GetVolume());
            Assert.AreEqual(0f, player.GetPanning());
            Assert.AreEqual(0f, player.GetSpeed());
            Assert.AreEqual(0f, player.GetPitch());

            player.Clear();
        }
    }
}