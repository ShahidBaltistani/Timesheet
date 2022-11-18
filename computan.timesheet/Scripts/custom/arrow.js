$(document).ready(function() {
    initDocument();

    /* Panel Collapsed Code onload */
    function initDocument() {
        //initControls();
        $(".arrow-click").attr("aria-expanded", "false");
        $(".arrow-click").addClass("collapsed");
        $(".clientDashboard .panel-collapse").attr("aria-expanded", "false");
        $(".clientDashboard .panel-collapse").removeClass("in");
        $(".arrow-click i").removeClass("fa-caret-down").addClass("fa-caret-right");
        $("#assign-project").select2();
        $("#assign-skills").select2();
        $("#users").select2();
        loadUsersTagInputs();
        loadTeamsTagInputs();
    }

    $("#btnsave").on("click",
        function() {
            var id = $("#last-projectid").val();
            var projectid = $("#assign-project").val();
            var skillid = $("#assign-skills").val();
            var userscsv = $(".assignedusersinput").val();
            var teamscsv = $(".assignedteaminput").val();
            if (projectid == "") {
                sweetAlert("Sorry", "Please select a project from the list.", "error");
                return false;
            }
            if (skillid === "") {
                sweetAlert("Sorry", "Please select a skill from the list.", "error");
                return false;
            }
            if (userscsv == "" && teamscsv == "") {
                sweetAlert("Sorry! Assignment Missing", "Please assign ticket to at least one user/team.", "error");
                return false;
            }

            var selectedTickets = [];
            $("#collapsible-" + id + " input:checked").each(function() {
                selectedTickets.push($(this).val());
            });
            //$('.' + projectid).find('input[type=checkbox]').is(':checked').each(function () {
            //    selectedTickets.push($(this).val());
            //});
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
            }).done(function(data) {
                if (data.error) {
                    //var source = $("#error-notification-template").html();
                    //var template = Handlebars.compile(source);
                    //var html = template(data);
                    //$("#modelassignuseradmindiv").prepend(html);
                    $("#assignuseradmin").modal("hide");
                    return false;
                }

                $.each(data.tickets,
                    function(index, value) {
                        if (projectid === id)
                            $("#" + value).animate({
                                    backgroundColor: "yellow"
                                },
                                1000).fadeOut(1000,
                                function() {
                                    $("#" + value).remove();
                                });
                        $("input:checkbox").prop("checked", false);
                    });
                new PNotify({
                    title: "Success",
                    text: "The task has been assigned to selected users.",
                    type: "success"
                });
                $("#assign-project").val("").trigger("change");
                $("#assign-skills").val("").trigger("change");
                $(".assignedusersinput").val("").trigger("change");
                $(".assignedteaminput").val("").trigger("change");
                $("#assignuseradmin").modal("hide");
                return false;
            });
        });

    /************************************************************
    *** On Project Title Click.
    *************************************************************/

    $(".assign-multiple").on("click",
        function() {
            getSkills();
            getProjects();
            var id = $(this).parents("tr").data("id");
            if ($("." + id).find("input[type=checkbox]").is(":checked")) {
                $("#assignuseradmin").modal("toggle");
                $("#last-projectid").val(id);
                $("#assign-project").val(id).trigger("change");
            } else {
                new PNotify({
                    title: "Warning!",
                    text: "Please select at least one ticket!",
                    type: "warning"
                });
            }
        });

    $(".check-all").on("click",
        function() {
            var id = $(this).parents("tr").data("id");
            if ($(this).is(":checked")) {
                $("." + id).prop("checked", true);
            } else {
                $("." + id).prop("checked", false);
            }

            //var id = $(this).attr("data-id");
            //if ($(this).is(':checked')) {
            //    $('#' + id).find('input[type=checkbox]').each(function () {
            //        $(this).prop("checked", true);
            //    });
            //}
            //else {
            //    $('#' + id).find('input[type=checkbox]').each(function () {
            //        $(this).prop("checked", false);

            //    });
            //}
        }).change();

    $(".arrow-click").on("click",
        function() {
            var arrow = $(this).find("i");

            if (arrow.hasClass("fa-caret-right")) {
                $(arrow).removeClass("fa-caret-right").addClass("fa-caret-down");
            } else {
                arrow.removeClass("fa-caret-down");
                arrow.addClass("fa-caret-right");
            }
        });

    jQuery(".assignedusersinput").on("beforeItemAdd",
        function(event) {
            var tag = event.item;
            $.ajax({
                url: "/tickets/PrefetchSingleTeams?UserID=" + tag.value,
                type: "GET",
            }).done(function(data) {
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
        function(event) {
            var tag = event.item;
            var ticketid = jQuery("#AssignmentDialogTicketId").val();

            if (!event.options || !event.options.preventPost) {
                jQuery.post("/Tickets/RemoveTicketUser",
                    { id: ticketid, userid: tag.value },
                    function(response) {
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

    function getSkills() {
        $.ajax({
            type: "GET",
            url: "/Clients/GetSkills",
            dataType: "json",
            success: function(result) {
                $("#assign-skills").html("");
                var rows1 = "<option value='' selected disabled > Select Skills </option >";
                $("#assign-skills").append(rows1);

                $.each(result,
                    function(i, item) {
                        var rows;
                        rows = "<option value='" + item.id + "'>" + item.name + "</option>";
                        $("#assign-skills").append(rows);
                    });
            }
        });
    };

    function getProjects() {
        $.ajax({
            type: "GET",
            url: "/Clients/GetProjects",
            dataType: "json",
            success: function(result) {
                $("#assign-project").html("");
                var rows1 = "<option value='' selected disabled > Select Projects </option >";
                $("#assign-project").append(rows1);

                $.each(result,
                    function(i, item) {
                        var rows;
                        rows = "<option value='" + item.Value + "'>" + item.Text + "</option>";
                        $("#assign-project").append(rows);
                    });
            }
        });
    };
});