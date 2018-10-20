
using DicomParser;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace ConsoleTester
{
    class Program
    {
        static void Main(string[] args)
        {
            var decoder = new DicomDecoder();
            
            decoder.Init(File.OpenRead(@"C:\Users\Reality_Shift\Desktop\New folder\MR.386348.Image 9.dcm"));
            decoder.dicomInfo.
                Zip(decoder.GetMeta(), (f, second) => $">>{f}<< >>{second}<<")
                .ToList()
                .ForEach(Console.WriteLine);
            var bmp = decoder.CreateImage16();
            try
            {
                bmp.Save("lol.png", ImageFormat.Png);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            Console.ReadKey();
        }

        
    }
}
