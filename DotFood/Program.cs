using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using DotFood.Data;
using DotFood.ViewModels;
using DotFood.Entity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DotFood.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace DotFood
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
   
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);

      
                builder.Services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddCookie(options =>
                {
                  options.LoginPath = "/Account/Login";
                     options.AccessDeniedPath = "/Account/AccessDenied";
                    })


            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],

                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey),

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                // Required for cookie auth fallback
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies["jwt"];
                        return Task.CompletedTask;
                    }
                };
            });

            builder.Services.AddAuthorization();

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
            builder.Services.AddSingleton<JwtHelper>();

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
            app.UseMiddleware<JwtCookieAuthenticationMiddleware>();

            app.UseAuthentication();

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