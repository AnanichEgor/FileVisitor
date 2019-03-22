using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Visitor
{
    class FileSystemVisitor
    {
        internal delegate void ProcessState(string message);
        internal delegate bool ProcessSearch(string message, bool isAlive = true);
        internal delegate bool ProcessFilter(bool isAlive = true);
        internal delegate bool FilterAction(string fileName);

        // Data structure to hold names of subfolders to be
        // examined for files.
        public event ProcessState Start;
        public event ProcessState Finish;
        public event ProcessSearch FileFinded;
        public event ProcessSearch DirectoryFinded;
        public event ProcessFilter FilteredFileFinded;
        public event ProcessFilter FilteredDirectoryFinded;

        private List<FileInfo> _files = new List<FileInfo>();
        private List<DirectoryInfo> _subDirsAll = new List<DirectoryInfo>();
        private string _root;
        private Func<string, bool> _filter;

        public FileSystemVisitor(string root)
        {
            _root = root;
        }

        public FileSystemVisitor(string root, Func<string, bool> filterAction) : this(root)
        {
            _filter = filterAction;
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
                DirectoryFinded?.Invoke(root);
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

                        // if DirectoryFinded return "true" to exit seatch 
                        if (DirectoryFinded != null && DirectoryFinded.Invoke(str, true))
                        {
                            return;
                        }
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

                    // if FileFinded return "true" to exit seatch 
                    if (FileFinded != null && FileFinded.Invoke(fi.Name, true))
                    {
                        break;
                    }
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

        private Regex FileMaskToRegex(string sFileMask)
        {
            String convertedMask = "^" + Regex.Escape(sFileMask).Replace("\\*", ".*").Replace("\\?", ".") + "$";
            return new Regex(convertedMask, RegexOptions.IgnoreCase);
        }

        public IEnumerable<string> GetFiles()
        {
            foreach (var file in _files)
            {
                if (_filter != null && !_filter(file.Name))
                {
                    continue;
                }
                yield return $"{file.Name}: {file.Length} bytes, {file.CreationTime}";
            }
        }

        public IEnumerable<string> GetDirectories()
        {
            foreach (var directory in _subDirsAll)
            {
                if (_filter != null && !_filter(directory.Name))
                {
                    continue;
                }
                yield return $"{directory.Name}: {directory.CreationTime}";
            }
        }

        public void Execute()
        {
            Start?.Invoke("Start scan!");

            TraverseTree(_root);

            Finish?.Invoke("Finish scan!");
        }
    }
}
