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
    const tableInfo = {
        'APLICACAO_INSUMO': {
            label: 'Aplicações de insumos',
            description: 'Operações registradas',
            icon: 'fa-solid fa-tractor',
            className: 'register-blue'
        },

        'CODES_SENT': {
            label: 'Códigos enviados',
            description: 'Códigos de verificação enviados',
            icon: 'fa-solid fa-envelope-circle-check',
            className: 'register-green'
        },

        'COLHEITA': {
            label: 'Colheitas',
            description: 'Colheitas cadastradas',
            icon: 'fa-solid fa-wheat-awn',
            className: 'register-yellow'
        },

        'CULTURA': {
            label: 'Culturas',
            description: 'Culturas disponíveis',
            icon: 'fa-solid fa-leaf',
            className: 'register-green'
        },

        'INSUMO': {
            label: 'Insumos',
            description: 'Insumos cadastrados',
            icon: 'fa-solid fa-flask',
            className: 'register-blue'
        },

        'MIGRATIONS': {
            label: 'Migrações',
            description: 'Migrações executadas no banco de dados',
            icon: 'fa-solid fa-code-branch',
            className: 'register-yellow'
        },

        'PLANTIO': {
            label: 'Plantios',
            description: 'Plantios registrados',
            icon: 'fa-solid fa-seedling',
            className: 'register-green'
        },

        'PROPRIEDADE': {
            label: 'Propriedades',
            description: 'Propriedades cadastradas',
            icon: 'fa-solid fa-house-chimney',
            className: 'register-green'
        },

        'TALHAO': {
            label: 'Talhões',
            description: 'Talhões cadastrados',
            icon: 'fa-solid fa-table-cells-large',
            className: 'register-yellow'
        },

        'USERS': {
            label: 'Usuários',
            description: 'Usuários cadastrados no sistema',
            icon: 'fa-solid fa-users',
            className: 'register-blue'
        }
    };

    const visibleData = data;

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

        const info = tableInfo[tableName] || {
            label: tableName,
            description: 'Registros cadastrados',
            icon: 'fa-solid fa-database',
            className: 'register-green'
        };

        const rowsNumber = item.rowsNumber ?? 0;

        html += `
            <tr class="register-row">
                <td>
                    <div class="register-category">
                        <div class="register-icon ${info.className}">
                            <i class="${info.icon}"></i>
                        </div>

                        <div class="register-info">
                            <strong>${info.label}</strong>
                            <span>${info.description}</span>
                        </div>
                    </div>
                </td>

                <td class="register-count-cell">
                    <span class="register-count">
                        ${rowsNumber}
                    </span>
                </td>
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