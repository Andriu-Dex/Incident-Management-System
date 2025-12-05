# Sistema de Notificaciones por Email

Esta fase implementa el envío de correos electrónicos reales mediante SMTP para notificar a los usuarios sobre eventos importantes relacionados con sus incidentes. También incluye una página de perfil de usuario para gestionar preferencias personales.

---

## 🏗️ Arquitectura Implementada

### Patrón Decorator
Se utiliza el patrón Decorator para integrar las notificaciones por email sin modificar el servicio de notificaciones existente:

```
NotificationService (base)
    ↓
EmailNotificationDecorator (envía emails)
    ↓
RealTimeNotificationDecorator (SignalR)
```

### Flujo de Notificación
1. Se crea una notificación en el sistema
2. `EmailNotificationDecorator` verifica si el email está habilitado (global y por usuario)
3. Si está habilitado, envía el email correspondiente
4. `RealTimeNotificationDecorator` envía la notificación en tiempo real via SignalR

---

## 📁 Archivos Creados

### Capa de Aplicación

#### `IncidentsTI.Application/Services/IEmailService.cs`
Interface que define los métodos para envío de emails:
- `SendIncidentCreatedEmailAsync` - Notifica al usuario que su incidente fue creado
- `SendIncidentAssignedEmailAsync` - Notifica al técnico que se le asignó un incidente
- `SendStatusChangedEmailAsync` - Notifica cambios de estado
- `SendCommentAddedEmailAsync` - Notifica nuevos comentarios
- `SendIncidentEscalatedEmailAsync` - Notifica escalamientos
- `SendIncidentResolvedEmailAsync` - Notifica resolución de incidentes

### Capa de Infraestructura

#### `IncidentsTI.Infrastructure/Email/EmailSettings.cs`
Clase de configuración para SMTP:
```csharp
public class EmailSettings
{
    public string SmtpServer { get; set; }      // Servidor SMTP
    public int SmtpPort { get; set; }           // Puerto (587 para TLS)
    public bool UseSsl { get; set; }            // Usar SSL/TLS
    public string SenderEmail { get; set; }     // Email remitente
    public string SenderName { get; set; }      // Nombre del remitente
    public string Username { get; set; }        // Usuario SMTP
    public string Password { get; set; }        // Contraseña/App Password
    public bool IsEnabled { get; set; }         // Interruptor global
    public string BaseUrl { get; set; }         // URL base para enlaces
    public int TimeoutSeconds { get; set; }     // Timeout de conexión
}
```

#### `IncidentsTI.Infrastructure/Email/EmailService.cs`
Implementación del servicio de email usando MailKit:
- Conexión segura con SMTP
- Manejo de errores y logging
- Construcción de mensajes MIME
- Soporte para HTML y texto plano

#### `IncidentsTI.Infrastructure/Email/Templates/EmailTemplateService.cs`
Servicio de plantillas HTML con:
- Diseño responsive para todos los dispositivos
- Branding institucional (UTA)
- Badges de estado con colores dinámicos
- Botones de acción con enlaces directos
- Estilos inline para compatibilidad con clientes de email

#### `IncidentsTI.Infrastructure/Services/EmailNotificationDecorator.cs`
Decorador que intercepta las notificaciones y envía emails:
- Verifica configuración global (`IsEnabled`)
- Verifica preferencia del usuario (`EmailNotificationsEnabled`)
- Mapea tipos de notificación a métodos de email
- Manejo de errores sin afectar el flujo principal

### Capa Web

#### `IncidentsTI.Web/Components/Pages/Profile.razor`
Página de perfil de usuario con:
- **Información Personal**: Nombre, apellido, email, teléfono, departamento
- **Preferencias**: Toggle para notificaciones por email
- **Seguridad**: Cambio de contraseña con validación
- Diseño moderno con cards y animaciones

---

## 📝 Archivos Modificados

### `IncidentsTI.Domain/Entities/ApplicationUser.cs`
Nuevos campos agregados:
```csharp
public bool EmailNotificationsEnabled { get; set; } = true;
public string ThemePreference { get; set; } = "Auto";
```

### `IncidentsTI.Web/appsettings.json`
Nueva sección de configuración:
```json
"EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "UseSsl": true,
    "SenderEmail": "noreply@uta.edu.ec",
    "SenderName": "Sistema de Incidentes UTA",
    "Username": "",
    "Password": "",
    "IsEnabled": false,
    "BaseUrl": "https://localhost:7001",
    "TimeoutSeconds": 30
}
```

