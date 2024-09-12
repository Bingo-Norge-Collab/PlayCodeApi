using Microsoft.EntityFrameworkCore;
using PlayCodeApi.Proxy.OkBingo;

namespace PlayCodeApi.Proxy.Data;

public class OkBingoDbContext : DbContext
{
    public DbSet<OkBingoCommand> Commands { get; set; } = null!;
    
    public OkBingoDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // TODO: Move table name to config
        builder.Entity<OkBingoCommand>(e => e.ToTable("COM2"));
    }
}