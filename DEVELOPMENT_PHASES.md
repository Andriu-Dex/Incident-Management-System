# Plan de Desarrollo por Fases - Sistema de Gestión de Incidentes TI

## Estrategia General

El desarrollo se realizará de forma **incremental e iterativa**, implementando funcionalidades core primero y agregando características adicionales en fases posteriores. Cada fase culmina con:

- ✅ Código compilable y funcional
- ✅ Pruebas básicas de las funcionalidades implementadas
- ✅ Documentación actualizada
- ✅ Commit a Git

---

## 📋 FASE 0: Configuración Inicial del Proyecto (Fundamentos)

**Objetivo:** Preparar la infraestructura base del proyecto y configurar las dependencias necesarias.

### Tareas:

#### 0.1 Configuración de proyectos

- [ ] Instalar paquetes NuGet necesarios en cada capa:
  - **Domain**: Ninguno (solo entidades puras)
  - **Application**: MediatR, FluentValidation
  - **Infrastructure**: Entity Framework Core, SQL Server Provider, Identity
  - **Web**: Authentication, Authorization packages

#### 0.2 Configuración de DbContext

- [ ] Crear `ApplicationDbContext` en Infrastructure
- [ ] Configurar cadena de conexión en `appsettings.json`
- [ ] Configurar servicios en `Program.cs`

#### 0.3 Validación inicial

- [ ] Verificar que la solución compila sin errores
- [ ] Ejecutar migración inicial vacía para validar conexión a BD

**Resultado esperado:** Proyecto configurado, compilable y conectado a SQL Server.

---

## 🔐 FASE 1: Autenticación y Gestión de Usuarios (Core del Sistema)

**Objetivo:** Implementar el sistema de autenticación, roles y gestión básica de usuarios.

### Tareas:

#### 1.1 Capa de Dominio

- [ ] Crear entidad `User` (Id, UserName, Email, PasswordHash, IsActive, etc.)
- [ ] Crear enum `UserRole` (Student, Teacher, Administrative, Technician, Administrator)
- [ ] Crear entidad `Role` si necesitas roles dinámicos
- [ ] Definir interfaces de repositorio: `IUserRepository`

#### 1.2 Capa de Aplicación

- [ ] Implementar comando: `LoginCommand` (usuario, contraseña)
- [ ] Implementar comando: `RegisterUserCommand`
- [ ] Implementar query: `GetUserByIdQuery`
- [ ] Implementar query: `GetAllUsersQuery`
- [ ] Implementar comando: `ToggleUserStatusCommand` (activar/desactivar)
- [ ] Crear DTOs: `UserDto`, `LoginDto`, `AuthResponseDto`
- [ ] Implementar validaciones con FluentValidation

#### 1.3 Capa de Infraestructura

- [ ] Implementar `UserRepository`
- [ ] Configurar Entity Framework para `User`
- [ ] Crear migración inicial
- [ ] Implementar servicio de autenticación (JWT o cookies según necesites)
- [ ] Crear seed data con usuarios de prueba:
  - 3 docentes
  - 3 estudiantes
  - 2 técnicos
  - 2 administradores

#### 1.4 Capa de Presentación (Blazor)

- [ ] Crear página de Login (`/login`)
- [ ] Implementar formulario de login con validación
- [ ] Configurar autenticación en Blazor (AuthenticationStateProvider)
- [ ] Crear componente de navegación con logout
- [ ] Implementar autorización por roles en componentes
- [ ] Crear página de gestión de usuarios (solo admin):
  - Listar usuarios
  - Activar/desactivar usuarios
  - Ver detalles básicos

#### 1.5 Pruebas

- [ ] Probar login con diferentes roles
- [ ] Verificar que las rutas protegidas funcionen
- [ ] Probar activación/desactivación de usuarios
- [ ] Validar que usuarios inactivos no puedan loguearse

**Resultado esperado:** Sistema de login funcional con roles, gestión básica de usuarios y protección de rutas.

