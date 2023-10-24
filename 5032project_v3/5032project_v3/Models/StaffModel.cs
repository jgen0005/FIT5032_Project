using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace _5032project_v3.Models
{
    public partial class StaffModel : DbContext
    {
        public StaffModel()
            : base("name=StaffModel")
        {
        }

        public virtual DbSet<Staff> Staffs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
