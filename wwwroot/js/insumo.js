document.addEventListener('DOMContentLoaded', function () {
    const insumoModal = new bootstrap.Modal(document.getElementById('insumoModal'));
    const insumoForm = document.getElementById('insumoForm');
    const tableBody = document.querySelector('.management-table tbody');
    const btnAdd = document.querySelector('.management-toolbar .btn-primary');

    // Populate table
    async function loadInsumos() {
        try {
            const response = await fetch('/Insumo/GetList');
            const data = await response.json();

            if (data.length === 0) {
                tableBody.innerHTML = `
                    <tr>
                        <td colspan="6" class="management-empty">
                            <span class="management-empty-icon" aria-hidden="true"><i class="fa-regular fa-rectangle-list"></i></span>
                            <strong>Nenhum registro encontrado</strong>
                            <span>Os registros de insumos aparecerão aqui.</span>
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
                    <td>${item.tipo || '-'}</td>
                    <td>${item.unidadeMedida}</td>
                    <td>${item.estoque}</td>
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
            console.error('Erro ao carregar insumos:', error);
            alert('Erro ao carregar dados.');
        }
    }

    // Handle Add button
    if (btnAdd) {
        btnAdd.removeAttribute('disabled');
        btnAdd.title = '';
        btnAdd.addEventListener('click', function () {
            insumoForm.reset();
            document.getElementById('insumoId').value = '0';
            document.getElementById('insumoModalLabel').textContent = 'Novo Insumo';
            insumoModal.show();
        });
    }

    // Handle Form Submit
    insumoForm.addEventListener('submit', async function (e) {
        e.preventDefault();
        const id = parseInt(document.getElementById('insumoId').value);
        const formData = {
            id: id,
            nome: document.getElementById('insumoNome').value,
            tipo: document.getElementById('insumoTipo').value,
            unidadeMedida: document.getElementById('insumoUnidade').value,
            estoque: parseFloat(document.getElementById('insumoEstoque').value)
        };

        const url = id === 0 ? '/Insumo/Create' : '/Insumo/Update';
        try {
            const response = await fetch(url, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(formData)
            });
            const result = await response.json();
            if (result.success) {
                alert(result.message);
                insumoModal.hide();
                loadInsumos();
            } else {
                alert('Erro: ' + result.message);
            }
        } catch (error) {
            console.error('Erro ao salvar insumo:', error);
            alert('Erro ao processar requisição.');
        }
    });

    // Handle Edit and Delete buttons (delegation)
    tableBody.addEventListener('click', function (e) {
        const btn = e.target.closest('button');
        if (!btn) return;

        const id = parseInt(btn.getAttribute('data-id'));

        if (btn.classList.contains('btn-edit')) {
            openEditModal(id);
        } else if (btn.classList.contains('btn-delete')) {
                if (confirm('Tem certeza que deseja excluir este insumo?')) {
                    deleteInsumo(id);
                }
        }
    });

    async function openEditModal(id) {
        try {
            const response = await fetch('/Insumo/GetList'); // Simple way: find in current list or call GetById if implemented
            const data = await response.json();
            const item = data.find(i => i.id === id);

            if (item) {
                document.getElementById('insumoId').value = item.id;
                document.getElementById('insumoNome').value = item.nome;
                document.getElementById('insumoTipo').value = item.tipo || '';
                document.getElementById('insumoUnidade').value = item.unidadeMedida;
                document.getElementById('insumoEstoque').value = item.estoque;
                document.getElementById('insumoModalLabel').textContent = 'Editar Insumo';
                insumoModal.show();
            }
        } catch (error) {
            console.error('Erro ao abrir modal de edição:', error);
        }
    }

    async function deleteInsumo(id) {
        try {
            const response = await fetch(`/Insumo/Delete?id=${id}`, { method: 'POST' });
            const result = await response.json();
            if (result.success) {
                alert(result.message);
                loadInsumos();
            } else {
                alert('Erro: ' + result.message);
            }
        } catch (error) {
            console.error('Erro ao deletar insumo:', error);
            alert('Erro ao processar requisição.');
        }
    }

    loadInsumos();
});
