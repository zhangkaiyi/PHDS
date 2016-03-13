<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="clock.aspx.cs" Inherits="QyWeixin.clockin" %>

<!DOCTYPE html>

<html>

<head>
    <style>
        select, input, label, button {
            vertical-align: middle;
        }

        .std {
            height: 40px;
        }

        input[type=text], input[type=date], input[type=time],select {
            min-width: 150px;
            max-width: 150px;
            min-height:50px;
            width:150px;
        }

        #newbtn
        {
            width:150px;
            height:50px;
        }

    </style>

    <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1" />
    <meta charset="utf-8" />
    <title>拼花大师企业管理系统</title>

    <meta name="description" content="Static &amp; Dynamic Tables" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0" />

    <!-- bootstrap & fontawesome -->
    <link rel="stylesheet" href="/ace/assets/css/bootstrap.min.css" />
    <link rel="stylesheet" href="/ace/assets/css/font-awesome.min.css" />

    <!-- page specific plugin styles -->
    <link rel="stylesheet" href="/jquery-weui-build/dist/lib/weui.css">
    <link rel="stylesheet" href="/jquery-weui-build/dist/css/jquery-weui.css">

    <!-- text fonts -->
    <link rel="stylesheet" href="/ace/assets/css/ace-fonts.css" />

    <!-- ace styles -->
    <link rel="stylesheet" href="/ace/assets/css/ace.min.css" id="main-ace-style" />

    <!--[if lte IE 9]>
			<link rel="stylesheet" href="/ace/assets/css/ace-part2.min.css" />
		<![endif]-->
    <link rel="stylesheet" href="/ace/assets/css/ace-skins.min.css" />
    <link rel="stylesheet" href="/ace/assets/css/ace-rtl.min.css" />

    <!--[if lte IE 9]>
		  <link rel="stylesheet" href="/ace/assets/css/ace-ie.min.css" />
		<![endif]-->

    <!-- inline styles related to this page -->

    <!-- ace settings handler -->
    <script src="/ace/assets/js/ace-extra.min.js"></script>

    <!-- HTML5shiv and Respond.js for IE8 to support HTML5 elements and media queries -->

    <!--[if lte IE 8]>
		<script src="/ace/assets/js/html5shiv.min.js"></script>
		<script src="/ace/assets/js/respond.min.js"></script>
		<![endif]-->
</head>

<body class="no-skin">
    <!-- #section:basics/navbar.layout -->
    <div id="navbar" class="navbar navbar-default">


        <div class="navbar-container" id="navbar-container">

            <div class="navbar-header pull-left">
                <!-- #section:basics/navbar.layout.brand -->
                <a class="navbar-brand">
                    <small>
                        <i class="fa fa-leaf"></i>
                        拼花大师
                    </small>
                </a>

                <!-- /section:basics/navbar.layout.brand -->

            </div>
        </div>
        <!-- /.navbar-container -->
    </div>

    <!-- /section:basics/navbar.layout -->
    <div class="main-container" id="main-container">

        <div class="main-content">

            <!-- /section:basics/content.breadcrumbs -->
            <!-- /.page-content -->
            <div class="page-content">
                <div class="page-content-area">
                    <div class="page-header">
                        <h1>

                            <small>
                                <i class="ace-icon fa fa-angle-double-right"></i>
                                手动打卡
                            </small>
                        </h1>
                    </div>
                    <div id="querygroup">
                        <div style="margin-bottom: 10px;">
                            <input id="datepicker" class="std" type="date" value="" />
                            <input id="timepicker" class="std" type="time" value="07:30" />
                        </div>
                        <div style="margin-bottom: 10px;">
                            <select id="stafflist" class="std" disabled="disabled"></select>
                        </div>
                        <%--<div style="margin-bottom: 10px;">
                            <asp:TextBox ID="TimePicker1" runat="server" Style="height: 40px"></asp:TextBox>
                            <asp:Button ID="btn1" runat="server" CssClass="btn btn-success" Text="1" OnClick="btn1_Click" />
                            <asp:Button ID="btn2" runat="server" CssClass="btn btn-success" Text="2" OnClick="btn2_Click" />
                            <asp:Button ID="btn3" runat="server" CssClass="btn btn-success" Text="3" OnClick="btn3_Click" />
                            <asp:Button ID="btn4" runat="server" CssClass="btn btn-success" Text="4" OnClick="btn4_Click" />
                        </div>--%>
                        <%--<div>
                            <asp:Button ID="btnOK" runat="server" CssClass="btn btn-success width-100" Text="打卡" OnClick="btnOK_Click"  OnClientClick="return confirm('确认添加这条记录吗？');" />
                        </div>--%>
                        <div style="margin-bottom: 10px;">
                            <button id="newbtn"></button>
                        </div>
                        <div style="margin-bottom: 10px;">
                            <table id="newtable" class="table table-striped table-bordered table-hover"></table>
                        </div>
                        <%--<div>
                            <asp:GridView ID="GridView1" runat="server" CssClass="table table-striped table-bordered table-hover"></asp:GridView>
                        </div>--%>
                    </div>
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
        window.jQuery || document.write("<script src='/ace/assets/js/jquery.min.js'>" + "<" + "/script>");
    </script>

    <!-- <![endif]-->

    <!--[if IE]>
