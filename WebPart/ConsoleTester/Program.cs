using DicomImageViewer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace ConsoleTester
{
    class Program
    {
        static void Main(string[] args)
        {
            var decoder = new DicomDecoder();

            decoder.DicomFileName = @"C:\Users\Reality_Shift\Desktop\New folder\MR.386348.Image 9.dcm";

            List<byte> list8 = new List<byte>();
            decoder.GetPixels8(ref list8);
            List<ushort> list16 = new List<ushort>();
            decoder.GetPixels16(ref list16);
            List<byte> list24 = new List<byte>();
            decoder.GetPixels24(ref list24);
            Console.WriteLine(list16.Count);

            var bmp = CreateImage16(decoder, list16);
            bmp.Save("lol.png", ImageFormat.Png);
        }

        private static Bitmap CreateImage16(DicomDecoder decoder, List<ushort> pix16)
        {
            var lut16 = new byte[65536];
            var winMax = 33224;
            var winMin = 32888;
            int range = winMax - winMin;
            if (range < 1) range = 1;
            double factor = 255.0 / range;
            int i;

            for (i = 0; i < 65536; ++i)
            {
                if (i <= winMin)
                    lut16[i] = 0;
                else if (i >= winMax)
                    lut16[i] = 255;
                else
                {
                    lut16[i] = (byte)((i - winMin) * factor);
                }
            }

            var bmp = new Bitmap(decoder.width, 
                decoder.height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            BitmapData bmd = bmp.LockBits(new Rectangle(0, 0, decoder.width, decoder.height),
               System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);
            unsafe
            {
                int pixelSize = 3;
                int j, j1, i1;
                byte b;

                for (i = 0; i < bmd.Height; ++i)
                {
                    byte* row = (byte*)bmd.Scan0 + (i * bmd.Stride);
                    i1 = i * bmd.Width;

                    for (j = 0; j < bmd.Width; ++j)
                    {
                        b = lut16[pix16[i * bmd.Width + j]];
                        j1 = j * pixelSize;
                        row[j1] = b;            // Red
                        row[j1 + 1] = b;        // Green
                        row[j1 + 2] = b;        // Blue
                    }
                }
            }
            bmp.UnlockBits(bmd);
            return bmp;
        }
    }
}
