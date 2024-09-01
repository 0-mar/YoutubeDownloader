using System.Transactions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using YoutubeDownloader.Models.Database;

namespace YoutubeDownloader.Database;

public class AppDbContext : IdentityDbContext<User>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // put relationships here
        /*modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Category);
        modelBuilder.Entity<User>()
            .HasMany(u => u.Transactions);
        modelBuilder.Entity<User>()
            .HasMany(u => u.Categories);
        
        modelBuilder.Entity<User>().Navigation(u => u.Transactions).AutoInclude();
        modelBuilder.Entity<User>().Navigation(u => u.Categories).AutoInclude();
        modelBuilder.Entity<Transaction>().Navigation(t => t.Category).AutoInclude();
        
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();*/
    }
    
    //public DbSet<User> Users { get; set; }
    //public DbSet<HistoryRecord> HistoryRecords { get; set; }
}