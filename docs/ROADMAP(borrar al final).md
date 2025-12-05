# 🚀 Fase 10-15: Mejoras Avanzadas del Sistema

Este documento describe la planificación por fases de las mejoras adicionales a implementar en el Sistema de Gestión de Incidentes.

---

## 📋 Resumen de Mejoras

| Fase | Nombre | Descripción | Complejidad |
|------|--------|-------------|-------------|
| 10 | Notificaciones por Email | Envío de correos reales con SMTP | Media |
| 11 | Adjuntos en Incidentes | Subir imágenes/archivos | Media |
| 12 | Base de Conocimiento con IA | Búsqueda semántica y sugerencias | Alta |
| 13 | Calificación de Artículos | Sistema de feedback 👍/👎 | Baja |
| 14 | Modo Oscuro | Tema oscuro para la interfaz | Media |
| 15 | Chatbot de Soporte | Bot con IA para preguntas frecuentes | Alta |

---

## 📧 Fase 10: Notificaciones por Email Real ✅ COMPLETADA

### Objetivo
Implementar el envío de correos electrónicos reales utilizando SMTP para notificar a los usuarios sobre eventos importantes de sus incidentes.

### Tareas

#### 10.1 Configuración de Infraestructura
- [x] Configurar settings de SMTP en `appsettings.json`
- [x] Crear clase `EmailSettings` para configuración tipada
- [x] Implementar servicio `IEmailService` / `EmailService`
- [x] Configurar inyección de dependencias

#### 10.2 Plantillas de Email
- [x] Crear plantillas HTML para cada tipo de notificación:
  - Incidente creado
  - Incidente asignado
  - Cambio de estado
  - Comentario agregado
  - Incidente escalado
  - Incidente resuelto
  - Recuperación de contraseña (mejorar el existente)
- [x] Implementar motor de plantillas (EmailTemplateService)
- [x] Diseño responsive para emails

#### 10.3 Integración con Sistema de Notificaciones
- [x] Modificar `NotificationService` para enviar emails (EmailNotificationDecorator)
- [x] Agregar preferencias de notificación por usuario
- [x] Implementar decorador para cadena de notificaciones
- [x] Manejo de errores y logging

#### 10.4 Configuración de Usuario
- [x] Agregar campo `EmailNotificationsEnabled` a `ApplicationUser`
- [x] UI para preferencias de notificación en perfil (Profile.razor)
- [x] Enlace a perfil en menú de navegación

### Estructura de Archivos Implementados
```
IncidentsTI.Application/
├── Services/
│   └── IEmailService.cs                    ✅

IncidentsTI.Infrastructure/
├── Email/
│   ├── EmailSettings.cs                    ✅
│   ├── EmailService.cs                     ✅
│   └── Templates/
│       └── EmailTemplateService.cs         ✅
├── Services/
│   └── EmailNotificationDecorator.cs       ✅

IncidentsTI.Web/
├── Components/Pages/
│   └── Profile.razor                       ✅
├── Components/Layout/
│   └── NavMenu.razor                       ✅ (modificado)
```

### Dependencias NuGet Instaladas
- `MailKit 4.14.1` - Cliente SMTP moderno
- `MimeKit 4.14.0` - Construcción de mensajes MIME

### Configuración SMTP Implementada
```json
{
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
}
```

