# Phase 1 - Autenticación y Gestión de Usuarios

## ✅ Completado

Esta fase implementa el sistema completo de autenticación y gestión de usuarios con las siguientes características:

### Funcionalidades Implementadas

#### 1. **Autenticación**
- ✅ Login con email y contraseña
- ✅ Logout con confirmación
- ✅ Sesión persistente (Remember Me)
- ✅ Protección de rutas basada en roles
- ✅ Redirección automática a login para usuarios no autenticados
- ✅ Validación de usuarios activos/inactivos

#### 2. **Gestión de Usuarios**
- ✅ Lista completa de usuarios (solo Administradores)
- ✅ Creación de nuevos usuarios con asignación de roles
- ✅ Activar/desactivar usuarios
- ✅ Visualización de roles por usuario
- ✅ Interfaz con diseño Tailwind CSS

#### 3. **Arquitectura Implementada**

**Domain Layer:**
- `ApplicationUser` - Entidad de usuario extendida de IdentityUser
- `UserRole` - Enum con roles del sistema
- `IUserRepository` - Interfaz del repositorio

**Infrastructure Layer:**
- `UserRepository` - Implementación del repositorio
- `ApplicationDbContext` - Contexto con Identity y seed de roles
- `DatabaseSeeder` - Seed de usuarios de prueba
- Migraciones: `InitialCreate`, `AddApplicationUserAndRoles`

**Application Layer:**
- DTOs: `UserDto`, `LoginDto`, `AuthResponseDto`, `CreateUserDto`
- Commands: `LoginCommand`, `LogoutCommand`, `CreateUserCommand`, `ToggleUserStatusCommand`
- Queries: `GetAllUsersQuery`, `GetUserByIdQuery`
- Handlers para todos los commands y queries

**Presentation Layer:**
- `Login.razor` - Página de inicio de sesión
- `Home.razor` - Dashboard principal con información del usuario
- `Users.razor` - Gestión de usuarios (Administradores)
- `CreateUserModal.razor` - Modal para crear usuarios
- `NavMenu.razor` - Navegación con información de usuario y logout
- `MainLayout.razor` - Layout principal con diseño Tailwind
- `Routes.razor` - Configuración de rutas con protección

### Usuarios de Prueba

La aplicación viene con 10 usuarios pre-cargados para pruebas:

#### Administradores (2)
```
Email: admin@uta.edu.ec
Password: Admin123!
Nombre: Juan Administrador

Email: maria.admin@uta.edu.ec
Password: Admin123!
Nombre: María Administradora
```

#### Técnicos (2)
```
Email: carlos.tech@uta.edu.ec
Password: Tech123!
Nombre: Carlos Técnico

Email: ana.tech@uta.edu.ec
Password: Tech123!
Nombre: Ana Técnica
```

#### Docentes (3)
```
Email: pedro.docente@uta.edu.ec
Password: Teacher123!
Nombre: Pedro Docente

Email: laura.docente@uta.edu.ec
Password: Teacher123!
Nombre: Laura Docente

Email: roberto.docente@uta.edu.ec
Password: Teacher123!
Nombre: Roberto Docente
```

#### Estudiantes (3)
```
Email: sofia.estudiante@uta.edu.ec
Password: Student123!
Nombre: Sofía Estudiante

Email: diego.estudiante@uta.edu.ec
Password: Student123!
Nombre: Diego Estudiante

Email: valentina.estudiante@uta.edu.ec
Password: Student123!
Nombre: Valentina Estudiante
```

### Tecnologías Utilizadas

- **.NET 8** - Framework principal
- **Blazor Server** - UI interactiva con `@rendermode InteractiveServer`
- **ASP.NET Core Identity** - Autenticación y autorización
- **Entity Framework Core 8** - ORM
- **SQL Server** - Base de datos
- **MediatR 13.1.0** - Patrón CQRS
- **Tailwind CSS v3.4.18** - Estilos (compilado con Node.js v20.18.0)
- **Blazored.Toast 4.2.1** - Notificaciones
- **JavaScript (ES6)** - auth.js para login/logout sin conflictos de headers

### Ejecutar la Aplicación

1. **Compilar el proyecto:**
```bash
dotnet build IncidentsTI.sln
```

2. **Instalar dependencias de Node.js (primera vez):**
```bash
cd IncidentsTI.Web
npm install
```

