namespace ZhmApi.Models
{
    public class Treatment
    {
        public int Id {get; set;}
        public string Name {get; set;} = string.Empty;
        public string Description {get; set;} = null!;
        public string? Instructions {get; set;}
    }
}