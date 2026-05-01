using eVotingSystemWebAPIsBackUp.Models;
using Microsoft.EntityFrameworkCore;

namespace eVotingSystemWebAPIsBackUp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Voter> Voters { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<VotingRegistration> VotingRegistrations { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Voter>()
                .HasIndex(v => v.IdNo)
                .IsUnique();

            modelBuilder.Entity<Admin>()
                .HasIndex(a => a.StaffNumber)
                .IsUnique();
            modelBuilder.Entity<VotingRegistration>()
                 .HasIndex(v => v.IdNo)
                 .IsUnique();
        }
    }
}
