namespace ZhmApi.Models
{
    public enum NotificationType
    {
        AccessRequest,
        AccessRequestDecision,
        AppointmentReport,
        General
    }
    public class Notification
    {
        public int Id {get; set;}

        public int UserId {get; set;} 
        public string Message {get; set;} = null!;
        public bool IsRead { get; set;} = false;
        public NotificationType Type { get; set; }
        
        public int? AccessRequestId { get; set; }
        public AccessRequest? AccessRequest {get; set;} 
        public DateTime CreatedAt {get; set;} = DateTime.UtcNow;        
    }
}