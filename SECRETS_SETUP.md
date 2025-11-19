# Configuración de Secretos (User Secrets)

Este proyecto utiliza **User Secrets** para manejar información sensible como cadenas de conexión en desarrollo.

## ⚙️ Configuración para Desarrollo Local

### 1. Inicializar User Secrets (ya está hecho en el proyecto)

El proyecto ya tiene User Secrets inicializado. Si necesitas verificarlo:

```bash
dotnet user-secrets list --project IncidentsTI.Web
```

### 2. Configurar tu Cadena de Conexión

Ejecuta el siguiente comando con tu configuración de SQL Server:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=TU_SERVIDOR;Database=IncidentsTI;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true" --project IncidentsTI.Web
```

**Ejemplos:**

- SQL Server local con Windows Authentication:
  ```bash
  dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=IncidentsTI;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true" --project IncidentsTI.Web
  ```

- SQL Server con instancia nombrada:
  ```bash
  dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost\SQLEXPRESS;Database=IncidentsTI;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true" --project IncidentsTI.Web
  ```

- SQL Server con autenticación de usuario:
  ```bash
  dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=IncidentsTI;User Id=tu_usuario;Password=tu_password;TrustServerCertificate=True;MultipleActiveResultSets=true" --project IncidentsTI.Web
  ```

### 3. Verificar Configuración

```bash
dotnet user-secrets list --project IncidentsTI.Web
```

### 4. Aplicar Migraciones

Después de configurar tu cadena de conexión:

```bash
dotnet ef database update --project IncidentsTI.Infrastructure --startup-project IncidentsTI.Web
```

## 📍 Ubicación de User Secrets

Los User Secrets se almacenan en tu máquina local en:

- **Windows**: `%APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json`
- **Linux/Mac**: `~/.microsoft/usersecrets/<user_secrets_id>/secrets.json`

**ID del proyecto:** `a657c50d-c40d-4d07-8298-2e914912da7c`

## 🔒 Seguridad

- ✅ User Secrets **NO se suben** al repositorio Git
- ✅ Cada desarrollador configura sus propias credenciales
- ✅ Los archivos `appsettings.json` solo contienen placeholders
- ⚠️ User Secrets es **solo para desarrollo**, en producción usa variables de entorno o Azure Key Vault

## 🚀 Producción

Para producción, configura la cadena de conexión mediante:

- **Variables de entorno**
- **Azure App Service Configuration**
- **Azure Key Vault**
- **Kubernetes Secrets**

Ejemplo de variable de entorno:
```bash
export ConnectionStrings__DefaultConnection="tu-cadena-de-conexion-produccion"
```
