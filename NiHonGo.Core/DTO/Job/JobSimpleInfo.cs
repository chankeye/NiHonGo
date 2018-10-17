namespace NiHonGo.Core.DTO.Video
{
    public class JobSimpleInfo
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Detail { get; set; }

        public string Type { get; set; }

        public int? StartSalary { get; set; }

        public int? EndSalary { get; set; }

        public string Photo { get; set; }

        public string Area { get; set; }

        public string UpdateDate { get; set; }

        public int CompanyId { get; set; }

        public string CompanyName { get; set; }
    }
}