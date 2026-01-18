namespace ZhmApi.Models
{
    public class InsuranceInvoice
    {
        public int Id {get; set;}
        public int AppointmentReportId {get; set;}

        public int InsurerId {get; set;}
        public AppointmentReport AppointmentReport {get; set;} = null!;
        public decimal Amount {get; set;}
        public decimal? CoveredAmount {get; set;}

        public DateTime CreatedAt {get; set;} = DateTime.UtcNow; 
    }
}