---

## 📚 FASE 2: Catálogo de Servicios de TI (Fundamento para Incidentes)

**Objetivo:** Implementar el módulo de catálogo de servicios que será usado por los incidentes.

### Tareas:

#### 2.1 Capa de Dominio

- [ ] Crear entidad `Service` (Id, Name, Description, Category, IsActive)
- [ ] Crear enum `ServiceCategory` (Email, Network, AcademicSystems, Hardware, Software, Other)
- [ ] Definir interfaz: `IServiceRepository`

#### 2.2 Capa de Aplicación

- [ ] Implementar comando: `CreateServiceCommand`
- [ ] Implementar comando: `UpdateServiceCommand`
- [ ] Implementar comando: `DeleteServiceCommand` (soft delete)
- [ ] Implementar query: `GetAllServicesQuery`
- [ ] Implementar query: `GetActiveServicesQuery`
- [ ] Implementar query: `GetServiceByIdQuery`
- [ ] Crear DTOs: `ServiceDto`, `CreateServiceDto`, `UpdateServiceDto`

#### 2.3 Capa de Infraestructura

- [ ] Implementar `ServiceRepository`
- [ ] Configurar EF para `Service`
- [ ] Crear migración
- [ ] Crear seed data con servicios de ejemplo:
  - Correo institucional
  - Red inalámbrica (WiFi)
  - Sistemas académicos (LMS, matrícula)
  - Soporte de hardware
  - Soporte de software

#### 2.4 Capa de Presentación

- [ ] Crear página de administración de servicios (`/admin/services`):
  - Listar servicios
  - Crear nuevo servicio
  - Editar servicio
  - Activar/desactivar servicio
- [ ] Implementar componente reutilizable para selector de servicios
- [ ] Aplicar autorización (solo técnicos y admins)

#### 2.5 Pruebas

- [ ] Verificar CRUD completo de servicios
- [ ] Probar autorización por rol
- [ ] Validar seed data

**Resultado esperado:** Catálogo de servicios funcional con operaciones CRUD y datos de prueba.

---

## 🎫 FASE 3: Gestión Básica de Incidentes (MVP del Sistema)

**Objetivo:** Implementar la funcionalidad core de creación, visualización y cambio de estado de incidentes.

### Tareas:

#### 3.1 Capa de Dominio

- [ ] Crear entidad `Incident`:
  - Id, TicketNumber (generado automáticamente)
  - Title, Description
  - ServiceId (FK a Service)
  - UserId (FK a User - creador)
  - AssignedToId (FK a User - técnico asignado, nullable)
  - Status (enum)
  - Priority (enum)
  - CreatedAt, UpdatedAt
- [ ] Crear enum `IncidentStatus` (Open, InProgress, Escalated, Resolved, Closed)
- [ ] Crear enum `IncidentPriority` (Low, Medium, High, Critical)
- [ ] Crear enum `IncidentType` (Failure, Query, Request)
- [ ] Definir interfaz: `IIncidentRepository`

#### 3.2 Capa de Aplicación

- [ ] Implementar comando: `CreateIncidentCommand`
  - Generar número de ticket automáticamente
  - Asociar servicio del catálogo
- [ ] Implementar comando: `UpdateIncidentStatusCommand`
- [ ] Implementar comando: `AssignIncidentCommand` (asignar técnico)
- [ ] Implementar comando: `UpdateIncidentServiceCommand` (cambiar servicio asociado)
- [ ] Implementar query: `GetIncidentByIdQuery`
- [ ] Implementar query: `GetUserIncidentsQuery` (incidentes del usuario logueado)
- [ ] Implementar query: `GetAllIncidentsQuery` (con filtros: estado, servicio, prioridad)
- [ ] Implementar query: `GetAssignedIncidentsQuery` (incidentes asignados a un técnico)
- [ ] Crear DTOs: `IncidentDto`, `CreateIncidentDto`, `IncidentListDto`

