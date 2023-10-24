namespace _5032project_v3.Models
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

        public int AppointmentID { get; set; }

        public string Diagnosis { get; set; }

        public string FilePath { get; set; }
    }
}
