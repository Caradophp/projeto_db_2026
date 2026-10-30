let colheitas = [];
let plantios = [];

document.addEventListener('DOMContentLoaded', async () => {
    configurarEventos();

    await carregarPlantios();
    await carregarColheitas();
});

function getToken() {
    return localStorage.getItem('token');
}

function authHeaders(json = false) {
    const headers = {
        'Authorization': 'Bearer ' + getToken()
    };

    if (json) {
        headers['Content-Type'] = 'application/json';
    }

    return headers;
}

function configurarEventos() {
    document
        .getElementById('btnNovaColheita')
        .addEventListener('click', abrirNovaColheita);

    document
        .getElementById('btnCloseModal')
        .addEventListener('click', fecharModal);

    document
        .getElementById('btnCancelModal')
        .addEventListener('click', fecharModal);

    document
        .getElementById('colheitaForm')
        .addEventListener('submit', salvarColheita);

    document
        .getElementById('searchColheita')
        .addEventListener('input', aplicarFiltro);

    document
        .getElementById('colheitaModal')
        .addEventListener('click', event => {
            if (event.target.id === 'colheitaModal') {
                fecharModal();
            }
        });
}

async function carregarPlantios() {
    try {
        const response = await fetch('/Plantio/GetList', {
            headers: authHeaders()
        });

        if (!response.ok) {
            throw new Error('Não foi possível carregar os plantios.');
        }

        plantios = await response.json();

        preencherSelectPlantios();
    } catch (error) {
        console.error('Erro ao carregar plantios:', error);
    }
}

async function carregarColheitas() {
    try {
        const response = await fetch('/Colheita/GetList', {
            headers: authHeaders()
        });

        if (!response.ok) {
            throw new Error('Não foi possível carregar as colheitas.');
        }

        colheitas = await response.json();

        renderizarColheitas(colheitas);
        atualizarIndicadores();
    } catch (error) {
        console.error('Erro ao carregar colheitas:', error);
        renderizarColheitas([]);
    }
}

function preencherSelectPlantios() {
    const select = document.getElementById('plantioInput');

    select.innerHTML =
        '<option value="">Selecione o plantio</option>';

    plantios.forEach(plantio => {
        const option = document.createElement('option');

        option.value = plantio.id;

        let descricao = `Plantio #${plantio.id}`;

        if (plantio.talhaoId && plantio.culturaId) {
            descricao +=
                ` • Talhão ${plantio.talhaoId} • Cultura ${plantio.culturaId}`;
        }

        option.textContent = descricao;

        select.appendChild(option);
    });
}

function renderizarColheitas(lista) {
    const tbody =
        document.getElementById('colheitaTableBody');

    const empty =
        document.getElementById('emptyState');

    tbody.innerHTML = '';

    if (!lista || lista.length === 0) {
        empty.classList.add('show');
        return;
    }

    empty.classList.remove('show');

    lista.forEach(colheita => {
        const tr = document.createElement('tr');

        const observacoes =
            colheita.observacoes?.trim()
                ? escapeHtml(colheita.observacoes)
                : '—';

        tr.innerHTML = `
            <td>#${colheita.id}</td>

            <td>
                <span class="plantio-badge">
                    <i class="fa-solid fa-seedling"></i>
                    Plantio #${colheita.plantioId}
                </span>
            </td>

            <td>
                ${formatarData(colheita.dataColheita)}
            </td>

            <td>
                <span class="production-value">
                    ${formatarNumero(colheita.producaoTotal)}
                </span>
            </td>

            <td>
                <span
                    class="observation-text"
                    title="${observacoes}">
                    ${observacoes}
                </span>
            </td>

            <td>
                <div class="action-buttons">

                    <button
                        type="button"
                        class="action-btn action-edit"
                        title="Editar"
                        onclick="editarColheita(${colheita.id})">
                        <i class="fa-solid fa-pen"></i>
                    </button>

                    <button
                        type="button"
                        class="action-btn action-delete"
                        title="Excluir"
                        onclick="excluirColheita(${colheita.id})">
                        <i class="fa-solid fa-trash"></i>
                    </button>

                </div>
            </td>
        `;

        tbody.appendChild(tr);
    });
}

