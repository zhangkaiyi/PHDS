<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Kaoqin.aspx.cs" Inherits="QyWeixin._Kaoqin" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" lang="en">

<head>
    <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1" />
    <meta charset="utf-8" />
    <title>拼花大师企业管理系统</title>

    <meta name="description" content="Static &amp; Dynamic Tables" />
    <meta name="viewport" content="width=device-width, initial-scale=0.85, maximum-scale=0.85" />

    <!-- bootstrap & fontawesome -->
    <link rel="stylesheet" href="/assets/ace/css/bootstrap.min.css" />
    <link rel="stylesheet" href="/assets/ace/css/font-awesome.min.css" />

    <!-- page specific plugin styles -->


    <!-- text fonts -->
    <link rel="stylesheet" href="/assets/ace/css/ace-fonts.css" />

    <!-- ace styles -->
    <link rel="stylesheet" href="/assets/ace/css/ace.min.css" id="main-ace-style" />

    <!--[if lte IE 9]>
			<link rel="stylesheet" href="/assets/ace/css/ace-part2.min.css" />
		<![endif]-->
    <link rel="stylesheet" href="/assets/ace/css/ace-skins.min.css" />
    <link rel="stylesheet" href="/assets/ace/css/ace-rtl.min.css" />

    <!--[if lte IE 9]>
		  <link rel="stylesheet" href="/assets/ace/css/ace-ie.min.css" />
		<![endif]-->

    <!-- inline styles related to this page -->

    <!-- ace settings handler -->
    <script src="/assets/ace/js/ace-extra.min.js"></script>

    <!-- HTML5shiv and Respond.js for IE8 to support HTML5 elements and media queries -->

    <!--[if lte IE 8]>
		<script src="/assets/ace/js/html5shiv.min.js"></script>
		<script src="/assets/ace/js/respond.min.js"></script>
		<![endif]-->
</head>

