namespace project_v2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class XRayRecord
    {
        [Key]
        public int RecordID { get; set; }

        public int? AppointmentID { get; set; }

        [Required]
        public byte[] XRayImage { get; set; }

        [Column(TypeName = "date")]
        public DateTime DateTaken { get; set; }

        public string Description { get; set; }

        public string Diagnosis { get; set; }

        public virtual Appointment Appointment { get; set; }
    }
}
