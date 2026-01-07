namespace ZhmApi.Dtos
{
    public class CreateMedicalDocumentDto
    {
        public int PatientId { get; set; }
        public int? AppointmentId { get; set; } 
        public string Title { get; set; } = null!;
        public string Content { get; set; } = string.Empty;
    }

}