<body class="no-skin">
    <!-- #section:basics/navbar.layout -->
    <div id="navbar" class="navbar navbar-default">
        <script type="text/javascript">
            try { ace.settings.check('navbar', 'fixed') } catch (e) { }
        </script>

        <div class="navbar-container" id="navbar-container">
            <!-- #section:basics/sidebar.mobile.toggle -->
            <button type="button" class="navbar-toggle menu-toggler pull-left" id="menu-toggler">
                <span class="sr-only">Toggle sidebar</span>

                <span class="icon-bar"></span>

                <span class="icon-bar"></span>

                <span class="icon-bar"></span>
            </button>

            <!-- /section:basics/sidebar.mobile.toggle -->
            <div class="navbar-header pull-left">
                <!-- #section:basics/navbar.layout.brand -->
                <a href="#" class="navbar-brand">
                    <small>
                        <i class="fa fa-leaf"></i>
                        拼花大师
                    </small>
                </a>

                <!-- /section:basics/navbar.layout.brand -->

                <!-- #section:basics/navbar.toggle -->

                <!-- /section:basics/navbar.toggle -->
            </div>
        </div>
        <!-- /.navbar-container -->
    </div>

    <!-- /section:basics/navbar.layout -->
    <div class="main-container" id="main-container">
        <script type="text/javascript">
            try { ace.settings.check('main-container', 'fixed') } catch (e) { }
        </script>

        <!-- #section:basics/sidebar -->
        <div id="sidebar" class="sidebar                  responsive">
            <script type="text/javascript">
                try { ace.settings.check('sidebar', 'fixed') } catch (e) { }
            </script>

            <div class="sidebar-shortcuts" id="sidebar-shortcuts">
                <div class="sidebar-shortcuts-large" id="sidebar-shortcuts-large">
                    <button class="btn btn-success">
                        <i class="ace-icon fa fa-signal"></i>
                    </button>

                    <button class="btn btn-info">
                        <i class="ace-icon fa fa-pencil"></i>
                    </button>

                    <!-- #section:basics/sidebar.layout.shortcuts -->
                    <button class="btn btn-warning">
                        <i class="ace-icon fa fa-users"></i>
                    </button>

                    <button class="btn btn-danger">
                        <i class="ace-icon fa fa-cogs"></i>
                    </button>

                    <!-- /section:basics/sidebar.layout.shortcuts -->
                </div>

                <div class="sidebar-shortcuts-mini" id="sidebar-shortcuts-mini">
                    <span class="btn btn-success"></span>

                    <span class="btn btn-info"></span>

                    <span class="btn btn-warning"></span>

                    <span class="btn btn-danger"></span>
                </div>
            </div>
            <!-- /.sidebar-shortcuts -->

            <ul class="nav nav-list">
                <li class="active">
                    <a href="index.html">
                        <i class="menu-icon fa fa-tachometer"></i>
                        <span class="menu-text">首页 </span>
                    </a>

                    <b class="arrow"></b>
                </li>
                <li class="">
                    <a href="#" class="dropdown-toggle">
                        <i class="menu-icon fa fa-desktop"></i>
                        <span class="menu-text">销售 </span>

                        <b class="arrow fa fa-angle-down"></b>
                    </a>

                    <b class="arrow"></b>

                    <ul class="submenu">
                        <li class="">
                            <a href="tables.html">
                                <i class="menu-icon fa fa-caret-right"></i>
                                销售发货
                            </a>

                            <b class="arrow"></b>
                        </li>

                        <li class="">
                            <a href="jqgrid.html">
                                <i class="menu-icon fa fa-caret-right"></i>
                                退货出库
                            </a>

                            <b class="arrow"></b>
                        </li>
                        <li class="">
                            <a href="jqgrid.html">
                                <i class="menu-icon fa fa-caret-right"></i>
                                委外加工
                            </a>

                            <b class="arrow"></b>
                        </li>
                    </ul>
                </li>
                <li class="">
                    <a href="#" class="dropdown-toggle">
                        <i class="menu-icon fa fa-list"></i>
                        <span class="menu-text">采购 </span>
                        <b class="arrow fa fa-angle-down"></b>
                    </a>
                    <b class="arrow"></b>
                    <ul class="submenu">
                        <li class="">
                            <a href="tables.html">
                                <i class="menu-icon fa fa-caret-right"></i>
                                采购入库
                            </a>

                            <b class="arrow"></b>
                        </li>

                        <li class="">
                            <a href="jqgrid.html">
                                <i class="menu-icon fa fa-caret-right"></i>
                                采购退货
                            </a>

                            <b class="arrow"></b>
                        </li>
                    </ul>
                </li>
                <li class="">
                    <a href="#" class="dropdown-toggle">
                        <i class="menu-icon fa fa-pencil-square-o"></i>
                        <span class="menu-text">生产 </span>
                        <b class="arrow fa fa-angle-down"></b>
                    </a>
                </li>
            </ul>
            <!-- /.nav-list -->

            <!-- #section:basics/sidebar.layout.minimize -->
            <div class="sidebar-toggle sidebar-collapse" id="sidebar-collapse">
                <i class="ace-icon fa fa-angle-double-left" data-icon1="ace-icon fa fa-angle-double-left" data-icon2="ace-icon fa fa-angle-double-right"></i>
            </div>

            <!-- /section:basics/sidebar.layout.minimize -->
            <script type="text/javascript">
                try { ace.settings.check('sidebar', 'collapsed') } catch (e) { }
            </script>
        </div>

        <!-- /section:basics/sidebar -->
        <div class="main-content">

            <!-- /section:basics/content.breadcrumbs -->
            <!-- /.page-content -->
            <div class="page-content">
                <div class="page-content-area">
                    <div class="page-header">
                        <h1>
                            <asp:Label ID="Label1" runat="server"></asp:Label>
                            <small>
                                <i class="ace-icon fa fa-angle-double-right"></i>
                                <asp:Label ID="Label2" runat="server"></asp:Label>考勤数据
                            </small>
                        </h1>
                    </div>
                    <form runat="server" class="form-inline">
                        <asp:GridView ID="GridView1" runat="server" CssClass="table table-striped table-bordered table-hover"></asp:GridView>
                    </form>
                    <%--<div class="table-header">
                        Results for "Latest Registered Domains"
                    </div>
                    <table id="example" class="table table-striped table-bordered table-hover" width="100%"></table>--%>
                </div>
            </div>
        </div>
        <!-- /.main-content -->

        <div class="footer">
            <div class="footer-inner">
                <!-- #section:basics/footer -->
                <div class="footer-content">
                    <span class="bigger-120">
                        <span class="blue bolder">PHDS</span>
                        Application &copy; 2015-2016
                    </span>
                </div>

                <!-- /section:basics/footer -->
            </div>
        </div>

        <a href="#" id="btn-scroll-up" class="btn-scroll-up btn btn-sm btn-inverse">
            <i class="ace-icon fa fa-angle-double-up icon-only bigger-110"></i>
        </a>
    </div>
    <!-- /.main-container -->

    <!-- basic scripts -->

    <!--[if !IE]> -->
    <script type="text/javascript">
        window.jQuery || document.write("<script src='/assets/ace/js/jquery.min.js'>" + "<" + "/script>");
    </script>

    <!-- <![endif]-->

    <!--[if IE]>
<script type="text/javascript">
 window.jQuery || document.write("<script src='/assets/ace/js/jquery1x.min.js'>"+"<"+"/script>");
</script>
<![endif]-->
    <script type="text/javascript">
        if ('ontouchstart' in document.documentElement) document.write("<script src='/assets/ace/js/jquery.mobile.custom.min.js'>" + "<" + "/script>");
    </script>
    <script src="/assets/ace/js/bootstrap.min.js"></script>

    <!-- page specific plugin scripts -->
    <script src="assets/ace/js/jquery.dataTables.min.js"></script>
    <script src="assets/ace/js/jquery.dataTables.bootstrap.js"></script>

    <!-- ace scripts -->
    <script src="/assets/ace/js/ace-elements.min.js"></script>
    <script src="/assets/ace/js/ace.min.js"></script>


    <!-- inline scripts related to this page -->
    <script type="text/javascript">
        function getUrlParam(name) {
            //构造一个含有目标参数的正则表达式对象  
            var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
            //匹配目标参数  
            var r = window.location.search.substr(1).match(reg);
            //返回参数值  
            if (r != null) return unescape(r[2]);
            return null;
        }
        jQuery(function ($) {

            <%--$.ajax({
                url: "kaoqin.aspx?action=getkaoqin&id=" + getUrlParam("id") + "&date=" + getUrlParam("date"),
                dataType: 'json',
                success: function (json) {

                    $('#example').dataTable({
                        data: json.Table,
                        ordering: false,
                        paging: false,
                        columns:
                            [
                                { data: "日", title: "日" },
                                { data: "上午", title: "上午" },
                                { data: "下午", title: "下午" },
                                { data: "晚上", title: "晚上" },
                                { data: "出勤", title: "出勤" },
                            ]
                    });
                },
                error: function (msg) {

                }
            });--%>
        })
    </script>

</body>
</html>
