using System;
using System.IO;
using System.Runtime.InteropServices;

namespace MyDu
{
    class Program
    {
        [DllImport("kernel32.dll", SetLastError=true)]
        static extern uint GetCompressedFileSizeW(string lpFileName, out uint lpFileSizeHigh);
        //static extern uint GetCompressedFileSizeW([In, MarshalAs(UnmanagedType.LPWStr)] string lpFileName,
        //      [Out, MarshalAs(UnmanagedType.U4)] out uint lpFileSizeHigh);

        //[DllImport("kernel32.dll", SetLastError = true)]
        //private extern static bool Beep(uint dwFreq, uint dwDuration);

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
                }
                catch (UnauthorizedAccessException)
                {
                    Print(relativePath, null);
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
            }
            catch (UnauthorizedAccessException)
            {
                Print(".", null);
            }
        }

        static void Print(string path, SizeInfo sizeInfo)
        {
            if (sizeInfo == null)
            {
                Console.Write($"            -            -");
            }
            else
            {
                Console.Write($" {sizeInfo.Size,12} {sizeInfo.CompressedSize,12}");
            }
            Console.WriteLine($"  {path}");
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
            if (losize == 0xffffffff)
            {
                // エラーが発生
                int errCode = Marshal.GetLastWin32Error();
                Console.WriteLine($"{errCode}");
            }

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
