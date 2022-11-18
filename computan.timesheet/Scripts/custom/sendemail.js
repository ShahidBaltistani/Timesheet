jQuery(function() {
    /*****************************************************
     ** Associate controls.
     *****************************************************/
    // Start Working Modal.
    $(".CC").hide();
    $(".BCC").hide();
    // Full featured editor
    var replayemailsignature = jQuery("#UserEmailSignature").val();
    var html = $("#Emailbody").html();
    CKEDITOR.instances["Emailbody"].setData("<br/>" + replayemailsignature + "<hr><br/>" + html);
    /*****************************************************
     ** Gloabl actions performed on every load.
     *****************************************************/
    loadEmailSendTagInputs();
    /*****************************************************
     ** Remove Attachment.
     *****************************************************/
    jQuery(document).on("click",
        ".remove-attachment",
        function() {
            $(this).closest(".emailattachment").remove();
            var id = $(this).parent().parent().parent().attr("id");
            emailFormData.delete(id);
        });
    /*****************************************************
     ** Back to Message
     *****************************************************/
    jQuery(document).on("click",
        ".cancelemail",
        function() {
            //1. Change View to send reply all.
            jQuery(".view-email-actions").show();
            jQuery(".send-email-actions").hide();
            $("#emailType").val("");
            $(".emailcontainer").show();
            $("#replayPanel").hide();
            isCC = false;
            isBCC = false;
            $("#UploadAttachmentprogress").html("");
        });
    /*****************************************************
     ** Upload New Attachment.
     *****************************************************/
    var emailFormData = new FormData();
    var count = 1;
    jQuery("#attachNewFiles").change(function() {
        var userfiles = document.getElementById("attachNewFiles").files;
        jQuery(".send-attachments").show();
        for (var i = 0; i < userfiles.length; i++) {
            emailFormData.append("file_" + count, userfiles[i], userfiles[i].name);
            var newItem = "<div id='file_" +
                count +
                "' class='btn-group emailattachment newfile' style='margin-bottom: 3px; margin-right: 2px;'><a class='btn bg-slate-400'>" +
                userfiles[i].name +
                "</a> <button type='button' class='btn bg-slate-400 dropdown-toggle' data-toggle='dropdown' aria-expanded='false'><span class='caret'></span></button><ul class='dropdown-menu dropdown-menu-right'><li><a class='remove-attachment'><i class='icon-close2'></i>Remove Attachment</a></li></ul></div>";
            jQuery(".send-attachments").prepend(newItem);
            count++;
        }
        $("#attachNewFiles").val(null);
    });
    jQuery(".btnSendEmail").click(function() {
        var TOemailvalue = $("#EmailSendTo").val();
        if (TOemailvalue.indexOf(";") >= 0) {
            new PNotify({
                title: "Invalid Character",
                text: "Please Remove ; From TO",
                type: "warning",
                hide: false
            });
            return false;
        }
        var CCemailvalue = $("#EmailSendCC").val();
        if (CCemailvalue.indexOf(";") >= 0) {
            new PNotify({
                title: "Invalid Character",
                text: "Please Remove ; From CC",
                type: "warning",
                hide: false
            });
            return false;
        }
        var BCCemailvalue = $("#EmailSendBCC").val();
        if (BCCemailvalue.indexOf(";") >= 0) {
            new PNotify({
                title: "Invalid Character",
                text: "Please Remove ; From BCC",
                type: "warning",
                hide: false
            });
            return false;
        }
        var EmailSendTo = $("#EmailSendTo").val();
        if (!!EmailSendTo) {
            var sentItemID = $("#SentItemID").val();
            var emailType = $("#emailType").val();
            var EmailSendBCC = "";
            var EmailSendCC = "";
            if (isCC) {
                EmailSendCC = $("#EmailSendCC").val();
            } else {
                EmailSendCC = "";
            }
            if (isBCC) {
                EmailSendBCC = $("#EmailSendBCC").val();
            } else {
                EmailSendBCC = "";
            }
            var EmailSubject = $("#EmailSendSubject").val();
            var Emailbody = CKEDITOR.instances["Emailbody"].getData();
            var Attach = "";
            $(".send-attachments").find(".emailattachment").each(function(index, item) {
                if ($(item).hasClass("existingfile")) {
                    var id = $(item).attr("id");
                    id = id + "_E";
                    Attach += id + ",";
                }
            });
            var data = {
                type: emailType,
                TO: EmailSendTo,
                CC: EmailSendCC,
                BCC: EmailSendBCC,
                Subject: EmailSubject,
                body: Emailbody,
                Attach: Attach,
                SentItemID: sentItemID
            };
            emailFormData.append("data", JSON.stringify(data));
            Swal.fire({
                title: "Email sending...",
                allowEscapeKey: false,
                allowOutsideClick: false,
                showConfirmButton: false,
                didOpen: () => {
                    Swal.showLoading();
                }
            });
            $.ajax({
                url: "/GraphMails/SendMail/",
                processData: false,
                contentType: false,
                data: emailFormData,
                type: "POST",
            }).done(function(data) {
                swal.close();
                // Verify if an error has occured, notify the user.//point
                if (data.error) {
                    new PNotify({
                        title: "Error",
                        text: data.response,
                        type: "warning",
                        hide: false
                    });
                    return false;
                } else {
                    //$(".btncancelreplay").click();
                    $("#EmailSendTo").val("");
                    $("#EmailSendCC").val("");
                    $("#EmailSendBCC").val("");
                    $("#EmailSendSubject").val("");
                    $("#Emailbody").val("");
                    window.location.href = "http://timesheet.computan.com/tickets/index/1";
                    new PNotify({
                        title: "Success",
                        text: "Email Send Successfully",
                        type: "success",
                        hide: false
                    });
                    jQuery(".view-email-actions").show();
                    jQuery(".send-email-actions").hide();
                    jQuery(".emailcontainer").show();
                    jQuery(".sendemailcontainer").hide();
                }
            });
        } else {
            new PNotify({
                title: "Requird Info is Missing",
                text: "Please Enter To Email Address",
                type: "warning",
                hide: false
            });
        }
    });
});

