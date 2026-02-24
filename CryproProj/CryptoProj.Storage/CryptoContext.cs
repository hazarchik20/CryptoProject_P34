using CryptoProj.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CryptoProj.Storage;

public class CryptoContext : DbContext
{
    public CryptoContext(DbContextOptions<CryptoContext> options) : base( options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Cryptocurrency> Cryptocurrencies { get; set; }
    public DbSet<CryptoHistoryItem> CryptoHistoryItems { get; set; }
    public DbSet<CryptoAnalytics> CryptoAnalytics { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasKey(u => u.Id);
        
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
        
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasQueryFilter(u => !u.DeletedAt.HasValue);
        
        modelBuilder.Entity<Cryptocurrency>()
            .HasKey(c => c.Id);
        
        modelBuilder.Entity<Cryptocurrency>()
            .HasIndex(c => c.Symbol)
            .IsUnique();
        
        modelBuilder.Entity<Cryptocurrency>()
            .HasQueryFilter(c => !c.DeletedAt.HasValue);
        
        modelBuilder.Entity<CryptoHistoryItem>()
            .HasKey(h => h.Id);

        modelBuilder.Entity<CryptoHistoryItem>()
            .Property(h => h.Id)
            .ValueGeneratedNever();
        
        modelBuilder.Entity<CryptoHistoryItem>()
            .HasOne<Cryptocurrency>()
            .WithMany()
            .HasForeignKey(h => h.CryptocurrencyId);
    }
}