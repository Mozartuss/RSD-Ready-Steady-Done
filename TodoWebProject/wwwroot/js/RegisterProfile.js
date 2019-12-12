$(document).ready(function () {
    $('#myPassword').password({
        field: '#FirstName',
        filed: '#LastName',
        minimumLength: 6,
        fieldPartialMatch: true,
        animate: true,
    });
});


function readURL(input) {
    if (input.files && input.files[0]) {
        const reader = new FileReader();
        reader.onload = function (e) {
            $('#image_upload_preview').attr('src', e.target.result);
        };
        reader.readAsDataURL(input.files[0]);
    }
}


function reqiredOldConfirm() {
    var ip = $("#myPassword").val;
    if (ip != null) {
        $("#PasswordConfirmation").prop("required", true);
        $("#CurrentPassword").prop("required", true);
    }
}

