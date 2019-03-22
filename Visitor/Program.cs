using System;
using System.Text.RegularExpressions;

namespace Visitor
{
    class Program
    {
        static void Main(string[] args)
        {
            var a = new FileSystemVisitor("d:\\");
            a.Start += WriteToConsole;
            a.Finish += WriteToConsole;
            a.FileFinded += EventSearch;
            a.DirectoryFinded += EventSearch;
            a.FilteredFileFinded += EventFilter;
            a.FilteredDirectoryFinded += EventFilter;
            a.Execute();

            //var b = new FileSystemVisitor("d:\\", (fileName) => { return fileName .Length > 10; });
            //b.Execute();

            //var c = new FileSystemVisitor("d:\\", Fits, "a*.*");
            //c.Execute();

            var resultA = a.GetFiles();
            //var resultB = b.GetFiles();
            //var resultC = c.GetFiles();
            Console.ReadKey();
        }

        public static bool Fits(string sFileName, Regex sFileMaskRegex)
        {
            return sFileMaskRegex.IsMatch(sFileName);
        }

        public static void WriteToConsole(string message)
        {
            Console.WriteLine(message);
        }

        public static bool EventSearch(string message)
        {
            Console.WriteLine(message);
            // then write the desired implementation
            // if return "true" filtering will abort
            return false;
        }

        public static bool EventFilter(string message)
        {
            Console.WriteLine(message);
            // then write the desired implementation
            // if return "true" filtering will abort
            return false;
        }
    }
}
