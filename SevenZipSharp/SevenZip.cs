using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using SevenZip;
using SevenZip.Compression.LZMA;

namespace SevenZip
{
    public class LZMAEncoderStream : Stream
    {
        private int _lzmaDictionarySize;
        private LzmaCycleEncoder encoder;
        private Stream _baseStream;
        private bool _streamOwner;
        private CircularStream tmpBuffer;

        public int LzmaDictionarySize
        {
            get
            {
                return _lzmaDictionarySize;
            }
            set
            {
                _lzmaDictionarySize = value;
            }
        }

        public LZMAEncoderStream() : this(new MemoryStream())
        {
        }

        public LZMAEncoderStream(Stream baseStream) : this(baseStream, true, 1<<15)
        {
        }

        public LZMAEncoderStream(Stream baseStream, bool streamOwner, int buffersize)
        {
            _baseStream = baseStream;
            _lzmaDictionarySize = 22;
            encoder = new LzmaCycleEncoder();
            WriteLzmaProperties(encoder);
            _streamOwner = streamOwner;
            tmpBuffer = new CircularStream(buffersize);
            encoder.WriteCoderProperties(_baseStream);
            encoder.Start(tmpBuffer, baseStream);
        }

        internal void WriteLzmaProperties(LzmaCycleEncoder encoder)
        {
            CoderPropID[] propIDs =
                {
                    CoderPropID.DictionarySize,
                    CoderPropID.PosStateBits,
                    CoderPropID.LitContextBits,
                    CoderPropID.LitPosBits,
                    CoderPropID.Algorithm,
                    CoderPropID.NumFastBytes,
                    CoderPropID.MatchFinder,
                    CoderPropID.EndMarker
                };
            object[] properties =
                {
                    _lzmaDictionarySize,
                    2,
                    3,
                    0,
                    2,
                    256,
                    "bt4",
                    true
                };
            encoder.SetCoderProperties(propIDs, properties);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            int totalWrite = count;
            int readPos = 0;

            while (totalWrite > 0)
            {
                int writeAmt = Math.Min((int)(tmpBuffer.Capacity - tmpBuffer.Length), totalWrite);
                tmpBuffer.Write(buffer, offset + readPos, writeAmt);
                totalWrite -= writeAmt;
                readPos += writeAmt;
                encoder.Code();
            }

        }

        public void WritePlain(byte[] buffer, int offset, int count)
        {
            _baseStream.Write(buffer, offset, count);
        }

        public override bool CanRead { get { return false; } }
        public override bool CanWrite { get { return true; } }
        public override bool CanSeek { get { return false; } }

        public override void Flush()
        {
            encoder.Finish();
        }

        public override long Length
        {
            get { throw new NotImplementedException(); }
        }

        public override long Position
        {
            get
            {
                return 0;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Close()
        {
            _baseStream.Close();
            base.Close();
        }
    }

    public class LZMADecoderStream : Stream
    {
        private LzmaCycleDecoder decoder;
        private Stream _baseStream;
        private bool _streamOwner;
        private CircularStream tmpBuffer;
        Int64 readPos;

        public LZMADecoderStream(Stream baseStream)
            : this(baseStream, true, 1<<15)
        {
        }

        public LZMADecoderStream(Stream baseStream, bool streamOwner, int buffersize)
        {
            _baseStream = baseStream;
            tmpBuffer = new CircularStream(buffersize);
            decoder = new LzmaCycleDecoder();
            decoder.SetDecoderProperties(GetLzmaProperties(baseStream));
            decoder.Start(baseStream, tmpBuffer);
            readPos = 0;
            _streamOwner = streamOwner;
        }

        internal byte[] GetLzmaProperties(Stream inStream)
        {
            byte[] LZMAProps = new byte[5];
            inStream.Read(LZMAProps, 0, 5);
            return LZMAProps;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override bool CanRead { get { return true; } }
        public override bool CanWrite { get { return false; } }
        public override bool CanSeek { get { return false; } }

        public override void Flush()
        {

        }

        public override long Length
        {
            get { throw new NotImplementedException(); }
        }

        public override long Position
        {
            get
            {
                return 0;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int totalRead = count;
            int amtRead = 0;
            while (totalRead > 0)
            {
                int toDecode = Math.Min((int)(tmpBuffer.Capacity - tmpBuffer.Length) - tmpBuffer.Capacity / 2, totalRead);
                toDecode = Math.Max(toDecode, 0);
                decoder.Code(readPos + toDecode);
                int amt = tmpBuffer.Read(buffer, offset + amtRead, count);
                readPos = readPos + amt;
                totalRead = totalRead - amt;
                amtRead += amt;
                if (amt == 0)
                    return amtRead;
            }
            return amtRead;
        }

        public int ReadPlain(byte[] buffer, int offset, int count)
        {
            return _baseStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Close()
        {
            this.Flush();
            _baseStream.Close();
            base.Close();
        }
    }
}
