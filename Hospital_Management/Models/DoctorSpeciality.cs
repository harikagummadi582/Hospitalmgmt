using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Hospital_Management.Models
{
    public class DoctorSpecialty
    {
        [Key]
        public int DoctorSpecialtyID { get; set; }
        public int DoctorID { get; set; }
        public int SpecialtyID { get; set; }

        [ForeignKey("DoctorID")]
        public virtual Doctor? Doctor { get; set; }
        [ForeignKey("SpecialtyID")]
        public virtual Specialty? Specialty { get; set; }
    }
}
