namespace ShareX.UploadersLib.FileUploaders
{
    public class AmazonS3Settings
    {
        public string AccessKeyID { get; set; }
        public string SecretAccessKey { get; set; }
        public string Endpoint { get; set; }
        public string Bucket { get; set; }
        public string ObjectPrefix { get; set; }
        public bool UseCustomCNAME { get; set; }
        public string CustomDomain { get; set; }
        public bool UseReducedRedundancyStorage { get; set; }
    }
}