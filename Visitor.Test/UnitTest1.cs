using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;

namespace Visitor.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
                {
                    { @"c:\myfile.txt", new MockFileData("Testing is meh.") },
                    { @"c:\demo\jQuery.js", new MockFileData("some js") },
                    { @"c:\demo\image.gif", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
                });
            var a = fileSystem.DirectoryInfo.FromDirectoryName(@"c:\");
        }
    }
}
