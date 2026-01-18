namespace ZhmApi.Dtos
{
    public class AdminReportOverviewDto
    {
        public int ReportId { get; set; }
        public string PatientName { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public decimal TotalCost { get; set; }
    }
}