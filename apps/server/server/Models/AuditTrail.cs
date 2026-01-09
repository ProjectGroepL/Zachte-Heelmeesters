namespace ZhmApi.Models
{
    public class AuditTrail
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string Method { get; set; } = default!;   // POST, PUT, DELETE
        public string Path { get; set; } = default!;     // /api/users/5

        public string IpAddress { get; set; } = default!;
        public string? UserAgent { get; set; }

        public int StatusCode { get; set; }

        public string? Details { get; set; }

        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    }
}