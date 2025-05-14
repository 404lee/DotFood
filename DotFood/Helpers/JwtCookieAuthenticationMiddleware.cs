 using Microsoft.AspNetCore.Http;
 using Microsoft.Extensions.Configuration;
 using Microsoft.IdentityModel.Tokens;
 using System.IdentityModel.Tokens.Jwt;
 using System.Security.Claims;
 using System.Text;
 using System.Threading.Tasks;
namespace DotFood.Helpers
{

 public class JwtCookieAuthenticationMiddleware
 {
     private readonly RequestDelegate _next;
     private readonly IConfiguration _config;

     public JwtCookieAuthenticationMiddleware(RequestDelegate next, IConfiguration config)
     {
         _next = next;
         _config = config;
     }

     public async Task InvokeAsync(HttpContext context)
     {
         var token = context.Request.Cookies["jwt"];
                
         if (!string.IsNullOrEmpty(token))
         {
             try
             {
                 var tokenHandler = new JwtSecurityTokenHandler();
                 var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                 {
                     ValidateIssuer = true,
                     ValidateAudience = true,
                     ValidateLifetime = true,
                     ValidateIssuerSigningKey = true,
                     ValidIssuer = _config["JwtSettings:Issuer"],
                     ValidAudience = _config["JwtSettings:Audience"],
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"]))
                 }, out SecurityToken validatedToken);

                 context.User = principal;
                    context.User = new ClaimsPrincipal(new ClaimsIdentity()); 

                }
                catch
             {
                 
                 context.Response.Cookies.Delete("jwt");
             }
         }


         await _next(context); 
     }
 }

}
  
          
       