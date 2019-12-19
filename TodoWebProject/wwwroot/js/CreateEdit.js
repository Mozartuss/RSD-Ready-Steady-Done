var openCreateModal = function () {
    $.get({
        url: "/Todo/Create",
        dataType: "html",
        error: function (jqXHR, textStatus, errorThrown) {
            alert(textStatus + ": Couldn't add form. " + errorThrown);
        },
        success: function (data) {
            $('#modal-placeholder').html(data);
            $('#modal-placeholder > #createModal').modal('show');
            var textarea = document.getElementById('description');
            charcountupdate(textarea.value);
            align_label();
        }
    });
};

var openEditModal = function (id) {
    $.get({
        url: `/Todo/Edit/${id}`,
        dataType: "html",
        error: function (jqXHR, textStatus, errorThrown) {
            alert(textStatus + ": Couldn't add form. " + errorThrown);
        },
        success: function (data) {
            $('#modal-placeholder').html(data);
            $('#modal-placeholder > #editModal').modal('show');
            var textarea = document.getElementById('description');
            charcountupdate(textarea.value);
            align_label();
        }
    });
};

/* Image Loader */
function readURL(input) {
    if (input.files && input.files[0]) {

        var reader = new FileReader();

        reader.onload = function (e) {
            var imagepath = e.target.result;
            $('.image-upload-wrap').hide();
            $('.file-upload-image').attr('src', imagepath);
            $('.file-upload-content').show();
            document.getElementById("emptyImage").value = "False";
        };
        reader.readAsDataURL(input.files[0]);
    } else {
        removeUpload();
    }
}
function removeUpload() {
    $('.file-upload-input').replaceWith($('.file-upload-input').clone());
    var fileImage = document.getElementById("fileImage");
    var emptyImage = document.getElementById("emptyImage");
    if (fileImage !== null && emptyImage.value === "False" ) {
        emptyImage.value = "True";
    };
    
    $('.file-upload-content').hide();
    $('.image-upload-wrap').show();
}

/*Text area count chars*/
function charcountupdate(str) {
    var lng = str.length + str.split('\n').length - 1;

    if (lng > 0) {
        $('#description').addClass('input');
    } else {
        $('#description').removeClass('input');
    };

    document.getElementById("charcount").innerHTML = lng + ' out of 1000 characters';
    resize();
}

/*Text area auto resize*/
function resize() {
    var textarea = document.getElementById('description'),
        hiddenDiv = document.createElement('div'),
        content = null;

    textarea.classList.add('txtstuff');

    hiddenDiv.classList.add('txta');
    hiddenDiv.style.display = 'none';
    hiddenDiv.style.whiteSpace = 'pre-wrap';
    hiddenDiv.style.wordWrap = 'break-word';


    (function (textarea) {
        textarea.parentNode.appendChild(hiddenDiv);
        content = textarea.value;
        content = content.replace(/\n/g, '<br>');
        hiddenDiv.innerHTML = content + '<br style="line-height: 30px;">';
        hiddenDiv.style.visibility = 'hidden';
        hiddenDiv.style.display = 'block';
        hiddenDiv.style.padding = '10px 10px 10px 5px';
        hiddenDiv.style.fontSize = '18px';
        hiddenDiv.style.width = '100%';
        textarea.style.height = hiddenDiv.offsetHeight + 'px';
        hiddenDiv.style.visibility = 'visible';
        hiddenDiv.style.display = 'none';
    })(textarea);

}

function align_label() {
    var i = $('select[name=align]').val();
    if (i != null) {
        $('select[name=align]').addClass('input');
        $('select[name=align]').removeClass('empty-input');

    } else {
        $('select[name=align]').removeClass('input');
        $('select[name=align]').addClass('empty-input');
    };
}

async function AddTask(oFormElement) {
    const d = new FormData(oFormElement);
    $.ajax({
        type: "post",
        url: "/Todo/Create",
        contentType: false,
        processData: false,
        data: d,
        success: function (result) {
            if (result.status === "failure") {
                $.each(result.formErrors, function () {
                    $(`#CreateForm [data-valmsg-for="${this.key}"]`).html(this.errors.join());
                });
            }
            if (result.status === "success") {
                sendNotification();
                $('#createModal').modal('hide');
                $("#partial").load("/Todo/LoadTodoList");
            }
        }
    });
}

async function UpdateTask(oFormElement) {
    const d = new FormData(oFormElement);
    $.ajax({
        type: "post",
        url: "/Todo/Edit",
        contentType: false,
        processData: false,
        data: d,
        success: function (result) {
            if (result.status === "failure") {
                $.each(result.formErrors, function () {
                    $(`#CreateForm [data-valmsg-for="${this.key}"]`).html(this.errors.join());
                });
            }
            if (result.status === "success") {
                $('#editModal').modal('hide');
                $("#partial").load("/Todo/LoadTodoList");
            }
        }
    });
}