### Notas de Implementación
- Se usa patrón Decorator para integrar emails con el sistema de notificaciones existente
- Cadena: NotificationService → EmailNotificationDecorator → RealTimeNotificationDecorator
- Emails se envían solo si el usuario tiene habilitada la preferencia
- Plantillas HTML responsivas con soporte de badges de estado coloridos
- Para activar emails, configurar `IsEnabled: true` y credenciales SMTP válidas
```

---

## 📎 Fase 11: Adjuntos y Capturas de Pantalla

### Objetivo
Permitir a los usuarios adjuntar imágenes y archivos a los incidentes para mejorar la descripción visual de los problemas.

### Tareas

#### 11.1 Capa de Dominio
- [ ] Crear entidad `IncidentAttachment`
  - Id, IncidentId, FileName, OriginalFileName
  - ContentType, FileSize, StoragePath
  - UploadedById, UploadedAt
- [ ] Crear interfaz `IIncidentAttachmentRepository`
- [ ] Agregar navegación en `Incident`

#### 11.2 Capa de Aplicación
- [ ] DTOs: `AttachmentDto`, `UploadAttachmentDto`
- [ ] Commands: `UploadAttachmentCommand`, `DeleteAttachmentCommand`
- [ ] Queries: `GetIncidentAttachmentsQuery`
- [ ] Handlers correspondientes
- [ ] Servicio `IFileStorageService` para abstracción

#### 11.3 Capa de Infraestructura
- [ ] `IncidentAttachmentRepository`
- [ ] `LocalFileStorageService` (almacenamiento local)
- [ ] Migración de base de datos
- [ ] Configuración de límites (tamaño máximo, tipos permitidos)

#### 11.4 Capa de Presentación
- [ ] Componente `FileUpload.razor` reutilizable
- [ ] Integración en `CreateIncident.razor`
- [ ] Integración en `IncidentDetail.razor`
- [ ] Galería de imágenes con lightbox
- [ ] Drag & drop para archivos
- [ ] Preview de imágenes antes de subir
- [ ] Indicador de progreso de subida

### Estructura de Archivos
```
IncidentsTI.Domain/
├── Entities/
│   └── IncidentAttachment.cs
├── Interfaces/
│   └── IIncidentAttachmentRepository.cs

IncidentsTI.Application/
├── DTOs/
│   └── Attachments/
│       ├── AttachmentDto.cs
│       └── UploadAttachmentDto.cs
├── Commands/
│   ├── UploadAttachmentCommand.cs
│   └── DeleteAttachmentCommand.cs
├── Queries/
│   └── GetIncidentAttachmentsQuery.cs
├── Services/
│   └── IFileStorageService.cs

IncidentsTI.Infrastructure/
├── Repositories/
│   └── IncidentAttachmentRepository.cs
├── Services/
│   └── LocalFileStorageService.cs

IncidentsTI.Web/
├── Components/
│   └── Shared/
│       ├── FileUpload.razor
│       └── ImageGallery.razor
├── wwwroot/
│   └── uploads/
│       └── incidents/
```

### Configuración
```json
{
  "FileStorage": {
    "BasePath": "wwwroot/uploads",
    "MaxFileSizeMB": 10,
    "AllowedExtensions": [".jpg", ".jpeg", ".png", ".gif", ".pdf", ".doc", ".docx", ".txt"],
    "MaxFilesPerIncident": 5
  }
}
```

### Validaciones
- Tamaño máximo: 10 MB por archivo
- Tipos permitidos: imágenes, PDF, documentos Office
- Máximo 5 archivos por incidente
- Escaneo básico de seguridad (extensión vs contenido)

---

## 🧠 Fase 12: Base de Conocimiento con IA

### Objetivo
Implementar búsqueda semántica inteligente y sugerencias automáticas de artículos relevantes usando procesamiento de lenguaje natural.

### Tareas

#### 12.1 Búsqueda Semántica
- [ ] Integrar servicio de embeddings (OpenAI/Azure OpenAI/Local)
- [ ] Crear tabla `ArticleEmbedding` para vectores
- [ ] Implementar `ISemanticSearchService`
- [ ] Generar embeddings al crear/actualizar artículos
- [ ] Búsqueda por similitud vectorial

#### 12.2 Sugerencias Automáticas al Crear Incidente
- [ ] Componente `ArticleSuggestions.razor`
- [ ] Debounce en input de descripción (500ms)
- [ ] Query `GetSuggestedArticlesQuery`
- [ ] Handler con búsqueda semántica
- [ ] UI con tarjetas de artículos sugeridos
- [ ] Opción "Esto resolvió mi problema" (evitar crear ticket)

#### 12.3 Mejoras en Búsqueda Existente
- [ ] Combinar búsqueda por keywords + semántica
- [ ] Ranking de relevancia mejorado
- [ ] Resaltado de términos coincidentes
- [ ] Filtros avanzados (servicio, tipo, fecha)

### Estructura de Archivos
```
IncidentsTI.Domain/
├── Entities/
│   └── ArticleEmbedding.cs

