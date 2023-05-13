using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI;
using nebrangu.Models;
using Attribute = nebrangu.Models.Attribute;

namespace nebrangu
{
    public class nebranguContext : DbContext
    {
        public nebranguContext(DbContextOptions<nebranguContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Attribute> Attributes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<Season> Seasons { get; set; }
        public DbSet<Weather> Weathers { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Mood> Moods { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // define any custom model configuration here, such as setting up relationships between entities
        }
    }
}
