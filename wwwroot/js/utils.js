$(document).ready(() => {
    $.ajaxSetup({
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Authorization", "Bearer " + localStorage.getItem("token"));
        }
    });

    setInterval(() => {
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
                const currentPage =
                    location.href.split('/')[3]?.toLowerCase() ?? '';

                if (currentPage !== 'login' && currentPage !== 'user') {
                    localStorage.removeItem("token");
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

$("#logout").click(() => {
    localStorage.removeItem("token");
    location.href = "/login";
});