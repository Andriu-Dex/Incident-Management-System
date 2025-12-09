# 🔄 Fase 13: Sistema de Escalamiento por Niveles de Usuario

Esta fase implementa una mejora significativa al sistema de escalamiento existente, donde cada rol de usuario tiene un nivel de soporte asignado y los incidentes fluyen entre niveles según la complejidad del problema.

---

## 📋 Resumen Ejecutivo

Se implementó un sistema de escalamiento basado en roles donde:
- **Pasantes** → Nivel 1 (Soporte Inicial)
- **Técnicos** → Nivel 2 (Soporte Técnico)
- **Administradores** → Nivel 3 (Soporte Avanzado)

Al escalar un incidente, este se **libera** y queda disponible para que personal del nivel superior lo reclame (sistema "first-come-first-serve").

---

## 🎯 Objetivos Implementados

1. ✅ **Asociar cada rol con un nivel de soporte**
2. ✅ **Asignar nivel automáticamente** al reclamar un incidente
3. ✅ **Filtrar incidentes disponibles** según el nivel del usuario
4. ✅ **Liberar incidentes al escalar** para que el nivel superior los reclame
5. ✅ **Permitir seguimiento en solo lectura** al técnico que escaló

---

## 🏗️ Arquitectura del Sistema

### Niveles de Soporte Redefinidos

| Nivel | Nombre | Roles Asociados | Puede Ver/Reclamar |
|-------|--------|-----------------|-------------------|
| **1** | Soporte Inicial | Pasante | Solo Nivel 1 y sin nivel |
| **2** | Soporte Técnico | Técnico | Nivel 1 y 2 |
| **3** | Soporte Avanzado | Administrador | Todos los niveles |

### Flujo de Incidentes

```
  Usuario crea incidente
          │
          ▼
  ┌───────────────┐
  │ Status: Open  │ ◄─── Sin nivel, sin asignar
  │ Nivel: null   │      Visible para TODOS (N1, N2, N3)
  │ Asignado: null│
  └───────┬───────┘
          │
          │ Pasante/Técnico/Admin reclama
          ▼
  ┌───────────────────┐
  │ Status: InProgress│ ◄─── Nivel = según quien reclamó
  │ Nivel: 1, 2, o 3  │      Solo el asignado trabaja en él
  │ Asignado: userId  │
  └───────┬───────────┘
          │
          ├──────────────────────────────────┐
          │                                  │
          │ Resuelve                         │ Escala a nivel superior
          ▼                                  ▼
  ┌───────────────┐               ┌─────────────────────┐
  │ Status:       │               │ Status: Escalated   │
  │ Resolved      │               │ Nivel: N+1          │
  └───────────────┘               │ Asignado: null      │ ◄─── SE LIBERA
                                  │ EscaladoPor: userId │
                                  └──────────┬──────────┘
                                             │
                                             │ Visible para nivel >= N+1
                                             │ El que escaló ve en SOLO LECTURA
                                             ▼
                                  ┌─────────────────────┐
                                  │ Técnico/Admin       │
                                  │ de nivel superior   │
                                  │ reclama             │
                                  └──────────┬──────────┘
                                             │
                                             ▼
                                  ┌─────────────────────┐
                                  │ Status: InProgress  │
                                  │ Asignado: nuevo     │
                                  └─────────────────────┘
```

---

## 📁 Archivos Creados

### Capa de Aplicación

| Archivo | Descripción |
|---------|-------------|
| `Common/UserLevelResolver.cs` | Clase estática para mapear roles a niveles |
| `Queries/GetEscalatedByMeIncidentsQuery.cs` | Query para obtener incidentes escalados por el usuario |
| `Handlers/GetEscalatedByMeIncidentsQueryHandler.cs` | Handler de la query |

### Capa de Dominio

| Archivo | Cambio |
|---------|--------|
| `Entities/Incident.cs` | Agregado `EscalatedByUserId` y navegación `EscalatedByUser` |

