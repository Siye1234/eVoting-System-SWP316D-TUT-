using eVotingSystemWebAPIs.Models;
using Microsoft.EntityFrameworkCore;

namespace eVotingSystemWebAPIs.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Voter> Voters { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Voter>()
                .HasIndex(v => v.IdNo)
                .IsUnique();

            modelBuilder.Entity<Admin>()
                .HasIndex(a => a.StaffNumber)
                .IsUnique();
        }

        public DbSet<Admin> Admins { get; set; }
    }
}
