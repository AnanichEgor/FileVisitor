using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Visitor
{
    class Program
    {
        static void Main(string[] args)
        {
            //var b = new FileSystemVisitor("d:\\", (fileName) => { return fileName .Length > 10; });
            //b.Execute();

            //var c = new FileSystemVisitor("d:\\", Fits, "a*.*");
            //c.Execute();
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
