var user = true;
var tempz;
var isreload = false;
var counter = 0;
jQuery(function () {
    //var link = $('.emailcontainer').find('a');
    //    link.attr('href', 'https://www.google.com/url?q='+link.attr('href'));
    /*****************************************************
     ** Associate controls.
     *****************************************************/
    // Start Working Modal.
    jQuery("#projectid").select2();
    jQuery("#skillid").select2();
    jQuery("#Assignmenttaskproject").select2();
    jQuery("#Assignmenttaskskill").select2();
    jQuery("#StartDate").datepicker();
    jQuery("#EndDate").datepicker();
    // Task Detail section
    jQuery("#taskprojectid").select2();
    jQuery("#taskskillid").select2();
    // Add Time Section
    jQuery('#workdate').datepicker({
        maxDate: 0
    });
    jQuery("#timespentinminutes").numeric({
        allowMinus: false,
        allowThouSep: false
    });
    jQuery("#billabletimeinminutes").numeric({
        allowMinus: false,
        allowThouSep: false
    });
    jQuery("#estimateTimeInput").numeric({
        allowMinus: false,
        allowThouSep: false
    });
    jQuery(document).on('change', "#tlcheckbox", function () {
        if (this.checked) {
            var projectid = jQuery("#taskprojectid").val();
            var skillid = jQuery("#taskskillid").val();
            var timespent = jQuery("#timespentinminutes").val();
            var logdescription = jQuery("#description").val();
            if (projectid <= 0) {
                new PNotify({
                    title: 'Error',
                    text: "Please select the project",
                    type: 'error',
                    addclass: "stack-bottom-right",
                    hide: true
                });
            }
            if (skillid <= 0) {
                new PNotify({
                    title: 'Error',
                    text: 'Please select the skill',
                    type: 'error',
                    addclass: "stack-bottom-right",
                    hide: true
                });
            }
            if (timespent == 0) {
                new PNotify({
                    title: 'Error',
                    text: 'Please enter the time spent',
                    type: 'error',
                    addclass: "stack-bottom-right",
                    hide: true
                });
            }
            if (logdescription == '') {
                new PNotify({
                    title: 'Error',
                    text: 'Please enter the task description',
                    type: 'error',
                    addclass: "stack-bottom-right",
                    hide: true
                });
            }
        }
    });
    if (jQuery("#commentPermalink").val() != null) {
        var linumber = jQuery("#commentPermalink").val();
        var focusli = jQuery('.commentbody').find('#' + linumber);//+linumber);
        $('html, body').animate({
            scrollTop: $(focusli).offset().top
        }, 'slow');
    }

    /*Muhmmad Nasir*/
    jQuery(".ticketMeta").click(function () {
        //alert("ticket Meta");
        var tid = jQuery(this).data("ticketidmeta");
        $.ajax({
            url: '/tickets/LoadTicketMeta?TicketId=' + tid,
            type: 'GET',
        }).done(function (data) {
            if (data.error) {
                jQuery('#ticketMetaInformation').html('');
                jQuery('#ticketMetaInformation').append(JSON.stringify(data.errortext));
                return false;
            }
            else {
                jQuery('#ticketMetaInformation').html('');
                var tablecontent = "";
                tablecontent += "<table class='table table-bordered table-xxs metadatatable'>";
                tablecontent += "<thead>";
                tablecontent += "<tr class='bg-slate-600'>";
                tablecontent += "<th><b>Ticket # " + tid + " META information</b></th></tr></thead>";
                tablecontent += "<tbody><tr class='active'><td>Last Updated On:<b>" + data.ticketdate + "</b></td></tr>";

                //jQuery('#ticketMetaInformation').append("<b>Ticket # "+ tid +" META information</b><br>");
                //jQuery('#ticketMetaInformation').append("Created On:<b> " + data.ticketdate + "</b><br>");
                var count = 0;
                $.each(data.TicketMeta, function (index, element) {
                    count = count + 1;
                    if (count % 2 == 0) {
                        tablecontent += "<tr class='active'><td>" + element.ActionDescription + "</td></tr>";
                    }
                    else {
                        tablecontent += "<tr class=''><td>" + element.ActionDescription + "</td></tr>";
                    }

                    //jQuery('#ticketMetaInformation').append(element.ActionDescription + "<br>");
                });
                tablecontent += "</tbody></table>";
                jQuery('#ticketMetaInformation').append(tablecontent);

                //$('.metadatatable').find('tr:even').css({'background-color': 'red'}).end().find('tr:odd').css({'background-color': 'blue'});
            }
        });
        $("#ticketMeta").modal("show");
    });

    // $(".CC").hide();
    $(".BCC").hide();

    // Full featured editor
    var editor = CKEDITOR.replace('txtComments', {
        extraPlugins: 'autolink,autogrow,base64image',
        autoGrow_minHeight: 75,
        autoGrow_bottomSpace: 5,
        removePlugins: 'resize,easyimage, cloudservices',
        height: '75px',
        toolbar: [
            ['Font', 'FontSize', 'Bold', 'Italic', 'Underline', 'Strike', '-', 'Blockquote', '-', 'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock'],
            ['BidiLtr', 'BidiRtl'],
            ['TextColor', 'BGColor'],
            ['customimage', 'customsmiley', 'Link', 'Image'],
            ['Flash', 'customfiles', 'Table', '-', 'Outdent', 'Indent'],
            ['NumberedList', 'BulletedList', 'HorizontalRule'],
            ['Styles', 'Format'], ['-'], ['Paste', 'PasteText', 'PasteFromWord'],
            ['-', 'Source'],
            ['Maximize']
        ]
    });

    var editor = CKEDITOR.replace('Emailbody', {
        height: '500px',
        extraPlugins: 'forms,base64image',
        removePlugins: 'wsc, easyimage, cloudservices',
        scayt_autoStartup: true
    });

    /*****************************************************
     ** Gloabl actions performed on every load.
     *****************************************************/
    initDefaults();

    /*****************************************************
     ** make all the hyperlinks in emil-body open in new tab automatically.
     *****************************************************/
    jQuery('#email-body').find('a[href^="http://"], a[href^="https://"]').attr('target', '_blank');
    //jQuery(document).on('change', "#taskprojectid", function () {
    //    alert("hited");
    //}

    jQuery(document).on('change', "#taskprojectid", function () {
        var ticketid = jQuery(this).closest(".sidebar-secondary").data("ticketid");
        var ticketitemid = jQuery(this).closest(".sidebar-secondary").data("ticketitemid");
        var projectid = $(this).val();
        var projectname = $(this).find("option:selected").text();
        var text = '<a class="my-link" href="/project/index/' + projectid + '" target="_blank"> ' + projectname + ' </a><i class="fa fa-edit changer"></i>';
        jQuery.post("/tickets/updateticketproject", { id: ticketid, ticketitemid: ticketitemid, projectid: projectid }).done(function (data) {
            if (data.success) {
                $('.task-project-link').html(text);
                $('.project-list').removeClass('col-md-12').addClass('col-md-11');
                $('.cross').removeClass('hidden');
                $('#cprojectid').val(projectid);
                new PNotify({
                    title: 'Success',
                    text: data.messagetext,
                    type: 'success',
                    addclass: "stack-bottom-right",
                    hide: true
                });
            } else {
                new PNotify({
                    title: 'Error',
                    text: data.messagetext,
                    type: 'error',
                    addclass: "stack-bottom-right",
                    hide: true
                });
            }
        });
        return false;
    });

    jQuery(document).on('change', "#taskskillid", function () {
        var ticketid = jQuery(this).closest(".sidebar-secondary").data("ticketid");
        var ticketitemid = jQuery(this).closest(".sidebar-secondary").data("ticketitemid");
        var skillid = $(this).val();

        jQuery.post("/tickets/updateticketskill", { id: ticketid, ticketitemid: ticketitemid, skillid: skillid }).done(function (data) {
            if (data.success) {
                //new PNotify({
                //    title: 'Success',
                //    text: data.messagetext,
                //    type: 'success',
                //    hide: false
                //});
            } else {
                new PNotify({
                    title: 'Error',
                    text: data.messagetext,
                    type: 'error',
                    addclass: "stack-bottom-right",
                    hide: false
                });
            }
        });
        return false;
    });

    /*****************************************************
     ** Add Time Tab
     *****************************************************/
    jQuery('#timespentinminutes').on('input', function (e) {
        jQuery("#billabletimeinminutes").val(jQuery(this).val());
    });

    jQuery('#description').on('focus', function (e) {
        jQuery(this).select();
    });

    jQuery(document).on('click', '.savetime', function () {
        var ticketitemid = jQuery(this).closest(".sidebar-secondary").data("ticketitemid");
        var projectid = jQuery("#taskprojectid").val();
        var skillid = jQuery("#taskskillid").val();
        var workdate = jQuery("#workdate").val();
        var timespent = jQuery("#timespentinminutes").val();
        var billable = jQuery("#billabletimeinminutes").val();
        var title = encodeURI(jQuery(".ticket-title").text().trim());
        var description = jQuery("#description").val();
        var IsWarning = jQuery("#WarningStatus").val();
        var WarningText = jQuery("#WarningText").val();
        var comments = "";

        if (ticketitemid == "" || ticketitemid == 0) {
            new PNotify({
                title: 'Project/Skill Missing',
                text: "Open <b>Task Details</b> section, and select a valid project and skill.",
                type: 'error',
                addclass: "stack-bottom-right",
                hide: false
            });
            return false;
        }

        if (projectid == "") {
            new PNotify({
                title: 'Project Missing',
                text: "Open <b>Task Details</b> section, and select a valid project.",
                type: 'error',
                addclass: "stack-bottom-right",
                hide: false
            });
            return false;
        }

        if (skillid == "") {
            new PNotify({
                title: 'Skill Missing',
                text: "Open <b>Task Details</b> section, and select a valid skill.",
                type: 'error',
                addclass: "stack-bottom-right",
                hide: false
            });
            return false;
        }

        if (workdate == "") {
            new PNotify({
                title: 'Work Date Required',
                text: "Please select a work date.",
                type: 'error',
                addclass: "stack-bottom-right",
                hide: false
            });
            return false;
        }

        if (timespent == "" || timespent == "0") {
            new PNotify({
                title: 'Time Spent',
                text: "Please enter a valid time spent.",
                type: 'error',
                addclass: "stack-bottom-right",
                hide: false
            });
            return false;
        }

        if (description == "") {
            new PNotify({
                title: 'Description Required',
                text: "Please enter description.",
                type: 'error',
                addclass: "stack-bottom-right",
                hide: false
            });
            return false;
        }
        $.ajax({
            url: '/tickets/addtime/',
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
            type: 'POST',
        }).done(function (data) {
            if (data.error) {
                new PNotify({
                    title: 'Error',
                    text: data.errortext,
                    type: 'error',
                    addclass: "stack-bottom-right",
                    hide: true
                });
                //var source = $("#error-notification-template").html();
                //var template = Handlebars.compile(source);
                //var html = template(data);
                //$("#modeladdtimebodydiv").prepend(html);
                return false;
            }
            swal({
                title: "Success",
                text: "Time has been added successfully.",
                type: "success",
                confirmButtonColor: "#2196F3"
            });
            //new PNotify({
            //    title: 'Success',
            //    text: 'Time has been added successfully.',
            //    type: 'success',
            //    addclass: "stack-bottom-right",
            //    hide: true
            //});

            clearAddTime();

            return false;
        });

        // Display warning, if enabled.
        if (IsWarning) {
            new PNotify({
                title: '<h1>Warning!</h1>',
                text: '<h3>' + WarningText + '</h3>',
                type: 'error',
                addclass: "stack-bottom-right",
                hide: true
            });
        }
        return false;
    });

    /*****************************************************
     ** Conversation Emails Tab
     *****************************************************/
    jQuery('.filteremails').livefilter({ selector: '.emailcollection li' });

    jQuery(document).on('click', '.emailitem', function () {
        $(".emailcontainer").show();
        $("#replayPanel").hide();
        var ticketitemid = jQuery(this).data("emailid");
        var graphEmailId = jQuery(this).data("graphemailid");
        jQuery("#conversation").val(graphEmailId);
        jQuery("#ticketdetail").data('ticketitemid', ticketitemid);
        loadEmail(ticketitemid);
        jQuery(this).addClass("active");

        return false;
    });

    /*****************************************************
  ** Display Comments of ticket.
  *****************************************************/

    jQuery(document).on('click', '.displaycomment', function () {
        var url = "/Tickets/loadCommentByTicketId/";
        var tid = jQuery(this).data("ticketid");
        $.ajax({
            url: '/Tickets/loadCommentByTicketId/',
            data: { id: tid },
            type: 'POST',
        }).done(function (data) {
            $(".commentbody").html('');
            if (data.commentlist == "") {
                $(".commentbody").append($.parseHTML("<strong class='Norecordfound'>No record found</strong>"));
            }
            else {
                $.each(data.commentlist, function (index, element) {
                    var commentdate = $.datepicker.formatDate('mm/dd/yy', new Date(Date(element.commenton)));//new Date(parseInt(element.commenton.substr(6))).format("dd/mm/yyyy");
                    var commentbody = "<li class='media' id=" + element.id + "><div class='media-left'><a href='#'>";
                    if (element.ProfileImage != undefined) {
                        commentbody += "<img id=" + element.id + "' src='/Content/images/" + element.ProfileImage + "' class='img-circle img-sm' alt='Not Loaded'>";
                    }
                    else {
                        commentbody += "<img id=" + element.id + "' src='/Content/images/placeholder.jpg' class='img-circle img-sm' alt='Not Loaded'>";
                    }
                    commentbody += "</a></div><div class='media-body'><div class='media-heading'><a href='#' class='text-semibold'> " + element.commentbyusername + "</a><span class='media-annotation dotted'>" + commentdate + "</span></div>";
                    commentbody += "<div id='CommentBody'>" + element.commentbody + "</div><br/>";
                    commentbody += "<ul class='list-inline list-inline-separate text-size-small'>";
                    commentbody += "<li class='EditComments' data-id='" + element.id + "'><a>Edit</a></li><li class='DeleteComments' data-id='" + element.id + "'><a >Delete</a></li></ul></div></li>";
                    $(".commentbody").append($.parseHTML(commentbody));
                });
                responsiveImg();
            }
        });
    });

    /*****************************************************
     ** Start working on the ticket.
     *****************************************************/
    jQuery(document).on('click', '.startworking', function () {
        var projectid = jQuery(this).data("projectid");
        var skillid = jQuery(this).data("skillid");

        $("#projectid").val(projectid).trigger("change");
        $("#skillid").val(skillid).trigger("change");

        jQuery('#startWorkingModal').modal('toggle');
        return false;
    });

    jQuery(document).on('click', '.startworkingaction', function () {
        var ticketid = jQuery(".startworking").data("ticketid");
        var statusid = 2; // in-progress
        var projectid = $("#projectid option:selected").val();
        var skillid = $("#skillid option:selected").val();
        var quotedtime = $("#quotabletime").val();
        if (quotedtime == "") {
            quotedtime = 0;
        }

        // make sure project and skill has been selected.
        if (projectid == "" || skillid == "") {
            sweetAlert("Sorry", "Please select project and skill.", "error")
            return false;
        }

        $.ajax({
            url: '/tickets/startworking/',
            data:
            {
                id: ticketid,
                status: statusid,
                project: projectid,
                quotedtime: quotedtime,
                skill: skillid
            },
            type: 'POST'
        }).done(function (data) {
            if (data.error) {
                new PNotify({
                    title: 'Error',
                    text: data.errortext,
                    addclass: "stack-bottom-right",
                    type: 'error'
                });
                return false;
            }

            if (data.success) {
                new PNotify({
                    title: 'Success',
                    text: data.successtext,
                    addclass: "stack-bottom-right",
                    type: 'success'
                });

                $('.assignedusersinput').tagsinput('add', { id: data.userid, text: data.username });
                $('.newassignedusersinput').tagsinput('add', { id: data.userid, text: data.username });
            } else {
                new PNotify({
                    title: 'Info',
                    text: data.successtext,
                    addclass: "stack-bottom-right",
                    type: 'info'
                });
            }

            jQuery('#startWorkingModal').modal('toggle');
        });
    });

    /*****************************************************
    ** Email Assignments
    *****************************************************/
    //jQuery('#EmailSendTo').on('beforeItemAdd', function (event) {
    //});
    //jQuery('#EmailSendCC').on('beforeItemAdd', function (event) {
    //});
    //jQuery('#EmailSendBCC').on('beforeItemAdd', function (event) {
    //});

    /*****************************************************
     ** User Assignments
     *****************************************************/
    jQuery('.newassignedusersinput').on('beforeItemAdd', function (event) {
        var tag = event.item;
        $.ajax({
            url: '/tickets/PrefetchSingleTeams?UserID=' + tag.value,
            type: 'GET',
        }).done(function (data) {
            if (data.error) {
                alert(JSON.stringify(data.errortext));
                return false;
            }
            else {
                jQuery('.assignedteaminput').tagsinput('add', { value: data.teamid, text: data.teamName });
                return false;
            }
        });
    });
    jQuery("#basic-addon2").click(function () {
        loadModalData();
    });
    jQuery('.assignedusersinput').on('beforeItemAdd', function (event) {
        if (user) {
            tempz = event.item;
            loadModalData();
            $(".newassignedusersinput").tagsinput('add', tempz, { preventPost: true });
            $("#ticketAssignModal").modal("show");

            var tag = event.item;
            $.ajax({
                url: '/tickets/PrefetchSingleTeams?UserID=' + tag.value,
                type: 'GET',
            }).done(function (data) {
                if (data.error) {
                    alert(JSON.stringify(data.errortext));
                    return false;
                }
                else {
                    jQuery('.assignedteaminput').tagsinput('add', { value: data.teamid, text: data.teamName });
                    return false;
                }
            });
        }
    });

    jQuery('.newassignedusersinput').on('beforeItemRemove', function (event) {
        if (user) {
            user = false;
            var tag = event.item;
            var ticketid = jQuery("#ticketid").val();
            if (!event.options || !event.options.preventPost) {
                jQuery.post('/tickets/removeticketuser', { id: ticketid, userid: tag.value }, function (response) {
                    if (!response.success) {
                        // Re-add the tag since there was a failure
                        // "preventPost" here will stop this ajax call from running when the tag is added
                        jQuery('.newassignedusersinput').tagsinput('add', tag, { preventPost: true });
                    }
                    if (response.success) {
                        new PNotify({
                            title: 'Success!',
                            text: response.messagetext,
                            addclass: "stack-bottom-right",
                            type: 'success'
                        });
                    }
                    reloadAssignment();
                });
            }

            setTimeout(function () { user = true; }, 100);
        }
        // event.item: contains the item
        // event.cancel: set to true to prevent the item getting removed
    });

    jQuery('.assignedusersinput').on('beforeItemRemove', function (event) {
        if (user) {
            user = false;
            var tag = event.item;
            var ticketid = jQuery("#ticketid").val();
            if (!event.options || !event.options.preventPost) {
                jQuery.post('/tickets/removeticketuser', { id: ticketid, userid: tag.value }, function (response) {
                    if (!response.success) {
                        // Re-add the tag since there was a failure
                        // "preventPost" here will stop this ajax call from running when the tag is added
                        jQuery('.assignedusersinput').tagsinput('add', tag, { preventPost: true });
                    }
                    if (response.success) {
                        new PNotify({
                            title: 'Success!',
                            text: response.messagetext,
                            addclass: "stack-bottom-right",
                            type: 'success'
                        });
                    }
                    reloadAssignment();
                });
            }

            setTimeout(function () { user = true; }, 100);
        }
        // event.item: contains the item
        // event.cancel: set to true to prevent the item getting removed
    });
    jQuery('.assignedteaminput').on('beforeItemRemove', function (event) {
        var tag = event.item;
        var ticketid = jQuery("#ticketid").val();
        if (!event.options || !event.options.preventPost) {
            jQuery.post('/tickets/RemoveTicketteam', {
                id: ticketid, teamid: tag.value
            }, function (response) {
                if (!response.success) {
                    // Re-add the tag since there was a failure
                    // "preventPost" here will stop this ajax call from running when the tag is added
                    //jQuery('.assignedteaminput').tagsinput('add', tag, {
                    //    preventPost: true
                    //});
                }
                if (response.success) {
                    new PNotify({
                        title: 'Success!',
                        text: response.messagetext,
                        addclass: "stack-bottom-right",
                        type: 'success'
                    });
                }
                reloadAssignment();
            });
        }

        // event.item: contains the item
        // event.cancel: set to true to prevent the item getting removed
    });

    jQuery('#ticketAssignModal').on('hidden.bs.modal', function () {
        counter = 1;
    })
    jQuery(document).on("click", ".assign-single-ticket", function () {
        // Fetch user input data.
        //var temp = jQuery(this).parent().parent().parent().parent().parent().parent().parent().data("ticketid");
        var ticketid = jQuery("#ticketid").val();
        var projectid = jQuery("#Assignmenttaskproject").val();
        var skillid = jQuery("#Assignmenttaskskill").val();
        var userscsv = jQuery(".newassignedusersinput").val();
        var teamscsv = jQuery(".assignedteaminput").val();
        var comment = jQuery("#ticketcomments").val();
        var StartDate = jQuery("#StartDate").val();
        var EndDate = jQuery("#EndDate").val();
        var ticketscsv = ticketid;
        var EstimatedTime = jQuery("#estimateTimeInput").val();
        var EstimateTimeHidden = jQuery("#EstimateTimeHidden").val();
        // Validate required information.
        //if (EstimatedTime == "" || EstimatedTime == 0) { sweetAlert("Sorry! Assignment Missing", "Please add ticket estimate time.", "error"); return false; }
        if (userscsv == "" || userscsv == null || teamscsv == "" || teamscsv == null) { sweetAlert("Sorry", "Please select both users and teams from the list.", "error"); return false; }
        if (projectid == "" || projectid == null) { sweetAlert("Sorry", "Please select a project from the list.", "error"); return false; }
        if (skillid == "" || skillid == null) { sweetAlert("Sorry", "Please select a skill from the list.", "error"); return false; }
        if (userscsv == "" && teamscsv == "") { sweetAlert("Sorry! Assignment Missing", "Please assign ticket to at least one user/team.", "error"); return false; }
        if (EstimatedTime === EstimateTimeHidden) { EstimatedTime = null; }
        $.ajax({
            url: '/tickets/assigntickets/',
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
            type: 'POST',
        }).done(function (data) {
            if (data.error) {
                var source = $("#error-notification-template").html();
                var template = Handlebars.compile(source);
                var html = template(data);
                jQuery("#ticketAssignModal").find(".panel-body").prepend(html);
                return false;
            }
            reloadAssignment();
            $('#taskprojectid').val(projectid);
            $('#taskprojectid').select2().trigger('change');
            $('#taskskillid').val(skillid);
            $('#taskskillid').select2().trigger('change');
            $('#cskillid').val(skillid);
            $('#cprojectid').val(projectid);
            // display success message.
            new PNotify({
                title: 'Success',
                text: 'The task has been assigned to selected users and teams.',
                type: 'success',
                addclass: "stack-bottom-right",
                hide: true
            });
            // hide the dialog box.
            jQuery('#ticketAssignModal').modal('hide');
            return false;
        });
    });

    /*****************************************************
     ** Remove Attachment.
     *****************************************************/
    var emailFormData = new FormData();
    var count = 1;
    jQuery(document).on('click', '.remove-attachment', function () {
        $(this).closest('.emailattachment').remove();
        var id = $(this).parent().parent().parent().attr("id")
        emailFormData.delete(id)
    });

    /*****************************************************
     ** Reply Feature.
     *****************************************************/
    jQuery(document).on('click', '#btnReply', function () {
        //1. Change View to send reply.
        jQuery(".view-email-actions").hide();
        jQuery(".send-email-actions").show();
        $("#emailType").val("Reply");
        var ticketitemid = jQuery("#leftNevMenu").find(".active").data("emailid");
        jQuery("#ticketdetail").data('ticketitemid', ticketitemid);
        var emailTo = jQuery("#emailcontentwrapper").find("#email-" + ticketitemid).find(".from").html().trim();
        //var CheckToEmailAddressFlag = false;
        //var emailToArray = jQuery("#emailcontentwrapper").find("#email-" + ticketitemid).find(".to-addresses").html().trim().split(';');
        //$.each(emailToArray, function (index) {
        //    var emailtoarray1 = emailToArray[index].split(',');

        //    $.each(emailtoarray1, function (value) {
        //        if (emailTo == emailtoarray1[value])
        //        {
        //            CheckToEmailAddressFlag = true;
        //        }
        //        if (emailtoarray1[value] != '')
        //        {
        //            jQuery('#EmailSendTo').tagsinput('add', { value: emailtoarray1[value], text: emailtoarray1[value] });
        //        }
        //        alert(emailtoarray1[value]);
        //    });

        //    if (CheckToEmailAddressFlag== false)
        //    {
        //        jQuery('#EmailSendTo').tagsinput('add', { value: emailTo, text: emailTo });
        //    }
        //    // $.each(emailToArray[index].split(','), function (value) {
        //    //jQuery('#EmailSendTo').tagsinput('add', { value: emailToArray[index], text: emailToArray[index] });
        //    //  });

        //    // jQuery("#EmailSendTo").val(jQuery("#emailcontentwrapper").find("#email-" + ticketitemid).find(".to-addresses").html().trim());
        //});
        jQuery('#EmailSendTo').tagsinput('add', { value: emailTo, text: emailTo });
        //var EmailSendCCArray = jQuery("#emailcontentwrapper").find("#email-" + ticketitemid).find(".to-addresses").html().trim().split(';');
        //$.each(EmailSendCCArray,function(index){
        //    jQuery("#EmailSendCC").tagsinput('add', { value: EmailSendCCArray[index], text: EmailSendCCArray[index] });
        //});
        jQuery("#EmailSendCC").tagsinput('removeAll');
        //jQuery("#EmailSendCC").val(jQuery("#emailcontentwrapper").find("#email-" + ticketitemid).find(".cc-addresses").html().trim());
        //jQuery("#EmailSendBCC").val(jQuery("#emailcontentwrapper").find("#email-" + ticketitemid).find(".bcc-addresses").html().trim());
        var title = $(".ticket-title").find("span:first").html().trim();
        if (title.substring(0, 3) == 'RE:' || title.substring(0, 3) == 'Re:' || title.substring(0, 3) == 'Fw:' || title.substring(0, 3) == 'FW:') {
            title = title.substring(3);
        }
        var emailsendsubject = jQuery("#EmailSendSubject").val("RE:" + title.trim());
        if (emailsendsubject.val().toLowerCase().indexOf("[ticket#") == -1) {
            var tid = jQuery(".ticketidonspan").attr("value");
            var element = jQuery("#EmailSendSubject").val(emailsendsubject.val());// + " [Ticket#" + tid + "]");
        }
        var replayemailsignature = jQuery('#UserEmailSignature').val();
        var html = $("#ticketdetail").find(".emailcontainer").find("#email-body").html().trim();
        CKEDITOR.instances["Emailbody"].setData("<br/><br/>" + replayemailsignature + "<hr><br/>" + html);
        $(".emailcontainer").hide();
        $("#replayPanel").show();
        // $(".CC").hide();
        $(".BCC").hide();
    });

    /*****************************************************
     ** Reply All Feature.
     *****************************************************/
    jQuery(document).on('click', '#btnReplyAll', function () {
        //1. Change View to send reply all.
        jQuery(".view-email-actions").hide();
        jQuery(".send-email-actions").show();
        $("#emailType").val("ReplyAll");

        var ticketitemid = jQuery("#leftNevMenu").find(".active").data("emailid");
        //jQuery("#EmailSendTo").val(jQuery("#emailcontentwrapper").find("#email-" + ticketitemid).find(".from").html().trim());

        // jQuery("#EmailSendTo").tagsinput('add', { value: jQuery("#emailcontentwrapper").find("#email-" + ticketitemid).find(".from").html().trim(), text: jQuery("#emailcontentwrapper").find("#email-" + ticketitemid).find(".from").html().trim() });
        var emailTo = jQuery("#emailcontentwrapper").find("#email-" + ticketitemid).find(".from").html().trim();
        // jQuery("#emailcontentwrapper").find("#email-" + ticketitemid).find(".to-addresses").html().trim().replace(/\,/g, ';');
        var emailToArray = jQuery("#emailcontentwrapper").find("#email-" + ticketitemid).find(".to-addresses").html().trim().split(';');
        var CheckToEmailAddressFlag = false;
        $.each(emailToArray, function (index) {
            var emailtoarray1 = emailToArray[index].split(',');
            $.each(emailtoarray1, function (value) {
                if (emailTo == emailtoarray1[value]) {
                    CheckToEmailAddressFlag = true;
                }
                if (emailtoarray1[value] != '') {
                    jQuery('#EmailSendTo').tagsinput('add', { value: emailtoarray1[value], text: emailtoarray1[value] });
                }
            });
            // $.each(emailToArray[index].split(','), function (value) {
            //jQuery('#EmailSendTo').tagsinput('add', { value: emailToArray[index], text: emailToArray[index] });
            //  });

            // jQuery("#EmailSendTo").val(jQuery("#emailcontentwrapper").find("#email-" + ticketitemid).find(".to-addresses").html().trim());
        });
        if (CheckToEmailAddressFlag == false) {
            jQuery('#EmailSendTo').tagsinput('add', { value: emailTo, text: emailTo });
        }
        if (jQuery("#emailcontentwrapper").find("#email-" + ticketitemid).find(".cc-addresses").hasClass("has-data")) {
            var emailCCArray = jQuery("#emailcontentwrapper").find("#email-" + ticketitemid).find(".cc-addresses").html().trim().split(';');
            // var emailCCArray = jQuery("#emailcontentwrapper").find("#email-" + ticketitemid).find(".cc-addresses").html().trim().split(';');
            $.each(emailCCArray, function (index) {
                jQuery('#EmailSendCC').tagsinput('add', { value: emailCCArray[index], text: emailCCArray[index] });
            });
            //jQuery("#EmailSendCC").val(jQuery("#emailcontentwrapper").find("#email-" + ticketitemid).find(".to-addresses").html().trim());
            jQuery("#EmailSendCC").val(jQuery("#emailcontentwrapper").find("#email-" + ticketitemid).find(".cc-addresses").html().trim());
        }
        jQuery("#EmailSendBCC").val(jQuery("#emailcontentwrapper").find("#email-" + ticketitemid).find(".bcc-addresses").html().trim());
        var title = $(".ticket-title").find("span:first").html().trim();
        if (title.substr(0, 3) == 'RE:' || title.substr(0, 3) == 'Re:' || title.substr(0, 3) == 'FW:' || title.substr(0, 3) == 'Fw:') {
            title = title.substring(3);
        }
        var emailsendsubject = jQuery("#EmailSendSubject").val("RE:" + title.trim());

        if (emailsendsubject.val().toLowerCase().indexOf("[ticket#") == -1) {
            var tid = jQuery(".ticketidonspan").attr("value");
            var element = jQuery("#EmailSendSubject").val(emailsendsubject.val());// + " [Ticket#" + tid + "]");
        }
        //  jQuery("#EmailSendTo").val(jQuery("#EmailSendTo").val().trim() + "," + $(".mail-to-addresses").html().substring(3).trim());
        var replayemailsignature = jQuery('#UserEmailSignature').val();
        var html = $("#ticketdetail").find(".emailcontainer").find("#email-body").html().trim();
        CKEDITOR.instances["Emailbody"].setData("<br/>" + replayemailsignature + "<hr>" +
            "<br/>" + html);
        $(".emailcontainer").hide();
        $("#replayPanel").show();
        //  $(".CC").hide();
        $(".BCC").hide();
    });

    /*****************************************************
     ** Forward Feature.
     *****************************************************/
    jQuery(document).on('click', '#btnForword', function () {
        //1. Change View to send reply all.
        jQuery(".view-email-actions").hide();
        jQuery(".send-email-actions").show();
        $("#emailType").val("Forward");
        var ticketitemid = jQuery("#leftNevMenu").find(".active").data("emailid");
        jQuery("#EmailSendTo").val("");
        jQuery("#EmailSendCC").val("");
        jQuery("#EmailSendBCC").val("");
        var subject = $(".ticket-title").find("span:first").html().trim();
        var title = $(".ticket-title").find("span:first").html().trim();
        if (title.substr(0, 3) == 'RE:' || title.substr(0, 3) == 'Re:' || title.substr(0, 3) == 'FW:' || title.substr(0, 3) == 'Fw:') {
            title = title.substring(3);
        }
        //jQuery("#EmailSendSubject").val("FW:" + $(".ticket-title").find("span").html().trim());
        var emailsendsubject = jQuery("#EmailSendSubject").val("Fw:" + title);

        if (emailsendsubject.val().toLowerCase().indexOf("[ticket#") == -1) {
            var tid = jQuery(".ticketidonspan").attr("value");
            var element = jQuery("#EmailSendSubject").val(emailsendsubject.val());// + " [Ticket#" + tid + "]");
        }

        var replayemailsignature = jQuery('#UserEmailSignature').val();
        var html = $("#ticketdetail").find(".emailcontainer").find("#email-body").html().trim();
        CKEDITOR.instances["Emailbody"].setData("<br/>" + html + replayemailsignature);

        if (jQuery("#emailcontentwrapper").find("#email-" + ticketitemid).find(".attachments").hasClass('has-data')) {
            jQuery(".send-attachments").html("").html("<hr style='margin: 10px 0px;' />").show();

            jQuery("#emailcontentwrapper").find("#email-" + ticketitemid).find(".attachments").find(".attachment-item").each(function (index, item) {
                jQuery(".send-attachments").prepend(jQuery(this).html());
            });
        }
        else {
            jQuery(".send-attachments").hide();
        }
        $(".emailcontainer").hide();
        $("#replayPanel").show();
        //$(".CC").hide();
        $(".BCC").hide();
    });

    /*****************************************************
     ** Back to Message
     *****************************************************/
    jQuery(document).on('click', '.cancelemail', function () {
        //1. Change View to send reply all.
        jQuery(".view-email-actions").show();
        jQuery(".send-email-actions").hide();
        $("#emailType").val("");

        $(".emailcontainer").show();
        $("#replayPanel").hide();

        isCC = false;
        isBCC = false;
        $("#UploadAttachmentprogress").html("");
        $('#EmailSendTo').html("");
        $('#EmailSendTo').tagsinput('removeAll');
    });

    /*****************************************************
     ** Upload New Attachment.
     *****************************************************/

    jQuery("#attachNewFiles").change(function () {
        var userfiles = document.getElementById("attachNewFiles").files;
        jQuery(".send-attachments").show();
        for (var i = 0; i < userfiles.length; i++) {
            emailFormData.append("file_" + count, userfiles[i], userfiles[i].name);
            var newItem = "<div id='file_" + count + "' class='btn-group emailattachment newfile' style='margin-bottom: 3px;margin-right: 2px;'><a class='btn bg-slate-400'>" + userfiles[i].name + "</a> <button type='button' class='btn bg-slate-400 dropdown-toggle' data-toggle='dropdown' aria-expanded='false'><span class='caret'></span></button><ul class='dropdown-menu dropdown-menu-right'><li><a class='remove-attachment'><i class='icon-close2'></i>Remove Attachment</a></li></ul></div>";
            jQuery(".send-attachments").prepend(newItem);
            count++;
        }
    });
    //jQuery("#attachNewFiles").change(function () {
    //    //1. Get uploaded files.
    //    var userfiles = document.getElementById("attachNewFiles").files;
    //    //2. show attachments section.
    //    jQuery(".send-attachments").show();

    //    //3. Show Progressbar.
    //    jQuery(".fileuploadprogress").removeClass("hide");

    //    //4. Prepare FormData.
    //    var formData = new FormData();
    //    formData.append("TicketID", jQuery("#ticketid").val());
    //    formData.append("Type", $("#emailType").val());

    //    //5. Append progressbars each for uploaded file.
    //    for (var i = 0; i < userfiles.length; i++) {
    //        var name = userfiles[i].name;
    //        var size = parseInt(parseInt(userfiles[i].size) / 1024);

    //        // append files information in formData.
    //        formData.append(userfiles[i].name, userfiles[i]);

    //        // Update Progressbar.
    //        if (userfiles.length > 1) {
    //            currentProgress = Math.floor((10 / userfiles.length) * (i + 1) + currentProgress);
    //            if (currentProgress > 15) currentProgress = 15;
    //        } else {
    //            currentProgress = 15;
    //        }
    //        jQuery(".fileuploadprogress").width(currentProgress + "%").html(currentProgress + "%");
    //    }

    //    // Display progress upto 95% to the user and put on wait.
    //    progressInterval = setInterval(updateFileUploadProgress, 300);

    //    $.ajax({
    //        type: 'POST',
    //        url: '/Tickets/UploadAttachment',
    //        data: formData,
    //        cache: false,
    //        contentType: false,
    //        processData: false,
    //        success: function (data) {
    //            currentProgress = 100;
    //            jQuery(".fileuploadprogress").width(currentProgress + "%").html(currentProgress + "%")
    //            if (data.error == false) {
    //                var names = data.filename.toString().split(',');

    //                for (var i = 0; i < names.length; i++) {
    //                    var fileinfo = names[i].split(';');
    //                    var newItem = "<div id='" + fileinfo[1] + "' class='btn-group emailattachment newfile' style='margin-bottom: 3px;'><a href='" + fileinfo[2] + "' class='btn bg-slate-400'>" + fileinfo[0] + "</a> <button type='button' class='btn bg-slate-400 dropdown-toggle' data-toggle='dropdown' aria-expanded='false'><span class='caret'></span></button><ul class='dropdown-menu dropdown-menu-right'><li><a class='remove-attachment'><i class='icon-close2'></i>Remove Attachment</a></li></ul></div>";
    //                    jQuery(".send-attachments").prepend(newItem);
    //                }

    //                // Hide Progressbar.
    //                jQuery(".fileuploadprogress").delay(500).hide();
    //            }
    //        },
    //        error: function (xhr, status, error) {
    //            console.log(xhr.responseText);
    //            alert("file is too large or not allowed! see detail in log window");
    //        }
    //    });

    //    //empty the value of input type=file
    //    $("#attachNewFiles").val(null);
    //});

    //$('#UploadAttachment').change(function () {
    //    // select the form and submit
    //    var formData = new FormData();
    //    var files = document.getElementById("UploadAttachment").files;
    //    var UploadAttachmentprogress = $("#UploadAttachmentprogress");
    //    formData.append("TicketID", $("#ticketid").val());
    //    formData.append("Type", $("#emailType").val());
    //    for (var i = 0; i < files.length; i++) {
    //        formData.append(files[i].name, files[i]);
    //        filename = files[i].name;
    //        $(UploadAttachmentprogress).append($.parseHTML(
    //            "<div id='" + filename + "' class='col-md-4 prog'><div class='progress' style='height: 20px;'>" +
    //            files[i].name.toString().substring(0, 30) + " (" + parseInt(parseInt(files[i].size) / 1024) + " KB)" + "<span class='close' style='padding-right:5px;' title='Discard This Attachment' onclick='DeleteAttachment(this)'>&times;</span></div></div>"));
    //    }
    //    $.ajax({
    //        type: 'POST',
    //        url: '/Tickets/EmailAttachment',
    //        data: formData,
    //        cache: false,
    //        contentType: false,
    //        processData: false,
    //        success: function (data) {
    //            if (data.error == false) {
    //                var names = data.filename.toString().split(',');
    //                $("#UploadAttachmentprogress").find(".prog").each(function (index, item) {
    //                    for (var i = 0; i < names.length; i++) {
    //                        if ($(item).attr('id') == names[i].split(';')[0]) {
    //                            $(item).attr('id', names[i].split(';')[1]);
    //                            $(item).find(".progress").addClass("progress-bar-success");
    //                        }
    //                    }
    //                });
    //            }
    //        },
    //        error: function (xhr, status, error) {
    //            console.log(xhr.responseText);
    //            alert("file is too large or not allowed! see detail in log window");
    //        }
    //    });
    //});
    jQuery(document).on('click', '.btnSendEmail', function () {
        var ticketitemid = jQuery(".timelogWithEmailSending").data("ticketitemid");
        var ischecked = false;
        if ($('#tlcheckbox').is(':checked') == true) {
            ischecked = true;
            var projectid = jQuery("#taskprojectid").val();
            var skillid = jQuery("#taskskillid").val();
            var timespent = jQuery("#timespentinminutes").val();
            var logdescription = jQuery("#description").val();
            if (projectid <= 0) {
                new PNotify({
                    title: 'Error',
                    text: "Please select the project",
                    type: 'error',
                    addclass: "stack-bottom-right",
                    hide: true
                });
                return false;
            }
            if (skillid <= 0) {
                new PNotify({
                    title: 'Error',
                    text: 'Please select the skill',
                    type: 'error',
                    addclass: "stack-bottom-right",
                    hide: true
                });
                return false;
            }
            if (timespent == 0) {
                new PNotify({
                    title: 'Error',
                    text: 'Please enter the time spent',
                    type: 'error',
                    addclass: "stack-bottom-right",
                    hide: true
                });
                return false;
            }
            if (logdescription == '') {
                new PNotify({
                    title: 'Error',
                    text: 'Please enter the task description',
                    type: 'error',
                    addclass: "stack-bottom-right",
                    hide: true
                });
                return false;
            }
        }
        var TOemailvalue = $('#EmailSendTo').val().replace(/\;/g, ',');
        var conversationid = $('#conversation').val();
        if (TOemailvalue.indexOf(';') >= 0) {
            new PNotify({
                title: 'Invalid Character',
                text: "Please Remove ; From TO",
                type: 'warning',
                sticker: false,
                addclass: "stack-bottom-right",
                hide: false
            });
            return false;
        }
        var CCemailvalue = $('#EmailSendCC').val().replace(/\;/g, ',');
        if (CCemailvalue.indexOf(';') >= 0) {
            new PNotify({
                title: 'Invalid Character',
                text: "Please Remove ; From CC",
                type: 'warning',
                sticker: false,
                addclass: "stack-bottom-right",
                hide: false
            });
            return false;
        }
        var BCCemailvalue = $('#EmailSendBCC').val().replace(/\;/g, ',');
        if (BCCemailvalue.indexOf(';') >= 0) {
            new PNotify({
                title: 'Invalid Character',
                text: "Please Remove ; From BCC",
                type: 'warning',
                sticker: false,
                addclass: "stack-bottom-right",
                hide: false
            });
            return false;
        }
        var ticketitemid = jQuery("#leftNevMenu").find(".active").data("emailid");
        var EmailSendTo = $("#EmailSendTo").val();
        if (!!EmailSendTo) {
            var ticketID = $("#ticketid").val();
            var emailType = $("#emailType").val();
            var EmailSendBCC = "";
            var EmailSendCC = "";
            if (isCC) { EmailSendCC = $("#EmailSendCC").val().replace(/\;/g, ','); }
            else { EmailSendCC = ""; }
            if (isBCC) { EmailSendBCC = $("#EmailSendBCC").val().replace(/\;/g, ','); }
            else { EmailSendBCC = ""; }
            var EmailSubject = $("#EmailSendSubject").val();
            var Emailbody = CKEDITOR.instances["Emailbody"].getData();
            var Attach = "";
            var ticketitemid = jQuery(".timelogWithEmailSending").data("ticketitemid");
            var projectid = jQuery("#taskprojectid").val();
            var skillid = jQuery("#taskskillid").val();
            var workdate = jQuery("#workdate").val();
            var timespent = jQuery("#timespentinminutes").val();
            var billable = jQuery("#billabletimeinminutes").val();
            var title = jQuery(".ticket-title").text().trim();
            var description = jQuery("#description").val();
            var IsWarning = jQuery("#WarningStatus").val();
            var WarningText = jQuery("#WarningText").val();
            var comments = "";
            $(".send-attachments").find('.emailattachment').each(function (index, item) {
                if ($(item).hasClass("existingfile")) {
                    var id = $(item).attr('id');
                    id = id + '_E';
                    Attach += id + ',';
                }
            });
            var data = {
                type: emailType,
                TO: EmailSendTo,
                CC: EmailSendCC,
                BCC: EmailSendBCC,
                Subject: EmailSubject,
                body: Emailbody,
                TicketID: ticketID,
                Attach: Attach,
                //tcktitemid: ticketitemid,
                //pid: projectid,
                //sid: skillid,
                //spenttime: timespent,
                //billtime: billable,
                //workdate: workdate,
                TicketTitle: title,
                //description: description,
                //comments: comments,
                //ischecked: ischecked
                MessageId: conversationid
            }
            emailFormData.append("data", JSON.stringify(data));
            swal({
                title: "Email sending...",
                text: "Please wait",
                buttons: false,
                showCancelButton: false,
                showConfirmButton: false,
                closeOnClickOutside: false,
                closeOnEsc: false
            });
            $.ajax({
                url: '/GraphMails/SendMail/',
                processData: false,
                contentType: false,
                data: emailFormData,
                type: 'POST',
            }).done(function (data) {
                swal.close();
                // Verify if an error has occured, notify the user.//point
                if (data.error) {
                    new PNotify({
                        title: "Error",
                        text: data.response,
                        type: 'warning',
                        addclass: "stack-bottom-right",
                        hide: false
                    });
                    return false;
                }
                else {
                    $(".btncancelreplay").click();
                    new PNotify({
                        title: 'Success',
                        text: data.response,
                        type: 'success',
                        sticker: false,
                        addclass: "stack-bottom-right",
                        hide: false
                    });
                    jQuery(".view-email-actions").show();
                    jQuery(".send-email-actions").hide();
                    jQuery(".emailcontainer").show();
                    jQuery(".sendemailcontainer").hide();
                }
            });
        }
        else {
            new PNotify({
                title: 'Requird Info is Missing',
                text: "Please Enter To Email Address",
                type: 'warning',
                sticker: false,
                addclass: "stack-bottom-right",
                hide: false
            });
        }
    });

    //'.emailcontainer:not(#skipDiv)')
    //.not('.mail-attachments')
    //$('.emailcontainer').find('a').not('.mail-attachments').each(function () {
    //    var link = $(this).attr("href");
    //    $(this).attr("href", "https://google.com/url?q=" + link);
    //});
    $('.emailcontainer').find('a').each(function () {
        var link = $(this).attr("href");
        $(this).attr("href", "https://google.com/url?q=" + link);
    });

    $('.mail-attachments').find('a').each(function () {
        var link = $(this).attr("href");
        $(this).attr("href", link.substring(25, link.length));
    });

    $(document).on('click', '.DeleteComments', function () {
        var id = $(this).data("id");
        var obj = $(this);
        if (confirm("Are you shur you want to delete this")) {
            $.ajax({
                url: '/Tickets/DeleteCommentById/',
                data: { id: id },
                type: 'POST',
            }).done(function (data) {
                if (data == "Deleted") {
                    $(obj).parent().parent().parent().remove();
                    var length = parseInt($(".commentbody").first("li .media-body").html().length);
                    if (length == 0) {
                        $(".commentbody").append($.parseHTML("<strong class='Norecordfound'>No record found</strong>"));
                    }
                } else { alert("Failed"); }
            });
        }
    });
    $(document).on('click', '.EditComments', function () {
        EditObj = $(this);
        EditCommentID = $(this).data("id");
        var msg = $(this).parent().parent().parent().find("#CommentBody").html();
        CKEDITOR.instances["txtComments"].setData(msg);
        $("#btnAddComments").hide();
        $("#btnSaveComments").show();
        $("#btnCancelComments").show();
        responsiveImg();
    });
    $(document).on('click', '#btnSaveComments', function () {
        var editorData = CKEDITOR.instances['txtComments'].getData();
        if (editorData.length > 0) {
            $.ajax({
                url: "/Tickets/EditCommentById",
                data: {
                    comments: editorData,
                    Id: EditCommentID
                },
                type: 'POST',
            }).done(function (result) {
                if (result.error) {
                    alert("Failed");
                    return false;
                }

                if (result == "Success") {
                    $(EditObj).parent().parent().parent().find("#CommentBody").html(editorData);
                    CKEDITOR.instances["txtComments"].setData("");
                    $("#btnAddComments").show();
                    $("#btnSaveComments").hide();
                    $("#btnCancelComments").hide();
                }
                else { alert("Failed"); }
                return false;
            });
        }
    });
    $(document).on('click', '#btnCancelComments', function () {
        $("#btnAddComments").show();
        $("#btnSaveComments").hide();
        $("#btnCancelComments").hide();
        CKEDITOR.instances["txtComments"].setData("");
    });
    jQuery(function () {
    });
    jQuery(document).on('click', '#btnAddComments', function () {
        var editorData = CKEDITOR.instances['txtComments'].getData();
        var ticketid = $("#ticketid").val();
        if (editorData.length > 0) {
            $.ajax({
                url: "/Tickets/AddCommentByTicketId",
                data: {
                    comments: editorData,
                    ticketId: $("#ticketid").val()
                },
                type: 'POST',
            }).done(function (element) {
                if (element.error) {
                    alert("Failed");
                    return false;
                }

                var commentdate = $.datepicker.formatDate('mm/dd/yy', new Date(Date(element.commenton)));//new Date(parseInt(element.commenton.substr(6))).format("dd/mm/yyyy");
                var commentbody = "<li class='media'><div class='media-left'><a href='#'>";
                if (element.ProfileImage != undefined) {
                    commentbody += "<img id='id" + element.id + "' src='/Content/images/" + element.ProfileImage + "' class='img-circle img-sm' alt='Not Loaded'>";
                }
                else {
                    commentbody += "<img id='id" + element.id + "' src='/Content/images/placeholder.jpg' class='img-circle img-sm' alt='Not Loaded'>";
                }
                commentbody += "</a></div><div class='media-body'><div class='media-heading'><a href='#' class='text-semibold'> " + element.commentbyusername + "</a><span class='media-annotation dotted'>" + commentdate + "</span></div>";
                commentbody += "<div id='CommentBody'>" + element.commentbody + "</div><br/>";
                commentbody += "<ul class='list-inline list-inline-separate text-size-small'>";
                commentbody += "<li class='EditComments' data-id='" + element.id + "'><a>Edit</a></li><li class='DeleteComments' data-id='" + element.id + "'><a >Delete</a></li></ul></div>";
                commentbody += "<div class='media-right'><i data-ticketid='" + ticketid + "' data-commentid='" + element.id + "' class='fa fa-clipboard comment-link'></i></div></li>";
                $(".commentbody").prepend($.parseHTML(commentbody));
                CKEDITOR.instances["txtComments"].setData("");
                $(".Norecordfound").remove();
                responsiveImg();
                return false;
            });
        }
    });
    function responsiveImg() {
        $(".commentbody").find("img").each(function (index, item) {
            if ($(item).hasClass("img-responsive") == false) { $(item).addClass("img-responsive"); }
        });
    }
    jQuery(document).on('click', '.changeticketstatus', function () {
        var tobj = $(this);
        var statusid = jQuery(this).data("statusid");
        var ticketid = jQuery("#ticketid").val();
        var ticketitemid = 0;

        // remove any previous notification.
        $(".usernotification").remove();

        // Send post request to server to change the status.
        $.ajax({
            url: '/tickets/chnageticketstatus/',
            data: {
                id: ticketid,
                status: statusid,
            },
            type: 'POST',
        }).done(function (data) {
            // Verify if an error has occured, notify the user.
            if (data.error) {
                var source = $("#error-notification-template").html();
                var template = Handlebars.compile(source);
                var html = template(data);
                //$("#maincontent").prepend(html);
                $(".usernotification").prepend(html);
                return false;
            }
            $(tobj).remove();

            var drp = "";
            if (statusid <= 2 || statusid == 5) {
                drp += "<button type='button' class='btn btn-xs bg-danger-700 changeticketstatus' data-statusid='3'>Close</button>";
                drp += "<button type='button' class='btn btn-xs bg-danger-700 dropdown-toggle' data-toggle='dropdown'>";
                drp += "<span class='caret'></span>";
                drp += "</button>";
            }
            else {
                drp += "<button type='button' class='btn btn-xs bg-success-700 changeticketstatus' data-statusid='2'>Open</button>";
                drp += "<button type='button' class='btn btn-xs bg-success-700 dropdown-toggle' data-toggle='dropdown'>";
                drp += "<span class='caret'></span>";
                drp += "</button>";
            }

            drp += "<ul class='dropdown-menu' role='menu' style='left:-90px;'>";
            if (statusid <= 3) {
                drp += "<li class='changeticketstatus' data-statusid='5'><a href='#'>Review</a></li>";
            }
            if (statusid <= 2) {
                drp += "<li class='changeticketstatus' data-statusid='4'><a href='#'>Wont Fix</a></li>";
            }
            drp += "</ul>";

            $("#StatusDropdown").html(drp);
            var newstatusvalue;
            if (statusid == 1) {
                newstatusvalue = 'New Task';
            }
            else if (statusid == 2) {
                newstatusvalue = 'In Progress';
            }
            else if (statusid == 3) {
                newstatusvalue = 'Done';
            }
            else if (statusid == 4) {
                newstatusvalue = 'On Hold';
            }
            else if (statusid == 5) {
                newstatusvalue = 'QC';
            }
            else if (statusid == 6) {
                newstatusvalue = 'Assigned';
            }
            else if (statusid == 7) {
                newstatusvalue = 'In Review';
            }
            else if (statusid == 8) {
                newstatusvalue = 'Trash';
            }

            $("#statuschangedValuedAjax").html(newstatusvalue);
            $(".sidebarticketstatus").html(newstatusvalue);
            new PNotify({
                title: 'Success!',
                text: "Successfully status updated to " + newstatusvalue,
                addclass: "stack-bottom-right",
                type: 'success'
            });
            // Update ticket status count.
            //if (statusid == 3) {
            //    statusid = 5;
            //}
            //if (statusid == 4) {
            //    statusid = 8;
            //}

            //////var pcount = $("#" + statusid + ".my_task_status").text();
            //////pcount = parseInt(pcount);
            //////pcount = pcount + 1;
            //////$("#" + statusid + ".my_task_status").text(pcount);

            //////var pagestatusid = $(".filterbystatus").find('.active').prop("id");
            //////var cscount = $("#" + pagestatusid + ".my_task_status").text();
            //////cscount = parseInt(cscount);
            //////cscount = cscount - 1;
            //////$("#" + pagestatusid + ".my_task_status").text(cscount);

            //////// Display success message.
            //////var source = $("#success-notification-template").html();
            //////var template = Handlebars.compile(source);
            //////var html = template(data);
            //////$("#maincontent").prepend(html);

            //////// Remove the current ticket row.
            //////var ticketrow = $("tr#" + ticketid);
            //////ticketrow.animate({ backgroundColor: 'yellow' }, 1000).fadeOut(1000, function () {
            //////    ticketrow.remove();
            //////});
        });

        return false;
    });

    $(document).on('click', '.add-new-skill', function () {
        $('#skillname').val('');
        $('#AddSkillModal').modal('toggle');
    });

    $(document).on('click', '.add-credential', function () {
        $('#crendentialtypeid').select2().val('2').trigger('change');
        $('#credentiallevelid').select2().val('4').trigger('change');
        $('#credentialcategoryid').select2().val('9').trigger('change');
        $('#AddCredentialModal').modal('toggle');
    });

    $('#AddProjectModal').on('hidden.bs.modal', function () {
        $(this).find('#projectname').val('');
        $(this).find("textarea,select").val('').end();
    });

    $(document).on('click', '.add-new-project', function () {
        jQuery('#clientid').select2();
        $('#AddProjectModal').modal('toggle');
    });

    $(document).on("click", ".save-credential", function () {
        var categoryname = $("#credentialcategoryid option:selected").text();
        var projectname = $("#taskprojectid option:selected").text();
        var title = $('#ctitle').val();
        var url = $("#url").val();
        var username = $("#username").val();
        var password = $("#password").val();
        var ccategoryid = $("#credentialcategoryid").val();
        var ctypeid = $("#crendentialtypeid").val();
        var clevelid = $("#credentiallevelid").val();
        var host = $("#host").val();
        var port = $("#port").val();
        var comments = $("#ccomment").val();
        var projectid = $("#cprojectid").val();
        var skillid = $("#cskillid").val();

        if (ccategoryid == null || ccategoryid == "") {
            new PNotify({
                title: 'Error!',
                text: "Please select credential category!",
                addclass: "stack-bottom-right",
                type: 'error'
            });
            return false;
        }
        else if (ctypeid == null || ctypeid == "") {
            new PNotify({
                title: 'Error!',
                text: "Please select credential type!",
                addclass: "stack-bottom-right",
                type: 'error'
            });
            return false;
        }
        else if (clevelid == null || clevelid == "") {
            new PNotify({
                title: 'Error!',
                text: "Please select credential Level!",
                addclass: "stack-bottom-right",
                type: 'error'
            });
            return false;
        }
        else if (projectid == null || projectid == "") {
            new PNotify({
                title: 'Error!',
                text: "Project not selected",
                addclass: "stack-bottom-right",
                type: 'error'
            });
            return false;
        }
        else if (username == null || username == "") {
            new PNotify({
                title: 'Error!',
                text: "Please add username",
                addclass: "stack-bottom-right",
                type: 'error'
            });
            return false;
        }
        else if (password == null || password == "") {
            new PNotify({
                title: 'Error!',
                text: "Please add password",
                addclass: "stack-bottom-right",
                type: 'error'
            });
            return false;
        }
        else {
            $.ajax({
                url: '/tickets/CreateCredential/',
                data: {
                    url: url,
                    username: username,
                    password: password,
                    projectid: projectid,
                    catogeryid: ccategoryid,
                    typeid: ctypeid,
                    levelid: clevelid,
                    skillid: skillid,
                    title: title,
                    host: host,
                    port: port,
                    comments: comments
                },
                type: 'POST',
            }).done(function (data) {
                if (data.error) {
                    alert(data.msg);
                    return false;
                }
                else {
                    var tablerow = "<tr><td colspan='4'>";
                    if (port != null || port != "") {
                        tablerow += "<span class='credential_title'>" + title + "</span><br />";
                    }
                    tablerow += "<a target='_blank' href=" + url + ">" + url + "</a><br /><span class='label bg-grey-400'>" + clevelid + "</span>" +
                        "<span class='label bg-success-400'>" + categoryname + "</span>";
                    if (port != null || port != "") {
                        tablerow += "<span class='label bg-indigo-400'>Port:" + port + "</span>";
                    }
                    if (host != null || host != "") {
                        tablerow += "<span class='label bg-indigo-400'>Host/IP:" + host + "</span>";
                    }
                    tablerow += "<span class='text-semibold label bg-blue-400'>" + projectname + "</span></td>" +
                        "<td><span class='text-semibold usernameval'>" + username + "</span><a href = '#' class='usernameclick' style='color:green;'>&nbsp; &nbsp;<i class='fa fa-clipboard' title='Copy to Clipboard'>" +
                        "</i></a><br /><span class='text-muted passwordval'>" + password + "</span><a href = '#' class='pwdclick' style='color:green;'>&nbsp; &nbsp;<i class='fa fa-clipboard' title='Copy to Clipboard'>" +
                        "</i></a> </td>" +
                        "<td>" + comments + "</td><td><a class='btn btn-primary btn-xs editcredentialsbtn' href='/credentials/Edit/" + data.id + "' data-id=" + data.id + " style='color: white'>Edit</a></td>></tr>";
                    $('.cTableList').append(tablerow);
                    $('#AddCredentialModal').modal('hide');;
                    return false;
                }
            });
        }
    });

    jQuery(document).on("click", ".add-project", function () {
        var clientid = $('#clientid').val();
        var projectname = $('#projectname').val();
        if (clientid == null || clientid == "") {
            sweetAlert("Sorry", "Please select a client from the list.", "error");
            return false;
        }
        if (projectname == null || projectname == "") {
            sweetAlert("Sorry", "Please add the project name.", "error");
            return false;
        }
        $.ajax({
            url: '/home/AddProject/',
            data: {
                clientid: clientid,
                projectname: projectname
            },
            type: 'POST',
        }).done(function (data) {
            if (data.error) {
                alert(data.message);
                return false;
            }
            else {
                $('#Assignmenttaskproject').append($('<option>', {
                    value: data.prid,
                    text: data.prname
                }));
            }
            jQuery('#AddProjectModal').modal('hide');
        });
    });

    jQuery(document).on("click", ".add-skill", function () {
        var skillname = $('#skillname').val();
        if (skillname == "" || skillname == null) {
            sweetAlert("Sorry", "Please add the skill name.", "error");
            return false;
        }
        $.ajax({
            url: '/home/AddSkill/',
            data: {
                name: skillname
            },
            type: 'POST'
        }).done(function (data) {
            if (data.error) {
                alert(data.message);
                return false;
            }
            else {
                $('#Assignmenttaskskill').append($('<option>', {
                    value: data.skid,
                    text: data.skname
                }));
            }
            jQuery('#AddSkillModal').modal('hide');
        });
    });

    $(document).on('click', '.changer', function () {
        $('.task-project-link').addClass('hidden');
        $('.task-project-dropdown').removeClass('hidden');
    });

    $(document).on('click', '.cross', function () {
        $('.task-project-link').removeClass('hidden');
        $('.task-project-dropdown').addClass('hidden');
    });

    jQuery(document).on('click', '.comment-link', function () {
        var ticketid = $(this).data('ticketid');
        var commentid = $(this).data('commentid');
        var url = "https://" + window.location.host + "/tickets/comment/" + ticketid + "/" + commentid;
        var $temp = $("<input>");
        $("body").append($temp);
        $temp.val(url.trim()).select();
        document.execCommand("copy");
        $temp.remove();
        new PNotify({
            title: 'Success!',
            text: "Successfully copied comment link",
            addclass: "stack-bottom-right",
            type: 'success'
        });
        return false;
    });
});

