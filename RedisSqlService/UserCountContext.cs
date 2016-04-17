namespace RedisSqlService
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class UserCountContext : DbContext
    {
        public UserCountContext()
            : base("name=UserCountContext")
        {
        }

        public virtual DbSet<UserCount> UserCounts { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserCount>()
                .Property(e => e.ServerName)
                .IsUnicode(false);
        }
    }
}
