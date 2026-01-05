namespace ZhmApi.Models
{
    public class Notification
    {
        public int Id {get; set;}

        public int UserId {get; set;} 
        public string Message {get; set;} = null!;
        public bool IsRead { get; set;} = false;

        public int? AccessRequestId { get; set; }
        public AccessRequest AccessRequest {get; set;} = null!; 
        public DateTime CreatedAt {get; set;} = DateTime.UtcNow;        
    }
}