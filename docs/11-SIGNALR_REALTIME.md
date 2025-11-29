# 11. Comunicación en Tiempo Real con SignalR

El sistema implementa comunicación bidireccional en tiempo real utilizando **SignalR** integrado con Blazor Server. Esto permite que los usuarios reciban actualizaciones instantáneas sin necesidad de recargar la página.

## Arquitectura

```
┌─────────────────────────────────────────────────────────────────┐
│                        Cliente (Browser)                         │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐  │
│  │ TechnicianDash  │  │  AdminIncidents │  │   MyIncidents   │  │
│  └────────┬────────┘  └────────┬────────┘  └────────┬────────┘  │
│           │                    │                    │            │
│           └────────────────────┼────────────────────┘            │
│                                │                                 │
│                    ┌───────────▼───────────┐                     │
│                    │  signalr-client.js    │                     │
│                    │  (Auto-conexión)      │                     │
│                    └───────────┬───────────┘                     │
└────────────────────────────────┼────────────────────────────────┘
                                 │ WebSocket
                                 │
┌────────────────────────────────┼────────────────────────────────┐
│                        Servidor                                  │
│                    ┌───────────▼───────────┐                     │
│                    │   NotificationHub     │                     │
│                    │   /hubs/notifications │                     │
│                    └───────────┬───────────┘                     │
│                                │                                 │
│           ┌────────────────────┼────────────────────┐            │
│           │                    │                    │            │
│  ┌────────▼────────┐  ┌────────▼────────┐  ┌───────▼───────┐    │
│  │    Admins       │  │   Technicians   │  │    Users      │    │
│  │    (Group)      │  │    (Group)      │  │   (Group)     │    │
│  └─────────────────┘  └─────────────────┘  └───────────────┘    │
│                                                                  │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │          RealTimeNotificationDecorator                    │   │
│  │  (Intercepta INotificationService para enviar eventos)   │   │
│  └──────────────────────────────────────────────────────────┘   │
└──────────────────────────────────────────────────────────────────┘
```

## Componentes Principales

### 1. NotificationHub (`Hubs/NotificationHub.cs`)

Hub central de SignalR que maneja las conexiones y grupos.

```csharp
[Authorize]
public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        // Auto-asignar usuarios a grupos según su rol
        if (user?.IsInRole("Administrador") == true)
            await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
        if (user?.IsInRole("Tecnico") == true)
            await Groups.AddToGroupAsync(Context.ConnectionId, "Technicians");
        if (user?.IsInRole("Usuario") == true)
            await Groups.AddToGroupAsync(Context.ConnectionId, "Users");
    }

    public async Task Ping()
    {
        await Clients.Caller.SendAsync("Pong", DateTime.UtcNow);
    }
}
```

**Grupos disponibles:**
| Grupo | Descripción |
|-------|-------------|
| `Admins` | Administradores del sistema |
| `Technicians` | Técnicos de soporte |
| `Users` | Usuarios finales |

### 2. IRealTimeNotificationService (`Hubs/Services/IRealTimeNotificationService.cs`)

Interfaz para enviar eventos en tiempo real.

```csharp
public interface IRealTimeNotificationService
{
    // Notificaciones Toast
    Task NotifyUserAsync(string userId, string title, string message, string? url = null);
    Task NotifyAllAsync(string title, string message, string? url = null);
    Task NotifyGroupAsync(string groupName, string title, string message, string? url = null);
    
    // Actualizaciones de Datos
    Task SendIncidentUpdateAsync(int incidentId, string action);
    Task SendDashboardRefreshAsync(string targetGroup);
    Task SendNotificationCountUpdateAsync(string userId, int count);
    
    // Eventos Genéricos
    Task SendEventToGroupAsync(string groupName, string eventName, object data);
    Task SendEventToUserAsync(string userId, string eventName, object data);
    Task SendEventToAllAsync(string eventName, object data);
}
```

### 3. RealTimeNotificationDecorator (`Services/RealTimeNotificationDecorator.cs`)

