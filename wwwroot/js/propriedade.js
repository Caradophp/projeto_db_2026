
document.addEventListener('DOMContentLoaded', function () {

    const modalElement = document.getElementById('propriedadeModal');
    const propModal = new bootstrap.Modal(modalElement);

    const propForm = document.getElementById('propriedadeForm');
    const tableBody = document.getElementById('propertyTableBody');
    const btnAdd = document.getElementById('btnNovaPropriedade');
    const searchInput = document.getElementById('propertySearch');

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

    // =========================================================
    // CARREGAMENTO
    // =========================================================

    async function loadPropriedades() {

        showLoading();

        try {

           const response = await fetch(
    '/Propriedade/GetList',
    {
        method: 'GET',
        headers: getAuthHeaders()
    }
);

            if (!response.ok) {
                throw new Error(
                    'Não foi possível carregar as propriedades.'
                );
            }

            const data = await response.json();

            propriedades = Array.isArray(data)
                ? data
                : [];

            updateIndicators();

            renderPropriedades(propriedades);

        } catch (error) {

            console.error(
                'Erro ao carregar propriedades:',
                error
            );

            showLoadError();
        }
    }


    // =========================================================
    // INDICADORES
    // =========================================================

    function updateIndicators() {

        const total = propriedades.length;

        const totalArea = propriedades.reduce(
            (sum, item) => {

                const area = Number(item.areaTotal);

                return sum +
                    (Number.isFinite(area) ? area : 0);

            },
            0
        );

        const averageArea =
            total > 0
                ? totalArea / total
                : 0;

        const users = new Set(
            propriedades
                .map(item => item.userId)
                .filter(userId =>
                    userId !== null &&
                    userId !== undefined
                )
        );

        setText(
            'propertyTotal',
            total
        );

        setText(
            'propertyTotalArea',
            formatArea(totalArea)
        );

        setText(
            'propertyAverageArea',
            formatArea(averageArea)
        );

        setText(
            'propertyUsers',
            users.size
        );
    }


    function setText(id, value) {

        const element =
            document.getElementById(id);

        if (element) {
            element.textContent = value;
        }
    }


    // =========================================================
    // TABELA
    // =========================================================

    function renderPropriedades(data) {

        if (!tableBody) {
            return;
        }

        updateResultsText(
            data.length,
            propriedades.length
        );

        if (data.length === 0) {

            const isSearching =
                searchInput &&
                searchInput.value.trim().length > 0;

            tableBody.innerHTML = `
                <tr>
                    <td colspan="5">

                        <div class="property-empty">

                            <div class="property-empty-icon">
                                <i class="fa-solid ${
                                    isSearching
                                        ? 'fa-magnifying-glass'
                                        : 'fa-seedling'
                                }"></i>
                            </div>

                            <strong>
                                ${
                                    isSearching
                                        ? 'Nenhuma propriedade encontrada'
                                        : 'Nenhuma propriedade cadastrada'
                                }
                            </strong>

                            <span>
                                ${
                                    isSearching
                                        ? 'Tente pesquisar por outro nome, ID ou usuário.'
                                        : 'Cadastre sua primeira propriedade para começar.'
                                }
                            </span>

                            ${
                                !isSearching
                                    ? `
                                        <button
                                            type="button"
                                            class="property-empty-button"
                                            id="emptyAddProperty">

                                            <i class="fa-solid fa-plus"></i>

                                            Cadastrar propriedade
                                        </button>
                                      `
                                    : ''
                            }

                        </div>

                    </td>
                </tr>
            `;

            const emptyAddButton =
                document.getElementById(
                    'emptyAddProperty'
                );

            emptyAddButton?.addEventListener(
                'click',
                openCreateModal
            );

            return;
        }


        tableBody.innerHTML =
            data.map(item => {

                const id =
                    Number(item.id ?? 0);

                const nome =
                    escapeHtml(
                        item.nome || 'Sem nome'
                    );

                const area =
                    formatArea(item.areaTotal);

                const userId =
                    escapeHtml(
                        String(
                            item.userId ?? '-'
                        )
                    );

                return `
                    <tr>

                        <td class="property-id">
                            #${id}
                        </td>

                        <td>

                            <div class="property-name-cell">

                                <span class="property-row-icon">
                                    <i class="fa-solid fa-house-chimney"></i>
                                </span>

                                <div>
                                    <strong>
                                        ${nome}
                                    </strong>

                                    <span>
                                        Propriedade #${id}
                                    </span>
                                </div>

                            </div>

                        </td>

                        <td>

                            <span class="property-area">
                                ${area}
                            </span>

                        </td>

                        <td>

                            <span class="property-user">

                                <i class="fa-regular fa-user"></i>

                                Usuário ${userId}

                            </span>

                        </td>

                        <td>

                            <div class="property-actions">

                                <button
                                    type="button"
                                    class="property-action-button edit btn-edit"
                                    data-id="${id}"
                                    title="Editar propriedade"
                                    aria-label="Editar propriedade">

                                    <i class="fa-solid fa-pen"></i>

                                </button>

                                <button
                                    type="button"
                                    class="property-action-button delete btn-delete"
                                    data-id="${id}"
                                    title="Excluir propriedade"
                                    aria-label="Excluir propriedade">

                                    <i class="fa-solid fa-trash"></i>

                                </button>

                            </div>

                        </td>

                    </tr>
                `;

            }).join('');
    }


    // =========================================================
    // RODAPÉ DA TABELA
    // =========================================================

    function updateResultsText(
        visible,
        total
    ) {

        const element =
            document.getElementById(
                'propertyResultsText'
            );

        if (!element) {
            return;
        }

        if (total === 0) {

            element.textContent =
                'Nenhuma propriedade cadastrada';

            return;
        }

        if (visible === total) {

            element.textContent =
                total === 1
                    ? 'Mostrando 1 propriedade'
                    : `Mostrando ${total} propriedades`;

            return;
        }

        element.textContent =
            `Mostrando ${visible} de ${total} propriedades`;
    }


    // =========================================================
    // FORMATAÇÃO
    // =========================================================

    function formatArea(value) {

        const area = Number(value);

        if (!Number.isFinite(area)) {
            return '0,00 ha';
        }

        return `${area.toLocaleString(
            'pt-BR',
            {
                minimumFractionDigits: 2,
                maximumFractionDigits: 2
            }
        )} ha`;
    }


    // =========================================================
    // LOADING
    // =========================================================

    function showLoading() {

        if (!tableBody) {
            return;
        }

        tableBody.innerHTML = `
            <tr>
                <td colspan="5">

                    <div class="property-loading">

                        <span class="property-loading-icon">
                            <i class="fa-solid fa-seedling"></i>
                        </span>

                        <span>
                            Carregando propriedades...
                        </span>

                    </div>

                </td>
            </tr>
        `;
    }


    // =========================================================
    // ERRO
    // =========================================================

    function showLoadError() {

        if (!tableBody) {
            return;
        }

        tableBody.innerHTML = `
            <tr>
                <td colspan="5">

                    <div class="property-empty property-error">

                        <div class="property-empty-icon">
                            <i class="fa-solid fa-triangle-exclamation"></i>
                        </div>

                        <strong>
                            Não foi possível carregar os dados
                        </strong>

                        <span>
                            Tente atualizar a página novamente.
                        </span>

                    </div>

                </td>
            </tr>
        `;

        const results =
            document.getElementById(
                'propertyResultsText'
            );

        if (results) {
            results.textContent =
                'Erro ao carregar propriedades';
        }
    }


    // =========================================================
    // MODAL - CRIAÇÃO
    // =========================================================

    function openCreateModal() {

        propForm.reset();

        document.getElementById(
            'propriedadeId'
        ).value = '0';

        document.getElementById(
            'propriedadeModalLabel'
        ).textContent =
            'Nova propriedade';

        propModal.show();
    }


    // =========================================================
    // MODAL - EDIÇÃO
    // =========================================================

    function openEditModal(id) {

        const item =
            propriedades.find(
                propriedade =>
                    Number(propriedade.id) ===
                    Number(id)
            );

        if (!item) {
            return;
        }

        document.getElementById(
            'propriedadeId'
        ).value =
            item.id;

        document.getElementById(
            'propNome'
        ).value =
            item.nome ?? '';

        document.getElementById(
            'propArea'
        ).value =
            item.areaTotal ?? '';

        document.getElementById(
            'propUser'
        ).value =
            item.userId ?? '';

        document.getElementById(
            'propriedadeModalLabel'
        ).textContent =
            'Editar propriedade';

        propModal.show();
    }


    // =========================================================
    // SALVAR
    // =========================================================

    async function savePropriedade(event) {

        event.preventDefault();

        const id =
            Number(
                document.getElementById(
                    'propriedadeId'
                ).value
            );

        const formData = {

            id: id,

            nome:
                document.getElementById(
                    'propNome'
                ).value.trim(),

            areaTotal:
                Number(
                    document.getElementById(
                        'propArea'
                    ).value
                ),

            userId:
                Number(
                    document.getElementById(
                        'propUser'
                    ).value
                )
        };


        const url =
            id === 0
                ? '/Propriedade/Create'
                : '/Propriedade/Update';


        try {

           const response =
    await fetch(
        url,
        {
            method: 'POST',

            headers:
                getAuthHeaders(true),

            body:
                JSON.stringify(
                    formData
                )
        }
    );

            const result =
                await response.json();


            if (!result.success) {

               await CultivaAlert.error(
    'Erro: ' + result.message,
    'Erro ao salvar'
);

                return;
            }


            propModal.hide();

            await loadPropriedades();

        } catch (error) {

            console.error(
                'Erro ao salvar propriedade:',
                error
            );

            await CultivaAlert.error(
    'Não foi possível salvar a propriedade.',
    'Erro ao salvar'
);
        }
    }


    // =========================================================
    // EXCLUIR
    // =========================================================

    async function deletePropriedade(id) {

        const item =
            propriedades.find(
                propriedade =>
                    Number(propriedade.id) ===
                    Number(id)
            );

        const nome =
            item?.nome
                ? `"${item.nome}"`
                : 'esta propriedade';


        const confirmed =
    await CultivaAlert.delete(
        'Excluir propriedade?',
        `Tem certeza que deseja excluir ${nome}?`
    );


        if (!confirmed) {
            return;
        }


        try {

            const response =
    await fetch(
        `/Propriedade/Delete?id=${id}`,
        {
            method: 'POST',
            headers: getAuthHeaders()
        }
    );

            const result =
                await response.json();


           if (!result.success) {

    await CultivaAlert.error(
        result.message || 'Não foi possível excluir a propriedade.',
        'Erro ao excluir'
    );

    return;
}

            await loadPropriedades();

        } catch (error) {

            console.error(
                'Erro ao excluir propriedade:',
                error
            );

            await CultivaAlert.error(
    'Não foi possível excluir a propriedade.',
    'Erro ao excluir'
);
        }
    }


    // =========================================================
    // PESQUISA
    // =========================================================

    function filterPropriedades() {

        const search =
            searchInput.value
                .trim()
                .toLocaleLowerCase(
                    'pt-BR'
                );


        if (!search) {

            renderPropriedades(
                propriedades
            );

            return;
        }


        const filtered =
            propriedades.filter(
                item => {

                    const nome =
                        String(
                            item.nome || ''
                        )
                        .toLocaleLowerCase(
                            'pt-BR'
                        );

                    const id =
                        String(
                            item.id ?? ''
                        );

                    const userId =
                        String(
                            item.userId ?? ''
                        );


                    return (
                        nome.includes(search) ||
                        id.includes(search) ||
                        userId.includes(search)
                    );
                }
            );


        renderPropriedades(
            filtered
        );
    }


    // =========================================================
    // SEGURANÇA DO HTML
    // =========================================================

    function escapeHtml(value) {

        return String(value)
            .replaceAll(
                '&',
                '&amp;'
            )
            .replaceAll(
                '<',
                '&lt;'
            )
            .replaceAll(
                '>',
                '&gt;'
            )
            .replaceAll(
                '"',
                '&quot;'
            )
            .replaceAll(
                "'",
                '&#039;'
            );
    }


    // =========================================================
    // EVENTOS
    // =========================================================

    btnAdd?.addEventListener(
        'click',
        openCreateModal
    );


    propForm?.addEventListener(
        'submit',
        savePropriedade
    );


    searchInput?.addEventListener(
        'input',
        filterPropriedades
    );


    tableBody?.addEventListener(
        'click',
        function (event) {

            const button =
                event.target.closest(
                    'button[data-id]'
                );

            if (!button) {
                return;
            }


            const id =
                Number(
                    button.dataset.id
                );


            if (
                button.classList.contains(
                    'btn-edit'
                )
            ) {

                openEditModal(id);

                return;
            }


            if (
                button.classList.contains(
                    'btn-delete'
                )
            ) {

                deletePropriedade(id);
            }
        }
    );


    // =========================================================
    // INICIALIZAÇÃO
    // =========================================================

    loadPropriedades();

});
