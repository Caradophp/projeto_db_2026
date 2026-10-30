// ============================================================
// CULTIVA# - COMPONENTES GLOBAIS
// ============================================================

window.CultivaAlert = (() => {

    function createModal() {
        let overlay = document.getElementById('cultivaAlertOverlay');

        if (overlay) {
            return overlay;
        }

        overlay = document.createElement('div');
        overlay.id = 'cultivaAlertOverlay';
        overlay.className = 'cultiva-alert-overlay';

        overlay.innerHTML = `
            <div class="cultiva-alert-modal" role="dialog" aria-modal="true">

                <div class="cultiva-alert-icon" id="cultivaAlertIcon">
                    <i class="fa-solid fa-circle-info"></i>
                </div>

                <div class="cultiva-alert-content">
                    <h2 id="cultivaAlertTitle">Aviso</h2>
                    <p id="cultivaAlertMessage"></p>
                </div>

                <div class="cultiva-alert-actions" id="cultivaAlertActions"></div>

            </div>
        `;

        document.body.appendChild(overlay);

        return overlay;
    }


    function getElements() {
        const overlay = createModal();

        return {
            overlay,
            modal: overlay.querySelector('.cultiva-alert-modal'),
            icon: overlay.querySelector('#cultivaAlertIcon'),
            title: overlay.querySelector('#cultivaAlertTitle'),
            message: overlay.querySelector('#cultivaAlertMessage'),
            actions: overlay.querySelector('#cultivaAlertActions')
        };
    }


    function setType(elements, type) {
        elements.modal.classList.remove(
            'cultiva-alert-success',
            'cultiva-alert-error',
            'cultiva-alert-warning',
            'cultiva-alert-info',
            'cultiva-alert-danger'
        );

        elements.modal.classList.add(`cultiva-alert-${type}`);

        const icons = {
            success: 'fa-circle-check',
            error: 'fa-circle-xmark',
            warning: 'fa-triangle-exclamation',
            info: 'fa-circle-info',
            danger: 'fa-trash-can'
        };

        elements.icon.innerHTML =
            `<i class="fa-solid ${icons[type] || icons.info}"></i>`;
    }


    function open(elements) {
        document.body.classList.add('cultiva-alert-open');

        requestAnimationFrame(() => {
            elements.overlay.classList.add('show');
        });
    }


    function close(elements) {
        elements.overlay.classList.remove('show');
        document.body.classList.remove('cultiva-alert-open');
    }


    function message({
        title = 'Aviso',
        message = '',
        type = 'info',
        buttonText = 'Entendi'
    }) {
        return new Promise(resolve => {

            const elements = getElements();

            setType(elements, type);

            elements.title.textContent = title;
            elements.message.textContent = message;

            elements.actions.innerHTML = '';

            const button = document.createElement('button');
            button.type = 'button';
            button.className = 'cultiva-alert-button primary';
            button.textContent = buttonText;

            elements.actions.appendChild(button);

            const finish = () => {
                close(elements);

                setTimeout(() => {
                    resolve(true);
                }, 180);
            };

            button.onclick = finish;

            open(elements);

            setTimeout(() => button.focus(), 50);
        });
    }


    function confirm({
        title = 'Confirmar ação',
        message = 'Deseja continuar?',
        confirmText = 'Confirmar',
        cancelText = 'Cancelar',
        type = 'warning'
    }) {
        return new Promise(resolve => {

            const elements = getElements();

            setType(elements, type);

            elements.title.textContent = title;
            elements.message.textContent = message;

            elements.actions.innerHTML = '';

            const cancelButton = document.createElement('button');
            cancelButton.type = 'button';
            cancelButton.className = 'cultiva-alert-button secondary';
            cancelButton.textContent = cancelText;

            const confirmButton = document.createElement('button');
            confirmButton.type = 'button';
            confirmButton.className =
                type === 'danger'
                    ? 'cultiva-alert-button danger'
                    : 'cultiva-alert-button primary';

            confirmButton.textContent = confirmText;

            elements.actions.appendChild(cancelButton);
            elements.actions.appendChild(confirmButton);

            const finish = value => {
                close(elements);

                setTimeout(() => {
                    resolve(value);
                }, 180);
            };

            cancelButton.onclick = () => finish(false);
            confirmButton.onclick = () => finish(true);

            open(elements);

            setTimeout(() => cancelButton.focus(), 50);
        });
    }


    return {

        success(message, title = 'Tudo certo!') {
            return messageAlert(title, message, 'success');
        },

        error(message, title = 'Algo deu errado') {
            return messageAlert(title, message, 'error');
        },

        warning(message, title = 'Atenção') {
            return messageAlert(title, message, 'warning');
        },

        info(message, title = 'Informação') {
            return messageAlert(title, message, 'info');
        },

        confirm,

        delete(title, message) {
            return confirm({
                title,
                message,
                type: 'danger',
                confirmText: 'Excluir',
                cancelText: 'Cancelar'
            });
        }
    };


    function messageAlert(title, text, type) {
        return message({
            title,
            message: text,
            type
        });
    }

})();