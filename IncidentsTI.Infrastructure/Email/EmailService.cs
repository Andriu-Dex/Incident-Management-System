using IncidentsTI.Application.Services;
using IncidentsTI.Domain.Entities;
using IncidentsTI.Domain.Enums;
using IncidentsTI.Infrastructure.Email.Templates;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace IncidentsTI.Infrastructure.Email;

/// <summary>
/// Implementación del servicio de envío de emails usando MailKit
/// </summary>
public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<bool> SendEmailAsync(string toEmail, string toName, string subject, string htmlBody, string? plainTextBody = null)
    {
        if (!_settings.IsEnabled)
        {
            _logger.LogInformation("Email service is disabled. Skipping email to {ToEmail}", toEmail);
            return true;
        }

        if (string.IsNullOrEmpty(_settings.SmtpServer))
        {
            _logger.LogWarning("SMTP server not configured. Skipping email to {ToEmail}", toEmail);
            return false;
        }

        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
            message.To.Add(new MailboxAddress(toName, toEmail));
            message.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = htmlBody
            };

            if (!string.IsNullOrEmpty(plainTextBody))
            {
                builder.TextBody = plainTextBody;
            }

            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            
            // Configurar timeout
            client.Timeout = _settings.TimeoutSeconds * 1000;

            // Conectar al servidor SMTP
            var secureSocketOptions = _settings.UseSsl 
                ? SecureSocketOptions.StartTls 
                : SecureSocketOptions.Auto;

            await client.ConnectAsync(_settings.SmtpServer, _settings.SmtpPort, secureSocketOptions);

            // Autenticar si hay credenciales
            if (!string.IsNullOrEmpty(_settings.Username) && !string.IsNullOrEmpty(_settings.Password))
            {
                await client.AuthenticateAsync(_settings.Username, _settings.Password);
            }

            // Enviar el email
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email sent successfully to {ToEmail}: {Subject}", toEmail, subject);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {ToEmail}: {Subject}", toEmail, subject);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task SendIncidentCreatedEmailAsync(Incident incident, ApplicationUser user)
    {
        if (!ShouldSendEmail(user)) return;

        var subject = $"[{incident.TicketNumber}] Incidente Creado - {incident.Title}";
        var htmlBody = EmailTemplateService.GetIncidentCreatedTemplate(incident, user, _settings.BaseUrl);

        await SendEmailAsync(user.Email!, $"{user.FirstName} {user.LastName}", subject, htmlBody);
    }

    /// <inheritdoc />
    public async Task SendIncidentAssignedEmailAsync(Incident incident, ApplicationUser assignedTo, ApplicationUser assignedBy)
    {
        if (!ShouldSendEmail(assignedTo)) return;

        var subject = $"[{incident.TicketNumber}] Incidente Asignado - {incident.Title}";
        var htmlBody = EmailTemplateService.GetIncidentAssignedTemplate(incident, assignedTo, assignedBy, _settings.BaseUrl);

        await SendEmailAsync(assignedTo.Email!, $"{assignedTo.FirstName} {assignedTo.LastName}", subject, htmlBody);
    }

    /// <inheritdoc />
    public async Task SendStatusChangedEmailAsync(Incident incident, ApplicationUser user, IncidentStatus oldStatus, IncidentStatus newStatus)
    {
        if (!ShouldSendEmail(user)) return;

        var subject = $"[{incident.TicketNumber}] Estado Actualizado - {incident.Title}";
        var htmlBody = EmailTemplateService.GetStatusChangedTemplate(incident, user, oldStatus, newStatus, _settings.BaseUrl);

        await SendEmailAsync(user.Email!, $"{user.FirstName} {user.LastName}", subject, htmlBody);
    }

    /// <inheritdoc />
    public async Task SendCommentAddedEmailAsync(Incident incident, ApplicationUser user, IncidentComment comment, ApplicationUser commenter)
    {
        if (!ShouldSendEmail(user)) return;

        // No enviar email si el usuario es quien comentó
        if (user.Id == commenter.Id) return;

        var subject = $"[{incident.TicketNumber}] Nuevo Comentario - {incident.Title}";
        var htmlBody = EmailTemplateService.GetCommentAddedTemplate(incident, user, comment, commenter, _settings.BaseUrl);

        await SendEmailAsync(user.Email!, $"{user.FirstName} {user.LastName}", subject, htmlBody);
    }

    /// <inheritdoc />
    public async Task SendIncidentEscalatedEmailAsync(Incident incident, ApplicationUser user, string fromLevel, string toLevel, string reason)
    {
        if (!ShouldSendEmail(user)) return;

        var subject = $"[{incident.TicketNumber}] Incidente Escalado - {incident.Title}";
        var htmlBody = EmailTemplateService.GetIncidentEscalatedTemplate(incident, user, fromLevel, toLevel, reason, _settings.BaseUrl);

        await SendEmailAsync(user.Email!, $"{user.FirstName} {user.LastName}", subject, htmlBody);
    }

    /// <inheritdoc />
    public async Task SendIncidentResolvedEmailAsync(Incident incident, ApplicationUser user, string? resolution = null)
    {
        if (!ShouldSendEmail(user)) return;

        var subject = $"[{incident.TicketNumber}] ✅ Incidente Resuelto - {incident.Title}";
        var htmlBody = EmailTemplateService.GetIncidentResolvedTemplate(incident, user, resolution, _settings.BaseUrl);

        await SendEmailAsync(user.Email!, $"{user.FirstName} {user.LastName}", subject, htmlBody);
    }

    /// <inheritdoc />
    public async Task SendIncidentClosedEmailAsync(Incident incident, ApplicationUser user)
    {
        if (!ShouldSendEmail(user)) return;

        var subject = $"[{incident.TicketNumber}] Incidente Cerrado - {incident.Title}";
        var htmlBody = EmailTemplateService.GetIncidentClosedTemplate(incident, user, _settings.BaseUrl);

        await SendEmailAsync(user.Email!, $"{user.FirstName} {user.LastName}", subject, htmlBody);
    }

    /// <inheritdoc />
    public async Task SendPasswordResetEmailAsync(ApplicationUser user, string resetToken, string resetUrl)
    {
        if (string.IsNullOrEmpty(user.Email)) return;

        var subject = "Restablecer Contraseña - Sistema de Incidentes UTA";
        var htmlBody = EmailTemplateService.GetPasswordResetTemplate(user, resetUrl, _settings.BaseUrl);

        await SendEmailAsync(user.Email, $"{user.FirstName} {user.LastName}", subject, htmlBody);
    }

    /// <inheritdoc />
    public async Task SendWelcomeEmailAsync(ApplicationUser user, string temporaryPassword)
    {
        if (string.IsNullOrEmpty(user.Email)) return;

        var subject = "Bienvenido al Sistema de Gestión de Incidentes - UTA";
        var htmlBody = EmailTemplateService.GetWelcomeTemplate(user, temporaryPassword, _settings.BaseUrl);

        await SendEmailAsync(user.Email, $"{user.FirstName} {user.LastName}", subject, htmlBody);
    }

    /// <inheritdoc />
    public async Task<bool> TestConnectionAsync()
    {
        if (!_settings.IsEnabled)
        {
            _logger.LogInformation("Email service is disabled");
            return false;
        }

        if (string.IsNullOrEmpty(_settings.SmtpServer))
        {
            _logger.LogWarning("SMTP server not configured");
            return false;
        }

        try
        {
            using var client = new SmtpClient();
            client.Timeout = _settings.TimeoutSeconds * 1000;

            var secureSocketOptions = _settings.UseSsl 
                ? SecureSocketOptions.StartTls 
                : SecureSocketOptions.Auto;

            await client.ConnectAsync(_settings.SmtpServer, _settings.SmtpPort, secureSocketOptions);

            if (!string.IsNullOrEmpty(_settings.Username) && !string.IsNullOrEmpty(_settings.Password))
            {
                await client.AuthenticateAsync(_settings.Username, _settings.Password);
            }

            await client.DisconnectAsync(true);

            _logger.LogInformation("SMTP connection test successful");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SMTP connection test failed");
            return false;
        }
    }

    /// <summary>
    /// Verifica si se debe enviar email al usuario
    /// </summary>
    private bool ShouldSendEmail(ApplicationUser user)
    {
        if (string.IsNullOrEmpty(user.Email))
        {
            _logger.LogDebug("User {UserId} has no email address", user.Id);
            return false;
        }

        if (!user.IsActive)
        {
            _logger.LogDebug("User {UserId} is not active", user.Id);
            return false;
        }

        // Verificar preferencia de notificaciones (si está implementada)
        if (!user.EmailNotificationsEnabled)
        {
            _logger.LogDebug("User {UserId} has disabled email notifications", user.Id);
            return false;
        }

        return true;
    }
}
