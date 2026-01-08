namespace ZhmApi.Dtos
{
    public class AppointmentReportDto
    {
        public int Id {get; set;}
        public string Summary {get; set;} = string.Empty;
        public decimal TotalCost {get; set;} 
        public DateTime CreatedAt {get; set;}
        public List<AppointmentReportItemDto> Items {get; set;} = new();
    }
    public class AppointmentReportItemDto
    {
        public string Description {get; set;} = string.Empty;
        public decimal Cost {get; set;}
    }
}