#### 3.3 Capa de Infraestructura

- [ ] Implementar `IncidentRepository`
- [ ] Configurar relaciones en EF:
  - Incident -> Service
  - Incident -> User (Creator)
  - Incident -> User (AssignedTo)
- [ ] Crear migración
- [ ] Implementar generador de números de ticket (ej: INC-2024-0001)

#### 3.4 Capa de Presentación

- [ ] Crear página de creación de incidente (`/incidents/new`):
  - Selector de servicio
  - Selector de tipo de incidente
  - Campo de título y descripción
  - Campos prellenados del usuario (no editables)
- [ ] Crear página de listado de incidentes (`/incidents`):
  - Para usuarios: sus propios incidentes
  - Para técnicos/admins: todos los incidentes
  - Filtros por estado, servicio, prioridad
- [ ] Crear página de detalle de incidente (`/incidents/{id}`):
  - Mostrar toda la información
  - Botones de cambio de estado (según rol)
  - Información del servicio asociado
- [ ] Panel para técnicos (`/technician/dashboard`):
  - Incidentes asignados
  - Cambiar estado
- [ ] Panel para administradores (`/admin/incidents`):
  - Asignar incidentes a técnicos
  - Ver estadísticas básicas

#### 3.5 Pruebas

- [ ] Crear incidente como usuario normal
- [ ] Ver listado como usuario (solo sus incidentes)
- [ ] Ver listado como técnico (todos los incidentes)
- [ ] Cambiar estado de incidente
- [ ] Asignar incidente a técnico
- [ ] Cambiar servicio asociado al incidente
- [ ] Verificar generación de número de ticket

**Resultado esperado:** Sistema funcional de gestión de incidentes con CRUD básico, asignación y cambio de estados.

---

## 📝 FASE 4: Trazabilidad y Comentarios (Auditoría de Incidentes)

**Objetivo:** Implementar el sistema de trazabilidad con historial de cambios y comentarios.

### Tareas:

#### 4.1 Capa de Dominio

- [ ] Crear entidad `IncidentHistory`:
  - Id, IncidentId (FK)
  - UserId (quién hizo el cambio)
  - Action (enum: StatusChanged, ServiceChanged, Assigned, CommentAdded, etc.)
  - OldValue, NewValue (campos JSON o texto)
  - Timestamp
- [ ] Crear entidad `IncidentComment`:
  - Id, IncidentId (FK)
  - UserId (FK)
  - Comment (texto)
  - IsInternal (bool - visible solo para TI)
  - CreatedAt
- [ ] Definir interfaces: `IIncidentHistoryRepository`, `IIncidentCommentRepository`

#### 4.2 Capa de Aplicación

- [ ] Implementar comando: `AddCommentCommand`
- [ ] Implementar query: `GetIncidentHistoryQuery`
- [ ] Implementar query: `GetIncidentCommentsQuery`
- [ ] Modificar comandos existentes para registrar historial automáticamente:
  - Al cambiar estado
  - Al asignar técnico
  - Al cambiar servicio
- [ ] Crear DTOs: `IncidentHistoryDto`, `CommentDto`

#### 4.3 Capa de Infraestructura

- [ ] Implementar repositorios
- [ ] Configurar relaciones en EF
- [ ] Crear migración

#### 4.4 Capa de Presentación

- [ ] Agregar sección de comentarios en detalle de incidente
- [ ] Agregar timeline de historial de cambios
- [ ] Implementar formulario para agregar comentarios
- [ ] Diferenciar visualmente comentarios internos vs públicos

#### 4.5 Pruebas

- [ ] Verificar que se registre historial en cada cambio
- [ ] Probar agregar comentarios
- [ ] Validar visibilidad de comentarios internos

**Resultado esperado:** Sistema de trazabilidad completo con historial y comentarios.

---

## ⬆️ FASE 5: Escalamiento de Incidentes (Gestión Avanzada)

