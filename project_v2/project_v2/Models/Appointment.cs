namespace project_v2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Appointment
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Appointment()
        {
            XRayRecords = new HashSet<XRayRecord>();
        }

        public int AppointmentID { get; set; }

        [StringLength(128)]
        public string PatientID { get; set; }

        [Required]
        [StringLength(128)]
        public string StaffID { get; set; }

        [Column(TypeName = "date")]
        public DateTime DateOfAppointment { get; set; }

        [Required]
        [StringLength(50)]
        public string TimeSlot { get; set; }

        public int Status { get; set; }

        public int? FeedbackRating { get; set; }

        public string FeedbackComment { get; set; }

        [StringLength(10)]
        public string Description { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        public virtual AspNetUser AspNetUser1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<XRayRecord> XRayRecords { get; set; }
    }
}
