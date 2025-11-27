# Phase 4 - Trazabilidad y Comentarios (Auditoría de Incidentes)

Esta fase implementa el sistema completo de trazabilidad con historial de cambios automático y comentarios para cada incidente.

## Funcionalidades Implementadas

### 1. **Historial de Cambios (IncidentHistory)**
- ✅ Registro automático de todos los cambios en incidentes
- ✅ Captura de valores anteriores y nuevos
- ✅ Registro del usuario que realizó cada cambio
- ✅ Timestamps precisos para cada acción
- ✅ Descripciones formateadas legibles para el usuario

### 2. **Acciones Rastreadas**
| Acción | Descripción | Ejemplo |
|--------|-------------|---------|
| `Created` | Incidente creado | "Incidente creado" |
| `StatusChanged` | Cambio de estado | "cambió el estado de 'Abierto' a 'En Progreso'" |
| `PriorityChanged` | Cambio de prioridad | "cambió la prioridad de 'Media' a 'Alta'" |
| `TypeChanged` | Cambio de tipo | "cambió el tipo de 'Consulta' a 'Falla'" |
| `ServiceChanged` | Cambio de servicio | "cambió el servicio de 'WiFi' a 'Email'" |
| `Assigned` | Asignación de técnico | "asignó el incidente a Carlos Técnico" |
| `Unassigned` | Desasignación | "removió la asignación del incidente" |
| `CommentAdded` | Comentario agregado | "agregó un comentario" |

### 3. **Sistema de Comentarios (IncidentComment)**
- ✅ Comentarios públicos (visibles para todos)
- ✅ Comentarios internos (solo para técnicos y administradores)
- ✅ Identificación visual de comentarios internos (badge amarillo)
- ✅ Información del autor y fecha de cada comentario

### 4. **Interfaces de Usuario**

#### Vista de Usuario Normal (`/incidents/{id}`)
- Timeline de historial con todas las acciones
- Sección de comentarios públicos
- Formulario para agregar comentarios

#### Vista de Técnico (Modal en Dashboard)
- Acordeón colapsable de Comentarios
- Acordeón colapsable de Historial
- Formulario para comentarios (públicos e internos)
- Historial detallado con `FormattedAction`

#### Vista de Administrador (Modal en Admin Incidents)
- Acordeón colapsable de Comentarios
- Acordeón colapsable de Historial
- Formulario para comentarios (públicos e internos)
- Historial detallado con `FormattedAction`

---

## Arquitectura Implementada

### Domain Layer
```
Entities/
├── IncidentHistory.cs       # Entidad de historial de cambios
├── IncidentComment.cs       # Entidad de comentarios

Enums/
├── HistoryAction.cs         # Enum con tipos de acciones

Interfaces/
├── IIncidentHistoryRepository.cs
├── IIncidentCommentRepository.cs
```

### Application Layer
```
Commands/
├── AddIncidentCommentCommand.cs

Queries/
├── GetIncidentHistoryQuery.cs
├── GetIncidentCommentsQuery.cs

Handlers/
├── AddIncidentCommentCommandHandler.cs
├── GetIncidentHistoryQueryHandler.cs
├── GetIncidentCommentsQueryHandler.cs

DTOs/
├── IncidentHistoryDto.cs    # DTO con FormattedAction
├── IncidentCommentDto.cs

Services/
├── IIncidentHistoryService.cs
├── IncidentHistoryService.cs
```

### Infrastructure Layer
```
Repositories/
├── IncidentHistoryRepository.cs
├── IncidentCommentRepository.cs

Migrations/
├── AddIncidentHistoryAndComments   # Tablas IncidentHistories, IncidentComments
```

### Presentation Layer
```
Components/Pages/
├── IncidentDetail.razor     # Vista usuario con historial y comentarios
├── TechnicianDashboard.razor # Modal con acordeones de historial/comentarios
├── AdminIncidents.razor     # Modal con acordeones de historial/comentarios
```

---

## Entidades de Dominio

### IncidentHistory
```csharp
public class IncidentHistory
{
    public int Id { get; set; }
    public int IncidentId { get; set; }
    public string UserId { get; set; }
    public HistoryAction Action { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? Description { get; set; }
    public DateTime Timestamp { get; set; }
    
    // Navigation
    public Incident Incident { get; set; }
    public ApplicationUser User { get; set; }
    
    // Computed property
    public string FormattedAction { get; }
}
```

### IncidentComment
```csharp
public class IncidentComment
{
    public int Id { get; set; }
    public int IncidentId { get; set; }
    public string UserId { get; set; }
    public string Content { get; set; }
    public bool IsInternal { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation
    public Incident Incident { get; set; }
    public ApplicationUser User { get; set; }
}
```

### HistoryAction Enum
```csharp
public enum HistoryAction
{
    Created,
    StatusChanged,
    PriorityChanged,
    TypeChanged,
    ServiceChanged,
    Assigned,
    Unassigned,
    CommentAdded,
    Escalated,
    Resolved,
    Closed,
    Reopened
}
```

---

## Características Técnicas

