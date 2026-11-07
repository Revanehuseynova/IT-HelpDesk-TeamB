using HelpDesk.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Web.Data;

/// <summary>
/// Database-first: the schema is created by database/database.sql.
/// The application never creates or migrates the schema by itself.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Ticket> Tickets => Set<Ticket>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("Users");
            e.Property(u => u.FullName).HasMaxLength(100);
            e.Property(u => u.Email).HasMaxLength(150);
            e.Property(u => u.Role).HasMaxLength(30);
            e.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<Category>(e =>
        {
            e.ToTable("Categories");
            e.Property(c => c.Name).HasMaxLength(80);
        });

        modelBuilder.Entity<Ticket>(e =>
        {
            e.ToTable("Tickets");
            e.Property(t => t.Title).HasMaxLength(150);
            e.Property(t => t.Description).HasMaxLength(2000);

            e.HasOne(t => t.Category).WithMany(c => c.Tickets)
                .HasForeignKey(t => t.CategoryId).OnDelete(DeleteBehavior.Restrict);

            e.HasOne(t => t.CreatedBy).WithMany(u => u.CreatedTickets)
                .HasForeignKey(t => t.CreatedById).OnDelete(DeleteBehavior.Restrict);

            e.HasOne(t => t.AssignedTo).WithMany(u => u.AssignedTickets)
                .HasForeignKey(t => t.AssignedToId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
