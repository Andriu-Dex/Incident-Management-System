using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace IncidentsTI.Web.Hubs;

/// <summary>
/// Hub de SignalR para notificaciones en tiempo real.
/// Versión mínima para verificar compatibilidad con Blazor Server.
/// </summary>
[Authorize]
public class NotificationHub : Hub
{
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(ILogger<NotificationHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        var user = Context.User;
        
        _logger.LogInformation("Usuario conectado al NotificationHub. ConnectionId: {ConnectionId}, UserId: {UserId}", 
            Context.ConnectionId, userId);

        // Agregar a grupos según rol
        if (user?.IsInRole("Administrador") == true)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
        }
        if (user?.IsInRole("Tecnico") == true)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Technicians");
        }
        if (user?.IsInRole("Usuario") == true)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Users");
        }
        
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Usuario desconectado del NotificationHub. ConnectionId: {ConnectionId}", 
            Context.ConnectionId);
        
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Método simple de ping para probar la conexión.
    /// </summary>
    public async Task Ping()
    {
        await Clients.Caller.SendAsync("Pong", DateTime.UtcNow);
    }
}
