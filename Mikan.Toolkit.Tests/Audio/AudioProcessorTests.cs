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
    public class AudioProcessorTests
    {
        [TestMethod()]
        public void GetFXHandlerTest()
        {
            AudioProcessor player = new QuickPlayer(AudioProcessor.Preset.Default);

            player.Play("the-builder.mp3");

            Assert.AreEqual(0, player.GetFXHandler(ManagedBass.EffectType.BQF));

            player.SetBiquadFilter(ManagedBass.Fx.BQFType.LowPass, 300);

            Thread.Sleep(2000);

            Assert.AreNotEqual(0, player.GetFXHandler(ManagedBass.EffectType.BQF));

            player.Stop();
        }

        [TestMethod()]
        public void SetFXHandlerTest()
        {
            AudioProcessor player = new QuickPlayer(AudioProcessor.Preset.Default);

            player.Play("the-builder.mp3");

            Thread.Sleep(2000);

            player.SetFXHandler(ManagedBass.EffectType.BQF, 999);

            Assert.AreEqual(999, player.GetFXHandler(ManagedBass.EffectType.BQF));

            player.Stop();
        }
    }
}