# 1. Requisitos del proyecto

## 1.1. Contexto y objetivo general

- **Proyecto:** Sistema de Gestión de Incidentes y Base de Conocimiento de TI.
- **Organización:** Oficina de TI de la UTA.
- **Objetivo general:**
  Diseñar, implementar y evaluar (con TAM) un sistema que permita:

  - Registrar y gestionar incidentes de TI.
  - Asociarlos a un catálogo de servicios.
  - Mantener una base de conocimiento con soluciones reutilizables.
  - Gestionar escalamiento, trazabilidad y notificaciones a usuarios.
  - Evaluar la usabilidad/aceptación del sistema con el modelo TAM.

El proyecto se organiza en **tres fases oficiales**:

1. **Análisis y diseño.**
2. **Codificación.**
3. **Análisis de usabilidad (TAM).**

---

## 1.2. Alcance funcional mínimo (módulos obligatorios)

Según el documento del profe y tus notas, el sistema debe cubrir al menos:

1. **Login y gestión de roles.**
2. **Gestión de incidentes de TI (Help Desk / Service Desk).**
3. **Gestión de Base de Conocimiento.**
4. **Uso de catálogo de servicios en todos los incidentes.**
5. **Escalamiento y trazabilidad de los incidentes.**
6. **Notificaciones a usuarios y personal de TI.**
7. **Evaluación de usabilidad mediante TAM sobre el software funcional.**

A partir de ahí, se derivan los requisitos funcionales.

---

## 1.3. Requisitos funcionales

### 1.3.1. Autenticación y roles

- El sistema debe proporcionar una **pantalla de login**.
- Debe manejar **diferentes tipos de usuario** (roles), al menos:

  - Estudiante
  - Docente
  - Administrativo
  - Técnico de TI
  - Administrador
  - (Opcionalmente: pasante, especialista, proveedor externo)

- El administrador debe poder:

  - Ver la lista de usuarios.
  - (Al menos) activar/desactivar usuarios o ver sus datos básicos.

- Tendrás **usuarios precargados** para pruebas (ej: 3 docentes, 3 admins, técnicos, etc.).

### 1.3.2. Catálogo de servicios de TI

- Debe existir una entidad/módulo de **Catálogo de Servicios**, donde se definan los servicios de TI que ofrece la UTA (ej: correo institucional, red inalámbrica, sistemas académicos, etc.).
- **Todo incidente debe estar asociado a un servicio del catálogo.**
- El técnico debe poder **corregir el servicio asociado** si el usuario se equivocó (cambio de asociación).

### 1.3.3. Gestión de incidentes

- El usuario (docente/estudiante/administrativo) debe poder:

  - Crear un **nuevo incidente** seleccionando:

    - Servicio asociado (del catálogo).
    - Tipo de incidente (ej: falla, consulta, requerimiento).
    - Descripción del problema.

  - Ver el **estado de sus incidentes** (abierto, en progreso, escalado, resuelto, cerrado).
  - Ver el **número de ticket** y detalles básicos.

- El personal de TI (técnico/admin) debe poder:

  - Ver una **bandeja/listado de incidentes** (filtros por estado, servicio, prioridad).
  - Ver el **detalle** del incidente.
  - Cambiar estado del incidente.
  - Registrar **acciones, comentarios u observaciones** (trazabilidad).
  - **Escalar** el incidente a otro nivel/técnico/especialista, dejando trazabilidad de quién lo atendió y por qué se escaló.

- El administrador debe poder:

  - **Asignar** un incidente a un técnico específico.
  - Ver estadísticas básicas (ej: número de incidentes por servicio, por estado, etc. – aunque esto puede ser opcional si el tiempo no alcanza).

### 1.3.4. Base de Conocimiento

- Debe existir un módulo de **Base de Conocimiento** relacionado con:

  - Servicios del catálogo.
  - Tipos de incidentes.
  - Tickets que originaron la solución.

- La base de conocimiento debe permitir:

  - **Registrar soluciones** cuando se resuelve un problema nuevo:

    - Título del problema.
    - Servicio asociado.
    - Tipo de incidente.
    - Descripción del problema.
    - Pasos detallados de solución (Paso 1, Paso 2, …).
    - Recomendaciones.
    - Palabras clave (para búsqueda).
    - Tiempo estimado de solución.
    - Autor de la solución (técnico).

  - **Consultar soluciones** por:

    - Palabras clave.
    - Servicio.
    - Tipo de incidente.

- Integración mínima con incidentes:

  - Antes de crear el incidente o al atenderlo, el sistema **puede sugerir soluciones similares** (aunque esto puede ser inicialmente manual: técnico busca en la base).
  - Cuando un incidente se resuelve usando una solución de la base, se guarda el **link entre ticket y solución**.