### Capa de Infraestructura

| Archivo | Descripción |
|---------|-------------|
| `Migrations/AddEscalatedByUserIdAndUpdateLevelNames.cs` | Migración para nuevo campo y FK |

---

## 📝 Archivos Modificados

### Backend

| Archivo | Cambios |
|---------|---------|
| `Application/DTOs/IncidentDto.cs` | Agregado `EscalatedByUserId`, `EscalatedByUserName` |
| `Application/Queries/GetAvailableIncidentsQuery.cs` | Agregado parámetro `UserId` |
| `Application/Handlers/GetAvailableIncidentsQueryHandler.cs` | Filtrado por nivel de usuario |
| `Application/Handlers/ClaimIncidentCommandHandler.cs` | Asignación automática de nivel |
| `Application/Handlers/EscalateIncidentCommandHandler.cs` | Liberación del incidente al escalar |
| `Application/Handlers/GetIncidentByIdQueryHandler.cs` | Mapeo de `EscalatedByUser` |
| `Infrastructure/Data/ApplicationDbContext.cs` | Configuración FK con `NoAction` |
| `Infrastructure/Data/DatabaseSeeder.cs` | Auto-actualización de nombres de niveles |

### Frontend

| Archivo | Cambios |
|---------|---------|
| `Components/Pages/TechnicianDashboard.razor` | Nueva sección "Incidentes que Escalé" |
| `Components/Pages/AvailableIncidents.razor` | Badge de escalado, filtrado por UserId |
| `Components/Pages/IncidentDetail.razor` | Banner de modo solo lectura |
| `Components/Shared/EscalateIncidentModal.razor` | Mensaje explicativo del flujo de liberación |

---

## 🗃️ Migración de Base de Datos

### `AddEscalatedByUserIdAndUpdateLevelNames`

```sql
-- Agregar campo para rastrear quién escaló
ALTER TABLE Incidents ADD EscalatedByUserId NVARCHAR(450) NULL;

-- Crear índice
CREATE INDEX IX_Incidents_EscalatedByUserId ON Incidents(EscalatedByUserId);

-- Crear FK con NoAction (evitar múltiples cascadas)
ALTER TABLE Incidents ADD CONSTRAINT FK_Incidents_AspNetUsers_EscalatedByUserId 
    FOREIGN KEY (EscalatedByUserId) REFERENCES AspNetUsers(Id);
```

**Nota:** Se usa `NoAction` en lugar de `SetNull` porque SQL Server no permite múltiples rutas de cascade delete hacia la misma tabla.

---

## 🔧 UserLevelResolver

Clase estática que centraliza la lógica de mapeo rol → nivel:

```csharp
public static class UserLevelResolver
{
    public static int GetUserLevel(IEnumerable<string> roles)
    {
        if (roles.Contains("Administrator")) return 3;
        if (roles.Contains("Technician")) return 2;
        if (roles.Contains("Pasante")) return 1;
        return 0; // No es personal de soporte
    }

    public static bool CanAccessLevel(int userLevel, int? incidentLevel)
    {
        if (userLevel == 0) return false;
        if (!incidentLevel.HasValue) return true; // Sin nivel = nuevo
        return userLevel >= incidentLevel.Value;
    }
}
```

---

## 🎨 Cambios en UI/UX

### Dashboard del Técnico

Nueva sección **"Incidentes que Escalé"** con:
- Tabla con tickets, título, servicio, nivel actual, asignado y estado
- Indicador visual con color ámbar/naranja
- Badge informativo de "modo solo lectura"
- Click para ver detalle (sin acciones)

### Incidentes Disponibles

- **Badge de escalado**: Muestra "Escalado a Nivel X por [nombre]"
- **Filtrado automático**: Solo ve incidentes de su nivel o inferior

### Detalle del Incidente

- **Banner amarillo**: "Modo Solo Lectura" cuando el usuario actual escaló el incidente
- **Botones deshabilitados**: No puede realizar acciones sobre incidentes que escaló

