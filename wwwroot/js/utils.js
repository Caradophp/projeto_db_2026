$(document).ready(() => {
    $.ajaxSetup({
        beforeSend: function (xhr) {
            xhr.setRequestHeader("X-Api-Key", localStorage.getItem("token"));
        }
    });

    setInterval(()=> {
        $.ajax({
            url: '/Home/CheckToken',
            method: 'GET',
            dataType: 'json',
            data: {
                token: localStorage.getItem("token")
            },
            success: () => {
                console.log("Sessão ativa e operante");
            },
            error: () => {
                if (location.href.split('/')[3] != 'login' && location.href.split('/')[3] != 'user') {
                    location.href = '/login';
                }
            }
        });
    }, 1000)
});

function showDialog(html) {
    $('.content').html(html);
    $('.dialog').modal('show');
}

function openLoader() {
    $('#loader').removeClass('visually-hidden');
}

function closeLoader() {
    $('#loader').addClass('visually-hidden');
}