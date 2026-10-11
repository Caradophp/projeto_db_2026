document.addEventListener('DOMContentLoaded', function () {
    const propModal = new bootstrap.Modal(document.getElementById('propriedadeModal'));
    const propForm = document.getElementById('propriedadeForm');
    const tableBody = document.querySelector('.management-table tbody');
    const btnAdd = document.querySelector('.management-toolbar .btn-primary');

    async function loadPropriedades() {
        try {
            const response = await fetch('/Propriedade/GetList');
            const data = await response.json();

            if (data.length === 0) {
                tableBody.innerHTML = `
                    <tr>
                        <td colspan="5" class="management-empty">
                            <span class="management-empty-icon" aria-hidden="true"><i class="fa-regular fa-rectangle-list"></i></span>
                            <strong>Nenhum registro encontrado</strong>
                            <span>As propriedades aparecerão aqui.</span>
                        </td>
                    </tr>`;
                return;
            }

            tableBody.innerHTML = '';
            data.forEach(item => {
                const row = document.createElement('tr');
                row.innerHTML = `
                    <td>${item.id}</td>
                    <td>${item.nome}</td>
                    <td>${item.areaTotal}</td>
                    <td>${item.userId}</td>
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
            console.error('Erro ao carregar propriedades:', error);
            alert('Erro ao carregar dados.');
        }
    }

    if (btnAdd) {
        btnAdd.removeAttribute('disabled');
        btnAdd.title = '';
        btnAdd.addEventListener('click', function () {
            propForm.reset();
            document.getElementById('propriedadeId').value = '0';
            document.getElementById('propriedadeModalLabel').textContent = 'Nova Propriedade';
            propModal.show();
        });
    }

    propForm.addEventListener('submit', async function (e) {
        e.preventDefault();
        const id = parseInt(document.getElementById('propriedadeId').value);
        const formData = {
            id: id,
            nome: document.getElementById('propNome').value,
            areaTotal: parseFloat(document.getElementById('propArea').value),
            userId: parseInt(document.getElementById('propUser').value)
        };

        const url = id === 0 ? '/Propriedade/Create' : '/Propriedade/Update';
        try {
            const response = await fetch(url, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(formData)
            });
            const result = await response.json();
            if (result.success) {
                alert(result.message);
                propModal.hide();
                loadPropriedades();
            } else {
                alert('Erro: ' + result.message);
            }
        } catch (error) {
            console.error('Erro ao salvar propriedade:', error);
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
                if (confirm('Tem certeza que deseja excluir esta propriedade?')) {
                    deletePropriedade(id);
                }
        }
    });

    async function openEditModal(id) {
        try {
            const response = await fetch('/Propriedade/GetList');
            const data = await response.json();
            const item = data.find(i => i.id === id);

            if (item) {
                document.getElementById('propriedadeId').value = item.id;
                document.getElementById('propNome').value = item.nome;
                document.getElementById('propArea').value = item.areaTotal;
                document.getElementById('propUser').value = item.userId;
                document.getElementById('propriedadeModalLabel').textContent = 'Editar Propriedade';
                propModal.show();
            }
        } catch (error) {
            console.error('Erro ao abrir modal de edição:', error);
        }
    }

    async function deletePropriedade(id) {
        try {
            const response = await fetch(`/Propriedade/Delete?id=${id}`, { method: 'POST' });
            const result = await response.json();
            if (result.success) {
                alert(result.message);
                loadPropriedades();
            } else {
                alert('Erro: ' + result.message);
            }
        } catch (error) {
            console.error('Erro ao deletar propriedade:', error);
            alert('Erro ao processar requisição.');
        }
    }

    loadPropriedades();
});
