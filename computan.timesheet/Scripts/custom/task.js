jQuery(function() {
    // apply search filter.
    jQuery(".filterclients").livefilter({ selector: ".clientcollection li" });

    /*
 *   Search my task Feature.
 */
    jQuery(document).on("click",
        "#btnsearch",
        function() {
            var url = "/ticketitems/Searchtask";
            var statusid = $(this).attr("data-statusid");
            var searchstring = $("#searchtext").val();
            $("#searchvalue").val(searchstring);
            $("#pagenumber").val(0);
            $.ajax({
                url: url,
                data: {
                    searchstring: searchstring,
                    statusid: statusid,
                    pagenum: 0
                },
                type: "GET",
            }).done(function(data) {
                $("#mytaskrow").html(data.tasks);
                var count = $("#mytaskrow").find("tr").length;
                $("#" + statusid + ".my_task_status").html(data.totalcount);
                $(".presearchstring").removeClass("hidden");
                $(".checkall").prop("checked", false);
                if (searchstring) {
                    $(".searchstring").html(" <b>' " + searchstring + " '</b>");
                    $("#searchtext").val("");
                } else {
                    $(".presearchstring").addClass("hidden");
                }
                return false;
            });
            return false;
        });
    $(document).on("click",
        ".clearsearch",
        function() {
            if ($("#searchvalue").val() != "") {
                var id = $("#currentStatusId").val();
                $("#searchtext").val("");
                $(".searchstring").html("");
                $("#searchvalue").val("");
                $(".checkall").prop("checked", false);
                $(".presearchstring").addClass("hidden");
                $("#pagenumber").val(0);
                $("#" + id + ".mytaskstatuslink").click();
            }

            return false;
        });

    /************************************************************
    *** Enable/Disable all checkboxes
    *************************************************************/
    jQuery(document).on("click",
        ".checkall",
        function() {
            jQuery(".selecttask").prop("checked", this.checked);
        });

    jQuery(document).on("click",
        ".selecttask",
        function() {
            if (!this.checked) {
                $(".checkall").prop("checked", false);
            } else if ($(".selecttask").length == $(".selecttask:checked").length) {
                $(".checkall").prop("checked", true);
            }
        });

    /************************************************************
    *** Add multiple items to the bucket.
    *************************************************************/
    jQuery(document).on("click",
        ".addmultibucket",
        function() {
            $(".currentdate").hide();
            // Make sure at least one ticket is selected.
            if ($(".selecttask:checked").length == 0) {
                new PNotify({
                    title: "Warning!",
                    text: "Please select at least one ticket to add in the bucket!",
                    type: "warning"
                });
                return false;
            }

            // Show today's date.
            var currentDate = new Date();
            $("#bucketkdate").val(getFormattedDate(currentDate));

            // Open Popup for multi assignments.
            $("#btnaddtobucket").removeClass("savesinglebucket").addClass("savemultibucket");
            $("#addbucketmodel").modal("toggle");
            return false;
        });

    jQuery(document).on("click",
        ".savemultibucket",
        function() {
            // make sure user has provided a date.
            var workdate = $("#bucketkdate").val();
            if (workdate == "") {
                new PNotify({
                    title: "Error!",
                    text: "Please select a date to add tickets to the bucket.",
                    type: "error"
                });
                return false;
            }

            // Get all the selected team members.
            var userTasks = [];
            $(".selecttask:checked").each(function() {
                userTasks.push($(this).val());
            });
            //var userTasksCSV = userTasks.join(",");
            $.ajax({
                url: "/TicketTimeLogs/SaveMultipleBuckets/",
                data: {
                    bucketdate: workdate,
                    ticketlist: userTasks
                },
                type: "POST",
            }).done(function(data) {
                if (data.error) {
                    var source = $("#error-notification-template").html();
                    var template = Handlebars.compile(source);
                    var html = template(data);
                    $("#modelassignuseradmindiv").prepend(html);
                    return false;
                }
                // update Global my Buckets
                updateglobalbuckets();
                $(".selecttask:checked").prop("checked", false);
                $(".checkall").prop("checked", false);
                new PNotify({
                    title: "Success",
                    text: data.Successtext,
                    type: "success"
                });

                return false;
            });

            // Remove the popup.
            $("#addbucketmodel").modal("toggle");

            return false;
        });

    /************************************************************
    *** Add single item to the bucket.
    *************************************************************/
    //Add Bucket start
    var bpid; //product id for bucket
    var bsid; // skill id for bucket
    var btitle; // title bucket
    var bticketitems; //ticket item for bucket
    //  current date selction to bucket date
    $(document).on("click",
        ".currentdate",
        function() {
            var todaydate = new Date();
            todaydate = getFormattedDate(todaydate);
            $("#bucketkdate").val(todaydate);
            $("#bucketkdate_withproid").val(todaydate);
        });
    $(document).on("click",
        ".addbucketktime",
        function() {
            $(".currentdate").show();
            bpid = $(this).data("projectid");
            bsid = $(this).data("skillid");
            bticketitems = $(this).data("itemid");
            btitle = $(this).data("title");
            var emaildate = $(this).data("selecteddate");
            emaildate = new Date(emaildate);
            var currentDate = getFormattedDate(emaildate);
            $("#bucketkdate").val(currentDate);
            $("#bucketkdate_withproid").val(currentDate);
            if (bpid == "" || bsid == "") {
                $("#addbucketmodelwithprojectid").modal("show");
            } else {
                $("#btnaddtobucket").removeClass("savemultibucket").addClass("savesinglebucket");
                $("#addbucketmodel").modal("show");
            }
        });
    //****Add bucket btn end****
    $(document).on("click",
        ".savesinglebucket",
        function() {
            var workdate = $("#bucketkdate").val();
            if (workdate == "") {
                sweetAlert("Sorry...", "All fields having * are Required.", "error");
                return false;
            }
            $.ajax({
                url: "/TicketTimeLogs/addtime/",
                data: {
                    tcktitemid: bticketitems,
                    pid: bpid,
                    sid: bsid,
                    spenttime: 0,
                    billtime: 0,
                    workdate: workdate,
                    title: btitle,
                    description: "",
                    comments: ""
                },
                type: "POST",
            }).done(function(data) {
                if (data.error) {
                    var source = $("#error-notification-template").html();
                    var template = Handlebars.compile(source);
                    var html = template(data);
                    $("#modeladdtimebodydiv").prepend(html);
                    return false;
                }
                // update Global my Buckets
                updateglobalbuckets();
                new PNotify({
                    title: "Success",
                    text: "Time has been added successfully.",
                    type: "success"
                });
                $("#btnaddtobucket").removeClass("savemultibucket").addClass("savesinglebucket");
                $("#addbucketmodel").modal("toggle");
                //$("#" + tcktitemid + ".notimeadded").remove();
                //var source = $("#Add-time-after-template").html();
                //var template = Handlebars.compile(source);
                //var html = template(data);
                //$("#" + tcktitemid + ".addtimeafter").append(html);

                return false;
            });
        });
    $(document).on("click",
        ".addbucketwithproidpopupbtn",
        function() {
            bpid = $("#projectid_addbucket").val();
            bsid = $("#skillid_addbicket").val();
            var workdatewithproid = $("#bucketkdate_withproid").val();
            if (workdatewithproid == "" || bpid == "" || bsid == "") {
                sweetAlert("Sorry...", "All fields having * are Required.", "error");
                return false;
            }

            $.ajax({
                url: "/TicketTimeLogs/addtime/",
                data: {
                    tcktitemid: bticketitems,
                    pid: bpid,
                    sid: bsid,
                    spenttime: 0,
                    billtime: 0,
                    workdate: workdatewithproid,
                    title: btitle,
                    description: "",
                    comments: ""
                },
                type: "POST",
            }).done(function(data) {
                if (data.error) {
                    var source = $("#error-notification-template").html();
                    var template = Handlebars.compile(source);
                    var html = template(data);
                    $("#modeladdtimebodydiv").prepend(html);
                    return false;
                }
                new PNotify({
                    title: "Success",
                    text: "Time has been added successfully.",
                    type: "success"
                });
                // update Global my Buckets
                updateglobalbuckets();
                $("#addbucketmodelwithprojectid").modal("toggle");
                //$("#" + tcktitemid + ".notimeadded").remove();
                //var source = $("#Add-time-after-template").html();
                //var template = Handlebars.compile(source);
                //var html = template(data);
                //$("#" + tcktitemid + ".addtimeafter").append(html);

                return false;
            });
        });

    /************************************************************
    *** Change Status Actions based on type of status to change.
    *************************************************************/
    jQuery("input[name='statustyperadio']").change(function(e) {
        if ($(this).val() == "ticket") {
            jQuery(this).closest(".row").find(".ticketstatusaction").html(jQuery("#ticketstatuses").html());
        } else {
            jQuery(this).closest(".row").find(".ticketstatusaction").html(jQuery("#ticketitemstatuses").html());
        }
    });

    /*****************************
    *** Change Ticket Status.
    ******************************/
    jQuery(document).on("click",
        ".changeticketstatus",
        function() {
            var selected = jQuery(this).closest(".row").find(".ticketstatusbar").find(".active")
                .find("input[name='statustyperadio']");
            var statusid = jQuery(this).data("statusid");
            var ticketid = jQuery(selected).data("ticketid");
            var ticketitemid = jQuery(selected).data("ticketitemid");
            var tickettype = jQuery(selected).val();

            switch (tickettype) {
            case "ticket":
                changeTicketStatus(ticketid, ticketitemid, statusid);
                break;
            case "email":
                changeEmailStatus(ticketitemid, statusid);
                break;
            case "user":
                changeUserStatus(ticketitemid, statusid);
                break;
            }

            return false;
        });

    function changeTicketStatus(ticketid, ticketitemid, statusid) {
        // remove any previous notification.
        $(".usernotification").remove();

        // Send post request to server to change the status.
        $.ajax({
            url: "/tickets/chnageticketstatus/",
            data: {
                id: ticketid,
                status: statusid,
            },
            type: "POST",
        }).done(function(data) {
            // Verify if an error has occured, notify the user.
            if (data.error) {
                var source = $("#error-notification-template").html();
                var template = Handlebars.compile(source);
                var html = template(data);
                $("#maincontent").prepend(html);
                return false;
            }

            // Update ticket status count.
            if (statusid == 3) {
                statusid = 5;
            }
            if (statusid == 4) {
                statusid = 8;
            }
            var pagestatusid = $("#currentstatusid").val();
            var pcount = $("#" + statusid + ".my_task_status").text();
            pcount = parseInt(pcount);
            pcount = pcount + 1;
            $("#" + statusid + ".my_task_status").text(pcount);
            var cscount = $("#" + pagestatusid + ".my_task_status").text();
            cscount = parseInt(cscount);
            cscount = cscount - 1;
            $("#" + pagestatusid + ".my_task_status").text(cscount);

            // Display success message.
            var source = $("#success-notification-template").html();
            var template = Handlebars.compile(source);
            var html = template(data);
            $("#maincontent").prepend(html);

            // Remove the current ticket row.
            var ticketrow = $("tr#" + ticketitemid);
            ticketrow.animate({ backgroundColor: "yellow" }, 1000).fadeOut(1000,
                function() {
                    ticketrow.remove();
                });
        });
    }

    function changeEmailStatus(ticketitemid, statusid) {
        // remove any previous notification.
        $(".usernotification").remove();

        // Send post request to server to change the status.
        $.ajax({
            url: "/ticketitems/changeemailstatus/",
            data: {
                id: ticketitemid,
                status: statusid,
                quotedtime: 0
            },
            type: "POST",
        }).done(function(data) {
            // Verify if an error has occured, notify the user.
            if (data.error) {
                new PNotify({
                    title: "Error",
                    text: data.error,
                    type: "error"
                });
                return false;
            }

            // Update ticket status count.
            var pagestatusid = $("#currentstatusid").val();
            var pcount = $("#" + statusid + ".my_task_status").text();
            pcount = parseInt(pcount);
            pcount = pcount + 1;
            $("#" + statusid + ".my_task_status").text(pcount);
            var cscount = $("#" + pagestatusid + ".my_task_status").text();
            cscount = parseInt(cscount);
            cscount = cscount - 1;
            $("#" + pagestatusid + ".my_task_status").text(cscount);

            // Display success message to user.
            new PNotify({
                title: "Success",
                text: "This ticket status has been updated.",
                type: "success"
            });

            // Remove ticket row.
            var ticketrow = $("tr#" + ticketitemid);
            ticketrow.animate({ backgroundColor: "yellow" }, 1000).fadeOut(1000,
                function() {
                    ticketrow.remove();
                });
        });
    }

    /*****************************************************
     ** Change Multiple status
     *****************************************************/
    jQuery(document).on("click",
        ".closeemultitask",
        function() {
            // Make sure at least one ticket is selected.
            if ($(".selecttask:checked").length == 0) {
                new PNotify({
                    title: "Warning!",
                    text: "Please select at least one task to close",
                    type: "warning"
                });
                return false;
            }
            // Open Popup for multi status change.
            $("#changemultiplestatusmodel").modal("toggle");
            return false;
        });
    jQuery(document).on("click",
        ".btnclosemultipletask",
        function() {
            // make sure user has provided a date.
            var statustype = $("#statustype").val();
            if (statustype == "") {
                alert("please select status type");
                return false;
            }
            var statusid = $("#statusid").val();
            if (statusid == "") {
                alert("please select status");
                return false;
            }
            // Get all the selected team members.
            var ticketitems = [];
            $(".selecttask:checked").each(function() {
                ticketitems.push($(this).val());
            });
            $.ajax({
                url: "/ticketitems/Closemultiplestatus/",
                data: {
                    statustype: statustype,
                    statusid: statusid,
                    ticketitems: ticketitems
                },
                type: "POST",
            }).done(function(data) {
                if (data.error) {
                    var source = $("#error-notification-template").html();
                    var template = Handlebars.compile(source);
                    var html = template(data);
                    $("#modelassignuseradmindiv").prepend(html);
                    return false;
                }
                new PNotify({
                    title: "Success",
                    text: data.successtext,
                    type: "success"
                });
                return false;
            });
            $.each(ticketitems,
                function(key, value) {
                    var ticketrow = $("tr#" + value);
                    ticketrow.animate({ backgroundColor: "yellow" }, 1000).fadeOut(1000,
                        function() {
                            ticketrow.remove();
                        });
                    var pagestatusid = $("#currentstatusid").val();
                    var pcount = $("#" + statusid + ".my_task_status").text();
                    pcount = parseInt(pcount);
                    pcount = pcount + 1;
                    $("#" + statusid + ".my_task_status").text(pcount);
                    var cscount = $("#" + pagestatusid + ".my_task_status").text();
                    cscount = parseInt(cscount);
                    cscount = cscount - 1;
                    $("#" + pagestatusid + ".my_task_status").text(cscount);
                });
            $(".checkall").prop("checked", false);
            // Remove the popup.
            $("#changemultiplestatusmodel").modal("toggle");

            return false;
        });

    function changeUserStatus(ticketitemid, statusid) {
        // remove any previous notification.
        $(".usernotification").remove();

        // Send post request to server to change the status.
        $.ajax({
            url: "/ticketitems/changeuserstatus/",
            data: {
                ticketitemid: ticketitemid,
                statusid: statusid
            },
            type: "POST",
        }).done(function(data) {
            // Verify if an error has occured, notify the user.
            if (data.error) {
                new PNotify({
                    title: "Error",
                    text: data.error,
                    type: "error"
                });
                return false;
            }

            // Update ticket status count.
            var pagestatusid = $("#currentstatusid").val();
            var pcount = $("#" + statusid + ".my_task_status").text();
            pcount = parseInt(pcount);
            pcount = pcount + 1;
            $("#" + statusid + ".my_task_status").text(pcount);
            var cscount = $("#" + pagestatusid + ".my_task_status").text();
            cscount = parseInt(cscount);
            cscount = cscount - 1;
            $("#" + pagestatusid + ".my_task_status").text(cscount);

            // Display success message to user.
            new PNotify({
                title: "Success",
                text: "Your task status has been updated.",
                type: "success"
            });

            // Remove ticket row.
            var ticketrow = $("tr#" + ticketitemid);
            ticketrow.animate({ backgroundColor: "yellow" }, 1000).fadeOut(1000,
                function() {
                    ticketrow.remove();
                });
        });
    }

    /*****************************************************
     ** Filter tickets by status.
     *****************************************************/
    $(document).on("click",
        ".mytaskstatuslink",
        function() {
            // Fetch information required to send the post request.
            var id = $(this).attr("data-id"); // statusid
            var topic = $("#searchvalue").val();
            var pagenum = 0;

            // update hidden fields
            $("#currentStatusId").val(id);
            $("#pagenumber").val(pagenum);

            // Select All clients.
            $(".clientli").each(function() {
                $(this).removeClass("active");
            });
            $(".clientli#client-0").addClass("active");

            // Fetch tickets based on user selection.
            $.post("/ticketitems/mytaskajax/",
                { id, pagenum, topic },
                function(data) {
                    if (data.itemcount != 0) {
                        $("#mytaskrow").html(data.ticketitems);
                        $("#btnsearch").attr("data-statusid", id);
                        $(".collapse").collapse();
                        $(".statusli").each(function() {
                            $(this).removeClass("active");
                        });
                        $(".my_task_status").each(function() {
                            $(this).removeClass("badge-success");
                            $(this).addClass("badge-default");
                        });
                        $(".statusli#" + id).addClass("active");
                        $("#" + id + ".my_task_status").removeClass("badge-default");
                        $("#" + id + ".my_task_status").addClass("badge-success");
                        var count = $("#mytaskrow").find("tr").length;
                        $("#" + id + ".my_task_status").html(data.totalcount);
                        return false;
                    } else {
                        $("#mytaskrow")
                            .html('<tr><td colspan="2" class="text-center">Sorry, No more task found.</td></tr>');
                        $("#btnsearch").attr("data-statusid", id);
                        $(".statusli").each(function() {
                            $(this).removeClass("active");
                        });
                        $(".my_task_status").each(function() {
                            $(this).removeClass("badge-success");
                            $(this).addClass("badge-default");
                        });
                        $(".statusli#" + id).addClass("active");
                        $("#" + id + ".my_task_status").removeClass("badge-default");
                        $("#" + id + ".my_task_status").addClass("badge-success");
                        var count = $("#mytaskrow").find("tr").length;
                        $("#" + id + ".my_task_status").html(data.totalcount);
                        return false;
                    }
                });

            // Update URL & URL History
            if (typeof (history.pushState) == "undefined") {
                alert("Browser does not support HTML5.");
                return false;
            }
            var objHistory = { Title: "new", Url: "/ticketitems/mytasks/" + id };
            history.pushState(objHistory, objHistory.Title, objHistory.Url);

            return false;
        });

    /*****************************************************
     ** Filter tickets by client.
     *****************************************************/
    jQuery(document).on("click",
        ".myclientlink",
        function() {
            // Fetch information required to send post request.
            var id = $("#currentStatusId").val(); //statusid
            var clientid = $(this).attr("data-clientid");
            var topic = $("#searchvalue").val();
            var pagenum = 0;

            // update hidden fields
            $("#currentClientId").val(clientid);
            $("#pagenumber").val(pagenum);

            $.post("/ticketitems/mytasksasync/",
                { id, clientid, pagenum, topic },
                function(data) {
                    if (data.itemcount != 0) {
                        $("#mytaskrow").html(data.ticketitems);
                        $("#btnsearch").attr("data-statusid", id);
                        $(".collapse").collapse();

                        $(".clientli").each(function() {
                            $(this).removeClass("active");
                        });

                        //$('.my_task_status').each(function () {
                        //    $(this).removeClass('badge-success');
                        //    $(this).addClass('badge-default');
                        //});
                        $(".clientli#client-" + clientid).addClass("active");
                        //$('#' + id + '.my_task_status').removeClass('badge-default');
                        //$('#' + id + '.my_task_status').addClass('badge-success');
                        //var count = $("#mytaskrow").find("tr").length;
                        //$("#" + id + ".my_task_status").html(data.totalcount);
                        return false;
                    } else {
                        $("#mytaskrow")
                            .html('<tr><td colspan="2" class="text-center">Sorry, No more task found.</td></tr>');
                        $("#btnsearch").attr("data-statusid", id);

                        $(".clientli").each(function() {
                            $(this).removeClass("active");
                        });

                        $(".clientli#client-" + clientid).addClass("active");

                        return false;
                    }
                });

            // Update URL & URL History
            if (typeof (history.pushState) == "undefined") {
                alert("Browser does not support HTML5.");
                return false;
            }
            var objHistory = { Title: "new", Url: "/ticketitems/mytasks/" + id + "/" + clientid };
            history.pushState(objHistory, objHistory.Title, objHistory.Url);

            return false;
        });

    /*****************************************************
     ** Add Favourite Client
     *****************************************************/
    jQuery(document).on("click",
        ".addfavclient",
        function() {
            var control = jQuery(this);
            var id = jQuery(this).siblings("section").data("clientid");

            $.post("/ticketitems/addfavouriteclient/",
                { id },
                function(data) {
                    if (data.success == 1) {
                        jQuery(control).removeClass("fa-star-o").removeClass("addfavclient").addClass("fa-star")
                            .addClass("removefavclient");
                    }
                });

            return false;
        });

    /*****************************************************
     ** Remove Favourite Client
     *****************************************************/
    jQuery(document).on("click",
        ".removefavclient",
        function() {
            var control = jQuery(this);
            var id = jQuery(this).data("userfavid");

            $.post("/ticketitems/removefavouriteclient/",
                { id },
                function(data) {
                    if (data.success == 1) {
                        jQuery(control).removeClass("fa-star").removeClass("removefavclient").addClass("fa-star-o")
                            .addClass("addfavclient");
                    }
                });

            return false;
        });

    /*****************************
    *** Common Functions.
    ******************************/
    function getFormattedDate(userDate) {
        var myDateString = ("0" + (userDate.getMonth() + 1)).slice(-2) +
            "/" +
            ("0" + userDate.getDate()).slice(-2) +
            "/" +
            userDate.getFullYear();

        return myDateString;
    }

    function updateglobalbuckets() {
        $.ajax({
            url: "/GlobalBuckets/index/",
            data: "",
            type: "GET",
        }).done(function(data) {
            if (data.error) {
            }
            $(".GlobalBucketslist").html(data);
            var trcount = $("div.bucketitemrow");
            trcount = trcount.length;
            $(".bucktcount").html(trcount);
        });
    }
});