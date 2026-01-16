namespace ZhmApi.Dtos
{
    public class InsuranceInvoiceForInsurerDto
{
    public int InvoiceId { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public decimal? CoveredAmount { get; set; }
    public decimal PatientPays { get; set; }
}
}