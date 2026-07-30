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
using Source.Service.Admin.Interface;
using Source.Service.Admin;
namespace Source
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Tắt hoàn toàn việc theo dõi file appsettings.json để tránh tràn giới hạn inotify trên Render
            builder.Configuration.Sources.Clear();
            builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<Data.ApplicationDbContext>(options =>
                options.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString),
                    mySqlOptions =>
                    {
                        mySqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    }));
                    

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
            //User service here
            builder.Services.AddScoped<IAuthService,AuthService>();
            builder.Services.AddScoped<IOnboardingService, OnboardingService>();
            builder.Services.AddScoped<IQuizService, QuizService>();
            builder.Services.AddScoped<ICareerService, CareerService>();
            builder.Services.AddScoped<ILearningPathService, LearningPathService>();
            builder.Services.AddScoped<ISubjectService, SubjectService>();
            builder.Services.AddScoped<ITopicService, TopicService>();
            builder.Services.AddScoped<IUserAnswersService,UserAnswersService>();
            builder.Services.AddScoped<IProfileService,ProfileService>();

            
            // Admin Services there 
            builder.Services.AddScoped<IAdminFieldService, AdminFieldService>();
            builder.Services.AddScoped<IAdminCareerService, AdminCareerService>();
            builder.Services.AddScoped<IAdminQuizService, AdminQuizService>();
            builder.Services.AddScoped<IAdminAnswerCareerWeightService, AdminAnswerCareerWeightService>();
            builder.Services.AddScoped<IAdminSubjectService, AdminSubjectService>();
            builder.Services.AddScoped<IAdminTopicService, AdminTopicService>();
            builder.Services.AddScoped<IAdminLearningQuestionService, AdminLearningQuestionService>();
            builder.Services.AddScoped<IAdminCareerSubjectService,AdminCareerSubjectService>();
            builder.Services.AddScoped<IAdminRecoAnswers, AdminRecommendationAnswer>();
            builder.Services.AddScoped<IAdminRecoQuestions, AdminRecommendationQuestions>();
            builder.Services.AddScoped<IAdminLearningAnswers, AdminLearningAnswers>();
        

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                });
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

            // Thêm đoạn này vào để định nghĩa "AllowFrontend"
         builder.Services.AddCors(options =>
         {
             options.AddPolicy("AllowFrontend", policy =>
            {
                policy.WithOrigins(
                    "http://localhost:3000",
                    "http://localhost:3001",
                    "http://localhost:3002",
                    "http://127.0.0.1:3000",
                    "http://127.0.0.1:3001",
                    "http://127.0.0.1:3002",
                    "https://localhost:3000",
                    "https://localhost:3001",
                    "https://localhost:3002",
                    "https://edu-vn-git-main-pbao06s-projects.vercel.app"
                ) // Cho phép Frontend trên nhiều port
                      .AllowAnyMethod()
                     .AllowAnyHeader()
                     .AllowCredentials();
             });

             // Policy mở rộng cho development
             options.AddPolicy("AllowAll", policy =>
             {
                 policy.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
             });
         });

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();




            var app = builder.Build();

            // ===== Seed Data =====
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDbContext>();
                var userManager = services.GetRequiredService<UserManager<User>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                try
                {
                    // Use migrations for database creation and updates instead of EnsureCreated.
                    // This keeps schema changes versioned and avoids conflicts with EF migrations.
                    context.Database.Migrate();

                    DataSeeder.SeedDataAsync(context).GetAwaiter().GetResult();

                    DataSeeder.SeedIdentityAsync(userManager, roleManager).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error initializing database: {ex.Message}");
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
            app.UseRouting();
            app.UseCors(app.Environment.IsDevelopment() ? "AllowAll" : "AllowFrontend");
            // app.UseHttpsRedirection(); // Comment out để tránh redirect từ http sang https trong development
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
