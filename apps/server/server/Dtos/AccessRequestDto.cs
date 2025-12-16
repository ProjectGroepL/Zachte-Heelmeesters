using ZhmApi.Models;

namespace ZhmApi.Dtos
{
    public class AccessRequestDto
    {
        public int Id {get; set;}
        public int SpecialistId {get; set;}
        public AccessRequestStatus Status {get; set;}
        public DateTime RequestedAt {get; set;}
    }
}