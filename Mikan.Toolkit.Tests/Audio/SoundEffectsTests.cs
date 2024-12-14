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
    public class SoundEffectsTests
    {
        [TestMethod()]
        public void SetBiquadFilterTest()
        {
            PersistentPlayer player = new(AudioProcessor.Preset.Default);

            player.Play("the-builder.mp3");

            Thread.Sleep(1000);

            player.SetBiquadFilter(ManagedBass.Fx.BQFType.LowPass, 300);

            Thread.Sleep(1000);

            Assert.AreNotEqual(0, player.GetFXHandler(ManagedBass.EffectType.BQF));

            Thread.Sleep(1000);

            player.SetBiquadFilter(ManagedBass.Fx.BQFType.LowPass, -300);

            player.Clear();
        }

        [TestMethod()]
        public void SetEchoEffectTest()
        {
            PersistentPlayer player = new(AudioProcessor.Preset.Default);

            player.Play("the-builder.mp3");

            Thread.Sleep(1000);

            player.SetEchoEffect(1f, 0.1f, 0.4f, 0.4f);

            Thread.Sleep(1000);

            Assert.AreNotEqual(0, player.GetFXHandler(ManagedBass.EffectType.Echo));

            Thread.Sleep(1000);

            player.SetEchoEffect(-1f, 2f, 3f, -0.4f);

            player.Clear();
        }

        [TestMethod()]
        public void RemoveAllFxTest()
        {
            PersistentPlayer player = new(AudioProcessor.Preset.Default);

            player.Play("the-builder.mp3");

            Thread.Sleep(1000);

            player.SetBiquadFilter(ManagedBass.Fx.BQFType.LowPass, 300);

            Thread.Sleep(1000);

            player.SetEchoEffect(0.2f, 0.1f, 0.4f, 0.4f);

            Thread.Sleep(1000);

            player.RemoveAllFx();

            Thread.Sleep(1000);

            Assert.AreEqual(0, player.GetFXHandler(ManagedBass.EffectType.BQF));
            Assert.AreEqual(0, player.GetFXHandler(ManagedBass.EffectType.Echo));

            player.Clear();

            // Player with no effects added.
            PersistentPlayer playerNoFX = new(AudioProcessor.Preset.Default);

            playerNoFX.Play("the-builder.mp3");

            Thread.Sleep(1000);

            playerNoFX.RemoveAllFx();

            Thread.Sleep(1000);

            Assert.AreEqual(0, playerNoFX.GetFXHandler(ManagedBass.EffectType.BQF));
            Assert.AreEqual(0, playerNoFX.GetFXHandler(ManagedBass.EffectType.Echo));

            playerNoFX.Clear();
        }
    }
}