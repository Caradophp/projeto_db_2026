document.addEventListener('DOMContentLoaded', () => {
    const token = localStorage.getItem('token');

    const tableBody = document.getElementById('aplicacaoTableBody');
    const searchInput = document.getElementById('aplicacaoSearch');
    const resultsText = document.getElementById('aplicacaoResultsText');
    const btnNovaAplicacao = document.getElementById('btnNovaAplicacao');

    const form = document.getElementById('aplicacaoForm');
    const idInput = document.getElementById('aplicacaoId');
    const talhaoSelect = document.getElementById('aplicacaoTalhao');
    const insumoSelect = document.getElementById('aplicacaoInsumo');
    const dataInput = document.getElementById('aplicacaoData');
  const quantidadeInput = document.getElementById('aplicacaoQuantidadeInput');
    const observacoesInput = document.getElementById('aplicacaoObservacoes');

    const modalElement = document.getElementById('aplicacaoModal');
    const modalLabel = document.getElementById('aplicacaoModalLabel');
    const modal = bootstrap.Modal.getOrCreateInstance(modalElement);

    let aplicacoes = [];
    let talhoes = [];
    let insumos = [];

    function getAuthHeaders(includeJson = false) {
        const headers = {};

        if (token) {
            headers.Authorization = `Bearer ${token}`;
        }

        if (includeJson) {
            headers['Content-Type'] = 'application/json';
        }

        return headers;
    }

    function escapeHtml(value) {
        if (value === null || value === undefined) {
            return '';
        }

        return String(value)
            .replaceAll('&', '&amp;')
            .replaceAll('<', '&lt;')
            .replaceAll('>', '&gt;')
            .replaceAll('"', '&quot;')
            .replaceAll("'", '&#039;');
    }

    function formatDate(value) {
        if (!value) {
            return '-';
        }

        const datePart = String(value).split('T')[0];
        const parts = datePart.split('-');

        if (parts.length !== 3) {
            return value;
        }

        return `${parts[2]}/${parts[1]}/${parts[0]}`;
    }

    function toInputDate(value) {
        if (!value) {
            return '';
        }

        return String(value).split('T')[0];
    }

    function formatNumber(value) {
        const number = Number(value);

        if (!Number.isFinite(number)) {
            return '0,00';
        }

        return number.toLocaleString('pt-BR', {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2
        });
    }

    function getTalhaoNome(id) {
        const talhao = talhoes.find(item => Number(item.id) === Number(id));
        return talhao?.nome ?? `Talhão #${id}`;
    }

    function getInsumoNome(id) {
        const insumo = insumos.find(item => Number(item.id) === Number(id));
        return insumo?.nome ?? `Insumo #${id}`;
    }

    async function loadReferences() {
        const [talhaoResponse, insumoResponse] = await Promise.all([
            fetch('/Talhao/GetList', {
                headers: getAuthHeaders()
            }),
            fetch('/Insumo/GetList', {
                headers: getAuthHeaders()
            })
        ]);

        if (!talhaoResponse.ok || !insumoResponse.ok) {
            throw new Error('Não foi possível carregar talhões e insumos.');
        }

        talhoes = await talhaoResponse.json();
        insumos = await insumoResponse.json();

        fillReferenceSelects();
    }

    function fillReferenceSelects() {
        talhaoSelect.innerHTML =
            '<option value="">Selecione um talhão...</option>';

        talhoes.forEach(talhao => {
            const option = document.createElement('option');
            option.value = talhao.id;
            option.textContent = talhao.nome ?? `Talhão #${talhao.id}`;
            talhaoSelect.appendChild(option);
        });

        insumoSelect.innerHTML =
            '<option value="">Selecione um insumo...</option>';

        insumos.forEach(insumo => {
            const option = document.createElement('option');
            option.value = insumo.id;
            option.textContent = insumo.nome ?? `Insumo #${insumo.id}`;
            insumoSelect.appendChild(option);
        });
    }

    async function loadAplicacoes() {
        showLoading();

        try {
            const response = await fetch('/AplicacaoInsumo/GetList', {
                headers: getAuthHeaders()
            });

            if (!response.ok) {
                throw new Error('Não foi possível carregar as aplicações.');
            }

            aplicacoes = await response.json();

            updateIndicators();
            renderAplicacoes(aplicacoes);
        } catch (error) {
            console.error(error);
            showError('Não foi possível carregar as aplicações.');
        }
    }

    function updateIndicators() {
        const total = aplicacoes.length;

        const talhoesUtilizados = new Set(
            aplicacoes.map(item => Number(item.talhaoId))
        ).size;

        const insumosUtilizados = new Set(
            aplicacoes.map(item => Number(item.insumoId))
        ).size;

        const quantidadeTotal = aplicacoes.reduce(
            (totalAtual, item) =>
                totalAtual + (Number(item.quantidade) || 0),
            0
        );

        document.getElementById('aplicacaoTotal').textContent = total;
        document.getElementById('aplicacaoTalhoes').textContent = talhoesUtilizados;
        document.getElementById('aplicacaoQuantidade').textContent =
            formatNumber(quantidadeTotal);
        document.getElementById('aplicacaoInsumos').textContent =
            insumosUtilizados;
    }

    function renderAplicacoes(lista) {
        if (!lista.length) {
            showEmpty();
            updateResultsText(0);
            return;
        }

        tableBody.innerHTML = lista.map(aplicacao => `
            <tr>
                <td>
                    <span class="aplicacao-id">
                        #${escapeHtml(aplicacao.id)}
                    </span>
                </td>

                <td>
                    <strong>
                        ${escapeHtml(getTalhaoNome(aplicacao.talhaoId))}
                    </strong>
                </td>

                <td>
                    ${escapeHtml(getInsumoNome(aplicacao.insumoId))}
                </td>

                <td>
                    ${escapeHtml(formatDate(aplicacao.dataAplicacao))}
                </td>

                <td>
                    <strong>
                        ${escapeHtml(formatNumber(aplicacao.quantidade))}
                    </strong>
                </td>

                <td>
                    ${escapeHtml(aplicacao.observacoes || '-')}
                </td>

                <td>
                    <div class="aplicacao-actions">

                        <button
                            type="button"
                            class="aplicacao-action-button edit"
                            data-action="edit"
                            data-id="${aplicacao.id}"
                            title="Editar aplicação">
                            <i class="fa-solid fa-pen"></i>
                        </button>

                        <button
                            type="button"
                            class="aplicacao-action-button delete"
                            data-action="delete"
                            data-id="${aplicacao.id}"
                            title="Excluir aplicação">
                            <i class="fa-solid fa-trash"></i>
                        </button>

                    </div>
                </td>
            </tr>
        `).join('');

        updateResultsText(lista.length);
    }

    function showLoading() {
        tableBody.innerHTML = `
            <tr>
                <td colspan="7">
                    <div class="aplicacao-loading">
                        <span class="aplicacao-loading-icon">
                            <i class="fa-solid fa-droplet"></i>
                        </span>
                        <span>Carregando aplicações...</span>
                    </div>
                </td>
            </tr>
        `;
    }

    function showEmpty() {
        tableBody.innerHTML = `
            <tr>
                <td colspan="7">
                    <div class="aplicacao-empty">
                        <span class="aplicacao-empty-icon">
                            <i class="fa-solid fa-droplet"></i>
                        </span>

                        <strong>Nenhuma aplicação encontrada</strong>
                        <span>
                            Registre uma aplicação de insumo para começar.
                        </span>
                    </div>
                </td>
            </tr>
        `;
    }

    function showError(message) {
        tableBody.innerHTML = `
            <tr>
                <td colspan="7">
                    <div class="aplicacao-empty">
                        <span class="aplicacao-empty-icon">
                            <i class="fa-solid fa-triangle-exclamation"></i>
                        </span>

                        <strong>Não foi possível carregar os dados</strong>
                        <span>${escapeHtml(message)}</span>
                    </div>
                </td>
            </tr>
        `;

        resultsText.textContent = 'Erro ao carregar aplicações';
    }

    function updateResultsText(count) {
        if (count === 0) {
            resultsText.textContent = 'Nenhuma aplicação registrada';
            return;
        }

        if (count === 1) {
            resultsText.textContent = '1 aplicação encontrada';
            return;
        }

        resultsText.textContent = `${count} aplicações encontradas`;
    }

    function filterAplicacoes() {
        const term = searchInput.value
            .trim()
            .toLocaleLowerCase('pt-BR');

        if (!term) {
            renderAplicacoes(aplicacoes);
            return;
        }

        const filtered = aplicacoes.filter(aplicacao => {
            const searchable = [
                aplicacao.id,
                getTalhaoNome(aplicacao.talhaoId),
                getInsumoNome(aplicacao.insumoId),
                aplicacao.observacoes,
                formatDate(aplicacao.dataAplicacao),
                aplicacao.quantidade
            ]
                .filter(value => value !== null && value !== undefined)
                .join(' ')
                .toLocaleLowerCase('pt-BR');

            return searchable.includes(term);
        });

        renderAplicacoes(filtered);
    }

    function openCreateModal() {
        form.reset();

        idInput.value = '0';
        modalLabel.textContent = 'Nova aplicação';

        const today = new Date();
        const year = today.getFullYear();
        const month = String(today.getMonth() + 1).padStart(2, '0');
        const day = String(today.getDate()).padStart(2, '0');

        dataInput.value = `${year}-${month}-${day}`;

        modal.show();
    }

    function openEditModal(id) {
        const aplicacao = aplicacoes.find(
            item => Number(item.id) === Number(id)
        );

        if (!aplicacao) {
            alert('Aplicação não encontrada.');
            return;
        }

        form.reset();

        idInput.value = aplicacao.id;
        talhaoSelect.value = aplicacao.talhaoId;
        insumoSelect.value = aplicacao.insumoId;
        dataInput.value = toInputDate(aplicacao.dataAplicacao);
        quantidadeInput.value = aplicacao.quantidade;
        observacoesInput.value = aplicacao.observacoes ?? '';

        modalLabel.textContent = 'Editar aplicação';

        modal.show();
    }

    async function saveAplicacao(event) {
        event.preventDefault();

        const id = Number(idInput.value || 0);
        const talhaoId = Number(talhaoSelect.value);
        const insumoId = Number(insumoSelect.value);
        const quantidadeField = document.getElementById('aplicacaoQuantidadeInput');

if (!quantidadeField) {
    console.error('Campo aplicacaoQuantidadeInput não encontrado.');
    alert('Não foi possível localizar o campo de quantidade.');
    return;
}

const quantidade = Number(quantidadeField.value);

        if (!talhaoId) {
            alert('Selecione um talhão.');
            talhaoSelect.focus();
            return;
        }

        if (!insumoId) {
            alert('Selecione um insumo.');
            insumoSelect.focus();
            return;
        }

        if (!dataInput.value) {
            alert('Informe a data da aplicação.');
            dataInput.focus();
            return;
        }

        if (!Number.isFinite(quantidade) || quantidade <= 0) {
            alert('A quantidade deve ser maior que zero.');
            quantidadeField.focus();
            return;
        }

        const payload = {
            id,
            talhaoId,
            insumoId,
            dataAplicacao: dataInput.value,
            quantidade,
            observacoes: observacoesInput.value.trim() || null
        };

        const endpoint = id > 0
            ? '/AplicacaoInsumo/Update'
            : '/AplicacaoInsumo/Create';

        try {
            const response = await fetch(endpoint, {
                method: 'POST',
                headers: getAuthHeaders(true),
                body: JSON.stringify(payload)
            });

            if (!response.ok) {
                throw new Error('Erro ao salvar a aplicação.');
            }

            const result = await response.json();

            if (!result.success) {
                alert(result.message || 'Não foi possível salvar a aplicação.');
                return;
            }

            modal.hide();

            await loadAplicacoes();
        } catch (error) {
            console.error(error);
            alert('Não foi possível salvar a aplicação.');
        }
    }

    async function deleteAplicacao(id) {
        const aplicacao = aplicacoes.find(
            item => Number(item.id) === Number(id)
        );

        if (!aplicacao) {
            alert('Aplicação não encontrada.');
            return;
        }

        const confirmed = confirm(
            `Deseja realmente excluir a aplicação do insumo "${getInsumoNome(aplicacao.insumoId)}"?`
        );

        if (!confirmed) {
            return;
        }

        try {
            const response = await fetch(
                `/AplicacaoInsumo/Delete?id=${encodeURIComponent(id)}`,
                {
                    method: 'POST',
                    headers: getAuthHeaders()
                }
            );

            if (!response.ok) {
                throw new Error('Erro ao excluir a aplicação.');
            }

            const result = await response.json();

            if (!result.success) {
                alert(result.message || 'Não foi possível excluir a aplicação.');
                return;
            }

            await loadAplicacoes();
        } catch (error) {
            console.error(error);
            alert('Não foi possível excluir a aplicação.');
        }
    }

    btnNovaAplicacao.addEventListener('click', openCreateModal);
    searchInput.addEventListener('input', filterAplicacoes);
    form.addEventListener('submit', saveAplicacao);

    tableBody.addEventListener('click', event => {
        const button = event.target.closest('[data-action]');

        if (!button) {
            return;
        }

        const id = Number(button.dataset.id);

        if (button.dataset.action === 'edit') {
            openEditModal(id);
            return;
        }

        if (button.dataset.action === 'delete') {
            deleteAplicacao(id);
        }
    });

    async function initialize() {
        showLoading();

        try {
            await loadReferences();
            await loadAplicacoes();
        } catch (error) {
            console.error(error);
            showError(error.message);
        }
    }

    initialize();
});