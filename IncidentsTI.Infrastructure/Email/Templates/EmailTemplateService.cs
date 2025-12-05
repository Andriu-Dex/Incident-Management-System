using IncidentsTI.Domain.Entities;
using IncidentsTI.Domain.Enums;

namespace IncidentsTI.Infrastructure.Email.Templates;

/// <summary>
/// Servicio para generar plantillas de email HTML
/// </summary>
public static class EmailTemplateService
{
    private const string PrimaryColor = "#3B82F6";
    private const string SuccessColor = "#22C55E";
    private const string WarningColor = "#F59E0B";
    private const string DangerColor = "#EF4444";
    private const string InfoColor = "#06B6D4";

    /// <summary>
    /// Genera la plantilla base HTML para todos los emails
    /// </summary>
    private static string GetBaseTemplate(string title, string content, string? footerText = null)
    {
        return $@"
<!DOCTYPE html>
<html lang=""es"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>{title}</title>
    <style>
        * {{
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }}
        body {{
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
            line-height: 1.6;
            color: #1f2937;
            background-color: #f3f4f6;
        }}
        .container {{
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
        }}
        .email-wrapper {{
            background-color: #ffffff;
            border-radius: 12px;
            box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
            overflow: hidden;
        }}
        .header {{
            background: linear-gradient(135deg, {PrimaryColor} 0%, #1d4ed8 100%);
            color: white;
            padding: 30px 20px;
            text-align: center;
        }}
        .header h1 {{
            font-size: 24px;
            font-weight: 700;
            margin-bottom: 5px;
        }}
        .header p {{
            font-size: 14px;
            opacity: 0.9;
        }}
        .logo {{
            width: 60px;
            height: 60px;
            background: white;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            margin: 0 auto 15px;
            font-size: 28px;
        }}
        .content {{
            padding: 30px 25px;
        }}
        .greeting {{
            font-size: 18px;
            font-weight: 600;
            margin-bottom: 15px;
            color: #111827;
        }}
        .message {{
            color: #4b5563;
            margin-bottom: 20px;
        }}
        .info-card {{
            background-color: #f9fafb;
            border: 1px solid #e5e7eb;
            border-radius: 8px;
            padding: 20px;
            margin: 20px 0;
        }}
        .info-row {{
            display: flex;
            padding: 8px 0;
            border-bottom: 1px solid #e5e7eb;
        }}
        .info-row:last-child {{
            border-bottom: none;
        }}
        .info-label {{
            font-weight: 600;
            color: #6b7280;
            width: 140px;
            flex-shrink: 0;
        }}
        .info-value {{
            color: #111827;
        }}
        .button {{
            display: inline-block;
            padding: 14px 28px;
            background: linear-gradient(135deg, {PrimaryColor} 0%, #1d4ed8 100%);
            color: white !important;
            text-decoration: none;
            border-radius: 8px;
            font-weight: 600;
            font-size: 14px;
            margin: 20px 0;
            text-align: center;
        }}
        .button:hover {{
            opacity: 0.9;
        }}
        .button-success {{
            background: linear-gradient(135deg, {SuccessColor} 0%, #16a34a 100%);
        }}
        .button-warning {{
            background: linear-gradient(135deg, {WarningColor} 0%, #d97706 100%);
        }}
        .status-badge {{
            display: inline-block;
            padding: 4px 12px;
            border-radius: 20px;
            font-size: 12px;
            font-weight: 600;
            text-transform: uppercase;
        }}
        .status-open {{ background-color: #dbeafe; color: #1d4ed8; }}
        .status-inprogress {{ background-color: #fef3c7; color: #d97706; }}
        .status-escalated {{ background-color: #fee2e2; color: #dc2626; }}
        .status-resolved {{ background-color: #d1fae5; color: #059669; }}
        .status-closed {{ background-color: #e5e7eb; color: #4b5563; }}
        .priority-low {{ background-color: #d1fae5; color: #059669; }}
        .priority-medium {{ background-color: #fef3c7; color: #d97706; }}
        .priority-high {{ background-color: #fed7aa; color: #ea580c; }}
        .priority-critical {{ background-color: #fee2e2; color: #dc2626; }}
        .footer {{
            background-color: #f9fafb;
            padding: 20px;
            text-align: center;
            border-top: 1px solid #e5e7eb;
        }}
        .footer p {{
            color: #6b7280;
            font-size: 12px;
            margin: 5px 0;
        }}
        .footer a {{
            color: {PrimaryColor};
            text-decoration: none;
        }}
        .divider {{
            height: 1px;
            background-color: #e5e7eb;
            margin: 20px 0;
        }}
        .alert {{
            padding: 15px;
            border-radius: 8px;
            margin: 15px 0;
        }}
        .alert-info {{
            background-color: #dbeafe;
            border-left: 4px solid {PrimaryColor};
            color: #1e40af;
        }}
        .alert-success {{
            background-color: #d1fae5;
            border-left: 4px solid {SuccessColor};
            color: #065f46;
        }}
        .alert-warning {{
            background-color: #fef3c7;
            border-left: 4px solid {WarningColor};
            color: #92400e;
        }}
        .alert-danger {{
            background-color: #fee2e2;
            border-left: 4px solid {DangerColor};
            color: #991b1b;
        }}
        @media only screen and (max-width: 600px) {{
            .container {{
                padding: 10px;
            }}
            .content {{
                padding: 20px 15px;
            }}
            .info-row {{
                flex-direction: column;
            }}
            .info-label {{
                width: 100%;
                margin-bottom: 4px;
            }}
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""email-wrapper"">
            <div class=""header"">
                <div class=""logo"">🎫</div>
                <h1>Sistema de Incidentes</h1>
                <p>Universidad Técnica de Ambato</p>
            </div>
            <div class=""content"">
                {content}
            </div>
            <div class=""footer"">
                <p>{footerText ?? "Este es un mensaje automático del Sistema de Gestión de Incidentes de TI."}</p>
                <p>© {DateTime.Now.Year} Universidad Técnica de Ambato - Oficina de TI</p>
                <p><a href=""{{{{BaseUrl}}}}"">Acceder al Sistema</a></p>
            </div>
        </div>
    </div>
</body>
</html>";
    }

    /// <summary>
    /// Plantilla para incidente creado
    /// </summary>
    public static string GetIncidentCreatedTemplate(Incident incident, ApplicationUser user, string baseUrl)
    {
        var priorityClass = GetPriorityClass(incident.Priority);
        var statusClass = GetStatusClass(incident.Status);

        var content = $@"
            <p class=""greeting"">¡Hola {user.FirstName}!</p>
            <p class=""message"">Tu incidente ha sido registrado exitosamente en nuestro sistema. Nuestro equipo técnico lo revisará a la brevedad.</p>
            
            <div class=""info-card"">
                <div class=""info-row"">
                    <span class=""info-label"">Número de Ticket:</span>
                    <span class=""info-value""><strong>{incident.TicketNumber}</strong></span>
                </div>
                <div class=""info-row"">
                    <span class=""info-label"">Título:</span>
                    <span class=""info-value"">{incident.Title}</span>
                </div>
                <div class=""info-row"">
                    <span class=""info-label"">Servicio:</span>
                    <span class=""info-value"">{incident.Service?.Name ?? "No especificado"}</span>
                </div>
                <div class=""info-row"">
                    <span class=""info-label"">Prioridad:</span>
                    <span class=""info-value""><span class=""status-badge {priorityClass}"">{GetPriorityText(incident.Priority)}</span></span>
                </div>
                <div class=""info-row"">
                    <span class=""info-label"">Estado:</span>
                    <span class=""info-value""><span class=""status-badge {statusClass}"">{GetStatusText(incident.Status)}</span></span>
                </div>
                <div class=""info-row"">
                    <span class=""info-label"">Fecha:</span>
                    <span class=""info-value"">{incident.CreatedAt:dd/MM/yyyy HH:mm}</span>
                </div>
            </div>

            <p class=""message"">Puedes consultar el estado de tu incidente en cualquier momento haciendo clic en el botón de abajo.</p>
            
            <div style=""text-align: center;"">
                <a href=""{baseUrl}/incidents/{incident.Id}"" class=""button"">Ver Mi Incidente</a>
            </div>

            <div class=""alert alert-info"">
                <strong>💡 Consejo:</strong> Recibirás notificaciones por email cuando haya actualizaciones en tu incidente.
            </div>";

        return GetBaseTemplate("Incidente Creado - " + incident.TicketNumber, content)
            .Replace("{{BaseUrl}}", baseUrl);
    }

    /// <summary>
    /// Plantilla para incidente asignado
    /// </summary>
    public static string GetIncidentAssignedTemplate(Incident incident, ApplicationUser assignedTo, ApplicationUser assignedBy, string baseUrl)
    {
        var priorityClass = GetPriorityClass(incident.Priority);

        var content = $@"
            <p class=""greeting"">¡Hola {assignedTo.FirstName}!</p>
            <p class=""message"">Se te ha asignado un nuevo incidente para su atención.</p>
            
            <div class=""info-card"">
                <div class=""info-row"">
                    <span class=""info-label"">Número de Ticket:</span>
                    <span class=""info-value""><strong>{incident.TicketNumber}</strong></span>
                </div>
                <div class=""info-row"">
                    <span class=""info-label"">Título:</span>
                    <span class=""info-value"">{incident.Title}</span>
                </div>
                <div class=""info-row"">
                    <span class=""info-label"">Servicio:</span>
                    <span class=""info-value"">{incident.Service?.Name ?? "No especificado"}</span>
                </div>
                <div class=""info-row"">
                    <span class=""info-label"">Prioridad:</span>
                    <span class=""info-value""><span class=""status-badge {priorityClass}"">{GetPriorityText(incident.Priority)}</span></span>
                </div>
                <div class=""info-row"">
                    <span class=""info-label"">Asignado por:</span>
                    <span class=""info-value"">{assignedBy.FirstName} {assignedBy.LastName}</span>
                </div>
                <div class=""info-row"">
                    <span class=""info-label"">Reportado por:</span>
                    <span class=""info-value"">{incident.User?.FirstName} {incident.User?.LastName}</span>
                </div>
            </div>

            <p class=""message""><strong>Descripción del problema:</strong></p>
            <div class=""alert alert-info"">
                {incident.Description}
            </div>
            
            <div style=""text-align: center;"">
                <a href=""{baseUrl}/technician/dashboard"" class=""button"">Ir a Mi Dashboard</a>
            </div>";

        return GetBaseTemplate("Incidente Asignado - " + incident.TicketNumber, content)
            .Replace("{{BaseUrl}}", baseUrl);
    }

    /// <summary>
    /// Plantilla para cambio de estado
    /// </summary>
    public static string GetStatusChangedTemplate(Incident incident, ApplicationUser user, IncidentStatus oldStatus, IncidentStatus newStatus, string baseUrl)
    {
        var newStatusClass = GetStatusClass(newStatus);
        var alertClass = newStatus switch
        {
            IncidentStatus.Resolved => "alert-success",
            IncidentStatus.Closed => "alert-info",
            IncidentStatus.Escalated => "alert-warning",
            _ => "alert-info"
        };

        var content = $@"
            <p class=""greeting"">¡Hola {user.FirstName}!</p>
            <p class=""message"">El estado de tu incidente ha sido actualizado.</p>
            
            <div class=""alert {alertClass}"">
                <strong>Estado actualizado:</strong> {GetStatusText(oldStatus)} → <span class=""status-badge {newStatusClass}"">{GetStatusText(newStatus)}</span>
            </div>

            <div class=""info-card"">
                <div class=""info-row"">
                    <span class=""info-label"">Número de Ticket:</span>
                    <span class=""info-value""><strong>{incident.TicketNumber}</strong></span>
                </div>
                <div class=""info-row"">
                    <span class=""info-label"">Título:</span>
                    <span class=""info-value"">{incident.Title}</span>
                </div>
                <div class=""info-row"">
                    <span class=""info-label"">Nuevo Estado:</span>
                    <span class=""info-value""><span class=""status-badge {newStatusClass}"">{GetStatusText(newStatus)}</span></span>
                </div>
            </div>
            
            <div style=""text-align: center;"">
                <a href=""{baseUrl}/incidents/{incident.Id}"" class=""button"">Ver Detalles</a>
            </div>";

        return GetBaseTemplate("Actualización de Incidente - " + incident.TicketNumber, content)
            .Replace("{{BaseUrl}}", baseUrl);
    }

    /// <summary>
    /// Plantilla para comentario agregado
    /// </summary>
    public static string GetCommentAddedTemplate(Incident incident, ApplicationUser user, IncidentComment comment, ApplicationUser commenter, string baseUrl)
    {
        var content = $@"
            <p class=""greeting"">¡Hola {user.FirstName}!</p>
            <p class=""message"">Se ha agregado un nuevo comentario a tu incidente.</p>
            
            <div class=""info-card"">
                <div class=""info-row"">
                    <span class=""info-label"">Número de Ticket:</span>
                    <span class=""info-value""><strong>{incident.TicketNumber}</strong></span>
                </div>
                <div class=""info-row"">
                    <span class=""info-label"">Comentado por:</span>
                    <span class=""info-value"">{commenter.FirstName} {commenter.LastName}</span>
                </div>
                <div class=""info-row"">
                    <span class=""info-label"">Fecha:</span>
                    <span class=""info-value"">{comment.CreatedAt:dd/MM/yyyy HH:mm}</span>
                </div>
            </div>

            <p class=""message""><strong>Comentario:</strong></p>
            <div class=""alert alert-info"">
                {comment.Content}
            </div>
            
            <div style=""text-align: center;"">
                <a href=""{baseUrl}/incidents/{incident.Id}"" class=""button"">Ver Conversación</a>
            </div>";

        return GetBaseTemplate("Nuevo Comentario - " + incident.TicketNumber, content)
            .Replace("{{BaseUrl}}", baseUrl);
    }

    /// <summary>
    /// Plantilla para incidente escalado
    /// </summary>
    public static string GetIncidentEscalatedTemplate(Incident incident, ApplicationUser user, string fromLevel, string toLevel, string reason, string baseUrl)
    {
        var content = $@"
            <p class=""greeting"">¡Hola {user.FirstName}!</p>
            <p class=""message"">Tu incidente ha sido escalado a un nivel superior de soporte para una atención más especializada.</p>
            
            <div class=""alert alert-warning"">
                <strong>⬆️ Escalamiento:</strong> {fromLevel} → {toLevel}
            </div>

            <div class=""info-card"">
                <div class=""info-row"">
                    <span class=""info-label"">Número de Ticket:</span>
                    <span class=""info-value""><strong>{incident.TicketNumber}</strong></span>
                </div>
                <div class=""info-row"">
                    <span class=""info-label"">Título:</span>
                    <span class=""info-value"">{incident.Title}</span>
                </div>
                <div class=""info-row"">
                    <span class=""info-label"">Nuevo Nivel:</span>
                    <span class=""info-value""><strong>{toLevel}</strong></span>
                </div>
            </div>

            <p class=""message""><strong>Motivo del escalamiento:</strong></p>
            <div class=""alert alert-info"">
                {reason}
            </div>
            
            <div style=""text-align: center;"">
                <a href=""{baseUrl}/incidents/{incident.Id}"" class=""button button-warning"">Ver Incidente</a>
            </div>";

        return GetBaseTemplate("Incidente Escalado - " + incident.TicketNumber, content)
            .Replace("{{BaseUrl}}", baseUrl);
    }

    /// <summary>
    /// Plantilla para incidente resuelto
    /// </summary>
    public static string GetIncidentResolvedTemplate(Incident incident, ApplicationUser user, string? resolution, string baseUrl)
    {
        var resolutionSection = !string.IsNullOrEmpty(resolution) 
            ? $@"<p class=""message""><strong>Solución aplicada:</strong></p>
                 <div class=""alert alert-success"">{resolution}</div>" 
            : "";

        var content = $@"
            <p class=""greeting"">¡Hola {user.FirstName}!</p>
            <p class=""message"">¡Buenas noticias! Tu incidente ha sido resuelto.</p>
            
            <div class=""alert alert-success"">
                <strong>✅ Estado:</strong> Tu incidente ha sido marcado como <strong>Resuelto</strong>
            </div>

            <div class=""info-card"">
                <div class=""info-row"">
                    <span class=""info-label"">Número de Ticket:</span>
                    <span class=""info-value""><strong>{incident.TicketNumber}</strong></span>
                </div>
                <div class=""info-row"">
                    <span class=""info-label"">Título:</span>
                    <span class=""info-value"">{incident.Title}</span>
                </div>
                <div class=""info-row"">
                    <span class=""info-label"">Resuelto el:</span>
                    <span class=""info-value"">{DateTime.UtcNow:dd/MM/yyyy HH:mm}</span>
                </div>
            </div>

            {resolutionSection}

            <p class=""message"">Por favor, verifica que el problema haya sido solucionado correctamente. Si aún tienes inconvenientes, puedes reabrir el incidente o crear uno nuevo.</p>
            
            <div style=""text-align: center;"">
                <a href=""{baseUrl}/incidents/{incident.Id}"" class=""button button-success"">Ver Solución</a>
            </div>";

        return GetBaseTemplate("Incidente Resuelto - " + incident.TicketNumber, content)
            .Replace("{{BaseUrl}}", baseUrl);
    }

    /// <summary>
    /// Plantilla para incidente cerrado
    /// </summary>
    public static string GetIncidentClosedTemplate(Incident incident, ApplicationUser user, string baseUrl)
    {
        var content = $@"
            <p class=""greeting"">¡Hola {user.FirstName}!</p>
            <p class=""message"">Tu incidente ha sido cerrado oficialmente.</p>
            
            <div class=""info-card"">
                <div class=""info-row"">
                    <span class=""info-label"">Número de Ticket:</span>
                    <span class=""info-value""><strong>{incident.TicketNumber}</strong></span>
                </div>
                <div class=""info-row"">
                    <span class=""info-label"">Título:</span>
                    <span class=""info-value"">{incident.Title}</span>
                </div>
                <div class=""info-row"">
                    <span class=""info-label"">Estado Final:</span>
                    <span class=""info-value""><span class=""status-badge status-closed"">Cerrado</span></span>
                </div>
                <div class=""info-row"">
                    <span class=""info-label"">Cerrado el:</span>
                    <span class=""info-value"">{DateTime.UtcNow:dd/MM/yyyy HH:mm}</span>
                </div>
            </div>

            <p class=""message"">Gracias por utilizar el Sistema de Gestión de Incidentes. Si necesitas ayuda en el futuro, no dudes en crear un nuevo ticket.</p>
            
            <div style=""text-align: center;"">
                <a href=""{baseUrl}/create-incident"" class=""button"">Crear Nuevo Incidente</a>
            </div>";

        return GetBaseTemplate("Incidente Cerrado - " + incident.TicketNumber, content)
            .Replace("{{BaseUrl}}", baseUrl);
    }

    /// <summary>
    /// Plantilla para recuperación de contraseña
    /// </summary>
    public static string GetPasswordResetTemplate(ApplicationUser user, string resetUrl, string baseUrl)
    {
        var content = $@"
            <p class=""greeting"">¡Hola {user.FirstName}!</p>
            <p class=""message"">Hemos recibido una solicitud para restablecer la contraseña de tu cuenta.</p>
            
            <div class=""alert alert-warning"">
                <strong>⚠️ Importante:</strong> Este enlace expirará en <strong>1 hora</strong>.
            </div>

            <p class=""message"">Si solicitaste este cambio, haz clic en el botón de abajo para crear una nueva contraseña:</p>
            
            <div style=""text-align: center;"">
                <a href=""{resetUrl}"" class=""button"">Restablecer Contraseña</a>
            </div>

            <div class=""divider""></div>

            <p class=""message"" style=""font-size: 13px; color: #6b7280;"">
                Si no solicitaste este cambio, puedes ignorar este correo. Tu contraseña actual seguirá siendo válida.
            </p>
            
            <p class=""message"" style=""font-size: 12px; color: #9ca3af;"">
                Por seguridad, este enlace solo puede usarse una vez. Si necesitas otro enlace, solicita uno nuevo desde la página de inicio de sesión.
            </p>";

        return GetBaseTemplate("Restablecer Contraseña", content, "Este enlace expirará en 1 hora.")
            .Replace("{{BaseUrl}}", baseUrl);
    }

    /// <summary>
    /// Plantilla para bienvenida de nuevo usuario
    /// </summary>
    public static string GetWelcomeTemplate(ApplicationUser user, string temporaryPassword, string baseUrl)
    {
        var content = $@"
            <p class=""greeting"">¡Bienvenido/a {user.FirstName}!</p>
            <p class=""message"">Tu cuenta en el Sistema de Gestión de Incidentes de TI ha sido creada exitosamente.</p>
            
            <div class=""info-card"">
                <div class=""info-row"">
                    <span class=""info-label"">Email:</span>
                    <span class=""info-value""><strong>{user.Email}</strong></span>
                </div>
                <div class=""info-row"">
                    <span class=""info-label"">Contraseña temporal:</span>
                    <span class=""info-value""><code style=""background: #f3f4f6; padding: 4px 8px; border-radius: 4px;"">{temporaryPassword}</code></span>
                </div>
            </div>

            <div class=""alert alert-warning"">
                <strong>⚠️ Importante:</strong> Por seguridad, te recomendamos cambiar tu contraseña temporal después de iniciar sesión por primera vez.
            </div>
            
            <div style=""text-align: center;"">
                <a href=""{baseUrl}/login"" class=""button"">Iniciar Sesión</a>
            </div>

            <p class=""message"">Si tienes alguna duda, no dudes en contactar con el equipo de soporte técnico.</p>";

        return GetBaseTemplate("Bienvenido al Sistema de Incidentes", content)
            .Replace("{{BaseUrl}}", baseUrl);
    }

    #region Helper Methods

    private static string GetStatusText(IncidentStatus status) => status switch
    {
        IncidentStatus.Open => "Abierto",
        IncidentStatus.InProgress => "En Progreso",
        IncidentStatus.Escalated => "Escalado",
        IncidentStatus.Resolved => "Resuelto",
        IncidentStatus.Closed => "Cerrado",
        _ => status.ToString()
    };

    private static string GetStatusClass(IncidentStatus status) => status switch
    {
        IncidentStatus.Open => "status-open",
        IncidentStatus.InProgress => "status-inprogress",
        IncidentStatus.Escalated => "status-escalated",
        IncidentStatus.Resolved => "status-resolved",
        IncidentStatus.Closed => "status-closed",
        _ => "status-open"
    };

    private static string GetPriorityText(IncidentPriority priority) => priority switch
    {
        IncidentPriority.Low => "Baja",
        IncidentPriority.Medium => "Media",
        IncidentPriority.High => "Alta",
        IncidentPriority.Critical => "Crítica",
        _ => priority.ToString()
    };

    private static string GetPriorityClass(IncidentPriority priority) => priority switch
    {
        IncidentPriority.Low => "priority-low",
        IncidentPriority.Medium => "priority-medium",
        IncidentPriority.High => "priority-high",
        IncidentPriority.Critical => "priority-critical",
        _ => "priority-medium"
    };

    #endregion
}
