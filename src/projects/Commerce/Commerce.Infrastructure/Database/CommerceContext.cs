using Commerce.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Commerce.Infrastructure.Database;

public class CommerceContext : DbContext
{
    public CommerceContext()
    {
        
    }

    public CommerceContext(DbContextOptions<CommerceContext> options) : base(options)
    {
        
    }

    public virtual DbSet<Product> Products { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CommerceContext).Assembly);
    }
}