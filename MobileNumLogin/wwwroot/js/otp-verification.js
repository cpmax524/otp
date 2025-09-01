document.addEventListener('DOMContentLoaded', function () {
    const otpForm = document.getElementById('otpForm');
    const otpInputs = document.querySelectorAll('.otp-input');
    const verifyBtn = document.getElementById('verifyOtpBtn');
    const resendBtn = document.getElementById('resendOtpBtn');
    const phoneDisplay = document.getElementById('phoneDisplay');
    const resendTimer = document.getElementById('resendTimer');

    let resendCountdown = 0;
    let resendInterval;

    // Initialize page
    initializePage();

    // OTP input handling
    otpInputs.forEach((input, index) => {
        input.addEventListener('input', function (e) {
            const value = e.target.value;

            // Only allow numbers
            if (!/^\d$/.test(value)) {
                e.target.value = '';
                return;
            }

            // Move to next input
            if (value && index < otpInputs.length - 1) {
                otpInputs[index + 1].focus();
            }

            // Update visual state
            updateOtpInputState(input, value);

            // Check if all inputs are filled
            checkOtpComplete();
        });

        input.addEventListener('keydown', function (e) {
            // Handle backspace
            if (e.key === 'Backspace' && !e.target.value && index > 0) {
                otpInputs[index - 1].focus();
                otpInputs[index - 1].value = '';
                updateOtpInputState(otpInputs[index - 1], '');
            }
        });

        input.addEventListener('paste', function (e) {
            e.preventDefault();
            const pastedData = e.clipboardData.getData('text');
            const digits = pastedData.replace(/\D/g, '').slice(0, 6);

            digits.split('').forEach((digit, i) => {
                if (otpInputs[i]) {
                    otpInputs[i].value = digit;
                    updateOtpInputState(otpInputs[i], digit);
                }
            });

            checkOtpComplete();
        });
    });

    // Form submission
    otpForm.addEventListener('submit', function (e) {
        e.preventDefault();

        const otp = getOtpValue();

        if (otp.length !== 6) {
            showToast('Please enter the complete 6-digit OTP', 'error');
            shakeOtpInputs();
            return;
        }

        // Show loading state
        showLoadingState(verifyBtn, true);

        // Simulate API call
        fetch('/Account/VerifyOtp', {
            method: 'POST',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            body: new URLSearchParams({ otpInput: otp })
        })
            .then(res => {
                if (res.redirected) {
                    showToast('OTP verified successfully!', 'success');
                    sessionStorage.clear();
                    setTimeout(() => window.location.href = res.url, 1500);
                } else {
                    showToast('Invalid OTP. Please try again.', 'error');
                    shakeOtpInputs();
                }
            })
            .catch(() => {
                showToast('Server error verifying OTP', 'error');
                shakeOtpInputs();
            })
            .finally(() => showLoadingState(verifyBtn, false));

    });

    // Resend OTP
    resendBtn.addEventListener('click', function () {
        if (resendCountdown > 0) return;

        const context = sessionStorage.getItem('otpContext');
        const phone = sessionStorage.getItem('phoneNumber');
        const regData = JSON.parse(sessionStorage.getItem('registrationData') || '{}');

        let url = '';
        let body = new URLSearchParams();

        if (context === 'login') {
            url = '/Account/Login';
            body.append('mobileNo', phone);
        } else {
            url = '/Account/Register';
            body.append('Salutation', regData.salutation);
            body.append('Fname', regData.firstName);
            body.append('Lname', regData.lastName);
            body.append('Email', regData.email);
            body.append('MobileNo', regData.phoneNumber);
        }

        fetch(url, {
            method: 'POST',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            body: body
        }).then(() => showToast('OTP resent successfully!', 'success'));

        startResendCountdown();
    });


    function initializePage() {
        const phoneNumber = sessionStorage.getItem('phoneNumber');
        if (phoneNumber) {
            // Display phone number without country code, with masking
            const maskedPhone = `${phoneNumber.slice(0, 2)}${'*'.repeat(6)}${phoneNumber.slice(-2)}`;
            phoneDisplay.textContent = maskedPhone;
        }

        // Start initial countdown
        startResendCountdown();

        // Focus first input
        otpInputs[0].focus();
    }

    function updateOtpInputState(input, value) {
        if (value) {
            input.classList.add('filled');
            input.classList.remove('is-invalid');
        } else {
            input.classList.remove('filled');
        }
    }

    function checkOtpComplete() {
        const otp = getOtpValue();
        const isComplete = otp.length === 6;

        verifyBtn.disabled = !isComplete;

        if (isComplete) {
            otpInputs.forEach(input => {
                input.classList.remove('is-invalid');
            });
        }
    }

    function getOtpValue() {
        return Array.from(otpInputs).map(input => input.value).join('');
    }

    function shakeOtpInputs() {
        otpInputs.forEach(input => {
            input.classList.add('is-invalid');
            input.parentElement.classList.add('shake');
        });

        setTimeout(() => {
            otpInputs.forEach(input => {
                input.parentElement.classList.remove('shake');
            });
        }, 500);
    }

    function startResendCountdown() {
        resendCountdown = 30;
        resendBtn.disabled = true;

        resendInterval = setInterval(() => {
            resendCountdown--;
            resendTimer.textContent = `(${resendCountdown}s)`;

            if (resendCountdown <= 0) {
                clearInterval(resendInterval);
                resendBtn.disabled = false;
                resendTimer.textContent = '';
            }
        }, 1000);
    }

    function showLoadingState(button, isLoading) {
        const btnText = button.querySelector('.btn-text');
        const spinner = button.querySelector('.spinner-border');

        if (isLoading) {
            btnText.textContent = 'Verifying...';
            spinner.classList.remove('d-none');
            button.disabled = true;
        } else {
            btnText.textContent = 'Verify OTP';
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
