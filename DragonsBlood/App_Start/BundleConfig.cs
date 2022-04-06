using System.Web.Optimization;

namespace DragonsBlood
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.signalR-{version}.js",
                        "~/Scripts/jquery.unobtrusive-ajax.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/bootstrap-wysiwyg.js",
                      "~/Scripts/tinymce/tinymce.js",
                      "~/Scripts/tinymce/themes/modern/theme.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/Themes/Default").Include(
                    "~/Content/Themes/bootstrap.dragon.css"));

            bundles.Add(new ScriptBundle("~/Scripts/Chat").Include(
                "~/Scripts/chat.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Color").Include(
                "~/Scripts/jquery-ui-colorpicker.js",
                "~/Scripts/color.js",
                "~/Scripts/feedback.js"));
        }
    }
}
