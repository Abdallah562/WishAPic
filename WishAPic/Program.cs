using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using System.Web.Http;
using WishAPic.Data;
using WishAPic.Identity;
using WishAPic.ServiceContracts;
using WishAPic.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using WishAPic.Controllers;

namespace WishAPic
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            //cross-origin resource sharing
            //builder.Services.AddCors(options =>
            //{
            //    options.AddPolicy("MyPolicy",
            //        policy =>
            //        {
            //            policy.WithOrigins("http://localhost:4200")
            //                  .AllowAnyMethod()
            //                  .AllowAnyHeader();
            //        });
            //});

            //  Add services
            builder.Services.AddControllers(options =>
            {
                //Authorization Policy
                var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser().Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });

            builder.Services.AddHttpClient<IImageGenerator, ImageGenerator>();

            //  Enable Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();



            //builder.Services.AddIdentity<Users, IdentityRole>(options =>
            //{
            //    options.Password.RequireNonAlphanumeric = false;
            //    options.Password.RequiredLength = 8;
            //    options.Password.RequireUppercase = false;
            //    options.Password.RequireLowercase = false;
            //    options.User.RequireUniqueEmail = true;
            //    options.SignIn.RequireConfirmedPhoneNumber = true;
            //    options.SignIn.RequireConfirmedEmail = true;
            //    options.SignIn.RequireConfirmedAccount = true;
            //})
            //    .AddEntityFrameworkStores<ApplicationDbContext>()
            //    .AddDefaultTokenProviders();

            builder.Services
                .AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    options.Password.RequiredLength = 5;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireDigit = true;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddUserStore<UserStore<ApplicationUser, ApplicationRole,ApplicationDbContext,Guid>>()
                .AddRoleStore<RoleStore<ApplicationRole,ApplicationDbContext,Guid>>();

            builder.Services.AddScoped<RoleManager<ApplicationRole>>();

            builder.Services.AddTransient<IJwtService, JwtService>();
            //builder.Services.AddHttpClient<IImagesAdderService, ImagesAdderService>();
            builder.Services.AddScoped<IImagesAdderService, ImagesAdderService>();
            builder.Services.AddScoped<IImagesDeleterService, ImagesDeleterService>();
            builder.Services.AddTransient<IImagesGetterService, ImagesGetterService>();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration
                .GetConnectionString("DefaultConnection")));

            //JWT 
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateAudience = true,
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        ValidateIssuer = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                    };
                });
            builder.Services.AddAuthorization(options => { });


            var app = builder.Build();
            //app.UseMiddleware<CustomCorsMiddleware>(); // Use the CORS middleware
            app.UseStaticFiles();
            //  Enable Swagger UI
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            //app.UseCors("MyPolicy");

            #region Config. CORS
            app.UseCors(options =>
                    options.WithOrigins("http://localhost:4200")
                              .AllowAnyMethod()
                              .AllowAnyHeader());
            #endregion

            app.UseAuthentication();
            app.UseAuthorization();

            //app
            //    .MapGroup("/api")
            //    .MapIdentityApi<ApplicationUser>();

            //app.MapPost("/api/signup", async (
            //    UserManager<ApplicationUser> userManager,
            //    [FromBody] UserRegistrationModel userRegistrationModel
            //    ) =>
            //{
            //    ApplicationUser user = new ApplicationUser()
            //    {
            //        UserName = userRegistrationModel.Email,
            //        Email = userRegistrationModel.Email,
            //        FullName = userRegistrationModel.FullName,
            //    };
            //    var result = await userManager.CreateAsync(user, userRegistrationModel.Password);

            //    if (result.Succeeded)
            //        return Results.Ok(result);
            //    else 
            //        return Results.BadRequest(result);
            //});
            app.MapControllers();
            app.Run();



            //var builder = WebApplication.CreateBuilder(args);
            //builder.Services.AddControllers();

            //builder.Services.AddHttpClient<IImageGenerator, ImageGenerator>();

            //var app = builder.Build();
            //app.UseMiddleware<CustomCorsMiddleware>(); // Use the CORS middleware

            //app.UseRouting();
            //app.UseAuthorization();
            //app.MapControllers();

            //app.Run();
        }
    }
    public class UserRegistrationModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
    }
}
