jQuery(function () {
    initDocument();

    jQuery(document).on("mouseover",
        ".changeticketstatus",
        function () {
            jQuery(this).closest("tr").find(".hideable").show();
        });
    //jQuery(document).on('focus', '.CustSelect2', function (e) {
    //    if (e.originalEvent) {
    //        jQuery(this).siblings('select').select2('open');
    //    }
    //});

    var $chkboxes = $(".selticket");
    var lastChecked = null;

    $chkboxes.click(function (e) {
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

    /************************************************************
    *** On Project Title Click.
    *************************************************************/

    $("arrow-click").on("click",
        function () {
            $("arrow-click i").removeClass("fa-caret-right");
            $("arrow-click i").addClass("fa-caret-down");
        });

    /************************************************************
    *** Change Ticket Status.
    *************************************************************/
    jQuery(document).on("click",
        ".changeticketstatus",
        function () {
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
            }).done(function (data) {
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
                //var count = $("#" + statusid).text();
                //alert(count);
                //var pcount = $("#" + statusid + ".my_task_status").text();
                //pcount = parseInt(pcount);
                //pcount = pcount + 1;
                //$("#" + statusid + ".my_task_status").text(pcount);

                //var pagestatusid = $(".filterbystatus").find('.active').prop("id");
                //var cscount = $("#" + pagestatusid + ".my_task_status").text();
                //cscount = parseInt(cscount);
                //cscount = cscount - 1;
                //$("#" + pagestatusid + ".my_task_status").text(cscount);

                // Display success message.
                var source = $("#success-notification-template").html();
                var template = Handlebars.compile(source);
                var html = template(data);
                $("#maincontent").prepend(html);

                // Remove the current ticket row.
                var ticketrow = $("tr#" + ticketid);
                ticketrow.animate({ backgroundColor: "yellow" }, 1000).fadeOut(1000,
                    function () {
                        ticketrow.remove();
                    });
            });

            return false;
        });
    /*
    *   Search Tickets Feature.
    */
    jQuery(document).on("click",
        "#btnsearch",
        function () {
            //alert('test 1');
            var url = "/tickets/SearchTickets";
            var statusid = $(this).attr("data-statusid");
            var searchstring = $("#searchtext").val();
            $("#searchvalue").val(searchstring);
            $("#pagenumber").val(0);
            //alert('test 2');
            $.ajax({
                url: url,
                data: {
                    searchstring: searchstring,
                    statusid: statusid,
                    pagenum: 0
                },
                type: "GET",
            }).done(function (data) {
                $("#productList").html(data.tickets);
                var count = $("#productList").find("tr").length;
                $("#" + statusid + ".conversationstatus").html(data.totalcount);
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
        function () {
            if ($("#searchvalue").val() != "") {
                var id = $("#TicketStatusId").val();
                $("#pagenumber").val(0);
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
    /*
    ** Enable/Disable all checkboxes
    */
    jQuery(document).on("click",
        ".checkall",
        function () {
            var checkstatus = this.checked;
            jQuery("#productList").find("tr").each(function () {
                if (!jQuery(this).hasClass("lf-hidden")) {
                    jQuery(this).find(".selticket:checkbox").prop("checked", checkstatus);
                }
            });
        });

    jQuery(document).on("click",
        ".selticket",
        function () {
            if (!this.checked) {
                $(".checkall").prop("checked", false);
            } else if ($(".selticket").length == $(".selticket:checked").length) {
                $(".checkall").prop("checked", true);
            }
        });

    /*
    ** Multiple Assignment.
    */
    //jQuery(document).on("click",
    //    ".multiassign",
    //    function () {
    //        debugger
    //        // Make sure at least one ticket is selected.
    //        if ($(".selticket:checked").length == 0) {
    //            new PNotify({
    //                title: "Warning!",
    //                text: "Please select at least one ticket for assignment!",
    //                type: "warning"
    //            });
    //            return false;
    //        }

    //        // Open Popup for multi assignments.
    //        $("#btnassignticket").removeClass("saveticketassignment").addClass("assignmultitickets");
    //        initDocument();
    //        $("#assignuseradmin").modal("toggle");
    //        return false;
    //    });

    jQuery(document).on("click",
        ".assignmultitickets",
        function () {
            // Make sure user has select project, skill and at least one team member.
            var userscsv = jQuery(".assignedusers").val();
            var teamscsv = jQuery(".assignedteams").val();
            var projectid = $("#projectid_assignuser").val();
            var skillid = $("#skillid_assignuser").val();
            if (projectid == "") {
                sweetAlert("Sorry", "Please select a project from the list.", "error");
                return false;
            }
            if (skillid === "") {
                sweetAlert("Sorry", "Please select a skill from the list.", "error");
                return false;
            }
            //if ($(".teammember:checked").length == 0 || $(".shiftteammember:checked").length == 0) {
            //    sweetAlert("Sorry", "Please select Both  at least one team and one user.", "error");
            //    return false;
            //}
            ////Get All Shift Id for which we have to assign task
            //var shift_teamMembersAll = [];
            //$(".shiftteammember:checked").each(function() {
            //    shift_teamMembersAll.push($(this).data("teamid"));
            //});
            //var shift_teamMembersCSVAll = shift_teamMembersAll.join(",");

            //// Get all the selected team members.
            //var teamMembers = [];
            //$(".teammember:checked").each(function() {
            //    teamMembers.push($(this).data("userid"));
            //});
            //var teamMembersCSV = teamMembers.join(",");

            //// Get all the selected tickets.
            var selectedTickets = [];
            $(".selticket:checked").each(function () {
                selectedTickets.push($(this).val());
            });
            var selectedTicketsCSV = selectedTickets.join(",");

            $.ajax({
                url: "/Tickets/AssignMultipleTickets/",
                data: {
                    projectid: projectid,
                    skillid: skillid,
                    ticketcsv: selectedTicketsCSV,
                    usercsv: userscsv,
                    shiftteamID: teamscsv
                },
                type: "POST",
            }).done(function (data) {
                if (data.error) {
                    var source = $("#error-notification-template").html();
                    var template = Handlebars.compile(source);
                    var html = template(data);
                    $("#modelassignuseradmindiv").prepend(html);
                    return false;
                }

                $.each(data.tickets,
                    function (index, value) {
                        $("#" + value).animate({ backgroundColor: "yellow" }, 1000).fadeOut(1000,
                            function () {
                                $("#" + value).remove();
                            });
                        $("input:checkbox").removeAttr("checked");
                        $("#searchtext").val("");
                        var pcount = $("#2.conversationstatus").text();
                        pcount = parseInt(pcount);
                        pcount = pcount + 1;
                        $("#2.conversationstatus").text(pcount);
                        var cscount = $("#1.conversationstatus").text();
                        cscount = parseInt(cscount);
                        cscount = cscount - 1;
                        $("#1.conversationstatus").text(cscount);
                    });

                new PNotify({
                    title: "Success",
                    text: "The task has been assigned to selected users.",
                    type: "success"
                });
                $("#projectid_assignuser").val("").trigger("change");
                $("#skillid_assignuser").val("").trigger("change");
                $(".teammember:checked").prop("checked", false);
                jQuery(".assignedusers").tagsinput("removeAll");
                jQuery(".assignedteams").tagsinput("removeAll");
                return false;
            });
            // Remove Popup and revert it back for normal assignment.
            $("#assignuserspopup").removeClass("assignmultitickets").addClass("saveticketassignment");
            $("#assignuseradmin").find(".modal-title").html("Assign users");
            $("#assignuseradmin").modal("toggle");

            return false;
        });

    /*
    ** Preview current ticket.
    */
    jQuery(document).on("click",
        ".previewticket",
        function () {
            var id = $(this).data("id");
            $.ajax({
                url: "/ticketitems/loadconversation/" + id,
                data: "",
                type: "GET",
            }).done(function (data) {
                if (data.error) {
                    return false;
                }
                $("#conversationmodelbodydiv").html(data);
                $("#modelemailbody a").attr("target", "_blank");
                $("#uniquebodymodel").modal("show");
                return false;
            });
        });

    /*
    ** Preview Ticket detail
    */
    //jQuery(document).on("click", ".previewticketdetail", function () {
    //    alert("Test");
    //    var id = $(this).data("id");
    //    $.ajax({
    //        url: '/tickets/LoadTicketDetail/' + id,
    //        data: "",
    //        type: 'GET',

    //    }).done(function (data) {
    //        //$("#conversationmodelbodydiv").html(data);
    //        $('#modelemailbody a').attr('target', '_blank');
    //        $('#uniquebodymodel').modal('show');
    //        return false;
    //    });
    //});

    /*
    ** Assign current ticket.
    */
    jQuery(document).on("click",
        ".assignticket",
        function () {
            $("#btnassignticket").removeClass("assignmultitickets").addClass("saveticketassignment");
            // Clear any existing users checked.
            $(".shiftteammember").each(function () {
                $(this).prop("checked", false);
            });

            $(".teammember").each(function () {
                $(this).prop("checked", false);
            });

            var ticketid = $(this).data("ticketid");
            var projectid = $(this).data("projectid");
            var skillid = $(this).data("skillid");

            if (projectid != "") {
                $("#projectid_assignuser").val(projectid).trigger("change");
            }
            if (skillid != "") {
                $("#skillid_assignuser").val(skillid).trigger("change");
            }

            $("#assignticket_ticketid").val(ticketid);
            $("#assignuseradmin").modal("toggle");
        });

    jQuery(document).on("click",
        ".assignticketaction",
        function () {
            jQuery("#ticketprojectid").val("").trigger("change");
            jQuery("#ticketskillid").val("").trigger("change");
            jQuery("#estimateTimeInput").val("").trigger("change");
            jQuery(".assignedusersinput").tagsinput("removeAll");
            jQuery(".assignedteaminput").tagsinput("removeAll");
            jQuery("#ticketcomment").val("");

            var ticketid = jQuery(this).data("ticketid");
            var projectid = jQuery(this).data("projectid");
            var skillid = jQuery(this).data("skillid");

            jQuery("#AssignmentDialogTicketId").val(ticketid);

            if (projectid != "") {
                jQuery("#ticketprojectid").val(projectid).trigger("change");
            }
            if (skillid != "") {
                jQuery("#ticketskillid").val(skillid).trigger("change");
            }

            $.ajax({
                url: "/Tickets/FetchUsersAndTeams/",
                data: {
                    Ticketid: ticketid
                },
                type: "GET",
            }).done(function (data) {
                if (data.error == 1) {
                    sweetAlert("Sorry", data.TextContext, "error");
                    alert("test");
                    return false;
                } else {
                    if (data.Users != null) {
                        $(data.Users).each(function (index, value) {
                            jQuery(".assignedusersinput").tagsinput("add", { value: value.Id, text: value.FullName });
                        });
                    }

                    if (data.Teams != null) {
                        $(data.Teams).each(function (index, value) {
                            jQuery(".assignedteaminput").tagsinput("add", { value: value.id, text: value.name });
                        });
                    }
                    if (data.Ticketstatus > 1) {
                        jQuery("#ticketprojectid").val(data.projectId).trigger("change");
                        jQuery("#ticketskillid").val(data.skillId).trigger("change");
                    }
                    $.ajax({
                        url: "/Home/FetchEstimatetasktime/",
                        data: {
                            Ticketid: ticketid
                        },
                        type: "GET",
                    }).done(function (data) {
                        jQuery("#estimateTimeInput").val(data.estimatetime).trigger("change");
                        jQuery("#EstimateTimeHidden").val(data.estimatetime).trigger("change");
                    });
                    jQuery(".tag").removeClass("label-info").addClass("label-success");
                    jQuery("#ticketAssignModal").modal("toggle");
                }
            });
        });
    jQuery(document).on("click",
        ".assign-single-ticket",
        function () {
            // Fetch user input data.
            var ticketid = jQuery("#AssignmentDialogTicketId").val();
            var projectid = jQuery("#ticketprojectid").val();
            var skillid = jQuery("#ticketskillid").val();
            var userscsv = jQuery(".assignedusersinput").val();
            var teamscsv = jQuery(".assignedteaminput").val();
            var comment = jQuery("#ticketcomment").val();
            var StartDate = jQuery("#StartDate").val();
            var EndDate = jQuery("#EndDate").val();
            var ticketscsv = ticketid;
            var EstimatedTime = jQuery("#estimateTimeInput").val();
            var EstimateTimeHidden = jQuery("#EstimateTimeHidden").val();
            // Validate required information.
            //if (EstimatedTime == "" || EstimatedTime == 0) { sweetAlert("Sorry! Assignment Missing", "Please add ticket estimate time.", "error");return false;}
            if (projectid == "") {
                sweetAlert("Sorry", "Please select a project from the list.", "error");
                return false;
            }
            if (skillid == "") {
                sweetAlert("Sorry", "Please select a skill from the list.", "error");
                return false;
            }
            if (userscsv == "" && teamscsv == "") {
                sweetAlert("Sorry! Assignment Missing", "Please assign ticket to at least one user/team.", "error");
                return false;
            }
            if (EstimatedTime === EstimateTimeHidden) {
                EstimatedTime = null;
            }
            $.ajax({
                url: "/tickets/assigntickets/",
                data: {
                    ticketcsv: ticketscsv,
                    projectid: projectid,
                    skillid: skillid,
                    StartDate: StartDate,
                    EndDate: EndDate,
                    usercsv: userscsv,
                    teamcsv: teamscsv,
                    comment: comment,
                    EstimatedTime: EstimatedTime
                },
                type: "POST",
            }).done(function (data) {
                if (data.error) {
                    alert(data.errortext);
                    return false;
                }
                $.each(data.tickets,
                    function (index, value) {
                        // remove the ticket row with animation.
                        var ticketrow = $("tr#" + value.toString());
                        ticketrow.animate({ backgroundColor: "yellow" }, 1000).fadeOut(1000,
                            function () {
                                $(ticketrow).remove();
                            });
                        // update the counts.
                        var pcount = $("#2.conversationstatus").text();
                        pcount = parseInt(pcount);
                        pcount = pcount + 1;
                        $("#2.conversationstatus").text(pcount);
                        var cscount = $("#1.conversationstatus").text();
                        cscount = parseInt(cscount);
                        cscount = cscount - 1;
                        $("#1.conversationstatus").text(cscount);
                    });

                // display success message.
                new PNotify({
                    title: "Success",
                    text: "The task has been assigned to selected users.",
                    type: "success"
                });
                // hide the dialog box.
                jQuery("#ticketAssignModal").modal("hide");

                return false;
            });
        });

    // @@ Save Current Ticket Assignments.
    jQuery(document).on("click",
        ".saveticketassignment",
        function () {
            var ticketid = $("#assignticket_ticketid").val();
            var projectid = $("#projectid_assignuser").val();
            var skillid = $("#skillid_assignuser").val();

            if (projectid == "") {
                sweetAlert("Sorry", "Please select a project from the list.", "error");
                return false;
            }
            if (projectid == "" || skillid === "") {
                sweetAlert("Sorry", "Peasle select a skill from the list.", "error");
                return false;
            }

            if ($(".teammember:checked").length == 0 && $(".shiftteammember:checked").length == 0) {
                sweetAlert("Sorry", "Please select at least one team member to assign the ticket.", "error");
                return false;
            }

            var shift_teamMembers = [];
            $(".shiftteammember:checked").each(function () {
                shift_teamMembers.push($(this).data("teamid"));
            });
            var shift_teamMembersCSV = shift_teamMembers.join(",");

            var AllteamMembers = [];
            $(".teammember:checked").each(function () {
                AllteamMembers.push($(this).data("userid"));
            });
            var AllteamMembersCSV = AllteamMembers.join(",");

            //alert($(".teammember:checked").length);
            //alert($(".shiftteammember:checked").length);

            // alert(AllteamMembersCSV);
            $.ajax({
                url: "/Tickets/AssignTicket/",
                data: {
                    ticketid: ticketid,
                    projectid: projectid,
                    skillid: skillid,
                    users: AllteamMembersCSV,
                    shiftteamID: shift_teamMembersCSV
                },
                type: "POST",
            }).done(function (data) {
                if (data.error || data.error == true) {
                    var source = $("#error-notification-template").html();
                    var template = Handlebars.compile(source);
                    var html = template(data);
                    $("#modelassignuseradmindiv").find(".alert").remove();
                    $("#modelassignuseradmindiv").prepend(html);
                    return false;
                } else {
                    // remove the modal
                    $("#assignuseradmin").modal("toggle");
                    $("body").on("hidden.bs.modal",
                        "#assignuseradmin",
                        function () {
                            $(this).removeData("bs.modal");
                        });

                    // animate remove the ticket row.
                    var ticketrow = $("tr#" + data.ticketid.toString());
                    ticketrow.animate({ backgroundColor: "yellow" }, 1000).fadeOut(1000,
                        function () {
                            $(ticketrow).remove();
                        });

                    var pcount = $("#2.conversationstatus").text();
                    pcount = parseInt(pcount);
                    pcount = pcount + 1;
                    $("#2.conversationstatus").text(pcount);
                    var cscount = $("#1.conversationstatus").text();
                    cscount = parseInt(cscount);
                    cscount = cscount - 1;
                    $("#1.conversationstatus").text(cscount);
                    new PNotify({
                        title: "Success",
                        text: "The task has been assigned to selected users.",
                        type: "success"
                    });
                    $("#projectid_assignuser").val("").trigger("change");
                    $("#skillid_assignuser").val("").trigger("change");
                    $(".teammember:checked").prop("checked", false);
                    return false;
                }
            });
        });

    /*****************************************************
    ** Change Multiple status
    *****************************************************/
    jQuery(document).on("click",
        ".closeemultitask",
        function () {
            // Make sure at least one ticket is selected.
            if ($(".selticket:checked").length == 0) {
                new PNotify({
                    title: "Warning!",
                    text: "Please select at least one ticket to close",
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
        function () {
            // make sure user has provided a date.
            var statusid = $(this).data("statusid");

            //var statustype = 1;
            //if (statustype == "") {
            //    alert("please select status type")
            //    return false;
            //}

            //var statusid = $("#statusid").val();
            //if (statusid == "") {
            //    alert("please select status")
            //    return false;
            //}
            // Get all the selected team members.
            var tickets = [];
            $(".selticket:checked").each(function () {
                tickets.push($(this).val());
            });

            //var tickets = "[";
            //var first = true;
            //$(".selticket:checked").each(function () {
            //    if (first == false) {
            //        tickets += ",";
            //    }
            //    else {
            //        first = false;
            //    }
            //    tickets += ($(this).val());
            //});
            //tickets += "]"
            //alert(tickets);

            $.ajax({
                url: "/tickets/Closemultiplestatus/",
                data: {
                    statustype: 1,
                    statusid: statusid,
                    ticketitems: tickets
                },
                type: "POST",
            }).done(function (data) {
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
            $.each(tickets,
                function (key, value) {
                    var ticketrow = $("tr#" + value);
                    ticketrow.animate({ backgroundColor: "yellow" }, 1000).fadeOut(1000,
                        function () {
                            ticketrow.remove();
                        });
                    var pagestatusid = $("#currentstatus").val();
                    var pcount = $("#" + statusid + ".conversationstatus").text();
                    pcount = parseInt(pcount);
                    pcount = pcount + 1;
                    $("#" + statusid + ".conversationstatus").text(pcount);
                    var cscount = $("#" + pagestatusid + ".conversationstatus").text();
                    cscount = parseInt(cscount);
                    cscount = cscount - 1;
                    $("#" + pagestatusid + ".conversationstatus").text(cscount);
                });
            $(".checkall").prop("checked", false);
            // Remove the popup.
            $("#changemultiplestatusmodel").modal("toggle");

            return false;
        });

    jQuery(".assignedusersinput").on("beforeItemAdd",
        function (event) {
            var tag = event.item;
            $.ajax({
                url: "/tickets/PrefetchSingleTeams?UserID=" + tag.value,
                type: "GET",
            }).done(function (data) {
                if (data.error) {
                    alert(JSON.stringify(data.errortext));
                    return false;
                } else {
                    jQuery(".assignedteaminput").tagsinput("add", { value: data.teamid, text: data.teamName });
                    return false;
                }
            });
        });

    jQuery(".assignedusersinput").on("beforeItemRemove",
        function (event) {
            var tag = event.item;
            var ticketid = jQuery("#AssignmentDialogTicketId").val();

            if (!event.options || !event.options.preventPost) {
                jQuery.post("/Tickets/RemoveTicketUser",
                    { id: ticketid, userid: tag.value },
                    function (response) {
                        if (!response.success) {
                            jQuery(".assignedusersinput").tagsinput("add", tag, { preventPost: true });
                        }
                        if (response.success) {
                            new PNotify({
                                title: "Success!",
                                text: response.messagetext,
                                type: "success"
                            });
                            //jQuery('.assignedusersinput').tagsinput('remove', { value: tag.value, text: tag.text });
                        }
                        return true;
                    });
            }
        });

    jQuery(".assignedteaminput").on("beforeItemRemove",
        function (event) {
            var tag = event.item;
            var ticketid = jQuery("#AssignmentDialogTicketId").val();

            if (!event.options || !event.options.preventPost) {
                jQuery.post("/Tickets/RemoveTicketteam",
                    { id: ticketid, teamid: tag.value },
                    function (response) {
                        if (!response.success) {
                            // Re-add the tag since there was a failure
                            // "preventPost" here will stop this ajax call from running when the tag is added
                            jQuery(".assignedteaminput").tagsinput("add", tag, { preventPost: true });
                        }
                        if (response.success) {
                            new PNotify({
                                title: "Success!",
                                text: response.messagetext,
                                type: "success"
                            });
                        }
                    });
            }
            // event.item: contains the item
            // event.cancel: set to true to prevent the item getting removed
        });

    jQuery(document).on("click",
        ".teammember",
        function () {
            if (this.checked) {
                var tag = event.item;
                $.ajax({
                    url: "/tickets/PrefetchSingleTeams?UserID=" + $(this).data("userid"),
                    type: "GET",
                }).done(function (data) {
                    if (data.error) {
                        alert(JSON.stringify(data.errortext));
                        return false;
                    } else {
                        $(".shiftteammember").each(function (index, item) {
                            if ($(item).data("teamid") == data.teamid) {
                                $(item).prop("checked", true);
                            }
                        });
                        return false;
                    }
                });
            }
        });

    /************************************************************
    *** Add/Remove Ticket Team.
    *************************************************************/
    jQuery(document).on("click",
        ".teamitem",
        function () {
            var ticketid = jQuery(this).closest("tr").attr("id");
            var teamid = jQuery(this).data("teamid");
            var currentbutton = jQuery(this);
            var teamname = jQuery(this).attr("name");
            $.ajax({
                url: "/tickets/UpdateTicketTeam?TicketId=" + ticketid + "&TeamId=" + teamid,
                type: "GET",
            }).done(function (data) {
                if (data.error) {
                    if (data.flag == 1) {
                        new PNotify({
                            title: "Error",
                            text: data.errortext,
                            type: "error"
                        });
                        //alert(JSON.stringify(data.errortext));
                        return false;
                    } else {
                        new PNotify({
                            title: "Error",
                            text: data.errortext,
                            type: "error"
                        });
                        //alert(JSON.stringify(data.errortext));
                        return false;
                    }
                } else {
                    new PNotify({
                        title: "Success",
                        text: "The task has been assigned to selected team.",
                        type: "success",
                        hide: true
                    });

                    jQuery(currentbutton).closest(".ticketstatusaction").html(
                        '<button type="button" class="btn bg-success-700" data-teamname=' +
                        teamname +
                        " data-teamid=" +
                        teamid +
                        ">" +
                        teamname +
                        "</button>");
                    return false;
                }
            });
        });
});

function initDocument() {
    initControls();
    loadUsersTagInputs();
    loadTeamsTagInputs();
}

function initControls() {
    $("#ticketprojectid").select2();
    $("#ticketskillid").select2();
    $("#StartDate").datepicker();
    $("#EndDate").datepicker();
}

function loadUsersTagInputs() {
    // Use Bloodhound engine
    var users = new Bloodhound({
        datumTokenizer: Bloodhound.tokenizers.obj.whitespace("text"),
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        limit: 10,
        prefetch: {
            url: "/tickets/PrefetchUsers",
            prepare: function (settings) {
                settings.type = "POST";
                settings.contentType = "application/json; charset=UTF-8";
                return settings;
            },
            remote: function (query, settings) {
                settings.type = "POST";
                settings.data = { q: query, foo: "bar" }; // you can pass some data if you need to
                return settings;
            }
        }
    });

    // Kicks off the loading/processing of `local` and `prefetch`
    users.initialize();

    // Define element
    elt = $(".assignedusersinput");

    // Initialize
    elt.tagsinput({
        itemValue: "value",
        itemText: "text"
    });

    // Attach Typeahead
    elt.tagsinput("input").typeahead(null,
        {
            name: "assignedusers",
            valueKey: "value",
            displayKey: "text",
            source: users.ttAdapter(),
            templates: "<p>{{text}}</p>"
        }).bind("typeahead:selected",
            $.proxy(function (obj, datum) {
                this.tagsinput("add", datum);
                this.tagsinput("input").typeahead("val", "");
            },
                elt));
}

function loadTeamsTagInputs() {
    // Use Bloodhound engine
    var teams = new Bloodhound({
        datumTokenizer: Bloodhound.tokenizers.obj.whitespace("text"),
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        limit: 10,
        prefetch: {
            url: "/tickets/PrefetchTeams",
            prepare: function (settings) {
                settings.type = "POST";
                settings.contentType = "application/json; charset=UTF-8";
                return settings;
            },
            remote: function (query, settings) {
                settings.type = "POST";
                settings.data = { q: query, foo: "bar" }; // you can pass some data if you need to
                return settings;
            }
        }
    });

    // Kicks off the loading/processing of `local` and `prefetch`
    teams.initialize();

    // Define element
    elt = $(".assignedteaminput");

    // Initialize
    elt.tagsinput({
        itemValue: "value",
        itemText: "text"
    });

    // Attach Typeahead
    elt.tagsinput("input").typeahead(null,
        {
            name: "assignedteams",
            valueKey: "value",
            displayKey: "text",
            source: teams.ttAdapter(),
            templates: "<p>{{text}}</p>"
        }).bind("typeahead:selected",
            $.proxy(function (obj, datum) {
                this.tagsinput("add", datum);
                this.tagsinput("input").typeahead("val", "");
            },
                elt));
}

function userClicked(el) {
}

/*
    ** Preview Merge Model
    */
jQuery(document).ready(function () {
    jQuery(document).on("click",
        ".previewticketdetail",
        function () {
            var id = $(this).data("id");
            $("#firstticketid").val(id);
            $("#ticketMergeModal").modal("show");
        });

    jQuery(document).on("click",
        "#searchmergabletickets",
        function () {
            var subject = $(".subjectforsearch").val().trim();
            var Criteria = "subject";
            //var Criteria = $("input[name='criteria']:checked").val().trim();
            $.ajax({
                url: "/Tickets/Searchbysubject",
                type: "Get",
                data: {
                    subject: subject,
                    criteria: Criteria
                },
                cache: false,
                async: true,
                success: function (data) {
                    document.getElementById("ticketstomerge").innerHTML = data.Tickets;
                    //alert(data.Tickets);
                }
            });
            //alert(subject);
        });

    //Preview Merge Model
    jQuery(document).on("click",
        "#Mergeticket",
        function () {
            var firstticketid = $("#firstticketid").val();
            var selectedarray = [];
            $("input[name='selectformerge']:checked").each(function () {
                var x = $(this).val();
                selectedarray.push(x);
            });
            if (selectedarray.length <= 0) {
                swal({
                    title: "Please select any ticket to merge !",
                    confirmButtonColor: "#2196F3"
                });
            } else {
                swal({
                    title: "Are you sure?",
                    text: "You will not be able to recover this this Ticket!",
                    type: "warning",
                    showCancelButton: true,
                    confirmButtonColor: "#EF5350",
                    confirmButtonText: "Yes, Merge it!",
                    cancelButtonText: "No, cancel pls!",
                    closeOnConfirm: false,
                    closeOnCancel: false
                },
                    function (isConfirm) {
                        if (isConfirm) {
                            $.ajax({
                                url: "/Tickets/MergeMultipleTickets",
                                type: "Post",
                                data: {
                                    firstticketid: firstticketid,
                                    selectedtickets: selectedarray
                                },
                                cache: false,
                                async: true,
                                success: function (data) {
                                    if (!data.error) {
                                        swal({
                                            title: "Merge!",
                                            text: "Your ticket is successfully merged",
                                            confirmButtonColor: "#66BB6A",
                                            type: "success"
                                        });
                                        if (isConfirm) {
                                            $("#ticketMergeModal").modal("hide");
                                            $(".subjectforsearch").val("");
                                            $("#ticketstomerge").html("");
                                        }
                                    } else {
                                        swal({
                                            title: "Error",
                                            text: data.Message,
                                            confirmButtonColor: "#2196F3",
                                            type: "error"
                                        });
                                    }
                                }
                            });
                        } else {
                            swal({
                                title: "Cancelled",
                                text: "Your tickect is not merged :)",
                                confirmButtonColor: "#2196F3",
                                type: "error"
                            });
                        }
                    });
            }
        });

    jQuery(document).on("click",
        ".add-fav-ticket",
        function () {
            var button = $(this);
            var ticketid = button.closest("td").data("ticketid");
            button.removeClass("add-fav-ticket").removeClass("fa-star-o").addClass("fa-star")
                .addClass("remove-fav-ticket");
            $.ajax({
                url: "/Tickets/AddFavouriteTickets",
                type: "Post",
                data: {
                    ticketid: ticketid
                },
                success: function (data) {
                    if (data.success == 1) {
                        new PNotify({
                            title: "Success",
                            text: "Ticket flag successfully added ",
                            type: "success",
                        });
                    } else {
                        new PNotify({
                            title: "Success",
                            text: "Error occour! ticket flag is not added",
                            type: "error"
                        });
                        button.removeClass("remove-fav-ticket").removeClass("fa-star").addClass("fa-star-o")
                            .addClass("add-fav-ticket");
                    }
                }
            });
        });

    jQuery(document).on("click",
        ".remove-fav-ticket",
        function () {
            var button = $(this);
            var ticketid = button.closest("td").data("ticketid");
            button.removeClass("remove-fav-ticket").removeClass("fa-star").addClass("fa-star-o")
                .addClass("add-fav-ticket");
            $.ajax({
                url: "/Tickets/RemoveFavouriteTickets",
                type: "Post",
                data: {
                    ticketid: ticketid
                },
                success: function (data) {
                    if (data.success == 1) {
                        new PNotify({
                            title: "Success",
                            text: "Ticket flag removed successfully ",
                            type: "success",
                        });
                    } else {
                        new PNotify({
                            title: "Error",
                            text: "Error Occour! flag is not remove try again",
                            type: "error"
                        });
                        button.removeClass("add-fav-ticket").removeClass("fa-star-o").addClass("fa-star")
                            .addClass("remove-fav-ticket");
                    }
                }
            });
        });

    jQuery(document).on("click",
        "#closeMergeButton",
        function () {
            $(".subjectforsearch").val("");
            $("#ticketstomerge").html("");
        });

    $(document).on("click",
        ".add-new-skill",
        function () {
            $("#skillname").val("");
            $("#AddSkillModal").modal("toggle");
        });

    $(document).on("click",
        ".add-new-project",
        function () {
            jQuery("#clientid").select2();
            $("#AddProjectModal").modal("toggle");
        });

    $("#AddProjectModal").on("hidden.bs.modal",
        function () {
            $(this).find("#projectname").val("");
            $(this).find("textarea,select").val("").end();
        });

    jQuery(document).on("click",
        ".add-project",
        function () {
            var clientid = $("#clientid").val();
            var projectname = $("#projectname").val();
            if (clientid == null || clientid == "") {
                sweetAlert("Sorry", "Please select a client from the list.", "error");
                return false;
            }
            if (projectname == null || projectname == "") {
                sweetAlert("Sorry", "Please add the project name.", "error");
                return false;
            }
            $.ajax({
                url: "/home/AddProject/",
                data: {
                    clientid: clientid,
                    projectname: projectname
                },
                type: "POST",
            }).done(function (data) {
                if (data.error) {
                    alert(data.message);
                    return false;
                } else {
                    $("#ticketprojectid").append($("<option>",
                        {
                            value: data.prid,
                            text: data.prname
                        }));

                    new PNotify({
                        title: "Success",
                        text: "The project has been successfully added.",
                        type: "success"
                    });
                }
                jQuery("#AddProjectModal").modal("hide");
            });
        });

    jQuery(document).on("click",
        ".add-skill",
        function () {
            var skillname = $("#skillname").val();
            if (skillname == "" || skillname == null) {
                sweetAlert("Sorry", "Please add the skill name.", "error");
                return false;
            }
            $.ajax({
                url: "/home/AddSkill/",
                data: {
                    name: skillname
                },
                type: "POST"
            }).done(function (data) {
                if (data.error) {
                    alert(data.message);
                    return false;
                } else {
                    $("#ticketskillid").append($("<option>",
                        {
                            value: data.skid,
                            text: data.skname
                        }));

                    new PNotify({
                        title: "Success",
                        text: "The skill has been added successfully.",
                        type: "success"
                    });
                }
                jQuery("#AddSkillModal").modal("hide");
            });
        });

    jQuery(document).on("click",
        ".mergemultitask",
        function () {
            //var firstticketid = $("#firstticketid").val();
            $("input.selticket:checkbox:checked").each(function () {
                var value = $(this).val();
                var html = $(this).closest("tr").find(".ticketdetail").html();
                html = '<tr id="' +
                    value +
                    '"><td style="width:5px;"><input type="radio" name="selradio" class="mergeinto" value="' +
                    value +
                    '" /></td><td style="width:5px;"><input type="checkbox" class="mergeto" value="' +
                    value +
                    '" /></td><td class="merge-table">' +
                    html +
                    "</td></tr>";
                $("#tasktomerge").append(html);
            });
            $("#ticketglobalMergeModal").modal("show");
        });
    jQuery(document).on("click",
        ".mergeinto",
        function () {
            $(".mergeto").attr("disabled", false).attr("checked", false);
            $(this).closest("tr").find("input.mergeto").attr("disabled", true);
        });
    jQuery(document).on("click",
        ".merge_global_ticket",
        function () {
            var mergeinto = $("input[name=selradio]:checked").val();
            var selectedarray = [];
            $("input.mergeto:checkbox:checked").each(function () {
                var selected = $(this).val();
                selectedarray.push(selected);
            });

            if (selectedarray.length <= 0) {
                swal({
                    title: "Please select any ticket to merge !",
                    confirmButtonColor: "#2196F3"
                });
            } else {
                swal({
                    title: "Are you sure?",
                    text: "You will not be able to recover this this Ticket!",
                    type: "warning",
                    showCancelButton: true,
                    confirmButtonColor: "#EF5350",
                    confirmButtonText: "Yes, Merge it!",
                    cancelButtonText: "No, cancel pls!",
                    closeOnConfirm: false,
                    closeOnCancel: false
                },
                    function (isConfirm) {
                        if (isConfirm) {
                            $.ajax({
                                url: "/Tickets/MergeMultipleTickets",
                                type: "Post",
                                data: {
                                    firstticketid: mergeinto,
                                    selectedtickets: selectedarray
                                },
                                cache: false,
                                async: true,
                                success: function (data) {
                                    if (!data.error) {
                                        swal({
                                            title: "Merge!",
                                            text: "Your ticket is successfully merged",
                                            confirmButtonColor: "#66BB6A",
                                            type: "success"
                                        });
                                        if (isConfirm) {
                                            $(".selticket").prop("checked", false);
                                            $("#ticketglobalMergeModal").modal("hide");
                                            if (data.fails > 0) {
                                                var row;
                                                var messagesarray = data.message.split("-");
                                                var titles = data.tickets.split("-");
                                                for (let i = 0; i < data.fails; i++) {
                                                    row = row +
                                                        "<tr><td>" +
                                                        titles[i] +
                                                        "</td><td>" +
                                                        messagesarray[i] +
                                                        "</td></tr>";
                                                }
                                                $("#errorstatus").append(row);
                                                $("#status").modal("show");
                                            }

                                            if (data.successfullmergeids != "" && data.successfullmergeids != null) {
                                                var removerows = data.successfullmergeids.split("-");
                                                for (let i = 0; i < removerows.length; i++) {
                                                    $("#" + removerows[i]).remove();
                                                }
                                            }
                                        }
                                        return false;
                                    } else {
                                        //console.log(data.Message);
                                        swal({
                                            title: "Error",
                                            text: data.Message,
                                            confirmButtonColor: "#2196F3",
                                            type: "error"
                                        });
                                    }
                                }
                            });
                        } else {
                            swal({
                                title: "Cancelled",
                                text: "Your tickect is not merged :)",
                                confirmButtonColor: "#2196F3",
                                type: "error"
                            });
                        }
                    });
            }
        });
    $("#ticketglobalMergeModal").on("hidden.bs.modal",
        function () {
            $("#tasktomerge").html("");
        });



    /*Multiple Ticket Assignment*/
    function initDocumentMultipleTicketAssigned() {
        loadUsersTag();
        loadTeamsTag();
    }

    jQuery(document).on("click",
        ".multiassign",
        function () {
            // Make sure at least one ticket is selected.
            if ($(".selticket:checked").length == 0) {
                new PNotify({
                    title: "Warning!",
                    text: "Please select at least one ticket for assignment!",
                    type: "warning"
                });
                return false;
            }

            // Open Popup for multi assignments.
            $("#btnassignticket").removeClass("saveticketassignment").addClass("assignmultitickets");
            initDocumentMultipleTicketAssigned();
            $("#assignuseradmin").modal("toggle");
            return false;
        });
    jQuery(".assignedusers").on("beforeItemAdd",
        function (event) {
            var tag = event.item;
            $.ajax({
                url: "/tickets/PrefetchSingleTeams?UserID=" + tag.value,
                type: "GET",
            }).done(function (data) {
                if (data.error) {
                    alert(JSON.stringify(data.errortext));
                    return false;
                } else {
                    jQuery(".assignedteams").tagsinput("add", { value: data.teamid, text: data.teamName });
                    return false;
                }
            });
        });

    jQuery(".assignedusers").on("beforeItemRemove",
        function (event) {
            var tag = event.item;
            var ticketid = jQuery("#AssignmentDialogTicketId").val();

            if (!event.options || !event.options.preventPost) {
                jQuery.post("/Tickets/RemoveTicketUser",
                    { id: ticketid, userid: tag.value },
                    function (response) {
                        if (!response.success) {
                            jQuery(".assignedusersinput").tagsinput("add", tag, { preventPost: true });
                        }
                        if (response.success) {
                            new PNotify({
                                title: "Success!",
                                text: response.messagetext,
                                type: "success"
                            });
                            //jQuery('.assignedusersinput').tagsinput('remove', { value: tag.value, text: tag.text });
                        }
                        return true;
                    });
            }
        });

    jQuery(".assignedteams").on("beforeItemRemove",
        function (event) {
            var tag = event.item;
            var ticketid = jQuery("#AssignmentDialogTicketId").val();

            if (!event.options || !event.options.preventPost) {
                jQuery.post("/Tickets/RemoveTicketteam",
                    { id: ticketid, teamid: tag.value },
                    function (response) {
                        if (!response.success) {
                            // Re-add the tag since there was a failure
                            // "preventPost" here will stop this ajax call from running when the tag is added
                            jQuery(".assignedteaminput").tagsinput("add", tag, { preventPost: true });
                        }
                        if (response.success) {
                            new PNotify({
                                title: "Success!",
                                text: response.messagetext,
                                type: "success"
                            });
                        }
                    });
            }
            // event.item: contains the item
            // event.cancel: set to true to prevent the item getting removed
        });
    function loadUsersTag() {
        // Use Bloodhound engine
        var users = new Bloodhound({
            datumTokenizer: Bloodhound.tokenizers.obj.whitespace("text"),
            queryTokenizer: Bloodhound.tokenizers.whitespace,
            limit: 10,
            prefetch: {
                url: "/tickets/PrefetchUsers",
                prepare: function (settings) {
                    a
                    settings.type = "POST";
                    settings.contentType = "application/json; charset=UTF-8";
                    return settings;
                },
                remote: function (query, settings) {
                    settings.type = "POST";
                    settings.data = { q: query, foo: "bar" }; // you can pass some data if you need to
                    return settings;
                }
            }
        });

        // Kicks off the loading/processing of `local` and `prefetch`
        users.initialize();

        // Define element
        elt = $(".assignedusers");

        // Initialize
        elt.tagsinput({
            itemValue: "value",
            itemText: "text"
        });

        // Attach Typeahead
        elt.tagsinput("input").typeahead(null,
            {
                name: "assignedusers",
                valueKey: "value",
                displayKey: "text",
                source: users.ttAdapter(),
                templates: "<p>{{text}}</p>"
            }).bind("typeahead:selected",
                $.proxy(function (obj, datum) {
                    this.tagsinput("add", datum);
                    this.tagsinput("input").typeahead("val", "");
                },
                    elt));
    }
    function loadTeamsTag() {
        // Use Bloodhound engine
        var teams = new Bloodhound({
            datumTokenizer: Bloodhound.tokenizers.obj.whitespace("text"),
            queryTokenizer: Bloodhound.tokenizers.whitespace,
            limit: 10,
            prefetch: {
                url: "/tickets/PrefetchTeams",
                prepare: function (settings) {
                    settings.type = "POST";
                    settings.contentType = "application/json; charset=UTF-8";
                    return settings;
                },
                remote: function (query, settings) {
                    settings.type = "POST";
                    settings.data = { q: query, foo: "bar" }; // you can pass some data if you need to
                    return settings;
                }
            }
        });

        // Kicks off the loading/processing of `local` and `prefetch`
        teams.initialize();

        // Define element
        elt = $(".assignedteams");

        // Initialize
        elt.tagsinput({
            itemValue: "value",
            itemText: "text"
        });

        // Attach Typeahead
        elt.tagsinput("input").typeahead(null,
            {
                name: "assignedteams",
                valueKey: "value",
                displayKey: "text",
                source: teams.ttAdapter(),
                templates: "<p>{{text}}</p>"
            }).bind("typeahead:selected",
                $.proxy(function (obj, datum) {
                    this.tagsinput("add", datum);
                    this.tagsinput("input").typeahead("val", "");
                },
                    elt));
    }

});





