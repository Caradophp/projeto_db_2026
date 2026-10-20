$(document).ready(() => {
    const urlParams = new URLSearchParams(window.location.search);

    const showNewPasswordForm = urlParams.get('showNewPasswordForm');
    const userEmail = urlParams.get('email');

    if (showNewPasswordForm === 'true') {
        $('#email').prop('disabled', true);
        $('#email').val(userEmail);

        $('.p1').addClass('visually-hidden');
        $('.p2').removeClass('visually-hidden');

        $('#newPassForm').removeClass('visually-hidden');
        $('#newPassBtn').removeClass('visually-hidden');
        $('#enterBtn').addClass('visually-hidden');
    }

    $('#togglePassword').click(() => {
        const passwordInput = $('#password');
        const toggleButton = $('#togglePassword');
        const passwordIcon = $('#passwordIcon');

        const passwordIsHidden =
            passwordInput.attr('type') === 'password';

        if (passwordIsHidden) {
            passwordInput.attr('type', 'text');
            passwordIcon.text('○');

            toggleButton.attr('aria-label', 'Ocultar senha');
            toggleButton.attr('title', 'Ocultar senha');
        } else {
            passwordInput.attr('type', 'password');
            passwordIcon.text('◉');

            toggleButton.attr('aria-label', 'Mostrar senha');
            toggleButton.attr('title', 'Mostrar senha');
        }
    });

    $('#email, #password').on('input', function () {
        $(this).removeClass('input-error');

        if (
            $('#email').val().trim() !== '' &&
            $('#password').val() !== ''
        ) {
            $('.alert').addClass('visually-hidden');
        }
    });
});


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
            let message = 'Não foi possível realizar o login.';

            try {
                const responseJSON = JSON.parse(xhr.responseText);

                if (responseJSON.detail) {
                    message = responseJSON.detail;
                }
            } catch {
                // Mantém uma mensagem amigável caso a resposta não seja JSON.
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
            let responseJSON = JSON.parse(xhr.responseText);

            $('.alert').html(responseJSON.detail);
            $('.alert').removeClass('visually-hidden');
        }
    });
});


$('#sendEmailBtn').click(() => {
    let email = $('#email').val();

    if (email === '') {
        $('.alert').html('E-mail deve ser informado');
        $('.alert').removeClass('visually-hidden');
        return;
    }

    openLoader();

    $.ajax({
        url: '/Login/SendEmailForRetrivePassword',
        method: 'POST',
        dataType: 'json',

        data: {
            email
        },

        success: () => {
            let alert = $('.alert');

            $('#codeForm').removeClass('visually-hidden');
            $('.mainForm').addClass('visually-hidden');

            alert.removeClass('alert-danger visually-hidden');
            alert.addClass('alert-info');

            alert.html(
                'O código de recuperação de senha foi enviado para seu e-mail, informe o mesmo abaixo para continuar com a recuperação de senha.'
            );

            closeLoader();
        },

        error: (xhr) => {
            closeLoader();

            let responseJSON = JSON.parse(xhr.responseText);

            $('.alert').html(responseJSON.detail);
            $('.alert').removeClass('visually-hidden');
        }
    });
});


$('#checkCodeBtn').click(() => {
    let codigo = $('#codigo').val();
    let email = $('#email').val();

    if (codigo === '') {
        let alert = $('.alert');

        alert.removeClass('alert-info');
        alert.addClass('alert-danger');

        alert.html('Código deve ser informado');
        alert.removeClass('visually-hidden');

        return;
    }

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
            window.location.href =
                '/login?showNewPasswordForm=true&email=' +
                encodeURIComponent(email);

            closeLoader();
        },

        error: (xhr) => {
            let responseJSON = JSON.parse(xhr.responseText);

            $('.alert').addClass('alert-danger');
            $('.alert').html(responseJSON.detail);
            $('.alert').removeClass('visually-hidden');

            closeLoader();
        }
    });
});


$('#newPassBtn').click(() => {
    const email = $('#email');
    const password = $('#password');
    const confirmPassword = $('#confirmPassword');
    const alert = $('.alert');

    alert.removeClass('visually-hidden');

    if (email.val() === '') {
        alert.html('E-mail deve ser informado');

    } else if (password.val() === '') {
        alert.html('Senha deve ser informada');

    } else if (confirmPassword.val() === '') {
        alert.html('Confirmação da senha deve ser informada');

    } else if (password.val() !== confirmPassword.val()) {
        alert.html('As senhas informadas devem ser iguais');

    } else {
        alert.addClass('visually-hidden');

        openLoader();

        $.ajax({
            url: '/Login/ChangeUserPassword',
            method: 'PATCH',
            dataType: 'json',

            data: {
                email: email.val(),
                password: password.val(),
                confirmPassword: confirmPassword.val()
            },

            success: () => {
                alert.removeClass('alert-danger visually-hidden');
                alert.addClass('alert-info');

                alert.html(
                    'Senha alterada com sucesso! Estamos te redirecionando para a tela de login.'
                );

                closeLoader();

                setTimeout(() => {
                    window.location.href = '/Login';
                }, 3000);
            },

            error: (xhr) => {
                let responseJSON = JSON.parse(xhr.responseText);

                $('.alert').addClass('alert-danger');
                $('.alert').html(responseJSON.detail);
                $('.alert').removeClass('visually-hidden');

                closeLoader();
            }
        });
    }
});