<script type="text/javascript">
 window.jQuery || document.write("<script src='/ace/assets/js/jquery1x.min.js'>"+"<"+"/script>");
</script>
<![endif]-->
    <script type="text/javascript">
        if ('ontouchstart' in document.documentElement) document.write("<script src='/ace/assets/js/jquery.mobile.custom.min.js'>" + "<" + "/script>");
    </script>
    <script src="/ace/assets/js/bootstrap.min.js"></script>

    <!-- page specific plugin scripts -->
    <script src="/jquery-weui-build/dist/js/jquery-weui.js"></script>

    <!-- ace scripts -->
    <script src="/ace/assets/js/ace-elements.min.js"></script>
    <script src="/ace/assets/js/ace.min.js"></script>

    <script type="text/javascript">

        function getDropdownList() {
            var select_id = "#stafflist";
            var x = $(select_id).val();
            $(select_id).attr({ "disabled": "disabled" });
            
            $.ajax({
                url: "clock.aspx?action=getstafflist&date="+datepicker.value,    //后台webservice里的方法名称  
                type: "get",
                dataType: 'json',
                success: function (data) {
                    var optionstring = "";
                    for (var i in data) {
                        var jsonObj = data[i];
                        optionstring += "<option value=\"" + jsonObj.人员编号 + "\" >" + jsonObj.姓名 + "</option>";
                    }
                    $(select_id).removeAttr('disabled');
                    $(select_id).html("<option value='请选择'>请选择...</option> " + optionstring);
                    if (x == null) {
                        $(select_id).val("请选择");
                    }
                    else {
                        $(select_id).val(x);
                    }
                    getTimeRecords();

                },
                error: function (msg) {
                    // 失败就禁用下拉列表
                    $(select_id).attr({ "disabled": "disabled" });
                }
            });
        };

        function getTimeRecords() {
            var select_id = "#stafflist";

            $.ajax({
                url: "clock.aspx?action=gettimerecords&id=" + $(select_id).val() + "&date=" + datepicker.value,
                type: "get",
                dataType: 'json',
                success: function (data) {
                    var thead = "<thead><tr><th>卡号</th><th>打卡时间</th></tr></thead>";
                    var tbody = "<tbody>";
                    for (var i in data) {
                        var jsonObj = data[i];
                        tbody += "<tr><td>" + jsonObj.卡号 + "</td><td>" + jsonObj.打卡时间 + "</td></tr>"
                    }
                    tbody += "</tbody>";
                    //alert(thead+tbody);
                    $("#newtable").html(thead + tbody);
                },
                error: function (msg) {
                    alert("error");
                }
            });
        };
    </script>
    <!-- inline scripts related to this page -->
    <script type="text/javascript">

        jQuery(function ($) {

            $("#datepicker").blur(getDropdownList);
            $("#stafflist").change(getTimeRecords);
            $("#newbtn").click(function () {
                $.confirm("确定录入这条考勤记录吗？", function () {
                    //点击确认后的回调函数
                }, function () {
                    //点击取消后的回调函数
                });
            });

        })
    </script>

    <!-- the following scripts are used in demo only for onpage help and you don't need them -->

</body>
</html>
