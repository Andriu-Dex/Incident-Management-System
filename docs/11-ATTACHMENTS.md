# Sistema de Archivos Adjuntos

Esta fase implementa la funcionalidad de subir, visualizar y gestionar archivos adjuntos en los incidentes. Los archivos se almacenan directamente en la base de datos para simplificar el backup y la gestión.

---

## 🏗️ Arquitectura Implementada

### Almacenamiento en Base de Datos
Los archivos se almacenan como `VARBINARY(MAX)` en SQL Server, lo que proporciona:
- **Simplicidad**: Todo en un solo backup
- **Transaccionalidad**: ACID completo
- **Seguridad**: Sin acceso directo al filesystem

### Límites Configurados
| Restricción | Valor |
|-------------|-------|
| Tamaño máximo por archivo | 5 MB |
| Archivos por incidente | 5 máximo |
| Tipos permitidos | Imágenes, PDF, Word, Excel, TXT |

---

## 📁 Archivos Creados

### Capa de Dominio

#### `IncidentsTI.Domain/Entities/IncidentAttachment.cs`
Entidad que representa un archivo adjunto:
```csharp
public class IncidentAttachment
{
    public int Id { get; set; }
    public int IncidentId { get; set; }
    public string FileName { get; set; }          // Nombre único (con GUID)
    public string OriginalFileName { get; set; }   // Nombre original
    public string ContentType { get; set; }        // MIME type
    public long FileSize { get; set; }             // Tamaño en bytes
    public byte[] FileContent { get; set; }        // Contenido del archivo
    public string UploadedById { get; set; }
    public DateTime UploadedAt { get; set; }
    
    // Propiedades calculadas
    public bool IsImage { get; }                   // Detecta si es imagen
    public string FormattedSize { get; }           // "1.5 MB", "500 KB", etc.
}
```

#### `IncidentsTI.Domain/Interfaces/IIncidentAttachmentRepository.cs`
Interface del repositorio:
```csharp
public interface IIncidentAttachmentRepository
{
    Task<IncidentAttachment?> GetByIdAsync(int id);
    Task<IEnumerable<IncidentAttachment>> GetByIncidentIdAsync(int incidentId);
    Task<IncidentAttachment> AddAsync(IncidentAttachment attachment);
    Task DeleteAsync(int id);
    Task<int> GetAttachmentCountByIncidentAsync(int incidentId);
    Task<bool> ExistsAsync(int id);
}
```

### Capa de Aplicación

#### `IncidentsTI.Application/DTOs/AttachmentDto.cs`
DTO para transferencia de datos (sin el contenido binario):
```csharp
public class AttachmentDto
{
    public int Id { get; set; }
    public int IncidentId { get; set; }
    public string FileName { get; set; }
    public string OriginalFileName { get; set; }
    public string ContentType { get; set; }
    public long FileSize { get; set; }
    public string FormattedSize { get; set; }
    public bool IsImage { get; set; }
    public string UploadedById { get; set; }
    public string UploadedByName { get; set; }
    public DateTime UploadedAt { get; set; }
}
```

#### `IncidentsTI.Application/Commands/UploadAttachmentCommand.cs`
Comando para subir archivos con validaciones:
- Valida tipo de archivo permitido
- Valida tamaño máximo (5 MB)
- Valida cantidad máxima por incidente (5)
- Genera nombre único con GUID

#### `IncidentsTI.Application/Commands/DeleteAttachmentCommand.cs`
Comando para eliminar archivos:
- Verifica que el usuario sea el propietario o admin
- Elimina de la base de datos

#### `IncidentsTI.Application/Queries/GetIncidentAttachmentsQuery.cs`
Query para obtener adjuntos de un incidente.

#### `IncidentsTI.Application/Queries/GetAttachmentContentQuery.cs`
Query para obtener el contenido binario (descarga).

### Capa de Infraestructura

#### `IncidentsTI.Infrastructure/Repositories/IncidentAttachmentRepository.cs`
Implementación del repositorio con Entity Framework.

### Capa de Presentación

#### `IncidentsTI.Web/Components/Shared/FileUpload.razor`
Componente reutilizable de carga de archivos:
- **Drag & Drop**: Arrastra archivos al área
- **Click to upload**: Click para seleccionar
- **Preview**: Muestra miniaturas de imágenes
- **Validación**: Tipo y tamaño en cliente
- **Feedback visual**: Indicador de carga

#### `IncidentsTI.Web/Components/Shared/AttachmentGallery.razor`
Componente para mostrar adjuntos:
- **Grid responsive**: 2-4 columnas según pantalla
- **Lightbox**: Click en imagen para ampliar
- **Navegación**: Flechas para recorrer imágenes
- **Descarga**: Botón de descarga directa
- **Eliminar**: Solo propietario o admin

---

## 📝 Archivos Modificados

### `IncidentsTI.Domain/Entities/Incident.cs`
Agregada colección de navegación:
```csharp
public ICollection<IncidentAttachment> Attachments { get; set; } = new List<IncidentAttachment>();
```

