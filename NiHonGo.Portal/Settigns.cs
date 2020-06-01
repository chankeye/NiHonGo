namespace NiHonGo.Portal
{
    public static class Settigns
    {
        public static string ProfilePhotoFolder
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["UploadFolder:UserPhoto"]; }
        }

        public static string MailForm
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["Email"]; }
        }

        public static string SMTP
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["SMTP"]; }
        }
    }
}