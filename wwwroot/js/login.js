$(document).ready(() => {

    // =========================================================
    // LOGIN - MOSTRAR / OCULTAR SENHA
    // =========================================================

    $('#togglePassword').click(() => {
        const passwordInput = $('#password');
        const toggleButton = $('#togglePassword');
        const passwordIcon = $('#passwordIcon');

        const passwordIsHidden =
            passwordInput.attr('type') === 'password';

        if (passwordIsHidden) {
            passwordInput.attr('type', 'text');
            passwordIcon.text('◉');

            toggleButton.attr('aria-label', 'Ocultar senha');
            toggleButton.attr('title', 'Ocultar senha');
        } else {
            passwordInput.attr('type', 'password');
            passwordIcon.text('◎');

            toggleButton.attr('aria-label', 'Mostrar senha');
            toggleButton.attr('title', 'Mostrar senha');
        }
    });


    // =========================================================
    // LIMPAR ERROS DO LOGIN ENQUANTO DIGITA
    // =========================================================

    $('#email, #password').on('input', function () {
        $(this).removeClass('input-error');

        if (
            $('#email').val()?.trim() !== '' &&
            $('#password').val() !== ''
        ) {
            $('.alert').addClass('visually-hidden');
        }
    });


    // =========================================================
    // RECUPERAÇÃO - ACEITAR APENAS NÚMEROS NO CÓDIGO
    // =========================================================

    $('#codigo').on('input', function () {
        const codigo = $(this)
            .val()
            .replace(/\D/g, '')
            .slice(0, 6);

        $(this).val(codigo);
        $(this).removeClass('input-error');
    });


    // =========================================================
    // RECUPERAÇÃO - LIMPAR ERROS DAS NOVAS SENHAS
    // =========================================================

    $('#newPassword, #confirmNewPassword').on(
        'input',
        function () {
            $(this).removeClass('input-error');
        }
    );
});


// =============================================================
// LOGIN
// =============================================================

$('#enterBtn').click(() => {
    const emailInput = $('#email');
    const passwordInput = $('#password');
    const alert = $('.alert');
    const enterButton = $('#enterBtn');

    const email = emailInput.val().trim();
    const password = passwordInput.val();

    emailInput.removeClass('input-error');
    passwordInput.removeClass('input-error');

    if (email === '') {
        emailInput.addClass('input-error');

        alert
            .html('E-mail deve ser informado.')
            .removeClass('visually-hidden');

        emailInput.focus();
        return;
    }

    const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    if (!emailPattern.test(email)) {
        emailInput.addClass('input-error');

        alert
            .html('Informe um e-mail válido.')
            .removeClass('visually-hidden');

        emailInput.focus();
        return;
    }

    if (password === '') {
        passwordInput.addClass('input-error');

        alert
            .html('Senha deve ser informada.')
            .removeClass('visually-hidden');

        passwordInput.focus();
        return;
    }

    alert.addClass('visually-hidden');

    enterButton
        .prop('disabled', true)
        .text('Entrando...');

    $.ajax({
        url: '/Login/CheckUserLogin',
        method: 'POST',
        dataType: 'json',

        data: {
            email,
            password
        },

        success: (result) => {
            localStorage.setItem('token', result.token);
            location.href = '/';
        },

        error: (xhr) => {
            let message =
                'Não foi possível realizar o login.';

            try {
                const responseJSON =
                    JSON.parse(xhr.responseText);

                if (responseJSON.detail) {
                    message = responseJSON.detail;
                }
            } catch {
                // Mantém uma mensagem amigável.
            }

            alert
                .html(message)
                .removeClass('visually-hidden');
        },

        complete: () => {
            enterButton
                .prop('disabled', false)
                .text('Entrar');
        }
    });
});


// =============================================================
// CADASTRO
// =============================================================

