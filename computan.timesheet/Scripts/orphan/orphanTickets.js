$(document).ready(function() {
    $("#TeamId").select2({ width: 200 });
    $("#StatusId").select2({ width: 200 });
    $("#AgeId").select2({ width: 200 });

});

//$(document).on('click', '#suppressBtn', function () {
//    var id = $(this).data('id');
//    var tr = $(this).closest("tr");
//    swal({
//        title: "Are you sure!",
//        text: "Do you want to Modify this?",
//        showCancelButton: true,
//        confirmButtonColor: "#d33",
//        closeOnConfirm: false,
//        animation: "slide-from-top",
//    },
//        function (inputValue) {
//            if (inputValue === true) {
//                $.ajax({
//                    url: "/Orphan/SupressTicket/" + id,
//                    type: 'GET',
//                    success: function (response) {
//                        $(".orphanDetail").text(response);
//                        swal({
//                            title: response,
//                            text: "Your data has been " + response,
//                            type: "success",
//                            confirmButtonColor: "#2196F3"
//                        });
//                        tr.remove();
//                    },
//                    error: function () {
//                        swal({
//                            title: "Oops...",
//                            text: "Something went wrong!",
//                            confirmButtonColor: "#EF5350",
//                            type: "error"
//                        });
//                    }
//                });
//            }
//        });
//});
jQuery(document).on("click",
    "#suppressBtn",
    function() {
        var obj = $(this);
        var id = $(this).attr("data-id");
        $.post("/Orphan/SuppressTicket/" + id,
            function(data) {
                if (data.error) {
                    new PNotify({
                        title: "Warning!",
                        text: "Something went wrong please try again",
                        type: "warning"
                    });
                } else {
                    $(".orphanDetail").text(data.message);
                    new PNotify({
                        title: "Success",
                        text: "Ticket " + data.text +" Sccessfully",
                        type: "success"
                    });
                    $(obj).closest("tr").remove();;
                }
                return false;
            });
        return false;
    });