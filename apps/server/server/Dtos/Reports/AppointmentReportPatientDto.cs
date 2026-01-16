namespace ZhmApi.Dtos
{
    public class AppointmentReportPatientDto
    {
        public int Id { get; set; }
        public string Summary { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public List<string> Items { get; set; } = [];
    }
}
