namespace ShareX.UploadersLib.FileUploaders
{
    public class PlikSettings
    {
        public string URL = "";
        public string APIKey = "";
        public bool isSecured = false;
        public string Login = "";
        public string Password = "";
        public bool Removable = false;
        public bool OneShot = false;
        public int TTLUnit = 0;
        public decimal TTL = 30;
        public bool hasComment = false;
        public string Comment = "";
    }
}