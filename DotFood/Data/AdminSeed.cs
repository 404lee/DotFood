using DotFood.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DotFood.Data
{
    public static class AdminSeed
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            
            string adminUserId = "b3c0f8a5-5678-4901-9012-abcdef123456";
            string adminRoleId = "a2b9e7f4-1234-4567-890a-123456789abc";
                
            //Admin password : Admin@1234

            string passwordHash = "AQAAAAEAACcQAAAAEN1vyd46yTHzXQu3nraB3TxRY4zrBksasDfY9JhUQnEpafSZn2CtR7W5mD0+7QsLKw==";

            
            modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = adminRoleId,
                Name = "Admin",
                NormalizedName = "ADMIN"
            });

            
            modelBuilder.Entity<Users>().HasData(new Users
            {
                Id = adminUserId,
                UserName = "admin@example.com",
                NormalizedUserName = "ADMIN@EXAMPLE.COM",
                Email = "admin@example.com",
                NormalizedEmail = "ADMIN@EXAMPLE.COM",
                EmailConfirmed = true,
                PasswordHash = passwordHash,
                SecurityStamp = Guid.NewGuid().ToString(),
                FullName = "Admin User",
                Country = "Country",
                City = "City"
            });

            
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = adminRoleId,
                UserId = adminUserId
            });
        }
    }
}