function loadEmailSendTagInputs() {
    // Use Bloodhound engine
    var emails = new Bloodhound({
        datumTokenizer: Bloodhound.tokenizers.obj.whitespace("text"),
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        limit: 10,
        prefetch: {
            url: "/tickets/PrefetchContactEmails",
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
    emails.clear();
    emails.clearPrefetchCache();
    emails.clearRemoteCache();
    // Kicks off the loading/processing of `local` and `prefetch`
    emails.initialize();
    // Define element
    eltTo = $("#EmailSendTo");
    // Initialize
    eltTo.tagsinput({
        itemValue: "value",
        itemText: "text",
    });
    // Attach Typeahead
    eltTo.tagsinput("input").typeahead(null,
        {
            name: "EmailSendTo",
            valueKey: "value",
            displayKey: "text",
            source: emails.ttAdapter(),
            templates: "<p>{{text}}</p>"
        }).bind("typeahead:selected",
        $.proxy(function(obj, datum) {
                this.tagsinput("add", datum);
                this.tagsinput("input").typeahead("val", "");
            },
            eltTo));

    // Define element
    eltCC = $("#EmailSendCC");
    // Initialize
    eltCC.tagsinput({
        itemValue: "value",
        itemText: "text",
    });
    // Attach Typeahead
    eltCC.tagsinput("input").typeahead(null,
        {
            name: "EmailSendCC",
            valueKey: "value",
            displayKey: "text",
            source: emails.ttAdapter(),
            templates: "<p>{{text}}</p>"
        }).bind("typeahead:selected",
        $.proxy(function(obj, datum) {
                this.tagsinput("add", datum);
                this.tagsinput("input").typeahead("val", "");
            },
            eltCC));

    eltBCC = $("#EmailSendBCC");
    // Initialize
    eltBCC.tagsinput({
        itemValue: "value",
        itemText: "text",
    });
    // Attach Typeahead
    eltBCC.tagsinput("input").typeahead(null,
        {
            name: "EmailSendBCC",
            valueKey: "value",
            displayKey: "text",
            source: emails.ttAdapter(),
            templates: "<p>{{text}}</p>"
        }).bind("typeahead:selected",
        $.proxy(function(obj, datum) {
                this.tagsinput("add", datum);
                this.tagsinput("input").typeahead("val", "");
            },
            eltBCC));
}