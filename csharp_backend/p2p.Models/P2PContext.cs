using Microsoft.EntityFrameworkCore; 
namespace p2p.models
{
    public class P2PContext : DbContext
    {
        public P2PContext(DbContextOptions<P2PContext> options) : base(options)
        {
        }

        public DbSet<P2PItems> P2PItems { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=DB.db",
                b => b.MigrationsAssembly("p2p.api")); // Your API project
        }
    }
}
