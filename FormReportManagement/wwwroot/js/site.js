// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
async function loadForms() {
    const response = await fetch('/Form/GetForms');
    const forms = await response.json();
    console .log(forms);
}
async function loadFormsFromApi() {
    const response = await fetch('/api/forms');
    const data = await response.json();
    document .getElementById('apiResult').textContent = JSON.stringify(data, null, 2);
}
function exportData() {

    const format = document.getElementById("exportFormat").value;

    if (format === "pdf") {
        window.location.href = "/api/forms/export/pdf";
    }
    else if (format === "excel") {
        window.location.href = "/api/forms/export/excel";
    }
    else if (format === "csv") {
        window.location.href = "/api/forms/export/csv";
    }

    // Reset dropdown
    document.getElementById("exportFormat").value = "";
}