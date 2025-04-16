using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DotFood.Data
{
    public class UsersContext : IdentityDbContext
    {
        public UsersContext(DbContextOptions<UsersContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.;Database=DotFood;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true");

            base.OnConfiguring(optionsBuilder);
        }

    }
}
