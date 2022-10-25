using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users => Set<User>();

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseNpgsql(x => x.MigrationsAssembly("API"));
    }
}