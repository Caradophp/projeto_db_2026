document.addEventListener('DOMContentLoaded', function () {
    const talModal = new bootstrap.Modal(document.getElementById('talhaoModal'));
    const talForm = document.getElementById('talhaoForm');
    const tableBody = document.querySelector('.management-table tbody');
    const btnAdd = document.querySelector('.management-toolbar .btn-primary');
    const propSelect = document.getElementById('talProp');

    async function loadPropriedades() {
        try {
            const response = await fetch('/Propriedade/GetList');
            const data = await response.json();

            propSelect.innerHTML = '<option value="">Selecione uma propriedade...</option>';
            data.forEach(prop => {
                const option = document.createElement('option');
                option.value = prop.id;
                option.textContent = prop.nome;
                propSelect.appendChild(option);
            });
        } catch (error) {
            console.error('Erro ao carregar propriedades para o dropdown:', error);
        }
    }

    async function loadTalhoes() {
        try {
            const response = await fetch('/Talhao/GetList');
            const talhoes = await response.json();

            // We also need properties to map the ID to a Name in the table
            const propResponse = await fetch('/Propriedade/GetList');
            const propriedades = await propResponse.json();
            const propMap = {};
            propriedades.forEach(p => propMap[p.id] = p.nome);

            if (talhoes.length === 0) {
                tableBody.innerHTML = `
                    <tr>
                        <td colspan="6" class="management-empty">
                            <span class="management-empty-icon" aria-hidden="true"><i class="fa-regular fa-rectangle-list"></i></span>
                            <strong>Nenhum registro encontrado</strong>
                            <span>Os talhões aparecerão aqui.</span>
                        </td>
                    </tr>`;
                return;
            }

            tableBody.innerHTML = '';
            talhoes.forEach(item => {
                const row = document.createElement('tr');
                row.innerHTML = `
                    <td>${item.id}</td>
                    <td>${item.nome}</td>
                    <td>${item.area}</td>
                    <td>${propMap[item.propriedadeId] || 'N/A'}</td>
                    <td>${item.descricao || '-'}</td>
                    <td>
                        <button class="btn btn-sm btn-outline-primary me-2 btn-edit" data-id="${item.id}">
                            <i class="fa-solid fa-pen-to-square"></i>
                        </button>
                        <button class="btn btn-sm btn-outline-danger btn-delete" data-id="${item.id}">
                            <i class="fa-solid fa-trash"></i>
                        </button>
                    </td>
                `;
                tableBody.appendChild(row);
            });
        } catch (error) {
            console.error('Erro ao carregar talhões:', error);
            alert('Erro ao carregar dados.');
        }
    }

    if (btnAdd) {
        btnAdd.removeAttribute('disabled');
        btnAdd.title = '';
        btnAdd.addEventListener('click', async function () {
            await loadPropriedades();
            talForm.reset();
            document.getElementById('talhaoId').value = '0';
            document.getElementById('talhaoModalLabel').textContent = 'Novo Talhão';
            talModal.show();
        });
    }

    talForm.addEventListener('submit', async function (e) {
        e.preventDefault();
        const id = parseInt(document.getElementById('talhaoId').value);
        const formData = {
            id: id,
            nome: document.getElementById('talNome').value,
            area: parseFloat(document.getElementById('talArea').value),
            propriedadeId: parseInt(document.getElementById('talProp').value),
            descricao: document.getElementById('talDesc').value
        };

        const url = id === 0 ? '/Talhao/Create' : '/Talhao/Update';
        try {
            const response = await fetch(url, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(formData)
            });
            const result = await response.json();
            if (result.success) {
                alert(result.message);
                talModal.hide();
                loadTalhoes();
            } else {
                alert('Erro: ' + result.message);
            }
        } catch (error) {
            console.error('Erro ao salvar talhão:', error);
            alert('Erro ao processar requisição.');
        }
    });

    tableBody.addEventListener('click', function (e) {
        const btn = e.target.closest('button');
        if (!btn) return;

        const id = parseInt(btn.getAttribute('data-id'));

        if (btn.classList.contains('btn-edit')) {
            openEditModal(id);
        } else if (btn.classList.contains('btn-delete')) {
                if (confirm('Tem certeza que deseja excluir este talhão?')) {
                    deleteTalhao(id);
                }
        }
    });

    async function openEditModal(id) {
        try {
            await loadPropriedades();
            const response = await fetch('/Talhao/GetList');
            const data = await response.json();
            const item = data.find(i => i.id === id);

            if (item) {
                document.getElementById('talhaoId').value = item.id;
                document.getElementById('talNome').value = item.nome;
                document.getElementById('talArea').value = item.area;
                document.getElementById('talProp').value = item.propriedadeId;
                document.getElementById('talDesc').value = item.descricao || '';
                document.getElementById('talhaoModalLabel').textContent = 'Editar Talhão';
                talModal.show();
            }
        } catch (error) {
            console.error('Erro ao abrir modal de edição:', error);
        }
    }

    async function deleteTalhao(id) {
        try {
            const response = await fetch(`/Talhao/Delete?id=${id}`, { method: 'POST' });
            const result = await response.json();
            if (result.success) {
                alert(result.message);
                loadTalhoes();
            } else {
                alert('Erro: ' + result.message);
            }
        } catch (error) {
            console.error('Erro ao deletar talhão:', error);
            alert('Erro ao processar requisição.');
        }
    }

    loadTalhoes();
});
