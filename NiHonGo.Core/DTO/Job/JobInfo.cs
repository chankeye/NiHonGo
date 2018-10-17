namespace NiHonGo.Core.DTO.Video
{
    public class JobInfo
    {
        public int CompanyId { get; set; }

        public string CompanyName { get; set; }

        public string Name { get; set; }

        public string What { get; set; }

        public string Why { get; set; }

        public string How { get; set; }

        public string Detail { get; set; }

        public string Type { get; set; }

        public int TypeId { get; set; }

        public int? StartSalary { get; set; }

        public int? EndSalary { get; set; }

        public string Status { get; set; }

        public int StatusId { get; set; }

        public string Tag { get; set; }

        public string Area { get; set; }

        public int AreaId { get; set; }

        public string CreateDate { get; set; }

        public string UpdateDate { get; set; }

        public string Photo { get; set; }
    }
}