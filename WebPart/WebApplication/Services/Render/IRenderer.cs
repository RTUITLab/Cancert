using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Services.Render
{
    interface IRenderer
    {
        Stream Render(Stream dicomStream);
    }
}
