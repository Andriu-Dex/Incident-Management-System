using IncidentsTI.Application.Services;
using IncidentsTI.Application.Common;
using IncidentsTI.Application.Reports.DTOs;
using IncidentsTI.Application.Reports.Interfaces;
using IncidentsTI.Application.Queries;
using IncidentsTI.Domain.Entities;
using IncidentsTI.Domain.Interfaces;
using IncidentsTI.Infrastructure.Data;
using IncidentsTI.Infrastructure.Repositories;
using IncidentsTI.Infrastructure.Reports;
using IncidentsTI.Infrastructure.Services;
using IncidentsTI.Web.Components;
using IncidentsTI.Web.Hubs;
using IncidentsTI.Web.Hubs.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;
using Blazored.Toast;
using Blazored.Modal;

namespace IncidentsTI.Web
{
    // DTO for login endpoint
    public record LoginRequest(string Email, string Password, bool RememberMe);
    
    // DTOs for password recovery endpoints
    public record ForgotPasswordRequest(string Email);
    public record ValidateTokenRequest(string Token);
    public record ResetPasswordRequest(string Token, string NewPassword);

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
            builder.Services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
            
            // Register Application Services
            builder.Services.AddScoped<IIncidentHistoryService, IncidentHistoryService>();
            builder.Services.AddScoped<NotificationService>(); // Base service
            builder.Services.AddScoped<IReportService, DashboardReportService>();

