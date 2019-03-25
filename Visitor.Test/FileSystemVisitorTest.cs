using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.IO;

namespace Visitor.Test
{
    [TestClass]
    public class FileSystemVisitorTest
    {

        private FileSystemVisitor _directoryInfo;

        [TestInitialize]
        public void SetUp()
        {
            var mock = new Mock<IDirectoryInfo>();
            mock.Setup(p => p.FullName).Returns("TestHead");
            mock.Setup(p => p.Exists).Returns(true);

            var subDir1 = new Mock<IDirectoryInfo>();
            subDir1.Setup(p => p.FullName).Returns("TestHead\\subTest1");
            subDir1.Setup(p => p.Exists).Returns(true);
            FileInfo[] fileList1 = { new FileInfo("test1.txt"), new FileInfo("test1.png") };
            subDir1.Setup(m => m.GetFiles()).Returns(fileList1);

            var subDir2 = new Mock<IDirectoryInfo>();
            subDir2.Setup(p => p.FullName).Returns("TestHead\\subTest2");
            subDir2.Setup(p => p.Exists).Returns(true);
            FileInfo[] fileList2 = { new FileInfo("test2.txt"), new FileInfo("test2.png") };
            subDir2.Setup(m => m.GetFiles()).Returns(fileList2);

            IDirectoryInfo[] subDirectories = { subDir1.Object, subDir2.Object };

            mock.Setup(m => m.GetDirectories()).Returns(() => subDirectories);

            _directoryInfo = new FileSystemVisitor(mock.Object) ;
        }


        [TestMethod]
        public void Event_Start_ReturnMessageStartEvent()
        {
            string str = "";
            _directoryInfo.Start += (message) => str = message;
            _directoryInfo.Execute();
            Assert.AreEqual(str, "Start scan!");
        }

        [TestMethod]
        public void Event_Finish_ReturnMessageFinishEvent()
        {
            string str = "";
            _directoryInfo.Finish += (message) => str = message;
            _directoryInfo.Execute();
            Assert.AreEqual(str, "Finish scan!");
        }

        [TestMethod]
        public void Directories_ReturnAllDirectoriesCount()
        {
            IEnumerable<string> dirs;
            int count = 0;
            _directoryInfo.Execute();
            dirs = _directoryInfo.Directories;
            foreach (var item in dirs)
            {
                count++;
            }

            Assert.AreEqual(3, count);
        }

        public void Event_DirectoryFinded_ReturnCountCallsEvent()
        {
            int count = 0;
            _directoryInfo.DirectoryFinded += (message) => { count++; return true; } ; 
            Assert.AreEqual(3, count);
        }

        public void Event_FileFinded_ReturnCountCallsEvent()
        {
            int count = 0;
            _directoryInfo.FileFinded += (message) => { count++; return true; };
            Assert.AreEqual(4, count);
        }


        [TestMethod]
        public void Event_Files_ReturnCountCallsEvent()
        {
            IEnumerable<string> files;
            int count = 0;
            _directoryInfo.Execute();
            files = _directoryInfo.Files;
            foreach (var item in files)
            {
                count++;
            }

            Assert.AreEqual(4, count);
        }

        [TestMethod]
        public void Files_ReturnAllFilesCount()
        {
            IEnumerable<string> files;
            int count = 0;
            _directoryInfo.Execute();
            files = _directoryInfo.Files;
            foreach (var item in files)
            {
                count++;
            }

            Assert.AreEqual(4, count);
        }


    }
}