**Objetivo:** Implementar el sistema de escalamiento multinivel.

### Tareas:

#### 5.1 Capa de Dominio

- [ ] Crear entidad `EscalationLevel`:
  - Id, Name (Level 1, Level 2, Level 3)
  - Description
  - Order (para ordenar niveles)
- [ ] Agregar a `Incident`:
  - CurrentEscalationLevelId (FK, nullable)
- [ ] Crear entidad `IncidentEscalation`:
  - Id, IncidentId (FK)
  - FromUserId (quien escaló)
  - ToUserId (a quien se escaló, nullable)
  - FromLevel, ToLevel
  - Reason
  - EscalatedAt
- [ ] Definir interfaces necesarias

#### 5.2 Capa de Aplicación

- [ ] Implementar comando: `EscalateIncidentCommand`
- [ ] Implementar query: `GetEscalationHistoryQuery`
- [ ] Crear DTOs correspondientes

#### 5.3 Capa de Infraestructura

- [ ] Implementar repositorios
- [ ] Configurar relaciones en EF
- [ ] Crear seed data con niveles de escalamiento:
  - Level 1: Mesa de ayuda
  - Level 2: Especialista
  - Level 3: Proveedor externo
- [ ] Crear migración

#### 5.4 Capa de Presentación

- [ ] Agregar botón de escalamiento en detalle de incidente
- [ ] Modal para seleccionar nivel y razón de escalamiento
- [ ] Mostrar historial de escalamientos
- [ ] Indicador visual del nivel actual

#### 5.5 Pruebas

- [ ] Probar escalamiento entre niveles
- [ ] Verificar registro en historial
- [ ] Validar que solo usuarios autorizados puedan escalar

**Resultado esperado:** Sistema de escalamiento funcional con trazabilidad completa.

---

## 💡 FASE 6: Base de Conocimiento (Knowledge Base)

**Objetivo:** Implementar el módulo de base de conocimiento para reutilizar soluciones.

### Tareas:

#### 6.1 Capa de Dominio

- [ ] Crear entidad `KnowledgeArticle`:
  - Id, Title
  - ServiceId (FK - servicio relacionado)
  - IncidentType (enum)
  - ProblemDescription
  - Steps (JSON o tabla separada para pasos)
  - Recommendations
  - Keywords (para búsqueda)
  - EstimatedResolutionTime
  - AuthorId (FK a User - técnico que creó la solución)
  - RelatedIncidentId (FK opcional - ticket que originó la solución)
  - IsActive
  - CreatedAt, UpdatedAt
- [ ] Crear entidad `SolutionStep`:
  - Id, ArticleId (FK)
  - StepNumber
  - Description
- [ ] Crear entidad `ArticleKeyword` (muchos a muchos)
- [ ] Definir interfaz: `IKnowledgeArticleRepository`

#### 6.2 Capa de Aplicación

- [ ] Implementar comando: `CreateKnowledgeArticleCommand`
- [ ] Implementar comando: `UpdateKnowledgeArticleCommand`
- [ ] Implementar query: `SearchKnowledgeArticlesQuery` (por keywords, servicio, tipo)
- [ ] Implementar query: `GetArticleByIdQuery`
- [ ] Implementar query: `GetRelatedArticlesQuery` (sugerir soluciones similares)
- [ ] Implementar comando: `LinkArticleToIncidentCommand` (vincular solución a ticket)
- [ ] Crear DTOs: `KnowledgeArticleDto`, `CreateArticleDto`, `SolutionStepDto`

#### 6.3 Capa de Infraestructura

- [ ] Implementar `KnowledgeArticleRepository`
- [ ] Configurar relaciones en EF
- [ ] Implementar búsqueda (puede ser con LIKE, Full-Text Search o Azure Search)
- [ ] Crear migración
- [ ] Crear algunos artículos de ejemplo

#### 6.4 Capa de Presentación