3. **Compilar CSS (Tailwind):**
```bash
npm run css:build
```
O en modo watch para desarrollo:
```bash
npm run css:watch
```

4. **Ejecutar la aplicación:**
```bash
dotnet run
```

5. **Abrir en el navegador:**
```
http://localhost:5132  (HTTP)
https://localhost:7117 (HTTPS)
```

### Solución de Problemas Comunes

#### "Headers are read-only, response has already started"
✅ **Solucionado:** Se usan endpoints HTTP (`/api/auth/login` y `/api/auth/logout`) fuera del circuito de Blazor Server para manejar autenticación.

#### Modal no aparece
✅ **Solucionado:** Modal personalizado con estilos inline y `z-index: 9999`. No depende de librerías externas.

#### Tailwind CSS no aplica estilos
- Ejecutar `npm run css:build` antes de iniciar la aplicación
- Verificar que existe `wwwroot/css/app.min.css`
- En desarrollo, usar `npm run css:watch` para recompilar automáticamente

#### Error de circuito Blazor durante logout
✅ **Esperado:** Los errores de circuito durante logout son normales porque la sesión se cierra antes de que el circuito SignalR termine. No afectan la funcionalidad.

### Protección de Rutas

- `/` - Requiere autenticación
- `/login` - Acceso público
- `/admin/users` - Solo **Administradores**
- `/access-denied` - Página de acceso denegado

### Próximas Fases

✅ **Fase 0:** Configuración inicial (Completada)
✅ **Fase 1:** Autenticación y Gestión de Usuarios (Completada)
⏳ **Fase 2:** Gestión de Incidentes
⏳ **Fase 3:** Catálogo de Servicios
⏳ **Fase 4:** Base de Conocimiento
⏳ **Fase 5:** Sistema de Notificaciones
⏳ **Fase 6:** Escalamiento de Incidentes
⏳ **Fase 7:** Reportes y Análisis
⏳ **Fase 8:** Configuración del Sistema
⏳ **Fase 9:** Optimización y Mejoras
⏳ **Fase 10:** Testing y Deployment

### Notas Técnicas

#### Autenticación con Endpoints HTTP
Para evitar el error "Headers are read-only, response has already started" en Blazor Server, se implementó:
- **Endpoints HTTP fuera del circuito Blazor:** `/api/auth/login` y `/api/auth/logout`
- **JavaScript para redirección:** `auth.js` con funciones `loginUser()` y `logoutUser()`
- **SignIn/SignOut en endpoints HTTP:** La autenticación ocurre fuera del circuito SignalR de Blazor
- **NavigationManager con forceLoad:** Recarga completa de la página después de autenticación

#### Modal Personalizado
Se reemplazó Blazored.Modal por un modal personalizado debido a conflictos de z-index y CSS:
- **Estilos inline:** `position: fixed; z-index: 9999` para máxima compatibilidad
- **EventCallbacks:** `OnUserCreated` y `OnCancel` para comunicación entre componentes
- **Overlay con backdrop:** Click en el fondo cierra el modal

#### Renderizado Estratégico
- **Componentes estáticos:** `NavMenu`, `MainLayout` (acceso correcto a `CascadingAuthenticationState`)
- **Componentes interactivos:** Solo `Login.razor`, `Home.razor`, `Users.razor` con `@rendermode InteractiveServer`
- **CascadingAuthenticationState:** Solo en `Routes.razor` (único lugar correcto)

#### Otras Características
- **Seed Automático:** Al iniciar la aplicación, se ejecuta automáticamente el seed de roles y usuarios si la base de datos está vacía
- **Validaciones:** DataAnnotations en DTOs + validación en cliente (Blazor)
- **Seguridad:** Las contraseñas requieren: 1 mayúscula, 1 minúscula, 1 dígito, mínimo 6 caracteres
- **Sesiones:** La sesión expira después de 8 horas de inactividad con sliding expiration
- **UI en Español:** Toda la interfaz está en español como se solicitó en los requirements
- **Tailwind CSS:** Compilación automática con `npm run css:build` (watch mode: `npm run css:watch`)

---

**Desarrollado con:** 🚀 .NET 8 + Blazor Server + Tailwind CSS  
**Arquitectura:** 🏗️ Onion Architecture + CQRS Pattern  

