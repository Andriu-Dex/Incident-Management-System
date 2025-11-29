/**
 * Cliente SignalR para notificaciones en tiempo real
 * Versión mínima para verificar conexión
 */

// Estado global
window.SignalRNotifications = {
    connection: null,
    isConnected: false
};

/**
 * Inicializa la conexión SignalR
 */
async function initSignalRNotifications() {
    // Verificar que signalR esté disponible
    if (typeof signalR === 'undefined') {
        console.error('[SignalR] La librería signalR no está cargada');
        return false;
    }

    // Si ya está conectado, no hacer nada
    if (window.SignalRNotifications.isConnected) {
        console.log('[SignalR] Ya conectado');
        return true;
    }

    try {
        // Crear conexión
        window.SignalRNotifications.connection = new signalR.HubConnectionBuilder()
            .withUrl("/hubs/notifications")
            .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
            .configureLogging(signalR.LogLevel.Information)
            .build();

        // Eventos de conexión
        window.SignalRNotifications.connection.onreconnecting((error) => {
            console.log('[SignalR] Reconectando...', error);
            window.SignalRNotifications.isConnected = false;
        });

        window.SignalRNotifications.connection.onreconnected((connectionId) => {
            console.log('[SignalR] Reconectado:', connectionId);
            window.SignalRNotifications.isConnected = true;
        });

        window.SignalRNotifications.connection.onclose((error) => {
            console.log('[SignalR] Conexión cerrada', error);
            window.SignalRNotifications.isConnected = false;
        });

        // Handler para Pong (respuesta al ping)
        window.SignalRNotifications.connection.on("Pong", (timestamp) => {
            console.log('[SignalR] Pong recibido:', timestamp);
        });

        // Handler para recibir notificaciones toast
        window.SignalRNotifications.connection.on("ReceiveNotification", (notification) => {
            console.log('[SignalR] 📬 Notificación recibida:', notification);
            showNotificationToast(notification);
        });

        // Handler para actualizaciones de incidentes
        window.SignalRNotifications.connection.on("IncidentUpdated", (data) => {
            console.log('[SignalR] 🔄 Incidente actualizado:', data);
            triggerEvent('incident-updated', data);
        });

        // Handler para refresh de dashboard
        window.SignalRNotifications.connection.on("DashboardRefresh", (data) => {
            console.log('[SignalR] 📊 Dashboard refresh:', data);
            triggerEvent('dashboard-refresh', data);
        });

        // Handler para contador de notificaciones
        window.SignalRNotifications.connection.on("NotificationCountUpdated", (count) => {
            console.log('[SignalR] 🔔 Contador actualizado:', count);
            triggerEvent('notification-count-updated', { count: count });
            updateNotificationBadge(count);
        });

        // Iniciar conexión
        await window.SignalRNotifications.connection.start();
        window.SignalRNotifications.isConnected = true;
        console.log('[SignalR] ✅ Conectado exitosamente');
        
        return true;
    } catch (error) {
        console.error('[SignalR] ❌ Error al conectar:', error);
        return false;
    }
}

/**
 * Envía un ping al servidor
 */
async function pingSignalR() {
    if (!window.SignalRNotifications.isConnected) {
        console.warn('[SignalR] No conectado');
        return;
    }
    
    try {
        await window.SignalRNotifications.connection.invoke("Ping");
        console.log('[SignalR] Ping enviado');
    } catch (error) {
        console.error('[SignalR] Error en ping:', error);
    }
}

/**
 * Cierra la conexión
 */
async function stopSignalRNotifications() {
    if (window.SignalRNotifications.connection) {
        await window.SignalRNotifications.connection.stop();
        window.SignalRNotifications.isConnected = false;
        console.log('[SignalR] Desconectado');
    }
}

// Exponer funciones globalmente
window.initSignalRNotifications = initSignalRNotifications;
window.pingSignalR = pingSignalR;
window.stopSignalRNotifications = stopSignalRNotifications;

/**
 * Muestra una notificación toast
 */
