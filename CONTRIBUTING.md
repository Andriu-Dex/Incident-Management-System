# 🤝 Guía de Contribución

¡Gracias por tu interés en contribuir a **IncidentsTI**! Este documento proporciona las pautas y mejores prácticas para contribuir al proyecto.

---

## 📋 Tabla de Contenidos

- [Código de Conducta](#-código-de-conducta)
- [¿Cómo Puedo Contribuir?](#-cómo-puedo-contribuir)
- [Configuración del Entorno](#-configuración-del-entorno)
- [Flujo de Trabajo con Git](#-flujo-de-trabajo-con-git)
- [Estándares de Código](#-estándares-de-código)
- [Proceso de Pull Request](#-proceso-de-pull-request)
- [Reportar Bugs](#-reportar-bugs)
- [Solicitar Funcionalidades](#-solicitar-funcionalidades)

---

## 📜 Código de Conducta

Este proyecto y todos sus participantes están regidos por un código de conducta basado en el respeto mutuo. Al participar, se espera que:

- Uses un lenguaje acogedor e inclusivo
- Respetes los diferentes puntos de vista y experiencias
- Aceptes con gracia las críticas constructivas
- Te enfoques en lo que es mejor para la comunidad
- Muestres empatía hacia otros miembros de la comunidad

---

## 🎯 ¿Cómo Puedo Contribuir?

### 🐛 Reportando Bugs

Si encuentras un bug, por favor crea un [issue](https://github.com/Andriu-Dex/Incident-Management-System/issues) incluyendo:

- **Título descriptivo** del problema
- **Pasos para reproducir** el error
- **Comportamiento esperado** vs **comportamiento actual**
- **Capturas de pantalla** si aplica
- **Entorno**: Sistema operativo, navegador, versión de .NET

### 💡 Sugiriendo Mejoras

¿Tienes una idea para mejorar el proyecto? ¡Genial! Abre un issue con:

- **Descripción clara** de la funcionalidad
- **Justificación** de por qué sería útil
- **Ejemplos de uso** si es posible
- **Mockups o diagramas** opcionales

### 💻 Contribuyendo con Código

1. Busca issues etiquetados como `good first issue` o `help wanted`
2. Comenta en el issue para indicar que trabajarás en él
3. Sigue el flujo de trabajo descrito abajo

### 📝 Mejorando la Documentación

La documentación siempre puede mejorar. Puedes:

- Corregir errores tipográficos
- Mejorar explicaciones existentes
- Agregar ejemplos de código
- Traducir documentación

---

## ⚙️ Configuración del Entorno

### Prerrequisitos

```bash
# Verificar versiones instaladas
dotnet --version    # Requiere .NET 8.0+
node --version      # Requiere Node.js 18+
git --version       # Requiere Git 2.x+
```

### Instalación para Desarrollo

```bash
# 1. Fork del repositorio en GitHub

# 2. Clonar tu fork
git clone https://github.com/TU-USUARIO/Incident-Management-System.git
cd Incident-Management-System/IncidentsTI

# 3. Agregar el repositorio original como upstream
git remote add upstream https://github.com/Andriu-Dex/Incident-Management-System.git

# 4. Instalar dependencias
dotnet restore
cd IncidentsTI.Web && npm install && cd ..

# 5. Configurar base de datos (ver README.md)

# 6. Compilar CSS en modo watch (desarrollo)
cd IncidentsTI.Web
npm run css:watch
```

### Ejecutar en Modo Desarrollo

```bash
# Terminal 1: Tailwind CSS watch
cd IncidentsTI.Web
npm run css:watch

# Terminal 2: Aplicación
dotnet watch run --project IncidentsTI.Web
```

---

## 🌿 Flujo de Trabajo con Git

### Estrategia de Ramas

```
main                    # Producción - código estable
├── develop            # Integración - próxima versión
│   ├── feature/xxx    # Nuevas funcionalidades
│   ├── bugfix/xxx     # Corrección de bugs
│   └── hotfix/xxx     # Fixes urgentes para producción
```

### Convención de Nombres de Ramas

```
feature/descripcion-corta     # Nueva funcionalidad
bugfix/descripcion-del-bug    # Corrección de bug
hotfix/fix-critico            # Fix urgente
docs/actualizar-readme        # Cambios en documentación
refactor/mejorar-servicio     # Refactorización
```

### Proceso de Desarrollo

```bash
# 1. Actualizar develop
git checkout develop
git pull upstream develop

# 2. Crear rama de trabajo
git checkout -b feature/mi-nueva-funcionalidad

# 3. Hacer cambios y commits
git add .
git commit -m "feat: agregar nueva funcionalidad X"

# 4. Mantener actualizado con develop
git fetch upstream
git rebase upstream/develop

# 5. Push a tu fork
git push origin feature/mi-nueva-funcionalidad

# 6. Crear Pull Request en GitHub
```

### Convención de Commits

Seguimos [Conventional Commits](https://www.conventionalcommits.org/):

```
<tipo>(<alcance>): <descripción>

[cuerpo opcional]

[notas de pie opcionales]
```

**Tipos permitidos:**

| Tipo | Descripción |
|------|-------------|
| `feat` | Nueva funcionalidad |
| `fix` | Corrección de bug |
| `docs` | Cambios en documentación |
| `style` | Formato, punto y coma, etc. (sin cambios de código) |
| `refactor` | Refactorización de código |
| `test` | Agregar o corregir tests |
| `chore` | Mantenimiento, dependencias |
| `perf` | Mejoras de rendimiento |

**Ejemplos:**

```bash
feat(incidents): agregar filtro por fecha de creación
fix(auth): corregir error de sesión expirada
docs(readme): actualizar instrucciones de instalación
refactor(services): extraer lógica a nuevo servicio
style(ui): aplicar formato consistente a botones
```

---

## 📏 Estándares de Código

### C# / .NET

- Seguir las [convenciones de Microsoft](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- Usar `PascalCase` para clases, métodos y propiedades públicas
- Usar `camelCase` para variables locales y parámetros
- Usar `_camelCase` para campos privados
- Documentar métodos públicos con XML comments

```csharp
/// <summary>
/// Crea un nuevo incidente en el sistema.
/// </summary>
/// <param name="command">Datos del incidente a crear.</param>
/// <returns>ID del incidente creado.</returns>
public async Task<int> Handle(CreateIncidentCommand command, CancellationToken cancellationToken)
{
    // Implementación
}
```

### Blazor / Razor

- Un componente por archivo
- Separar lógica compleja en archivos `.razor.cs`
- Usar parámetros tipados con `[Parameter]`
- Manejar estados de carga correctamente

```razor
@* Componente bien estructurado *@
@if (_isLoading)
{
    <LoadingSkeleton />
}
else if (_data is null)
{
    <EmptyState Message="No hay datos" />
}
else
{
    <DataDisplay Data="_data" />
}
```

### CSS / Tailwind

- Preferir clases de Tailwind sobre CSS personalizado
- Usar `@apply` para componentes repetitivos
- Mantener consistencia con el sistema de diseño existente
- Seguir principios de accesibilidad (contraste, focus visible)

```css
/* Evitar */
.btn-custom {
    background-color: #3b82f6;
    padding: 8px 16px;
}

/* Preferir */
.btn-custom {
    @apply bg-blue-500 px-4 py-2 rounded-lg hover:bg-blue-600 
           focus:outline-none focus:ring-2 focus:ring-blue-500;
}
```

### SQL / Entity Framework

- Usar migraciones para cambios de esquema
- Evitar queries N+1 (usar `.Include()`)
- Índices para campos de búsqueda frecuente
- Nombrar migraciones descriptivamente

```bash
# Crear migración
dotnet ef migrations add NombreDescriptivo --project IncidentsTI.Infrastructure --startup-project IncidentsTI.Web
```

---

## 🔄 Proceso de Pull Request

### Antes de Crear el PR

- [ ] El código compila sin errores
- [ ] Se han probado los cambios localmente
- [ ] Se ha verificado que no se rompe funcionalidad existente
- [ ] Se han actualizado los tests si aplica
- [ ] Se ha actualizado la documentación si es necesario
- [ ] Los commits siguen la convención establecida

### Crear el Pull Request

1. Ve a tu fork en GitHub
2. Click en "Compare & pull request"
3. Selecciona `develop` como rama destino
4. Completa la plantilla del PR:

```markdown
## Descripción
Breve descripción de los cambios realizados.

## Tipo de Cambio
- [ ] Bug fix (cambio que corrige un issue)
- [ ] Nueva funcionalidad (cambio que agrega funcionalidad)
- [ ] Breaking change (cambio que afecta funcionalidad existente)
- [ ] Documentación

## ¿Cómo se Probó?
Describe los pasos para probar los cambios.

## Checklist
- [ ] Mi código sigue los estándares del proyecto
- [ ] He revisado mi propio código
- [ ] He comentado código complejo
- [ ] He actualizado la documentación
- [ ] Mis cambios no generan nuevos warnings
```

### Revisión de Código

- El PR será revisado por al menos un mantenedor
- Responde a los comentarios de forma constructiva
- Realiza los cambios solicitados en commits adicionales
- Una vez aprobado, el PR será mergeado

---

## 🐛 Reportar Bugs

### Plantilla de Bug Report

```markdown
**Descripción del Bug**
Una descripción clara y concisa del bug.

**Pasos para Reproducir**
1. Ir a '...'
2. Click en '...'
3. Scroll hasta '...'
4. Ver el error

**Comportamiento Esperado**
Qué debería haber pasado.

**Comportamiento Actual**
Qué pasó realmente.

**Capturas de Pantalla**
Si aplica, agregar capturas.

**Entorno:**
 - OS: [ej: Windows 11]
 - Navegador: [ej: Chrome 120]
 - Versión de .NET: [ej: 8.0.11]

**Contexto Adicional**
Cualquier información adicional relevante.
```

---

## 💡 Solicitar Funcionalidades

### Plantilla de Feature Request

```markdown
**¿Tu solicitud está relacionada con un problema?**
Una descripción clara del problema. Ej: Me frustra cuando...

**Describe la solución que te gustaría**
Descripción clara de lo que quieres que pase.

**Describe alternativas consideradas**
Otras soluciones o funcionalidades que hayas considerado.

**Contexto Adicional**
Mockups, diagramas o cualquier información adicional.
```

---

## 📞 Contacto

Si tienes preguntas que no están cubiertas aquí, puedes:

- Abrir un [issue de discusión](https://github.com/Andriu-Dex/Incident-Management-System/issues)
- Contactar por correo: andriudex@gmail.com

---

<div align="center">

**¡Gracias por contribuir a IncidentsTI!** 🎉

</div>
