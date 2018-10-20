using DicomImageViewer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Services.Render
{
    public class DicomRenderer : IRenderer
    {
        public Stream Render(Stream dicomStream)
        {
            var decoder = new DicomDecoder();
            decoder.Init(dicomStream);
            throw new NotImplementedException();
        }
    }
}