- [ ] Crear página de búsqueda de base de conocimiento (`/knowledge`):
  - Buscador por keywords
  - Filtros por servicio y tipo de incidente
  - Listado de resultados
- [ ] Crear página de detalle de artículo (`/knowledge/{id}`):
  - Mostrar problema, pasos, recomendaciones
  - Tiempo estimado de solución
- [ ] Panel para técnicos (`/technician/knowledge`):
  - Crear nuevo artículo
  - Editar artículos existentes
- [ ] Integración con incidentes:
  - Botón "Buscar en base de conocimiento" al crear incidente
  - Sugerencias automáticas basadas en servicio y tipo
  - Botón "Crear artículo" al resolver incidente

#### 6.5 Pruebas

- [ ] Crear artículo desde un incidente resuelto
- [ ] Buscar artículos por diferentes criterios
- [ ] Vincular artículo a incidente
- [ ] Verificar sugerencias automáticas

**Resultado esperado:** Base de conocimiento funcional con búsqueda y vinculación a incidentes.

---

## 🔔 FASE 7: Sistema de Notificaciones (Comunicación Automatizada)

**Objetivo:** Implementar notificaciones para mantener informados a los usuarios.

### Tareas:

#### 7.1 Capa de Dominio

- [ ] Crear entidad `Notification`:
  - Id, UserId (FK)
  - Title, Message
  - Type (enum: IncidentCreated, StatusChanged, Assigned, etc.)
  - RelatedEntityId (IncidentId, por ejemplo)
  - IsRead
  - CreatedAt
- [ ] Crear interfaz: `INotificationService`
- [ ] Definir interfaz: `INotificationRepository`

#### 7.2 Capa de Aplicación

- [ ] Implementar servicio de notificaciones
- [ ] Implementar query: `GetUserNotificationsQuery`
- [ ] Implementar comando: `MarkNotificationAsReadCommand`
- [ ] Modificar comandos existentes para generar notificaciones:
  - Al crear incidente → notificar a personal de TI
  - Al cambiar estado → notificar al creador
  - Al asignar → notificar al técnico asignado
  - Al escalar → notificar a involucrados
- [ ] Crear DTOs: `NotificationDto`

#### 7.3 Capa de Infraestructura

- [ ] Implementar `NotificationRepository`
- [ ] Implementar servicio de notificaciones in-app
- [ ] (Opcional) Implementar envío de emails usando SMTP
- [ ] Configurar plantillas de notificación
- [ ] Crear migración

#### 7.4 Capa de Presentación

- [ ] Agregar campana de notificaciones en barra de navegación
- [ ] Mostrar badge con cantidad de notificaciones no leídas
- [ ] Crear dropdown/panel de notificaciones
- [ ] Marcar como leída al hacer clic
- [ ] Link a la entidad relacionada (incidente)

#### 7.5 Pruebas

- [ ] Verificar notificaciones al crear incidente
- [ ] Verificar notificaciones al cambiar estado
- [ ] Probar marcar como leída
- [ ] (Si implementas) Probar envío de emails

**Resultado esperado:** Sistema de notificaciones in-app funcional con eventos automáticos.

---

## 📊 FASE 8: Estadísticas y Reportes Básicos (Opcional/Mejoras)

**Objetivo:** Agregar visualizaciones y métricas para administradores.

### Tareas:

#### 8.1 Capa de Aplicación

- [ ] Implementar query: `GetIncidentStatisticsQuery`
  - Total de incidentes por estado
  - Total por servicio
  - Total por prioridad
  - Tiempo promedio de resolución
  - Incidentes por técnico
- [ ] Crear DTOs: `StatisticsDto`

#### 8.2 Capa de Presentación

- [ ] Crear dashboard de administrador (`/admin/dashboard`):
  - Gráficos de barras/pastel (usando Chart.js o similar)
  - Métricas clave (KPIs)
  - Tendencias
