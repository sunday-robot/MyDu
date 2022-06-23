using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDu
{
    public sealed class SizeInfo
    {
        public long Size { get; }
        public long CompressedSize { get; }
        public SizeInfo(long size, long compressedSize)
        {
            Size = size;
            CompressedSize = compressedSize;
        }

        public static SizeInfo operator +(SizeInfo a, SizeInfo b)
        {
            return new SizeInfo(a.Size + b.Size, a.CompressedSize + b.CompressedSize);
        }
    }
}
