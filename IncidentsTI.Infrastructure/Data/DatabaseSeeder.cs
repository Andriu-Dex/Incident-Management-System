using IncidentsTI.Domain.Entities;
using IncidentsTI.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace IncidentsTI.Infrastructure.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
        {
            // Check if users already exist
            var existingUsers = userManager.Users.Any();
            if (existingUsers)
            {
                return; // Users already seeded
            }

            var users = new List<(ApplicationUser User, string Password, UserRole Role)>
            {
                // Administrators
                (new ApplicationUser
                {
                    FirstName = "Juan",
                    LastName = "Administrador",
                    UserName = "admin@uta.edu.ec",
                    Email = "admin@uta.edu.ec",
                    EmailConfirmed = true,
                    IsActive = true
                }, "Admin123!", UserRole.Administrator),

                (new ApplicationUser
                {
                    FirstName = "María",
                    LastName = "Administradora",
                    UserName = "maria.admin@uta.edu.ec",
                    Email = "maria.admin@uta.edu.ec",
                    EmailConfirmed = true,
                    IsActive = true
                }, "Admin123!", UserRole.Administrator),

                // Technicians
                (new ApplicationUser
                {
                    FirstName = "Carlos",
                    LastName = "Técnico",
                    UserName = "carlos.tech@uta.edu.ec",
                    Email = "carlos.tech@uta.edu.ec",
                    EmailConfirmed = true,
                    IsActive = true
                }, "Tech123!", UserRole.Technician),

                (new ApplicationUser
                {
                    FirstName = "Ana",
                    LastName = "Técnica",
                    UserName = "ana.tech@uta.edu.ec",
                    Email = "ana.tech@uta.edu.ec",
                    EmailConfirmed = true,
                    IsActive = true
                }, "Tech123!", UserRole.Technician),

                // Teachers
                (new ApplicationUser
                {
                    FirstName = "Pedro",
                    LastName = "Docente",
                    UserName = "pedro.docente@uta.edu.ec",
                    Email = "pedro.docente@uta.edu.ec",
                    EmailConfirmed = true,
                    IsActive = true
                }, "Teacher123!", UserRole.Teacher),

                (new ApplicationUser
                {
                    FirstName = "Laura",
                    LastName = "Docente",
                    UserName = "laura.docente@uta.edu.ec",
                    Email = "laura.docente@uta.edu.ec",
                    EmailConfirmed = true,
                    IsActive = true
                }, "Teacher123!", UserRole.Teacher),

                (new ApplicationUser
                {
                    FirstName = "Roberto",
                    LastName = "Docente",
                    UserName = "roberto.docente@uta.edu.ec",
                    Email = "roberto.docente@uta.edu.ec",
                    EmailConfirmed = true,
                    IsActive = true
                }, "Teacher123!", UserRole.Teacher),

                // Students
                (new ApplicationUser
                {
                    FirstName = "Sofía",
                    LastName = "Estudiante",
                    UserName = "sofia.estudiante@uta.edu.ec",
                    Email = "sofia.estudiante@uta.edu.ec",
                    EmailConfirmed = true,
                    IsActive = true
                }, "Student123!", UserRole.Student),

                (new ApplicationUser
                {
                    FirstName = "Diego",
                    LastName = "Estudiante",
                    UserName = "diego.estudiante@uta.edu.ec",
                    Email = "diego.estudiante@uta.edu.ec",
                    EmailConfirmed = true,
                    IsActive = true
                }, "Student123!", UserRole.Student),

                (new ApplicationUser
                {
                    FirstName = "Valentina",
                    LastName = "Estudiante",
                    UserName = "valentina.estudiante@uta.edu.ec",
                    Email = "valentina.estudiante@uta.edu.ec",
                    EmailConfirmed = true,
                    IsActive = true
                }, "Student123!", UserRole.Student)
            };

            foreach (var (user, password, role) in users)
            {
                var result = await userManager.CreateAsync(user, password);
                
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role.ToString());
                }
            }
        }
    }
}
