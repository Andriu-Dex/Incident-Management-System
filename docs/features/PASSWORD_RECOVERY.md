# 🔐 Sistema de Recuperación de Contraseña

## Descripción General

Se implementó un sistema completo de recuperación de contraseña que permite a los usuarios restablecer su contraseña de forma segura cuando la olvidan. El sistema utiliza tokens criptográficamente seguros con expiración temporal.

---

## 📋 Características Implementadas

### Seguridad
- ✅ **Tokens criptográficamente seguros**: Generados con `RandomNumberGenerator` (32 bytes)
- ✅ **Hash SHA256**: Los tokens se almacenan hasheados en la base de datos
- ✅ **Expiración temporal**: Los tokens expiran después de 1 hora
- ✅ **Uso único**: Cada token solo puede usarse una vez
- ✅ **Anti-enumeración**: Misma respuesta independientemente de si el email existe
- ✅ **Registro de IP**: Se guarda la IP desde donde se solicita el reset
- ✅ **Invalidación automática**: Tokens anteriores se invalidan al generar uno nuevo

### Experiencia de Usuario
- ✅ **Formulario intuitivo**: Diseño consistente con el resto de la aplicación
- ✅ **Indicador de fortaleza**: Barra visual que muestra la seguridad de la contraseña
- ✅ **Mostrar/ocultar contraseña**: Toggle para ver la contraseña mientras se escribe
- ✅ **Validación en tiempo real**: Verificación de coincidencia de contraseñas
- ✅ **Estados visuales claros**: Loading, éxito y error bien diferenciados
- ✅ **Responsive**: Funciona correctamente en móviles y desktop

---

## 🏗️ Arquitectura

### Capa de Dominio (`IncidentsTI.Domain`)

```
Domain/
├── Entities/
│   └── PasswordResetToken.cs      # Entidad para tokens de reset
└── Interfaces/
    └── IPasswordResetTokenRepository.cs  # Contrato del repositorio
```

**PasswordResetToken.cs**
- `Id`: Identificador único (Guid)
- `UserId`: Referencia al usuario
- `Token`: Hash SHA256 del token
- `CreatedAt`: Fecha de creación
- `ExpiresAt`: Fecha de expiración (1 hora)
- `IsUsed`: Indica si ya fue utilizado
- `UsedAt`: Fecha de uso (nullable)
- `RequestedFromIp`: IP desde donde se solicitó

### Capa de Aplicación (`IncidentsTI.Application`)

```
Application/
├── DTOs/
│   └── Auth/
│       ├── ForgotPasswordDto.cs   # DTO para solicitar reset
│       └── ResetPasswordDto.cs    # DTO para validar y resetear
├── Commands/
│   ├── RequestPasswordResetCommand.cs   # Comando solicitar reset
│   ├── ValidateResetTokenCommand.cs     # Comando validar token
│   └── ResetPasswordCommand.cs          # Comando cambiar contraseña
└── Handlers/
    ├── RequestPasswordResetCommandHandler.cs  # Genera token seguro
    ├── ValidateResetTokenCommandHandler.cs    # Valida token
    └── ResetPasswordCommandHandler.cs         # Cambia contraseña
```

### Capa de Infraestructura (`IncidentsTI.Infrastructure`)

```
Infrastructure/
├── Data/
│   └── ApplicationDbContext.cs    # Configuración EF Core (modificado)
└── Repositories/
    └── PasswordResetTokenRepository.cs  # Implementación del repositorio
```

**Configuración de Base de Datos:**
- Índice único en `Token`
- Índice en `UserId`
- Índice en `ExpiresAt`
- Índice compuesto en (`Token`, `IsUsed`, `ExpiresAt`)

### Capa Web (`IncidentsTI.Web`)

