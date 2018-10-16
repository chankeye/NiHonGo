namespace NiHonGo.Core.Utilities
{
    public class Constant
    {
        /// <summary>
        /// Default password
        /// </summary>
        public const string DefaultPassword = "123456";
        public const string DefaultAdminEmail = "admin@trails.somee.com";

        /// <summary>
        /// Account, English word to begin, only can use English, number and _
        /// </summary>
        public const string PatternAccount = @"^[A-Za-z]{1}\w{1,}$";

        public const string PatternEmail = @"^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$";

        public const string PatternUrl = @"^http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?$";

        public const string PatternImage = @"\d{1,20}\.[png|jpg]";
    }
}