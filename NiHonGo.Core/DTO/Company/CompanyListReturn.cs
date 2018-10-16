using System.Collections.Generic;

namespace NiHonGo.Core.DTO.Company
{
    public class CompanyListReturn
    {
        public List<CompanySimpleInfo> List { get; set; }
        public int Count { get; set; }
    }
}