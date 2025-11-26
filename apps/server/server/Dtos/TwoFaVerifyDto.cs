using System;

namespace ZhmApi.Dtos
{
    public class TwoFaVerifyDto
    {
        public int TempSessionId { get; set; } // or userId if you want
        public string Code { get; set; } = string.Empty;
    }
}
