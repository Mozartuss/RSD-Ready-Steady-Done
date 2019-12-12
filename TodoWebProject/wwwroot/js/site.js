var openProfileModal = function (id) {
    $.get({
        url: `/Account/Profile/${id}`,
        dataType: "html",
        error: function (jqXHR, textStatus, errorThrown) {
            alert(textStatus + ": Couldn't add form. " + errorThrown);
        },
        success: function (data) {
            $('#modal-placeholder').html(data);
            $('#modal-placeholder > #profileModal').modal('show');
        }
    });
};