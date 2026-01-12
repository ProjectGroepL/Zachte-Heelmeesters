namespace ZhmApi.Models
{
    public class AppointmentReportItem
    {
        public int Id {get; set;}

        public int AppointmentReportId {get; set;}
        public AppointmentReport AppointmentReport {get; set;} = null!;

        public string Description {get; set;} = string.Empty; 

        public decimal Cost {get; set;}
    }
}