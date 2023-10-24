namespace _5032project_v3.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Appointment
    {
        public int AppointmentID { get; set; }

        [StringLength(128)]
        public string PatientID { get; set; }

        public int? StaffID { get; set; }

        public string DateOfAppointment { get; set; }

        [Required]
        [StringLength(50)]
        public string TimeSlot { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Pending";

        public int? FeedbackRating { get; set; }

        public string FeedbackComment { get; set; }

        public string Description { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
