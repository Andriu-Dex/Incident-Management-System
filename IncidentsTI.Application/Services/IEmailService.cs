using IncidentsTI.Domain.Entities;
using IncidentsTI.Domain.Enums;

namespace IncidentsTI.Application.Services;

/// <summary>
/// Interfaz del servicio de envío de emails
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Envía un email genérico
    /// </summary>
    /// <param name="toEmail">Email del destinatario</param>
    /// <param name="toName">Nombre del destinatario</param>
    /// <param name="subject">Asunto del email</param>
    /// <param name="htmlBody">Contenido HTML del email</param>
    /// <param name="plainTextBody">Contenido de texto plano (opcional)</param>
    Task<bool> SendEmailAsync(string toEmail, string toName, string subject, string htmlBody, string? plainTextBody = null);

    /// <summary>
    /// Envía email de notificación cuando se crea un incidente
    /// </summary>
    Task SendIncidentCreatedEmailAsync(Incident incident, ApplicationUser user);

    /// <summary>
    /// Envía email de notificación cuando se asigna un incidente
    /// </summary>
    Task SendIncidentAssignedEmailAsync(Incident incident, ApplicationUser assignedTo, ApplicationUser assignedBy);

    /// <summary>
    /// Envía email de notificación cuando cambia el estado de un incidente
    /// </summary>
    Task SendStatusChangedEmailAsync(Incident incident, ApplicationUser user, IncidentStatus oldStatus, IncidentStatus newStatus);

    /// <summary>
    /// Envía email de notificación cuando se agrega un comentario
    /// </summary>
    Task SendCommentAddedEmailAsync(Incident incident, ApplicationUser user, IncidentComment comment, ApplicationUser commenter);

    /// <summary>
    /// Envía email de notificación cuando se escala un incidente
    /// </summary>
    Task SendIncidentEscalatedEmailAsync(Incident incident, ApplicationUser user, string fromLevel, string toLevel, string reason);

    /// <summary>
    /// Envía email de notificación cuando se resuelve un incidente
    /// </summary>
    Task SendIncidentResolvedEmailAsync(Incident incident, ApplicationUser user, string? resolution = null);

    /// <summary>
    /// Envía email de notificación cuando se cierra un incidente
    /// </summary>
    Task SendIncidentClosedEmailAsync(Incident incident, ApplicationUser user);

    /// <summary>
    /// Envía email de recuperación de contraseña
    /// </summary>
    Task SendPasswordResetEmailAsync(ApplicationUser user, string resetToken, string resetUrl);

    /// <summary>
    /// Envía email de bienvenida a nuevo usuario
    /// </summary>
    Task SendWelcomeEmailAsync(ApplicationUser user, string temporaryPassword);

    /// <summary>
    /// Verifica si el servicio de email está configurado correctamente
    /// </summary>
    Task<bool> TestConnectionAsync();
}
