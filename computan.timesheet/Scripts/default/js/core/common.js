
$(function() {
    var call = $(".ajaxnotcall").val();
    if (call != 1) {
        $(document).ajaxStart(function() {
            $("#spinner").show();
        });
        $(document).ajaxError(function() {
            $("#spinner").hide();
        });
        $(document).ajaxComplete(function() {
            $("#spinner").hide();
        });
    }

});