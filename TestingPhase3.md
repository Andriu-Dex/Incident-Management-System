## 📋 Plan de Testing Completo

### **1. Testing como Usuario Normal (Estudiante/Docente/Administrativo)**

**Login:**
- Email: `sofia.estudiante@uta.edu.ec` o `pedro.docente@uta.edu.ec`
- Password: `Student123!` o `Teacher123!`

**Pruebas:**
- ✅ **Ver "Mis Incidentes"** en el menú lateral (debe aparecer)
- ✅ **NO ver** "Dashboard Técnico" ni "Gestión de Incidentes" (roles restringidos)
- ✅ **Crear nuevo incidente:**
  - Click en "➕ Nuevo Incidente"
  - Seleccionar servicio (ej: "Correo Institucional")
  - Tipo: Falla, Consulta o Requerimiento
  - Prioridad: Baja, Media, Alta o Crítica
  - Título: "No puedo acceder a mi correo institucional"
  - Descripción: Detalle del problema
  - Verificar que se genera número de ticket **INC-2025-0001**
  - Verificar mensaje de éxito con toast
- ✅ **Ver lista de mis incidentes:**
  - Debe mostrar solo los incidentes creados por ti
  - Verificar contadores (Total, Abiertos, En Progreso, Cerrados)
  - Probar filtros por Estado y Prioridad
  - Click en "👁️ Ver" para ver detalle
- ✅ **Ver detalle del incidente:**
  - Verificar toda la información: ticket, título, descripción, servicio, estado, prioridad
  - Verificar que apareces como "Reportado por"
  - Si no está asignado, debe decir "Sin asignar"

---

### **2. Testing como Técnico**

**Login:**
- Email: `carlos.tech@uta.edu.ec`
- Password: `Tech123!`

**Pruebas:**
- ✅ **Ver "Dashboard Técnico"** en el menú lateral
- ✅ **Ver lista vacía inicialmente** (no hay incidentes asignados)
- ✅ **Esperar a que el administrador te asigne un incidente**
- ✅ **Una vez asignado:**
  - Ver incidente en tu dashboard
  - Verificar estadísticas (Asignados, En Progreso, Alta Prioridad, Resueltos)
  - **Cambiar estado del incidente** usando el dropdown:
    - Cambiar a "En Progreso"
    - Verificar toast de éxito
    - Cambiar a "Resuelto"
    - Cambiar a "Cerrado"
  - Click en "👁️ Ver" para ver el detalle completo

---

### **3. Testing como Administrador**

**Login:**
- Email: `admin@uta.edu.ec`
- Password: `Admin123!`

**Pruebas:**
- ✅ **Ver TODOS los menús:**
  - Mis Incidentes
  - Dashboard Técnico
  - Gestión de Incidentes
  - Servicios
  - Usuarios
- ✅ **Ir a "Gestión de Incidentes"** (`/admin/incidents`)
- ✅ **Verificar estadísticas:**
  - Total de incidentes
  - Abiertos, En Progreso, Sin Asignar, Cerrados
- ✅ **Probar filtros:**
  - Por Estado (Abierto, En Progreso, etc.)
  - Por Prioridad (Baja, Media, Alta, Crítica)
  - Por Asignación (Sin Asignar, Asignados)
  - Click en "🔄 Limpiar" para resetear filtros
- ✅ **Asignar incidente a técnico:**
  - En la columna "Asignado", seleccionar un técnico del dropdown
  - Debe aparecer Carlos Técnico o Ana Técnica
  - Verificar toast de éxito "✓ Técnico asignado"
  - Verificar que el contador "Sin Asignar" disminuye
- ✅ **Desasignar incidente:**
  - Seleccionar "-- Sin asignar --" en el dropdown
  - Verificar toast "✓ Incidente desasignado"
- ✅ **Ver que los cambios se reflejan en tiempo real**

---

### **4. Testing de Flujo Completo (Integración)**

**Escenario: Ciclo de vida de un incidente**

1. **Usuario crea incidente** (como estudiante)
   - Crea incidente: "Problema con WiFi"
   - Servicio: "Red Inalámbrica"
   - Prioridad: Alta
   - Verifica ticket: **INC-2025-0002** (secuencial)

