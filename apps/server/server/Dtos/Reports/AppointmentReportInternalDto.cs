namespace ZhmApi.Dtos
{
    public class AppointmentReportInternalDto
    {
        public int Id { get; set; }
        public string Summary { get; set; } = string.Empty;
        public decimal TotalCost { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<AppointmentReportItemDto> Items { get; set; } = [];
    }
}