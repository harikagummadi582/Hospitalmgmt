using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Hospital_Management.Models
{
    public class Doctor
    {
        [Key]
        public int DoctorID { get; set; }

        [Required]
        public int UserID { get; set; }

        [ForeignKey("UserID")]
        public virtual User? User { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(10)]
        public string Gender { get; set; }

        public int? Experience { get; set; }

        public string AboutDoctor { get; set; }

        public virtual ICollection<DoctorSpecialty>? DoctorSpecialties { get; set; }
        public virtual ICollection<Appointment>? Appointments { get; set; }
        /*public virtual ICollection<Specialty> Specialities{ get; set; }*/
    }
}
    
