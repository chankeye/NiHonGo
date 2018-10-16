using System.Collections.Generic;

namespace NiHonGo.Core.DTO.Profile
{
    public class ProfileInfo
    {
        public string Introduction { get; set; }

        public int? Age { get; set; }

        public bool? IsMarried { get; set; }

        public string Photo { get; set; }

        public string Name { get; set; }

        public string Display { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public int? VisaId { get; set; }

        public string Visa { get; set; }

        public string CreateDate { get; set; }

        public string UpdateDate { get; set; }

        public List<ExperienceInfo> ExperienceList { get; set; }

        public List<WorksInfo> WorksList { get; set; }

        public List<SchoolInfo> SchoolList { get; set; }

        public List<KeyValueItem> LanguageList { get; set; }

        public List<KeyValueItem> SkillList { get; set; }

        public List<KeyValueItem> LicenseList { get; set; }
    }

    public class KeyValueItem
    {
        public int Id { get; set; }

        public string Display { get; set; }
    }
}