function atualizarIndicadores() {
    document.getElementById('totalColheitas').textContent =
        colheitas.length;

    const producao =
        colheitas.reduce(
            (total, item) =>
                total + Number(item.producaoTotal || 0),
            0
        );

    document.getElementById('producaoTotal').textContent =
        formatarNumero(producao);

    const plantiosUnicos =
        new Set(
            colheitas.map(item => item.plantioId)
        );

    document.getElementById('plantiosColhidos').textContent =
        plantiosUnicos.size;

    if (colheitas.length === 0) {
        document.getElementById('ultimaColheita').textContent =
            '—';

        return;
    }

    const datas =
        colheitas
            .map(item => new Date(item.dataColheita))
            .filter(data => !Number.isNaN(data.getTime()))
            .sort((a, b) => b - a);

    document.getElementById('ultimaColheita').textContent =
        datas.length > 0
            ? datas[0].toLocaleDateString('pt-BR')
            : '—';
}

function abrirNovaColheita() {
    document.getElementById('colheitaForm').reset();
    document.getElementById('colheitaId').value = '';
    document.getElementById('modalTitle').textContent =
        'Nova colheita';

    document
        .getElementById('colheitaModal')
        .classList.add('show');
}

function editarColheita(id) {
    const colheita =
        colheitas.find(item => item.id === id);

    if (!colheita) {
        return;
    }

    document.getElementById('colheitaId').value =
        colheita.id;

    document.getElementById('plantioInput').value =
        colheita.plantioId;

    document.getElementById('dataColheitaInput').value =
        obterDataInput(colheita.dataColheita);

    document.getElementById('producaoTotalInput').value =
        colheita.producaoTotal;

    document.getElementById('observacoesInput').value =
        colheita.observacoes || '';

    document.getElementById('modalTitle').textContent =
        'Editar colheita';

    document
        .getElementById('colheitaModal')
        .classList.add('show');
}

function fecharModal() {
    document
        .getElementById('colheitaModal')
        .classList.remove('show');
}

async function salvarColheita(event) {
    event.preventDefault();

    const id =
        Number(document.getElementById('colheitaId').value || 0);

    const payload = {
        id: id,
        plantioId:
            Number(document.getElementById('plantioInput').value),

        dataColheita:
            document.getElementById('dataColheitaInput').value,

        producaoTotal:
            Number(
                document.getElementById('producaoTotalInput').value
            ),

        observacoes:
            document.getElementById('observacoesInput').value.trim()
    };

    const endpoint =
        id > 0
            ? '/Colheita/Update'
            : '/Colheita/Create';

    try {
        const response = await fetch(endpoint, {
            method: 'POST',
            headers: authHeaders(true),
            body: JSON.stringify(payload)
        });

        const result = await response.json();

        if (!result.success) {
            await CultivaAlert.error(
    result.message || 'Não foi possível salvar a colheita.',
    'Erro ao salvar'
);
            return;
        }

        fecharModal();
        await carregarColheitas();

    } catch (error) {
        console.error(error);
        await CultivaAlert.error(
    'Erro ao salvar a colheita.',
    'Erro ao salvar'
);
    }
}

async function excluirColheita(id) {
   const confirmar =
    await CultivaAlert.delete(
        'Excluir colheita?',
        'Deseja realmente excluir esta colheita?'
    );
    if (!confirmar) {
        return;
    }

    try {
        const response =
            await fetch(`/Colheita/Delete?id=${id}`, {
                method: 'POST',
                headers: authHeaders()
            });

        const result = await response.json();

        if (!result.success) {
           await CultivaAlert.error(
    result.message || 'Não foi possível excluir a colheita.',
    'Erro ao excluir'
);
            return;
        }

        await carregarColheitas();

    } catch (error) {
        console.error(error);
       await CultivaAlert.error(
    'Erro ao excluir a colheita.',
    'Erro ao excluir'
);
    }
}

function aplicarFiltro() {
    const termo =
        document
            .getElementById('searchColheita')
            .value
            .trim()
            .toLowerCase();

    if (!termo) {
        renderizarColheitas(colheitas);
        return;
    }

    const filtradas =
        colheitas.filter(item => {
            const texto = [
                item.id,
                item.plantioId,
                formatarData(item.dataColheita),
                item.producaoTotal,
                item.observacoes
            ]
                .join(' ')
                .toLowerCase();

            return texto.includes(termo);
        });

    renderizarColheitas(filtradas);
}

function formatarData(valor) {
    if (!valor) {
        return '—';
    }

    const data = new Date(valor);

    if (Number.isNaN(data.getTime())) {
        return '—';
    }

    return data.toLocaleDateString('pt-BR', {
        timeZone: 'UTC'
    });
}

function obterDataInput(valor) {
    if (!valor) {
        return '';
    }

    return valor.substring(0, 10);
}

function formatarNumero(valor) {
    return Number(valor || 0).toLocaleString('pt-BR', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    });
}

function escapeHtml(valor) {
    const div = document.createElement('div');
    div.textContent = valor ?? '';
    return div.innerHTML;
}