using DicomImageViewer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ConsoleTester
{
    class Program
    {
        static void Main(string[] args)
        {
            var decoder = new DicomDecoder();

            decoder.Init(File.OpenRead(@"C:\Users\Reality_Shift\Desktop\New folder\MR.386348.Image 9.dcm"));

            List<byte> list8 = new List<byte>();
            decoder.GetPixels8(ref list8);
            List<ushort> list16 = new List<ushort>();
            decoder.GetPixels16(ref list16);
            List<byte> list24 = new List<byte>();
            decoder.GetPixels24(ref list24);
            Console.WriteLine(list16.Count);

            var bmp = decoder.CreateImage16(decoder, list16);
            try
            {
                bmp.Save("lol.png", ImageFormat.Png);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        
    }
}