### Modal de Escalamiento

- **Caja informativa azul**: Explica que el incidente será liberado
- **Texto mejorado**: "Al escalar, el incidente será liberado para que el personal del nivel seleccionado pueda reclamarlo"

---

## 🔒 Reglas de Negocio

| Regla | Descripción |
|-------|-------------|
| Asignación de nivel | Al reclamar un incidente sin nivel, se asigna el nivel del usuario |
| Filtro de disponibles | Usuario solo ve incidentes de nivel ≤ su nivel |
| Liberación al escalar | `AssignedToId = null` al escalar |
| Registro de escalador | `EscalatedByUserId` guarda quién escaló |
| Solo lectura | Usuario que escaló puede ver pero no modificar |
| Validación de nivel | No se puede reclamar incidente de nivel superior |

---

## 🧪 Escenarios de Prueba

| # | Escenario | Resultado Esperado |
|---|-----------|-------------------|
| 1 | Pasante ve incidentes nuevos | ✅ Ve todos los incidentes sin nivel |
| 2 | Pasante ve incidentes N2 | ❌ No los ve |
| 3 | Técnico ve incidentes N1 y N2 | ✅ Ve ambos |
| 4 | Admin ve todos los incidentes | ✅ Ve N1, N2, N3 |
| 5 | Pasante reclama incidente nuevo | ✅ Se asigna nivel 1 |
| 6 | Técnico reclama incidente nuevo | ✅ Se asigna nivel 2 |
| 7 | Pasante escala a N2 | ✅ Incidente liberado, visible para N2+ |
| 8 | Pasante que escaló ve el incidente | ✅ Solo lectura |
| 9 | Técnico reclama incidente escalado | ✅ Funciona correctamente |
| 10 | Incidente escalado no aparece para Pasantes | ✅ Filtrado correcto |

---

## 👥 Usuarios de Prueba

| Rol | Email | Contraseña | Nivel |
|-----|-------|------------|-------|
| Pasante | miguel.pasante@uta.edu.ec | Intern123! | 1 |
| Pasante | camila.pasante@uta.edu.ec | Intern123! | 1 |
| Técnico | carlos.tech@uta.edu.ec | Tech123! | 2 |
| Técnico | ana.tech@uta.edu.ec | Tech123! | 2 |
| Admin | admin@uta.edu.ec | Admin123! | 3 |

---

## ⚠️ Breaking Changes

### GetAvailableIncidentsQuery

**Antes:**
```csharp
var query = new GetAvailableIncidentsQuery();
```

**Ahora:**
```csharp
var query = new GetAvailableIncidentsQuery { UserId = currentUserId };
```

El parámetro `UserId` es **requerido** para filtrar los incidentes según el nivel del usuario.

---

## 📌 Notas Técnicas

- **FK con NoAction**: SQL Server no permite múltiples rutas de cascade delete. Se usa `DeleteBehavior.NoAction` para `EscalatedByUser`.
- **Auto-update de niveles**: El `DatabaseSeeder` actualiza automáticamente los nombres de niveles existentes si difieren de los nuevos.
- **Compatibilidad**: Los incidentes existentes sin nivel siguen siendo visibles para todos los niveles de soporte.

---

## ✅ Estado de Implementación

- [x] Crear `UserLevelResolver`
- [x] Agregar campo `EscalatedByUserId`
- [x] Modificar `ClaimIncidentHandler` (asignar nivel)
- [x] Modificar `EscalateIncidentHandler` (liberar incidente)
- [x] Modificar `GetAvailableIncidentsQuery` (filtrar por nivel)
- [x] Crear `GetEscalatedByMeIncidentsQuery`
- [x] Actualizar UI: Dashboard, AvailableIncidents, IncidentDetail
- [x] Actualizar modal de escalamiento
- [x] Actualizar nombres de niveles en BD
- [x] Migración de base de datos
- [x] Documentación
