using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SevenZipSharp
{
    public class SevenZipFile
    {
        public byte[] codec;
        public byte[] codecAttr;
        public UInt64 UnpackedSize;
        public UInt64 PackedSize;
        public string name;
        public MemoryStream source;
        public DateTime MTime;
        public UInt32 attr;

        public static byte[] LZMACodec = { 0x03, 0x01, 0x01 };

        public SevenZipFile(byte[] codec, byte[] codecAttr, UInt64 UnpackedSize, UInt64 PackedSize, string name, MemoryStream source)
        {
            this.codec = codec;
            this.codecAttr = codecAttr;
            this.UnpackedSize = UnpackedSize;
            this.PackedSize = PackedSize;
            this.name = name;
            this.source = source;
            MTime = DateTime.Now;
            attr = (uint)FileAttributes.Normal;
        }

    }
}
