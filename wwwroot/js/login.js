$('#enterBtn').click(() => {
    const email = $('#email').val();
    const password = $('#password').val();

    if (email === '') {
        $('.alert').html('E-mail deve ser informado');
        $('.alert').removeClass('visually-hidden');
        return;
    }

    if (password === '') {
        $('.alert').html('Senha deve ser informada');
        $('.alert').removeClass('visually-hidden');
        return;
    }

    $('.alert').addClass('visually-hidden');

    $.ajax({
        url: '/Login/CheckUserLogin',
        method: 'POST',
        dataType: 'json',
        data: {
            email,
            password 
        },
        success: () => {
            location.href = '/';
        },
        error: (xhr, status, error) => {
            let responseJSON = JSON.parse(xhr.responseText);
            $('.alert').html(responseJSON.detail);
            $('.alert').removeClass('visually-hidden');
        }
    })
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
        error: (xhr, status, error) => {
            let responseJSON = JSON.parse(xhr.responseText);
            $('.alert').html(responseJSON.detail);
            $('.alert').removeClass('visually-hidden');
        }
    });
});