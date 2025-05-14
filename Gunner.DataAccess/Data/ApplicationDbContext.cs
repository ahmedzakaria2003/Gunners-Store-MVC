using Gunner.Models.Models;
using Gunner.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Gunner.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; } 

        public DbSet<Category> categories { get; set; }
        public DbSet<Product> products { get; set; }

        public DbSet<OrderHeader> OrderHeaders { get; set; }

        public DbSet<OrderDetail> orderDetails { get; set; }

        public DbSet<shoppinCart> shoppingCarts { get; set; }           



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); 
            modelBuilder.Entity<ApplicationUser>()
           .Property(u => u.Name).HasColumnType("nvarchar(max)");
            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.StreetAddress).HasColumnType("nvarchar(max)");
            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.City).HasColumnType("nvarchar(max)");
            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.State).HasColumnType("nvarchar(max)");
            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.PostalCode).HasColumnType("nvarchar(max)");


            modelBuilder.Entity<OrderHeader>()
                .HasMany(o => o.OrderDetails)
                .WithOne(od => od.OrderHeader)
                .HasForeignKey(od => od.OrderHeaderId);






        }
    }
}
