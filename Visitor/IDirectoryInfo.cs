using System;
using System.IO;

namespace Visitor
{
    public interface IDirectoryInfo
    {
        bool Exists { get; }
        string FullName { get; }
        IDirectoryInfo[] GetDirectories();
        FileInfo[] GetFiles();
        DateTime CreationTime { get; set; }
    }
}