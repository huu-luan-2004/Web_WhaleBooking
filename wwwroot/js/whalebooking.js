// WhaleBooking Common JavaScript Functions

// Firebase configuration helper
window.WhaleBooking = {
    // Cấu hình Firebase - Thực tế cho WhaleBooking
    firebaseConfig: {
        apiKey: "AIzaSyC49tuQriwSQjBz-Y2JLFJvtbdmS7VHah4",
        authDomain: "whalebooking-e3ea2.firebaseapp.com",
        projectId: "whalebooking-e3ea2",
        storageBucket: "whalebooking-e3ea2.firebasestorage.app",
        messagingSenderId: "58075955129",
        appId: "1:58075955129:web:b70c27a51349ce95cd4068",
        measurementId: "G-3BCN8YD3QH"
    },

    // API URLs (point FE to external API base on port 5000 as per spec)
    getApiUrl: function() {
        const isLocalhost = window.location.hostname === 'localhost' || window.location.hostname === '127.0.0.1';
        return isLocalhost ? 'http://localhost:5000/api' : 'http://10.0.2.2:5000/api';
    },

    // Local storage helpers
    storage: {
        setToken: function(token) {
            localStorage.setItem('accessToken', token);
        },
        getToken: function() {
            return localStorage.getItem('accessToken');
        },
        setUserInfo: function(userInfo) {
            localStorage.setItem('userInfo', JSON.stringify(userInfo));
        },
        getUserInfo: function() {
            try {
                return JSON.parse(localStorage.getItem('userInfo') || '{}');
            } catch {
                return {};
            }
        },
        setUserRoles: function(roles) {
            localStorage.setItem('userRoles', JSON.stringify(roles));
        },
        getUserRoles: function() {
            try {
                return JSON.parse(localStorage.getItem('userRoles') || '[]');
            } catch {
                return [];
            }
        },
        clear: function() {
            localStorage.removeItem('accessToken');
            localStorage.removeItem('userInfo');
            localStorage.removeItem('userRoles');
        }
    },

    // API helpers
    api: {
        // Build absolute image URL when backend returns relative path
        fullImageUrl: function(url) {
            if (!url) return 'https://via.placeholder.com/600x400?text=No+Image';
            if (/^https?:\/\//i.test(url)) return url;
            const base = WhaleBooking.getApiUrl().replace(/\/api$/, '');
            return base + (url.startsWith('/') ? url : ('/' + url));
        },
        call: async function(endpoint, options = {}) {
            const token = WhaleBooking.storage.getToken();
            const defaultHeaders = {};
            // Only set JSON when body is not FormData and caller didn't set it
            const isFormData = options && options.body && options.body instanceof FormData;
            if (!isFormData) {
                defaultHeaders['Content-Type'] = 'application/json';
            }
            
            if (token) {
                defaultHeaders['Authorization'] = `Bearer ${token}`;
            }

            const config = {
                headers: { ...defaultHeaders, ...(options.headers || {}) },
                ...options
            };

            const response = await fetch(`${WhaleBooking.getApiUrl()}${endpoint}`, config);
            
            if (response.status === 401) {
                // Token expired
                WhaleBooking.auth.logout();
                return null;
            }
            
            return response.json();
        },

        // Upload helper for multipart/form-data; do not set Content-Type so browser sets boundary
        upload: async function(endpoint, formData, options = {}) {
            const token = WhaleBooking.storage.getToken();
            const headers = { ...(options.headers || {}) };
            if (token) headers['Authorization'] = `Bearer ${token}`;
            const response = await fetch(`${WhaleBooking.getApiUrl()}${endpoint}`, {
                method: options.method || 'POST',
                headers,
                body: formData
            });
            if (response.status === 401) {
                WhaleBooking.auth.logout();
                return null;
            }
            return response.json();
        }
    },

    // Authentication helpers
    auth: {
        isLoggedIn: function() {
            return !!WhaleBooking.storage.getToken();
        },
        
        hasRole: function(role) {
            const userInfo = WhaleBooking.storage.getUserInfo();
            const roles = WhaleBooking.storage.getUserRoles();
            
            return userInfo.vaiTro === role || 
                   userInfo.role === role || 
                   roles.includes(role);
        },
        
        isAdmin: function() {
            return this.hasRole('Admin');
        },
        
        isOwner: function() {
            return this.hasRole('ChuCoSo');
        },
        
        logout: function() {
            WhaleBooking.storage.clear();
            window.location.href = '/Auth/Login';
        }
    },

    // UI helpers
    ui: {
        showAlert: function(message, type = 'info', elementId = 'alertMessage') {
            const alertElement = document.getElementById(elementId);
            if (alertElement) {
                alertElement.className = `alert alert-${type}`;
                alertElement.innerHTML = `<i class="fas fa-info-circle"></i> ${message}`;
                alertElement.classList.remove('d-none');
                
                // Auto hide after 5 seconds
                setTimeout(() => {
                    alertElement.classList.add('d-none');
                }, 5000);
            }
        },

        showLoading: function(elementId = 'loadingOverlay') {
            const element = document.getElementById(elementId);
            if (element) {
                element.classList.remove('d-none');
            }
        },

        hideLoading: function(elementId = 'loadingOverlay') {
            const element = document.getElementById(elementId);
            if (element) {
                element.classList.add('d-none');
            }
        },

        setButtonLoading: function(buttonId, loading, originalText = '') {
            const button = document.getElementById(buttonId);
            if (button) {
                if (loading) {
                    button.disabled = true;
                    button.innerHTML = '<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>Đang xử lý...';
                } else {
                    button.disabled = false;
                    button.innerHTML = originalText || button.getAttribute('data-original-text') || 'Hoàn thành';
                }
            }
        }
    },

    // Validation helpers
    validation: {
        isValidEmail: function(email) {
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            return emailRegex.test(email);
        },

        isValidPhone: function(phone) {
            const phoneRegex = /^\+?[1-9]\d{1,14}$/;
            return phoneRegex.test(phone.replace(/\s/g, ''));
        },

        isValidPassword: function(password) {
            return password && password.length >= 6;
        }
    },

    // Initialize function
    init: function() {
        console.log('WhaleBooking initialized');
        
        // Add global error handler
        window.addEventListener('error', function(e) {
            console.error('Global error:', e.error);
        });

        // Add unhandled promise rejection handler
        window.addEventListener('unhandledrejection', function(e) {
            console.error('Unhandled promise rejection:', e.reason);
        });
    }
};

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    WhaleBooking.init();
});

// Export for use in modules
if (typeof module !== 'undefined' && module.exports) {
    module.exports = WhaleBooking;
}