using Models;

namespace Databases
{
    public class RedisDatabase : DbContext
    {
        public RedisDatabase(DbContextOptions<RedisDatabase> options) : base(options)
        {
            if (options is null)
                return;
        }

        public DbSet<JwtResponse> BlackListedTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
