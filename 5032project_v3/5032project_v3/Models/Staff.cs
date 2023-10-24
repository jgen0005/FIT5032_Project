namespace _5032project_v3.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Staff")]
    public partial class Staff
    {
        public int StaffID { get; set; }

        [Required]
        [StringLength(255)]
        public string FName { get; set; }

        [Required]
        [StringLength(255)]
        public string LName { get; set; }

        [Required]
        [StringLength(255)]
        public string Email { get; set; }
    }
}
