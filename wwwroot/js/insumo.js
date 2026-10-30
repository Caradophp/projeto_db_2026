document.addEventListener('DOMContentLoaded', () => {
    const modalElement = document.getElementById('insumoModal');
    const form = document.getElementById('insumoForm');
    const tableBody = document.getElementById('insumoTableBody');
    const searchInput = document.getElementById('insumoSearch');
    const btnNovo = document.getElementById('btnNovoInsumo');

    if (!modalElement || !form || !tableBody || !btnNovo) {
        console.error('Elementos da tela de insumos não foram encontrados.');
        return;
    }

    const modal = new bootstrap.Modal(modalElement);

    let insumos = [];

    function getHeaders(json = false) {
        const token = localStorage.getItem('token');

        const headers = {};

        if (token) {
            headers['Authorization'] = `Bearer ${token}`;
        }

        if (json) {
            headers['Content-Type'] = 'application/json';
        }

        return headers;
    }

    function escapeHtml(value) {
        return String(value ?? '')
            .replaceAll('&', '&amp;')
            .replaceAll('<', '&lt;')
            .replaceAll('>', '&gt;')
            .replaceAll('"', '&quot;')
            .replaceAll("'", '&#039;');
    }

    function formatEstoque(value) {
        return Number(value || 0).toLocaleString('pt-BR', {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2
        });
    }

    function updateIndicators() {
        const total = insumos.length;

        const tipos = new Set(
            insumos
                .map(item => (item.tipo || '').trim().toLowerCase())
                .filter(tipo => tipo !== '')
        ).size;

        const estoqueTotal = insumos.reduce(
            (totalAtual, item) =>
                totalAtual + Number(item.estoque || 0),
            0
        );

        const totalElement =
            document.getElementById('insumoTotal');

        const typesElement =
            document.getElementById('insumoTypes');

        const stockElement =
            document.getElementById('insumoStock');

        if (totalElement) {
            totalElement.textContent = total;
        }

        if (typesElement) {
            typesElement.textContent = tipos;
        }

        if (stockElement) {
            stockElement.textContent =
                estoqueTotal.toLocaleString('pt-BR', {
                    minimumFractionDigits: 0,
                    maximumFractionDigits: 2
                });
        }
    }

    function updateResults(total) {
        const element =
            document.getElementById('insumoResultsText');

        if (!element) return;

        element.textContent =
            total === 1
                ? '1 insumo encontrado'
                : `${total} insumos encontrados`;
    }

    function renderTable(lista) {
        if (lista.length === 0) {
            tableBody.innerHTML = `
                <tr>
                    <td colspan="6">
                        <div class="insumo-empty">
                            <div class="insumo-empty-icon">
                                <i class="fa-solid fa-flask"></i>
                            </div>

                            <strong>Nenhum insumo encontrado</strong>
                            <span>Cadastre um insumo para começar.</span>
                        </div>
                    </td>
                </tr>
            `;

            updateResults(0);
            return;
        }

        tableBody.innerHTML = lista.map(item => `
            <tr>
                <td class="insumo-id">
                    #${item.id}
                </td>

                <td>
                    <div class="insumo-name-cell">
                        <div class="insumo-row-icon">
                            <i class="fa-solid fa-flask"></i>
                        </div>

                        <div>
                            <strong>
                                ${escapeHtml(item.nome)}
                            </strong>

                            <span>
                                ${escapeHtml(
                                    item.tipo || 'Sem tipo informado'
                                )}
                            </span>
                        </div>
                    </div>
                </td>

                <td>
                    ${escapeHtml(item.tipo || '-')}
                </td>

                <td>
                    ${escapeHtml(item.unidadeMedida)}
                </td>

                <td>
                    <span class="insumo-stock">
                        ${formatEstoque(item.estoque)}
                    </span>
                </td>

                <td>
                    <div class="insumo-actions">
                        <button
                            type="button"
                            class="insumo-action edit"
                            data-action="edit"
                            data-id="${item.id}"
                            title="Editar insumo">

                            <i class="fa-solid fa-pen"></i>
                        </button>

                        <button
                            type="button"
                            class="insumo-action delete"
                            data-action="delete"
                            data-id="${item.id}"
                            title="Excluir insumo">

                            <i class="fa-solid fa-trash"></i>
                        </button>
                    </div>
                </td>
            </tr>
        `).join('');

        updateResults(lista.length);
    }

    function filterInsumos() {
        const termo =
            (searchInput?.value || '')
                .trim()
                .toLowerCase();

        if (!termo) {
            renderTable(insumos);
            return;
        }

        const filtrados = insumos.filter(item =>
            String(item.id).includes(termo) ||
            (item.nome || '').toLowerCase().includes(termo) ||
            (item.tipo || '').toLowerCase().includes(termo) ||
            (item.unidadeMedida || '')
                .toLowerCase()
                .includes(termo)
        );

        renderTable(filtrados);
    }

    async function loadInsumos() {
        try {
            const response = await fetch(
                '/Insumo/GetList',
                {
                    headers: getHeaders()
                }
            );

            if (!response.ok) {
                throw new Error(`HTTP ${response.status}`);
            }

            insumos = await response.json();

            updateIndicators();
            filterInsumos();
        }
        catch (error) {
            console.error(
                'Erro ao carregar insumos:',
                error
            );

            tableBody.innerHTML = `
                <tr>
                    <td colspan="6">
                        <div class="insumo-empty">
                            <strong>
                                Não foi possível carregar os insumos
                            </strong>

                            <span>
                                Tente novamente em alguns instantes.
                            </span>
                        </div>
                    </td>
                </tr>
            `;
        }
    }

    function openCreateModal() {
        form.reset();

        document.getElementById('insumoId').value = '0';

        document.getElementById(
            'insumoModalLabel'
        ).textContent = 'Novo insumo';

        modal.show();

        setTimeout(() => {
            document.getElementById('insNome')?.focus();
        }, 200);
    }

    function openEditModal(id) {
        const item = insumos.find(
            i => Number(i.id) === Number(id)
        );

        if (!item) return;

        document.getElementById('insumoId').value =
            item.id;

        document.getElementById('insNome').value =
            item.nome || '';

        document.getElementById('insTipo').value =
            item.tipo || '';

        document.getElementById('insUnidade').value =
            item.unidadeMedida || '';

        document.getElementById('insEstoque').value =
            item.estoque ?? 0;

        document.getElementById(
            'insumoModalLabel'
        ).textContent = 'Editar insumo';

        modal.show();
    }

    async function saveInsumo(event) {
        event.preventDefault();

        const id = Number(
            document.getElementById('insumoId').value || 0
        );

        const nome =
            document.getElementById('insNome')
                .value
                .trim();

        const tipo =
            document.getElementById('insTipo')
                .value
                .trim();

        const unidadeMedida =
            document.getElementById('insUnidade')
                .value
                .trim();

        const estoque = Number(
            document.getElementById('insEstoque').value
        );

        if (
            !nome ||
            !unidadeMedida ||
            Number.isNaN(estoque)
        ) {
            await CultivaAlert.warning(
    'Preencha os campos obrigatórios.',
    'Campos obrigatórios'
);
            return;
        }

        if (estoque < 0) {
           await CultivaAlert.warning(
    'O estoque não pode ser negativo.',
    'Estoque inválido'
);
            return;
        }

        const dados = {
            id,
            nome,
            tipo: tipo || null,
            unidadeMedida,
            estoque
        };

        const url =
            id === 0
                ? '/Insumo/Create'
                : '/Insumo/Update';

        try {
            const response = await fetch(url, {
                method: 'POST',
                headers: getHeaders(true),
                body: JSON.stringify(dados)
            });

            if (!response.ok) {
                throw new Error(`HTTP ${response.status}`);
            }

            const result = await response.json();

            if (!result.success) {
                await CultivaAlert.error(
    result.message ||
    'Não foi possível salvar o insumo.',
    'Erro ao salvar'
);

                return;
            }

            modal.hide();

            await loadInsumos();
        }
        catch (error) {
            console.error(
                'Erro ao salvar insumo:',
                error
            );

            await CultivaAlert.error(
    'Erro ao salvar o insumo.',
    'Erro ao salvar'
);
        }
    }

    async function deleteInsumo(id) {
        const item = insumos.find(
            i => Number(i.id) === Number(id)
        );

        if (!item) return;

        const confirmar = await CultivaAlert.delete(
    'Excluir insumo?',
    `Deseja excluir o insumo "${item.nome}"?`
);

        if (!confirmar) return;

        try {
            const response = await fetch(
                `/Insumo/Delete?id=${id}`,
                {
                    method: 'POST',
                    headers: getHeaders()
                }
            );

            if (!response.ok) {
                throw new Error(`HTTP ${response.status}`);
            }

            const result = await response.json();

            if (!result.success) {
                await CultivaAlert.error(
    result.message ||
    'Não foi possível excluir o insumo.',
    'Erro ao excluir'
);

                return;
            }

            await loadInsumos();
        }
        catch (error) {
            console.error(
                'Erro ao excluir insumo:',
                error
            );

            await CultivaAlert.error(
    'Erro ao excluir o insumo.',
    'Erro ao excluir'
);
        }
    }

    btnNovo.addEventListener(
        'click',
        openCreateModal
    );

    form.addEventListener(
        'submit',
        saveInsumo
    );

    searchInput?.addEventListener(
        'input',
        filterInsumos
    );

    tableBody.addEventListener(
        'click',
        event => {
            const button =
                event.target.closest('[data-action]');

            if (!button) return;

            const id = Number(button.dataset.id);
            const action = button.dataset.action;

            if (action === 'edit') {
                openEditModal(id);
            }

            if (action === 'delete') {
                deleteInsumo(id);
            }
        }
    );

    loadInsumos();
});