$(function() {
    /*
        ** Assign current ticket.
        */
    var tr;
    jQuery(document).on("click",
        ".assignticket",
        function() {
            $("#MyModel-Content").html("");
            //$("#btnassignticket").removeClass("assignmultitickets").addClass("saveticketassignment");
            // Clear any existing users checked.
            $(".teammember").each(function() {
                $(this).prop("checked", false);
            });
            tr = $(this).closest("tr");
            var ticketid = $(this).data("ticketid");
            var projectid = $(this).data("projectid");
            var skillid = $(this).data("skillid");
            var clientid = $("#currentclient").val();
            $.ajax({
                url: "/TicketsByClient/AssignUsersModel/",
                data: {
                    clientid: clientid,
                },
                type: "GET",
            }).done(function(data) {
                if (data.error) {
                    alert("error");
                    return false;
                }
                $("#MyModel-Content").html(data);
                if (projectid != "") {
                    $("#projectid_assignuser").val(projectid);
                }
                if (skillid != "") {
                    $("#skillid_assignuser").val(skillid);
                }
                $("#assignticket_ticketid").val(ticketid);
            });

            $("#MyModel").modal("toggle");
        });

    // @@ Save Current Ticket Assignments.
    jQuery(document).on("click",
        ".saveticketassignment",
        function() {
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
            if ($(".teammember:checked").length == 0) {
                sweetAlert("Sorry", "Please select at least one team member to assign the ticket.", "error");
                return false;
            }

            var teamMembers = [];
            $(".teammember:checked").each(function() {
                teamMembers.push($(this).data("userid"));
            });
            var teamMembersCSV = teamMembers.join(",");

            $.ajax({
                url: "/Tickets/AssignTicket/",
                data: {
                    ticketid: ticketid,
                    projectid: projectid,
                    skillid: skillid,
                    users: teamMembersCSV
                },
                type: "POST",
            }).done(function(data) {
                if (data.error) {
                    alert(data.error);
                    return false;
                }

                // remove the modal
                $("#MyModel").modal("toggle");
                $("#MyModel-Content").html("");
                $("body").on("hidden.bs.modal",
                    "#MyModel",
                    function() {
                        $(this).removeData("bs.modal");
                    });

                // animate remove the ticket row.
                //var ticketrow = $(document).find("tobdy#conversationtbody tr#" + data.ticketid);
                tr.animate({ backgroundColor: "yellow" }, 1000).fadeOut(1000,
                    function() {
                        tr.remove();
                    });

                //var pcount = $("#2.conversationstatus").text();
                //pcount = parseInt(pcount);
                //pcount = pcount + 1;
                //$("#2.conversationstatus").text(pcount);
                //var cscount = $("#1.conversationstatus").text();
                //cscount = parseInt(cscount);
                //cscount = cscount - 1;
                //$("#1.conversationstatus").text(cscount);
                new PNotify({
                    title: "Success",
                    text: "The task has been assigned to selected users.",
                    type: "success"
                });
                $("#projectid_assignuser").val("").trigger("change");
                $("#skillid_assignuser").val("").trigger("change");
                $(".teammember:checked").prop("checked", false);
                return false;
            });
        });
});