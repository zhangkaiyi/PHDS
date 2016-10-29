using System.Web;
using System.Web.Optimization;

namespace PHDS.Web
{
    public class BundleConfig
    {
        // 有关绑定的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jQueryTools/jquery.PrintArea.js",
                        "~/Scripts/jQueryTools/printThis.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // 使用要用于开发和学习的 Modernizr 的开发版本。然后，当你做好
            // 生产准备时，请使用 http://modernizr.com 上的生成工具来仅选择所需的测试。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/bootstrap").Include("~/Content/bootstrap.css"));

            bundles.Add(new StyleBundle("~/Content/Mvc4/css").Include("~/Content/Mvc4/site.css"));

            bundles.Add(new StyleBundle("~/Content/Mvc4/themes/base/css").Include(
                        "~/Content/Mvc4/themes/base/jquery.ui.core.css",
                        "~/Content/Mvc4/themes/base/jquery.ui.resizable.css",
                        "~/Content/Mvc4/themes/base/jquery.ui.selectable.css",
                        "~/Content/Mvc4/themes/base/jquery.ui.accordion.css",
                        "~/Content/Mvc4/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/Mvc4/themes/base/jquery.ui.button.css",
                        "~/Content/Mvc4/themes/base/jquery.ui.dialog.css",
                        "~/Content/Mvc4/themes/base/jquery.ui.slider.css",
                        "~/Content/Mvc4/themes/base/jquery.ui.tabs.css",
                        "~/Content/Mvc4/themes/base/jquery.ui.datepicker.css",
                        "~/Content/Mvc4/themes/base/jquery.ui.progressbar.css",
                        "~/Content/Mvc4/themes/base/jquery.ui.theme.css"));

            bundles.Add(new StyleBundle("~/Assets/light7/dist/css/include").Include("~/Assets/light7/dist/css/light7.css"));
            bundles.Add(new ScriptBundle("~/Assets/light7/dist/js/include").Include("~/Assets/light7/dist/js/light7.js",
                "~/Assets/light7/dist/js/i18n/cn.js"));

            bundles.Add(new StyleBundle("~/Content/AdminLTE/dist/css/include").Include("~/Content/AdminLTE/dist/css/AdminLTE.css",
                "~/Content/AdminLTE/dist/css/skins/skin-blue.min.css"));
            bundles.Add(new ScriptBundle("~/Content/AdminLTE/dist/js/include").Include("~/Content/AdminLTE/dist/js/app.js"));
            bundles.Add(new ScriptBundle("~/Content/AdminLTE/plugins/chartjs/include").Include("~/Content/AdminLTE/plugins/chartjs/Chart.js"));

            bundles.Add(new StyleBundle("~/Content/DataTables/css/datatables").Include(
                //"~/Content/DataTables/css/jquery.dataTables.css",
                "~/Content/DataTables/css/dataTables.bootstrap.css"
                ));
            bundles.Add(new ScriptBundle("~/Scripts/DataTables/datatables").Include(
                "~/Scripts/DataTables/jquery.dataTables.js",
                "~/Scripts/DataTables/dataTables.bootstrap.js",
                "~/Scripts/DataTables/dataTables.constant.js"
                ));
        }
    }
}
