using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Source.Data;
using Microsoft.AspNetCore.Builder;
using Source.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using Source.Service;
using Source.Service.Interface;
using Source.Middleware;
namespace Source
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddDbContext<Data.ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // ===== Add Identity =====
            builder.Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();


            // ===== Config Identity Options =====
            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            });

            // ===== JWT Configuration =====
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Bearer";
                options.DefaultChallengeScheme = "Bearer";
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
                    // Hỗ trợ multiple issuers/audiences cho development
                    ValidIssuers = builder.Configuration.GetSection("Jwt:ValidIssuers").Get<string[]>(),
                    ValidAudiences = builder.Configuration.GetSection("Jwt:ValidAudiences").Get<string[]>()
                };
            });
            builder.Services.AddScoped<IAuthService,AuthService>();
            builder.Services.AddScoped<IOnboardingService, OnboardingService>();
            builder.Services.AddScoped<IQuizService, QuizService>();
            builder.Services.AddScoped<ICareerService, CareerService>();
            builder.Services.AddControllers();
            builder.Services.AddScoped<ExceptionMiddleware>();
            builder.Services.AddEndpointsApiExplorer();

            // ===== Cấu hình Swagger với JWT Authentication =====
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "EduPath API",
                    Version = "v1",
                    Description = "API cho nền tảng định hướng nghề nghiệp"
                });

                // Cấu hình JWT Bearer Authentication cho Swagger
                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Nhập JWT token vào đây (Swagger sẽ tự động thêm 'Bearer ' prefix)"
                });

                // Yêu cầu sử dụng security scheme cho tất cả các endpoint có [Authorize]
                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();




            var app = builder.Build();

            // ===== Seed Data =====
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                try
                {
                    DataSeeder.SeedDataAsync(context).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error seeding data: {ex.Message}");
                }
            }

            // Configure the HTTP request pipeline.
           // ===== Middleware =====
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi(); // Map endpoint OpenAPI mới
                app.UseSwagger(); // Enable Swagger middleware
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "EduPath API v1");
                    options.RoutePrefix = string.Empty; // Set Swagger UI at root URL
                });
            }

            // app.UseHttpsRedirection(); // Comment out để tránh redirect từ http sang https trong development
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
