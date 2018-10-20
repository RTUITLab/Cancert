using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Services.Render
{
<<<<<<< HEAD
    public interface IRenderer
    {
        Task<Stream> Render(Stream dicomStream);
=======
    interface IRenderer
    {
        Stream Render(Stream dicomStream);
>>>>>>> develop
    }
}