2. **Administrador asigna** (como admin)
   - Ve el nuevo incidente en lista
   - Lo asigna a Carlos Técnico
   - Verifica que aparece en "Asignados"

3. **Técnico trabaja el incidente** (como técnico)
   - Ve el incidente en su dashboard
   - Cambia estado a "En Progreso"
   - Trabaja en la solución
   - Cambia estado a "Resuelto"

4. **Usuario verifica resolución** (como estudiante)
   - Ve en "Mis Incidentes" que el estado cambió a "Resuelto"
   - Ve quién lo atendió (Carlos Técnico)

5. **Técnico cierra** (como técnico)
   - Cambia estado final a "Cerrado"

---

### **5. Testing de Validaciones**

- ✅ **Campos requeridos en crear incidente:**
  - Intentar crear sin seleccionar servicio → debe mostrar error
  - Título vacío → error de validación
  - Descripción vacía → error de validación
- ✅ **Límites de caracteres:**
  - Título: máximo 200 caracteres (debe mostrar contador)
  - Descripción: máximo 2000 caracteres (debe mostrar contador)

---

### **6. Testing de Autorización**

- ✅ **Usuario normal NO puede:**
  - Acceder a `/technician/dashboard` → debe redirigir o mostrar Access Denied
  - Acceder a `/admin/incidents` → debe redirigir o mostrar Access Denied
  - Ver opciones de cambiar estado en sus propios incidentes

- ✅ **Técnico puede:**
  - Ver su dashboard
  - Cambiar estados de incidentes asignados
  - Ver detalle de cualquier incidente

- ✅ **Administrador puede TODO**

---

### **7. Testing de Generación de Tickets**

- ✅ **Crear múltiples incidentes y verificar:**
  - INC-2025-0001
  - INC-2025-0002
  - INC-2025-0003
  - Numeración secuencial correcta
  - No duplicados (índice único en base de datos)

---

### **8. Testing de UI/UX (ISO 9241)**

- ✅ **Diseño visual:**
  - Colores distintivos por prioridad (verde→amarillo→naranja→rojo)
  - Colores por estado (verde→amarillo→naranja→azul→gris)
  - Badges con bordes de 2px para contraste
  - Espaciado adecuado (px-8 py-6)
  - Hover effects funcionando
- ✅ **Contadores actualizándose** en tiempo real
- ✅ **Toast notifications** apareciendo correctamente
- ✅ **Iconos SVG** mostrándose en cada sección
- ✅ **Responsividad:** probar en diferentes tamaños de ventana

---

### **9. Verificaciones en Base de Datos (Opcional)**

Si quieres verificar directamente en SQL Server:

```sql
-- Ver todos los incidentes
SELECT * FROM Incidents;

-- Ver tickets generados
SELECT TicketNumber, Title, Status, Priority, CreatedAt FROM Incidents;

-- Ver relaciones
SELECT 
    i.TicketNumber,
    u.UserName as CreatedBy,
    s.Name as Service,
    t.UserName as AssignedTo
FROM Incidents i
JOIN AspNetUsers u ON i.UserId = u.Id
JOIN Services s ON i.ServiceId = s.Id
LEFT JOIN AspNetUsers t ON i.AssignedToId = t.Id;
```

---

## ✅ Checklist Final

Marca cada ítem cuando lo pruebes:

- [ ] Usuario puede crear incidente
- [ ] Tickets se generan secuencialmente
- [ ] Usuario ve solo sus incidentes
- [ ] Filtros funcionan correctamente
- [ ] Vista de detalle muestra toda la información
- [ ] Administrador ve todos los incidentes
- [ ] Administrador puede asignar técnicos
- [ ] Técnico ve solo incidentes asignados
- [ ] Técnico puede cambiar estados
- [ ] Contadores se actualizan correctamente
- [ ] Validaciones de formulario funcionan
- [ ] Autorización por roles funciona
- [ ] UI responsive y con buen contraste
- [ ] Toast notifications aparecen
