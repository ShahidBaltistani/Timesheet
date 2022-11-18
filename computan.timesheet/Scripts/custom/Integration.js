jQuery(function() {
    jQuery(document).on("beforeItemRemove",
        ".teamassign",
        function(event) {
            var tag = event.item;
            var othervalue = $(this).val();
            var avoid = tag.value;
            var teamcsv = othervalue.replace(avoid, "");
            teamcsv = teamcsv.replace(",,", ",");
            var projectid = $(this).closest("tr").data("id");
            if (!event.options || !event.options.preventPost) {
                $.ajax({
                    url: "/FreedCampProject/RemoveTeams",
                    type: "GET",
                    data: {
                        id: projectid,
                        teamscsv: teamcsv
                    }
                }).done(function(data) {
                    if (data.error) {
                        jQuery("#team" + projectid).tagsinput("add",
                            tag,
                            {
                                preventPost: true
                            });
                        alert(data.Message);
                        return false;
                    } else {
                        new PNotify({
                            title: "Success!",
                            text: data.Message,
                            type: "success",
                            hide: true,
                            buttons: {
                                sticker: false
                            }
                        });
                    }
                });
            }
        });
    jQuery(document).on("beforeItemRemove",
        ".userassign",
        function(event) {
            var tag = event.item;
            var othervalue = $(this).val();
            var avoid = tag.value;
            var usercsv = othervalue.replace(avoid, "");
            usercsv = usercsv.replace(",,", ",");
            var projectid = $(this).closest("tr").data("id");
            //console.log(usercsv);
            if (!event.options || !event.options.preventPost) {
                $.ajax({
                    url: "/FreedCampProject/RempoveUsers",
                    type: "GET",
                    data: {
                        id: projectid,
                        usercsv: usercsv
                    }
                }).done(function(data) {
                    if (data.error) {
                        jQuery("#User-" + projectid).tagsinput("add",
                            tag,
                            {
                                preventPost: true
                            });
                        alert(data.Message);
                        return false;
                    } else {
                        new PNotify({
                            title: "Success!",
                            text: data.Message,
                            type: "success",
                            hide: true,
                            buttons: {
                                sticker: false
                            }
                        });
                    }
                });
            }
        });

    jQuery(document).on("beforeItemAdd",
        ".teamassign",
        function(event) {
            var tag = event.item;
            //console.log(event);
            var projectid = $(this).closest("tr").data("id");
            if (!event.options || !event.options.preventPost) {
                jQuery.post("/FreedCampProject/AddTeams",
                    {
                        id: projectid,
                        teamid: tag.value
                    },
                    function(response) {
                        if (!response.error) {
                            if (response.teamadded) {
                                new PNotify({
                                    title: "Success!",
                                    text: response.Message,
                                    type: "success",
                                    hide: true,
                                    buttons: {
                                        sticker: false
                                    }
                                });
                            } else {
                                jQuery("#Team-" + projectid).tagsinput("remove", tag, { preventPost: true });
                            }
                        } else {
                            jQuery("#Team-" + projectid).tagsinput("remove", tag, { preventPost: true });
                        }
                        //reloadAssignment();
                    });
            }

            // event.item: contains the item
            // event.cancel: set to true to prevent the item getting removed
        });
    jQuery(document).on("beforeItemAdd",
        ".userassign",
        function(event) {
            var tag = event.item;
            var row = $(this).closest("tr");
            var project = row.find(".project").val();
            var skill = row.find(".skill").val();
            var projectid = $(this).closest("tr").data("id");

            if (!event.options || !event.options.preventPost) {
                if (project === "" || project === null) {
                    return false;
                }
                if (skill === "" || skill === null) {
                    return false;
                }

                $.ajax({
                    url: "/FreedCampProject/AddUsers",
                    type: "GET",
                    data: {
                        id: projectid,
                        userid: tag.value
                    }
                }).done(function(data) {
                    if (data.error) {
                        alert(data.Message);
                        return false;
                    } else {
                        if (data.teamadd) {
                            jQuery("#Team-" + projectid).tagsinput("add",
                                { value: data.team_id, text: data.teamName },
                                { preventPost: true });
                        }
                        if (!data.useradd) {
                            jQuery("#User-" + projectid).tagsinput("remove", tag, { preventPost: true });
                        }
                        new PNotify({
                            title: "Success!",
                            text: data.Message,
                            type: "success",
                            hide: true,
                            buttons: {
                                sticker: false
                            }
                        });
                    }
                });
            }
        });
    jQuery(document).on("itemAdded",
        ".userassign",
        function(event) {
            var tag = event.item;
            var row = $(this).closest("tr");
            var project = row.find(".project").val();
            var skill = row.find(".skill").val();
            var projectid = $(this).closest("tr").data("id");
            if (!event.options || !event.options.preventPost) {
                if (project === "" || project === null) {
                    new PNotify({
                        title: "Error!",
                        text: "Please select project",
                        type: "error",
                        hide: true,
                        buttons: {
                            sticker: false
                        }
                    });
                    jQuery("#User-" + projectid).tagsinput("remove", tag, { preventPost: true });
                    return false;
                }
                if (skill === "" || skill === null) {
                    new PNotify({
                        title: "Error!",
                        text: "Please select skill",
                        type: "error",
                        hide: true,
                        buttons: {
                            sticker: false
                        }
                    });
                    $("#User-" + projectid).tagsinput("remove", tag, { preventPost: true });
                    return false;
                }
            }
        });

    jQuery(document).on("change",
        ".skill",
        function() {
            var skillid = $(this).val();
            if (skillid == null || skillid == "") {
                skillid = 0;
            }
            var projectid = $(this).closest("tr").data("id");
            $.ajax({
                url: "/FreedCampProject/AddSkill",
                data: {
                    id: projectid,
                    skillid: skillid
                },
                type: "Post"
            }).done(function(data) {
                new PNotify({
                    title: "Success!",
                    text: data.Message,
                    type: "success",
                    hide: true,
                    buttons: {
                        sticker: false
                    }
                });
                return false;
            });
        });
    jQuery(document).on("change",
        ".project",
        function() {
            var tsid = $(this).val();
            if (tsid == null || tsid == "") {
                tsid = 0;
            }
            var projectid = $(this).closest("tr").data("id");
            $.ajax({
                url: "/FreedCampProject/AddProject",
                data: {
                    id: projectid,
                    projectid: tsid
                },
                type: "Post"
            }).done(function(data) {
                new PNotify({
                    title: "Success!",
                    text: data.Message,
                    type: "success",
                    hide: true,
                    buttons: {
                        sticker: false
                    }
                });
                return false;
            });
        });
});

