using System.Collections.Generic;

namespace NiHonGo.Core.DTO.Company
{
    public class JobListReturn
    {
        public List<JobSimpleInfo> List { get; set; }
        public int Count { get; set; }
    }
}