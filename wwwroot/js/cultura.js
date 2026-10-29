document.addEventListener('DOMContentLoaded', function () {
    const modalElement = document.getElementById('culturaModal');
    const culturaModal = new bootstrap.Modal(modalElement);

    const form = document.getElementById('culturaForm');
    const tableBody = document.getElementById('culturaTableBody');
    const searchInput = document.getElementById('culturaSearch');
    const btnNova = document.getElementById('btnNovaCultura');

    const totalElement = document.getElementById('culturaTotal');
    const withDescriptionElement =
        document.getElementById('culturaWithDescription');
    const withoutDescriptionElement =
        document.getElementById('culturaWithoutDescription');
    const resultsText = document.getElementById('culturaResultsText');

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

    async function loadCulturas() {
        showLoading();

        try {
            const response = await fetch('/Cultura/GetList', {
                method: 'GET',
                headers: getAuthHeaders()
            });

            if (!response.ok) {
                throw new Error(
                    `Erro ao carregar culturas (${response.status})`
                );
            }

            culturas = await response.json();

            updateIndicators();
            renderCulturas(culturas);

        } catch (error) {
            console.error('Erro ao carregar culturas:', error);
            showError();
        }
    }

    function updateIndicators() {
        const total = culturas.length;

        const withDescription = culturas.filter(item =>
            String(item.descricao || '').trim().length > 0
        ).length;

        totalElement.textContent = total;
        withDescriptionElement.textContent = withDescription;
        withoutDescriptionElement.textContent =
            total - withDescription;
    }

    function renderCulturas(lista) {
        tableBody.innerHTML = '';

        if (!lista.length) {
            showEmpty();
            updateResultsText(0);
            return;
        }

        lista.forEach(item => {
            const row = document.createElement('tr');

            const nome = escapeHtml(item.nome);

            const descricao = item.descricao
                ? escapeHtml(item.descricao)
                : '<span class="cultura-muted">Sem descrição</span>';

            row.innerHTML = `
                <td>
                    <span class="cultura-id">#${item.id}</span>
                </td>

                <td>
                    <div class="cultura-name-cell">
                        <span class="cultura-row-icon">
                            <i class="fa-solid fa-leaf"></i>
                        </span>

                        <div>
                            <strong>${nome}</strong>
                            <small>Cultura agrícola</small>
                        </div>
                    </div>
                </td>

                <td class="cultura-description">
                    ${descricao}
                </td>

                <td>
                    <div class="cultura-actions">

                        <button
                            type="button"
                            class="cultura-action-button edit"
                            data-action="edit"
                            data-id="${item.id}"
                            title="Editar cultura"
                            aria-label="Editar cultura">

                            <i class="fa-solid fa-pen"></i>
                        </button>

                        <button
                            type="button"
                            class="cultura-action-button delete"
                            data-action="delete"
                            data-id="${item.id}"
                            title="Excluir cultura"
                            aria-label="Excluir cultura">

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
                <td colspan="4">
                    <div class="cultura-loading">
                        <span class="cultura-loading-icon">
                            <i class="fa-solid fa-seedling"></i>
                        </span>

                        <span>Carregando culturas...</span>
                    </div>
                </td>
            </tr>
        `;
    }

    function showEmpty() {
        tableBody.innerHTML = `
            <tr>
                <td colspan="4">
                    <div class="cultura-empty">

                        <span class="cultura-empty-icon">
                            <i class="fa-solid fa-seedling"></i>
                        </span>

                        <strong>Nenhuma cultura encontrada</strong>

                        <span>
                            Cadastre uma cultura para começar.
                        </span>

                        <button
                            type="button"
                            class="cultura-empty-button"
                            id="btnEmptyNovaCultura">

                            <i class="fa-solid fa-plus"></i>
                            Cadastrar cultura
                        </button>

                    </div>
                </td>
            </tr>
        `;

        document
            .getElementById('btnEmptyNovaCultura')
            ?.addEventListener('click', openCreateModal);
    }

    function showError() {
        tableBody.innerHTML = `
            <tr>
                <td colspan="4">
                    <div class="cultura-error">

                        <span class="cultura-error-icon">
                            <i class="fa-solid fa-triangle-exclamation"></i>
                        </span>

                        <strong>
                            Não foi possível carregar as culturas
                        </strong>

                        <span>
                            Verifique a conexão e tente novamente.
                        </span>

                        <button
                            type="button"
                            class="cultura-retry-button"
                            id="btnRetryCultura">

                            <i class="fa-solid fa-rotate-right"></i>
                            Tentar novamente
                        </button>

                    </div>
                </td>
            </tr>
        `;

        document
            .getElementById('btnRetryCultura')
            ?.addEventListener('click', loadCulturas);

        resultsText.textContent = 'Erro ao carregar registros';
    }

    function updateResultsText(total) {
        if (total === 0) {
            resultsText.textContent = 'Nenhuma cultura encontrada';
            return;
        }

        if (total === 1) {
            resultsText.textContent = '1 cultura encontrada';
            return;
        }

        resultsText.textContent = `${total} culturas encontradas`;
    }

    function filterCulturas() {
        const term = searchInput.value
            .trim()
            .toLowerCase();

        if (!term) {
            renderCulturas(culturas);
            return;
        }

        const filtered = culturas.filter(item => {
            const nome = String(item.nome || '').toLowerCase();
            const descricao =
                String(item.descricao || '').toLowerCase();
            const id = String(item.id);

            return (
                nome.includes(term) ||
                descricao.includes(term) ||
                id.includes(term)
            );
        });

        renderCulturas(filtered);
    }

    function openCreateModal() {
        form.reset();

        document.getElementById('culturaId').value = '0';
        document.getElementById('culturaModalLabel').textContent =
            'Nova cultura';

        culturaModal.show();
    }

    function openEditModal(id) {
        const item = culturas.find(
            cultura => Number(cultura.id) === Number(id)
        );

        if (!item) {
            alert('Cultura não encontrada.');
            return;
        }

        document.getElementById('culturaId').value = item.id;
        document.getElementById('cultNome').value = item.nome;
        document.getElementById('cultDesc').value =
            item.descricao || '';

        document.getElementById('culturaModalLabel').textContent =
            'Editar cultura';

        culturaModal.show();
    }

    async function saveCultura(event) {
        event.preventDefault();

        const id = Number(
            document.getElementById('culturaId').value
        );

        const nome = document
            .getElementById('cultNome')
            .value
            .trim();

        const descricao = document
            .getElementById('cultDesc')
            .value
            .trim();

        if (!nome) {
            alert('Informe o nome da cultura.');
            return;
        }

        const formData = {
            id,
            nome,
            descricao
        };

        const url = id === 0
            ? '/Cultura/Create'
            : '/Cultura/Update';

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
                alert(
                    result.message ||
                    'Não foi possível salvar a cultura.'
                );
                return;
            }

            culturaModal.hide();

            await loadCulturas();

        } catch (error) {
            console.error('Erro ao salvar cultura:', error);

            alert(
                'Não foi possível salvar a cultura. Tente novamente.'
            );
        }
    }

    async function deleteCultura(id) {
        const item = culturas.find(
            cultura => Number(cultura.id) === Number(id)
        );

        const nome = item?.nome || 'esta cultura';

        const confirmed = confirm(
            `Deseja realmente excluir "${nome}"?`
        );

        if (!confirmed) {
            return;
        }

        try {
            const response = await fetch(
                `/Cultura/Delete?id=${id}`,
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
                alert(
                    result.message ||
                    'Não foi possível excluir a cultura.'
                );
                return;
            }

            await loadCulturas();

        } catch (error) {
            console.error('Erro ao excluir cultura:', error);

            alert(
                'Não foi possível excluir a cultura.'
            );
        }
    }

    btnNova.addEventListener('click', openCreateModal);
    searchInput.addEventListener('input', filterCulturas);
    form.addEventListener('submit', saveCultura);

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
            deleteCultura(id);
        }
    });

    loadCulturas();
});