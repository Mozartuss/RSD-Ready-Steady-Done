function checkProvider() {
    var pro = document.getElementById("provider").value
    if (pro) {
        $("#PasswordConfirmation").prop("disabled", true).addClass("disabled");
        $("#CurrentPassword").prop("disabled", true).addClass("disabled");
        $("#myPassword").prop("disabled", true).addClass("disabled");
        $("#Email").prop("readonly", true);
    } else {

        $('#myPassword').password({
            field: '#FirstName',
            filed: '#LastName',
            minimumLength: 6,
            fieldPartialMatch: true,
            animate: true,
        });
    }
}


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
    var pw = $("#myPassword").val;
    if (pw != null) {
        $("#PasswordConfirmation").prop("required", true);
        $("#CurrentPassword").prop("required", true);
    }
}

async function UpdateProfile(oFormElement) {
    const d = new FormData(oFormElement);
    $.ajax({
        type: "post",
        url: "/Account/Profile",
        contentType: false,
        processData: false,
        data: d,
        success: function (result) {
            if (result.status === "failure") {
                $.each(result.formErrors, function () {
                    $(`#contactForm [data-valmsg-for="${this.key}"]`).html(this.errors.join());
                });
            }
            if (result.status === "success") {
                $('#profileModal').modal('hide');
            }
        }
    });
}
function checkInput(e) {
    var i = e.value;
    var x = $(e).attr("id");
    if (i.trim()) {
        $(`#${x}`).addClass("input");
    } else {
        $(`#${x}`).removeClass("input");
    }
};

