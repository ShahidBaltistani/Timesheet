jQuery(function() {
    // apply search filter.
    jQuery(".searchclient").livefilter({ selector: ".clientcollection li" });

    // filter selected client.
    jQuery(document).on("click",
        ".client",
        function() {
            var clientid = jQuery(this).data("clientid");
            $("#activeclient").val(clientid);
            $(".tickets").click();
            var subclientid = "subclients-" + clientid.toString();
            var clientprojects = "clinet-projects-" + clientid.toString();

            jQuery(".fileredclient ul li").data("clientid", clientid);
            jQuery(".fileredclient ul li a").html(jQuery(this).html());
            jQuery(".fileredclient ul li a")
                .append(
                    '<span class="label"><i style="color: black;" class="icon-cancel-circle2 removeclientfilter"></i></span>');

            //var subclients = jQuery("#" + subclientid).find("ul").html();
            //jQuery(".subclientcollection ul").html(subclients);
            //jQuery("#subclientcategory").show();

            jQuery("#noclientsection").hide();

            //var clientprojects = jQuery("#" + subclientid).find("#"+clientprojects).html();
            //jQuery("#clientprojects-section").html(clientprojects);
            jQuery("#clientprojects-section").show();

            jQuery(".searchcontainer").hide();
            jQuery(".clientcollection").hide();
            jQuery(".fileredclient").show();
        });

    // Remove filter.
    jQuery(document).on("click",
        ".removeclientfilter",
        function() {
            jQuery(".searchcontainer").show();
            jQuery(".clientcollection").show();
            jQuery(".fileredclient").hide();
            jQuery("#subclientcategory").hide();
            jQuery("#clientprojects-section").hide();
            jQuery("#subclients").hide();
            jQuery("#noclientsection").show();
        });

    // Display Sub client projects.
    jQuery(document).on("click",
        ".subclient",
        function() {
            var subclientid = jQuery(this).data("subclientid");
            var clientprojects = "subclient-projects-" + subclientid.toString();

            jQuery("#subclientprojects-section").html(jQuery("#" + clientprojects).html());
            jQuery("#subclientprojects-section").show();
        });
});