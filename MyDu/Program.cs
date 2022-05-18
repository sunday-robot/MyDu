using System;
using System.IO;

namespace MyDu
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Usage();
            }
            var subDirectoryPathes = Directory.GetDirectories(args[0]);
            Array.Sort(subDirectoryPathes);
            foreach (var e in subDirectoryPathes)
            {
                Console.WriteLine($"{Path.GetRelativePath(args[0], e)}, {GetDirectorySize(e)}");
            }
            long size = 0;
            foreach (var e in Directory.GetFiles(args[0]))
            {
                size += GetFileSize(e);
            }
            Console.WriteLine($"., {size}");
        }

        static long GetDirectorySize(string directoryPath)
        {
            long size = 0;
            foreach (var e in Directory.GetDirectories(directoryPath))
            {
                size += GetDirectorySize(e);
            }
            foreach (var e in Directory.GetFiles(directoryPath))
            {
                size += GetFileSize(e);
            }
            return size;
        }

        static long GetFileSize(string filePath)
        {
            var fi = new FileInfo(filePath);
            return fi.Length;
        }

        static void Usage()
        {
            Console.WriteLine("Usage: MyDu <directory path>");
            Environment.Exit(1);
        }
    }
}
