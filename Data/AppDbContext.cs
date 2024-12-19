using Microsoft.EntityFrameworkCore;
using SaveLoadApp.Models;

namespace SaveLoadApp.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Record> Records { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Record>()
                .HasIndex(r => r.Version)
                .IsUnique();
        }
    }
}