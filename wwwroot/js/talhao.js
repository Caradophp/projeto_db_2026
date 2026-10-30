document.addEventListener('DOMContentLoaded', function () {
    const modalElement = document.getElementById('talhaoModal');
    const talhaoModal = new bootstrap.Modal(modalElement);

    const form = document.getElementById('talhaoForm');
    const tableBody = document.getElementById('talhaoTableBody');
    const searchInput = document.getElementById('talhaoSearch');
    const btnNovo = document.getElementById('btnNovoTalhao');
    const propSelect = document.getElementById('talProp');

    const totalElement = document.getElementById('talhaoTotal');
    const totalAreaElement = document.getElementById('talhaoTotalArea');
    const averageAreaElement = document.getElementById('talhaoAverageArea');
    const propertiesElement = document.getElementById('talhaoProperties');
    const resultsText = document.getElementById('talhaoResultsText');

    let talhoes = [];
    let propriedades = [];

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

    function formatArea(value) {
        const area = Number(value) || 0;

        return area.toLocaleString('pt-BR', {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2
        }) + ' ha';
    }

    function escapeHtml(value) {
        const div = document.createElement('div');
        div.textContent = value ?? '';
        return div.innerHTML;
    }

    function getPropriedadeNome(id) {
        const propriedade = propriedades.find(
            item => Number(item.id) === Number(id)
        );

        return propriedade ? propriedade.nome : `Propriedade ${id}`;
    }

    async function loadPropriedades() {
        const response = await fetch('/Propriedade/GetList', {
            method: 'GET',
            headers: getAuthHeaders()
        });

        if (!response.ok) {
            throw new Error(
                `Erro ao carregar propriedades (${response.status})`
            );
        }

        propriedades = await response.json();

        propSelect.innerHTML =
            '<option value="">Selecione uma propriedade...</option>';

        propriedades.forEach(prop => {
            const option = document.createElement('option');

            option.value = prop.id;
            option.textContent = prop.nome;

            propSelect.appendChild(option);
        });
    }

    async function loadTalhoes() {
        showLoading();

        try {
            await loadPropriedades();

            const response = await fetch('/Talhao/GetList', {
                method: 'GET',
                headers: getAuthHeaders()
            });

            if (!response.ok) {
                throw new Error(
                    `Erro ao carregar talhões (${response.status})`
                );
            }

            talhoes = await response.json();

            updateIndicators();
            renderTalhoes(talhoes);

        } catch (error) {
            console.error('Erro ao carregar talhões:', error);
            showError();
        }
    }

    function updateIndicators() {
        const total = talhoes.length;

        const totalArea = talhoes.reduce(
            (sum, item) => sum + (Number(item.area) || 0),
            0
        );

        const averageArea = total > 0
            ? totalArea / total
            : 0;

        const propertyIds = new Set(
            talhoes
                .map(item => Number(item.propriedadeId))
                .filter(id => id > 0)
        );

        totalElement.textContent = total;
        totalAreaElement.textContent = formatArea(totalArea);
        averageAreaElement.textContent = formatArea(averageArea);
        propertiesElement.textContent = propertyIds.size;
    }

    function renderTalhoes(lista) {
        tableBody.innerHTML = '';

        if (!lista.length) {
            showEmpty();
            updateResultsText(0);
            return;
        }

        lista.forEach(item => {
            const row = document.createElement('tr');

            const nome = escapeHtml(item.nome);
            const propriedade = escapeHtml(
                getPropriedadeNome(item.propriedadeId)
            );

            const descricao = item.descricao
                ? escapeHtml(item.descricao)
                : '<span class="talhao-muted">Sem descrição</span>';

            row.innerHTML = `
                <td>
                    <span class="talhao-id">#${item.id}</span>
                </td>

                <td>
                    <div class="talhao-name-cell">
                        <span class="talhao-row-icon">
                            <i class="fa-solid fa-seedling"></i>
                        </span>

                        <div>
                            <strong>${nome}</strong>
                            <small>Área de cultivo</small>
                        </div>
                    </div>
                </td>

                <td>
                    <strong class="talhao-area">
                        ${formatArea(item.area)}
                    </strong>
                </td>

                <td>
                    <span class="talhao-property-badge">
                        <i class="fa-solid fa-house-chimney"></i>
                        ${propriedade}
                    </span>
                </td>

                <td class="talhao-description">
                    ${descricao}
                </td>

                <td>
                    <div class="talhao-actions">

                        <button
                            type="button"
                            class="talhao-action-button edit"
                            data-action="edit"
                            data-id="${item.id}"
                            title="Editar talhão"
                            aria-label="Editar talhão">

                            <i class="fa-solid fa-pen"></i>
                        </button>

                        <button
                            type="button"
                            class="talhao-action-button delete"
                            data-action="delete"
                            data-id="${item.id}"
                            title="Excluir talhão"
                            aria-label="Excluir talhão">

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
                <td colspan="6">
                    <div class="talhao-loading">
                        <span class="talhao-loading-icon">
                            <i class="fa-solid fa-seedling"></i>
                        </span>

                        <span>Carregando talhões...</span>
                    </div>
                </td>
            </tr>
        `;
    }

    function showEmpty() {
        tableBody.innerHTML = `
            <tr>
                <td colspan="6">
                    <div class="talhao-empty">

                        <span class="talhao-empty-icon">
                            <i class="fa-solid fa-seedling"></i>
                        </span>

                        <strong>Nenhum talhão encontrado</strong>

                        <span>
                            Cadastre uma área de cultivo para começar.
                        </span>

                        <button
                            type="button"
                            class="talhao-empty-button"
                            id="btnEmptyNovoTalhao">

                            <i class="fa-solid fa-plus"></i>
                            Cadastrar talhão
                        </button>

                    </div>
                </td>
            </tr>
        `;

        document
            .getElementById('btnEmptyNovoTalhao')
            ?.addEventListener('click', openCreateModal);
    }

    function showError() {
        tableBody.innerHTML = `
            <tr>
                <td colspan="6">
                    <div class="talhao-error">

                        <span class="talhao-error-icon">
                            <i class="fa-solid fa-triangle-exclamation"></i>
                        </span>

                        <strong>Não foi possível carregar os talhões</strong>

                        <span>
                            Verifique a conexão e tente novamente.
                        </span>

                        <button
                            type="button"
                            class="talhao-retry-button"
                            id="btnRetryTalhao">

                            <i class="fa-solid fa-rotate-right"></i>
                            Tentar novamente
                        </button>

                    </div>
                </td>
            </tr>
        `;

        document
            .getElementById('btnRetryTalhao')
            ?.addEventListener('click', loadTalhoes);

        resultsText.textContent = 'Erro ao carregar registros';
    }

    function updateResultsText(total) {
        if (total === 0) {
            resultsText.textContent = 'Nenhum talhão encontrado';
            return;
        }

        if (total === 1) {
            resultsText.textContent = '1 talhão encontrado';
            return;
        }

        resultsText.textContent = `${total} talhões encontrados`;
    }

    function filterTalhoes() {
        const term = searchInput.value
            .trim()
            .toLowerCase();

        if (!term) {
            renderTalhoes(talhoes);
            return;
        }

        const filtered = talhoes.filter(item => {
            const nome = String(item.nome || '').toLowerCase();

            const descricao = String(
                item.descricao || ''
            ).toLowerCase();

            const propriedade = String(
                getPropriedadeNome(item.propriedadeId)
            ).toLowerCase();

            const id = String(item.id);

            return (
                nome.includes(term) ||
                descricao.includes(term) ||
                propriedade.includes(term) ||
                id.includes(term)
            );
        });

        renderTalhoes(filtered);
    }

    async function openCreateModal() {
        try {
            await loadPropriedades();

            form.reset();

            document.getElementById('talhaoId').value = '0';
            document.getElementById('talhaoModalLabel').textContent =
                'Novo talhão';

            talhaoModal.show();

        } catch (error) {
            console.error(
                'Erro ao preparar cadastro de talhão:',
                error
            );

            await CultivaAlert.error(
    'Não foi possível carregar as propriedades.',
    'Erro ao carregar'
);
        }
    }

    async function openEditModal(id) {
        try {
            await loadPropriedades();

            const item = talhoes.find(
                talhao => Number(talhao.id) === Number(id)
            );

            if (!item) {
                await CultivaAlert.warning(
    'Talhão não encontrado.',
    'Atenção'
);
                return;
            }

            document.getElementById('talhaoId').value = item.id;
            document.getElementById('talNome').value = item.nome;
            document.getElementById('talArea').value = item.area;
            document.getElementById('talProp').value =
                item.propriedadeId;

            document.getElementById('talDesc').value =
                item.descricao || '';

            document.getElementById('talhaoModalLabel').textContent =
                'Editar talhão';

            talhaoModal.show();

        } catch (error) {
            console.error(
                'Erro ao abrir edição do talhão:',
                error
            );

            await CultivaAlert.error(
    'Não foi possível carregar os dados para edição.',
    'Erro ao carregar'
);
        }
    }

    async function saveTalhao(event) {
        event.preventDefault();

        const id = Number(
            document.getElementById('talhaoId').value
        );

        const nome = document
            .getElementById('talNome')
            .value
            .trim();

        const area = Number(
            document.getElementById('talArea').value
        );

        const propriedadeId = Number(
            document.getElementById('talProp').value
        );

        const descricao = document
            .getElementById('talDesc')
            .value
            .trim();

        if (!nome) {
            await CultivaAlert.warning(
    'Informe o nome do talhão.',
    'Campo obrigatório'
);
            return;
        }

        if (!area || area <= 0) {
            await CultivaAlert.warning(
    'Informe uma área maior que zero.',
    'Área inválida'
);
            return;
        }

        if (!propriedadeId) {
            await CultivaAlert.warning(
    'Selecione uma propriedade.',
    'Campo obrigatório'
);
            return;
        }

        const formData = {
            id,
            nome,
            area,
            propriedadeId,
            descricao
        };

        const url = id === 0
            ? '/Talhao/Create'
            : '/Talhao/Update';

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
    result.message || 'Não foi possível salvar.',
    'Erro ao salvar'
);
                return;
            }

            talhaoModal.hide();

            await loadTalhoes();

        } catch (error) {
            console.error('Erro ao salvar talhão:', error);

           await CultivaAlert.error(
    'Não foi possível salvar o talhão. Tente novamente.',
    'Erro ao salvar'
);
        }
    }

    async function deleteTalhao(id) {
        const item = talhoes.find(
            talhao => Number(talhao.id) === Number(id)
        );

        const nome = item?.nome || 'este talhão';

        const confirmed = await CultivaAlert.delete(
    'Excluir talhão?',
    `Deseja realmente excluir "${nome}"?`
);

        if (!confirmed) {
            return;
        }

        try {
            const response = await fetch(
                `/Talhao/Delete?id=${id}`,
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
    result.message || 'Não foi possível excluir.',
    'Erro ao excluir'
);
                return;
            }

            await loadTalhoes();

        } catch (error) {
            console.error('Erro ao excluir talhão:', error);

            await CultivaAlert.error(
    'Não foi possível excluir o talhão.',
    'Erro ao excluir'
);
        }
    }

    btnNovo.addEventListener('click', openCreateModal);
    searchInput.addEventListener('input', filterTalhoes);
    form.addEventListener('submit', saveTalhao);

    tableBody.addEventListener('click', function (event) {
        const button = event.target.closest(
            '[data-action]'
        );

        if (!button) {
            return;
        }

        const id = Number(button.dataset.id);
        const action = button.dataset.action;

        if (action === 'edit') {
            openEditModal(id);
        }

        if (action === 'delete') {
            deleteTalhao(id);
        }
    });

    loadTalhoes();
});