Decorador que intercepta las operaciones de `INotificationService` para enviar eventos en tiempo real automáticamente.

```csharp
public class RealTimeNotificationDecorator : INotificationService
{
    public async Task NotifyIncidentCreatedAsync(Incident incident)
    {
        // 1. Guardar en BD (servicio original)
        await _inner.NotifyIncidentCreatedAsync(incident);

        // 2. Enviar toast en tiempo real
        await _realTimeService.NotifyGroupAsync("Technicians", 
            "🆕 Nuevo Incidente", 
            $"{incident.TicketNumber} - {incident.Title}");

        // 3. Disparar evento para refrescar listas
        await _realTimeService.SendIncidentUpdateAsync(incident.Id, "created");
        await _realTimeService.SendDashboardRefreshAsync("Technicians");
    }
}
```

### 4. Cliente JavaScript (`wwwroot/js/signalr-client.js`)

Cliente que maneja la conexión SignalR y los eventos.

```javascript
// Inicialización automática
window.SignalRNotifications = {
    connection: null,
    isConnected: false
};

// Eventos soportados
connection.on("ReceiveNotification", (notification) => {
    showNotificationToast(notification);
});

connection.on("IncidentUpdated", (data) => {
    triggerEvent('incident-updated', data);
});

connection.on("DashboardRefresh", (data) => {
    triggerEvent('dashboard-refresh', data);
});

connection.on("NotificationCountUpdated", (count) => {
    updateNotificationBadge(count);
});
```

## Eventos Disponibles

### Eventos del Servidor → Cliente

| Evento | Descripción | Datos |
|--------|-------------|-------|
| `ReceiveNotification` | Muestra un toast de notificación | `{title, message, url, timestamp}` |
| `IncidentUpdated` | Indica que un incidente cambió | `{incidentId, action, timestamp}` |
| `DashboardRefresh` | Solicita refrescar el dashboard | `{timestamp}` |
| `NotificationCountUpdated` | Actualiza el contador de notificaciones | `count (int)` |
| `Pong` | Respuesta al ping | `timestamp` |

### Acciones de Incidentes

| Acción | Cuándo se dispara |
|--------|-------------------|
| `created` | Nuevo incidente creado |
| `status-changed` | Cambio de estado |
| `assigned` | Incidente asignado |
| `resolved` | Incidente resuelto |
| `closed` | Incidente cerrado |

## Integración en Componentes Blazor

### Patrón de Integración

```razor
@inject IJSRuntime JSRuntime
@implements IDisposable

@code {
    private DotNetObjectReference<MiComponente>? dotNetRef;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            dotNetRef = DotNetObjectReference.Create(this);
            await JSRuntime.InvokeVoidAsync("registerSignalRCallback", 
                dotNetRef, "incident-updated", "OnIncidentUpdated");
        }
    }

    [JSInvokable]
    public async Task OnIncidentUpdated(object data)
    {
        await LoadData();
        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        if (dotNetRef != null)
        {
            _ = JSRuntime.InvokeVoidAsync("unregisterSignalRCallback", 
                dotNetRef, "incident-updated");
            dotNetRef.Dispose();
        }
    }
}
```

### Componentes Integrados

| Componente | Eventos Escuchados | Acción |
|------------|-------------------|--------|
| `TechnicianDashboard` | `incident-updated`, `dashboard-refresh` | Recarga lista de incidentes |
| `AdminIncidents` | `incident-updated`, `dashboard-refresh` | Recarga lista de incidentes |
| `MyIncidents` | `incident-updated` | Recarga incidentes del usuario |
| `NotificationBell` | `notification-count-updated` | Actualiza contador del badge |

## Configuración

### Program.cs

```csharp
// El servicio SignalR ya está configurado por Blazor Server
// Solo necesitamos mapear el Hub y registrar servicios

// Registrar servicio de tiempo real
builder.Services.AddScoped<IRealTimeNotificationService, RealTimeNotificationService>();

// Decorador para INotificationService (automático)
builder.Services.Decorate<INotificationService, RealTimeNotificationDecorator>();

// Mapear el Hub
app.MapHub<NotificationHub>("/hubs/notifications");
```

