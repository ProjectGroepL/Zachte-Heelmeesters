namespace zhmApi.Models
{
    public class AuditEvent
    {
        public int Id {get; set;}
        public string EventType {get; set;} // "LoginSuccess","LoginFailed","2FARequested","2FAVerified"
        public int? UserId {get; set; }
        public string Meta {get; set;}
        public DateTime Timestamp {get; set;} = DateTime.UtcNow;
    }
}