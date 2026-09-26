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