function initDefaults() {
    clearAddTime();
    loadTaskDetails();
    loadActiveTicketDetails();
    loadAssignedToTagInputs();

    loadEmailSendTagInputs();
    //loadEmailSendToTagInputs();
    //loadEmailSendCcTagInputs();
    //loadEmailSendBccTagInputs();

    loadTicketUsers();
    //team
    loadnewTeamsTagInputs();
    loadTicketnewteam();
    setInterval(function () {
        if (counter > 0) {
            jQuery('.assignedusersinput').tagsinput('removeAll');
            loadTicketUsers();
            counter++;
            if (counter > 5) { counter = 0; }
        }
    }, 250);
}
function clearAddTime() {
    jQuery('#workdate').val(getFormattedDate(new Date()));
    jQuery("#timespentinminutes").val("0");
    jQuery("#billabletimeinminutes").val("0");
    jQuery("#description").val("");
}
function loadEmail(ticketitemid) {
    $("#btnReplay").show();
    $("#btnReplayall").show();
    $("#btnforword").show();
    $(".hiddenfield").hide();
    jQuery(".mail-modifiedtime").html(jQuery("#emailcontentwrapper").find("#email-" + ticketitemid).find(".lastmodifiedtime").html());
    jQuery(".mail-sender").html(jQuery("#emailcontentwrapper").find("#email-" + ticketitemid).find(".sender").html());
    jQuery(".mail-subject").html(jQuery("#emailcontentwrapper").find("#email-" + ticketitemid).find(".subject").html());
    jQuery(".mail-to-addresses").html("To: " + jQuery("#emailcontentwrapper").find("#email-" + ticketitemid).find(".to-addresses").html());

    if (jQuery("#emailcontentwrapper").find("#email-" + ticketitemid).find(".cc-addresses").hasClass('has-data')) {
        jQuery(".mail-cc-addresses").html("Cc: " + jQuery("#emailcontentwrapper").find("#email-" + ticketitemid).find(".cc-addresses").html());
    } else {
        jQuery(".mail-cc-addresses").hide();
    }

    if (jQuery("#emailcontentwrapper").find("#email-" + ticketitemid).find(".bcc-addresses").hasClass('has-data')) {
        jQuery(".mail-bcc-addresses").html("Cc: " + jQuery("#emailcontentwrapper").find("#email-" + ticketitemid).find(".bcc-addresses").html());
    } else {
        jQuery(".mail-bcc-addresses").hide();
    }

    if (jQuery("#emailcontentwrapper").find("#email-" + ticketitemid).find(".attachments").hasClass('has-data')) {
        jQuery(".mail-attachments").html("");

        jQuery("#emailcontentwrapper").find("#email-" + ticketitemid).find(".attachments").find(".attachment-item").each(function (index, item) {
            jQuery(".mail-attachments").append(jQuery(this).html());
        });
    }
    else {
        jQuery(".mail-attachments").hide();
    }

    var emailbody = jQuery("#emailcontentwrapper").find("#email-" + ticketitemid).find(".email-body").html();

    jQuery("#email-body").html(HTMLDecode(emailbody));
    jQuery('#email-body').find('a[href^="http://"], a[href^="https://"]').attr('target', '_blank');

    jQuery(".emailitem").removeClass("active");
}
function loadTaskDetails() {
    var projectid = jQuery(".startworking").data("projectid");
    var skillid = jQuery(".startworking").data("skillid");

    jQuery("#taskprojectid").val(projectid).trigger("change");
    jQuery("#taskskillid").val(skillid).trigger("change");
    jQuery("#Assignmenttaskproject").val(projectid).trigger("change");
    jQuery("#Assignmenttaskskill").val(skillid).trigger("change");
}
function loadActiveTicketDetails() {
    var ticketitemid = jQuery(".emailcollection li.active").data("emailid");
    var graphEmailId = jQuery(".emailcollection li.active").data("graphemailid");

    jQuery("#conversation").val(graphEmailId);
    loadEmail(ticketitemid);
    jQuery(".emailitem:first").addClass("active");

    return false;
}
function loadAssignedToTagInputs() {
    // Use Bloodhound engine
    var users = new Bloodhound({
        datumTokenizer: Bloodhound.tokenizers.obj.whitespace('text'),
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        limit: 10,
        prefetch: {
            ttl: 1,
            url: '/tickets/PrefetchUsers',
            prepare: function (settings) {
                settings.type = "POST";
                settings.contentType = "application/json; charset=UTF-8";
                return settings;
            },
            remote: function (query, settings) {
                settings.type = "POST";
                settings.data = { q: query, foo: 'bar' }; // you can pass some data if you need to
                return settings;
            }
        }
    });

    // Kicks off the loading/processing of `local` and `prefetch`
    users.clear();
    users.clearPrefetchCache();
    users.clearRemoteCache();
    users.initialize();

    // Define element
    elt = $('.newassignedusersinput');

    // Initialize
    elt.tagsinput({
        itemValue: 'value',
        itemText: 'text'
    });

    // Attach Typeahead
    elt.tagsinput('input').typeahead(null, {
        name: 'assignedusers',
        valueKey: 'value',
        displayKey: 'text',
        source: users.ttAdapter(),
        templates: '<p>{{text}}</p>'
    }).bind('typeahead:selected', $.proxy(function (obj, datum) {
        this.tagsinput('add', datum);
        this.tagsinput('input').typeahead('val', '');
    }, elt));

    /////--------------------------------------
    //Define element
    elt = $('.assignedusersinput');

    // Initialize
    elt.tagsinput({
        itemValue: 'value',
        itemText: 'text'
    });

    // Attach Typeahead
    elt.tagsinput('input').typeahead(null, {
        name: 'assignedusers',
        valueKey: 'value',
        displayKey: 'text',
        source: users.ttAdapter(),
        templates: '<p>{{text}}</p>'
    }).bind('typeahead:selected', $.proxy(function (obj, datum) {
        this.tagsinput('add', datum);
        this.tagsinput('input').typeahead('val', '');
    }, elt));
}

