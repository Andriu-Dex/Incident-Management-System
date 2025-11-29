# Phase 2 - Catálogo de Servicios

Esta fase implementa el módulo completo de Catálogo de Servicios que será utilizado como base para los incidentes en las siguientes fases.

### Funcionalidades Implementadas

#### 1. **Gestión de Servicios**
- ✅ Crear nuevos servicios con nombre, descripción y categoría
- ✅ Editar servicios existentes
- ✅ Activar/desactivar servicios (soft delete)
- ✅ Listar todos los servicios con información completa
- ✅ Filtrar servicios activos vs inactivos
- ✅ Validación de datos en formularios

#### 2. **Categorización de Servicios**
- ✅ 6 categorías predefinidas:
  - 📧 **Email** - Servicios de correo institucional
  - 📡 **Network** - Conectividad y redes
  - 📚 **AcademicSystems** - Sistemas académicos y LMS
  - 💻 **Hardware** - Equipos y dispositivos
  - 💿 **Software** - Instalación y licencias
  - 📦 **Other** - Soporte general

#### 3. **Interfaz de Usuario (UI/UX)**
- ✅ Diseño basado en **ISO 9241** (Ergonomía de interacción humano-sistema)
- ✅ Principios de **Diseño Centrado en el Usuario (DCU)**
- ✅ Tabla responsiva con información organizada
- ✅ Colores distintivos por categoría con indicadores visuales
- ✅ Iconos SVG personalizados con gradientes
- ✅ Espaciado vertical mejorado (mejor legibilidad)
- ✅ Badges con bordes y sombras para jerarquía visual
- ✅ Hover effects suaves en filas y botones
- ✅ Modal moderno para crear/editar servicios
- ✅ Contador de caracteres en campos de texto
- ✅ Estados visuales claros (Activo/Inactivo)
- ✅ Área de clic ampliada para accesibilidad (WCAG 2.1)

#### 4. **Autorización y Seguridad**
- ✅ Acceso restringido a roles específicos:
  - **Administrator** - Acceso completo
  - **Technician** - Acceso completo
- ✅ Otros roles no pueden acceder a la gestión de servicios
- ✅ Validación de permisos en backend y frontend

### Arquitectura Implementada

**Domain Layer:**
- `Service` - Entidad principal con Id, Name, Description, Category, IsActive, timestamps
- `ServiceCategory` - Enum con las 6 categorías
- `IServiceRepository` - Interfaz del repositorio

**Application Layer:**
- **Commands:**
  - `CreateServiceCommand` - Crear nuevo servicio
  - `UpdateServiceCommand` - Actualizar servicio existente
  - `DeleteServiceCommand` - Soft delete (marcar como inactivo)
  - `ToggleServiceStatusCommand` - Cambiar estado activo/inactivo
- **Queries:**
  - `GetAllServicesQuery` - Obtener todos los servicios
  - `GetActiveServicesQuery` - Obtener solo servicios activos
  - `GetServiceByIdQuery` - Obtener servicio por ID
- **DTOs:**
  - `ServiceDto` - Representación del servicio para lectura
  - `CreateServiceDto` - Datos para crear servicio
  - `UpdateServiceDto` - Datos para actualizar servicio
- **Handlers:**
  - 4 Command Handlers
  - 3 Query Handlers

**Infrastructure Layer:**
- `ServiceRepository` - Implementación del repositorio con EF Core
- Configuración de DbContext con `DbSet<Service>`
- Migración `AddServicesTable` - Tabla Services en base de datos
- Seed data con 8 servicios predefinidos

**Presentation Layer:**
- `Services.razor` - Página principal de gestión (`/admin/services`)
- `ServiceModal.razor` - Modal para crear/editar servicios
- Integración con Tailwind CSS para estilos
- Toast notifications para feedback
- Navegación con enlace en `NavMenu.razor`

### Servicios Precargados (Seed Data)

El sistema viene con 8 servicios de ejemplo listos para usar:

1. **Correo Institucional** (Email)
   - Creación, recuperación de contraseña, configuración

2. **Red Inalámbrica (WiFi)** (Network)
   - Problemas de conectividad, acceso a WiFi

3. **Sistema de Gestión Académica** (Academic Systems)
   - Matrícula, calificaciones, horarios, plataforma LMS

4. **Soporte de Hardware** (Hardware)
   - Reparación y mantenimiento de equipos

5. **Instalación de Software** (Software)
   - Instalación, actualización y configuración

6. **Acceso a Recursos Digitales** (Academic Systems)
   - Bibliotecas virtuales, bases de datos académicas

7. **VPN Institucional** (Network)
   - Configuración y soporte para acceso remoto

8. **Soporte Técnico General** (Other)
   - Consultas y soporte general

### Paleta de Colores por Categoría

Diseñada siguiendo principios de accesibilidad y contraste:

| Categoría | Color Principal | Fondo | Borde |
|-----------|----------------|-------|-------|
| Email | Púrpura (`purple-500`) | `purple-50` | `purple-300` |
| Network | Cian (`cyan-500`) | `cyan-50` | `cyan-300` |
| Academic Systems | Índigo (`indigo-500`) | `indigo-50` | `indigo-300` |
| Hardware | Naranja (`orange-500`) | `orange-50` | `orange-300` |
| Software | Rosa (`pink-500`) | `pink-50` | `pink-300` |
| Other | Gris (`gray-500`) | `gray-50` | `gray-300` |

### Características de Usabilidad

