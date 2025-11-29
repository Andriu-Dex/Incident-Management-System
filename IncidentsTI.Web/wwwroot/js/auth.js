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

// Password Recovery Functions

/**
 * Request a password reset email
 * @param {string} email - User's email address
 * @returns {Promise<{success: boolean, message: string, resetLink?: string}>}
 */
window.requestPasswordReset = async function(email) {
    try {
        const response = await fetch('/api/auth/forgot-password', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ email })
        });

        return await response.json();
    } catch (error) {
        console.error('Error requesting password reset:', error);
        return {
            success: false,
            message: 'Error de conexión. Por favor, intente nuevamente.'
        };
    }
};

/**
 * Validate a password reset token
 * @param {string} token - The reset token from the URL
 * @returns {Promise<{isValid: boolean, maskedEmail?: string, errorMessage?: string}>}
 */
window.validateResetToken = async function(token) {
    try {
        const response = await fetch('/api/auth/validate-reset-token', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ token })
        });

        return await response.json();
    } catch (error) {
        console.error('Error validating reset token:', error);
        return {
            isValid: false,
            errorMessage: 'Error de conexión. Por favor, intente nuevamente.'
        };
    }
};

/**
 * Reset password with a valid token
 * @param {string} token - The reset token
 * @param {string} newPassword - The new password
 * @returns {Promise<{success: boolean, message: string}>}
 */
window.resetPassword = async function(token, newPassword) {
    try {
        const response = await fetch('/api/auth/reset-password', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ token, newPassword })
        });

        return await response.json();
    } catch (error) {
        console.error('Error resetting password:', error);
        return {
            success: false,
            message: 'Error de conexión. Por favor, intente nuevamente.'
        };
    }
};

/**
 * Get URL parameter by name
 * @param {string} name - Parameter name
 * @returns {string|null} - Parameter value or null
 */
window.getUrlParameter = function(name) {
    const urlParams = new URLSearchParams(window.location.search);
    return urlParams.get(name);
};
