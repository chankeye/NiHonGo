namespace NiHonGo.Core.DTO.Company
{
    public class EditJob
    {
        public string Name { get; set; }

        public string What { get; set; }

        public string Why { get; set; }

        public string How { get; set; }

        public string Detail { get; set; }

        public int TypeId { get; set; }

        public int? StartSalary { get; set; }

        public int? EndSalary { get; set; }

        public int StatusId { get; set; }

        public string Tag { get; set; }

        public int AreaId { get; set; }

        public string Photo { get; set; }
    }
}