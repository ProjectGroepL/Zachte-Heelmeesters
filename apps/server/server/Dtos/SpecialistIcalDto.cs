using System.ComponentModel.DataAnnotations;

namespace ZhmApi.Dtos
{
    public class SpecialistIcalDto
    {
        [Required]
        [Url]
        public string Url { get; set; } = string.Empty;
    }
}
