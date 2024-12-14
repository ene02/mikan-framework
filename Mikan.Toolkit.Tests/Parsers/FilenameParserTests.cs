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

            Assert.IsTrue(FilenameParser.Get(windowsPath) == "file");
            Assert.IsTrue(FilenameParser.Get(windowsPath, true) == "file.txt");

            Assert.IsTrue(FilenameParser.Get(unixPath) == "file");
            Assert.IsTrue(FilenameParser.Get(unixPath, true) == "file.txt");
        }

        [TestMethod()]
        public void NoExtensionGetTest()
        {
            string windowsPath = @"C:\Users\RandomUser\Documents\file";
            string unixPath = "/home/usr/folder/file";

            Assert.IsTrue(FilenameParser.Get(windowsPath) == "file");
            Assert.IsTrue(FilenameParser.Get(windowsPath, true) == "file");

            Assert.IsTrue(FilenameParser.Get(unixPath) == "file");
            Assert.IsTrue(FilenameParser.Get(unixPath, true) == "file");
        }

        [TestMethod()]
        public void FailGetTest()
        {
            string windowsPath = "a random string";
            string unixPath = "a random string";

            Assert.IsTrue(FilenameParser.Get(windowsPath) == string.Empty);
            Assert.IsTrue(FilenameParser.Get(windowsPath, true) == string.Empty);

            Assert.IsTrue(FilenameParser.Get(unixPath) == string.Empty);
            Assert.IsTrue(FilenameParser.Get(unixPath, true) == string.Empty);
        }
    }
}