IncidentsTI.Application/
├── Services/
│   └── ISemanticSearchService.cs
├── Queries/
│   └── GetSuggestedArticlesQuery.cs

IncidentsTI.Infrastructure/
├── Services/
│   └── SemanticSearchService.cs
│   └── OpenAIEmbeddingService.cs

IncidentsTI.Web/
├── Components/
│   └── Shared/
│       └── ArticleSuggestions.razor
```

### Opciones de Implementación

**Opción A: Azure OpenAI (Recomendado para producción)**
```json
{
  "AzureOpenAI": {
    "Endpoint": "https://xxx.openai.azure.com/",
    "ApiKey": "...",
    "EmbeddingDeployment": "text-embedding-ada-002"
  }
}
```

**Opción B: OpenAI Directo**
```json
{
  "OpenAI": {
    "ApiKey": "sk-...",
    "EmbeddingModel": "text-embedding-3-small"
  }
}
```

**Opción C: Local con Sentence Transformers (Sin costo)**
- Usar ML.NET o modelo local
- Menor precisión pero sin dependencias externas

### Dependencias NuGet
- `Azure.AI.OpenAI` o `OpenAI`
- `Microsoft.ML` (si opción local)

---

## ⭐ Fase 13: Sistema de Calificación de Artículos

### Objetivo
Implementar un sistema de feedback para que los usuarios califiquen la utilidad de los artículos de la base de conocimiento.

### Tareas

#### 13.1 Capa de Dominio
- [ ] Crear entidad `ArticleRating`
  - Id, ArticleId, UserId
  - IsHelpful (bool), CreatedAt
  - Comment (opcional)
- [ ] Interfaz `IArticleRatingRepository`
- [ ] Agregar propiedades calculadas a `KnowledgeArticle`:
  - HelpfulCount, NotHelpfulCount, HelpfulPercentage

#### 13.2 Capa de Aplicación
- [ ] DTOs: `ArticleRatingDto`, `RateArticleDto`
- [ ] Commands: `RateArticleCommand`
- [ ] Queries: `GetArticleRatingsQuery`, `GetUserArticleRatingQuery`
- [ ] Actualizar `KnowledgeArticleDto` con métricas

#### 13.3 Capa de Infraestructura
- [ ] `ArticleRatingRepository`
- [ ] Migración de base de datos
- [ ] Índices para consultas eficientes

#### 13.4 Capa de Presentación
- [ ] Componente `ArticleRating.razor` (👍/👎)
- [ ] Integrar en `KnowledgeArticleDetail.razor`
- [ ] Mostrar porcentaje de utilidad en listados
- [ ] Permitir cambiar calificación
- [ ] Modal opcional para comentario

### Estructura de Archivos
```
IncidentsTI.Domain/
├── Entities/
│   └── ArticleRating.cs
├── Interfaces/
│   └── IArticleRatingRepository.cs

IncidentsTI.Application/
├── DTOs/
│   └── KnowledgeBase/
│       └── ArticleRatingDto.cs
├── Commands/
│   └── RateArticleCommand.cs
├── Queries/
│   └── GetArticleRatingsQuery.cs

