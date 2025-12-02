namespace ZhmApi.Dtos
{
    public class ReferralResponse
    {
        public int Id{get; set;}
        public string Treatment{get; set;} = string.Empty;
        public DateTime CreatedAt {get; set;}
    }
}