### `IncidentsTI.Web/Program.cs`
Registro de servicios:
```csharp
// Configuración de Email
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();

// Cadena de decoradores
builder.Services.AddScoped<INotificationService>(provider =>
{
    var baseService = new NotificationService(...);
    var emailDecorator = new EmailNotificationDecorator(baseService, ...);
    var realTimeDecorator = new RealTimeNotificationDecorator(emailDecorator, ...);
    return realTimeDecorator;
});
```

### `IncidentsTI.Web/Components/Layout/NavMenu.razor`
Nuevo enlace al perfil en el menú lateral:
```html
<NavLink href="profile">
    <svg><!-- icono usuario --></svg>
    Mi Perfil
</NavLink>
```

---

## 🗃️ Migración de Base de Datos

### `AddEmailNotificationPreferences`
```sql
ALTER TABLE AspNetUsers ADD EmailNotificationsEnabled BIT NOT NULL DEFAULT 1;
ALTER TABLE AspNetUsers ADD ThemePreference NVARCHAR(20) NOT NULL DEFAULT 'Auto';
```

---

## 📦 Dependencias NuGet

| Paquete | Versión | Propósito |
|---------|---------|-----------|
| MailKit | 4.14.1 | Cliente SMTP moderno y seguro |
| MimeKit | 4.14.0 | Construcción de mensajes MIME |

---

## ⚙️ Configuración para Producción

### Gmail (con App Password)
1. Habilitar verificación en 2 pasos en la cuenta de Google
2. Generar App Password en: https://myaccount.google.com/apppasswords
3. Configurar en `appsettings.json`:
```json
"EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "UseSsl": true,
    "Username": "tu-cuenta@gmail.com",
    "Password": "xxxx-xxxx-xxxx-xxxx",
    "IsEnabled": true
}
```

### Office 365
```json
"EmailSettings": {
    "SmtpServer": "smtp.office365.com",
    "SmtpPort": 587,
    "UseSsl": true,
    "Username": "tu-cuenta@dominio.com",
    "Password": "tu-contraseña",
    "IsEnabled": true
}
```

### Servidor SMTP Propio
```json
"EmailSettings": {
    "SmtpServer": "mail.tudominio.com",
    "SmtpPort": 465,
    "UseSsl": true,
    "Username": "noreply@tudominio.com",
    "Password": "tu-contraseña",
    "IsEnabled": true
}
```

---

## 🔐 Niveles de Control

| Nivel | Ubicación | Alcance | Descripción |
|-------|-----------|---------|-------------|
| **Global** | `appsettings.json` → `IsEnabled` | Todo el sistema | Interruptor maestro del servicio SMTP |
| **Usuario** | Perfil → `EmailNotificationsEnabled` | Individual | Preferencia personal de cada usuario |

**Nota**: Ambos deben estar en `true` para que un usuario reciba emails.

---

## 📧 Tipos de Email Implementados

| Evento | Destinatario | Contenido |
|--------|--------------|-----------|
| Incidente Creado | Usuario reportante | Confirmación con número de ticket |
| Incidente Asignado | Técnico asignado | Detalles del incidente y enlace |
| Cambio de Estado | Usuario reportante | Nuevo estado y descripción |
| Comentario Agregado | Usuario/Técnico | Contenido del comentario |
| Incidente Escalado | Usuario y técnicos | Nivel de escalamiento y razón |
| Incidente Resuelto | Usuario reportante | Solución aplicada |

---

## 🎨 Diseño de Plantillas

Las plantillas de email incluyen:
- ✅ Logo institucional UTA
- ✅ Header con gradiente azul
- ✅ Badges de estado/prioridad con colores
- ✅ Información estructurada en secciones
- ✅ Botón de acción prominente
- ✅ Footer con información de contacto
- ✅ Responsive design (móvil y escritorio)
- ✅ Compatibilidad con principales clientes de email

---

## 📱 Página de Perfil

### Funcionalidades
1. **Ver y editar información personal**
   - Nombre y apellido
   - Teléfono de contacto
   - Departamento

2. **Preferencias de notificación**
   - Toggle para emails (on/off)
   - Preparado para tema oscuro (Fase 14)

3. **Cambiar contraseña**
   - Validación de contraseña actual
   - Confirmación de nueva contraseña
   - Requisitos mínimos de seguridad

---

## ✅ Estado de Implementación

- [x] Configuración de infraestructura SMTP
- [x] Servicio de envío de emails (MailKit)
- [x] Plantillas HTML responsivas
- [x] Decorador de notificaciones
- [x] Preferencias de usuario en BD
- [x] Página de perfil de usuario
- [x] Navegación al perfil
- [x] Documentación completa

