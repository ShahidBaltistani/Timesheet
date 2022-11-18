$(function() {
    var id;
    var tr;
    $(document).on("click",
        ".startworking",
        function() {
            $(".usernotification").remove();
            tr = $(this).closest("tr");
            id = $(this).data("id");
            var proid = $(this).data("projectid");
            var skid = $(this).data("skillid");
            if (proid != "") {
                $("#projectid").val(proid).trigger("change");
            }
            if (skid != "") {
                $("#skillid").val(skid).trigger("change");
            }

            $.ajax({
                url: "/tickets/IsAlreadyAssigned/",
                data: {
                    id: id,
                },
                type: "POST",
            }).done(function(data) {
                if (data.error) {
                    //var source = $("#error-notification-template").html();
                    //var template = Handlebars.compile(source);
                    //var html = template(data);
                    //$("#maincontent").prepend(html);
                    sweetAlert("Sorry...", data.errortext, "error");
                    return false;
                }
                $("#myModal").modal("toggle");
                return false;
            });
        });
    $(document).on("click",
        "#startworking",
        function() {
            var pid = $("#projectid option:selected").val();
            var sid = $("#skillid option:selected").val();
            var quotedtime = $("#quotabletime").val();
            if (quotedtime == "") {
                quotedtime = 0;
            }
            if (pid == "" || sid === "") {
                sweetAlert("Sorry...", "peasle select project and skill", "error");
                return false;
            }
            $.ajax({
                url: "/tickets/ticketassignment/",
                data: {
                    id: id,
                    projectid: pid,
                    skillid: sid,
                    quotedtime: quotedtime,
                    status: "2"
                },
                type: "POST",
            }).done(function(data) {
                if (data.error) {
                    var source = $("#error-notification-template").html();
                    var template = Handlebars.compile(source);
                    var html = template(data);
                    $("#modelbodydiv").prepend(html);
                    return false;
                }
                var source = $("#success-notification-template").html();
                var template = Handlebars.compile(source);
                var html = template(data);
                $("#myModal").modal("toggle");
                //var td = tr.find('td:nth-child(4) select.itemstatus').removeAttr('disabled');
                //var tstatus = tr.find('td:nth-child(4) select.itemstatus').val();
                //if (tstatus == 1 || tstatus == 2 || tstatus == 3) {
                //    tr.find('td:nth-child(4) select.itemstatus').val(2);
                //}
                tr.animate({ backgroundColor: "yellow" }, 1000).fadeOut(1000,
                    function() {
                        tr.remove();
                    });
                var pcount = $("#2.conversationstatus").text();
                pcount = parseInt(pcount);
                pcount = pcount + 1;
                $("#2.conversationstatus").text(pcount);
                var currentstatus = $("#currentpagestatus").val();
                if (currentstatus == "") {
                    currentstatus = $("#currentstatus").val();
                }
                var cscount = $("#" + currentstatus + ".conversationstatus").text();
                cscount = parseInt(cscount);
                cscount = cscount - 1;
                $("#" + currentstatus + ".conversationstatus").text(cscount);
                $("#maincontent").prepend(html);
                return false;

                //$('#myModal').modal('toggle');
            });
        });
    var oldstatusid;
    $(document).on("focus",
        ".itemstatus",
        function() {
            oldstatusid = $(this).val();
        });
    $(document).on("change",
        ".itemstatus",
        function() {
            $(".usernotification").remove();
            tr = $(this).closest("tr");
            var id = $(this).data("id");
            var statusid = $(this).val();
            if (statusid == 1) {
                sweetAlert("Sorry...", " You can change status to back.", "error");
                $(this).val(2);
                return false;
            }
            $.ajax({
                url: "/tickets/ticketstatusupdate/",
                data: {
                    id: id,
                    status: statusid,
                },
                type: "POST",
            }).done(function(data) {
                if (data.error) {
                    var source = $("#error-notification-template").html();
                    var template = Handlebars.compile(source);
                    var html = template(data);
                    $("#maincontent").prepend(html);
                    return false;
                }
                var source = $("#success-notification-template").html();
                var template = Handlebars.compile(source);
                var html = template(data);
                tr.animate({ backgroundColor: "yellow" }, 1000).fadeOut(1000,
                    function() {
                        tr.remove();
                    });
                var pcount = $("#" + statusid + ".conversationstatus").text();
                pcount = parseInt(pcount);
                pcount = pcount + 1;
                $("#" + statusid + ".conversationstatus").text(pcount);
                var cscount = $("#" + oldstatusid + ".conversationstatus").text();
                cscount = parseInt(cscount);
                cscount = cscount - 1;
                $("#" + oldstatusid + ".conversationstatus").text(cscount);
                $("#maincontent").prepend(html);

                return false;
            });
        });
});