function loadEmailSendTagInputs() {
    // Use Bloodhound engine
    var emails = new Bloodhound({
        datumTokenizer: Bloodhound.tokenizers.obj.whitespace('text'),
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        limit: 10,
        prefetch: {
            ttl: 1,
            url: '/tickets/PrefetchContactEmails',
            prepare: function (settings) {
                settings.type = "POST";
                settings.contentType = "application/json; charset=UTF-8";
                return settings;
            },
            remote: function (query, settings) {
                settings.type = "POST";
                settings.data = { q: query, foo: 'bar' }; // you can pass some data if you need to
                return settings;
            }
        }
    });

    // Kicks off the loading/processing of `local` and `prefetch`
    emails.clear();
    emails.clearPrefetchCache();
    emails.clearRemoteCache();
    emails.initialize();

    // Define element
    eltTo = $('#EmailSendTo');
    // Initialize
    eltTo.tagsinput({
        itemValue: 'value',
        itemText: 'text',
    });
    // Attach Typeahead
    eltTo.tagsinput('input').typeahead(null, {
        name: 'EmailSendTo',
        valueKey: 'value',
        displayKey: 'text',
        source: emails.ttAdapter(),
        templates: '<p>{{text}}</p>'
    }).bind('typeahead:selected', $.proxy(function (obj, datum) {
        this.tagsinput('add', datum);
        this.tagsinput('input').typeahead('val', '');
    }, eltTo));

    eltCC = $('#EmailSendCC');
    // Initialize
    eltCC.tagsinput({
        itemValue: 'value',
        itemText: 'text',
    });
    // Attach Typeahead
    eltCC.tagsinput('input').typeahead(null, {
        name: 'EmailSendCC',
        valueKey: 'value',
        displayKey: 'text',
        source: emails.ttAdapter(),
        templates: '<p>{{text}}</p>'
    }).bind('typeahead:selected', $.proxy(function (obj, datum) {
        this.tagsinput('add', datum);
        this.tagsinput('input').typeahead('val', '');
    }, eltCC));

    // Define element
    eltBCC = $('#EmailSendBCC');
    // Initialize
    eltBCC.tagsinput({
        itemValue: 'value',
        itemText: 'text',
    });
    // Attach Typeahead
    eltBCC.tagsinput('input').typeahead(null, {
        name: 'EmailSendBCC',
        valueKey: 'value',
        displayKey: 'text',
        source: emails.ttAdapter(),
        templates: '<p>{{text}}</p>'
    }).bind('typeahead:selected', $.proxy(function (obj, datum) {
        this.tagsinput('add', datum);
        this.tagsinput('input').typeahead('val', '');
    }, eltBCC));
}
//function loadEmailSendCcTagInputs() {
//    // Use Bloodhound engine
//    var emails = new Bloodhound({
//        datumTokenizer: Bloodhound.tokenizers.obj.whitespace('value'),
//        queryTokenizer: Bloodhound.tokenizers.whitespace,
//        limit: 10,
//        prefetch: {
//            ttl: 1,
//            url: '/tickets/PrefetchContactEmails',
//            prepare: function (settings) {
//                settings.type = "POST";
//                settings.contentType = "application/json; charset=UTF-8";
//                return settings;
//            },
//            remote: function (query, settings) {
//                settings.type = "POST";
//                settings.data = { q: query, foo: 'bar' }; // you can pass some data if you need to
//                return settings;
//            }
//        }
//    });
//    emails.clear();
//    emails.clearPrefetchCache();
//    emails.clearRemoteCache();
//    // Kicks off the loading/processing of `local` and `prefetch`
//    emails.initialize();

