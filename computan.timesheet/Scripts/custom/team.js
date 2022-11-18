jQuery(document).ready(function() {
    // initialize the document.
    initDocument();

    // If it is not an admin, automatically filter tickets for logged in user.
    filterTicketsForTeam();

    /*************************************************************************
     * Load projects based on the selected client.
     *************************************************************************/
    jQuery("#clients").change(function() {
        $("#projects").val("").trigger("change");
        var clientid = $(this).val();
        if (clientid == "") {
            $.getJSON("/tickettimeLogs/loadallprojects",
                function(data) {
                    var select = $("#projects");
                    //select.select2('destroy');
                    select.empty();
                    select.append($("<option/>",
                        {
                            value: "",
                            text: "Please Select"
                        }));
                    $.each(data,
                        function(index, itemData) {
                            select.append($("<option/>",
                                {
                                    value: itemData.Value,
                                    text: itemData.Text
                                }));
                            //select.select2({ width: 450 });
                        });
                });
        } else {
            $.getJSON("/tickettimeLogs/loadprojectsbyclient",
                { id: clientid },
                function(data) {
                    var select = $("#projects");
                    //select.select2('destroy');
                    select.empty();
                    select.append($("<option/>",
                        {
                            value: "",
                            text: "Select a project"
                        }));
                    $.each(data,
                        function(index, itemData) {
                            select.append($("<option/>",
                                {
                                    value: itemData.Value,
                                    text: itemData.Text
                                }));
                        });
                });
        }
    });

    /*************************************************************************
    * Archived the ticket from done column
    *************************************************************************/

    jQuery(document).on("click",
        ".archiveTicket",
        function() {
            // Fetch user input data.
            //var temp = jQuery(this).parent().parent().parent().parent().parent().parent().parent().data("ticketid");
            //alert(temp);
            //var ticketid = jQuery(this).closest(".modal-dialog").data("ticketid");
            var ticketid = $(this).data("ticketid");

            $.ajax({
                url: "/tickets/ArchivedTicket/",
                data: {
                    ticketcsv: ticketid
                },
                type: "POST",
            }).done(function(data) {
                if (data.error) {
                    alert(data.errortext);
                    return false;
                }
                $.each(data.tickets,
                    function(index, value) {
                        // remove the ticket row with animation.
                        var ticketrow = $("li#" + value.toString());
                        ticketrow.animate({ backgroundColor: "yellow" }, 1000).fadeOut(1000,
                            function() {
                                $(ticketrow).remove();
                            });
                        // update the counts.
                        var pcount = $("#donecounter").html();
                        pcount = parseInt(pcount);
                        pcount = pcount - 1;
                        $("#donecounter").html(pcount);
                        //var cscount = $("#1.conversationstatus").text();
                        //cscount = parseInt(cscount);
                        //cscount = cscount - 1;
                        //$("#1.conversationstatus").text(cscount);
                    });
                return false;
            });
        });

    /*************************************************************************
     * Filter tikcets when load button is clicked.
     *************************************************************************/
    jQuery("#btnLoad").click(function() {
        filter();
    });

    /************************************************************************
     * Open Dialog box to move ticket to another team.
     *************************************************************************/
    jQuery(document).on("click",
        ".moveteam",
        function() {
            if (jQuery(this).closest("li").hasClass("has-assignment")) {
                jQuery("#MoveTeamModel").find(".assignmentwarning").removeClass("hide");
            } else {
                jQuery("#MoveTeamModel").find(".assignmentwarning").addClass("hide");
            }

            var ticketid = jQuery(this).data("ticketid");
            jQuery(".move-team").data("ticketid", ticketid);

            var teamid = jQuery(this).data("btnteamid");
            jQuery(".move-team").data("btnteamid", teamid);

            jQuery("#MoveTeamModel").modal("toggle");
        });

    /************************************************************************
    * Send call to save ticket to another team.
    *************************************************************************/
    jQuery(document).on("click",
        ".move-team",
        function() {
            var teamID = jQuery(this).data("btnteamid");
            var TicketID = jQuery(this).data("ticketid");
            var NewTeamID = $("#tid").find(":selected").val();
            // Validate required information.
            if (NewTeamID == "") {
                sweetAlert("Sorry", "Please select a team from the list.", "error");
                return false;
            }
            if (NewTeamID == teamID) {
                sweetAlert("Sorry", "Already assigned to this team", "error");
                return false;
            }

            $.ajax({
                url: "/Home/ChangeTeam/",
                data: {
                    teamid: teamID,
                    ticketid: TicketID,
                    newteamid: NewTeamID
                },
                type: "GET",
            }).done(function(data) {
                if (data.error) {
                    sweetAlert("Sorry", data.TextContext, "error");
                    return false;
                } else {
                    new PNotify({
                        title: "Success",
                        text: data.TextContext,
                        type: "success",
                        hide: true
                    });
                    jQuery("#" + TicketID).remove();
                    jQuery("#MoveTeamModel").modal("toggle");
                }
            });
        });

    /************************************************************************
    * Open Assign Ticket Modal.
    *************************************************************************/
    jQuery(document).on("click",
        ".assignticketaction",
        function() {
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
                url: "/Home/FetchUsersAndTeams/",
                data: {
                    Ticketid: ticketid
                },
                type: "GET",
            }).done(function(data) {
                if (data.error == 1) {
                    sweetAlert("Sorry", data.TextContext, "error");
                    //alert('test');
                    return false;
                } else {
                    if (data.Users != null) {
                        $(data.Users).each(function(index, value) {
                            jQuery(".assignedusersinput").tagsinput("add", { value: value.Id, text: value.FullName });
                        });
                    }

                    if (data.Teams != null) {
                        $(data.Teams).each(function(index, value) {
                            jQuery(".assignedteaminput").tagsinput("add", { value: value.id, text: value.name });
                        });
                    }
                    $.ajax({
                        url: "/Home/FetchEstimatetasktime/",
                        data: {
                            Ticketid: ticketid
                        },
                        type: "GET",
                    }).done(function(data) {
                        jQuery("#estimateTimeInput").val(data.estimatetime).trigger("change");
                        jQuery("#EstimateTimeHidden").val(data.estimatetime).trigger("change");
                    });

                    jQuery(".tag").removeClass("label-info").addClass("label-success");
                    jQuery("#ticketAssignModal").modal("toggle");
                }
            });
        });

    $(document).on("click",
        ".add-new-skill",
        function() {
            $("#skillname").val("");
            $("#AddSkillModal").modal("toggle");
        });

    $(document).on("click",
        ".add-new-project",
        function() {
            jQuery("#clientid").select2();
            $("#AddProjectModal").modal("toggle");
        });

    $("#AddProjectModal").on("hidden.bs.modal",
        function() {
            $(this).find("#projectname").val("");
            $(this).find("textarea,select").val("").end();
        });

    jQuery(document).on("click",
        ".add-project",
        function() {
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
            }).done(function(data) {
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
        function() {
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
            }).done(function(data) {
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
});

function initDocument() {
    // initialize all the controls.
    initControls();
}

function initControls() {
    // Search filter controls.
    jQuery("#AssignmentFromDate").datepicker();
    jQuery("#AssignmentToDate").datepicker();
    jQuery("#projects").select2();
    jQuery("#clients").select2();
    jQuery("#ByUsers").select2();
    jQuery("#ToUsers").select2();

    // Assign Ticket Dialog
    $("#ticketprojectid").select2();
    $("#ticketskillid").select2();
    $("#StartDate").datepicker();
    $("#EndDate").datepicker();
    loadUsersTagInputs();
    loadTeamsTagInputs();

    // Move Tiket Dialog, Team dropdown
    jQuery("#tid").select2();
}

function loadUsersTagInputs() {
    // Use Bloodhound engine
    var users = new Bloodhound({
        datumTokenizer: Bloodhound.tokenizers.obj.whitespace("text"),
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        limit: 10,
        prefetch: {
            url: "/tickets/PrefetchUsers",
            prepare: function(settings) {
                settings.type = "POST";
                settings.contentType = "application/json; charset=UTF-8";
                return settings;
            },
            remote: function(query, settings) {
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
        $.proxy(function(obj, datum) {
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
            prepare: function(settings) {
                settings.type = "POST";
                settings.contentType = "application/json; charset=UTF-8";
                return settings;
            },
            remote: function(query, settings) {
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
        $.proxy(function(obj, datum) {
                this.tagsinput("add", datum);
                this.tagsinput("input").typeahead("val", "");
            },
            elt));
}

function filterTicketsForTeam() {
    if (jQuery("#IsAdmin").val() == 0) {
        var userid = jQuery("#CurrentUserId").val();
        jQuery("#ToUsers").val(userid).trigger("change");
        //filter();
    }
}

function filter() {
    //alert('TeamID= ' + $("#teamid").val());
    //alert('ByUsers= ' + $("#ByUsers").val());
    //alert('ToUsers= ' + $("#ToUsers").val());
    //alert('AssignmentFromDate= ' + $("#AssignmentFromDate").val());
    //alert('AssignmentToDate= ' + $("#AssignmentToDate").val());
    //alert('clientid= ' + $("#clients").val());
    //alert('projectid= ' + $("#tTitle").val());
    var querystring = "?teamid=" +
        $("#teamid").val() +
        "&ByUsers=" +
        $("#ByUsers").val() +
        "&ToUsers=" +
        $("#ToUsers").val() +
        "&AssignmentFromDate=" +
        $("#AssignmentFromDate").val() +
        "&AssignmentToDate=" +
        $("#AssignmentToDate").val() +
        "&clientid=" +
        $("#clients").val() +
        "&projectid=" +
        $("#projects").val() +
        "&ticketTitle=" +
        $("#tTitle").val();
    $.ajax({
        url: "/Home/teamAjax" + querystring,
        type: "GET",
    }).done(function(data) {
        $("#statusList").html(data);
        setTimeout(function() { bindEvent(); }, 50);
        return false;
    });
}

jQuery(document).on("click",
    ".add-fav-ticket",
    function() {
        var button = $(this);
        var ticketid = button.closest("li").data("itemid");
        button.removeClass("add-fav-ticket").removeClass("fa-star-o").addClass("fa-star").addClass("remove-fav-ticket");
        $.ajax({
            url: "/tickets/AddFavouriteTickets",
            type: "Post",
            data: {
                ticketid: ticketid
            },
            success: function(data) {
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
    function() {
        var button = $(this);
        var ticketid = button.closest("li").data("itemid");
        button.removeClass("remove-fav-ticket").removeClass("fa-star").addClass("fa-star-o").addClass("add-fav-ticket");
        $.ajax({
            url: "/tickets/RemoveFavouriteTickets",
            type: "Post",
            data: {
                ticketid: ticketid
            },
            success: function(data) {
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