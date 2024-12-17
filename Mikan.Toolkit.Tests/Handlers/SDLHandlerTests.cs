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
    public class SDLHandlerTests
    {
        [TestMethod()]
        public void CheckSDLInitTest()
        {
            InitHandler.CheckSDLInit();

            Assert.IsTrue(InitHandler.IsSDLInitialized);

            InitHandler.EndSDL();
        }

        [TestMethod()]
        public void EndSDLTest()
        {
            InitHandler.CheckSDLInit();
            InitHandler.EndSDL();

            Assert.IsFalse(InitHandler.IsSDLInitialized);
        }
    }
}