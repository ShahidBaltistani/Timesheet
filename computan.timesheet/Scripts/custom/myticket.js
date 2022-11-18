jQuery(function() {
    var $chkboxes = $(".selecttask");
    var lastChecked = null;

    $chkboxes.click(function(e) {
        if (!lastChecked) {
            lastChecked = this;
            return;
        }
        if (e.shiftKey) {
            var start = $chkboxes.index(this);
            var end = $chkboxes.index(lastChecked);
            $chkboxes.slice(Math.min(start, end), Math.max(start, end) + 1).prop("checked", lastChecked.checked);
        }
        lastChecked = this;
    });

    /*****************************************************
     ** Filter tickets by status.
     *****************************************************/
    jQuery(document).on("click",
        ".mytaskstatuslink",
        function() {
            // Fetch information required to send the post request.
            var id = $(this).attr("data-id"); // statusid
            var topic = $("#searchvalue").val();
            var pagenum = 0;
            var clientid = 0;

            // update hidden fields
            $("#currentStatusId").val(id);
            $("#pagenumber").val(pagenum);

            // Select All clients.
            $(".clientli").each(function() {
                $(this).removeClass("active");
            });
            $(".clientli#client-0").addClass("active");

            // Fetch tickets based on user selection.
            $.post("/tickets/mytickets/",
                { id, clientid, pagenum, topic },
                function(data) {
                    if (data.itemcount != 0) {
                        // update tickets.
                        $("#ticketscontainer").html(data.ticketitems);
                    } else {
                        // display no data found.
                        $("#ticketscontainer")
                            .html('<tr><td colspan="2" class="text-center">Sorry, No more task found.</td></tr>');
                    }

                    // highlight selected status.
                    $(".statusli").each(function() {
                        $(this).removeClass("active");
                    });
                    $(".statusli#" + id).addClass("active");

                    // clear search filter.
                    $("#searchtext").val("");

                    return false;
                });

            // Update URL & URL History
            if (typeof (history.pushState) == "undefined") {
                alert("Browser does not support HTML5.");
                return false;
            }
            var objHistory = { Title: "new", Url: "/tickets/mytickets/" + id };
            history.pushState(objHistory, objHistory.Title, objHistory.Url);

            return false;
        });

    /************************************************************
    ** Apply search filters.
    *************************************************************/
    jQuery(".filterclients").livefilter({ selector: ".clientcollection li" });

    jQuery("#searchtext").livefilter({ selector: "#ticketscontainer tr" });

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

            $.post("/tickets/mytickets/",
                { id, clientid, pagenum, topic },
                function(data) {
                    if (data.itemcount != 0) {
                        $("#ticketscontainer").html(data.ticketitems);
                    } else {
                        $("#ticketscontainer")
                            .html('<tr><td colspan="2" class="text-center">Sorry, No more task found.</td></tr>');
                    }

                    // activate current client tab.
                    $(".clientli").each(function() {
                        $(this).removeClass("active");
                    });
                    $(".clientli#client-" + clientid).addClass("active");

                    return false;
                });

            // Update URL & URL History
            if (typeof (history.pushState) == "undefined") {
                alert("Browser does not support HTML5.");
                return false;
            }
            var objHistory = { Title: "new", Url: "/tickets/mytickets/" + id + "/" + clientid };
            history.pushState(objHistory, objHistory.Title, objHistory.Url);

            return false;
        });

    /*****************************************************
     ** Add/Remove Favourite Client
     *****************************************************/
    jQuery(document).on("click",
        ".addfavclient",
        function() {
            var control = jQuery(this);
            var id = jQuery(this).siblings("section").data("clientid");

            $.post("/tickets/addfavouriteclient/",
                { id },
                function(data) {
                    if (data.success == 1) {
                        jQuery(control).removeClass("fa-star-o").removeClass("addfavclient").addClass("fa-star")
                            .addClass("removefavclient");
                    }
                });

            return false;
        });

    jQuery(document).on("click",
        ".removefavclient",
        function() {
            var control = jQuery(this);
            var id = jQuery(this).data("userfavid");

            $.post("/tickets/removefavouriteclient/",
                { id },
                function(data) {
                    if (data.success == 1) {
                        jQuery(control).removeClass("fa-star").removeClass("removefavclient").addClass("fa-star-o")
                            .addClass("addfavclient");
                    }
                });

            return false;
        });

    /************************************************************
    *** Enable/Disable all checkboxes
    *************************************************************/
    jQuery(document).on("click",
        ".checkall",
        function() {
            var checkstatus = this.checked;
            jQuery("#ticketscontainer").find("tr").each(function() {
                if (!jQuery(this).hasClass("lf-hidden")) {
                    jQuery(this).find(".selecttask:checkbox").prop("checked", checkstatus);
                }
            });

            //jQuery(".selecttask").prop('checked', this.checked);
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
    *** Add to Bucket methods.
    *************************************************************/
    jQuery(document).on("click",
        ".addtobucket",
        function() {
            // Update title of the model.
            $("#mybucketmodal").find(".panel-title").html("Add Ticket to Bucket");

            // Show today's date.
            $("#taskbucketdate").datepicker({ dateFormat: "mm/dd/yy" });
            $("#taskbucketdate").val(getFormattedDate(new Date()));
            $("#taskbucketdate").attr("data-tid", $(this).closest("tr").prop("id"));

            // Open Popup for single assignments.
            $("#btnsavetobucket").removeClass("savemultibucket").addClass("savesinglebucket");
            $("#mybucketmodal").modal("toggle");
        });

    jQuery(document).on("click",
        ".AddTimeLog",
        function() {
            ////////// Update title of the model.
            ////////$("#mybucketmodal").find(".panel-title").html("Add Ticket to Bucket");
            ////////// Show today's date.
            ////////$("#taskbucketdate").datepicker({ dateFormat: 'mm/dd/yy' });
            ////////$("#taskbucketdate").val(getFormattedDate(new Date()));
            ////////$("#taskbucketdate").attr("data-tid", $(this).closest("tr").prop("id"));
            ////////// Open Popup for single assignments.
            $(".ticket-title").html($(this).parent().find("a").html());
            var TicketID = $(this).closest("tr").prop("id");
            //$("#Skill").val($(this).closest("tr").find("#SkillName").val());
            //alert($("#Skill").val());
            $("#timespentinminutes").val("0");
            $("#billabletimeinminutes").val("0");
            $("#description").val("The task has been updated/completed.");

            $("#workdate").datepicker({ dateFormat: "mm/dd/yy", maxDate: 0 });
            $("#workdate").val(getFormattedDate(new Date()));

            $.ajax({
                url: "/tickets/TicketAllocation?id=" + TicketID,
                data: {
                },
                type: "GET",
            }).done(function(data) {
                if (data.error) {
                    return false;
                }
                $("#AddTimeLogModel").modal("toggle");
                $("#ticketItemID").val(data.ticketItemID);
                $("#ProjectID").val(data.projectid);
                $("#Project").val(data.projectname);
                $("#SkillID").val(data.skillid);
                $("#Skill").val(data.skillname);
                $("#WarningStatus").val(data.WarningStatus);
                $("#WarningText").val(data.Warningtext);
            });
        });

    jQuery("#timespentinminutes").on("input",
        function(e) {
            jQuery("#billabletimeinminutes").val(jQuery(this).val());
        });

    jQuery("#description").on("focus",
        function(e) {
            jQuery(this).select();
        });

    jQuery(document).on("click",
        ".savetimeTicket",
        function() {
            var ticketitemid = jQuery("#ticketItemID").val();
            var projectid = jQuery("#ProjectID").val();
            var skillid = jQuery("#SkillID").val();
            var workdate = jQuery("#workdate").val();
            var timespent = jQuery("#timespentinminutes").val();
            var billable = jQuery("#billabletimeinminutes").val();
            var title = encodeURI(jQuery(".ticket-title").text().trim());
            var description = jQuery("#description").val();
            var IsWarning = jQuery("#WarningStatus").val();
            var WarningText = jQuery("#WarningText").val();
            var comments = "";

            //if (ticketitemid == "" || ticketitemid == 0) {
            //    new PNotify({
            //        title: 'Project/Skill Missing',
            //        text: "Open <b>Task Details</b> section, and select a valid project and skill.",
            //        type: 'error',
            //        hide: false
            //    });
            //    return false;
            //}

            //if (projectid == "") {
            //    new PNotify({
            //        title: 'Project Missing',
            //        text: "Open <b>Task Details</b> section, and select a valid project.",
            //        type: 'error',
            //        hide: false
            //    });
            //    return false;
            //}

            //if (skillid == "") {
            //    new PNotify({
            //        title: 'Skill Missing',
            //        text: "Open <b>Task Details</b> section, and select a valid skill.",
            //        type: 'error',
            //        hide: false
            //    });
            //    return false;
            //}
            if (workdate == "") {
                new PNotify({
                    title: "Work Date Required",
                    text: "Please select a work date.",
                    type: "error",
                    hide: false
                });
                return false;
            }

            if (timespent == "" || timespent == "0") {
                new PNotify({
                    title: "Time Spent",
                    text: "Please enter a valid time spent.",
                    type: "error",
                    hide: false
                });
                return false;
            }

            if (description == "") {
                new PNotify({
                    title: "Description Required",
                    text: "Please enter description.",
                    type: "error",
                    hide: false
                });
                return false;
            }

            $.ajax({
                url: "/tickets/addtime/",
                data: {
                    tcktitemid: ticketitemid,
                    pid: projectid,
                    sid: skillid,
                    spenttime: timespent,
                    billtime: billable,
                    workdate: workdate,
                    title: title,
                    description: description,
                    comments: comments
                },
                type: "POST",
            }).done(function(data) {
                if (data.error) {
                    new PNotify({
                        title: "Error",
                        text: data.errortext,
                        type: "error",
                        hide: true
                    });
                    //var source = $("#error-notification-template").html();
                    //var template = Handlebars.compile(source);
                    //var html = template(data);
                    //$("#modeladdtimebodydiv").prepend(html);
                    return false;
                }
                new PNotify({
                    title: "Success",
                    text: "Time has been added successfully.",
                    type: "success",
                    hide: true
                });

                //clearAddTime();
                $("#AddTimeLogModel").modal("toggle");
                return false;
            });

            // Display warning, if enabled.
            if (IsWarning) {
                new PNotify({
                    title: "<h1>Warning!</h1>",
                    text: "<h3>" + WarningText + "</h3>",
                    type: "error",
                    hide: false
                });
            }

            return false;
        });

    jQuery(document).on("click",
        ".addmultibucket",
        function() {
            // Make sure at least one ticket is selected.
            if ($(".selecttask:checked").length == 0) {
                new PNotify({
                    title: "Warning!",
                    text: "Please select at least one ticket to add in the bucket!",
                    type: "warning"
                });
                return false;
            }

            // Update title of the model.
            $("#mybucketmodal").find(".panel-title").html("Add Multiple Tickets to Bucket");

            // Show today's date.
            $("#taskbucketdate").datepicker({ dateFormat: "mm/dd/yy" });
            $("#taskbucketdate").val(getFormattedDate(new Date()));

            // Open Popup for multi assignments.
            $("#btnsavetobucket").removeClass("savesinglebucket").addClass("savemultibucket");
            $("#mybucketmodal").modal("toggle");
            return false;
        });

    jQuery(document).on("click",
        ".savesinglebucket",
        function() {
            // make sure user has provided a date.
            var workdate = $("#taskbucketdate").val();
            if (workdate == "") {
                new PNotify({
                    title: "Sorry!",
                    text: "The date is required to add ticket to the bucket.",
                    type: "error"
                });
                return false;
            }

            // Get all the selected team members.
            var ticketid = $("#taskbucketdate").data("tid");
            var userTasks = [];
            userTasks.push(ticketid);

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

                new PNotify({
                    title: "Success",
                    text: data.Successtext,
                    type: "success"
                });

                return false;
            });

            // Remove the popup.
            $("#mybucketmodal").modal("toggle");

            return false;
        });

    jQuery(document).on("click",
        ".savemultibucket",
        function() {
            // make sure user has provided a date.
            var workdate = $("#taskbucketdate").val();
            if (workdate == "") {
                new PNotify({
                    title: "Sorry!",
                    text: "The date is required to add ticket to the bucket.",
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
            $("#mybucketmodal").modal("toggle");

            return false;
        });

    /************************************************************
    *** Show actions on hover.
    *************************************************************/
    jQuery(document).on("mouseover",
        ".changeticketstatus",
        function() {
            jQuery(this).closest("tr").find(".hideable").show();
        });

    /************************************************************
    *** Change Ticket Status.
    *************************************************************/
    jQuery(document).on("click",
        ".changeticketstatus",
        function() {
            var statusid = jQuery(this).data("statusid");
            var ticketid = jQuery(this).closest("tr").prop("id");
            var ticketitemid = 0;

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
                //if (statusid == 3) {
                //    statusid = 5;
                //}
                //if (statusid == 4) {
                //    statusid = 8;
                //}

                var pcount = $("#" + statusid + ".my_task_status").text();
                pcount = parseInt(pcount);
                pcount = pcount + 1;
                $("#" + statusid + ".my_task_status").text(pcount);

                var pagestatusid = $(".filterbystatus").find(".active").prop("id");
                var cscount = $("#" + pagestatusid + ".my_task_status").text();
                cscount = parseInt(cscount);
                cscount = cscount - 1;
                $("#" + pagestatusid + ".my_task_status").text(cscount);

                // Display success message.
                //var source = $("#success-notification-template").html();
                //var template = Handlebars.compile(source);
                //var html = template(data);
                //$("#maincontent").prepend(html);

                // Remove the current ticket row.
                var ticketrow = $("tr#" + ticketid);
                ticketrow.animate({ backgroundColor: "yellow" }, 1000).fadeOut(1000,
                    function() {
                        ticketrow.remove();
                    });
            });

            return false;
        });

    jQuery(document).on("click",
        ".openmultistatusmodal",
        function() {
            // Make sure at least one ticket is selected.
            if ($(".selecttask:checked").length == 0) {
                new PNotify({
                    title: "Warning!",
                    text: "Please select at least one ticket to change status",
                    type: "warning"
                });
                return false;
            }

            // Open Popup for multi status change.
            $("#myticketstatusmodal").modal("toggle");

            return false;
        });

    jQuery(document).on("click",
        ".changemultipleticketstatus",
        function() {
            // make sure user has provided a date.
            var statustype = 1; // Ticket
            var statusid = $(this).data("statusid");

            // Get all the selected team members.
            var ticketitems = [];
            $(".selecttask:checked").each(function() {
                ticketitems.push($(this).val());
            });

            $.ajax({
                url: "/tickets/Closemultiplestatus/",
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
                    $("#myticketstatusmodal").prepend(html);
                    return false;
                }

                $.each(ticketitems,
                    function(key, value) {
                        // remove selected row.
                        var ticketrow = $("tr#" + value);
                        ticketrow.animate({ backgroundColor: "yellow" }, 1000).fadeOut(1000,
                            function() {
                                ticketrow.remove();
                            });

                        var pcount = $("#" + statusid + ".my_task_status").text();
                        pcount = parseInt(pcount);
                        pcount = pcount + 1;
                        $("#" + statusid + ".my_task_status").text(pcount);

                        var pagestatusid = $(".filterbystatus").find(".active").prop("id");
                        var cscount = $("#" + pagestatusid + ".my_task_status").text();
                        cscount = parseInt(cscount);
                        cscount = cscount - 1;
                        $("#" + pagestatusid + ".my_task_status").text(cscount);
                    });

                new PNotify({
                    title: "Success",
                    text: data.successtext,
                    type: "success"
                });
                return false;
            });

            $(".checkall").prop("checked", false);
            // Remove the popup.
            $("#myticketstatusmodal").modal("toggle");

            return false;
        });

    /************************************************************
    *** Auto-Load products on scroll.
    *************************************************************/
    jQuery(window).scroll(function() {
        if (jQuery(window).scrollTop() == jQuery(document).height() - jQuery(window).height()) {
            var count = jQuery(".loadmoretickets").closest("tr").length;
            if (count > 0) {
                //alert("auto-scrolling for more products");
            }
        }
    });

    jQuery(document).on("click",
        ".loadmoretickets",
        function() {
            var pagenumber = parseInt(jQuery(this).data("pagenumber"));
            pagenumber++;
            loadProducts(pagenumber);
        });
});

