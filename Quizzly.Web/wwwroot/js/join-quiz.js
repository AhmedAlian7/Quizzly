document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('joinQuizForm');
    const joinBtn = document.getElementById('joinBtn');
    const accessTokenInput = document.getElementById('accessToken');

    // Form submission with loading state
    form.addEventListener('submit', function (e) {
        if (!accessTokenInput.value.trim()) {
            e.preventDefault();
            showError('Please enter a valid access token.');
            accessTokenInput.focus();
            return;
        }

        // Show loading state
        joinBtn.classList.add('loading');
        joinBtn.disabled = true;

        // Simulate loading time for better UX (remove in production)
        setTimeout(() => {
            if (joinBtn.classList.contains('loading')) {
                joinBtn.classList.remove('loading');
                joinBtn.disabled = false;
            }
        }, 3000);
    });

    // Input validation and formatting
    accessTokenInput.addEventListener('input', function (e) {
        let value = e.target.value.trim();

        // Remove any spaces and convert to uppercase for better UX
        value = value.replace(/\s+/g, '').toUpperCase();

        if (e.target.value !== value) {
            e.target.value = value;
        }

        // Clear any previous error states
        e.target.classList.remove('is-invalid');
    });

    // Auto-focus on the input
    accessTokenInput.focus();

    // Add paste event handler for better UX
    accessTokenInput.addEventListener('paste', function (e) {
        setTimeout(() => {
            let value = e.target.value.trim().replace(/\s+/g, '').toUpperCase();
            e.target.value = value;
        }, 10);
    });

    function showError(message) {
        // Create or update error alert
        let errorAlert = document.querySelector('.alert-danger');
        if (!errorAlert) {
            errorAlert = document.createElement('div');
            errorAlert.className = 'alert alert-danger alert-dismissible fade show';
            errorAlert.innerHTML = `
                        <i class="bi bi-exclamation-triangle me-2"></i>
                        <span></span>
                        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
                    `;
            form.insertBefore(errorAlert, form.firstChild);
        }
        errorAlert.querySelector('span').textContent = message;
        errorAlert.classList.add('show');
    }

    // Handle keyboard shortcuts
    accessTokenInput.addEventListener('keydown', function (e) {
        if (e.key === 'Enter' && !e.shiftKey) {
            e.preventDefault();
            form.dispatchEvent(new Event('submit'));
        }
    });
});