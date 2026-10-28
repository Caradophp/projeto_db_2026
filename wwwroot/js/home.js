$(document).ready(() => {
    loadDashboardData();
});

function loadDashboardData() {
    $.ajax({
        url: '/Home/GetRegisterNumbersPerTable',
        method: 'GET',
        dataType: 'json',

        success: (result) => {
            const data = Array.isArray(result) ? result : [];

            updateDashboardCards(data);
            renderRegisterTable(data);
        },

        error: () => {
            showDashboardError();
        }
    });
}

function updateDashboardCards(data) {
    const registers = {};

    data.forEach(item => {
        const tableName = String(item.name || '').toUpperCase();
        registers[tableName] = item.rowsNumber ?? 0;
    });

    setCardValue('stat-propriedade', registers['PROPRIEDADE'] ?? 0);
    setCardValue('stat-talhao', registers['TALHAO'] ?? 0);
    setCardValue('stat-cultura', registers['CULTURA'] ?? 0);
    setCardValue('stat-plantio', registers['PLANTIO'] ?? 0);
}

function setCardValue(elementId, value) {
    const element = document.getElementById(elementId);

    if (element) {
        element.textContent = value;
    }
}

function renderRegisterTable(data) {
    const hiddenTables = [
        'MIGRATIONS',
        'CODES_SENT',
        'USERS'
    ];

    const tableLabels = {
        'APLICACAO_INSUMO': 'Aplicações de insumos',
        'COLHEITA': 'Colheitas',
        'CULTURA': 'Culturas',
        'INSUMO': 'Insumos',
        'PLANTIO': 'Plantios',
        'PROPRIEDADE': 'Propriedades',
        'TALHAO': 'Talhões'
    };

    const visibleData = data.filter(item => {
        const tableName = String(item.name || '').toUpperCase();
        return !hiddenTables.includes(tableName);
    });

    if (visibleData.length === 0) {
        $('#tbody-tables-info').html(`
            <tr>
                <td colspan="2" class="dashboard-empty">
                    Nenhum registro encontrado.
                </td>
            </tr>
        `);

        return;
    }

    let html = '';

    visibleData.forEach(item => {
        const tableName = String(item.name || '').toUpperCase();
        const label = tableLabels[tableName] || tableName;
        const rowsNumber = item.rowsNumber ?? 0;

        html += `
            <tr>
                <td>${label}</td>
                <td>${rowsNumber}</td>
            </tr>
        `;
    });

    $('#tbody-tables-info').html(html);
}

function showDashboardError() {
    const cardIds = [
        'stat-propriedade',
        'stat-talhao',
        'stat-cultura',
        'stat-plantio'
    ];

    cardIds.forEach(id => {
        setCardValue(id, '--');
    });

    $('#tbody-tables-info').html(`
        <tr>
            <td colspan="2" class="dashboard-empty">
                Não foi possível carregar os dados do sistema.
            </td>
        </tr>
    `);
}