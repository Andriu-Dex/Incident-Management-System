# Phase 3 - Gestión Básica de Incidentes (MVP del Sistema)

Esta fase implementa el módulo core del sistema: la gestión completa de incidentes de TI con creación, visualización, asignación, cambio de estados y edición de atributos.

## Funcionalidades Implementadas

### 1. **Gestión de Incidentes**
- ✅ Crear nuevos incidentes con servicio, tipo, prioridad y descripción
- ✅ Generación automática de número de ticket (INC-YYYY-NNNN)
- ✅ Ver listado de incidentes propios (usuarios)
- ✅ Ver listado de incidentes asignados (técnicos)
- ✅ Ver todos los incidentes del sistema (administradores)
- ✅ Cambiar estado de incidentes (Open → InProgress → Resolved → Closed)
- ✅ Asignar/desasignar incidentes a técnicos
- ✅ Modificar servicio asociado, tipo y prioridad (técnicos/admins)
- ✅ Eliminar incidentes (modo Super Admin)

### 2. **Estados de Incidentes**
- ✅ **Open** (Abierto) - Incidente recién creado
- ✅ **InProgress** (En Progreso) - Siendo atendido
- ✅ **Escalated** (Escalado) - Requiere nivel superior
- ✅ **Resolved** (Resuelto) - Solución aplicada
- ✅ **Closed** (Cerrado) - Caso finalizado

### 3. **Prioridades**
- ✅ **Low** (Baja) - Color verde
- ✅ **Medium** (Media) - Color amarillo
- ✅ **High** (Alta) - Color naranja
- ✅ **Critical** (Crítica) - Color rojo

### 4. **Tipos de Incidente**
- ✅ **Failure** (Falla) - Problema técnico
- ✅ **Query** (Consulta) - Pregunta o duda
- ✅ **Request** (Requerimiento) - Solicitud de servicio

### 5. **Interfaces de Usuario**

#### Para Usuarios (Estudiantes, Docentes, Administrativos)
- ✅ Página "Mis Incidentes" (`/my-incidents`)
- ✅ Crear nuevo incidente (`/create-incident`)
- ✅ Ver detalle del incidente (`/incidents/{id}`)
- ✅ Filtros por estado

#### Para Técnicos
- ✅ Dashboard Técnico (`/technician/dashboard`)
- ✅ Ver incidentes asignados
- ✅ Cambiar estado directamente desde la tabla
- ✅ Modal de detalle con edición de servicio, tipo y prioridad
- ✅ Estadísticas: Asignados, En Progreso, Alta Prioridad, Resueltos

#### Para Administradores
- ✅ Gestión de Incidentes (`/admin/incidents`)
- ✅ Ver todos los incidentes del sistema
- ✅ Asignar/desasignar técnicos
- ✅ Filtros avanzados: estado, prioridad, asignación
- ✅ Modal de detalle con edición completa
- ✅ Modo Super Admin con eliminación de incidentes
- ✅ Estadísticas: Total, Abiertos, En Progreso, Sin Asignar, Cerrados

---

## Arquitectura Implementada

### Domain Layer
```
Entities/
├── Incident.cs              # Entidad principal de incidentes

Enums/
├── IncidentStatus.cs        # Estados del incidente
├── IncidentPriority.cs      # Niveles de prioridad
├── IncidentType.cs          # Tipos de incidente

Interfaces/
├── IIncidentRepository.cs   # Contrato del repositorio
```

### Application Layer
```
Commands/
├── CreateIncidentCommand.cs
├── UpdateIncidentStatusCommand.cs
├── AssignIncidentCommand.cs
├── UpdateIncidentServiceCommand.cs
├── UpdateIncidentTypeCommand.cs
├── UpdateIncidentPriorityCommand.cs
├── DeleteIncidentCommand.cs

Queries/
├── GetIncidentByIdQuery.cs
├── GetUserIncidentsQuery.cs
├── GetAllIncidentsQuery.cs
├── GetAssignedIncidentsQuery.cs

Handlers/
├── CreateIncidentCommandHandler.cs
├── UpdateIncidentStatusCommandHandler.cs
├── AssignIncidentCommandHandler.cs
├── UpdateIncidentServiceCommandHandler.cs
├── UpdateIncidentTypeCommandHandler.cs
├── UpdateIncidentPriorityCommandHandler.cs
├── DeleteIncidentCommandHandler.cs
├── GetIncidentByIdQueryHandler.cs
├── GetUserIncidentsQueryHandler.cs
├── GetAllIncidentsQueryHandler.cs
├── GetAssignedIncidentsQueryHandler.cs

DTOs/
├── IncidentDto.cs           # DTO completo para detalle
├── IncidentListDto.cs       # DTO ligero para listados
├── CreateIncidentDto.cs     # DTO para creación
```

### Infrastructure Layer
```
Repositories/
├── IncidentRepository.cs    # Implementación con EF Core

Migrations/
├── AddIncidentsTable        # Tabla de incidentes
```

