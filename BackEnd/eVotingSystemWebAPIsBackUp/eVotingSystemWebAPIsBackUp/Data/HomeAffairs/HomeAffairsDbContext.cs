using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace eVotingSystemWebAPIsBackUp.Data.HomeAffairs;

public partial class HomeAffairsDbContext : DbContext
{
    public HomeAffairsDbContext()
    {
    }

    public HomeAffairsDbContext(DbContextOptions<HomeAffairsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Person> Persons { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;port=3306;database=home_affairsdb;user=root;password=Phiwayinkosi1#;sslmode=Required", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.41-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("persons");

            entity.HasIndex(e => e.IdNo, "IdNo").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Firstname)
                .HasMaxLength(50)
                .HasColumnName("firstname");
            entity.Property(e => e.Gender)
                .HasColumnType("enum('MALE','FEMALE','OTHER')")
                .HasColumnName("gender");
            entity.Property(e => e.IdNo).HasMaxLength(13);
            entity.Property(e => e.Lastname)
                .HasMaxLength(50)
                .HasColumnName("lastname");
            entity.Property(e => e.Middlename)
                .HasMaxLength(50)
                .HasColumnName("middlename");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
