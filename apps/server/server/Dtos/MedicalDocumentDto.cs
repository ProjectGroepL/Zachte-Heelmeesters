using ZhmApi.Models;

namespace ZhmApi.Dtos
{
    public class MedicalDocumentDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = string.Empty;
        public MedicalDocumentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = null!;
    }
}