IncidentsTI.Web/
├── Components/
│   └── Shared/
│       └── ArticleRating.razor
```

### UI de Calificación
```
┌─────────────────────────────────────────────┐
│  ¿Te fue útil este artículo?                │
│                                             │
│     👍 Sí (85%)      👎 No (15%)            │
│                                             │
│  156 personas encontraron útil este artículo│
└─────────────────────────────────────────────┘
```

---

## 🌙 Fase 14: Modo Oscuro

### Objetivo
Implementar un tema oscuro completo para reducir la fatiga visual y mejorar la accesibilidad del sistema.

### Tareas

#### 14.1 Sistema de Temas con Tailwind CSS
- [ ] Configurar `darkMode: 'class'` en `tailwind.config.js`
- [ ] Crear variables CSS para colores del tema
- [ ] Definir paleta de colores oscuros

#### 14.2 Componentes Base
- [ ] Actualizar `MainLayout.razor` con clase `dark`
- [ ] Crear componente `ThemeToggle.razor`
- [ ] Persistir preferencia en localStorage
- [ ] Detectar preferencia del sistema (prefers-color-scheme)

#### 14.3 Actualización de Estilos
- [ ] Revisar y actualizar todos los componentes:
  - Navegación (NavMenu)
  - Tarjetas y modales
  - Tablas y formularios
  - Badges y botones
  - Gráficos (Chart.js)
- [ ] Asegurar contraste WCAG AA en modo oscuro
- [ ] Actualizar colores de estados y prioridades

#### 14.4 Preferencias de Usuario
- [ ] Agregar campo `ThemePreference` a `ApplicationUser`
- [ ] Opciones: Auto, Claro, Oscuro
- [ ] Sincronizar con perfil de usuario

### Estructura de Archivos
```
IncidentsTI.Web/
├── Components/
│   └── Shared/
│       └── ThemeToggle.razor
├── wwwroot/
│   ├── css/
│   │   └── themes.css
│   └── js/
│       └── theme.js
├── tailwind.config.js (modificar)
```

### Configuración Tailwind
```javascript
// tailwind.config.js
module.exports = {
  darkMode: 'class',
  theme: {
    extend: {
      colors: {
        dark: {
          bg: '#0f172a',
          card: '#1e293b',
          border: '#334155',
          text: '#e2e8f0',
          muted: '#94a3b8'
        }
      }
    }
  }
}
```

### Paleta de Colores Oscuros

| Elemento | Claro | Oscuro |
|----------|-------|--------|
| Fondo | `white` | `slate-900` |
| Tarjetas | `white` | `slate-800` |
| Bordes | `gray-200` | `slate-700` |
| Texto principal | `gray-900` | `slate-100` |
| Texto secundario | `gray-500` | `slate-400` |
| Input fondo | `white` | `slate-700` |

---

## 🤖 Fase 15: Chatbot de Soporte Inicial

### Objetivo
Implementar un asistente virtual con IA que responda preguntas frecuentes y guíe a los usuarios antes de crear un ticket.

### Tareas

#### 15.1 Interfaz del Chatbot
- [ ] Componente `Chatbot.razor` flotante
- [ ] Botón de activación en esquina inferior
- [ ] Ventana de chat con historial
- [ ] Indicador de escritura ("typing...")
- [ ] Respuestas con formato (markdown)
- [ ] Botones de acción rápida

#### 15.2 Backend del Chatbot
- [ ] Servicio `IChatbotService`
- [ ] Integración con OpenAI/Azure OpenAI
- [ ] Contexto con base de conocimiento
- [ ] Historial de conversación por sesión
- [ ] Fallback a creación de ticket

#### 15.3 Flujos de Conversación
- [ ] Saludo inicial y menú de opciones
- [ ] Preguntas frecuentes (FAQ)
- [ ] Búsqueda en base de conocimiento
- [ ] Recolección de información para ticket
- [ ] Transferencia a humano (crear ticket)

#### 15.4 Análisis y Mejora
- [ ] Logging de conversaciones (anonimizado)
- [ ] Métricas: tickets evitados, satisfacción
- [ ] Identificar preguntas sin respuesta
- [ ] Retroalimentación post-conversación

### Estructura de Archivos
```
IncidentsTI.Domain/
├── Entities/
│   ├── ChatSession.cs
│   └── ChatMessage.cs

IncidentsTI.Application/
├── DTOs/
│   └── Chat/
│       ├── ChatMessageDto.cs
│       └── ChatResponseDto.cs
├── Services/
│   └── IChatbotService.cs

IncidentsTI.Infrastructure/
├── Services/
│   └── OpenAIChatbotService.cs

