jQuery(function() {
    var count = $(".tbodyconditions").find("tr").length;
    if (count == 0) {
        $(".andordiv").addClass("hide");
    } else {
        $(".andordiv").removeClass("hide");
    }
    var count = $(".tbodyexceptions").find("tr").length;
    if (count == 0) {
        $(".andordiv-exception").addClass("hide");
    } else {
        $(".andordiv-exception").removeClass("hide");
    }
    /**********************************
    *** Perform Global Initializations.
    ***********************************/
    //jQuery("#ruleactionvalue").select2();
    //jQuery("#ruleactiontypeid").select2();
    //jQuery("#projectid").select2();
    //jQuery("#skillid").select2();
    //jQuery("#ruleconditiontypeid").select2();
    //jQuery("#ruleexceptiontypeid").select2();

    /**********************************
    *** Rule Conidtions Scripts.
    ***********************************/

    // Add a new rule.
    jQuery(document).on("click",
        "#btnsavecondition",
        function() {
            var newRuleCondition = {
                id: 0,
                ruleid: jQuery("#ruleid").val(),
                isrequired: $("input[name=isrequired]:checked").val(),
                ruleconditiontypeid: jQuery("#ruleconditiontypeid").val(),
                ruleconditionvalue: jQuery("#ruleconditionvalue").val()
            };
            if (jQuery("#ruleconditiontypeid").val() == "") {
                sweetAlert("Sorry", "The condition type value is requied!", "error");
                return false;
            }
            if (jQuery("#ruleconditionvalue").val() == "") {
                sweetAlert("Sorry", "The condition value is requied!", "error");
                return false;
            }
            jQuery.ajax({
                url: "/ruleconditions/create",
                type: "POST",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(newRuleCondition),
                success: function(result) {
                    if (result.success) {
                        if (result.success) {
                            $(".rulesconditions").html(result.ruleconditionlist);
                            $("#ruleconditiontypeid").val("").trigger("change");
                            $("#ruleconditionvalue").val("");
                            $(".tbodyconditions").removeClass("emptytr");
                            var count = $(".tbodyconditions").find("tr").length;
                            if (count == 0) {
                                $(".andordiv").addClass("hide");
                            } else {
                                $(".andordiv").removeClass("hide");
                            }
                        }
                    }
                },
                error: function(result) {
                    alert("Failed");
                }
            });

            return false;
        });

    // Edit existing rule.
    jQuery(document).on("click",
        ".eidtrulescondition",
        function() {
            tr = $(this).closest("tr");
            var id = $(this).data("id");
            var href = "/RuleConditions/edit/" + id;
            jQuery.ajax({
                url: href,
                type: "GET",
                data: "",
                success: function(result) {
                    jQuery(".rulesconditionsform").html(result);
                    $("#btnsavecondition").addClass("btneditcondition");
                    $("#btnsavecondition").val("Update Condition");
                    $(".btneditcondition").removeAttr("id");
                    $("html,body").animate({
                            scrollTop: $(".rulesconditionsform").offset().top - 150
                        },
                        "slow");
                    var count = $(".tbodyconditions").find("tr").length;
                    if (count == 0 || count == 1) {
                        $(".andordiv").addClass("hide");
                    } else {
                        $(".andordiv").removeClass("hide");
                    }
                },
                error: function(result) {
                    alert("Failed");
                }
            });
            return false;
        });
    // post rule condition edit
    jQuery(document).on("click",
        ".btneditcondition",
        function() {
            var count = $(".tbodyconditions").find("tr").length;
            if (count == 0 || count == 1) {
                $(".andordiv").addClass("hide");
            } else {
                $(".andordiv").removeClass("hide");
            }

            var newRuleCondition = {
                id: $("#id").val(),
                ruleid: jQuery("#ruleid").val(),
                isrequired: $("input[name=isrequired]:checked").val(),
                ruleconditiontypeid: jQuery("#ruleconditiontypeid").val(),
                ruleconditionvalue: jQuery("#ruleconditionvalue").val()
            };
            if (jQuery("#ruleconditiontypeid").val() == "") {
                sweetAlert("Sorry", "The condition type is requied!", "error");
                return false;
            }
            if (jQuery("#ruleconditionvalue").val() == "") {
                sweetAlert("Sorry", "The condition value is requied!", "error");
                return false;
            }

            jQuery.ajax({
                url: "/ruleconditions/edit",
                type: "POST",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(newRuleCondition),
                success: function(result) {
                    if (result.success) {
                        if (result.success) {
                            $(".rulesconditions").html(result.ruleconditionlist);
                            $("#ruleconditiontypeid").val("").trigger("change");
                            $("#ruleconditionvalue").val("");
                            $("#id").val("");
                            $(".btneditcondition").attr("id", "btnsavecondition");
                            $("#btnsavecondition").removeClass("btneditcondition");
                            $("#btnsavecondition").val("Save new condition");
                        }
                    }
                },
                error: function(result) {
                    alert("Failed");
                }
            });

            return false;
        });
    // delete rule condition
    $(document).on("click",
        ".deletetrulescondition",
        function() {
            var id = $(this).data("id");
            var tr = $(this).closest("tr");
            var rowscount = $(this).closest("tbody").find("tr").length;
            swal({
                    title: "Delete Rule Condition?",
                    text: "Are you sure, you want to delete this Rule Condition?",
                    type: "error",
                    showCancelButton: true,
                    closeOnConfirm: false,
                    confirmButtonColor: "#2196F3",
                    showLoaderOnConfirm: true
                },
                function() {
                    $.ajax({
                        url: "/ruleconditions/Delete/",
                        data: { id: id },
                        type: "Post",
                    }).done(function(data) {
                        if (data.error) {
                            swal({
                                title: data.error,
                                confirmButtonColor: "#2196F3",
                                type: "error"
                            });
                            return false;
                        }
                        // remove selected log from the list.
                        $(tr).remove();
                        if (rowscount == 1) {
                            var html =
                                '<tr style="text-align:center;"><td colspan="3">Sorry, no rule condition added yet.</td></tr>';
                            $(".tbodyconditions").addClass("emptytr");
                            $(".tbodyconditions").prepend(html);
                            $("#cancelrulecondition").click();
                        }
                        swal({
                            title: "The selected rule condition has been deleted successfully!",
                            confirmButtonColor: "#2196F3",
                            type: "success"
                        });
                        //var count = $(".tbodyconditions").find("tr").length;
                        //if (count == 0) {
                        //    $(".andordiv").addClass("hide");
                        //}
                        //else {
                        //    $(".andordiv").removeClass("hide");
                        //}
                    });
                });
            return false;
        });
    $(document).on("change",
        "#ruleactiontypeid",
        function() {
            var value = $(this).val();
            if (value == 1) {
                if ($("#projectid").hasClass("hidden")) {
                    $("#projectid").removeClass("hidden");
                    $("#skillid").removeClass("hidden");
                    $("#projectid").select2();
                    $("#skillid").select2();
                }
                if ($("#ruleactionvalue").hasClass("hidden")) {
                    $("#ruleactionvalue").removeClass("hidden");
                    $("#ruleactionvalue").select2();
                }
                if (!$("#statusid").hasClass("hidden")) {
                    $("#statusid").select2("destroy");
                    $("#statusid").addClass("hidden");
                }
            } else if (value == 2) {
                if (!$("#projectid").hasClass("hidden")) {
                    $("#projectid").select2("destroy");
                    $("#skillid").select2("destroy");
                    $("#projectid").addClass("hidden");
                    $("#skillid").addClass("hidden");
                }
                if ($("#ruleactionvalue").hasClass("hidden")) {
                    $("#ruleactionvalue").removeClass("hidden");
                    $("#ruleactionvalue").select2();
                }
                if (!$("#statusid").hasClass("hidden")) {
                    $("#statusid").select2("destroy");
                    $("#statusid").addClass("hidden");
                }
            } else if (value == 3) {
                if (!$("#projectid").hasClass("hidden")) {
                    $("#projectid").select2("destroy");
                    $("#skillid").select2("destroy");
                    $("#projectid").addClass("hidden");
                    $("#skillid").addClass("hidden");
                }
                if (!$("#ruleactionvalue").hasClass("hidden")) {
                    $("#ruleactionvalue").select2("destroy");
                    $("#ruleactionvalue").addClass("hidden");
                }
                if ($("#statusid").hasClass("hidden")) {
                    $("#statusid").removeClass("hidden");
                    $("#statusid").select2();
                }
            }
        });
    //add rule action
    jQuery(document).on("click",
        "#btnsaveaction",
        function() {
            var newRuleAction = {
                id: 0,
                ruleid: jQuery("#ruleid").val(),
                ruleactiontypeid: jQuery("#ruleactiontypeid").val(),
                ruleactionvalue: jQuery("#ruleactionvalue").val(),
                projectid: jQuery("#projectid").val(),
                skillid: jQuery("#skillid").val(),
                statusid: jQuery("#statusid").val()
            };
            if (jQuery("#ruleactiontypeid").val() == "") {
                sweetAlert("Sorry", "Rule action type is required!", "error");
                return false;
            }
            if (jQuery("#ruleactionvalue").val() == "" && (ruleactiontypeid == 1 || ruleactiontypeid == 2)) {
                sweetAlert("Sorry", "User is required!", "error");
                return false;
            }
            if (jQuery("#projectid").val() == "" && ruleactiontypeid == 1) {
                sweetAlert("Sorry", "Project is required!", "error");
                return false;
            }
            if (jQuery("#skillid").val() == "" && ruleactiontypeid == 1) {
                sweetAlert("Sorry", "Skill is required!", "error");
                return false;
            }
            jQuery.ajax({
                url: "/ruleactions/create",
                type: "POST",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(newRuleAction),
                success: function(result) {
                    if (result.success) {
                        $(".ruleactionlist").html(result.ruleslist);
                        $("#ruleactionvalue").val("").trigger("change");
                        $("#projectid").val("").trigger("change");
                        $("#skillid").val("").trigger("change");
                    }
                },
                error: function(result) {
                    alert("Failed");
                }
            });

            return false;
        });
    //Rj
    // Edit existing Action.
    jQuery(document).on("click",
        ".eidtrulesactions",
        function() {
            var id = $(this).data("id");
            var href = "/RuleActions/edit/" + id;
            jQuery.ajax({
                url: href,
                type: "GET",
                data: "",
                success: function(result) {
                    jQuery(".ruleactoinform").html(result);
                    $("#btnsaveaction").addClass("btneditaction");
                    $("#btnsaveaction").val("Update Action");
                    $(".btneditaction").removeAttr("id");
                    $("html,body").animate({
                            scrollTop: $(".ruleactoinform").offset().top - 150
                        },
                        "slow");
                    var value = $("#ruleactiontypeid").val();
                    if (value == 1) {
                        if ($("#ruleactionvalue").hasClass("hidden")) {
                            $("#ruleactionvalue").removeClass("hidden");
                            $("#ruleactionvalue").select2();
                        }
                        if ($("#projectid").hasClass("hidden")) {
                            $("#projectid").removeClass("hidden");
                            $("#skillid").removeClass("hidden");
                            $("#projectid").select2();
                            $("#skillid").select2();
                        }
                    } else if (value == 2) {
                        if ($("#ruleactionvalue").hasClass("hidden")) {
                            $("#ruleactionvalue").removeClass("hidden");
                            $("#ruleactionvalue").select2();
                        }
                        if (!$("#projectid").hasClass("hidden")) {
                            $("#projectid").select2("destroy");
                            $("#skillid").select2("destroy");
                            $("#projectid").addClass("hidden");
                            $("#skillid").addClass("hidden");
                        }
                        if (!$("#statusid").hasClass("hidden")) {
                            $("#statusid").select2("destory");
                            $("#statusid").addClass("hidden");
                        }
                    } else if (value == 3) {
                        if (!$("#ruleactionvalue").hasClass("hidden")) {
                            $("#ruleactionvalue").select2("destroy");
                            $("#ruleactionvalue").addClass("hidden");
                        }
                        if (!$("#projectid").hasClass("hidden")) {
                            $("#projectid").select2("destroy");
                            $("#skillid").select2("destroy");
                            $("#projectid").addClass("hidden");
                            $("#skillid").addClass("hidden");
                        }
                        if ($("#statusid").hasClass("hidden")) {
                            $("#statusid").removeClass("hidden");
                            $("#statusid").select2();
                        }
                    }
                },
                error: function(result) {
                    alert("Failed");
                }
            });
            return false;
        });
    // post rule actoin edit
    jQuery(document).on("click",
        ".btneditaction",
        function() {
            var newRuleAction = {
                id: $(".ruleactionid").data("id"),
                ruleid: $(".ruleid").data("id"),
                ruleactiontypeid: jQuery("#ruleactiontypeid").val(),
                ruleactionvalue: jQuery("#ruleactionvalue").val(),
                projectid: jQuery("#projectid").val(),
                skillid: jQuery("#skillid").val(),
                statusid: jQuery("#statusid").val()
            };
            if (jQuery("#ruleactiontypeid").val() == "") {
                sweetAlert("Sorry", "Rule action type is required!", "error");
                return false;
            }
            if (jQuery("#ruleactionvalue").val() == "" && (ruleactiontypeid == 1 || ruleactiontypeid == 2)) {
                sweetAlert("Sorry", "User is required!", "error");
                return false;
            }
            if (jQuery("#projectid").val() == "" && ruleactiontypeid == 1) {
                sweetAlert("Sorry", "Project is required!", "error");
                return false;
            }
            if (jQuery("#skillid").val() == "" && ruleactiontypeid == 1) {
                sweetAlert("Sorry", "Skill is required!", "error");
                return false;
            }

            jQuery.ajax({
                url: "/ruleactions/edit",
                type: "POST",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(newRuleAction),
                success: function(result) {
                    if (result.success) {
                        if (result.success) {
                            $(".ruleactionlist").html(result.ruleactionlist);
                            $("#ruleactiontypeid").val("").trigger("change");
                            $("#projectid").val("").trigger("change");
                            $("#skillid").val("").trigger("change");
                            $("#ruleactionvalue").val("");
                            $(".btneditaction").attr("id", "btnsaveaction");
                            $("#btnsaveaction").removeClass("btneditaction");
                            $("#btnsaveaction").val("Create");
                        }
                    }
                },
                error: function(result) {
                    alert("Failed");
                }
            });

            return false;
        });
    // delete rule action
    $(document).on("click",
        ".deletetruleaction",
        function() {
            var id = $(this).data("id");
            var tr = $(this).closest("tr");
            var rowscount = $(this).closest("tbody").find("tr").length;
            swal({
                    title: "Delete Log?",
                    text: "Are you sure, you want to delete this Rule Action?",
                    type: "error",
                    showCancelButton: true,
                    closeOnConfirm: false,
                    confirmButtonColor: "#2196F3",
                    showLoaderOnConfirm: true
                },
                function() {
                    $.ajax({
                        url: "/ruleactions/Delete/",
                        data: { id: id },
                        type: "Post",
                    }).done(function(data) {
                        if (data.error) {
                            swal({
                                title: data.error,
                                confirmButtonColor: "#2196F3",
                                type: "error"
                            });
                            return false;
                        }
                        // remove selected log from the list.
                        $(tr).remove();
                        if (rowscount == 1) {
                            var html =
                                '<tr style="text-align:center;"><td colspan="3">Sorry, no rule action added yet.</td></tr>';
                            $("#licensetable tbody").prepend(html);
                        }
                        swal({
                            title: "The selected rule action has been deleted successfully!",
                            confirmButtonColor: "#2196F3",
                            type: "success"
                        });
                        var onedit = $(".btneditaction").length;
                        if (onedit != 0) {
                            $("#ruleactiontypeid").val("").trigger("change");
                            $("#ruleactionvalue").val("").trigger("change");
                            $("#projectid").val("").trigger("change");
                            $("#skillid").val("").trigger("change");
                            $(".btneditaction").attr("id", "btnsaveaction");
                            $("#btnsaveaction").removeClass("btneditaction");
                            $("#btnsaveaction").val("Create");
                        }
                    });
                });
            return false;
        });
    //end
    jQuery(document).on("click",
        "#btnsaveexception",
        function() {
            var newRuleException = {
                id: 0,
                ruleid: jQuery("#ruleid").val(),
                ruleexceptiontypeid: jQuery("#ruleexceptiontypeid").val(),
                ruleexceptionvalue: jQuery("#ruleexceptionvalue").val(),
                isrequired: $(".isrequired-exception:checked").val()
            };
            if (jQuery("#ruleexceptiontypeid").val() == "") {
                sweetAlert("Sorry", "Exception type value is required!", "error");
                return false;
            }
            if (jQuery("#ruleexceptionvalue").val() == "") {
                sweetAlert("Sorry", "Exception value is required!", "error");
                return false;
            }

            jQuery.ajax({
                url: "/ruleexceptions/create",
                type: "POST",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(newRuleException),
                success: function(result) {
                    if (result.success) {
                        $(".rulexceptionlist").html(result.exceptionlist);
                        $("#ruleexceptiontypeid").val("").trigger("change");
                        $("#ruleexceptionvalue").val("");
                        $(".tbodyexceptions").removeClass("emptytr");
                        var count = $(".tbodyexceptions").find("tr").length;
                        if (count == 0) {
                            $(".andordiv-exception").addClass("hide");
                        } else {
                            $(".andordiv-exception").removeClass("hide");
                        }
                    }
                },
                error: function(result) {
                    alert("Failed");
                }
            });

            return false;
        });
    //Edit existing exception get
    jQuery(document).on("click",
        ".eidtrulesexception",
        function() {
            var id = $(this).data("id");
            var href = "/RuleExceptions/edit/" + id;
            jQuery.ajax({
                url: href,
                type: "GET",
                data: "",
                success: function(result) {
                    jQuery(".rulexceptionform").html(result);
                    $("#btnsaveexception").addClass("btneditexception");
                    $("#btnsaveexception").val("Update Exception");
                    $(".btneditexception").removeAttr("id");
                    $("html,body").animate({
                            scrollTop: $(".rulexceptionform").offset().top - 150
                        },
                        "slow");
                    var count = $(".tbodyexceptions").find("tr").length;
                    if (count == 0 || count == 1) {
                        $(".andordiv-exception").addClass("hide");
                    } else {
                        $(".andordiv-exception").removeClass("hide");
                    }
                },
                error: function(result) {
                    alert("Failed");
                }
            });
            return false;
        });
    // post rule exception edit
    jQuery(document).on("click",
        ".btneditexception",
        function() {
            var count = $(".tbodyexceptions").find("tr").length;
            if (count == 0 || count == 1) {
                $(".andordiv-exception").addClass("hide");
            } else {
                $(".andordiv-exception").removeClass("hide");
            }
            var newRuleException = {
                id: $(".ruleExceptionid").val(),
                ruleid: $("#ruleid").val(),
                ruleexceptiontypeid: jQuery("#ruleexceptiontypeid").val(),
                ruleexceptionvalue: jQuery("#ruleexceptionvalue").val(),
                isrequired: $(".isrequired-exception:checked").val()
            };
            if (jQuery("#ruleexceptiontypeid").val() == "") {
                sweetAlert("Sorry", "Exception type value is required!", "error");
                return false;
            }
            if (jQuery("#ruleexceptionvalue").val() == "") {
                sweetAlert("Sorry", "Exception value is required!", "error");
                return false;
            }
            jQuery.ajax({
                url: "/ruleexceptions/edit",
                type: "POST",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(newRuleException),
                success: function(result) {
                    if (result.success) {
                        if (result.success) {
                            $(".rulexceptionlist").html(result.exceptionlist);
                            $("#ruleexceptiontypeid").val("").trigger("change");
                            $("#ruleexceptionvalue").val("");
                            $(".btneditexception").attr("id", "btnsaveexception");
                            $("#btnsaveexception").removeClass("btneditexception");
                            $("#btnsaveexception").val("Create");
                        }
                    }
                },
                error: function(result) {
                    alert("Failed");
                }
            });

            return false;
        });
    //delete rule exception
    $(document).on("click",
        ".deletetrulesexception",
        function() {
            var id = $(this).data("id");
            var tr = $(this).closest("tr");
            var rowscount = $(this).closest("tbody").find("tr").length;
            swal({
                    title: "Delete Log?",
                    text: "Are you sure, you want to delete this Rule Exception?",
                    type: "error",
                    showCancelButton: true,
                    closeOnConfirm: false,
                    confirmButtonColor: "#2196F3",
                    showLoaderOnConfirm: true
                },
                function() {
                    $.ajax({
                        url: "/ruleexceptions/Delete/",
                        data: { id: id },
                        type: "Post",
                    }).done(function(data) {
                        if (data.error) {
                            swal({
                                title: data.error,
                                confirmButtonColor: "#2196F3",
                                type: "error"
                            });
                            return false;
                        }
                        // remove selected log from the list.
                        $(tr).remove();
                        if (rowscount == 1) {
                            var html =
                                '<tr style="text-align:center;"><td colspan="3">Sorry, no rule exception added yet.</td></tr>';
                            $("#licensetable tbody").prepend(html);
                            $(".tbodyexceptions").addClass("emptytr");
                        }
                        swal({
                            title: "The selected rule exception has been deleted successfully!",
                            confirmButtonColor: "#2196F3",
                            type: "success"
                        });
                    });
                });
            return false;
        });
    ////// Cancal buttons for all forms
    $(document).on("click",
        "#cancelrulecondition",
        function() {
            $("#id").val("");
            $("#ruleconditiontypeid").val("").trigger("change");
            $("#ruleconditionvalue").val("");
            $(".btneditcondition").attr("id", "btnsavecondition");
            $("#btnsavecondition").removeClass("btneditcondition");
            $("#btnsavecondition").val("Save new condition");
            var count = $(".tbodyconditions").find("tr").length;
            if (count == 0 || $(".tbodyconditions").hasClass("emptytr")) {
                $(".andordiv").addClass("hide");
            } else {
                $(".andordiv").removeClass("hide");
            }
            return false;
        });
    $(document).on("click",
        "#cancelruleaction",
        function() {
            $("#ruleactiontypeid").val("").trigger("change");
            $("#ruleactionvalue").val("").trigger("change");
            $("#projectid").val("").trigger("change");
            $("#skillid").val("").trigger("change");
            $(".btneditaction").attr("id", "btnsaveaction");
            $("#btnsaveaction").removeClass("btneditaction");
            $("#btnsaveaction").val("Create");
            return false;
        });
    $(document).on("click",
        "#cancelruleexception",
        function() {
            $("#ruleexceptiontypeid").val("").trigger("change");
            $("#ruleexceptionvalue").val("");
            $(".btneditexception").attr("id", "btnsaveexception");
            $("#btnsaveexception").removeClass("btneditexception");
            $("#btnsaveexception").val("Create");
            var count = $(".tbodyexceptions").find("tr").length;
            if (count == 0 || $(".tbodyexceptions").hasClass("emptytr")) {
                $(".andordiv-exception").addClass("hide");
            } else {
                $(".andordiv-exception").removeClass("hide");
            }
            return false;
        });
    //end
});