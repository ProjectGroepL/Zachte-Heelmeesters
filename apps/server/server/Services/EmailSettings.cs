namespace ZhmApi.Services
{
    public class EmailSettings
    {
        public string Host {get; set;} = string.Empty;
        public int Port {get; set;} = 587;
        public string? Username {get; set;}
        public string? Password {get; set;}
        public string SenderEmail {get; set;} = "no-reply@zhm.local";
        public string SenderName {get; set;} = "ZHM Medisch  Portaal";
    }
}