### `IncidentsTI.Infrastructure/Data/ApplicationDbContext.cs`
- Agregado DbSet<IncidentAttachment>
- Configuración de relaciones y columna VARBINARY(MAX)

### `IncidentsTI.Web/Program.cs`
- Registro del repositorio
- Endpoints API para descarga de archivos

### `IncidentsTI.Web/Components/Pages/CreateIncident.razor`
Integración del componente FileUpload para adjuntar archivos al crear.

### `IncidentsTI.Web/Components/Pages/IncidentDetail.razor`
- Sección colapsable de adjuntos
- Galería para ver/descargar
- Opción de agregar más archivos

---

## 🗃️ Migración de Base de Datos

### `AddIncidentAttachments`
```sql
CREATE TABLE [IncidentAttachments] (
    [Id] int NOT NULL IDENTITY,
    [IncidentId] int NOT NULL,
    [FileName] nvarchar(max) NOT NULL,
    [OriginalFileName] nvarchar(max) NOT NULL,
    [ContentType] nvarchar(max) NOT NULL,
    [FileSize] bigint NOT NULL,
    [FileContent] VARBINARY(MAX) NOT NULL,
    [UploadedById] nvarchar(450) NOT NULL,
    [UploadedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_IncidentAttachments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_IncidentAttachments_Incidents_IncidentId] FOREIGN KEY ON DELETE CASCADE,
    CONSTRAINT [FK_IncidentAttachments_AspNetUsers_UploadedById] FOREIGN KEY ON DELETE NO ACTION
);

CREATE INDEX [IX_IncidentAttachments_IncidentId];
CREATE INDEX [IX_IncidentAttachments_UploadedById];
```

---

## 🔌 API Endpoints

### Descargar archivo
```
GET /api/attachments/{id}/download
```
Retorna el archivo con headers de descarga.

### Obtener thumbnail (imágenes)
```
GET /api/attachments/{id}/thumbnail
```
Retorna la imagen para preview.

---

## 📋 Tipos de Archivo Permitidos

| Tipo | MIME Types |
|------|------------|
| **Imágenes** | image/jpeg, image/png, image/gif, image/webp |
| **Documentos** | application/pdf |
| **Word** | application/msword, application/vnd.openxmlformats-officedocument.wordprocessingml.document |
| **Excel** | application/vnd.ms-excel, application/vnd.openxmlformats-officedocument.spreadsheetml.sheet |
| **Texto** | text/plain |

---

## 🎨 Interfaz de Usuario

### Componente FileUpload
```
┌─────────────────────────────────────────────────────────┐
│                                                         │
│     ☁️ Haz clic para seleccionar o arrastra aquí       │
│                                                         │
│     Imágenes, PDF, Word, Excel, TXT • Máx. 5 MB        │
│                                                         │
└─────────────────────────────────────────────────────────┘

Archivos seleccionados (2):
┌─────────────────────────────────────────────────────────┐
│ 📷 [preview]  screenshot.png          1.2 MB    ❌     │
│ 📄 [icon]     documento.pdf           500 KB    ❌     │
└─────────────────────────────────────────────────────────┘
```

### Galería de Adjuntos
```
┌──────────┐  ┌──────────┐  ┌──────────┐
│  [IMG]   │  │  [IMG]   │  │   PDF    │
│          │  │          │  │  icon    │
├──────────┤  ├──────────┤  ├──────────┤
│ foto.png │  │ captura  │  │ manual   │
│ 1.2 MB   │  │ 800 KB   │  │ 2.1 MB   │
│ ⬇️  🗑️   │  │ ⬇️  🗑️   │  │ ⬇️  🗑️   │
└──────────┘  └──────────┘  └──────────┘
```

### Lightbox
Al hacer click en una imagen se abre en pantalla completa con:
- Navegación con flechas (si hay múltiples imágenes)
- Botón de descarga
- Click fuera para cerrar

---

## ✅ Funcionalidades Implementadas

- [x] Subir archivos al crear incidente
- [x] Subir archivos a incidente existente
- [x] Preview de imágenes antes de subir
- [x] Drag & drop de archivos
- [x] Validación de tipos permitidos
- [x] Validación de tamaño máximo
- [x] Límite de archivos por incidente
- [x] Galería de imágenes con lightbox
- [x] Descarga de archivos
- [x] Eliminación de archivos (propietario/admin)
- [x] Almacenamiento en base de datos
- [x] API endpoints para descarga

---

## 🔐 Seguridad

| Acción | Permiso |
|--------|---------|
| Ver adjuntos | Cualquier usuario autenticado |
| Subir adjuntos | Creador del incidente + Staff |
| Eliminar adjuntos | Quien lo subió + Administradores |
| Descargar | Cualquier usuario autenticado |

---

## ⚠️ Consideraciones

### Ventajas del almacenamiento en BD
- Backup unificado
- Sin configuración de filesystem
- Transacciones ACID

### Limitaciones
- Mayor uso de memoria de BD
- No ideal para archivos muy grandes (>10MB)
- No escalable para millones de archivos

### Recomendaciones futuras
Para sistemas de alta escala, considerar migrar a:
- Azure Blob Storage
- AWS S3
- Sistema de archivos con referencias en BD
