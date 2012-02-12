using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SevenZip;
using System.IO;
using System.Diagnostics;

namespace SevenZipTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LZMAEncoderStream outStream = new LZMAEncoderStream(File.Create("SevenZipSharp.dll.lz"));

            FileStream input = File.Open("SevenZipSharp.dll", FileMode.Open, FileAccess.Read, FileShare.Read);
            
            byte[] inputBuf = new byte[1000];
            int readAmt;

            byte[] len = new byte[8];

            for (int i = 0; i < 8; i++)
                len[i] = (byte) (input.Length >> (8*i));

            outStream.WritePlain(len, 0, 8);

            while ((readAmt = input.Read(inputBuf, 0, 1000)) != 0)
            {
                outStream.Write(inputBuf, 0, readAmt);
            }

            outStream.Flush();
            outStream.Close();
            input.Close();


            LZMADecoderStream inStream = new LZMADecoderStream(File.Open("SevenZipSharp.dll.lz", FileMode.Open));

            len = new byte[8];
            Int64 fileLen = 0;

            inStream.ReadPlain(len, 0, 8);

            for (int i = 0; i < 8; i++)
                fileLen |= (((long)(byte)len[i] << (8 * i)));
            
            byte[] wholeBuf = new byte[fileLen];

            inStream.Read(wholeBuf, 0, (int)fileLen);

            File.WriteAllBytes("SevenZipSharp.test.dll", wholeBuf);

            inStream.Close();

        }
    }
}
