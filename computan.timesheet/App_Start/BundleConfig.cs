using System.Web.Optimization;

namespace computan.timesheet
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-{version}.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/jquery.validate*"));

            //// Use the development version of Modernizr to develop with and learn from. Then, when you're
            //// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/Scripts/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          "~/Scripts/bootstrap.js",
            //          "~/Scripts/respond.js"));

            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //          "~/Content/bootstrap.css",
            //          "~/Content/site.min.css"));

            //
            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/default/css/icons/icomoon/styles.min.css",
                "~/Content/default/css/bootstrap.min.css",
                "~/Content/default/css/core.min.css",
                "~/Content/default/css/components.min.css",
                "~/Content/default/css/colors.min.css",
                "~/Content/default/css/style-admin.min.css",
                "~/Content/default/css/ClientDashboard.min.css"
            ));

            //bundles.Add(new StyleBundle("~/Content/pagingcss").Include(
            //    "~/Content/default/pagedlist.css"
            //));

            bundles.Add(new StyleBundle("~/Scripts/plugins/finecss").Include(
                "~/Scripts/default/plugins/jquery.fine-uploader/fine-uploader-new.min.css"
            ));

            bundles.Add(new StyleBundle("~/Scripts/plugins/colorpickercss").Include(
                "~/Scripts/default/plugins/bootstrap-colorpicker-plus/css/bootstrap-colorpicker.min.css",
                "~/Scripts/default/plugins/bootstrap-colorpicker-plus/css/bootstrap-colorpicker-plus.min.css",
                "~/Scripts/default/plugins/bootstrap-colorpicker-plus/css/bootstrap-colorpicker-custom.css"
            ));

            // ====================================================
            // Client Style Bundles
            // ====================================================
            bundles.Add(new StyleBundle("~/Content/simisue").Include(
                "~/Content/default/css/font-awesome.css",
                "~/Content/default/bootstrap.min.css",
                "~/Content/default/css/jquery.smartmenus.bootstrap.css",
                "~/Content/default/css/jquery.simpleLens.css",
                "~/Content/default/css/slick.css",
                "~/Content/default/css/theme-color/default-theme.css",
                "~/Content/default/css/sequence-theme.modern-slide-in.css",
                "~/Content/default/css/style-megamenu.css",
                "~/Content/default/css/ionicons.min.css",
                "~/Content/default/css/components.min.css",
                "~/Content/default/css/style.css",
                "~/Content/default/css/nouislider.css"
            ));

            bundles.Add(new StyleBundle("~/Scripts/plugins/simplecolorpickercss").Include(
                "~/Scripts/default/plugins/colorpicker/simplecolorpicker.css",
                "~/Scripts/default/plugins/colorpicker/simplecolorpicker-regularfont.css"
            ));

            bundles.Add(new StyleBundle("~/Scripts/plugins/select2optioncss").Include(
                "~/Scripts/default/plugins/select2option/select2option.css"
            ));

            // ====================================================
            // Script Bundles
            // ====================================================
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-2.2.4.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate.min.js",
                "~/Scripts/jquery.validate.unobtrusive.min.js"
            ));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //    "~/Scripts/default/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/default/js/core/libraries/bootstrap.min.js",
                "~/Scripts/default/respond.js"));

            bundles.Add(new ScriptBundle("~/Scripts/corejs").Include(
                "~/Scripts/default/js/plugins/loaders/pace.min.js",
                "~/Scripts/default/js/plugins/loaders/blockui.min.js",
                "~/Scripts/default/js/plugins/notifications/pnotify.min.js",
                "~/Scripts/default/js/plugins/notifications/bootbox.min.js",
                "~/Scripts/default/js/plugins/notifications/sweet_alert.min.js",
                "~/Scripts/default/js/core/app.min.js",
                "~/Scripts/default/js/core/common.min.js",
                "~/Scripts/default/js/pages/components_notifications_pnotify.min.js",
                "~/Scripts/default/js/pages/components_modals.min.js",
                "~/Scripts/default/js/plugins/ui/moment/moment.min.js",
                "~/Scripts/default/js/plugins/pickers/daterangepicker.min.js",
                "~/Scripts/default/js/plugins/pickers/anytime.min.js",
                "~/Scripts/default/datepicker/jquery.datepick.min.js",
                "~/Scripts/handlebars-v4.0.5.min.js"
            ));

            bundles.Add(new ScriptBundle("~/Scripts/loginjs").Include(
                "~/Scripts/default/js/plugins/forms/styling/uniform.min.js",
                "~/Scripts/default/js/pages/login.min.js"
            ));

            bundles.Add(new ScriptBundle("~/Scripts/dashboardjs").Include(
                "~/Scripts/default/js/plugins/visualization/d3/d3.min.js",
                "~/Scripts/default/js/plugins/visualization/d3/d3_tooltip.js",
                "~/Scripts/default/js/plugins/forms/styling/switchery.min.js",
                "~/Scripts/default/js/plugins/forms/styling/uniform.min.js",
                "~/Scripts/default/js/plugins/forms/selects/bootstrap_multiselect.js",
                "~/Scripts/default/js/plugins/ui/moment/moment.min.js",
                "~/Scripts/default/js/plugins/pickers/daterangepicker.min.js",
                "~/Scripts/default/js/pages/dashboard.js"
            ));

            bundles.Add(new ScriptBundle("~/Scripts/select2").Include(
                "~/Scripts/default/js/plugins/forms/selects/select2.min.js"
            ));

            bundles.Add(new ScriptBundle("~/Scripts/checkboxes_radios").Include(
                "~/Scripts/default/js/pages/form_checkboxes_radios.js"
            ));

            bundles.Add(new ScriptBundle("~/Scripts/datatables").Include(
                "~/Scripts/default/js/plugins/tables/datatables/datatables.min.js",
                "~/Scripts/default/js/plugins/forms/selects/select2.min.js",
                "~/Scripts/default/js/plugins/tables/datatables/extensions/buttons.min.js",
                "~/Scripts/default/js/plugins/tables/datatables/extensions/jszip/jszip.min.js",
                "~/Scripts/default/js/plugins/tables/datatables/extensions/pdfmake/pdfmake.min.js",
                //"~/Scripts/default/js/plugins/tables/datatables/extensions/pdfmake/vfs_fonts.min.js",
                "~/Scripts/default/js/pages/datatables_extension_buttons_html5.min.js"
            ));

            bundles.Add(new ScriptBundle("~/Scripts/ListingLayout").Include(
                "~/Scripts/default/js/plugins/tables/datatables/datatables.min.js",
                "~/Scripts/default/js/plugins/forms/selects/select2.min.js"
            ));

            bundles.Add(new ScriptBundle("~/Scripts/MasterLayout").Include(
                "~/Scripts/default/js/plugins/loaders/pace.min.js",
                "~/Scripts/default/js/core/libraries/jquery.min.js",
                "~/Scripts/default/js/core/libraries/bootstrap.min.js",
                "~/Scripts/default/js/plugins/loaders/blockui.min.js",
                "~/Scripts/default/js/plugins/visualization/d3/d3.min.js",
                "~/Scripts/default/js/plugins/visualization/d3/d3_tooltip.js",
                "~/Scripts/default/js/plugins/forms/styling/switchery.min.js",
                "~/Scripts/default/js/plugins/forms/styling/uniform.min.js",
                "~/Scripts/default/js/plugins/forms/selects/bootstrap_multiselect.js",
                "~/Scripts/default/js/plugins/ui/moment/moment.min.js",
                "~/Scripts/default/js/plugins/pickers/daterangepicker.min.js",
                "~/Scripts/default/js/core/app.min.js",
                "~/Scripts/default/js/pages/form_checkboxes_radios.js"
            /*"~/Scripts/js/pages/dashboard.js",*/
            ));

            bundles.Add(new ScriptBundle("~/Scripts/plugins/finejs").Include(
                "~/Scripts/plugins/jquery.fine-uploader/jquery.fine-uploader.min.js"
            ));

            bundles.Add(new ScriptBundle("~/Scripts/plugins/colorpicker").Include(
                "~/Scripts/default/plugins/bootstrap-colorpicker-plus/js/bootstrap-colorpicker.min.js",
                "~/Scripts/default/plugins/bootstrap-colorpicker-plus/js/bootstrap-colorpicker-plus.min.js"
            ));

            bundles.Add(new ScriptBundle("~/Scripts/jsclient").Include(
                "~/Scripts/default/js/jquery.smartmenus.min.js",
                "~/Scripts/default/js/jquery.smartmenus.bootstrap.min.js",
                "~/Scripts/default/js/jquery.simpleGallery.min.js",
                "~/Scripts/default/js/jquery.simpleLens.min.js",
                "~/Scripts/default/js/slick.min.js",
                "~/Scripts/default/js/nouislider.min.js",
                "~/Scripts/default/js/custom.min.js",
                "~/Scripts/default/js/classie.min.js",
                "~/Scripts/default/js/megamenu.min.js",
                "~/Scripts/default/handlebars-v{version}.js"
            ));

            bundles.Add(new ScriptBundle("~/Scripts/sequencejs").Include(
                "~/Scripts/js/sequence.js",
                "~/Scripts/js/sequence-theme.modern-slide-in.js"
            ));

            bundles.Add(new ScriptBundle("~/Scripts/plugins/simplecolorpickerjs").Include(
                "~/Scripts/default/plugins/colorpicker/simplecolorpicker.js"
            ));
            bundles.Add(new ScriptBundle("~/Scripts/plugins/select2optionjs").Include(
                "~/Scripts/default/plugins/select2option/select2option.js"
            ));
            bundles.Add(new ScriptBundle("~/Scripts/ckeditor4").Include(
                "~/Scripts/default/ckeditor/4.16.0/ckeditor.js"
            ));
            bundles.Add(new ScriptBundle("~/Scripts/plugins/livefilter").Include(
                "~/Scripts/plugins/jquery.livefilter.min.js"
            ));
            bundles.Add(new ScriptBundle("~/Scripts/custom/clientmanager").Include(
                "~/Scripts/custom/client.min.js"
            ));
            bundles.Add(new ScriptBundle("~/Scripts/custom/tagsinput").Include(
                "~/Scripts/default/js/plugins/forms/tags/tagsinput.min.js",
                "~/Scripts/default/js/plugins/forms/tags/tokenfield.min.js",
                "~/Scripts/default/js/plugins/ui/prism.min.js",
                "~/Scripts/default/js/plugins/forms/inputs/typeahead/typeahead.bundle.min.js"
            ));

            BundleTable.EnableOptimizations = false;
        }
    }
}