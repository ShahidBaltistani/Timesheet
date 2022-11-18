$('[data-toggle="tooltip"]').tooltip();
jQuery(function() {
    jQuery(document).on("click",
        ".selectall",
        function() {
            var checkstatus = this.checked;
            jQuery("#notificationcontainer").find("tr").each(function() {
                if (!jQuery(this).hasClass("lf-hidden")) {
                    jQuery(this).find(".selectnotification:checkbox").prop("checked", checkstatus);
                }
            });

            //jQuery(".selecttask").prop('checked', this.checked);
        });

    jQuery(document).on("click",
        ".loadmorenotifications",
        function() {
            var page = parseInt(jQuery(this).data("pagenumber"));
            page++;
            loadNotifications(page);
        });

    //jQuery(document).on('click', '#hreadall', function () {
    //    var notification = [];
    //    var unorderlist = $('.header-notification');
    //    $('.header-notification li').each(function (e) {
    //        var value = $(this).data('id');
    //        notification.push(value);
    //    });
    //    unorderlist.html("");
    //    unorderlist.append('<li class="media" style="padding-top:20px;padding-left:50px;"> No unread notification found </li>');
    //    //alert(notification);
    //    if (notification != null && notification.length > 0) {
    //        $.post("/notification/readMultipleNotification/", { notification }, function (data) {
    //            if (data.error) {
    //                new PNotify({
    //                    title: 'Warning!',
    //                    text: data.errortext,
    //                    type: 'warning'
    //                });
    //            }
    //            return false;
    //        });
    //    }
    //    else {
    //        new PNotify({
    //            title: 'Warning!',
    //            text: "Unread Notification not found",
    //            type: 'warning'
    //        });
    //    }
    //    return false;
    //});

    jQuery(document).on("click",
        "#hreadall",
        function() {
            $.post("/notification/ReadAll/",
                function(data) {
                    if (data.error) {
                        new PNotify({
                            title: "Warning!",
                            text: data.errortext,
                            type: "warning"
                        });
                    } else {
                        new PNotify({
                            title: "Success",
                            text: data.message,
                            type: "success"
                        });
                    }
                    return false;
                });
            return false;
        });

    jQuery(document).on("click",
        ".hmarkread",
        function() {
            var markicon = $(this);
            var id = markicon.data("id");
            var ulele = $(".header-notification");

            var listitem = markicon.closest("li");
            listitem.remove();

            $.post("/notification/readnotification/",
                { id },
                function(data) {
                    if (data.error) {
                    }
                    return false;
                });
            if (!ulele.find("li").length > 0) {
                ulele.append(
                    '<li class="media" style="padding-top:20px;padding-left:50px;"> No unread notification found </li>');
            }
            return false;
        });

    jQuery(document).on("click",
        ".markread",
        function() {
            var markicon = $(this);
            var id = markicon.data("id");

            $.post("/notification/readnotification/",
                { id },
                function(data) {
                    if (!data.error) {
                        markicon.find("i.icon-checkmark4").removeClass("notification-color-unread")
                            .addClass("notification-color-read");
                        markicon.closest("tr").attr("data-status", true);
                        markicon.removeClass("markread").addClass("markunread");
                        markicon.attr("title", "Mark Unread");
                    }
                    return false;
                });
        });

    jQuery(document).on("click",
        ".markunread",
        function() {
            var markicon = $(this);
            var id = markicon.data("id");

            $.post("/notification/UnreadNotification/",
                { id },
                function(data) {
                    if (!data.error) {
                        markicon.find("i.icon-checkmark4").removeClass("notification-color-read")
                            .addClass("notification-color-unread");
                        markicon.closest("tr").attr("data-status", false);
                        markicon.removeClass("markunread").addClass("markread");
                        markicon.attr("title", "Mark Read");
                    }
                    return false;
                });
        });

    jQuery(document).on("click",
        "#unreadall",
        function() {
            var notification = [];
            $(".selectnotification:checked").each(function() {
                var value = $(this).val();
                var check = $(this).closest("tr").data("status");
                if (check == "True") {
                    notification.push(value);
                }
            });
            if (notification != null && notification.length > 0) {
                $.post("/notification/UnreadMultipleNotification/",
                    { notification },
                    function(data) {
                        if (data.error) {
                            new PNotify({
                                title: "Warning!",
                                text: data.errortext,
                                type: "warning"
                            });
                        } else {
                            location.reload();
                        }
                        return false;
                    });
            } else {
                new PNotify({
                    title: "Warning!",
                    text: "please select atleast one read entry",
                    type: "warning"
                });
            }
        });

    jQuery(document).on("click",
        "#readall",
        function() {
            var notification = [];
            $(".selectnotification:checked").each(function() {
                var value = $(this).val();
                var check = $(this).closest("tr").data("status");
                if (check == "False") {
                    notification.push(value);
                }
            });
            if (notification != null && notification.length > 0) {
                $.post("/notification/readMultipleNotification/",
                    { notification },
                    function(data) {
                        if (data.error) {
                            new PNotify({
                                title: "Warning!",
                                text: data.errortext,
                                type: "warning"
                            });
                        } else {
                            location.reload();
                        }
                    });
            } else {
                new PNotify({
                    title: "Warning!",
                    text: "please select atleast one unread notification",
                    type: "warning"
                });
            }
        });

    jQuery(document).on("change",
        ".notification-on-off",
        function() {
            if ($(this).is(":checked")) {
                TurnOnNotifiacation();
            } else {
                TurnOffNotifiacation();
            }
        });
});

function TurnOnNotifiacation() {
    $.ajax({
        url: "/notification/notificationOn/",
        type: "Get",
    }).done(function(data) {
    });
}

function TurnOffNotifiacation() {
    $.ajax({
        url: "/notification/notificationOff/",
        type: "Get",
    }).done(function(data) {
    });
}

function loadNotifications(page) {
    $.post("/notification/paginatedNotification/",
        { page },
        function(data) {
            // remove current row.
            jQuery("#loadmorenotification").remove();
            // update tickets.
            $("#notificationcontainer").append(data.NotificationItem);
            return false;
        });
    return false;
}