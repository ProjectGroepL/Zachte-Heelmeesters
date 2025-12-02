using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZhmApi.Models
{
    public class Afspraak
    {
        public int Id { get; set; }
        
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public int SpecialistId { get; set; }
        public User Specialist { get; set; } = null!;

        public int PatientId { get; set; }
        public User Patient { get; set; } = null!;
        
        public int BehandelingId { get; set; }
        // TODO: add Behandelingen table 
        // public Behandeling Behandeling { get; set; } = null!;
    }
}