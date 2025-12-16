namespace ZhmApi.Dtos
{
    public class CreateAccessRequestDto
    {
        public int PatientId {get; set;}
        public string Reason {get; set;} = string.Empty;

    }
}