### 1.3.5. Notificaciones

- Cuando un usuario crea un incidente, se debe **notificar al personal de TI** (al menos a nivel lógico; si no implementas correo real, puedes simular notificaciones dentro del sistema).
- El usuario debe ser notificado (correo o notificación in-app) cuando:

  - Se crea el ticket.
  - Cambia el estado importante (ej: escalado, resuelto, cerrado).

- Idealmente, el administrador o sistema envía notificaciones “a todos los de TI”, pero solo uno será el responsable final del ticket.

### 1.3.6. Trazabilidad y escalamiento

- Cada incidente debe tener:

  - Historial de estados.
  - Historial de asignaciones (quién lo atendió, a quién se escaló).
  - Historial de comentarios/observaciones.

- Deben definirse **niveles de escalamiento** (ej: Nivel 1: Mesa de ayuda, Nivel 2: Especialista, Nivel 3: Proveedor externo), al menos en términos conceptuales.
- Todo escalamiento debe dejar registro:

  - Fecha y hora.
  - Usuario que escaló.
  - Motivo.

---

## 1.4. Requisitos no funcionales y de diseño de IU

### 1.4.1. Usabilidad y accesibilidad (ISO 9241, DCU, IURE)

El profesor pide específicamente que justifiques:

- **Organización de objetos en la interfaz.**
- **Principios de agrupación** (proximidad, similitud, alineación, etc.).
- **Características principales de una IU** (claridad, consistencia, feedback, control del usuario… y sus derivaciones).
- **Elementos importantes en una UI** (botones, campos, íconos, mensajes, etc., y cómo se usan).
- **Aplicación de estándares de IURE** y normas de usabilidad/accesibilidad (ej: ISO 9241, principios DCU).

Por tanto, el sistema debe:

- Ser **fácil de aprender** (poca curva de aprendizaje).
- Mostrar **estructura clara**: menús, paneles, secciones bien separadas.
- Usar **agrupación visual lógica** de elementos (ej: en el registro de incidente: datos del usuario, datos del servicio, datos del problema).
- Proporcionar **feedback inmediato** (mensajes de éxito/error, estados visibles).
- Evitar sobrecargar la pantalla.
- Usar colores con **significado consistente**:

  - Verde: éxito.
  - Amarillo: advertencia.
  - Rojo: error o incidente crítico.
  - Azul/neutral: información.

ITIL lo usarás solo para **reforzar decisiones** (ej: escalamiento, categorías de incidente), no para implementarlo al 100%.

### 1.4.2. Requisitos técnicos

De tus notas:

- **Arquitectura:** Onion Architecture (capas separadas: dominio, aplicación, infraestructura, presentación).
- **Backend:** .NET (Core) con C#.
- **Frontend:** Blazor (Server o WASM según lo que vean en clase).
- **Base de datos:** SQL Server.
- **ORM:** Entity Framework.

Mínimo:

- Separar **capas** (Domino, Aplicación, Infraestructura, UI).
- Usar Entity Framework para acceso a datos.
- Modelar entidades: Usuario, Rol, Servicio, Incidente, NivelEscalamiento, SolucionConocimiento, etc.

---

## 1.5. Requisitos de prototipado y diseño (Fase 1)

Antes del código, el profesor quiere **Análisis y Diseño** bien hechos:

Debes producir:

1. **Prototipos de baja fidelidad (bocetos en papel)**

   - Hechos a mano (hojas, papelote, cartulina, post-its).
   - Representan pantallas clave:

     - Login.
     - Registro de incidente.
     - Listado de incidentes.
     - Detalle del incidente.
     - Módulo de base de conocimiento.
     - Panel de administración (asignación de tickets).

   - Sirven para:

     - Definir la organización de objetos.
     - Probar la interacción básica con usuarios.

2. **Wireframes (de media/alta fidelidad, digitales)**

   - Para las mismas pantallas principales.
   - Sin diseño gráfico definitivo, pero con:

     - Layout claro (distribución horizontal/vertical).
     - Jerarquía visual.
     - Elementos de UI bien ubicados.

   - Incluyen **flujo de navegación**:

     - Diagrama o mapa que muestre cómo se pasa de una pantalla a otra.

3. **Diagrama de casos de uso**

   - Al menos casos de uso por rol:

     - Usuario: Crear incidente, Consultar estado.
     - Técnico: Ver incidentes, Atender, Escalar, Registrar solución.
     - Admin: Gestionar usuarios, asignar tickets, consultar estadísticas.

---
