using DicomParser;
using System;
using System.Collections.Generic;
using System.Text;

namespace PublicAPI.Responses
{
    public class RecordView
    {
        public Guid Id { get; set; }
        public Guid HospitalId { get; set; }
        public int PagesCount { get; set; }
        public long Size { get; set; }
        public List<Meta> Meta { get; set; }
    }
}