- [ ] Filtros por fecha

#### 8.3 Pruebas

- [ ] Verificar precisión de estadísticas
- [ ] Probar filtros de fecha

**Resultado esperado:** Dashboard con estadísticas básicas para administradores.

---

## 🎨 FASE 9: Mejoras de UI/UX y Accesibilidad

**Objetivo:** Refinar la interfaz aplicando principios de usabilidad (ISO 9241, DCU, IURE).

### Tareas:

#### 9.1 Diseño visual

- [ ] Mejorar paleta de colores (consistencia semántica):
  - Verde: éxito, resuelto
  - Amarillo: advertencia, en progreso
  - Rojo: error, crítico
  - Azul: información
- [ ] Mejorar tipografía y espaciado
- [ ] Agregar iconos significativos (FontAwesome, Material Icons)
- [ ] Crear componentes reutilizables (botones, cards, modales)

#### 9.2 Usabilidad

- [ ] Implementar feedback visual inmediato (toasts, spinners)
- [ ] Mejorar mensajes de validación
- [ ] Agregar confirmaciones para acciones destructivas
- [ ] Mejorar navegación (breadcrumbs)
- [ ] Implementar búsqueda global

#### 9.3 Accesibilidad

- [ ] Verificar contraste de colores (WCAG)
- [ ] Agregar atributos ARIA donde sea necesario
- [ ] Asegurar navegación por teclado
- [ ] Agregar textos alternativos en imágenes

#### 9.4 Responsive

- [ ] Adaptar layouts para tablets
- [ ] Adaptar layouts para móviles

**Resultado esperado:** Interfaz pulida, consistente y accesible.

---

## ✅ FASE 10: Testing y Documentación Final

**Objetivo:** Asegurar calidad y documentar el sistema.

### Tareas:

#### 10.1 Testing

- [ ] Pruebas de integración completas
- [ ] Pruebas de todos los roles
- [ ] Pruebas de casos extremos
- [ ] Pruebas de seguridad básicas

#### 10.2 Documentación

- [ ] Documentar arquitectura del proyecto
- [ ] Documentar decisiones de diseño (patrones usados)
- [ ] Crear manual de usuario
- [ ] Crear manual técnico
- [ ] Documentar API/servicios

#### 10.3 Preparación para TAM

- [ ] Preparar cuestionarios de usabilidad
- [ ] Preparar escenarios de prueba para usuarios
- [ ] Configurar ambiente de pruebas

**Resultado esperado:** Sistema completo, probado y documentado, listo para evaluación TAM.

---

## 📌 Notas Importantes

### Estrategia de Git

- Crear una rama por cada fase: `feature/phase-1-auth`, `feature/phase-2-catalog`, etc.
- Hacer commits frecuentes con mensajes descriptivos
- Hacer merge a `develop` al completar cada fase
- Hacer merge a `main` solo en versiones estables

### Buenas Prácticas Aplicadas

- **SOLID**: Separación de responsabilidades, inyección de dependencias
- **DRY**: Componentes y servicios reutilizables
- **Patrones**:
  - Repository Pattern (acceso a datos)
  - CQRS (Command Query Responsibility Segregation) con MediatR
  - Unit of Work (si es necesario)
  - Specification Pattern (para filtros complejos)

### Testing Continuo

- Después de cada fase, realizar testing manual exhaustivo
- Probar todos los roles
- Verificar que las funcionalidades anteriores sigan funcionando

### Priorización

Si el tiempo es limitado, estas son las fases **obligatorias**:

1. ✅ Fase 0, 1, 2, 3 (Base del sistema)
2. ✅ Fase 4 (Trazabilidad - requerimiento obligatorio)
3. ✅ Fase 6 (Base de Conocimiento - requerimiento obligatorio)
4. ✅ Fase 7 (Notificaciones - requerimiento obligatorio)

Las fases 5, 8, 9, 10 pueden ajustarse según el tiempo disponible.

