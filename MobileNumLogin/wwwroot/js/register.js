// Add this to the beginning of your register.js DOMContentLoaded event
document.addEventListener('DOMContentLoaded', function () {
    const phoneInput = document.getElementById('phoneNumber');

    // Pre-populate phone number if coming from login
    const unregisteredMobile = sessionStorage.getItem('unregisteredMobile');
    if (unregisteredMobile) {
        phoneInput.value = unregisteredMobile;
        sessionStorage.removeItem('unregisteredMobile'); // Clean up


    }

    // ... rest of your existing register.js code
});

document.addEventListener('DOMContentLoaded', function () {
    const registerForm = document.getElementById('registerForm');
    const phoneInput = document.getElementById('phoneNumber');
    const emailInput = document.getElementById('email');
    const firstNameInput = document.getElementById('firstName');
    const lastNameInput = document.getElementById('lastName');
    const salutationSelect = document.getElementById('salutation');
    const termsCheck = document.getElementById('termsCheck');
    const registerBtn = document.getElementById('registerBtn');

    // Only allow numeric input for phone
    phoneInput.addEventListener('keypress', function (e) {
        if (!/[0-9]/.test(e.key) &&
            !['Backspace', 'Delete', 'Tab', 'Escape', 'Enter', 'ArrowLeft', 'ArrowRight'].includes(e.key)) {
            e.preventDefault();
        }
    });

    // Form submission
    registerForm.addEventListener('submit', function (e) {
        e.preventDefault();

        const formData = {
            salutation: salutationSelect.value,
            firstName: firstNameInput.value.trim(),
            lastName: lastNameInput.value.trim(),
            email: emailInput.value.trim(),
            phoneNumber: phoneInput.value.trim(),
            termsAccepted: termsCheck.checked
        };

        // Basic validation - only check if required fields are filled
        if (!formData.firstName || !formData.lastName || !formData.email || !formData.phoneNumber || !formData.termsAccepted) {
            showToast('Please fill all required fields', 'error');
            return;
        }

        // Show loading state
        showLoadingState(registerBtn, true);

        fetch('/Account/Register', {
            method: 'POST',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            body: new URLSearchParams({
                Salutation: formData.salutation,
                Fname: formData.firstName,
                Lname: formData.lastName,
                Email: formData.email,
                MobileNo: formData.phoneNumber
            })
        })
            .then(res => {
                if (res.redirected) {
                    sessionStorage.setItem('registrationData', JSON.stringify(formData));
                    sessionStorage.setItem('phoneNumber', formData.phoneNumber);
                    sessionStorage.setItem('otpContext', 'register');
                    window.location.href = res.url;
                } else {
                    res.text().then(html => document.body.innerHTML = html);
                }
            })
            .catch(() => showToast('Server error sending OTP', 'error'))
            .finally(() => showLoadingState(registerBtn, false));
    });

    function showLoadingState(button, isLoading) {
        const btnText = button.querySelector('.btn-text');
        const spinner = button.querySelector('.spinner-border');

        if (isLoading) {
            btnText.textContent = 'Registering...';
            spinner.classList.remove('d-none');
            button.disabled = true;
        } else {
            btnText.textContent = 'Register';
            spinner.classList.add('d-none');
            button.disabled = false;
        }
    }

    function showToast(message, type = 'info') {
        const toast = document.getElementById('customToast');
        const toastBody = toast.querySelector('.toast-body');
        const toastTitle = toast.querySelector('.toast-title');

        toast.classList.remove('toast-success', 'toast-error', 'toast-warning', 'toast-info');
        toast.classList.add(`toast-${type}`);

        toastBody.textContent = message;
        toastTitle.textContent = getToastTitle(type);

        const bsToast = new bootstrap.Toast(toast);
        bsToast.show();
    }

    function getToastTitle(type) {
        const titles = {
            'success': 'Success',
            'error': 'Error',
            'warning': 'Warning',
            'info': 'Information'
        };
        return titles[type] || 'Notification';
    }
});
