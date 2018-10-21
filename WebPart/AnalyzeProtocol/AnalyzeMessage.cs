using System;
using System.Collections.Generic;

namespace AnalyzeProtocol
{
    public class AnalyzeMessage
    {
        public Guid AnalyzeId { get; set; }
        public Guid AlgorithmId { get; set; }
        public Guid RecordId { get; set; }
    }
}