### App.razor

```html
<!-- SignalR desde CDN -->
<script src="https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/8.0.0/signalr.min.js"></script>
<script src="js/signalr-client.js"></script>
```

## Características

### Auto-Conexión
El cliente se conecta automáticamente cuando la página carga (si el usuario está autenticado).

### Reconexión Automática
```javascript
.withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
```

Intentos de reconexión con backoff exponencial:
1. Inmediato
2. 2 segundos
3. 5 segundos
4. 10 segundos
5. 30 segundos

### Notificaciones Toast
Los toasts aparecen en la esquina superior derecha y se auto-eliminan después de 5 segundos.

```javascript
showNotificationToast({
    title: "🆕 Nuevo Incidente",
    message: "INC-2024-0001 - Problema con impresora",
    url: "/incidents/1"
});
```

## Uso Manual

### Desde JavaScript

```javascript
// Verificar conexión
console.log(window.SignalRNotifications.isConnected);

// Enviar ping
await pingSignalR();

// Mostrar toast manualmente
showNotificationToast({
    title: "Test",
    message: "Mensaje de prueba"
});

// Escuchar eventos personalizados
onSignalREvent('incident-updated', (data) => {
    console.log('Incidente actualizado:', data);
});
```

### Desde C# (Backend)

```csharp
// Inyectar el servicio
private readonly IRealTimeNotificationService _realTime;

// Enviar notificación a un usuario
await _realTime.NotifyUserAsync(userId, "Título", "Mensaje", "/url");

// Enviar a un grupo
await _realTime.NotifyGroupAsync("Technicians", "Título", "Mensaje");

// Evento personalizado
await _realTime.SendEventToGroupAsync("Admins", "custom-event", new { data = "valor" });
```

## Flujo de Ejemplo: Nuevo Incidente

```
1. Usuario crea incidente
   │
2. IncidentService.CreateAsync()
   │
3. NotificationService.NotifyIncidentCreatedAsync()
   │  ↓ (interceptado por decorador)
   │
4. RealTimeNotificationDecorator
   ├── Guarda notificación en BD
   ├── Envía toast a Technicians y Admins
   ├── Dispara evento "IncidentUpdated"
   └── Dispara evento "DashboardRefresh"
   │
5. Clientes reciben eventos
   │
6. TechnicianDashboard.OnIncidentUpdated()
   │
7. Lista se actualiza automáticamente
```

## Consideraciones de Seguridad

- El Hub requiere autenticación (`[Authorize]`)
- Los usuarios solo se agregan a grupos según sus roles verificados
- Los mensajes se envían solo a grupos/usuarios autorizados

## Troubleshooting

### La conexión no se establece
1. Verificar que el usuario esté autenticado
2. Revisar consola del navegador para errores
3. Verificar que el script de SignalR esté cargado

### Los eventos no llegan
1. Verificar que el componente esté registrado con `registerSignalRCallback`
2. Confirmar que el método `[JSInvokable]` existe
3. Revisar logs del servidor

### Reconexión constante
1. Verificar estabilidad de la red
2. Revisar timeouts del servidor
3. Confirmar que no hay conflictos con proxies/firewalls

## Archivos Relacionados

```
IncidentsTI.Web/
├── Hubs/
│   ├── NotificationHub.cs
│   └── Services/
│       ├── IRealTimeNotificationService.cs
│       └── RealTimeNotificationService.cs
├── Services/
│   └── RealTimeNotificationDecorator.cs
├── wwwroot/js/
│   └── signalr-client.js
├── Components/
│   ├── App.razor (scripts)
│   └── Pages/
│       ├── TechnicianDashboard.razor
│       ├── AdminIncidents.razor
│       └── MyIncidents.razor
└── Program.cs (configuración)
```
