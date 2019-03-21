using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Visitor
{
    class FileSystemVisitor
    {
        // Data structure to hold names of subfolders to be
        // examined for files.
        private List<FileInfo> _files = new List<FileInfo>();
        private List<DirectoryInfo> _subDirsAll = new List<DirectoryInfo>();

        public FileSystemVisitor(string root)
        {
            TraverseTree(root);
        }

        private void TraverseTree(string root)
        {
            Stack<string> dirs = new Stack<string>();

            if (!Directory.Exists(root))
            {
                throw new ArgumentException();
            }
            else
            {
                _subDirsAll.Add(new DirectoryInfo(root));
                dirs.Push(root);
            }

            dirs.Push(root);

            while (dirs.Count > 0)
            {
                string currentDir = dirs.Pop();
                string[] subDirs;

                try
                {
                    subDirs = Directory.GetDirectories(currentDir);
                    foreach (string str in subDirs)
                    {
                        _subDirsAll.Add(new DirectoryInfo(str));
                        dirs.Push(str);
                    }
                }

                catch (UnauthorizedAccessException e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }
                catch (DirectoryNotFoundException e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }

                string[] files = null;

                try
                {
                    files = Directory.GetFiles(currentDir);
                }

                catch (UnauthorizedAccessException e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }

                catch (DirectoryNotFoundException e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }

                _files.AddRange(GetFiles(files));
            }
        }

        private IEnumerable<FileInfo> GetFiles(string[] files)
        {
            foreach (string file in files)
            {
                FileInfo fi;
                try
                {
                    fi = new FileInfo(file);
                }
                catch (FileNotFoundException e)
                {
                    // If file was deleted by a separate application
                    //  or thread since the call to TraverseTree()
                    Console.WriteLine(e.Message);
                    continue;
                }

                yield return fi;
            }
        }

        public IEnumerable<string> GetFiles()
        {
            foreach (var file in _files)
            {
                yield return $"{file.Name}: {file.Length} bytes, {file.CreationTime}";
            }
        }

        public IEnumerable<string> GetDirectories()
        {
            foreach (var directory in _subDirsAll)
            {
                yield return $"{directory.Name}: {directory.CreationTime}";
            }
        }
    }
}
