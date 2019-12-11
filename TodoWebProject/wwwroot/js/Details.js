var openDetailsModal = function (id) {

    $.get({
        url: `/Todo/Details/${id}`,
        dataType: "html",
        error: function (jqXHR, textStatus, errorThrown) {
            alert(textStatus + ": Couldn't add form. " + errorThrown);
        },
        success: function (data) {
            $('#modal-placeholder').html(data);
            $('#modal-placeholder > #detailsModal').modal('show');
        }
    });

};