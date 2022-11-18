var body = document.body;
body.classList.add("has-detached-left");
//Customize Orphan Age
var id = "";
var orphanAge = "";
$(document).on("click",
    "#editAgeModal",
    function() {
        id = $(this).data("id");
        orphanAge = $(this).closest("tr").find("td:eq(1)").text().replace(/\s/g, "");
        $(".editAge").val(orphanAge);
        $("#EditOrphanAgeModal").modal("show");
    });
$(".submitStatus").click(function() {
    if ($(".editAge").val() == "") {
        swal({
            title: "Oops...",
            text: "Please fill the required field!",
            confirmButtonColor: "#EF5350",
            type: "warning"
        });
        return;
    }
    $("#EditOrphanAgeModal").modal("hide");
    $.ajax({
        url: "/Settings/EditOrphanAge",
        data: {
            id: id,
            OrphanAge: $(".editAge").val()
        },
        type: "GET",
    }).done(function(data) {
        window.location.reload();
    });
});
//Subscribe Orphan Teams
$(document).on("click",
    "#subscribeTeam",
    function() {
        var teamIds = $(this).data("id");
        var teamIds = $(this).data(val);
        $.ajax({
            url: "/Settings/SubscribeTeam/" + teamIds,
            type: "GET",
            data: { teamIds: teamIds },
            success: function(response) {
                swal({
                    title: response,
                    text: "You " + response + " the team successfully",
                    type: "success",
                    confirmButtonColor: "#2196F3"
                });
            },
            error: function() {
                swal({
                    title: "Oops...",
                    text: "Something went wrong!",
                    confirmButtonColor: "#EF5350",
                    type: "error"
                });
            }
        });
    });

$(document).on("click",
    "#twoFACheckBox",
    function () {
        
        var values = $(this).data("id");
        $.ajax({
            url: "/Settings/ChangeTwoFA/" + values,
            type: "GET",
            data: { values: values },
            success: function (response) {
                swal({
                    title: response,
                    text: "Changed successfully",
                    type: "success",
                    confirmButtonColor: "#2196F3"
                });

            },
            error: function () {
                swal({
                    title: "Oops...",
                    text: "Something went wrong!",
                    confirmButtonColor: "#EF5350",
                    type: "error"
                });
            }
        });
    });