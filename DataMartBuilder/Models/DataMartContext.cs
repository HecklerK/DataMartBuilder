using Microsoft.EntityFrameworkCore;

namespace DataMartBuilder.Models
{
    public class DataMartContext : DbContext
    {
        public DataMartContext(DbContextOptions<DataMartContext> options) : base(options) { }

        public DbSet<CustomTable> CustomTables { get; set; }
    }
}
