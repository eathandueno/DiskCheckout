using Microsoft.EntityFrameworkCore;
using MovieList.Models;

public class DiskInventoryContext : DbContext
{
    public DiskInventoryContext(DbContextOptions<DiskInventoryContext> options) : base(options) { }

    public DbSet<Status> Statuses { get; set; }
    public DbSet<DiskType> DiskTypes { get; set; }
    public DbSet<Genre> Genre { get; set; }
    public DbSet<Disk> Disk { get; set; }
    public DbSet<DiskHasBorrower> DiskHasBorrowers { get; set; }
    public DbSet<Borrower> Borrowers { get; set; }
    public DbSet<Artist> Artists { get; set; }
    public DbSet<DiskHasArtist> DiskHasArtists { get; set; }
    public DbSet<ArtistType> ArtistTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<DiskHasArtist>().ToTable("disk_has_artist");

        modelBuilder.Entity<DiskHasArtist>()
        .ToTable("disk_has_artist")
        .HasKey(dha => new { dha.DiskId, dha.ArtistId }); // Composite primary key

        modelBuilder.Entity<DiskHasArtist>()
            .Property(dha => dha.DiskId)
            .HasColumnName("disk_id");

        modelBuilder.Entity<DiskHasArtist>()
            .Property(dha => dha.ArtistId)
            .HasColumnName("artist_id");

        modelBuilder.Entity<DiskHasArtist>()
       .ToTable("disk_has_artist")
       .Property(d => d.DiskId)
       .HasColumnName("disk_id");

        modelBuilder.Entity<DiskHasArtist>()
            .Property(d => d.ArtistId)
            .HasColumnName("artist_id");

        // Map Borrower table
        modelBuilder.Entity<Borrower>()
            .ToTable("borrower")
            .HasKey(b => b.BorrowerId);

        modelBuilder.Entity<Borrower>()
            .Property(b => b.BorrowerId)
            .HasColumnName("borrower_id");

        // Map DiskHasBorrower table
        modelBuilder.Entity<DiskHasBorrower>()
            .ToTable("disk_has_borrower")
            .HasKey(dhb => dhb.DiskHasBorrowerId);

        modelBuilder.Entity<DiskHasBorrower>()
            .Property(dhb => dhb.DiskHasBorrowerId)
            .HasColumnName("disk_has_borrower_id");

        modelBuilder.Entity<DiskHasBorrower>()
            .Property(dhb => dhb.BorrowerId)
            .HasColumnName("borrower_id");

        // Cascading delete for DiskHasBorrower -> Borrower
        modelBuilder.Entity<DiskHasBorrower>()
            .HasOne(dhb => dhb.Borrower)
            .WithMany(b => b.DiskHasBorrowers)
            .HasForeignKey(dhb => dhb.BorrowerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Cascading delete for DiskHasBorrower -> Disk
        modelBuilder.Entity<DiskHasBorrower>()
            .HasOne(dhb => dhb.Disk)
            .WithMany()
            .HasForeignKey(dhb => dhb.DiskId)
            .OnDelete(DeleteBehavior.Cascade);

        // Map Disk table
        modelBuilder.Entity<Disk>()
            .ToTable("disk")
            .Property(d => d.GenreId).HasColumnName("genre_id");

        modelBuilder.Entity<Disk>()
            .HasOne(d => d.Genre)
            .WithMany()
            .HasForeignKey(d => d.GenreId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Disk>()
            .Property(d => d.StatusId).HasColumnName("status_id");

        modelBuilder.Entity<Disk>()
            .HasOne(d => d.Status)
            .WithMany()
            .HasForeignKey(d => d.StatusId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}