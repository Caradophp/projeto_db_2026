document.addEventListener('DOMContentLoaded', function () {
    const modalElement = document.getElementById('plantioModal');
    const plantioModal = new bootstrap.Modal(modalElement);

    const form = document.getElementById('plantioForm');
    const tableBody = document.getElementById('plantioTableBody');
    const searchInput = document.getElementById('plantioSearch');
    const btnNovo = document.getElementById('btnNovoPlantio');

    const talhaoSelect = document.getElementById('plantioTalhao');
    const culturaSelect = document.getElementById('plantioCultura');

    const totalElement = document.getElementById('plantioTotal');
    const andamentoElement = document.getElementById('plantioAndamento');
    const produtividadeElement = document.getElementById('plantioProdutividade');
    const culturasElement = document.getElementById('plantioCulturas');
    const resultsText = document.getElementById('plantioResultsText');

    let plantios = [];
    let talhoes = [];
    let culturas = [];

    function getAuthHeaders(includeContentType = false) {
        const token = localStorage.getItem('token');

        const headers = {
            'Accept': 'application/json'
        };

        if (token) {
            headers['Authorization'] = `Bearer ${token}`;
        }

        if (includeContentType) {
            headers['Content-Type'] = 'application/json';
        }

        return headers;
    }

    function escapeHtml(value) {
        const div = document.createElement('div');
        div.textContent = value ?? '';
        return div.innerHTML;
    }

    function formatDate(value) {
        if (!value) {
            return '—';
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
            return '—';
        }

        return number.toLocaleString('pt-BR', {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2
        });
    }

    function getTalhaoNome(id) {
        const item = talhoes.find(
            talhao => Number(talhao.id) === Number(id)
        );

        return item ? item.nome : `Talhão ${id}`;
    }

    function getCulturaNome(id) {
        const item = culturas.find(
            cultura => Number(cultura.id) === Number(id)
        );

        return item ? item.nome : `Cultura ${id}`;
    }

    function getStatusClass(status) {
        const value = String(status || '').toLowerCase();

        if (value.includes('conclu')) {
            return 'completed';
        }

        if (value.includes('planej')) {
            return 'planned';
        }

        if (value.includes('cancel')) {
            return 'cancelled';
        }

        return 'active';
    }

    async function loadReferences() {
        const [talhaoResponse, culturaResponse] = await Promise.all([
            fetch('/Talhao/GetList', {
                method: 'GET',
                headers: getAuthHeaders()
            }),
            fetch('/Cultura/GetList', {
                method: 'GET',
                headers: getAuthHeaders()
            })
        ]);

        if (!talhaoResponse.ok) {
            throw new Error(
                `Erro ao carregar talhões (${talhaoResponse.status})`
            );
        }

        if (!culturaResponse.ok) {
            throw new Error(
                `Erro ao carregar culturas (${culturaResponse.status})`
            );
        }

        talhoes = await talhaoResponse.json();
        culturas = await culturaResponse.json();

        talhaoSelect.innerHTML =
            '<option value="">Selecione um talhão...</option>';

        talhoes.forEach(talhao => {
            const option = document.createElement('option');

            option.value = talhao.id;
            option.textContent = talhao.nome;

            talhaoSelect.appendChild(option);
        });

        culturaSelect.innerHTML =
            '<option value="">Selecione uma cultura...</option>';

        culturas.forEach(cultura => {
            const option = document.createElement('option');

            option.value = cultura.id;
            option.textContent = cultura.nome;

            culturaSelect.appendChild(option);
        });
    }

    async function loadPlantios() {
        showLoading();

        try {
            await loadReferences();

            const response = await fetch('/Plantio/GetList', {
                method: 'GET',
                headers: getAuthHeaders()
            });

            if (!response.ok) {
                throw new Error(
                    `Erro ao carregar plantios (${response.status})`
                );
            }

            plantios = await response.json();

            updateIndicators();
            renderPlantios(plantios);

        } catch (error) {
            console.error('Erro ao carregar plantios:', error);
            showError();
        }
    }

    function updateIndicators() {
        const total = plantios.length;

        const andamento = plantios.filter(item =>
            String(item.status || '')
                .trim()
                .toLowerCase() === 'em andamento'
        ).length;

        const produtividade = plantios.reduce(
            (sum, item) =>
                sum + (Number(item.produtividadeEstimada) || 0),
            0
        );

        const culturaIds = new Set(
            plantios
                .map(item => Number(item.culturaId))
                .filter(id => id > 0)
        );

        totalElement.textContent = total;
        andamentoElement.textContent = andamento;
        produtividadeElement.textContent = formatNumber(produtividade);
        culturasElement.textContent = culturaIds.size;
    }

    function renderPlantios(lista) {
        tableBody.innerHTML = '';

        if (!lista.length) {
            showEmpty();
            updateResultsText(0);
            return;
        }

        lista.forEach(item => {
            const row = document.createElement('tr');

            const talhao = escapeHtml(
                getTalhaoNome(item.talhaoId)
            );

            const cultura = escapeHtml(
                getCulturaNome(item.culturaId)
            );

            const status = escapeHtml(
                item.status || 'Em andamento'
            );

            const statusClass = getStatusClass(item.status);

            row.innerHTML = `
                <td>
                    <span class="plantio-id">#${item.id}</span>
                </td>

                <td>
                    <div class="plantio-name-cell">
                        <span class="plantio-row-icon">
                            <i class="fa-solid fa-seedling"></i>
                        </span>

                        <div>
                            <strong>${talhao}</strong>
                            <small>Área de cultivo</small>
                        </div>
                    </div>
                </td>

                <td>
                    <span class="plantio-property-badge">
                        <i class="fa-solid fa-leaf"></i>
                        ${cultura}
                    </span>
                </td>

                <td>
                    <strong class="plantio-area">
                        ${formatDate(item.dataPlantio)}
                    </strong>
                </td>

                <td>
                    ${formatDate(item.dataPrevistaColheita)}
                </td>

                <td>
                    <strong class="plantio-area">
                        ${formatNumber(item.produtividadeEstimada)}
                    </strong>
                </td>

                <td>
                    <span class="plantio-status ${statusClass}">
                        ${status}
                    </span>
                </td>

                <td>
                    <div class="plantio-actions">

                        <button
                            type="button"
                            class="plantio-action-button edit"
                            data-action="edit"
                            data-id="${item.id}"
                            title="Editar plantio"
                            aria-label="Editar plantio">

                            <i class="fa-solid fa-pen"></i>
                        </button>

                        <button
                            type="button"
                            class="plantio-action-button delete"
                            data-action="delete"
                            data-id="${item.id}"
                            title="Excluir plantio"
                            aria-label="Excluir plantio">

                            <i class="fa-solid fa-trash"></i>
                        </button>

                    </div>
                </td>
            `;

            tableBody.appendChild(row);
        });

        updateResultsText(lista.length);
    }

    function showLoading() {
        tableBody.innerHTML = `
            <tr>
                <td colspan="8">
                    <div class="plantio-loading">
                        <span class="plantio-loading-icon">
                            <i class="fa-solid fa-seedling"></i>
                        </span>

                        <span>Carregando plantios...</span>
                    </div>
                </td>
            </tr>
        `;
    }

    function showEmpty() {
        tableBody.innerHTML = `
            <tr>
                <td colspan="8">
                    <div class="plantio-empty">

                        <span class="plantio-empty-icon">
                            <i class="fa-solid fa-seedling"></i>
                        </span>

                        <strong>Nenhum plantio encontrado</strong>

                        <span>
                            Cadastre um plantio para começar.
                        </span>

                        <button
                            type="button"
                            class="plantio-empty-button"
                            id="btnEmptyNovoPlantio">

                            <i class="fa-solid fa-plus"></i>
                            Cadastrar plantio
                        </button>

                    </div>
                </td>
            </tr>
        `;

        document
            .getElementById('btnEmptyNovoPlantio')
            ?.addEventListener('click', openCreateModal);
    }

    function showError() {
        tableBody.innerHTML = `
            <tr>
                <td colspan="8">
                    <div class="plantio-error">

                        <span class="plantio-error-icon">
                            <i class="fa-solid fa-triangle-exclamation"></i>
                        </span>

                        <strong>Não foi possível carregar os plantios</strong>

                        <span>
                            Verifique a conexão e tente novamente.
                        </span>

                        <button
                            type="button"
                            class="plantio-retry-button"
                            id="btnRetryPlantio">

                            <i class="fa-solid fa-rotate-right"></i>
                            Tentar novamente
                        </button>

                    </div>
                </td>
            </tr>
        `;

        document
            .getElementById('btnRetryPlantio')
            ?.addEventListener('click', loadPlantios);

        resultsText.textContent = 'Erro ao carregar registros';
    }

    function updateResultsText(total) {
        if (total === 0) {
            resultsText.textContent = 'Nenhum plantio encontrado';
            return;
        }

        if (total === 1) {
            resultsText.textContent = '1 plantio encontrado';
            return;
        }

        resultsText.textContent = `${total} plantios encontrados`;
    }

    function filterPlantios() {
        const term = searchInput.value
            .trim()
            .toLowerCase();

        if (!term) {
            renderPlantios(plantios);
            return;
        }

        const filtered = plantios.filter(item => {
            const talhao = String(
                getTalhaoNome(item.talhaoId)
            ).toLowerCase();

            const cultura = String(
                getCulturaNome(item.culturaId)
            ).toLowerCase();

            const status = String(
                item.status || ''
            ).toLowerCase();

            const observacoes = String(
                item.observacoes || ''
            ).toLowerCase();

            const id = String(item.id);

            return (
                talhao.includes(term) ||
                cultura.includes(term) ||
                status.includes(term) ||
                observacoes.includes(term) ||
                id.includes(term)
            );
        });

        renderPlantios(filtered);
    }

    async function openCreateModal() {
        try {
            await loadReferences();

            form.reset();

            document.getElementById('plantioId').value = '0';
            document.getElementById('plantioStatus').value =
                'Em andamento';

            document.getElementById('plantioModalLabel').textContent =
                'Novo plantio';

            plantioModal.show();

        } catch (error) {
            console.error(
                'Erro ao preparar cadastro de plantio:',
                error
            );

            await CultivaAlert.error(
    'Não foi possível carregar os talhões e culturas.',
    'Erro ao carregar'
);
        }
    }

    async function openEditModal(id) {
        try {
            await loadReferences();

            const item = plantios.find(
                plantio => Number(plantio.id) === Number(id)
            );

            if (!item) {
                await CultivaAlert.warning(
    'Plantio não encontrado.',
    'Atenção'
);
                return;
            }

            document.getElementById('plantioId').value =
                item.id;

            document.getElementById('plantioTalhao').value =
                item.talhaoId;

            document.getElementById('plantioCultura').value =
                item.culturaId;

            document.getElementById('plantioData').value =
                toInputDate(item.dataPlantio);

            document.getElementById('plantioColheita').value =
                toInputDate(item.dataPrevistaColheita);

            document.getElementById('plantioProd').value =
                item.produtividadeEstimada ?? '';

            document.getElementById('plantioCusto').value =
                item.custoProducao ?? '';

            document.getElementById('plantioStatus').value =
                item.status || 'Em andamento';

            document.getElementById('plantioObservacoes').value =
                item.observacoes || '';

            document.getElementById('plantioModalLabel').textContent =
                'Editar plantio';

            plantioModal.show();

        } catch (error) {
            console.error(
                'Erro ao abrir edição do plantio:',
                error
            );

            await CultivaAlert.error(
    'Não foi possível carregar os dados para edição.',
    'Erro ao carregar'
);
        }
    }

    async function savePlantio(event) {
        event.preventDefault();

        const id = Number(
            document.getElementById('plantioId').value
        );

        const talhaoId = Number(
            document.getElementById('plantioTalhao').value
        );

        const culturaId = Number(
            document.getElementById('plantioCultura').value
        );

        const dataPlantio =
            document.getElementById('plantioData').value;

        const dataPrevistaColheita =
            document.getElementById('plantioColheita').value;

        const produtividadeValue =
            document.getElementById('plantioProd').value;

        const custoValue =
            document.getElementById('plantioCusto').value;

        const status =
            document.getElementById('plantioStatus').value;

        const observacoes = document
            .getElementById('plantioObservacoes')
            .value
            .trim();

        if (!talhaoId) {
            await CultivaAlert.warning(
    'Selecione um talhão.',
    'Campo obrigatório'
);
            return;
        }

        if (!culturaId) {
           await CultivaAlert.warning(
    'Selecione uma cultura.',
    'Campo obrigatório'
);
            return;
        }

        if (!dataPlantio) {
           await CultivaAlert.warning(
    'Informe a data do plantio.',
    'Campo obrigatório'
);
            return;
        }

        if (
            dataPrevistaColheita &&
            dataPrevistaColheita < dataPlantio
        ) {
            await CultivaAlert.warning(
    'A colheita prevista não pode ser anterior à data do plantio.',
    'Data inválida'
);
            return;
        }

        const formData = {
            id,
            talhaoId,
            culturaId,
            dataPlantio,
            dataPrevistaColheita:
                dataPrevistaColheita || null,
            produtividadeEstimada:
                produtividadeValue === ''
                    ? null
                    : Number(produtividadeValue),
            custoProducao:
                custoValue === ''
                    ? null
                    : Number(custoValue),
            status,
            observacoes:
                observacoes || null
        };

        const url = id === 0
            ? '/Plantio/Create'
            : '/Plantio/Update';

        try {
            const response = await fetch(url, {
                method: 'POST',
                headers: getAuthHeaders(true),
                body: JSON.stringify(formData)
            });

            if (!response.ok) {
                throw new Error(
                    `Erro na requisição (${response.status})`
                );
            }

            const result = await response.json();

            if (!result.success) {
               await CultivaAlert.error(
    result.message ||
    'Não foi possível salvar o plantio.',
    'Erro ao salvar'
);
                return;
            }

            plantioModal.hide();

            await loadPlantios();

        } catch (error) {
            console.error('Erro ao salvar plantio:', error);

           await CultivaAlert.error(
    'Não foi possível salvar o plantio. Tente novamente.',
    'Erro ao salvar'
);
        }
    }

    async function deletePlantio(id) {
        const item = plantios.find(
            plantio => Number(plantio.id) === Number(id)
        );

        const description = item
            ? `${getTalhaoNome(item.talhaoId)} - ${getCulturaNome(item.culturaId)}`
            : 'este plantio';

        const confirmed = await CultivaAlert.delete(
    'Excluir plantio?',
    `Deseja realmente excluir "${description}"?`
);

        if (!confirmed) {
            return;
        }

        try {
            const response = await fetch(
                `/Plantio/Delete?id=${id}`,
                {
                    method: 'POST',
                    headers: getAuthHeaders()
                }
            );

            if (!response.ok) {
                throw new Error(
                    `Erro na requisição (${response.status})`
                );
            }

            const result = await response.json();

            if (!result.success) {
               await CultivaAlert.error(
    result.message ||
    'Não foi possível excluir o plantio.',
    'Erro ao excluir'
);
                return;
            }

            await loadPlantios();

        } catch (error) {
            console.error('Erro ao excluir plantio:', error);

            await CultivaAlert.error(
    'Não foi possível excluir o plantio.',
    'Erro ao excluir'
);
        }
    }

    btnNovo.addEventListener('click', openCreateModal);
    searchInput.addEventListener('input', filterPlantios);
    form.addEventListener('submit', savePlantio);

    tableBody.addEventListener('click', function (event) {
        const button = event.target.closest('[data-action]');

        if (!button) {
            return;
        }

        const id = Number(button.dataset.id);
        const action = button.dataset.action;

        if (action === 'edit') {
            openEditModal(id);
        }

        if (action === 'delete') {
            deletePlantio(id);
        }
    });

    loadPlantios();
});