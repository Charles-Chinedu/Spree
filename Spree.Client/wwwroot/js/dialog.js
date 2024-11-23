window.ShowDialog = function () {
    document.getElementById("my-dialog").showModal()
}

window.HideDialog = function () {
    var dialog = document.getElementById("my-dialog");
    if (dialog) {
        dialog.close();
    }

}
