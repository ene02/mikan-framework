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
        }

        [TestMethod()]
        public void GetVolumeTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetPanningTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetSpeedTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetPitchTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetPositionTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetPositionInBytesTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetVolumeTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetPanningTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetSpeedTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetPitchTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetAttributesToDefaultTest()
        {
            Assert.Fail();
        }
    }
}