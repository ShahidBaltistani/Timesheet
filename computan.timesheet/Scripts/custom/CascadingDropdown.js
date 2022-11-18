//Cascading dropdown
$(document).ready(function() {
    $("#CountryId").change(function() {
        var t = $("#CountryId").val();
        $.get("/UsersAdmin/GetStates",
            { CountryId: t },
            function(data) {
                $("#StateId").empty();
                $.each(data,
                    function(index, row) {
                        $("#StateId").append("<option value='" + row.id + "'>" + row.name + "</option>");
                    });
            });
    });
})