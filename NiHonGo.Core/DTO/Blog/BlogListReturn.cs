using System.Collections.Generic;

namespace NiHonGo.Core.DTO.Company
{
    public class BlogListReturn
    {
        public List<BlogSimpleInfo> List { get; set; }
        public int Count { get; set; }
    }
}