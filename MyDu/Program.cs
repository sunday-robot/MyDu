using System;
using System.IO;
using System.Runtime.InteropServices;

namespace MyDu
{
    class Program
    {
        [DllImport("kernel32.dll")]
        static extern uint GetCompressedFileSizeW([In, MarshalAs(UnmanagedType.LPWStr)] string lpFileName,
              [Out, MarshalAs(UnmanagedType.U4)] out uint lpFileSizeHigh);

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
                var relativePath = Path.GetRelativePath(args[0], e);
                try
                {
                    var sizeInfo = GetDirectorySize(e);
                    Print(relativePath, sizeInfo);
                    //Console.WriteLine($"{relativePath}, {sizeInfo.Size}, {sizeInfo.CompressedSize}");
                }
                catch (UnauthorizedAccessException)
                {
                    Print(relativePath, null);
                    //Console.WriteLine($"{relativePath}, access denied.");
                }
            }
            try
            {
                var sizeInfo = new SizeInfo(0, 0);
                foreach (var e in Directory.GetFiles(args[0]))
                {
                    sizeInfo += GetFileSize(e);
                }
                Print(".", sizeInfo);
                //Console.WriteLine($"., {sizeInfo.Size}, {sizeInfo.CompressedSize}");
            }
            catch (UnauthorizedAccessException)
            {
                Print(".", null);
                //Console.WriteLine($"., access denied.");
            }
        }

        static void Print(string path, SizeInfo sizeInfo)
        {
            Console.Write($"{path,-20}");
            if (sizeInfo == null)
            {
                Console.WriteLine($" (access denied.)");
            }
            else
            {
                Console.WriteLine($" {sizeInfo.Size,10} {sizeInfo.CompressedSize,10}");
            }
        }


        static SizeInfo GetDirectorySize(string directoryPath)
        {
            var size = new SizeInfo(0, 0);
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

        static SizeInfo GetFileSize(string filePath)
        {
            var fi = new FileInfo(filePath);
            var losize = GetCompressedFileSizeW(filePath, out var hosize);
            var compressedSize = (long)hosize << 32 | losize;
            return new SizeInfo(fi.Length, compressedSize);
        }

        static void Usage()
        {
            Console.WriteLine("Usage: MyDu <directory path>");
            Environment.Exit(1);
        }
    }
}
