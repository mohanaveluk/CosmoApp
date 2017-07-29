function openModal() {
    document.getElementById('modal-process').style.display = 'block';
    document.getElementById('fade-process').style.display = 'block';
}

function closeModal() {
    document.getElementById('modal-process').style.display = 'none';
    document.getElementById('fade-process').style.display = 'none';
}
$(function () {
    $('[data-toggle="tooltip"]').tooltip();
});
