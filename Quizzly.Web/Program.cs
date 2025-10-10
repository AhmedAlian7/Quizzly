using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Quizzly.Business.Services;
using Quizzly.Business.Services.Implementions;
using Quizzly.Business.Services.Interfaces;
using Quizzly.DataAccess.Constants;
using Quizzly.DataAccess.Data;
using Quizzly.DataAccess.Entities;
using Quizzly.DataAccess.Repositories.Implementions;
using Quizzly.DataAccess.Repositories.Interfaces;

namespace Quizzly.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
           

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("HostingConnection")));

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IInstructorManagementService, InstructorManagementService>();
            builder.Services.AddScoped<IInstructorAnalyticsService, InstructorAnalyticsService>();
            builder.Services.AddScoped<IFileUploadService, FileUploadService>();
            builder.Services.AddScoped<IQuizCategoriesService, QuizCategoriesService>();
            builder.Services.AddScoped<IQuizService, QuizService>();
            builder.Services.AddScoped<IStudentInstructorService, StudentInstructorService>();
            builder.Services.AddScoped<IStudentQuizService, StudentQuizService>();
            builder.Services.AddScoped<IManualGradingService, ManualGradingService>();

            // Add Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 3;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            // Register External Login
            builder.Services.AddAuthentication()
                .AddGoogle(options =>
                {
                    var googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");
                    options.ClientId = googleAuthNSection["ClientId"];
                    options.ClientSecret = googleAuthNSection["ClientSecret"];
                })
                .AddFacebook(options =>
                {
                    var facebookAuthNSection = builder.Configuration.GetSection("Authentication:Facebook");
                    options.ClientId = facebookAuthNSection["AppId"];
                    options.ClientSecret = facebookAuthNSection["AppSecret"];

                    options.SaveTokens = true;
                    options.CorrelationCookie.SameSite = SameSiteMode.None;
                    options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.CallbackPath = "/signin-facebook"; // default

                });



            var app = builder.Build();

            // Seed roles at startup
            SeedRolesAsync(app.Services).GetAwaiter().GetResult();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Instructor}/{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }

        private static async Task SeedRolesAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var role in AppRoles.All)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
