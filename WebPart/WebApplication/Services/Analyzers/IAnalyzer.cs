using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Models.Data;

namespace WebApplication.Services.Analyzers
{
    public interface IAnalyzer
    {
        Task Analyze(MrAnalyze mrAnalyze);
    }
}
