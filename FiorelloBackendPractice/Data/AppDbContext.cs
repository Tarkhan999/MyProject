using FiorelloBackendPractice.Models;
using Microsoft.EntityFrameworkCore;

namespace FiorelloBackendPractice.Data
{
    public class AppDbContext : DbContext
    {
      
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
       public DbSet<Product> Products { get; set; }
       public DbSet<Category> Categories { get; set; }
       public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<SliderInfo> SliderInfos { get; set; }
        public DbSet<Setting> Settings { get; set; }
    }
}