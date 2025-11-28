# Fase 7 - Sistema de Notificaciones In-App

La Fase 7 implementa un sistema completo de notificaciones in-app que alerta a los usuarios sobre eventos importantes relacionados con sus incidentes. Las notificaciones se muestran en tiempo real a través de una campana en el header de la aplicación.

---

## Arquitectura Implementada

### Diagrama de Flujo

```
┌─────────────────┐     ┌──────────────────┐     ┌─────────────────┐
│   Command       │────▶│ NotificationService│────▶│ Notification    │
│   Handler       │     │                  │     │ Repository      │
│ (CreateIncident,│     │ NotifyIncident   │     │                 │
│  Assign, etc.)  │     │ CreatedAsync()   │     │ CreateAsync()   │
└─────────────────┘     └──────────────────┘     └────────┬────────┘
                                                          │
                                                          ▼
┌─────────────────┐     ┌──────────────────┐     ┌─────────────────┐
│ NotificationBell│◀────│    MediatR       │◀────│   Database      │
│    Component    │     │    Queries       │     │  (Notifications)│
└─────────────────┘     └──────────────────┘     └─────────────────┘
```

---

## Componentes Implementados

### 1. Domain Layer

| Archivo | Descripción |
|---------|-------------|
| `Enums/NotificationType.cs` | Enum con 10 tipos de notificación |
| `Entities/Notification.cs` | Entidad de notificación con todas las propiedades |
| `Interfaces/INotificationRepository.cs` | Contrato del repositorio |
| `Interfaces/INotificationService.cs` | Contrato del servicio de notificaciones |

#### NotificationType Enum

```csharp
public enum NotificationType
{
    IncidentCreated,      // Nuevo incidente creado
    StatusChanged,        // Cambio de estado
    IncidentAssigned,     // Asignación a técnico
    IncidentReassigned,   // Reasignación
    IncidentEscalated,    // Escalamiento
    IncidentResolved,     // Resolución
    IncidentClosed,       // Cierre
    CommentAdded,         // Nuevo comentario
    ArticleLinked,        // Artículo KB vinculado
    SystemMessage         // Mensaje del sistema
}
```

### 2. Application Layer

| Archivo | Descripción |
|---------|-------------|
| `DTOs/Notifications/NotificationDto.cs` | DTO para transferencia de datos |
| `Queries/GetUserNotificationsQuery.cs` | Query para obtener notificaciones |
| `Queries/GetUnreadNotificationCountQuery.cs` | Query para contar no leídas |
| `Commands/MarkNotificationAsReadCommand.cs` | Marcar como leída |
| `Commands/MarkAllNotificationsAsReadCommand.cs` | Marcar todas como leídas |
| `Commands/DeleteNotificationCommand.cs` | Eliminar notificación |
| `Handlers/*` | Handlers para queries y commands |

### 3. Infrastructure Layer

| Archivo | Descripción |
|---------|-------------|
| `Repositories/NotificationRepository.cs` | Implementación EF Core del repositorio |
| `Services/NotificationService.cs` | Lógica de generación de notificaciones |
| `Migrations/AddNotifications.cs` | Migración de base de datos |

### 4. Presentation Layer

| Archivo | Descripción |
|---------|-------------|
| `Components/Shared/NotificationBell.razor` | Componente de campana con dropdown |
| `Components/Layout/MainLayout.razor` | Integración del header con campana |
| `Components/App.razor` | Inclusión de Bootstrap Icons |

---

## Características del Sistema

### Campana de Notificaciones

- **Ubicación:** Header superior derecho
- **Badge:** Muestra contador de notificaciones no leídas
- **Auto-refresh:** Actualiza el contador cada 30 segundos
- **Dropdown:** Panel con lista de notificaciones al hacer clic

### Panel de Notificaciones

- **Header:** Título "Notificaciones" con botón "Marcar todas"
- **Lista:** Muestra las últimas 10 notificaciones
- **Indicadores:**
  - Fondo azul claro para no leídas
  - Punto azul para notificaciones nuevas
  - Icono según tipo de notificación
- **Footer:** Enlace "Ver todas las notificaciones"

### Iconos por Tipo

| Tipo | Icono | Color |
|------|-------|-------|
| Incidente Creado | `bi-plus-circle-fill` | Azul |
| Cambio de Estado | `bi-arrow-repeat` | Cyan |
| Asignación | `bi-person-fill-add` | Verde |
| Reasignación | `bi-person-fill-gear` | Amarillo |
| Escalamiento | `bi-arrow-up-circle-fill` | Rojo |
| Resolución | `bi-check-circle-fill` | Verde |
| Cierre | `bi-x-circle-fill` | Gris |
| Comentario | `bi-chat-left-text-fill` | Cyan |
| Artículo Vinculado | `bi-journal-bookmark-fill` | Púrpura |
| Sistema | `bi-gear-fill` | Oscuro |

---

## Eventos que Generan Notificaciones

### 1. Crear Incidente (`CreateIncidentCommandHandler`)
- **Al creador:** "Tu incidente ha sido registrado"
- **A técnicos:** "Nuevo incidente creado"
- **A administradores:** "Nuevo incidente creado"