IncidentsTI.Web/
├── Components/
│   └── Shared/
│       ├── Chatbot.razor
│       ├── ChatMessage.razor
│       └── ChatInput.razor
├── wwwroot/
│   └── js/
│       └── chatbot.js
```

### Flujo de Conversación

```
┌─────────────────────────────────────────────────────────────┐
│  🤖 Asistente Virtual UTA                              [X]  │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  Bot: ¡Hola! Soy el asistente virtual de TI. ¿En qué      │
│       puedo ayudarte hoy?                                   │
│                                                             │
│       [🔧 Problemas técnicos]  [📧 Correo]  [🌐 WiFi]      │
│                                                             │
│  Usuario: No puedo conectarme al WiFi                       │
│                                                             │
│  Bot: Entiendo que tienes problemas con el WiFi.           │
│       Encontré estos artículos que podrían ayudarte:        │
│                                                             │
│       📄 Cómo conectarse a la red WiFi institucional        │
│       📄 Solución a problemas comunes de WiFi               │
│                                                             │
│       ¿Alguno de estos resuelve tu problema?                │
│                                                             │
│       [✅ Sí, gracias]  [❌ No, necesito más ayuda]         │
│                                                             │
├─────────────────────────────────────────────────────────────┤
│  [Escribe tu mensaje...]                          [Enviar]  │
└─────────────────────────────────────────────────────────────┘
```

### Prompt del Sistema (ejemplo)
```
Eres un asistente virtual de soporte técnico de la Universidad 
Técnica de Ambato. Tu rol es:

1. Ayudar a los usuarios con problemas técnicos comunes
2. Buscar soluciones en la base de conocimiento
3. Recopilar información si necesitan crear un ticket
4. Ser amable, conciso y profesional

Servicios disponibles: Correo, WiFi, Sistemas Académicos, 
Hardware, Software, VPN.

Si no puedes resolver el problema, ofrece crear un ticket 
de soporte.
```

### Dependencias NuGet
- `Azure.AI.OpenAI` o `OpenAI`
- `Microsoft.SemanticKernel` (opcional, para flujos complejos)

---

## 📅 Cronograma Estimado

| Fase | Duración Estimada | Dependencias |
|------|-------------------|--------------|
| 10 - Email | 3-4 días | Ninguna |
| 11 - Adjuntos | 3-4 días | Ninguna |
| 12 - IA KB | 5-7 días | Fase 11 (opcional) |
| 13 - Calificaciones | 2-3 días | Ninguna |
| 14 - Modo Oscuro | 3-4 días | Ninguna |
| 15 - Chatbot | 7-10 días | Fase 12 |

**Total estimado:** 23-32 días de desarrollo

---

## 🔧 Requisitos Técnicos Adicionales

### APIs Externas
- **SMTP**: Gmail, SendGrid, o servidor institucional
- **OpenAI/Azure OpenAI**: Para embeddings y chatbot
- **Almacenamiento**: Local o Azure Blob Storage (archivos)

### Variables de Entorno Necesarias
```env
# Email
SMTP_SERVER=smtp.gmail.com
SMTP_PORT=587
SMTP_USERNAME=...
SMTP_PASSWORD=...

# OpenAI (para IA)
OPENAI_API_KEY=sk-...
# o
AZURE_OPENAI_ENDPOINT=https://...
AZURE_OPENAI_KEY=...

# Almacenamiento (opcional)
AZURE_STORAGE_CONNECTION=...
```

---

## ✅ Criterios de Aceptación por Fase

### Fase 10 - Email
- [ ] Usuarios reciben email al crear incidente
- [ ] Email enviado en cambios de estado
- [ ] Plantillas con diseño profesional
- [ ] Opción de desactivar notificaciones

### Fase 11 - Adjuntos
- [ ] Subir hasta 5 archivos por incidente
- [ ] Preview de imágenes
- [ ] Descarga de archivos
- [ ] Validación de tipos y tamaños

### Fase 12 - IA en KB
- [ ] Búsqueda semántica funcional
- [ ] Sugerencias al escribir descripción
- [ ] Artículos relevantes mostrados

### Fase 13 - Calificaciones
- [ ] Botones 👍/👎 en artículos
- [ ] Porcentaje de utilidad visible
- [ ] Un voto por usuario por artículo

### Fase 14 - Modo Oscuro
- [ ] Toggle funcional en header
- [ ] Todos los componentes adaptados
- [ ] Preferencia persistida
- [ ] Contraste WCAG AA

### Fase 15 - Chatbot
- [ ] Widget flotante funcional
- [ ] Respuestas de IA coherentes
- [ ] Búsqueda en KB integrada
- [ ] Opción de crear ticket desde chat

---