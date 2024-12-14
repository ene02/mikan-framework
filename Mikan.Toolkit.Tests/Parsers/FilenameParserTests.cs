using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mikan.Toolkit.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mikan.Toolkit.Parsers.Tests
{
    [TestClass()]
    public class FilenameParserTests
    {
        [TestMethod()]
        public void GetTest()
        {
            string windowsPath = @"C:\Users\RandomUser\Documents\file.txt";
            string unixPath = "/home/usr/folder/file.txt";

            Assert.AreEqual("file", FilenameParser.Get(windowsPath));
            Assert.AreEqual("file.txt", FilenameParser.Get(windowsPath, true));

            Assert.AreEqual("file", FilenameParser.Get(unixPath));
            Assert.AreEqual("file.txt", FilenameParser.Get(unixPath, true));
        }

        [TestMethod()]
        public void NoExtensionGetTest()
        {
            string windowsPath = @"C:\Users\RandomUser\Documents\file";
            string unixPath = "/home/usr/folder/file";

            Assert.AreEqual("file", FilenameParser.Get(windowsPath));
            Assert.AreEqual("file", FilenameParser.Get(windowsPath, true));

            Assert.AreEqual("file", FilenameParser.Get(unixPath));
            Assert.AreEqual("file", FilenameParser.Get(unixPath, true));
        }

        [TestMethod()]
        public void FailGetTest()
        {
            string windowsPath = "a random string";
            string unixPath = "a random string";

            Assert.AreEqual(string.Empty, FilenameParser.Get(windowsPath));
            Assert.AreEqual(string.Empty, FilenameParser.Get(windowsPath, true));

            Assert.AreEqual(string.Empty, FilenameParser.Get(unixPath));
            Assert.AreEqual(string.Empty, FilenameParser.Get(unixPath, true));
        }
    }
}