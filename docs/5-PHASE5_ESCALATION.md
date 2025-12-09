# Fase 5: Sistema de Escalamiento de Incidentes - COMPLETADA


Se implementó un sistema completo de escalamiento de incidentes con múltiples niveles, permitiendo a técnicos y administradores escalar incidentes cuando requieren atención de niveles superiores.

---

## 🏗️ Componentes Implementados

### Backend (Domain Layer)

| Archivo | Descripción |
|---------|-------------|
| `Entities/EscalationLevel.cs` | Entidad para niveles de escalamiento |
| `Entities/IncidentEscalation.cs` | Entidad para registro de escalamientos |
| `Enums/HistoryAction.cs` | Agregado `Escalated = 8` |
| `Interfaces/IEscalationLevelRepository.cs` | Interfaz del repositorio |
| `Interfaces/IIncidentEscalationRepository.cs` | Interfaz del repositorio |

### Backend (Infrastructure Layer)

| Archivo | Descripción |
|---------|-------------|
| `Repositories/EscalationLevelRepository.cs` | Implementación del repositorio |
| `Repositories/IncidentEscalationRepository.cs` | Implementación del repositorio |
| `Data/ApplicationDbContext.cs` | Configuración de entidades y relaciones |
| `Migrations/AddEscalationTables.cs` | Migración de base de datos |

### Backend (Application Layer)

| Archivo | Descripción |
|---------|-------------|
| `DTOs/Escalation/EscalationLevelDto.cs` | DTO para niveles |
| `DTOs/Escalation/IncidentEscalationDto.cs` | DTO para escalamientos |
| `Commands/EscalateIncidentCommand.cs` | Comando CQRS para escalar |
| `Handlers/EscalateIncidentCommandHandler.cs` | Handler del comando |
| `Queries/GetEscalationLevelsQuery.cs` | Query para obtener niveles |
| `Queries/GetIncidentEscalationHistoryQuery.cs` | Query para historial |
| `Handlers/GetEscalationLevelsQueryHandler.cs` | Handler de query |
| `Handlers/GetIncidentEscalationHistoryQueryHandler.cs` | Handler de query |
| `Services/IncidentHistoryService.cs` | Agregado `RecordEscalationAsync()` |
| `DTOs/IncidentHistoryDto.cs` | Formato detallado de escalamiento |

### Frontend (Web Layer)

| Archivo | Descripción |
|---------|-------------|
| `Components/Shared/EscalateIncidentModal.razor` | Modal para escalar incidentes |
| `Components/Shared/EscalationHistory.razor` | Componente de historial |
| `Components/Pages/IncidentDetail.razor` | Tarjeta de escalamiento y modal |
| `Components/Pages/TechnicianDashboard.razor` | Integración completa |
| `Components/Pages/AdminIncidents.razor` | Integración completa |

---

## 🗃️ Estructura de Base de Datos

### Tabla: EscalationLevels
```sql
CREATE TABLE EscalationLevels (
    Id INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    [Order] INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1
);
```

### Tabla: IncidentEscalations
```sql
CREATE TABLE IncidentEscalations (
    Id INT PRIMARY KEY IDENTITY,
    IncidentId INT NOT NULL FOREIGN KEY,
    FromUserId NVARCHAR(450) NOT NULL FOREIGN KEY,
    ToUserId NVARCHAR(450) NULL FOREIGN KEY,
    FromLevelId INT NULL FOREIGN KEY,
    ToLevelId INT NOT NULL FOREIGN KEY,
    Reason NVARCHAR(1000) NOT NULL,
    Notes NVARCHAR(2000),
    EscalatedAt DATETIME2 NOT NULL
);
```

### Columna agregada a Incidents
```sql
ALTER TABLE Incidents ADD CurrentEscalationLevelId INT NULL FOREIGN KEY;
```

---

## 📊 Niveles de Escalamiento (Actualizados en Fase 13)

| Nivel | Nombre | Orden | Rol Asociado | Descripción |
|-------|--------|-------|--------------|-------------|
| 1 | Nivel 1 - Soporte Inicial | 1 | Pasante | Primer nivel de atención para incidentes básicos |
| 2 | Nivel 2 - Soporte Técnico | 2 | Técnico | Técnicos especializados para problemas complejos |
| 3 | Nivel 3 - Soporte Avanzado | 3 | Administrador | Casos que requieren permisos elevados o coordinación externa |

> **Nota:** En la [Fase 13](13-ESCALATION_LEVELS_BY_ROLE.md) se implementó la asignación automática de niveles según el rol del usuario y el sistema de liberación de incidentes al escalar.

---

## 🎨 Características de UI/UX

### Diseño del Botón de Escalar
- **Estilo:** Outline con borde ámbar (`border-amber-500`)
- **Hover:** Fondo suave ámbar (`hover:bg-amber-50`)
- **Razón:** Mejor contraste con el fondo según principios DCU

### Modal de Escalamiento
- Diseño consistente con Tailwind CSS
- Muestra nivel actual
- Solo muestra niveles superiores disponibles
- Campos obligatorios: Nivel destino y Motivo
- Campo opcional: Notas adicionales

### Historial de Cambios Mejorado
- **Antes:** "escaló el incidente"
- **Ahora:** "escaló el incidente de 'Nivel 1' a 'Nivel 2'. Motivo: [razón]"

---

## 🕐 Zona Horaria

Se implementó conversión de UTC a hora de Ecuador (UTC-5) en todas las vistas:

```csharp
private static readonly TimeZoneInfo EcuadorTimeZone = 
    TimeZoneInfo.CreateCustomTimeZone(
        "Ecuador Time", 
        TimeSpan.FromHours(-5), 
        "Ecuador Time", 
        "Ecuador Time");

private string FormatLocalDateTime(DateTime utcDateTime)
{
    var localTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, EcuadorTimeZone);
    return localTime.ToString("dd/MM/yyyy HH:mm");
}
```

**Páginas actualizadas:**
- TechnicianDashboard.razor
- AdminIncidents.razor
- IncidentDetail.razor
- EscalationHistory.razor

---

## 🔒 Control de Acceso

| Rol | Puede ver escalamiento | Puede escalar |
|-----|------------------------|---------------|
| Usuario | ✅ (solo lectura) | ❌ |
| Técnico | ✅ | ✅ |
| Administrador | ✅ | ✅ |

---


## 📌 Notas Técnicas

- El sistema usa MediatR para CQRS
- Entity Framework Core para persistencia
- Blazor Server con InteractiveServer render mode
- Tailwind CSS para estilos
- Patrón Repository para acceso a datos

---

## 🔗 Mejoras Posteriores

Esta fase fue mejorada en la **[Fase 13 - Escalamiento por Niveles de Usuario](13-ESCALATION_LEVELS_BY_ROLE.md)** con:

- Asignación automática de nivel según el rol del usuario al reclamar incidentes
- Filtrado de incidentes disponibles según el nivel del usuario
- Liberación de incidentes al escalar (sistema first-come-first-serve)
- Seguimiento en solo lectura para el usuario que escaló
- Nuevo campo `EscalatedByUserId` para rastrear quién escaló
