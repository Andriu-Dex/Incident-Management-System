using IncidentsTI.Application.Services;
using IncidentsTI.Domain.Entities;
using IncidentsTI.Domain.Interfaces;
using IncidentsTI.Infrastructure.Data;
using IncidentsTI.Infrastructure.Repositories;
using IncidentsTI.Infrastructure.Services;
using IncidentsTI.Web.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;
using Blazored.Toast;
using Blazored.Modal;

namespace IncidentsTI.Web
{
    // DTO for login endpoint
    public record LoginRequest(string Email, string Password, bool RememberMe);

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            // Configure Database
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            // Configure Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;

                // User settings
                options.User.RequireUniqueEmail = true;

                // SignIn settings
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // Configure Authentication
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/login";
                options.LogoutPath = "/logout";
                options.AccessDeniedPath = "/access-denied";
                options.ExpireTimeSpan = TimeSpan.FromHours(8);
                options.SlidingExpiration = true;
            });

            // Register Repositories
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
            builder.Services.AddScoped<IIncidentRepository, IncidentRepository>();
            builder.Services.AddScoped<IIncidentHistoryRepository, IncidentHistoryRepository>();
            builder.Services.AddScoped<IIncidentCommentRepository, IncidentCommentRepository>();
            builder.Services.AddScoped<IEscalationLevelRepository, EscalationLevelRepository>();
            builder.Services.AddScoped<IIncidentEscalationRepository, IncidentEscalationRepository>();
            builder.Services.AddScoped<IKnowledgeArticleRepository, KnowledgeArticleRepository>();
            builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
            
            // Register Application Services
            builder.Services.AddScoped<IIncidentHistoryService, IncidentHistoryService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();

            // Configure MediatR
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(
                typeof(IncidentsTI.Application.DTOs.Users.UserDto).Assembly));

            // Configure Blazored services
            builder.Services.AddBlazoredToast();
            builder.Services.AddBlazoredModal();

            // Configure Circuit options to suppress authentication state errors during logout
            builder.Services.AddServerSideBlazor()
                .AddCircuitOptions(options => 
                {
                    options.DetailedErrors = builder.Environment.IsDevelopment();
                });

            // Configure HttpClient for server-side Blazor
            builder.Services.AddScoped(sp =>
            {
                var navigationManager = sp.GetRequiredService<NavigationManager>();
                return new HttpClient { BaseAddress = new Uri(navigationManager.BaseUri) };
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            // API endpoint for login (outside Blazor circuit)
            app.MapPost("/api/auth/login", async (
                LoginRequest request,
                UserManager<ApplicationUser> userManager,
                SignInManager<ApplicationUser> signInManager) =>
            {
                var user = await userManager.FindByEmailAsync(request.Email);
                
                if (user == null || !user.IsActive)
                {
                    return Results.Json(new { success = false, message = "Correo electrónico o contraseña incorrectos" });
                }

                var result = await signInManager.PasswordSignInAsync(
                    user.UserName!,
                    request.Password,
                    request.RememberMe,
                    lockoutOnFailure: false);

                if (!result.Succeeded)
                {
                    return Results.Json(new { success = false, message = "Correo electrónico o contraseña incorrectos" });
                }

                var roles = await userManager.GetRolesAsync(user);

                return Results.Json(new 
                { 
                    success = true, 
                    message = "Inicio de sesión exitoso",
                    user = new 
                    {
                        id = user.Id,
                        email = user.Email,
                        fullName = $"{user.FirstName} {user.LastName}",
                        roles = roles
                    }
                });
            });

            // API endpoint for logout (outside Blazor circuit)
            app.MapPost("/api/auth/logout", async (SignInManager<ApplicationUser> signInManager) =>
            {
                await signInManager.SignOutAsync();
                return Results.Json(new { success = true, message = "Sesión cerrada exitosamente" });
            });

            // Seed database
            await SeedDatabaseAsync(app);

            app.Run();
        }

        private static async Task SeedDatabaseAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            
            try
            {
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var context = services.GetRequiredService<ApplicationDbContext>();
                
                // Ensure database is created and migrations are applied
                await context.Database.MigrateAsync();
                
                // Seed users (roles are seeded via OnModelCreating)
                await DatabaseSeeder.SeedUsersAsync(userManager);
                
                // Seed services
                await DatabaseSeeder.SeedServicesAsync(context);

                // Seed escalation levels
                await DatabaseSeeder.SeedEscalationLevelsAsync(context);

                // Seed knowledge articles
                var technicianUser = await userManager.FindByEmailAsync("carlos.tech@uta.edu.ec");
                if (technicianUser != null)
                {
                    await DatabaseSeeder.SeedKnowledgeArticlesAsync(context, technicianUser.Id);
                }
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }
    }
}
