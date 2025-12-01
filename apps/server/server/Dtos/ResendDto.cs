using System.ComponentModel.DataAnnotations;

namespace ZhmApi.Dtos
{
    public class ResendDto
    {
        [Required]
        public int TempSessionId { get; set; }
    }
}