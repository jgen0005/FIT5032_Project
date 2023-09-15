using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace project_v2.Models
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
    }
}
