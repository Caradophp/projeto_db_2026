document.addEventListener('DOMContentLoaded', function () {
    const cultModal = new bootstrap.Modal(document.getElementById('culturaModal'));
    const cultForm = document.getElementById('culturaForm');
    const tableBody = document.querySelector('.management-table tbody');
    const btnAdd = document.querySelector('.management-toolbar .btn-primary');

    async function loadCulturas() {
        try {
            const response = await fetch('/Cultura/GetList');
            const data = await response.json();

            if (data.length === 0) {
                tableBody.innerHTML = `
                    <tr>
                        <td colspan="4" class="management-empty">
                            <span class="management-empty-icon" aria-hidden="true"><i class="fa-regular fa-rectangle-list"></i></span>
                            <strong>Nenhum registro encontrado</strong>
                            <span>As culturas aparecerão aqui.</span>
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
            console.error('Erro ao carregar culturas:', error);
            alert('Erro ao carregar dados.');
        }
    }

    if (btnAdd) {
        btnAdd.removeAttribute('disabled');
        btnAdd.title = '';
        btnAdd.addEventListener('click', function () {
            cultForm.reset();
            document.getElementById('culturaId').value = '0';
            document.getElementById('culturaModalLabel').textContent = 'Nova Cultura';
            cultModal.show();
        });
    }

    cultForm.addEventListener('submit', async function (e) {
        e.preventDefault();
        const id = parseInt(document.getElementById('culturaId').value);
        const formData = {
            id: id,
            nome: document.getElementById('cultNome').value,
            descricao: document.getElementById('cultDesc').value
        };

        const url = id === 0 ? '/Cultura/Create' : '/Cultura/Update';
        try {
            const response = await fetch(url, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(formData)
            });
            const result = await response.json();
            if (result.success) {
                alert(result.message);
                cultModal.hide();
                loadCulturas();
            } else {
                alert('Erro: ' + result.message);
            }
        } catch (error) {
            console.error('Erro ao salvar cultura:', error);
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
                if (confirm('Tem certeza que deseja excluir esta cultura?')) {
                    deleteCultura(id);
                }
        }
    });

    async function openEditModal(id) {
        try {
            const response = await fetch('/Cultura/GetList');
            const data = await response.json();
            const item = data.find(i => i.id === id);

            if (item) {
                document.getElementById('culturaId').value = item.id;
                document.getElementById('cultNome').value = item.nome;
                document.getElementById('cultDesc').value = item.descricao || '';
                document.getElementById('culturaModalLabel').textContent = 'Editar Cultura';
                cultModal.show();
            }
        } catch (error) {
            console.error('Erro ao abrir modal de edição:', error);
        }
    }

    async function deleteCultura(id) {
        try {
            const response = await fetch(`/Cultura/Delete?id=${id}`, { method: 'POST' });
            const result = await response.json();
            if (result.success) {
                alert(result.message);
                loadCulturas();
            } else {
                alert('Erro: ' + result.message);
            }
        } catch (error) {
            console.error('Erro ao deletar cultura:', error);
            alert('Erro ao processar requisição.');
        }
    }

    loadCulturas();
});
