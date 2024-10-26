using Microsoft.EntityFrameworkCore;

namespace DataMartBuilder.Models
{
    public class DataMartDbContext : DbContext
    {
        public DbSet<DataMart> DataMarts { get; set; }
        public DbSet<DatabaseConnection> DatabaseConnections { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlServer("Data Source=datamarts.db");
    }
}
