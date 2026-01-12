namespace ZhmApi.Dtos
{
    public class AuditTrailDto
    {
        public long Id { get; set; }
        public int? UserId { get; set; }
        public string IpAddress { get; set; } = null!;
        public string? Details { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string Method { get; set; } = null!;
        public string Path { get; set; } = null!;
        public int StatusCode { get; set; }
        public string? UserAgent { get; set; }
    }
}
