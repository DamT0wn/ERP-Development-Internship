using ItemProcessor.Models;
using Microsoft.EntityFrameworkCore;

namespace ItemProcessor.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Item> Items => Set<Item>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Item>()
            .HasOne(i => i.ParentItem)
            .WithMany(i => i.ChildItems)
            .HasForeignKey(i => i.ParentItemId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Item>()
            .Property(i => i.Weight)
            .HasPrecision(18, 2);
    }
}
