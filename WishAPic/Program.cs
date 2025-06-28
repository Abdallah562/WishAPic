using Microsoft.AspNetCore.Authentication.JwtBearer;
using WishAPic.Data;
using WishAPic.Identity;
using WishAPic.ServiceContracts;
using WishAPic.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace WishAPic
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

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
            app.UseStaticFiles();
            //  Enable Swagger UI
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            #region Config. CORS
            app.UseCors(options =>
                    options.WithOrigins("http://localhost:4200")
                              .AllowAnyMethod()
                              .AllowAnyHeader());
            #endregion

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.Run();

        }
    }
}
