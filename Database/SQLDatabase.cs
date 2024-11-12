using Models;

namespace Databases
{
    public class SQLDatabase : DbContext
    {
        public SQLDatabase(DbContextOptions<SQLDatabase> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }

        public DbSet<JwtResponse> TokenBlackList { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