function showNotificationToast(notification) {
    // Crear contenedor si no existe
    let container = document.getElementById('signalr-toast-container');
    if (!container) {
        container = document.createElement('div');
        container.id = 'signalr-toast-container';
        container.className = 'fixed top-4 right-4 z-50 flex flex-col gap-2';
        document.body.appendChild(container);
    }

    // Crear toast
    const toast = document.createElement('div');
    toast.className = 'bg-white border-l-4 border-blue-500 shadow-lg rounded-lg p-4 max-w-sm transform transition-all duration-300 translate-x-0 opacity-100';
    toast.innerHTML = `
        <div class="flex items-start gap-3">
            <div class="flex-shrink-0 text-blue-500">
                <svg class="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
                    <path d="M10 2a6 6 0 00-6 6v3.586l-.707.707A1 1 0 004 14h12a1 1 0 00.707-1.707L16 11.586V8a6 6 0 00-6-6zM10 18a3 3 0 01-3-3h6a3 3 0 01-3 3z"/>
                </svg>
            </div>
            <div class="flex-1 min-w-0">
                <p class="font-semibold text-gray-900 text-sm">${notification.title || 'Notificación'}</p>
                <p class="text-gray-600 text-sm mt-1">${notification.message || ''}</p>
            </div>
            <button class="text-gray-400 hover:text-gray-600" onclick="this.closest('.bg-white').remove()">
                <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z" clip-rule="evenodd"/>
                </svg>
            </button>
        </div>
    `;

    // Click para navegar si hay URL
    if (notification.url) {
        toast.style.cursor = 'pointer';
        toast.onclick = (e) => {
            if (!e.target.closest('button')) {
                window.location.href = notification.url;
            }
        };
    }

    container.appendChild(toast);

    // Auto-remover después de 5 segundos
    setTimeout(() => {
        toast.classList.add('opacity-0', 'translate-x-full');
        setTimeout(() => toast.remove(), 300);
    }, 5000);
}

window.showNotificationToast = showNotificationToast;

/**
 * Dispara un evento personalizado en el documento
 */
function triggerEvent(eventName, detail) {
    const event = new CustomEvent(`signalr:${eventName}`, { detail: detail, bubbles: true });
    document.dispatchEvent(event);
}

/**
 * Actualiza el badge del contador de notificaciones en la UI
 */
function updateNotificationBadge(count) {
    const badges = document.querySelectorAll('[data-notification-count]');
    badges.forEach(badge => {
        badge.textContent = count > 99 ? '99+' : count.toString();
        badge.style.display = count > 0 ? '' : 'none';
    });
}

/**
 * Registra un callback para eventos de SignalR
 * Uso: onSignalREvent('incident-updated', (data) => { ... })
 */
function onSignalREvent(eventName, callback) {
    document.addEventListener(`signalr:${eventName}`, (e) => callback(e.detail));
}

window.triggerEvent = triggerEvent;
window.updateNotificationBadge = updateNotificationBadge;
window.onSignalREvent = onSignalREvent;

/**
 * Registra un DotNetObjectReference para callbacks desde SignalR
 * Uso desde Blazor: await JSRuntime.InvokeVoidAsync("registerSignalRCallback", DotNetObjectReference.Create(this), "MethodName")
 */
const signalRCallbacks = new Map();

function registerSignalRCallback(dotNetRef, eventType, methodName) {
    if (!signalRCallbacks.has(eventType)) {
        signalRCallbacks.set(eventType, []);
    }
    signalRCallbacks.get(eventType).push({ dotNetRef, methodName });
    console.log(`[SignalR] Callback registrado: ${eventType} -> ${methodName}`);
}

function unregisterSignalRCallback(dotNetRef, eventType) {
    if (signalRCallbacks.has(eventType)) {
        const callbacks = signalRCallbacks.get(eventType);
        const filtered = callbacks.filter(cb => cb.dotNetRef !== dotNetRef);
        signalRCallbacks.set(eventType, filtered);
        console.log(`[SignalR] Callback desregistrado: ${eventType}`);
    }
}

// Modificar triggerEvent para invocar callbacks de Blazor
const originalTriggerEvent = triggerEvent;
triggerEvent = function(eventName, detail) {
    // Disparar evento DOM
    const event = new CustomEvent(`signalr:${eventName}`, { detail: detail, bubbles: true });
    document.dispatchEvent(event);
    
    // Invocar callbacks de Blazor
    if (signalRCallbacks.has(eventName)) {
        signalRCallbacks.get(eventName).forEach(async ({ dotNetRef, methodName }) => {
            try {
                await dotNetRef.invokeMethodAsync(methodName, detail);
            } catch (e) {
                console.warn(`[SignalR] Error invocando callback ${methodName}:`, e);
            }
        });
    }
};

window.registerSignalRCallback = registerSignalRCallback;
window.unregisterSignalRCallback = unregisterSignalRCallback;

/**
 * Auto-conectar cuando la página carga (si el usuario está autenticado)
 */
document.addEventListener('DOMContentLoaded', function() {
    // Esperar un momento para que Blazor se inicialice
    setTimeout(async () => {
        // Verificar si hay un indicador de usuario autenticado
        // (el hub requiere autenticación, así que intentamos conectar)
        try {
            await initSignalRNotifications();
        } catch (e) {
            // Si falla, probablemente no está autenticado - ignorar silenciosamente
            console.log('[SignalR] No se pudo conectar (usuario no autenticado?)');
        }
    }, 1000);
});
