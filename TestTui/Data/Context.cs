using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
    }
}
