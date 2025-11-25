namespace ZhmApi.Models
{
    public class TwoFactorCode
    {
        [key]
        public Guid Id{get; set;}
        public int UserId{get; set;}
        public User User{get;set;}

        public string CodeHash {get; set;} // the code used to verify
        public DateTime CreatedAt{get; set;}
        public DateTime ExpiresAt{get; set;}
        public bool Used {get; set;}= false;

        public int ResendCount {get; set;} = 0;
        public DateTime? LastSentAt {get; set;}
    }
}