document.addEventListener('DOMContentLoaded', function () {
    const loginForm = document.getElementById('loginForm');
    const phoneInput = document.getElementById('phoneNumber');
    const getOtpBtn = document.getElementById('getOtpBtn');
    const otpForm = document.getElementById('otpForm');
    const otpInput = document.getElementById('otpInput');
    const verifyOtpBtn = document.getElementById('verifyOtpBtn');

    if (otpForm) {
        otpForm.addEventListener('submit', function (e) {
            e.preventDefault();

            const otpValue = otpInput.value.trim();

            if (!otpValue) {
                showToast('Please enter the OTP', 'error');
                return;
            }

            verifyOtpBtn.disabled = true;

            fetch('/Account/VerifyOtp', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'X-Requested-With': 'XMLHttpRequest'
                },
                body: new URLSearchParams({ otpInput: otpValue })
            })
                .then(res => res.json())
                .then(async result => {
                    if (result.success) {
                        if (result.proxyUrl) {
                            showToast('Login successful! Loading content...', 'success');
                            // Fetch the external content via your proxy
                            const proxyResponse = await fetch(result.proxyUrl);
                            if (!proxyResponse.ok) {
                                showToast('Failed to load external content', 'error');
                                return;
                            }
                            const content = await proxyResponse.text();
                            // Here you can decide how to display it
                            // For demo, let's open in new tab
                            const newWindow = window.open();
                            newWindow.document.write(content);
                            newWindow.document.close();
                        }
                    } else {
                        showToast(result.message || 'OTP verification failed', 'error');
                    }
                })
                .catch(() => showToast('Server error verifying OTP', 'error'))
                .finally(() => {
                    verifyOtpBtn.disabled = false;
                });
        });

    }

    // Show server-side messages on page load
    if (window.serverMessages) {
        if (window.serverMessages.success) {
            showToast(window.serverMessages.success, 'success');
        }
        if (window.serverMessages.error) {
            showToast(window.serverMessages.error, 'error');
        }
        if (window.serverMessages.errors && window.serverMessages.errors.length > 0) {
            window.serverMessages.errors.forEach(error => {
                showToast(error, 'error');
            });
        }
    }

    // Only allow numeric input for phone
    phoneInput.addEventListener('keypress', function (e) {
        if (!/[0-9]/.test(e.key) &&
            !['Backspace', 'Delete', 'Tab', 'Escape', 'Enter', 'ArrowLeft', 'ArrowRight'].includes(e.key)) {
            e.preventDefault();
        }
    });

    // Form submission
    loginForm.addEventListener('submit', function (e) {
        e.preventDefault();

        const phoneNumber = phoneInput.value.trim();

        // Basic validation - only check if phone number is provided
        if (!phoneNumber) {
            showToast('Please enter your phone number', 'error');
            return;
        }

        // Show loading state
        showLoadingState(getOtpBtn, true);

        fetch('/Account/Login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'X-Requested-With': 'XMLHttpRequest'
            },
            body: new URLSearchParams({ mobileNo: phoneNumber })
        })
            .then(async res => {
                const contentType = res.headers.get('content-type');

                if (contentType && contentType.includes('application/json')) {
                    // Handle JSON response
                    const result = await res.json();

                    if (result.success) {
                        // Success - store data and redirect
                        sessionStorage.setItem('phoneNumber', phoneNumber);
                        sessionStorage.setItem('otpContext', 'login');
                        window.location.href = result.redirectUrl;
                    } else {
                        // Show error toast
                        showToast(result.message, 'error');

                        // If it's an unregistered number, redirect after showing toast
                        if (result.redirectUrl && result.unregisteredMobile) {
                            setTimeout(() => {
                                // Store the mobile number for pre-population
                                sessionStorage.setItem('unregisteredMobile', result.unregisteredMobile);
                                window.location.href = result.redirectUrl;
                            }, 2000); // Wait 2 seconds to show the toast
                        }
                    }
                } else if (res.redirected) {
                    // Handle traditional redirect
                    sessionStorage.setItem('phoneNumber', phoneNumber);
                    sessionStorage.setItem('otpContext', 'login');
                    window.location.href = res.url;
                } else {
                    // Handle HTML response (fallback)
                    const html = await res.text();
                    document.body.innerHTML = html;
                }
            })
            .catch(() => showToast('Server error sending OTP', 'error'))
            .finally(() => showLoadingState(getOtpBtn, false));
    });

    function showLoadingState(button, isLoading) {
        const btnText = button.querySelector('.btn-text');
        const spinner = button.querySelector('.spinner-border');

        if (isLoading) {
            btnText.textContent = 'Sending OTP...';
            spinner.classList.remove('d-none');
            button.disabled = true;
        } else {
            btnText.textContent = 'Get OTP';
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