//    // Define element
//    elt = $('#EmailSendCC');

//    // Initialize
//    elt.tagsinput({
//        itemValue: 'value',
//        itemText: 'text',
//    });

//    // Attach Typeahead
//    elt.tagsinput('input').typeahead(null, {
//        name: 'EmailSendCC',
//        valueKey: 'value',
//        displayKey: 'text',
//        source: emails.ttAdapter(),
//        templates: '<p>{{text}}</p>'
//    }).bind('typeahead:selected', $.proxy(function (obj, datum) {
//        this.tagsinput('add', datum);
//        this.tagsinput('input').typeahead('val', '');
//    }, elt));

//}
//function loadEmailSendBccTagInputs() {
//    // Use Bloodhound engine
//    var emails = new Bloodhound({
//        datumTokenizer: Bloodhound.tokenizers.obj.whitespace('value'),
//        queryTokenizer: Bloodhound.tokenizers.whitespace,
//        limit: 10,
//        prefetch: {
//            ttl: 1,
//            url: '/tickets/PrefetchContactEmails',
//            prepare: function (settings) {
//                settings.type = "POST";
//                settings.contentType = "application/json; charset=UTF-8";
//                return settings;
//            },
//            remote: function (query, settings) {
//                settings.type = "POST";
//                settings.data = { q: query, foo: 'bar' }; // you can pass some data if you need to
//                return settings;
//            }
//        }
//    });

