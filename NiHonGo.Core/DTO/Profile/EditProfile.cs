namespace NiHonGo.Core.DTO.Profile
{
    public class EditProfile
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Display { get; set; }

        public string Introduction { get; set; }

        public int? VisaId { get; set; }

        public int? Age { get; set; }

        public bool IsMarried { get; set; }
    }
}