#### **Cumplimiento de ISO 9241-110:**
1. **Adecuación a la tarea** - Interfaz optimizada para gestión de servicios
2. **Auto-descripción** - Labels claros, tooltips informativos
3. **Conformidad con expectativas** - Patrones de diseño familiares
4. **Adecuación al aprendizaje** - Curva de aprendizaje mínima
5. **Controlabilidad** - Usuario tiene control sobre acciones
6. **Tolerancia a errores** - Validaciones y confirmaciones
7. **Adecuación a la individualización** - Adaptable a diferentes roles

#### **Principios de Diseño Aplicados:**
- **Ley de Fitts** - Área de clic ampliada (mínimo 44x44px)
- **Ley de Hick** - Opciones limitadas y claras
- **Gestalt** - Agrupación visual por proximidad y similitud
- **Jerarquía Visual** - Tamaños, colores y espaciado diferenciados
- **Contraste WCAG 2.1** - Bordes de 2px y colores con buen contraste
- **Feedback Inmediato** - Toast notifications y cambios visuales

### Ejecutar la Aplicación

1. **Navegar al proyecto:**
```bash
cd IncidentsTI.Web
```

2. **Ejecutar la aplicación:**
```bash
dotnet run
```

3. **Abrir en el navegador:**
```
http://localhost:5132
https://localhost:7117
```

4. **Iniciar sesión como administrador:**
```
Email: admin@uta.edu.ec
Password: Admin123!
```

5. **Acceder al catálogo:**
   - Click en "Servicios" en el menú lateral

### Flujo de Uso

#### Crear Nuevo Servicio:
1. Click en botón "➕ Nuevo Servicio"
2. Completar formulario:
   - Nombre del servicio
   - Descripción detallada (máx 1000 caracteres)
   - Seleccionar categoría
3. Click en "✓ Crear Servicio"
4. Toast de confirmación aparece
5. Servicio aparece en la lista

#### Editar Servicio:
1. Click en botón "Editar" en la fila del servicio
2. Modificar campos necesarios
3. Click en "✓ Guardar Cambios"
4. Toast de confirmación aparece
5. Cambios reflejados en la lista

#### Activar/Desactivar Servicio:
1. Click en botón "Desactivar" (rojo) o "Activar" (verde)
2. Toast de confirmación aparece
3. Estado actualizado con opacidad visual reducida si está inactivo

### Validaciones Implementadas

**Campos Obligatorios:**
- Nombre del servicio (máx 200 caracteres)
- Descripción (máx 1000 caracteres)
- Categoría

**Reglas de Negocio:**
- Los servicios no se eliminan físicamente, solo se marcan como inactivos
- El comando `ToggleServiceStatusCommand` invierte el estado actual
- Validación en cliente (Blazor) y servidor (DataAnnotations)

### Estadísticas de la Fase

**Archivos Creados:** 23 archivos nuevos
```
Domain Layer: 3 archivos
Application Layer: 13 archivos
Infrastructure Layer: 3 archivos
Presentation Layer: 2 archivos
Migrations: 2 archivos
```

**Líneas de Código:** ~1,475 líneas agregadas

**Componentes:**
- 4 Commands
- 3 Queries
- 7 Handlers
- 3 DTOs
- 1 Entity
- 1 Enum
- 1 Repository Interface + Implementation
- 2 Razor Components

### Tecnologías Utilizadas

- **.NET 8** - Framework principal
- **Blazor Server** - UI interactiva con `@rendermode InteractiveServer`
- **Entity Framework Core 8** - ORM y migraciones
- **SQL Server** - Base de datos
- **MediatR 13.1.0** - Patrón CQRS
- **Tailwind CSS v3.4.18** - Estilos y diseño responsivo
- **Blazored.Toast 4.2.1** - Notificaciones

### Integración con Futuras Fases

Este módulo es fundamental para las siguientes fases:

**Fase 3 - Gestión de Incidentes:**
- Cada incidente debe estar asociado a un servicio del catálogo
- Los usuarios seleccionarán el servicio al crear un incidente
- Los técnicos podrán corregir el servicio asociado si es necesario

**Fase 6 - Base de Conocimiento:**
- Los artículos de conocimiento estarán vinculados a servicios
- Búsqueda y filtrado de soluciones por servicio

**Fase 8 - Reportes:**
- Estadísticas de incidentes agrupados por servicio
- Análisis de servicios más problemáticos

### Próximas Fases

✅ **Fase 0:** Configuración inicial (Completada)
✅ **Fase 1:** Autenticación y Gestión de Usuarios (Completada)
✅ **Fase 2:** Catálogo de Servicios (Completada)
⏳ **Fase 3:** Gestión Básica de Incidentes
⏳ **Fase 4:** Trazabilidad y Comentarios
⏳ **Fase 5:** Escalamiento de Incidentes
⏳ **Fase 6:** Base de Conocimiento
⏳ **Fase 7:** Sistema de Notificaciones
⏳ **Fase 8:** Estadísticas y Reportes

### Commits Realizados

**Commit Principal:**
```
fad4444 - Fase 2 completada: Catálogo de Servicios funcional con UI mejorada
```

**Archivos Modificados:**
- 31 files changed
- 1,475 insertions(+)

### Capturas de Pantalla

#### Vista Principal del Catálogo
- Tabla con servicios organizados por categorías
- Colores distintivos con puntos indicadores
- Iconos con gradientes por categoría
- Contadores de Total y Activos
- Botones de acción con estados visuales claros

#### Modal de Creación/Edición
- Formulario con validaciones
- Contador de caracteres en descripción
- Selector de categoría con emojis
- Botones con iconos y estados de carga

---

**Desarrollado con:** 🚀 .NET 8 + Blazor Server + Tailwind CSS  
**Arquitectura:** 🏗️ Onion Architecture + CQRS Pattern  
**Diseño:** 🎨 ISO 9241 + DCU + WCAG 2.1
