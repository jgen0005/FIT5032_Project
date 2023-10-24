using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace _5032project_v3.Models
{
    public partial class XRayRecordModel : DbContext
    {
        public XRayRecordModel()
            : base("name=XRayRecordModel")
        {
        }

        public virtual DbSet<XRayRecord> XRayRecords { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }

        public System.Data.Entity.DbSet<_5032project_v3.Models.Appointment> Appointments { get; set; }

        public System.Data.Entity.DbSet<_5032project_v3.Models.AspNetUser> AspNetUsers { get; set; }
        public System.Data.Entity.DbSet<_5032project_v3.Models.Staff> Staff { get; set; }
    }
}
