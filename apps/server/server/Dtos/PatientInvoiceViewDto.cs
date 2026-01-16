namespace ZhmApi.Dtos
{
    public class PatientInvoiceViewDto
    {
        public int InvoiceId { get; set; }
        public decimal PatientPays { get; set; }
        public DateTime Date { get; set; }
    }
}