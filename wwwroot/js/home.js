$(document).ready(() => {
    $.ajax({
        url: '/Home/GetRegisterNumbersPerTable',
        method: 'GET',
        dataType: 'json',
        success: (result) => {
            const json = JSON.parse(JSON.stringify(result));

            let html = ''
            json.forEach(element => {
                html += '<tr>';
                html += '   <td>' + element.name + '</td>';
                html += '   <td>' + element.rowsNumber + '</td>';
                html += '</tr>';
            });

            
            $('#tbody-tables-info').html(html);
            

        },
        error: () => {  
            showDialog('Erro ao buscar quantidade de dados por tabela');
        }
    });
});