### Presentation Layer
```
Components/Pages/
├── CreateIncident.razor     # Formulario de creación
├── MyIncidents.razor        # Listado de usuario
├── IncidentDetail.razor     # Detalle del incidente
├── TechnicianDashboard.razor # Panel de técnicos
├── AdminIncidents.razor     # Panel de administración

Components/Layout/
├── ToastContainer.razor     # Contenedor de notificaciones
├── MainLayout.razor         # Layout principal
├── EmptyLayout.razor        # Layout para login
```

---

## Características Técnicas

### Generación de Tickets
El sistema genera automáticamente números de ticket con formato:
```
INC-YYYY-NNNN
Ejemplo: INC-2025-0001, INC-2025-0002, ...
```

### Sistema de Permisos
| Acción | Usuario | Técnico | Admin |
|--------|---------|---------|-------|
| Crear incidente | ✅ | ✅ | ✅ |
| Ver sus incidentes | ✅ | ✅ | ✅ |
| Ver incidentes asignados | ❌ | ✅ | ✅ |
| Ver todos los incidentes | ❌ | ❌ | ✅ |
| Cambiar estado | ❌ | ✅ | ✅ |
| Modificar servicio | ❌ | ✅ | ✅ |
| Modificar tipo | ❌ | ✅ | ✅ |
| Modificar prioridad | ❌ | ✅ | ✅ |
| Asignar técnico | ❌ | ❌ | ✅ |
| Eliminar incidente | ❌ | ❌ | ✅ (Super Admin) |

### Notificaciones Toast
Sistema de notificaciones funcional usando `Blazored.Toast`:
- ✅ Toast de éxito (verde) - Acciones completadas
- ✅ Toast de error (rojo) - Errores de operación
- ✅ Toast de advertencia (amarillo) - Modo Super Admin activado
- ✅ Toast informativo (azul) - Modo Super Admin desactivado

### Modales de Edición
Los técnicos y administradores pueden editar incidentes directamente desde un modal:
- Ver información completa del ticket
- Editar servicio asociado (dropdown)
- Editar tipo de incidente (dropdown)
- Editar prioridad (dropdown)
- Guardar cambios con un clic

### Modo Super Admin
Toggle especial en la gestión de incidentes (Admin):
- Icono de rayo (⚡) en la esquina superior derecha
- Al activarse, aparece botón de eliminar en cada incidente
- Confirmación antes de eliminar
- Notificación toast al activar/desactivar

---

## Flujos de Usuario

### Flujo: Usuario Crea Incidente
1. Usuario navega a "Crear Incidente"
2. Selecciona servicio del catálogo
3. Selecciona tipo (Falla, Consulta, Requerimiento)
4. Selecciona prioridad
5. Ingresa título y descripción
6. Hace clic en "Crear Incidente"
7. Sistema genera número de ticket
8. Usuario es redirigido a "Mis Incidentes"

### Flujo: Técnico Atiende Incidente
1. Técnico accede a Dashboard
2. Ve incidentes asignados con estadísticas
3. Cambia estado directamente desde dropdown
4. O hace clic en "Ver" para abrir modal
5. Modifica servicio/tipo/prioridad si es necesario
6. Guarda cambios
7. Toast de confirmación aparece

### Flujo: Admin Asigna Incidente
1. Admin accede a Gestión de Incidentes
2. Ve todos los incidentes con filtros
3. Selecciona técnico en dropdown de asignación
4. Toast confirma la asignación
5. Incidente aparece en dashboard del técnico

---

## Correcciones y Mejoras Realizadas

### Bug Fixes
- ✅ **Sin Asignar permanente:** Corregido `AssignIncidentCommandHandler` para establecer `AssignedToId = null` en lugar de string vacío

### Mejoras de UX
- ✅ **Modales en lugar de navegación:** Ver detalle abre modal, no nueva página
- ✅ **Toast Container interactivo:** Solucionado problema de renderizado SSR
- ✅ **Password visibility:** Toggle de visibilidad en login

### Limpieza de Código
- ✅ Eliminados archivos `Class1.cs` vacíos de Domain, Application e Infrastructure

---

## Estadísticas de la Fase

**Archivos Creados/Modificados:**
```
Domain Layer:      5 archivos
Application Layer: 22 archivos (Commands, Queries, Handlers, DTOs)
Infrastructure:    3 archivos
Presentation:      8 archivos
```

**Componentes Nuevos:**
- 7 Commands + 7 Handlers
- 4 Queries + 4 Handlers
- 3 DTOs
- 5 Páginas Razor
- 1 ToastContainer component

---

## Tecnologías Utilizadas

- **.NET 8** - Framework principal
- **Blazor Server** - UI interactiva con `@rendermode InteractiveServer`
- **Entity Framework Core 8** - ORM y migraciones
- **SQL Server** - Base de datos
- **MediatR 13.1.0** - Patrón CQRS
- **Tailwind CSS v3.4.18** - Estilos y diseño responsivo
- **Blazored.Toast 4.2.1** - Notificaciones toast

---

## Próximas Fases

✅ **Fase 0:** Configuración inicial (Completada)
✅ **Fase 1:** Autenticación y Gestión de Usuarios (Completada)
✅ **Fase 2:** Catálogo de Servicios (Completada)
✅ **Fase 3:** Gestión Básica de Incidentes (Completada)
⏳ **Fase 4:** Trazabilidad y Comentarios
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
