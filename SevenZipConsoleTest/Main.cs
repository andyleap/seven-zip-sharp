using System;
using System.IO;
using SevenZipSharp;

namespace SevenZipConsoleTest
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			SevenZipArchive test = new SevenZipArchive();
			test.AddFile(new SevenZipFile("SevenZipSharp.dll"));
			test.AddFile(new SevenZipFile("CircularBuffer.dll"));
			test.CreateFile(File.Create("SevenZipSharp.dll"));
		}
	}
}
