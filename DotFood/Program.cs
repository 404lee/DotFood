using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using DotFood.Data;
using DotFood.Models;
using DotFood.Entity;

namespace DotFood
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<UsersContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<Users, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddEntityFrameworkStores<UsersContext>()
            .AddDefaultTokenProviders();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var dbContext = services.GetRequiredService<UsersContext>();
                    dbContext.Database.Migrate();

                    var userManager = services.GetRequiredService<UserManager<Users>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                    await InitializeAdminUser(userManager, roleManager);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred during migration or admin user setup");
                }
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.UseStatusCodePagesWithReExecute("/Home/Error");


            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }

        private static async Task InitializeAdminUser(UserManager<Users> userManager, RoleManager<IdentityRole> roleManager)
        {
            var roleExist = await roleManager.RoleExistsAsync("Admin");
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            var adminUser = await userManager.FindByEmailAsync("admin@example.com");
            if (adminUser == null)
            {
                adminUser = new Users
                {
                    UserName = "admin@example.com",
                    Email = "admin@example.com",
                    FullName = "Admin User",
                    Country = "Country",
                    City = "City"
                };
                var result = await userManager.CreateAsync(adminUser, "Admin@1234");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}