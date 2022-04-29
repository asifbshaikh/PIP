namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class AppSettings
    {
        public string SecretKey { get; set; }
        public string ApiUrl { get; set; }
        public string AppUrl { get; set; }
        public int TokenExpiryTime {get; set;}

    }
}