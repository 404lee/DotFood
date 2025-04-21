using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DotFood.Entity;

namespace DotFood.Data
{
    public class UsersContext : IdentityDbContext<Users>
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<Cart> Cart { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<VendorStatus> VendorStatus { get; set; }

        public UsersContext(DbContextOptions<UsersContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.;Database=DotFood;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true");
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure decimal precision
            modelBuilder.Entity<Order>()
                .Property(o => o.TotalPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.DeliveryFee)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderDetails>()
                .Property(od => od.Price)
                .HasPrecision(18, 2);

            // Orders relationships 
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany()
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Vendor)
                .WithMany()
                .HasForeignKey(o => o.VendorId)
                .OnDelete(DeleteBehavior.Restrict); 

            // Product-Vendor relationship
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Vendor)
                .WithMany(u => u.Products)
                .HasForeignKey(p => p.VendorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Vendor-status relationship
            modelBuilder.Entity<VendorStatus>()
                .HasOne(v => v.User)           
                .WithOne(u => u.VendorStatus)  
                .HasForeignKey<VendorStatus>(v => v.UserId)  
                .OnDelete(DeleteBehavior.Cascade);  


            // OrderDetails relationship
            modelBuilder.Entity<OrderDetails>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderDetails>()
                .HasOne(od => od.Product)
                .WithMany()
                .HasForeignKey(od => od.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cart relationships
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Customer)
                .WithMany()
                .HasForeignKey(c => c.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Product)
                .WithMany()
                .HasForeignKey(c => c.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Product-Category relationship
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            //Order Status 
            modelBuilder.Entity<OrderStatus>()
               .HasOne(os => os.Order)
               .WithOne(o => o.OrderStatus)  
               .HasForeignKey<OrderStatus>(os => os.OrderId) 
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}