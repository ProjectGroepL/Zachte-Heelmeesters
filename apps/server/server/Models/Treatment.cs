namespace ZhmApi.Models
{
    public class Treatment
    {
        public int Id {get; set;}
        public string Description {get; set;} = null!;
        public string? Instructions {get; set;}
    }
}