            // Configure MediatR
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(
                typeof(IncidentsTI.Application.DTOs.Users.UserDto).Assembly));

            // Configure Blazored services
            builder.Services.AddBlazoredToast();
            builder.Services.AddBlazoredModal();

            // Register real-time notification service
            builder.Services.AddScoped<IRealTimeNotificationService, RealTimeNotificationService>();
            
            // Register INotificationService with real-time decorator
            builder.Services.AddScoped<INotificationService>(sp =>
            {
                var baseService = sp.GetRequiredService<NotificationService>();
                var realTimeService = sp.GetRequiredService<IRealTimeNotificationService>();
                var logger = sp.GetRequiredService<ILogger<IncidentsTI.Web.Services.RealTimeNotificationDecorator>>();
                return new IncidentsTI.Web.Services.RealTimeNotificationDecorator(baseService, realTimeService, logger);
            });

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

            // SignalR Hub para notificaciones en tiempo real
            app.MapHub<NotificationHub>("/hubs/notifications");

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

            // API endpoint for forgot password - Request password reset
            app.MapPost("/api/auth/forgot-password", async (
                ForgotPasswordRequest request,
                IMediator mediator,
                HttpContext httpContext) =>
            {
                try
                {
                    // Get base URL for reset link
                    var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
                    
                    // Get client IP for audit
                    var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();

                    var command = new IncidentsTI.Application.Commands.RequestPasswordResetCommand
                    {
                        Email = request.Email,
                        BaseUrl = baseUrl,
                        IpAddress = ipAddress
                    };

                    var result = await mediator.Send(command);

                    return Results.Json(new 
                    { 
                        success = result.Success, 
                        message = result.Message,
                        resetLink = result.ResetLink // Only for development - remove in production
                    });
                }
                catch (Exception ex)
                {
                    return Results.Json(new 
                    { 
                        success = false, 
                        message = "Error al procesar la solicitud. Por favor, intente nuevamente." 
                    });
                }
            });

            // API endpoint for validate reset token
            app.MapPost("/api/auth/validate-reset-token", async (
                ValidateTokenRequest request,
                IMediator mediator) =>
            {
                try
                {
                    var command = new IncidentsTI.Application.Commands.ValidateResetTokenCommand
                    {
                        Token = request.Token
                    };

                    var result = await mediator.Send(command);

                    return Results.Json(new 
                    { 
                        isValid = result.IsValid, 
                        maskedEmail = result.MaskedEmail,
                        errorMessage = result.ErrorMessage
                    });
                }
                catch (Exception)
                {
                    return Results.Json(new 
                    { 
                        isValid = false, 
                        errorMessage = "Error al validar el enlace." 
                    });
                }
            });

            // API endpoint for reset password
            app.MapPost("/api/auth/reset-password", async (
                ResetPasswordRequest request,
                IMediator mediator) =>
            {
                try
                {
                    var command = new IncidentsTI.Application.Commands.ResetPasswordCommand
                    {
                        Token = request.Token,
                        NewPassword = request.NewPassword
                    };

                    var result = await mediator.Send(command);

                    return Results.Json(new 
                    { 
                        success = result.Success, 
                        message = result.Message
                    });
                }
                catch (Exception)
                {
                    return Results.Json(new 
                    { 
                        success = false, 
                        message = "Error al cambiar la contraseña. Por favor, intente nuevamente." 
                    });
                }
            });

            // API endpoint for dashboard PDF report generation
            app.MapPost("/api/reports/dashboard/pdf", async (
                GenerateReportRequest request,
                IReportService reportService,
                IMediator mediator,
                HttpContext httpContext) =>
            {
                try
                {
                    // Get current user name
                    var userName = httpContext.User?.Identity?.Name ?? "Sistema";

                    // Get dashboard statistics
                    var statisticsQuery = new GetDashboardStatisticsQuery
                    {
                        StartDate = request.StartDate,
                        EndDate = request.EndDate
                    };
                    var statistics = await mediator.Send(statisticsQuery);

                    // Create report DTO with Ecuador timezone
                    var reportData = new DashboardReportDto
                    {
                        Title = "Reporte de Estadísticas del Dashboard",
                        Subtitle = $"Período: {request.StartDate:dd/MM/yyyy} - {request.EndDate:dd/MM/yyyy}",
                        StartDate = request.StartDate,
                        EndDate = request.EndDate,
                        GeneratedAt = EcuadorTimeZone.Now,
                        GeneratedBy = userName,
                        Statistics = statistics,
                        IncludeSections = request.IncludeSections ?? new ReportSections()
                    };

                    // Generate PDF
                    var pdfBytes = await reportService.GenerateDashboardPdfAsync(reportData);

                    // Return PDF file
                    var fileName = $"Dashboard_Report_{request.StartDate:yyyyMMdd}_{request.EndDate:yyyyMMdd}.pdf";
                    return Results.File(pdfBytes, "application/pdf", fileName);
                }
                catch (Exception ex)
                {
                    return Results.Json(new { success = false, message = $"Error generando reporte: {ex.Message}" }, statusCode: 500);
                }
            }).RequireAuthorization();

            // API endpoint for dashboard Excel report generation
            app.MapPost("/api/reports/dashboard/excel", async (
                GenerateReportRequest request,
                IReportService reportService,
                IMediator mediator,
                HttpContext httpContext) =>
            {
                try
                {
                    // Get current user name
                    var userName = httpContext.User?.Identity?.Name ?? "Sistema";

                    // Get dashboard statistics
                    var statisticsQuery = new GetDashboardStatisticsQuery
                    {
                        StartDate = request.StartDate,
                        EndDate = request.EndDate
                    };
                    var statistics = await mediator.Send(statisticsQuery);

                    // Create report DTO with Ecuador timezone
                    var reportData = new DashboardReportDto
                    {
                        Title = "Reporte de Estadísticas del Dashboard",
                        Subtitle = $"Período: {request.StartDate:dd/MM/yyyy} - {request.EndDate:dd/MM/yyyy}",
                        StartDate = request.StartDate,
                        EndDate = request.EndDate,
                        GeneratedAt = EcuadorTimeZone.Now,
                        GeneratedBy = userName,
                        Statistics = statistics,
                        IncludeSections = request.IncludeSections ?? new ReportSections()
                    };

                    // Generate Excel
                    var excelBytes = await reportService.GenerateDashboardExcelAsync(reportData);

                    // Return Excel file
                    var fileName = $"Dashboard_Report_{request.StartDate:yyyyMMdd}_{request.EndDate:yyyyMMdd}.xlsx";
                    return Results.File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
                catch (Exception ex)
                {
                    return Results.Json(new { success = false, message = $"Error generando reporte Excel: {ex.Message}" }, statusCode: 500);
                }
            }).RequireAuthorization();

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
