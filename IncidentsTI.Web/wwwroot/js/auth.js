window.loginUser = async function(email, password, rememberMe) {
    try {
        const response = await fetch('/api/auth/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ email, password, rememberMe })
        });

        const result = await response.json();

        if (result.success) {
            // Redirigir a la página principal
            window.location.href = '/';
            return result;
        }

        return result;
    } catch (error) {
        console.error('Error en login:', error);
        return {
            success: false,
            message: 'Error de conexión: ' + error.message
        };
    }
};

window.logoutUser = function() {
    // Hacer el POST pero NO esperar la respuesta
    fetch('/api/auth/logout', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        }
    }).catch(() => {
        // Ignorar errores
    });
    
    // Redirigir inmediatamente
    window.location.href = '/login';
};
