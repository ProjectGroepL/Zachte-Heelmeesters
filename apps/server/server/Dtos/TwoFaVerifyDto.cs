using System.ComponentModel.DataAnnotations;

namespace ZhmApi.Dtos
{
    public class TwoFaVerifyDto
    {
        public int TempSessionId { get; set; }

        [Required]
        public string Code { get; set; } = string.Empty;
    }
}
