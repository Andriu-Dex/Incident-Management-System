using IncidentsTI.Domain.Entities;
using IncidentsTI.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

        public static async Task SeedServicesAsync(ApplicationDbContext context)
        {
            // Check if services already exist
            if (await context.Services.AnyAsync())
            {
                return; // Services already seeded
            }

            var services = new List<Service>
            {
                new Service
                {
                    Name = "Correo Institucional",
                    Description = "Servicios relacionados con el correo electrónico institucional (creación, recuperación de contraseña, configuración)",
                    Category = ServiceCategory.Email,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Service
                {
                    Name = "Red Inalámbrica (WiFi)",
                    Description = "Problemas de conectividad a la red inalámbrica de la universidad, acceso a WiFi",
                    Category = ServiceCategory.Network,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Service
                {
                    Name = "Sistema de Gestión Académica",
                    Description = "Soporte para sistemas académicos (matrícula, calificaciones, horarios, plataforma LMS)",
                    Category = ServiceCategory.AcademicSystems,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Service
                {
                    Name = "Soporte de Hardware",
                    Description = "Reparación y mantenimiento de equipos de cómputo, impresoras, proyectores y otros dispositivos",
                    Category = ServiceCategory.Hardware,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Service
                {
                    Name = "Instalación de Software",
                    Description = "Instalación, actualización y configuración de software institucional y licencias",
                    Category = ServiceCategory.Software,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Service
                {
                    Name = "Acceso a Recursos Digitales",
                    Description = "Acceso a bibliotecas virtuales, bases de datos académicas y recursos digitales",
                    Category = ServiceCategory.AcademicSystems,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Service
                {
                    Name = "VPN Institucional",
                    Description = "Configuración y soporte para acceso remoto a través de VPN",
                    Category = ServiceCategory.Network,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Service
                {
                    Name = "Soporte Técnico General",
                    Description = "Consultas y soporte técnico general que no se categorizan en otros servicios",
                    Category = ServiceCategory.Other,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            };

            await context.Services.AddRangeAsync(services);
            await context.SaveChangesAsync();
        }

        public static async Task SeedEscalationLevelsAsync(ApplicationDbContext context)
        {
            // Check if escalation levels already exist
            if (await context.EscalationLevels.AnyAsync())
            {
                return; // Escalation levels already seeded
            }

            var levels = new List<EscalationLevel>
            {
                new EscalationLevel
                {
                    Name = "Nivel 1 - Mesa de Ayuda",
                    Description = "Primer nivel de soporte. Atención inicial de incidentes, solución de problemas básicos y documentación.",
                    Order = 1,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new EscalationLevel
                {
                    Name = "Nivel 2 - Especialista",
                    Description = "Segundo nivel de soporte. Técnicos especializados para problemas complejos que requieren conocimiento técnico avanzado.",
                    Order = 2,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new EscalationLevel
                {
                    Name = "Nivel 3 - Proveedor Externo",
                    Description = "Tercer nivel de soporte. Proveedores externos o fabricantes para problemas que requieren intervención especializada.",
                    Order = 3,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            };

            await context.EscalationLevels.AddRangeAsync(levels);
            await context.SaveChangesAsync();
        }

        public static async Task SeedKnowledgeArticlesAsync(ApplicationDbContext context, string technicianUserId)
        {
            // Check if knowledge articles already exist
            if (await context.KnowledgeArticles.AnyAsync())
            {
                return; // Knowledge articles already seeded
            }

            // Get service IDs
            var emailService = await context.Services.FirstOrDefaultAsync(s => s.Category == ServiceCategory.Email);
            var networkService = await context.Services.FirstOrDefaultAsync(s => s.Category == ServiceCategory.Network);
            var softwareService = await context.Services.FirstOrDefaultAsync(s => s.Name.Contains("Software"));
            var academicService = await context.Services.FirstOrDefaultAsync(s => s.Category == ServiceCategory.AcademicSystems);

            if (emailService == null || networkService == null) return;

            var articles = new List<KnowledgeArticle>
            {
                // Artículo 1: Recuperación de contraseña de correo
                new KnowledgeArticle
                {
                    Title = "Recuperación de contraseña de correo institucional",
                    ServiceId = emailService.Id,
                    IncidentType = IncidentType.Request,
                    ProblemDescription = "El usuario no puede acceder a su correo institucional debido a que olvidó su contraseña o la cuenta fue bloqueada por múltiples intentos fallidos.",
                    Recommendations = "Verificar la identidad del usuario con documento antes de realizar el restablecimiento. Recordar al usuario activar la verificación en dos pasos después del restablecimiento.",
                    EstimatedResolutionTimeMinutes = 10,
                    AuthorId = technicianUserId,
                    UsageCount = 15,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    Steps = new List<SolutionStep>
                    {
                        new SolutionStep { StepNumber = 1, Title = "Verificar identidad", Description = "Solicitar al usuario su documento de identidad o carné universitario para verificar que es el propietario de la cuenta.", Note = "Obligatorio antes de cualquier cambio de contraseña" },
                        new SolutionStep { StepNumber = 2, Title = "Acceder a Google Admin", Description = "Ingresar al panel de administración de Google Workspace con las credenciales de administrador.", Note = null },
                        new SolutionStep { StepNumber = 3, Title = "Buscar usuario", Description = "En el panel de usuarios, buscar por el correo institucional del usuario afectado.", Note = null },
                        new SolutionStep { StepNumber = 4, Title = "Restablecer contraseña", Description = "Hacer clic en 'Restablecer contraseña', generar una contraseña temporal segura y marcar la opción 'Solicitar cambio de contraseña en el siguiente inicio de sesión'.", Note = null },
                        new SolutionStep { StepNumber = 5, Title = "Comunicar al usuario", Description = "Proporcionar la contraseña temporal al usuario de forma segura e indicarle que debe cambiarla inmediatamente.", Note = "Nunca enviar contraseñas por correo electrónico" }
                    },
                    Keywords = new List<ArticleKeyword>
                    {
                        new ArticleKeyword { Keyword = "contraseña" },
                        new ArticleKeyword { Keyword = "correo" },
                        new ArticleKeyword { Keyword = "email" },
                        new ArticleKeyword { Keyword = "restablecer" },
                        new ArticleKeyword { Keyword = "olvidé" },
                        new ArticleKeyword { Keyword = "bloqueado" }
                    }
                },

                // Artículo 2: Problemas de conexión WiFi
                new KnowledgeArticle
                {
                    Title = "Solución a problemas de conexión WiFi institucional",
                    ServiceId = networkService.Id,
                    IncidentType = IncidentType.Failure,
                    ProblemDescription = "El usuario no puede conectarse a la red WiFi de la universidad o experimenta desconexiones frecuentes.",
                    Recommendations = "Si el problema persiste después de estos pasos, puede ser un problema de cobertura. Verificar si hay otros usuarios con el mismo problema en la zona.",
                    EstimatedResolutionTimeMinutes = 15,
                    AuthorId = technicianUserId,
                    UsageCount = 22,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-25),
                    Steps = new List<SolutionStep>
                    {
                        new SolutionStep { StepNumber = 1, Title = "Olvidar red guardada", Description = "En el dispositivo del usuario, ir a Configuración de WiFi, buscar la red 'UTA-WIFI' o 'EDUROAM', y seleccionar 'Olvidar esta red'.", Note = null },
                        new SolutionStep { StepNumber = 2, Title = "Reiniciar WiFi", Description = "Desactivar el WiFi del dispositivo, esperar 10 segundos y volver a activarlo.", Note = null },
                        new SolutionStep { StepNumber = 3, Title = "Reconectar", Description = "Buscar la red 'UTA-WIFI' e intentar conectarse nuevamente con las credenciales institucionales.", Note = null },
                        new SolutionStep { StepNumber = 4, Title = "Verificar credenciales", Description = "Asegurarse de que el usuario está usando su correo institucional completo (usuario@uta.edu.ec) y la contraseña correcta.", Note = null },
                        new SolutionStep { StepNumber = 5, Title = "Verificar fecha y hora", Description = "Revisar que la fecha y hora del dispositivo estén correctas, ya que esto puede causar problemas de autenticación.", Note = "Causa común en dispositivos móviles" }
                    },
                    Keywords = new List<ArticleKeyword>
                    {
                        new ArticleKeyword { Keyword = "wifi" },
                        new ArticleKeyword { Keyword = "red" },
                        new ArticleKeyword { Keyword = "conexión" },
                        new ArticleKeyword { Keyword = "internet" },
                        new ArticleKeyword { Keyword = "desconexión" },
                        new ArticleKeyword { Keyword = "wireless" }
                    }
                },

                // Artículo 3: Configuración de correo en Outlook
                new KnowledgeArticle
                {
                    Title = "Configuración de correo institucional en Microsoft Outlook",
                    ServiceId = emailService.Id,
                    IncidentType = IncidentType.Request,
                    ProblemDescription = "El usuario necesita configurar su cuenta de correo institucional en la aplicación Microsoft Outlook para escritorio o móvil.",
                    Recommendations = "Para dispositivos móviles, recomendar usar la app oficial de Gmail o Outlook. La configuración manual con IMAP no es recomendada.",
                    EstimatedResolutionTimeMinutes = 8,
                    AuthorId = technicianUserId,
                    UsageCount = 12,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-20),
                    Steps = new List<SolutionStep>
                    {
                        new SolutionStep { StepNumber = 1, Title = "Abrir Outlook", Description = "Abrir Microsoft Outlook en el equipo del usuario.", Note = null },
                        new SolutionStep { StepNumber = 2, Title = "Agregar cuenta", Description = "Ir a Archivo > Agregar cuenta (o Configuración de cuenta si ya hay cuentas).", Note = null },
                        new SolutionStep { StepNumber = 3, Title = "Ingresar correo", Description = "Escribir el correo institucional completo (usuario@uta.edu.ec) y hacer clic en Conectar.", Note = null },
                        new SolutionStep { StepNumber = 4, Title = "Autenticar con Google", Description = "Se abrirá una ventana de Google. El usuario debe ingresar su contraseña y completar la verificación en dos pasos si está habilitada.", Note = null },
                        new SolutionStep { StepNumber = 5, Title = "Autorizar Outlook", Description = "Permitir que Outlook acceda a la cuenta de Google cuando lo solicite.", Note = null },
                        new SolutionStep { StepNumber = 6, Title = "Finalizar", Description = "Esperar a que Outlook sincronice los correos. El proceso puede tomar varios minutos dependiendo del tamaño del buzón.", Note = null }
                    },
                    Keywords = new List<ArticleKeyword>
                    {
                        new ArticleKeyword { Keyword = "outlook" },
                        new ArticleKeyword { Keyword = "configurar" },
                        new ArticleKeyword { Keyword = "correo" },
                        new ArticleKeyword { Keyword = "email" },
                        new ArticleKeyword { Keyword = "microsoft" },
                        new ArticleKeyword { Keyword = "sincronizar" }
                    }
                },

                // Artículo 4: Acceso a plataforma académica
                new KnowledgeArticle
                {
                    Title = "Problemas de acceso al Sistema de Gestión Académica",
                    ServiceId = academicService?.Id ?? emailService.Id,
                    IncidentType = IncidentType.Failure,
                    ProblemDescription = "El usuario no puede acceder al sistema de gestión académica (SGA) para consultar calificaciones, horarios o realizar procesos académicos.",
                    Recommendations = "Si el usuario es nuevo, verificar que haya completado el proceso de matrícula. Los accesos se activan 24-48 horas después de la matrícula.",
                    EstimatedResolutionTimeMinutes = 12,
                    AuthorId = technicianUserId,
                    UsageCount = 18,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-15),
                    Steps = new List<SolutionStep>
                    {
                        new SolutionStep { StepNumber = 1, Title = "Verificar URL correcta", Description = "Asegurarse de que el usuario está accediendo a la URL oficial del SGA: https://sga.uta.edu.ec", Note = "Muchos usuarios intentan acceder por enlaces incorrectos" },
                        new SolutionStep { StepNumber = 2, Title = "Limpiar caché", Description = "Solicitar al usuario que limpie la caché y cookies del navegador, o que use una ventana de incógnito.", Note = null },
                        new SolutionStep { StepNumber = 3, Title = "Verificar credenciales", Description = "Confirmar que el usuario está usando su número de cédula como usuario y la contraseña correcta.", Note = null },
                        new SolutionStep { StepNumber = 4, Title = "Restablecer contraseña SGA", Description = "Si olvidó la contraseña, usar la opción 'Olvidé mi contraseña' en la página de login. Se enviará un enlace al correo institucional.", Note = null },
                        new SolutionStep { StepNumber = 5, Title = "Verificar estado de matrícula", Description = "Si persiste el problema, verificar en el sistema administrativo que el usuario tenga matrícula activa.", Note = null }
                    },
                    Keywords = new List<ArticleKeyword>
                    {
                        new ArticleKeyword { Keyword = "sga" },
                        new ArticleKeyword { Keyword = "académico" },
                        new ArticleKeyword { Keyword = "calificaciones" },
                        new ArticleKeyword { Keyword = "matrícula" },
                        new ArticleKeyword { Keyword = "sistema" },
                        new ArticleKeyword { Keyword = "acceso" }
                    }
                },

                // Artículo 5: Instalación de software
                new KnowledgeArticle
                {
                    Title = "Solicitud de instalación de software en equipos institucionales",
                    ServiceId = softwareService?.Id ?? emailService.Id,
                    IncidentType = IncidentType.Request,
                    ProblemDescription = "El usuario (docente o administrativo) necesita instalar un software específico en un equipo institucional para su trabajo.",
                    Recommendations = "Todo software debe ser aprobado antes de la instalación. El software debe tener licencia válida o ser de código abierto.",
                    EstimatedResolutionTimeMinutes = 30,
                    AuthorId = technicianUserId,
                    UsageCount = 8,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    Steps = new List<SolutionStep>
                    {
                        new SolutionStep { StepNumber = 1, Title = "Verificar solicitud", Description = "Revisar que el software solicitado esté en la lista de software autorizado de la universidad.", Note = "Consultar documento 'Lista de Software Autorizado' en SharePoint" },
                        new SolutionStep { StepNumber = 2, Title = "Verificar licencia", Description = "Confirmar que existe licencia disponible para el software, o que es software libre/código abierto.", Note = null },
                        new SolutionStep { StepNumber = 3, Title = "Descargar de fuente oficial", Description = "Obtener el instalador únicamente de la página oficial del fabricante o del repositorio institucional.", Note = "Nunca usar sitios de terceros" },
                        new SolutionStep { StepNumber = 4, Title = "Ejecutar instalación", Description = "Conectarse al equipo del usuario (remoto o presencial) e instalar el software con permisos de administrador.", Note = null },
                        new SolutionStep { StepNumber = 5, Title = "Configurar y probar", Description = "Realizar la configuración inicial si es necesaria y verificar que el software funcione correctamente.", Note = null },
                        new SolutionStep { StepNumber = 6, Title = "Documentar", Description = "Registrar la instalación en el inventario de software del equipo.", Note = null }
                    },
                    Keywords = new List<ArticleKeyword>
                    {
                        new ArticleKeyword { Keyword = "software" },
                        new ArticleKeyword { Keyword = "instalación" },
                        new ArticleKeyword { Keyword = "programa" },
                        new ArticleKeyword { Keyword = "instalar" },
                        new ArticleKeyword { Keyword = "licencia" },
                        new ArticleKeyword { Keyword = "aplicación" }
                    }
                }
            };

            await context.KnowledgeArticles.AddRangeAsync(articles);
            await context.SaveChangesAsync();
        }
    }
}