//    emails.clear();
//    emails.clearPrefetchCache();
//    emails.clearRemoteCache();
//    // Kicks off the loading/processing of `local` and `prefetch`
//    emails.initialize();

//    // Define element
//    elt = $('#EmailSendBCC');

//    // Initialize
//    elt.tagsinput({
//        itemValue: 'value',
//        itemText: 'text',
//    });

//    // Attach Typeahead
//    elt.tagsinput('input').typeahead(null, {
//        name: 'EmailSendBCC',
//        valueKey: 'value',
//        displayKey: 'text',
//        source: emails.ttAdapter(),
//        templates: '<p>{{text}}</p>'
//    }).bind('typeahead:selected', $.proxy(function (obj, datum) {
//        this.tagsinput('add', datum);
//        this.tagsinput('input').typeahead('val', '');
//    }, elt));

//}

function loadTicketUsers() {
    user = false;
    jQuery(".tua").each(function (index) {
        var userid = jQuery(this).data("userid");
        var username = jQuery(this).html();
        jQuery('.assignedusersinput').tagsinput('add', { value: userid, text: username });
    });
    user = true;
}
function loadEstimateTime() {
    var ticketid = jQuery("#ticketid").val();
    $.ajax({
        url: '/Home/FetchEstimatetasktime/',
        data: {
            Ticketid: ticketid
        },
        type: 'GET',
    }).done(function (data) {
        jQuery("#estimateTimeInput").val(data.estimatetime).trigger("change");
        jQuery("#EstimateTimeHidden").val(data.estimatetime).trigger("change");
    });
}
function loadTicketnewUsers() {
    jQuery('.newassignedusersinput').tagsinput('removeAll');
    jQuery(".tua").each(function (index) {
        var userid = jQuery(this).data("userid");
        var username = jQuery(this).html();
        jQuery('.newassignedusersinput').tagsinput('add', { value: userid, text: username });
    });
}
function loadTicketnewteam() {
    jQuery('.assignedteaminput').tagsinput('removeAll');
    jQuery(".tuat").each(function (index) {
        var userid = jQuery(this).data("userid");
        var username = jQuery(this).html();
        jQuery('.assignedteaminput').tagsinput('add', { value: userid, text: username });
    });
}
function getFormattedDate(userDate) {
    var myDateString = ('0' + (userDate.getMonth() + 1)).slice(-2) + '/'
        + ('0' + userDate.getDate()).slice(-2) + '/'
        + userDate.getFullYear();

    return myDateString;
}
var EditCommentID;
var EditObj;
function HTMLDecode(s) { return jQuery('<div></div>').html(s).text(); }

