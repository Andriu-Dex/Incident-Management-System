namespace IncidentsTI.Infrastructure.Email;

/// <summary>
/// Configuración para el servicio de email SMTP
/// </summary>
public class EmailSettings
{
    /// <summary>
    /// Nombre de la sección en appsettings.json
    /// </summary>
    public const string SectionName = "EmailSettings";

    /// <summary>
    /// Servidor SMTP (ej: smtp.gmail.com)
    /// </summary>
    public string SmtpServer { get; set; } = string.Empty;

    /// <summary>
    /// Puerto SMTP (ej: 587 para TLS, 465 para SSL)
    /// </summary>
    public int SmtpPort { get; set; } = 587;

    /// <summary>
    /// Usar conexión segura SSL/TLS
    /// </summary>
    public bool UseSsl { get; set; } = true;

    /// <summary>
    /// Email del remitente
    /// </summary>
    public string SenderEmail { get; set; } = string.Empty;

    /// <summary>
    /// Nombre del remitente que aparecerá en los emails
    /// </summary>
    public string SenderName { get; set; } = "Sistema de Incidentes UTA";

    /// <summary>
    /// Usuario para autenticación SMTP
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Contraseña para autenticación SMTP
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Habilitar o deshabilitar el envío de emails
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// URL base de la aplicación para generar enlaces en los emails
    /// </summary>
    public string BaseUrl { get; set; } = "https://localhost:7117";

    /// <summary>
    /// Tiempo de espera en segundos para la conexión SMTP
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;
}
