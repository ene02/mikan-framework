using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mikan.Toolkit.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mikan.Toolkit.Handlers.Tests
{
    [TestClass()]
    public class BassHandlerTests
    {
        [TestMethod()]
        public void CheckBassInitTest()
        {
            InitHandler.CheckBassInit();

            Assert.IsTrue(InitHandler.IsBASSInitialized);

            InitHandler.EndBass();
        }

        [TestMethod()]
        public void EndBassTest()
        {
            InitHandler.CheckBassInit();
            InitHandler.EndBass();

            Assert.IsFalse(InitHandler.IsBASSInitialized);
        }
    }
}