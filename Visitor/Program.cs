using System;

namespace Visitor
{
    class Program
    {
        static void Main(string[] args)
        {
            var a = new FileSystemVisitor("d:\\");
            Console.ReadKey();
        }
    }
}
