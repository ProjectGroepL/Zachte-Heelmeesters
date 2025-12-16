namespace ZhmApi.Models
{
    public class AuditTrail
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string IpAddress { get; set; }
        public string Action { get; set; }
        public string Details {get; set;}
        public DateTimeOffset Timestamp {get; set;} = DateTimeOffset.UtcNow;
        public string Result {get; set;}
    }
}