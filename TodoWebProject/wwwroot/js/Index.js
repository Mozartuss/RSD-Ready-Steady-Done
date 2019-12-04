
var Delete = function (id) {
    $("#deleteDialog").modal("show");
    $("#confirm").click(function () {
        $.ajax({
            type: "POST",
            url: `/Todo/Delete/${id}`,
            success: function (result) {
                $("#partial").html(result);
                $('#deleteDialog').modal('hide');
            }
        });
    });
};

var Check = function (id) {
    $("#check").modal("show");
    $("#confirmCheck").click(function () {
        $.ajax({
            type: "POST",
            url: `/Todo/Check/${id}`,
            success: function (result) {
                $("#partial").html(result);
                $('#check').modal('hide');
            }
        });
    });
};

var Uncheck = function (id) {
    $("#uncheck").modal("show");
    $("#confirmUncheck").click(function () {
        $.ajax({
            type: "POST",
            url: `/Todo/Uncheck/${id}`,
            success: function (result) {
                $("#partial").html(result);
                $('#uncheck').modal('hide');
            }
        });
    });
};

var Sort = function (sO, pS) {
    $.ajax({
        type: "POST",
        url: `/Todo/LoadTodoList/? pageSize=${pS}&sortOrder=${sO}`,
        success: function (result) {
            $("#partial").html(result);
        }
    });
};

var Filter = function (sO, pS, cF, pN, sC) {

    var sSV = $('#searchString').val();
    if (sC) {
        sSV = "";
    }

    var sort = sO == "null" || sO == null ? '' : `sortOrder=${sO}&`;
    var filter = cF == "null" || cF == null ? '' : `currentFilter=${cF}&`;
    var pageNum = pN == 0 || pN == null ? '' : `pageNumber=${pN}&`
    var search = !sSV.trim() || sSV == 'null' || sSV == null ? '' : `searchString=${sSV}&`

    $.ajax({
        type: "POST",
        url: `/Todo/LoadTodoList/?${sort}${search}${filter}${pageNum}pageSize=${pS}`,
        success: function (result) {
            $("#partial").html(result);
            highlight(sSV.toLowerCase());
            if ($('#searchString').val().length > 0) {
                $('#clear').addClass('clear-button-width');
                $('#clear').removeClass('zerowidth');
            }
        }
    });
}

var Search = function () {

    var rows = $(".table").find("tbody tr:not(.notthingShow)");
    var sT = $('#searchString').val();

    if (sT.length > 0) {
        $('.notthingShow').show();
        $('#clear').addClass('clear-button-width');
        $('#clear').removeClass('zerowidth');

        rows.removeClass("match").hide().filter(function () {
            var match = false;
            $(this).find("td.filter").each(function () {
                var indexOf = $(this).text().toLowerCase().indexOf(sT.toLowerCase());
                if (indexOf !== -1) {
                    match = true;
                    $('.notthingShow').hide();
                    return;
                }
            });
            return match;
        }).addClass("match").show();
    } else {
        $('#clear').addClass('zerowidth');
        $('#clear').removeClass('clear-button-width');
        rows.removeClass("match").show().find("b").contents().unwrap();
    }
    highlight(sT.toLowerCase());
}

var highlight = function (string) {
    $(".table").find("tbody tr td.filter").each(function () {
        var valLow = $(this).text().toLowerCase();
        var val = $(this).text();
        if (valLow.indexOf(string) === -1) return;

        var matchStartIndex = valLow.indexOf(string);
        var matchEndIndex = matchStartIndex + string.length - 1;

        var beforeMatch = val.slice(0, matchStartIndex);
        var matchText = val.slice(matchStartIndex, matchEndIndex + 1);
        var afterMatch = val.slice(matchEndIndex + 1);

        $(this).html(beforeMatch + "<b style='color:#0062cc' >" + matchText + "</b>" + afterMatch);
    });
};