```
Web/
├── Components/
│   └── Pages/
│       ├── Login.razor           # Modificado: link a recuperación
│       ├── ForgotPassword.razor  # Nueva página: solicitar reset
│       └── ResetPassword.razor   # Nueva página: cambiar contraseña
├── wwwroot/
│   └── js/
│       └── auth.js               # Funciones JavaScript agregadas
└── Program.cs                    # Endpoints API agregados
```

---

## 🔌 API Endpoints

### POST `/api/auth/forgot-password`
Solicita un token de recuperación de contraseña.

**Request:**
```json
{
  "email": "usuario@ejemplo.com"
}
```

**Response (siempre 200 para evitar enumeración):**
```json
{
  "success": true,
  "message": "Si el correo existe, recibirás instrucciones..."
}
```

### POST `/api/auth/validate-reset-token`
Valida si un token es válido.

**Request:**
```json
{
  "token": "abc123..."
}
```

**Response:**
```json
{
  "isValid": true,
  "maskedEmail": "u***@ejemplo.com"
}
```

### POST `/api/auth/reset-password`
Cambia la contraseña del usuario.

**Request:**
```json
{
  "token": "abc123...",
  "newPassword": "NuevaContraseña123!"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Contraseña actualizada correctamente"
}
```

---

## 🔄 Flujo de Usuario

```
┌─────────────────┐
│   Login Page    │
│                 │
│ [¿Olvidaste tu  │
│  contraseña?]   │──────────────────┐
└─────────────────┘                  │
                                     ▼
                          ┌─────────────────────┐
                          │  Forgot Password    │
                          │                     │
                          │ Ingresa tu email:   │
                          │ [________________]  │
                          │                     │
                          │ [Enviar enlace]     │
                          └─────────────────────┘
                                     │
                                     ▼
                          ┌─────────────────────┐
                          │  Mensaje de Éxito   │
                          │                     │
                          │ ✓ Revisa tu correo  │
                          │                     │
                          │ [Volver al login]   │
                          └─────────────────────┘
                                     │
                          (Usuario recibe email)
                                     │
                                     ▼
                          ┌─────────────────────┐
                          │   Reset Password    │
                          │                     │
                          │ Para: u***@mail.com │
                          │                     │
                          │ Nueva contraseña:   │
                          │ [________________]  │
                          │ ████████░░ Fuerte   │
                          │                     │
                          │ Confirmar:          │
                          │ [________________]  │
                          │                     │
                          │ [Cambiar contraseña]│
                          └─────────────────────┘
                                     │
                                     ▼
                          ┌─────────────────────┐
                          │  ✓ Éxito!           │
                          │                     │
                          │ Contraseña          │
                          │ actualizada         │
                          │                     │
                          │ [Ir al login]       │
                          └─────────────────────┘
```

---

## 🗃️ Migración de Base de Datos

Se aplicó la migración `AddPasswordResetTokens` que crea la tabla:

```sql
CREATE TABLE [PasswordResetTokens] (
    [Id] uniqueidentifier NOT NULL,
    [UserId] nvarchar(450) NOT NULL,
    [Token] nvarchar(128) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [ExpiresAt] datetime2 NOT NULL,
    [IsUsed] bit NOT NULL,
    [UsedAt] datetime2 NULL,
    [RequestedFromIp] nvarchar(45) NULL,
    CONSTRAINT [PK_PasswordResetTokens] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PasswordResetTokens_AspNetUsers_UserId] 
        FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE UNIQUE INDEX [IX_PasswordResetTokens_Token] ON [PasswordResetTokens] ([Token]);
CREATE INDEX [IX_PasswordResetTokens_UserId] ON [PasswordResetTokens] ([UserId]);
CREATE INDEX [IX_PasswordResetTokens_ExpiresAt] ON [PasswordResetTokens] ([ExpiresAt]);
CREATE INDEX [IX_PasswordResetTokens_Token_IsUsed_ExpiresAt] 
    ON [PasswordResetTokens] ([Token], [IsUsed], [ExpiresAt]);
```

---