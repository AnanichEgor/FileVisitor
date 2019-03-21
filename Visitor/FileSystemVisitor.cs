using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Visitor
{
    class FileSystemVisitor
    {
        // Data structure to hold names of subfolders to be
        // examined for files.
        private Stack<string> dirs = new Stack<string>();
        private List<System.IO.FileInfo> _files = new List<System.IO.FileInfo>();
        private List<System.IO.DirectoryInfo> _subDirsAll = new List<System.IO.DirectoryInfo>;

        private void TraverseTree(string root)
        {
            if (!System.IO.Directory.Exists(root))
            {
                throw new ArgumentException();
            }
            else
            {
                _subDirsAll.Add(System.IO.DirectoryInfo(root));
                dirs.Push(root);
            }

            dirs.Push(root);

            while (dirs.Count > 0)
            {
                string currentDir = dirs.Pop();
                string[] subDirs;

                try
                {
                    subDirs = System.IO.Directory.GetDirectories(currentDir);
                    foreach (string str in subDirs)
                    {
                        _subDirsAll.Add(System.IO.DirectoryInfo(str));
                        dirs.Push(str);
                    }
                }

                catch (UnauthorizedAccessException e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }
                catch (System.IO.DirectoryNotFoundException e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }

                string[] files = null;

                try
                {
                    files = System.IO.Directory.GetFiles(currentDir);
                }

                catch (UnauthorizedAccessException e)
                {

                    Console.WriteLine(e.Message);
                    continue;
                }

                catch (System.IO.DirectoryNotFoundException e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }

                _files.AddRange(GetFiles(files));
            }
        }

        private IEnumerable GetFiles(string[] files)
        {
            foreach (string file in files)
            {
                System.IO.FileInfo fi;
                try
                {
                    fi = new System.IO.FileInfo(file);
                }
                catch (System.IO.FileNotFoundException e)
                {
                    // If file was deleted by a separate application
                    //  or thread since the call to TraverseTree()
                    Console.WriteLine(e.Message);
                    continue;
                }

                yield return fi;
            }
        }
    }
}
