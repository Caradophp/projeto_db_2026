$(document).ready(() => {
    const nameInput = $('#name');
    const emailInput = $('#email');
    const passwordInput = $('#password');
    const confirmPasswordInput = $('#confirmPassword');

    const signupButton = $('#singupBtn');
    const alert = $('.alert');

    const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    function showError(message, input = null) {
        alert
            .removeClass('alert-info visually-hidden')
            .addClass('alert-danger')
            .html(message);

        if (input) {
            input.addClass('input-error');
            input.focus();
        }
    }

    function clearAlert() {
        alert
            .removeClass('alert-info alert-danger')
            .addClass('visually-hidden')
            .html('');
    }

    function togglePasswordVisibility(input, button, icon) {
        const isHidden = input.attr('type') === 'password';

        input.attr('type', isHidden ? 'text' : 'password');

        button.attr(
            'aria-label',
            isHidden ? 'Ocultar senha' : 'Mostrar senha'
        );

        button.attr(
            'title',
            isHidden ? 'Ocultar senha' : 'Mostrar senha'
        );

        icon.text(isHidden ? '◉' : '◈');
    }

    $('#togglePassword').click(() => {
        togglePasswordVisibility(
            passwordInput,
            $('#togglePassword'),
            $('#passwordIcon')
        );
    });

    $('#toggleConfirmPassword').click(() => {
        togglePasswordVisibility(
            confirmPasswordInput,
            $('#toggleConfirmPassword'),
            $('#confirmPasswordIcon')
        );
    });

    nameInput.on('input', function () {
        $(this).removeClass('input-error');
    });

    emailInput.on('input', function () {
        $(this).removeClass('input-error');
    });

    passwordInput.on('input', function () {
        $(this).removeClass('input-error');
        confirmPasswordInput.removeClass('input-error');
    });

    confirmPasswordInput.on('input', function () {
        $(this).removeClass('input-error');
        passwordInput.removeClass('input-error');
    });

    $('#name, #email, #password, #confirmPassword').on(
        'input',
        function () {
            clearAlert();
        }
    );

    signupButton.click(() => {
        const name = nameInput.val().trim();
        const email = emailInput.val().trim();
        const password = passwordInput.val();
        const confirmPassword = confirmPasswordInput.val();

        nameInput.removeClass('input-error');
        emailInput.removeClass('input-error');
        passwordInput.removeClass('input-error');
        confirmPasswordInput.removeClass('input-error');

        clearAlert();

        if (name === '') {
            showError('Informe seu nome.', nameInput);
            return;
        }

        if (name.length < 2) {
            showError(
                'Informe um nome válido.',
                nameInput
            );
            return;
        }

        if (email === '') {
            showError(
                'Informe seu e-mail.',
                emailInput
            );
            return;
        }

        if (!emailPattern.test(email)) {
            showError(
                'Informe um e-mail válido.',
                emailInput
            );
            return;
        }

        if (password === '') {
            showError(
                'Informe sua senha.',
                passwordInput
            );
            return;
        }

        if (password.length < 6) {
            showError(
                'A senha deve ter pelo menos 6 caracteres.',
                passwordInput
            );
            return;
        }

        if (confirmPassword === '') {
            showError(
                'Confirme sua senha.',
                confirmPasswordInput
            );
            return;
        }

        if (password !== confirmPassword) {
            passwordInput.addClass('input-error');
            confirmPasswordInput.addClass('input-error');

            showError(
                'As senhas informadas devem ser iguais.',
                confirmPasswordInput
            );

            return;
        }

        signupButton
            .prop('disabled', true)
            .text('Criando conta...');

        openLoader();

        $.ajax({
            url: '/User/Create',
            method: 'POST',
            dataType: 'json',

            data: {
                name,
                email,
                password,
                confirmPassword
            },

            success: () => {
                alert
                    .removeClass(
                        'alert-danger visually-hidden'
                    )
                    .addClass('alert-info')
                    .html(
                        'Conta criada com sucesso! Redirecionando para o login...'
                    );

                nameInput.val('');
                emailInput.val('');
                passwordInput.val('');
                confirmPasswordInput.val('');

                setTimeout(() => {
                    window.location.href = '/Login';
                }, 2000);
            },

            error: (xhr) => {
                let message =
                    'Não foi possível realizar o cadastro.';

                try {
                    const responseJSON =
                        JSON.parse(xhr.responseText);

                    if (responseJSON.detail) {
                        message = responseJSON.detail;
                    }
                } catch {
                    // Mantém a mensagem padrão.
                }

                showError(message);

                signupButton
                    .prop('disabled', false)
                    .text('Criar minha conta');
            },

            complete: () => {
                closeLoader();
            }
        });
    });
});