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
        public byte[] PackedCRC;

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
		
		public SevenZipFile(string file)
		{
			this.codec = LZMACodec;
			source = new MemoryStream();
			this.name = file;
			MTime = DateTime.Now;
			attr = (uint)FileAttributes.Normal;
			
			LZMAEncoderStream compSource = new LZMAEncoderStream(source, false);
			FileStream sourceFile = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read);
			codecAttr = compSource.GetProperties();
			sourceFile.CopyTo(compSource);
			compSource.Close();
			UnpackedSize = (UInt64)sourceFile.Length;
			PackedSize = (UInt64)source.Length;
			PackedCRC = CRC32.Calculate(source.ToArray()).Reverse().ToArray();
		}

    }
}
