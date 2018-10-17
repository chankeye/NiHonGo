using System.Collections.Generic;

namespace NiHonGo.Core.DTO.Video
{
    public class SearchJob
    {
        public int? CompanyId { get; set; }

        public string Keyword { get; set; }

        public List<int> TypeIds { get; set; }

        public List<int> AreaIds { get; set; }

        public bool IsOnlyRecruiting { get; set; }

        public int? StartSalary { get; set; }

        public int? EndSalary { get; set; }
    }
}