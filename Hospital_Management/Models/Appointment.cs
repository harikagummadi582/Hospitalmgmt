using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital_Management.Models
{
    public class Appointment
    {
        [Key]
        public int AppointmentID { get; set; }

        [Required]
        public int PatientID { get; set; }

        [Required]
        public int DoctorID { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime AppointmentDate { get; set; }

        [Required]  
        [DataType(DataType.Time)]
        public TimeSpan AppointmentTime { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = string.Empty;

        [StringLength(255)]
        public string CancellationReason { get; set; } = string.Empty;

        [StringLength(255)]
        public string PatientHealthIssues { get; set; } = string.Empty;

        [ForeignKey("PatientID")]
        public virtual Patient? Patient { get; set; }

        [ForeignKey("DoctorID")]
        public virtual Doctor? Doctor { get; set; }

     
    }
}
