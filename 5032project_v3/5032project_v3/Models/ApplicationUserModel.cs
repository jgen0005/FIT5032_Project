using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace _5032project_v3.Models
{
    public partial class ApplicationUserModel : DbContext
    {
        public ApplicationUserModel()
            : base("name=ApplicationUserModel")
        {
        }

        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