$('#singupBtn').click(() => {
    $.ajax({
        url: '/User/Create',
        method: 'POST',
        dataType: 'json',

        data: {
            name: $('#name').val(),
            email: $('#email').val(),
            password: $('#password').val(),
            confirmPassword: $('#confirmPassword').val()
        },

        success: () => {
            $('.alert').addClass('visually-hidden');
            location.href = '/Login';
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

            $('.alert')
                .html(message)
                .removeClass('visually-hidden');
        }
    });
});


// =============================================================
// RECUPERAÇÃO - ETAPA 1
// ENVIAR CÓDIGO PARA O E-MAIL
// =============================================================

$('#sendEmailBtn').click(() => {
    const emailInput = $('#email');
    const email = emailInput.val().trim();
    const alert = $('.alert');
    const button = $('#sendEmailBtn');

    emailInput.removeClass('input-error');

    if (email === '') {
        emailInput.addClass('input-error');

        alert
            .removeClass('alert-info')
            .addClass('alert-danger')
            .html('E-mail deve ser informado.')
            .removeClass('visually-hidden');

        emailInput.focus();
        return;
    }

    const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    if (!emailPattern.test(email)) {
        emailInput.addClass('input-error');

        alert
            .removeClass('alert-info')
            .addClass('alert-danger')
            .html('Informe um e-mail válido.')
            .removeClass('visually-hidden');

        emailInput.focus();
        return;
    }

    alert.addClass('visually-hidden');

    button.prop('disabled', true);

    openLoader();

    $.ajax({
        url: '/Login/SendEmailForRetrivePassword',
        method: 'POST',
        dataType: 'json',

        data: {
            email
        },

        success: () => {
            $('.mainForm').addClass('visually-hidden');

            $('#codeForm')
                .removeClass('visually-hidden');

            alert
                .removeClass('alert-danger')
                .addClass('alert-info')
                .html(
                    'O código de recuperação foi enviado para o seu e-mail.'
                )
                .removeClass('visually-hidden');

            $('#codigo').focus();
        },

        error: (xhr) => {
            let message =
                'Não foi possível enviar o código de recuperação.';

            try {
                const responseJSON =
                    JSON.parse(xhr.responseText);

                if (responseJSON.detail) {
                    message = responseJSON.detail;
                }
            } catch {
                // Mantém a mensagem padrão.
            }

            alert
                .removeClass('alert-info')
                .addClass('alert-danger')
                .html(message)
                .removeClass('visually-hidden');
        },

        complete: () => {
            button.prop('disabled', false);
            closeLoader();
        }
    });
});


// =============================================================
// RECUPERAÇÃO - ETAPA 2
// VALIDAR CÓDIGO
// =============================================================

$('#checkCodeBtn').click(() => {
    const codigoInput = $('#codigo');
    const codigo = codigoInput.val().trim();
    const email = $('#email').val().trim();

    const alert = $('.alert');
    const button = $('#checkCodeBtn');

    codigoInput.removeClass('input-error');

    if (codigo === '') {
        codigoInput.addClass('input-error');

        alert
            .removeClass('alert-info')
            .addClass('alert-danger')
            .html('Código deve ser informado.')
            .removeClass('visually-hidden');

        codigoInput.focus();
        return;
    }

    if (!/^\d{6}$/.test(codigo)) {
        codigoInput.addClass('input-error');

        alert
            .removeClass('alert-info')
            .addClass('alert-danger')
            .html('Informe o código de 6 dígitos.')
            .removeClass('visually-hidden');

        codigoInput.focus();
        return;
    }

    alert.addClass('visually-hidden');

    button.prop('disabled', true);

    openLoader();

    $.ajax({
        url: '/Login/CheckCode',
        method: 'POST',
        dataType: 'json',

        data: {
            codigo,
            email
        },

        success: () => {

            // Não redirecionamos mais para /login.
            // Apenas avançamos para a etapa 3.
            $('#codeForm')
                .addClass('visually-hidden');

            $('#newPasswordForm')
                .removeClass('visually-hidden');

            alert
                .removeClass('alert-danger alert-info')
                .addClass('visually-hidden')
                .html('');

            $('#newPassword').focus();
        },

        error: (xhr) => {
            let message =
                'Código inválido ou expirado.';

            try {
                const responseJSON =
                    JSON.parse(xhr.responseText);

                if (responseJSON.detail) {
                    message = responseJSON.detail;
                }
            } catch {
                // Mantém a mensagem padrão.
            }

            codigoInput.addClass('input-error');

            alert
                .removeClass('alert-info')
                .addClass('alert-danger')
                .html(message)
                .removeClass('visually-hidden');
        },

        complete: () => {
            button.prop('disabled', false);
            closeLoader();
        }
    });
});