### Registro Automático de Historial
Los siguientes CommandHandlers registran historial automáticamente:
- `CreateIncidentCommandHandler` → `HistoryAction.Created`
- `UpdateIncidentStatusCommandHandler` → `HistoryAction.StatusChanged`
- `UpdateIncidentPriorityCommandHandler` → `HistoryAction.PriorityChanged`
- `UpdateIncidentTypeCommandHandler` → `HistoryAction.TypeChanged`
- `UpdateIncidentServiceCommandHandler` → `HistoryAction.ServiceChanged`
- `AssignIncidentCommandHandler` → `HistoryAction.Assigned` / `Unassigned`
- `AddIncidentCommentCommandHandler` → `HistoryAction.CommentAdded`

### FormattedAction (Descripciones Legibles)
La propiedad `FormattedAction` en `IncidentHistory` genera descripciones como:
- "cambió el estado de 'Abierto' a 'En Progreso'"
- "cambió la prioridad de 'Media' a 'Crítica'"
- "asignó el incidente a Carlos Técnico"

### Acordeones Colapsables
Los modales de Técnico y Admin usan acordeones para ahorrar espacio:
```razor
<button @onclick="() => commentsExpanded = !commentsExpanded">
    💬 Comentarios (@selectedIncidentComments.Count)
    <svg class="@(commentsExpanded ? "rotate-180" : "")">...</svg>
</button>
```

### Visibilidad de Comentarios
| Tipo | Usuario | Técnico | Admin |
|------|---------|---------|-------|
| Público | ✅ | ✅ | ✅ |
| Interno | ❌ | ✅ | ✅ |

---

## Mejoras de UX Implementadas

### 1. Toggle de Visibilidad de Contraseña
- Ícono de ojo en campo de contraseña del modal "Crear Usuario"
- Alterna entre mostrar/ocultar contraseña

### 2. Acordeones en Modales
- Secciones de Comentarios e Historial colapsables
- Contador de elementos en cada sección
- Animación de rotación en ícono de flecha

---

## Flujos de Usuario

### Flujo: Usuario Agrega Comentario
1. Usuario abre detalle de su incidente
2. Escribe comentario en el formulario
3. Hace clic en "Enviar Comentario"
4. Comentario aparece en la lista
5. Se registra en historial: "agregó un comentario"

### Flujo: Técnico Cambia Estado
1. Técnico abre modal de incidente
2. Cambia estado en dropdown
3. Sistema registra automáticamente:
   - Nuevo estado en el incidente
   - Entrada en historial con valores old/new
4. Usuario ve el cambio en su timeline

### Flujo: Admin Revisa Historial
1. Admin abre modal de incidente
2. Expande acordeón de Historial
3. Ve timeline completo de cambios
4. Cada entrada muestra: usuario, acción formateada, fecha

---

## Base de Datos

### Tabla: IncidentHistories
```sql
CREATE TABLE IncidentHistories (
    Id INT PRIMARY KEY IDENTITY,
    IncidentId INT NOT NULL,
    UserId NVARCHAR(450) NOT NULL,
    Action INT NOT NULL,
    OldValue NVARCHAR(MAX),
    NewValue NVARCHAR(MAX),
    Description NVARCHAR(MAX),
    Timestamp DATETIME2 NOT NULL,
    
    FOREIGN KEY (IncidentId) REFERENCES Incidents(Id),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id)
);
```

### Tabla: IncidentComments
```sql
CREATE TABLE IncidentComments (
    Id INT PRIMARY KEY IDENTITY,
    IncidentId INT NOT NULL,
    UserId NVARCHAR(450) NOT NULL,
    Content NVARCHAR(MAX) NOT NULL,
    IsInternal BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL,
    
    FOREIGN KEY (IncidentId) REFERENCES Incidents(Id),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id)
);
```

---

## Estadísticas de la Fase

**Archivos Creados:**
```
Domain Layer:      4 archivos (entidades, enum, interfaces)
Application Layer: 9 archivos (commands, queries, handlers, DTOs, service)
Infrastructure:    3 archivos (repositories, migration)
Presentation:      3 archivos modificados
```

**Componentes Nuevos:**
- 1 Command + 1 Handler (AddIncidentComment)
- 2 Queries + 2 Handlers (GetHistory, GetComments)
- 2 DTOs (IncidentHistoryDto, IncidentCommentDto)
- 1 Service (IncidentHistoryService)
- 2 Repositories (History, Comments)

---

## Próximas Fases

✅ **Fase 0:** Configuración inicial (Completada)
✅ **Fase 1:** Autenticación y Gestión de Usuarios (Completada)
✅ **Fase 2:** Catálogo de Servicios (Completada)
✅ **Fase 3:** Gestión Básica de Incidentes (Completada)
✅ **Fase 4:** Trazabilidad y Comentarios (Completada)
⏳ **Fase 5:** Escalamiento de Incidentes
⏳ **Fase 6:** Base de Conocimiento
⏳ **Fase 7:** Sistema de Notificaciones

---

## Ejecutar la Aplicación

```bash
cd IncidentsTI.Web
dotnet run
```

**URLs:**
- HTTP: http://localhost:5132
- HTTPS: https://localhost:7117

**Usuarios de Prueba:**
```
Admin:    admin@uta.edu.ec / Admin123!
Técnico:  carlos.tech@uta.edu.ec / Tech123!
Docente:  pedro.docente@uta.edu.ec / Teacher123!
```

---

**Desarrollado con:** 🚀 .NET 8 + Blazor Server + Tailwind CSS  
**Arquitectura:** 🏗️ Onion Architecture + CQRS Pattern  
**Patrón de Auditoría:** 📝 Event Sourcing (simplificado)
