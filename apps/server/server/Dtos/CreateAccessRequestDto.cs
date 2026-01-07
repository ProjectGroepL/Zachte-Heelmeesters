namespace ZhmApi.Dtos
{
    public class CreateAccessRequestDto
    {
        public int AppointmentId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}