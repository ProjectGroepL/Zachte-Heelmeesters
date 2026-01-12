namespace ZhmApi.Dtos
{
    public class CreateAppointmentReportDto
    {
        public string Summary {get; set;} = string.Empty;
        public List<CreateAppointmentReportItemDto> Items {get; set;} = new();

    }

    public class CreateAppointmentReportItemDto
    {
        public string Description {get; set;} = string.Empty; 
        public decimal Cost {get; set;}
    }
}