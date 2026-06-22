using Microsoft.EntityFrameworkCore;
using RoomBoard.Web.Models;

namespace RoomBoard.Web.Data;

public sealed class RoomBoardDbContext : DbContext
{
    public RoomBoardDbContext(DbContextOptions<RoomBoardDbContext> options)
        : base(options)
    {
    }

    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<ClassGroup> ClassGroups => Set<ClassGroup>();
    public DbSet<LessonPeriod> LessonPeriods => Set<LessonPeriod>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<SchoolSettings> SchoolSettings => Set<SchoolSettings>();
    public DbSet<KioskSpotlightSettings> KioskSpotlightSettings => Set<KioskSpotlightSettings>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Room>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(80).IsRequired();
            entity.Property(e => e.Location).HasMaxLength(120).IsRequired();
            entity.HasIndex(e => e.DisplayOrder);
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.Property(e => e.FullName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Specialty).HasMaxLength(40);
        });

        modelBuilder.Entity<ClassGroup>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(40).IsRequired();
        });

        modelBuilder.Entity<LessonPeriod>(entity =>
        {
            entity.Ignore(e => e.Label);
            entity.HasIndex(e => e.Number).IsUnique();
        });

        modelBuilder.Entity<SchoolSettings>(entity =>
        {
            entity.Property(e => e.SchoolName).HasMaxLength(160).IsRequired();
            entity.Property(e => e.SchoolType).HasMaxLength(100).IsRequired();
            entity.Property(e => e.SchoolYear).HasMaxLength(30).IsRequired();
            entity.Property(e => e.Address).HasMaxLength(180);
            entity.Property(e => e.Phone).HasMaxLength(40);
            entity.Property(e => e.Email).HasMaxLength(120);
            entity.Property(e => e.LogoPath).HasMaxLength(240);
        });

        modelBuilder.Entity<KioskSpotlightSettings>(entity =>
        {
            entity.Property(e => e.Label).HasMaxLength(60).IsRequired();
            entity.Property(e => e.Title).HasMaxLength(120).IsRequired();
            entity.Property(e => e.Text).HasMaxLength(260).IsRequired();
            entity.Property(e => e.ImagePath).HasMaxLength(240);
            entity.Property(e => e.Credit).HasMaxLength(120);
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.Property(e => e.SubjectOrPurpose).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Notes).HasMaxLength(250);
            entity.HasIndex(e => new { e.Date, e.RoomId });
        });
    }
}
