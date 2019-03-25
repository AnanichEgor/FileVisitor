using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Visitor
{
    public class FileSystemVisitor
    {
        public delegate void ProcessState(string message);
        public delegate bool ProcessSearch(string message);
        public delegate bool ProcessFilter(string itemName);
        public delegate bool FilterAction(string itemName);
        public delegate bool FilterRexAction(string itemName);

        public event ProcessState Start;
        public event ProcessState Finish;
        public event ProcessSearch FileFinded;
        public event ProcessSearch DirectoryFinded;
        public event ProcessFilter FilteredFileFinded;
        public event ProcessFilter FilteredDirectoryFinded;

        // Data structure to hold names of subfolders to be
        // examined for files.
        private List<FileInfo> _files = new List<FileInfo>();
        private List<IDirectoryInfo> _subDirsAll = new List<IDirectoryInfo>();

        private IDirectoryInfo _root;
        private Regex _maskRegex;
        private Func<string, bool> _filter;
        private Func<string, Regex, bool> _filterRegex;

        public FileSystemVisitor(IDirectoryInfo directory)
        {
            _root = directory;
        }

        public FileSystemVisitor(IDirectoryInfo directory, Func<string, bool> filterAction) : this(directory)
        {
            _filter = filterAction;
        }

        public FileSystemVisitor(IDirectoryInfo directory, Func<string, Regex, bool> filterRegexAction, string mask) : this(directory)
        {
            _filterRegex = filterRegexAction;
            _maskRegex = FileMaskToRegex(mask);
        }

        private void TraverseTree(IDirectoryInfo directory)
        {
            Stack<IDirectoryInfo> dirs = new Stack<IDirectoryInfo>();

            if (!directory.Exists)
            {
                throw new ArgumentException();
            }
            else
            {
                _subDirsAll.Add(directory);
                dirs.Push(directory);
                DirectoryFinded?.Invoke(directory.FullName);
            }

            while (dirs.Count > 0)
            {
                IDirectoryInfo currentDir = dirs.Pop();
                IDirectoryInfo[] subDirs;

                try
                {
                    subDirs = currentDir.GetDirectories();
                    foreach (IDirectoryInfo dir in subDirs)
                    {
                        _subDirsAll.Add(dir);
                        dirs.Push(dir);

                        if (DirectoryFinded != null && DirectoryFinded.Invoke(dir.FullName))
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

                FileInfo[] files = null;

                try
                {
                    files = currentDir.GetFiles();
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

                foreach (FileInfo file in files)
                {
                    _files.Add(file);
                    if (FileFinded != null && FileFinded.Invoke(file.FullName))
                    {
                        return;
                    }
                }
            }
        }

        private Regex FileMaskToRegex(string sFileMask)
        {
            string convertedMask = "^" + Regex.Escape(sFileMask).Replace("\\*", ".*").Replace("\\?", ".") + "$";
            return new Regex(convertedMask, RegexOptions.IgnoreCase);
        }

        public IEnumerable<string> Files
        {
            get
            {
                foreach (var file in _files)
                {
                    if (_filter != null && !_filter(file.Name))
                    {
                        continue;
                    }

                    if (_filterRegex != null && !_filterRegex(file.Name, _maskRegex))
                    {
                        continue;
                    }

                    if (FilteredFileFinded != null && FilteredFileFinded.Invoke(file.Name))
                    {
                        break;
                    }

                    yield return $"{file.Name}: {file.CreationTime}";
                }
            }
        }

        public IEnumerable<string> Directories
        {
            get
            {
                foreach (var directory in _subDirsAll)
                {
                    if (_filter != null && !_filter(directory.FullName))
                    {
                        continue;
                    }

                    if (_filterRegex != null && !_filterRegex(directory.FullName, _maskRegex))
                    {
                        continue;
                    }

                    if (FilteredDirectoryFinded != null && FilteredDirectoryFinded.Invoke(directory.FullName))
                    {
                        break;
                    }

                    yield return $"{directory.FullName}: {directory.CreationTime}";
                }
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
