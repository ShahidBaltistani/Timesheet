jQuery(document).ready(function() {
    //****************************************************
    //*************** Get Tickets List By Status ************
    //****************************************************
    $(document).on("click",
        ".taskstatus",
        function() {
            var btn = $(this);
            var id = $(this).data("id");
            var statusid = $(this).data("statusid");
            $.ajax({
                url: "/project/Get_Task_ByStatus/",
                data: {
                    id: id,
                    statusid: statusid
                },
                type: "GET",
            }).done(function(data) {
                if (data == null) {
                    alert("Failed");
                    return false;
                } else {
                    alert(data);
                    $(".taskstatus").removeClass("active");
                    btn.addClass("active");
                    $(".contentdiv").empty();
                    $(".contentdiv").append(data);
                    return false;
                }
            });
        });
    //****************************************************
    //*************** Project Files ******************
    //****************************************************
    $(document).on("click",
        ".addfiles",
        function() {
            $("#uploadfielmodel").modal("show");
        });
    $(document).on("click",
        "#btnuploadfile",
        function() {
            $("#notificationdiv").html(" ");
            var formData = new FormData();
            var imagefile = $("#projectfile")[0].files[0];
            if (!imagefile) {
                var data = { errortext: "Please select a file." };
                var source = $("#error-notification-template").html();
                var template = Handlebars.compile(source);
                var html = template(data);
                $("#notificationdiv").prepend(html);
                return false;
            }
            formData.append("imageFile", imagefile);
            var id = $(".modelprojectid").val();
            $.ajax({
                url: "/project/UploadFile/" + id,
                type: "POST",
                contentType: false, // Not to set any content header
                processData: false, // Not to process data
                data: formData,
            }).done(function(data) {
                if (data.error) {
                    var source = $("#error-notification-template").html();
                    var template = Handlebars.compile(source);
                    var html = template(data);
                    $("#notificationdiv").prepend(html);
                    return false;
                }
                var source = $("#success-notification-template").html();
                var template = Handlebars.compile(source);
                var html = template(data);
                $("#notificationdiv").prepend(html);
                $("#projectfile").val(null);
                return false;
            });
        });
    $(document).on("click",
        "#btnDelete",
        function() {
            var id = $(this).data("id");
            var tr = $(this).closest("tr");
            var trcount = $(this).closest("tbody").find("tr").length;
            swal({
                    title: "Delete File?",
                    text: "Are you sure, you want to delete this file ?",
                    type: "error",
                    showCancelButton: true,
                    closeOnConfirm: false,
                    confirmButtonColor: "#2196F3",
                    showLoaderOnConfirm: true
                },
                function() {
                    $.ajax({
                        url: "/project/DeleteFile/" + id,
                        type: "GET",
                    }).done(function(data) {
                        if (data.error) {
                            swal({
                                title: data.response,
                                confirmButtonColor: "#2196F3",
                                type: "error"
                            });
                            return false;
                        }
                        // remove selected row from the list.
                        $(tr).remove();
                        if (trcount == 1) {
                            var html =
                                '<tr><td colspan = "4" style = "text-align: center;" > Sorry, no files found.</td></tr>';
                            $(".table-body").prepend(html);
                        }
                        swal({
                            title: data.response,
                            confirmButtonColor: "#2196F3",
                            type: "success"
                        });
                    });
                });
            return false;
        });

    //****************************************************
    //*************** Project Credentials ************
    //****************************************************
    $(document).on("click",
        ".btnaddcredentials",
        function() {
            var id = $(this).data("id");
            $.ajax({
                url: "/credentials/create/",
                data: {
                },
                type: "GET",
            }).done(function(data) {
                if (data.error) {
                    //var source = $("#error-notification-template").html();
                    //var template = Handlebars.compile(source);
                    //var html = template(data);
                    //$("#modeladdtimebodydiv").prepend(html);
                    alert("Failed");
                    return false;
                }
                var title = "Add Credentials";
                $(".credentialmodeltitle").text(title);
                $("#credentialformmodeldiv").html(data);
                var proid = "<input type='hidden' value='" + id + "' id='projectid' name='projectid'>";
                $("#form-group-productid").html("");
                $("#form-group-productid").hide();
                $("#addcredentials").append(proid);
                $("select").select2();
                $("#Addcredentialmodel").modal("show");
                return false;
            });
        });
    $(document).on("submit",
        "#addcredentials",
        function(e) {
            e.preventDefault(); // prevent the form's normal submission

            var dataToPost = $(this).serialize();

            $.post("/credentials/create", dataToPost)
                .done(function(response, status, jqxhr) {
                    sweetAlert("", "The credentials has been added successfully!", "success");
                    $(".projectcreadentials").click();
                    $("#Addcredentialmodel").modal("hide");
                    return false;
                })
                .fail(function(jqxhr, status, error) {
                    // this is the ""error"" callback
                    //sweetAlert("", error.errortext, "error");
                });
        });
    $(document).on("click",
        ".editcredentialsbtn",
        function() {
            $(".credentialmodeltitle").text("");
            var btn = $(this);
            var id = $(this).data("id");
            $.ajax({
                url: "/credentials/edit/" + id,
                data: {
                },
                type: "GET",
            }).done(function(data) {
                if (data.error) {
                    //var source = $("#error-notification-template").html();
                    //var template = Handlebars.compile(source);
                    //var html = template(data);
                    //$("#modeladdtimebodydiv").prepend(html);
                    alert("Failed");
                    return false;
                }
                $("#credentialformmodeldiv").html(data);
                var projectid = $("#projectid").val();
                var proid = "<input type='hidden' value='" + projectid + "' id='projectid' name='projectid'>";
                $("#form-group-productid").html("");
                $("#form-group-productid").hide();
                $("#editcredentials").append(proid);
                $("select").select2();
                $("#Addcredentialmodel").modal("show");
                //$("#" + tcktitemid + ".notimeadded").remove();
                //var source = $("#Add-time-after-template").html();
                //var template = Handlebars.compile(source);
                //var html = template(data);
                //$("#" + tcktitemid + ".addtimeafter").append(html);

                return false;
            });
        });
    $(document).on("submit",
        "#editcredentials",
        function(e) {
            e.preventDefault(); // prevent the form's normal submission

            var dataToPost = $(this).serialize();

            $.post("/credentials/edit", dataToPost)
                .done(function(response, status, jqxhr) {
                    sweetAlert("", "The credentials has been updated successfully!", "success");
                    $(".projectcreadentials").click();
                    $("#Addcredentialmodel").modal("hide");
                    return false;
                })
                .fail(function(jqxhr, status, error) {
                    // this is the ""error"" callback
                });
        });

    $(function() {
        $(".contentdiv").on("click",
            ".usernameclick",
            function() {
                var $temp = $("<input>");
                $("body").append($temp);
                $temp.val($(this).closest("tr").find(".usernameval").text().trim()).select();
                document.execCommand("copy");
                $temp.remove();
            });
    });
    $(function() {
        $(".contentdiv").on("click",
            ".pwdclick",
            function() {
                var credentialid = $(this).data("id");
                $.ajax({
                    url: "/credentials/getpassword",
                    data: {
                        id: credentialid
                    },
                    type: "GET",
                }).done(function(data) {
                    var $temp = $("<input>");
                    $("body").append($temp);
                    $temp.val(data.password).select();
                    document.execCommand("copy");
                    $temp.remove();
                    return false;
                });
            });
    });

    //****************************************************
    //*************** Get Task List **********************
    //****************************************************
    $(document).on("click",
        ".tasklist",
        function() {
            $(".Addcredentialsli").hide();
            var id = $(this).data("id");
            $.ajax({
                url: "/projectdashboard/get_tasklist/" + id,
                data: {
                },
                type: "GET",
            }).done(function(data) {
                if (data.error) {
                    //var source = $("#error-notification-template").html();
                    //var template = Handlebars.compile(source);
                    //var html = template(data);
                    //$("#modeladdtimebodydiv").prepend(html);
                    alert("Failed");
                    return false;
                }
                $(".contentdiv").html(data);
                $(".projectfilesli").removeClass("active");
                $(".credentialsli").removeClass("active");
                $(".notesli").removeClass("active");
                $(".taskli").addClass("active");
                $(".taskstatus").removeClass("active");
                $(".filterbystatus  .taskstatus").each(function(e) {
                    if ($(this).data("statusid") == 2) {
                        $(this).addClass("active");
                    }
                });
                //$("#" + tcktitemid + ".notimeadded").remove();
                //var source = $("#Add-time-after-template").html();
                //var template = Handlebars.compile(source);
                //var html = template(data);
                //$("#" + tcktitemid + ".addtimeafter").append(html);
                return false;
            });
        });

    //****************************************************
    //*************** Project Notes ************
    //****************************************************
    function getnotes(id) {
        $.ajax({
            url: "/project/GetNotes/",
            data: {
                id: id
            },
            type: "GET",
        }).done(function(data) {
            if (data.error) {
                return false;
            }
            var html =
                '<li class="media"><div class="media-left"><a href="#"><img src="/Content/images/placeholder.jpg" data-id="' +
                    data.id +
                    '" class="img-circle notesimg"title="' +
                    data.FullName +
                    '"></a></div><div class="media-body"><div class="media-content">' +
                    data.comments +
                    '</div><span class="media-annotation display-block mt-10"><b>' +
                    data.FullName +
                    "&nbsp;</b>" +
                    data.createdonutc +
                    '&nbsp;<span data-id="' +
                    data.id +
                    '" class="editnotemsg"><b>Edit</b></span> &nbsp;<span data-id="' +
                    data.id +
                    '" class="deletenotemsg"><b>Delete</b></span></span></div></li>';
            $(".ulmsg").append(html);
            $("#msg").val("");
            return false;
        });
    }

    $(document).on("click",
        ".btnsendmsg",
        function() {
            var text = $("#msg").val();
            var id = $(this).data("id");
            $.ajax({
                url: "/project/addprojectnotes/",
                data: {
                    id: id,
                    text: text
                },
                type: "GET",
            }).done(function(data) {
                if (data.error) {
                    alert("Failed");
                    return false;
                } else {
                    getnotes(id);
                }
                return false;
            });
        });

    $(document).on("click",
        ".editnotemsg",
        function() {
            var id = $(this).data("id");
            var text = $(this).closest("div").children(".media-content").text();
            var element = $(this).closest("div").children(".media-content");
            swal({
                    title: "Edit Note!",
                    type: "input",
                    inputValue: text,
                    showCancelButton: true,
                    closeOnConfirm: false,
                    confirmButtonColor: "#2196F3",
                    showLoaderOnConfirm: true
                },
                function(inputValue) {
                    if (inputValue === false) return false;
                    if (inputValue === "") {
                        swal.showInputError("You need to write something!");
                        return false;
                    }
                    $.ajax({
                        url: "/project/EditProjectNotes/",
                        data: {
                            id: id,
                            comment: inputValue
                        },
                        type: "GET",
                    }).done(function(data) {
                        if (data.error) {
                            swal({
                                title: data.response,
                                confirmButtonColor: "#2196F3",
                                type: "error"
                            });
                            return false;
                        }
                        $(element).html(inputValue);
                        swal({
                            title: data.response,
                            confirmButtonColor: "#2196F3",
                            type: "success"
                        });
                    });
                });
            return false;
        });
    $(document).on("click",
        ".deletenotemsg",
        function() {
            var id = $(this).data("id");
            var li = $(this).closest("li");
            var licount = $(this).closest("ul").find("li").length;
            swal({
                    title: "Delete Note?",
                    text: "Are you sure, you want to delete this note ?",
                    type: "error",
                    showCancelButton: true,
                    closeOnConfirm: false,
                    confirmButtonColor: "#2196F3",
                    showLoaderOnConfirm: true
                },
                function() {
                    $.ajax({
                        url: "/project/DeleteProjectNotes/" + id,
                        data: "",
                        type: "GET",
                    }).done(function(data) {
                        if (data.error) {
                            swal({
                                title: data.response,
                                confirmButtonColor: "#2196F3",
                                type: "error"
                            });
                            return false;
                        }
                        // remove selected row from the list.
                        $(li).remove();
                        if (licount == 1) {
                            var html =
                                '<li class="media"><span class="media-annotation display-block mt-10"><b>Sorry, no note found!</b></span></li>';
                            $(".ulmsg").prepend(html);
                        }
                        swal({
                            title: data.response,
                            confirmButtonColor: "#2196F3",
                            type: "success"
                        });
                    });
                });
            return false;
        });
    $("#uploadfielmodel").on("hidden.bs.modal",
        function() {
            location.reload();
        });
});