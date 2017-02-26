namespace ShareX.UploadersLib.FileUploaders
{
    public class PlikSettings
    {
        public string URL = "";
        public string APIKey = "";
        public bool IsSecured = false;
        public string Login = "";
        public string Password = "";
        public bool Removable = false;
        public bool OneShot = false;
        public int TTLUnit = 0;
        public decimal TTL = 30;
        public bool HasComment = false;
        public string Comment = "";
    }
}