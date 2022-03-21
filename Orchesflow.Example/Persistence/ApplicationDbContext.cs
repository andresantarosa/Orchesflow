using Microsoft.EntityFrameworkCore;
using Orchesflow.Example.Models;

namespace Orchesflow.Example.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
    {
        
    }
    
    public DbSet<Customer> Customer { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>().HasKey(b => b.Id);
        modelBuilder.Entity<Customer>().Property(b => b.Name)
            .IsRequired();
        
        base.OnModelCreating(modelBuilder);
    }
}