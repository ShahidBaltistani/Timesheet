$(function() {
    $('[data-toggle="tooltip"]').tooltip();
    autosize(document.querySelectorAll("textarea"));
    $(document).on("click",
        ".topbucketbtn",
        function(event) {
            event.preventDefault();
            $.ajax({
                url: "/GlobalBuckets/BucketSearchFilter/",
                type: "Get",
                success: function(data) {
                    $(".BucketSearchFilter").html(data);
                }
            });
            $.ajax({
                url: "/GlobalBuckets/Index/",
                type: "Get",
                success: function(data) {
                    $(".GlobalBucketslist").html(data);
                }
            });
            $("#topbucket").addClass("open");
        });
    $(".globalbucketform").submit(function(event) {
        event.preventDefault();
        return false;
    });
    $("button.close").on("click",
        function() {
            $("#topbucket").removeClass("open");
        });
    $(document).on("click",
        ".deletetimelogGobal",
        function() {
            var href = $(this).attr("href");
            var rowscount = $("div.bucketitemrow").length;
            var tr = $(this).closest("div.bucketitemrow");
            swal({
                    title: "Delete Bucket?",
                    text: "Are you sure, you want to delete this bucket ?",
                    type: "error",
                    showCancelButton: true,
                    closeOnConfirm: false,
                    confirmButtonColor: "#2196F3",
                    showLoaderOnConfirm: true
                },
                function() {
                    $.ajax({
                        url: href,
                        data: "",
                        type: "GET",
                    }).done(function(data) {
                        if (data.error) {
                            swal({
                                title: data.error,
                                confirmButtonColor: "#2196F3",
                                type: "error"
                            });
                            return false;
                        }
                        // remove selected log from the list.
                        $(tr).remove();
                        var counter = $(".bucktcount").html();
                        $(".bucktcount").html((parseInt(counter)) - 1);
                        if (rowscount == 1) {
                            var html = "<center><span><b>Sorry, no task found in your bucket</b></span></center>";
                            $(".GlobalBucketslist").prepend(html);
                            $(".savebtn").remove();
                        }
                        swal({
                            title: "Your Bucket item has been deleted successfully!",
                            confirmButtonColor: "#2196F3",
                            type: "success"
                        });
                        var globalbucketdate = $("#GlobalStartDate").val();
                        var bucketdate = $("#StartDate").val();
                        if ($("#StartDate").length != 0) {
                            if (bucketdate == globalbucketdate) {
                                updatebuckets(bucketdate);
                            }
                        }
                    });
                });
            return false;
        });
    $(document).on("click",
        ".globaladdtimeinbucketbtn",
        function() {
            var model = [];
            var trs = [];
            var btn = $(this);
            $("div.bucketitemrow").each(function() {
                var id = $(this).find(".id").val();
                var bucketdescription = $(this).find(".bucketdescription").val();
                var timespent = $(this).find(".timespent").val();
                var billtime = $(this).find(".timebill").val();
                if (timespent != "") {
                    trs.push($(this));
                }
                var SearchResults = {
                    'id': id,
                    'description': bucketdescription,
                    'timespent': timespent,
                    'billtime': billtime,
                };
                model.push(SearchResults);
            });
            $.ajax({
                url: "/tickettimelogs/Addbuckettime/",
                type: "POST",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(model),
                success: function(result) {
                    if (!result.error) {
                        $(trs).each(function() {
                            $(this).remove();
                        });
                        swal("Done", "Time added Succesfully.", "success");
                        var count = $("div.bucketitemrow");
                        $(".bucktcount").html(count.length);
                        if (count.length == 0) {
                            btn.remove();
                            $(".GlobalBucketslist")
                                .append("<center><span><b>Sorry, no task found in your bucket</b></span></center>");
                        }
                        var globalbucketdate = $("#GlobalStartDate").val();
                        var bucketdate = $("#StartDate").val();
                        if ($("#StartDate").length != 0) {
                            if (bucketdate == globalbucketdate) {
                                updatebuckets(bucketdate);
                            }
                        }
                    } else {
                        swal("Error", result.errortext, "error");
                    }
                },
                error: function(result) {
                    alert("Failed");
                }
            });
            return false;
        });
    $(document).on("click",
        ".globalsearchbtn",
        function() {
            var StartDate = $("#GlobalStartDate").val();
            $.ajax({
                url: "/GlobalBuckets/MyBucketsGlobal/",
                data: { StartDate: StartDate },
                type: "GET",
            }).done(function(data) {
                if (data.error) {
                    return false;
                }
                $(".GlobalBucketslist").html(data);
                var trcount = $("div.bucketitemrow");
                trcount = trcount.length;
                $(".bucktcount").html(trcount);
                return false;
            });
        });
});

function updatebuckets(bucketdate) {
    $.ajax({
        url: "/tickettimelogs/UpdateMyBuckets/",
        data: { StartDate: bucketdate },
        type: "GET",
    }).done(function(data) {
        if (data.error) {
        }
        $(".mybuckets").html(data);
        return false;
    });
}

function formatDate(fullDate) {
    var day = leftPad(fullDate.getDate(), 2);
    var twoDigitMonth = ((fullDate.getMonth().length + 1) === 1)
        ? (fullDate.getMonth() + 1)
        : "0" + (fullDate.getMonth() + 1);
    var currentDate = twoDigitMonth + "/" + day + "/" + fullDate.getFullYear();
    return currentDate;
}