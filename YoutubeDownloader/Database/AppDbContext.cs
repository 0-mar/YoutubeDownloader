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

        modelBuilder.Entity<User>()
            .HasMany(u => u.HistoryRecords)
            .WithOne(hr => hr.User)
            .HasForeignKey(hr => hr.UserId);

        //modelBuilder.Entity<User>().Navigation(u => u.Transactions).AutoInclude();
    }
    
    public DbSet<HistoryRecord> HistoryRecords { get; set; }
}