// =============================================================
// RECUPERAÇÃO - ETAPA 3
// ALTERAR SENHA
// =============================================================

$('#changePasswordBtn').click(() => {
    const email = $('#email').val().trim();

    // O código continua no campo da etapa anterior,
    // mesmo estando visualmente oculto.
    const codigo = $('#codigo').val().trim();

    const passwordInput = $('#newPassword');
    const confirmPasswordInput =
        $('#confirmNewPassword');

    const password = passwordInput.val();
    const confirmPassword =
        confirmPasswordInput.val();

    const alert = $('.alert');
    const button = $('#changePasswordBtn');

    passwordInput.removeClass('input-error');
    confirmPasswordInput.removeClass('input-error');

    if (password === '') {
        passwordInput.addClass('input-error');

        alert
            .removeClass('alert-info')
            .addClass('alert-danger')
            .html('Nova senha deve ser informada.')
            .removeClass('visually-hidden');

        passwordInput.focus();
        return;
    }

    if (confirmPassword === '') {
        confirmPasswordInput.addClass('input-error');

        alert
            .removeClass('alert-info')
            .addClass('alert-danger')
            .html(
                'Confirmação da nova senha deve ser informada.'
            )
            .removeClass('visually-hidden');

        confirmPasswordInput.focus();
        return;
    }

    if (password !== confirmPassword) {
        passwordInput.addClass('input-error');
        confirmPasswordInput.addClass('input-error');

        alert
            .removeClass('alert-info')
            .addClass('alert-danger')
            .html('As senhas informadas devem ser iguais.')
            .removeClass('visually-hidden');

        confirmPasswordInput.focus();
        return;
    }

    if (codigo === '') {
        alert
            .removeClass('alert-info')
            .addClass('alert-danger')
            .html(
                'Não foi possível validar a recuperação. Solicite um novo código.'
            )
            .removeClass('visually-hidden');

        return;
    }

    alert.addClass('visually-hidden');

    button.prop('disabled', true);

    openLoader();

    $.ajax({
        url: '/Login/ChangeUserPassword',
        method: 'PATCH',
        dataType: 'json',

        data: {
            email,
            password,
            confirmPassword,
            codigo
        },

        success: () => {
            alert
                .removeClass(
                    'alert-danger visually-hidden'
                )
                .addClass('alert-info')
                .html(
                    'Senha alterada com sucesso! Você será redirecionado para o login.'
                );

            passwordInput.val('');
            confirmPasswordInput.val('');
            $('#codigo').val('');

            setTimeout(() => {
                window.location.href = '/Login';
            }, 2500);
        },

        error: (xhr) => {
            let message =
                'Não foi possível alterar sua senha.';

            try {
                const responseJSON =
                    JSON.parse(xhr.responseText);

                if (responseJSON.detail) {
                    message = responseJSON.detail;
                }
            } catch {
                // Mantém a mensagem padrão.
            }

            alert
                .removeClass('alert-info')
                .addClass('alert-danger')
                .html(message)
                .removeClass('visually-hidden');

            button.prop('disabled', false);
        },

        complete: () => {
            closeLoader();
        }
    });
});