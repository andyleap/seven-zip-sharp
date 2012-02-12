using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SevenZipSharp;
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
            MemoryStream file1packedStream = new MemoryStream();
            LZMAEncoderStream file1outStream = new LZMAEncoderStream(file1packedStream, false);

            FileStream file1 = File.Open("SevenZipSharp.dll", FileMode.Open, FileAccess.Read, FileShare.Read);
            file1.CopyTo(file1outStream, 4096);
            file1outStream.Close();

            file1packedStream.Seek(0, SeekOrigin.Begin);

            MemoryStream file2packedStream = new MemoryStream();
            LZMAEncoderStream file2outStream = new LZMAEncoderStream(file2packedStream, false);

            FileStream file2 = File.Open("SevenZipTest.exe", FileMode.Open, FileAccess.Read, FileShare.Read);
            file2.CopyTo(file2outStream, 4096);
            file2outStream.Close();

            file2packedStream.Seek(0, SeekOrigin.Begin);

            SevenZipArchive archive = new SevenZipArchive();

            archive.AddFile(new SevenZipFile(SevenZipFile.LZMACodec, file1outStream.GetProperties(), (UInt64)file1.Length, (UInt64)file1packedStream.Length, "SevenZipSharp.dll", file1packedStream));
            archive.AddFile(new SevenZipFile(SevenZipFile.LZMACodec, file2outStream.GetProperties(), (UInt64)file2.Length, (UInt64)file2packedStream.Length, "SevenZipTest.exe", file2packedStream));

            archive.CreateFile(File.Create("SevenZipSharp.7z"));

        }
    }
}