function loadnewTeamsTagInputs() {
    // Use Bloodhound engine
    var teams = new Bloodhound({
        datumTokenizer: Bloodhound.tokenizers.obj.whitespace('text'),
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        limit: 10,
        prefetch: {
            url: '/tickets/PrefetchTeams',
            prepare: function (settings) {
                settings.cache = false;
                settings.type = "POST";
                settings.contentType = "application/json; charset=UTF-8";
                return settings;
            },
            remote: function (query, settings) {
                settings.cache = false;
                settings.type = "POST";
                settings.data = { q: query, foo: 'bar' }; // you can pass some data if you need to
                return settings;
            }
        }
    });

    // Kicks off the loading/processing of `local` and `prefetch`
    teams.initialize();

    // Define element
    elt = $('.assignedteaminput');

    // Initialize
    elt.tagsinput({
        itemValue: 'value',
        itemText: 'text'
    });

    elt.tagsinput('input').typeahead(null, {
        name: 'assignedteams',
        valueKey: 'value',
        displayKey: 'text',
        source: teams.ttAdapter(),
        templates: '<p>{{text}}</p>'
    }).bind('typeahead:selected', $.proxy(function (obj, datum) {
        this.tagsinput('add', datum);
        this.tagsinput('input').typeahead('val', '');
    }, elt));
}
function loadModalData() {
    loadTicketnewUsers();
    loadTicketnewteam();
    loadEstimateTime();
}
function reloadAssignment() {
    $.get("/tickets/TicketItemAssigned?id=" + jQuery("#ticketid").val(), function (data, status) {
        var ticketuserscontainer = $("#ticketuserscontainer");
        $(ticketuserscontainer).html("");
        $(data.ticketusers).each(function (index, item) {
            var span = "<span class='tua' data-userid='" + item.assignedtousersid + "'> " + item.user.FullName + " </span>";
            $(ticketuserscontainer).append(span);
        });
        var ticketTeamcontainer = $("#ticketTeamcontainer");
        $(ticketTeamcontainer).html("");
        $(data.ticketTeam).each(function (index, item) {
            var span = "<span class='tuat' data-userid='" + item.teamid + "'> " + item.team.name + " </span>";
            $(ticketTeamcontainer).append(span);
        });
    });
}

//function progress(e) {
//    if (e.lengthComputable) {
//        var max = e.total;
//        var current = e.loaded;

//        var Percentage = (current * 100) / max;
//        console.log(Percentage);

//        if (Percentage >= 100) {
//            // process completed
//        }
//    }
//}
//function DeleteAttachment(e) {
//    var id = $(e).parent().parent().attr('id');
//    $(e).parent().parent().remove();
//    $.ajax({
//        type: 'POST',
//        url: '/Tickets/DeleteAttachment?TicketReplayID=' + id,
//        cache: false,
//        contentType: false,
//        processData: false,
//        success: function (data) {
//            if (data.error == false) {
//                console.log("File deleted successfully");
//            }
//        },
//        error: function (xhr, status, error) {
//            console.log(xhr.responseText);
//            alert("file not deleted ! see detail in log window");
//        }
//    });
//}
//new attachment upload.
//var currentProgress = 5;
//var progressInterval = null;
//function updateFileUploadProgress() {
//    //alert(currentProgress);
//    currentProgress = currentProgress + 5;
//    jQuery(".fileuploadprogress").width(currentProgress + "%").html(currentProgress + "%");

//    if (currentProgress > 90) {
//        clearInterval(progressInterval);
//        progressInterval = null;
//    }
//}