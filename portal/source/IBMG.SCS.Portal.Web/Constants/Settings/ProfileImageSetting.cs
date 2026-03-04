namespace IBMG.SCS.Portal.Web.Constants.Settings
{
    public class ProfileImageSetting
    {
        public string BlobConnectionString { get; set; }

        public string BlobContainerName { get; set; }

        public string DefaultProfileImage { get; set; }

        public string[] AllowedProfileImageTypes { get; set; }

        public long MaxFileSize { get; set; } = 31457280;
    }
}