### 2. Cambiar Estado (`UpdateIncidentStatusCommandHandler`)
- **Al creador:** "El estado de tu incidente cambió"
- **Al técnico asignado:** "El incidente cambió de estado"

### 3. Asignar Incidente (`AssignIncidentCommandHandler`)
- **Al técnico asignado:** "Se te ha asignado el incidente"
- **Al creador:** "Tu incidente ha sido asignado"

### 4. Reasignar Incidente (`AssignIncidentCommandHandler`)
- **Al técnico anterior:** "El incidente fue reasignado"
- **Al nuevo técnico:** "Se te ha asignado el incidente"
- **Al creador:** "Tu incidente fue reasignado"

### 5. Escalar Incidente (`EscalateIncidentCommandHandler`)
- **Al creador:** "Tu incidente ha sido escalado"
- **Al técnico destino:** "Escalamiento recibido"
- **A administradores:** "Incidente escalado"

### 6. Resolver Incidente (`ResolveIncidentCommandHandler`)
- **Al creador:** "Tu incidente ha sido resuelto"

### 7. Agregar Comentario (`AddCommentCommandHandler`)
- **Al creador y técnico:** "Nuevo comentario en el incidente"
- **Nota:** Solo comentarios públicos generan notificación

### 8. Vincular Artículo KB (`LinkArticleToIncidentCommandHandler`)
- **Al creador:** "Se ha vinculado un artículo de conocimiento"

---

## Base de Datos

### Tabla: Notifications

```sql
CREATE TABLE Notifications (
    Id INT PRIMARY KEY IDENTITY,
    UserId NVARCHAR(450) NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Message NVARCHAR(1000) NOT NULL,
    Type INT NOT NULL,
    RelatedEntityId INT NULL,
    RelatedEntityType NVARCHAR(100) NULL,
    ActionUrl NVARCHAR(500) NULL,
    IsRead BIT NOT NULL DEFAULT 0,
    ReadAt DATETIME2 NULL,
    CreatedAt DATETIME2 NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id)
);

-- Índices
CREATE INDEX IX_Notifications_UserId ON Notifications(UserId);
CREATE INDEX IX_Notifications_UserId_IsRead ON Notifications(UserId, IsRead);
CREATE INDEX IX_Notifications_CreatedAt ON Notifications(CreatedAt DESC);
```

---

## Registro de Servicios

```csharp
// Program.cs
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();
```

---

## Dependencias Agregadas

```html
<!-- Bootstrap Icons (App.razor) -->
<link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css">
```

---

## Commits de la Fase

1. `feat(phase-7): Implementar sistema de notificaciones in-app`
2. `fix(notifications): Corregir dropdown de NotificationBell`
3. `fix(notifications): Agregar Bootstrap Icons para iconos`

---

## Archivos Creados

```
IncidentsTI.Domain/
├── Enums/
│   └── NotificationType.cs
├── Entities/
│   └── Notification.cs
└── Interfaces/
    ├── INotificationRepository.cs
    └── INotificationService.cs

IncidentsTI.Application/
├── DTOs/
│   └── Notifications/
│       └── NotificationDto.cs
├── Queries/
│   ├── GetUserNotificationsQuery.cs
│   └── GetUnreadNotificationCountQuery.cs
├── Commands/
│   ├── MarkNotificationAsReadCommand.cs
│   ├── MarkAllNotificationsAsReadCommand.cs
│   └── DeleteNotificationCommand.cs
└── Handlers/
    ├── GetUserNotificationsQueryHandler.cs
    ├── GetUnreadNotificationCountQueryHandler.cs
    ├── MarkNotificationAsReadCommandHandler.cs
    ├── MarkAllNotificationsAsReadCommandHandler.cs
    └── DeleteNotificationCommandHandler.cs

IncidentsTI.Infrastructure/
├── Repositories/
│   └── NotificationRepository.cs
├── Services/
│   └── NotificationService.cs
└── Migrations/
    └── XXXXXXXX_AddNotifications.cs

IncidentsTI.Web/
└── Components/
    └── Shared/
        └── NotificationBell.razor
```

---

## Archivos Modificados

| Archivo | Cambio |
|---------|--------|
| `ApplicationDbContext.cs` | DbSet<Notification>, configuración de relaciones |
| `Program.cs` | Registro de servicios de notificación |
| `MainLayout.razor` | Header con NotificationBell |
| `App.razor` | Link a Bootstrap Icons |
| `CreateIncidentCommandHandler.cs` | Integración con NotificationService |
| `UpdateIncidentStatusCommandHandler.cs` | Integración con NotificationService |
| `AssignIncidentCommandHandler.cs` | Integración con NotificationService |
| `EscalateIncidentCommandHandler.cs` | Integración con NotificationService |
| `ResolveIncidentCommandHandler.cs` | Integración con NotificationService |
| `AddCommentCommandHandler.cs` | Integración con NotificationService |
| `LinkArticleToIncidentCommandHandler.cs` | Integración con NotificationService |

---