function init() {
    $(".skill").select2();
    $(".project").select2();
    loadAssignedToTagInputs();
    loadnewTeamsTagInputs();
    loadusersandteam();
}

function loadusersandteam() {
    $.ajax({
        url: "/FreedCampProject/loadprojectuserandteam",
        type: "GET"
    }).done(function(data) {
        if (!data.error) {
            //console.log(data.model);
            $.each(data.model,
                function(key, value) {
                    var user = value.users;
                    var teams = value.teams;
                    var projectid = value.projectid;
                    $.each(user,
                        function(key, user) {
                            var tag = { value: user.userid, text: user.name };
                            jQuery("#User-" + projectid).tagsinput("add", tag, { preventPost: true });
                        });
                    $.each(teams,
                        function(key, team) {
                            var tag = { value: team.teamid, text: team.name };
                            jQuery("#Team-" + projectid).tagsinput("add", tag, { preventPost: true });
                        });
                    //jQuery("#user" + projectid).tagsinput('add', { value: userid, text: username }, { preventPost: true});
                });
        }

        return false;
    });
}

function loadnewTeamsTagInputs() {
    // Use Bloodhound engine
    var teams = new Bloodhound({
        datumTokenizer: Bloodhound.tokenizers.obj.whitespace("text"),
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        limit: 10,
        prefetch: {
            url: "/tickets/PrefetchTeams",
            prepare: function(settings) {
                settings.cache = false;
                settings.type = "POST";
                settings.contentType = "application/json; charset=UTF-8";
                return settings;
            },
            remote: function(query, settings) {
                settings.cache = false;
                settings.type = "POST";
                settings.data = { q: query, foo: "bar" }; // you can pass some data if you need to
                return settings;
            }
        }
    });

    // Kicks off the loading/processing of `local` and `prefetch`
    teams.initialize();

    var fields = $(".teamassign");
    //console.log("each function elt :");
    fields.each(function() {
        elt = $(this);
        // Initialize
        elt.tagsinput({
            itemValue: "value",
            itemText: "text"
        });
        //// Define element
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
    });
}

function loadAssignedToTagInputs() {
    // Use Bloodhound engine
    var users = new Bloodhound({
        datumTokenizer: Bloodhound.tokenizers.obj.whitespace("text"),
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        limit: 10,
        prefetch: {
            ttl: 1,
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
    users.clear();
    users.clearPrefetchCache();
    users.clearRemoteCache();
    users.initialize();

    var input = $(".userassign");
    input.each(function() {
        elt = $(this);

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
    });
    // Define element
}