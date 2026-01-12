using System.ComponentModel.DataAnnotations.Schema;

namespace ZhmApi.Models
{
    public class AppointmentReport
    {
        public int Id {get; set;}
        public int AppointmentId {get; set;}
        [ForeignKey(nameof(AppointmentId))]
        public Appointment Appointment {get; set;} = null!;
        public string Summary {get; set;} = string.Empty;
        public decimal TotalCost {get; set;}
        public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
        public ICollection<AppointmentReportItem> Items {get; set;}
        = new List<AppointmentReportItem>();
    }
}