
const s_discountCodeEleId           = 'discount-code-input';
const s_sellPlateModalId            = 'sell-plate-modal';
const s_totalRevenueEleId           = 'total-revenue-text';
const s_totalRevenueContainerEleId  = 'total-revenue-container';
const s_errorModalBodyId            = 'error-modal-body'
const s_errorModalId                = 'error-modal'

function showErrorModal(message) {
    const errorModalBody = document.getElementById(s_errorModalBodyId);
    errorModalBody.textContent = message || "An unexpected error occurred.";

    const errorModal = new bootstrap.Modal(document.getElementById(s_errorModalId));

    errorModal.show();
}

function loadTotalRevenue() {
    const container = document.getElementById(s_totalRevenueContainerEleId);

    const revenueElement = document.getElementById(s_totalRevenueEleId);

    const url = `${VIEW_BAG.catalogBaseUrl}/plates/revenue`;

    fetch(url)
        .then(async response => {
            if (response.ok) {
                const data = await response.json();

                revenueElement.innerText = data.totalRevenue.toLocaleString();
                container.classList.remove('d-none');
            }
            else {
                const errorText = await response.text();

                showErrorModal(errorText);
            }
        });
}

function sellPlate(plateId, sellButtonId, reserveButtonId) {
    const discountCode = document.getElementById(s_discountCodeEleId).value

    const sellButton = document.getElementById(sellButtonId);
    const reserveButton = document.getElementById(reserveButtonId);

    const initialInnerText = sellButton.innerText;
    const initialReserveDisabled = reserveButton.disabled;

    sellButton.disabled = true;
    sellButton.innerText = "Selling...";

    reserveButton.disabled = true;

    const url = `${VIEW_BAG.catalogBaseUrl}/plates/sell/${plateId}`;

    fetch(url, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            discountCode: discountCode
        })
    })
        .then(async response => {
            if (response.ok) {
                sellButton.innerText = "Sold";
                loadTotalRevenue();

            } else {
                const errorText = await response.text();

                showErrorModal(errorText);

                sellButton.disabled = false;

                reserveButton.disabled = initialReserveDisabled;
                sellButton.innerText = initialInnerText;
            }
        });
}

function reservePlate(plateId, reserveButtonId) {
    const url = `${VIEW_BAG.catalogBaseUrl}/plates/reserve/${plateId}`;

    const button = document.getElementById(reserveButtonId);

    const initialInnerText = button.innerText;

    button.disabled = true;
    button.innerText = "Reserving...";

    fetch(url, { method: "POST" })
        .then(async response => {
            if (response.ok) {
                button.innerText = "Reserved";
            }
            else {
                const errorText = await response.text();

                showErrorModal(errorText);

                button.disabled = false;
                button.innerText = initialInnerText;
            }
        });
}

document.addEventListener('DOMContentLoaded', loadTotalRevenue);