function loadProducts(pagenum) {
    var id = jQuery(".filterbystatus").find(".active").prop("id"); //statusid
    var clientid = jQuery(".filterbyclient").find(".active").data("clientid");
    var topic = $("#searchvalue").val();

    // Fetch tickets based on user selection.
    $.post("/tickets/mytickets/",
        { id, clientid, pagenum, topic },
        function(data) {
            // remove current row.
            jQuery("#loadmoretr").remove();

            // update tickets.
            $("#ticketscontainer").append(data.ticketitems);

            return false;
        });

    return false;
    //var search = $("#searchvalue").val();
    //var id = $("#btnsearch").attr('data-statusid');
    //if (pagenum != -1) {
    //    var statusid = $("#btnsearch").attr('data-statusid');
    //    $.ajax({
    //        url: '/ticketitems/mytaskajax',
    //        data: {
    //            id: statusid,
    //            topic: search,
    //            pagenum: pagenum
    //        },
    //        type: 'POST',

    //    }).done(function (data) {
    //        if (data.itemcount != 0) {
    //            $("#pagenumber").val(pagenum);
    //            $("#mytaskrow").append(data.ticketitems);
    //            var count = $("#mytaskrow").find("tr").length;
    //            $("#" + statusid + ".conversationstatus").html(data.totalcount);
    //        }
    //        else {
    //            $("#pagenumber").val(-1);
    //            $("#mytaskrow").append('<tr><td colspan="2" class="text-center">Sorry,no more task found.</td></tr>');
    //        }
    //        return false;
    //    });
    //    return false;
    //}
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

function getFormattedDate(userDate) {
    var myDateString = ("0" + (userDate.getMonth() + 1)).slice(-2) +
        "/" +
        ("0" + userDate.getDate()).slice(-2) +
        "/" +
        userDate.getFullYear();

    return myDateString;
}