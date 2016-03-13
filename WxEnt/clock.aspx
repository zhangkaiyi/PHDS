<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="clock.aspx.cs" Inherits="QyWeixin.KaoqinDaKa" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <style>
        select, input, label, button {
            vertical-align: middle;
        }
    </style>

    <meta charset="utf-8" />
    <title>拼花大师企业管理系统</title>

    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0" />

    <link rel="stylesheet" href="Assets/jquery-weui/dist/lib/weui.css">
    <link rel="stylesheet" href="Assets/jquery-weui/dist/css/jquery-weui.css">
    <link rel="stylesheet" href="Assets/datatables/css/jquery.dataTables.min.css">
</head>

<body ontouchstart="">
    <!-- #section:basics/navbar.layout -->
    <div id="navbar" class="navbar navbar-default">


        <div class="navbar-container" id="navbar-container">
        </div>
        <!-- /.navbar-container -->
    </div>

    <!-- /section:basics/navbar.layout -->
    <div class="main-container" id="main-container">

        <div class="main-content">

            <div id="querygroup">
                <h2 style="padding: 15px">手动考勤</h2>
                <div class="weui_cells weui_cells_form">
                    <div class="weui_cell">
                        <div class="weui_cell_hd">
                            <label for="" class="weui_label">日期</label>
                        </div>
                        <div class="weui_cell_bd weui_cell_primary">
                            <input class="weui_input" type="date" value="" id="datepicker">
                        </div>
                    </div>
                    <div class="weui_cell">
                        <div class="weui_cell_hd">
                            <label for="" class="weui_label">时间</label>
                        </div>
                        <div class="weui_cell_bd weui_cell_primary">
                            <input class="weui_input" type="time" value="" id="timepicker">
                        </div>
                    </div>
                    <div class="weui_cell">
                        <div class="weui_cell_hd">
                            <label for="" class="weui_label">人员</label>
                        </div>
                        <div class="weui_cell_bd weui_cell_primary">
                            <select class="weui_input" name="select1" id="stafflist">
                            </select>
                        </div>
                    </div>
                    <div class="weui_cell"><span id="newbtn" href="javascript:;" class="weui_btn weui_btn_primary" style="width: 100%">添加记录</span></div>
                    <div class="weui_cell"><span id="delbtn" href="javascript:;" class="weui_btn weui_btn_primary" style="width: 100%">删除记录</span></div>

                </div>
                <div id="kaoqin_table1"></div>
                <div id="kaoqin_table2">
                    <table id="example" class="display" cellspacing="0" width="100%">
                    </table>
                </div>
            </div>

        </div>

    </div>

    <script src="Assets/jquery-weui/dist/lib/jquery-2.1.4.js"></script>
    <script src="Assets/jquery-weui/dist/js/jquery-weui.js"></script>
    <script src="Assets/datatables/js/jquery.dataTables.min.js"></script>

    <script type="text/javascript">
        var datepicker_id = "#datepicker";
        var timepicker_id = "#timepicker";
        var select_id = "#stafflist";

        function fillDropdownList() {
            var x = $(select_id).val();
            $(select_id).attr({ "disabled": "disabled" });

            $.ajax({
                url: "clock.aspx?action=getstafflist&date=" + datepicker.value,    //后台webservice里的方法名称  
                type: "get",
                dataType: 'json',
                success: function (data) {
                    var optionstring = "<option value='请选择'>请选择...</option> ";
                    for (var i in data) {
                        var jsonObj = data[i];
                        optionstring += "<option value=\"" + jsonObj.人员编号 + "\" >" + jsonObj.姓名 + "</option>";
                    }
                    $(select_id).removeAttr('disabled');
                    $(select_id).html("" + optionstring);
                    if (x == null) {
                        $(select_id).val("请选择");
                    }
                    else {
                        $(select_id).val(x);
                    }
                    fillTimeRecords();

                },
                error: function (msg) {
                    // 失败就禁用下拉列表
                    $(select_id).attr({ "disabled": "disabled" });
                }
            });
        };

        function fillTimeRecords() {

            $.ajax({
                url: "clock.aspx?action=gettimerecords&id=" + $(select_id).val() + "&date=" + datepicker.value,
                type: "get",
                dataType: 'json',
                success: function (data) {
                    var thead = "<div class='weui_cells'><div class='weui_cell'><div class='weui_cell_bd weui_cell_primary'><p>卡号或类型</p></div><div class='weui_cell_ft'>打卡时间</div></div>";
                    var tbody = "";
                    for (var i in data) {
                        var jsonObj = data[i];
                        tbody += "<div class='weui_cell'><div class='weui_cell_bd weui_cell_primary'><p>" + jsonObj.卡号 + "</p></div><div class='weui_cell_ft'>" + jsonObj.打卡时间 + "</div></div>"
                    }
                    if (tbody == "") {
                        $("#kaoqin_table1").html(tbody);
                    } else {
                        $("#kaoqin_table1").html(thead + tbody + "</div>");
                    }
                },
                error: function (msg) {
                    alert("error");
                }
            });
        };

        function addTimeRecord() {

            if ($(datepicker_id).val() == "" || $(timepicker_id).val() == "" || $(select_id).val() == "" || $(select_id).val() == null) {
                $.toast("资料不完整");
                return;
            }

            $.confirm("确定录入这条考勤记录吗？", function () {

                $.ajax({
                    url: "clock.aspx?action=addnewrecord&id=" + $(select_id).val() + "&date=" + datepicker.value + "&time=" + timepicker.value,
                    type: "get",
                    dataType: 'json',
                    success: function (data) {

                        if (data.errmsg == "ok") {
                            $.toast("操作成功");
                        }
                        else {
                            $.toast("fail");
                        }
                        fillTimeRecords();
                    },
                    error: function (msg) {

                        $.toast("Ajax Fail");
                        console.log("fail");
                    }
                });
            }, function () {
                //点击取消后的回调函数
            });
        }

        function delTimeRecord() {

            $.confirm("确定删除吗？", function () {
                $.ajax({
                    url: "clock.aspx?action=deletenullrcid&id=" + $(select_id).val() + "&date=" + datepicker.value + "&time=" + timepicker.value,
                    type: "get",
                    dataType: 'json',
                    success: function (data) {

                        if (data.ErrorCode == 0) {

                            $.toast("删除了 " + $.parseJSON(data.Json).Count + " 条记录");
                            fillTimeRecords();
                        }
                        else {
                            $.toast("code:" + data.ErrorServer);

                        }
                    },
                    error: function (msg) {

                        $.toast("Ajax Fail");
                        console.log("fail");
                    }
                });
            }, function () {
                //点击取消后的回调函数
            });
        }
    </script>

    <script type="text/javascript">


        jQuery(function ($) {

            <%--$.ajax({
                url: "clock.aspx?action=getstafflist&date=2015-11-07",
                type: "get",
                dataType: 'json',
                success: function (json) {

                    $('#example').dataTable({
                        //ajax:json,
                        data: json,
                        columns:
                            [
                                { data: "人员编号", title: "abc" },
                                { data: "姓名" },
                            ]
                    });
                },
                error: function (msg) {
                    alert("table fail");
                }
            });--%>

            <%--$('#example').dataTable({
                ajax:
                    {
                        url: "clock.aspx",
                        dataSrc: "",
                        data: { "action": "getstafflist", "date": "2015-11-07" }
                    },
                columns: [
                        { data: "人员编号", title: "abc" },
                        { data: "姓名" },
                ]
            });--%>


            $('#datepicker').bind('keypress', function (event) {

                if (event.keyCode == "13") {

                    $('#datepicker').blur();
                }
            }); // 回车触发 blur

            $("#datepicker").blur(fillDropdownList);

            $("#stafflist").change(fillTimeRecords);

            $("#newbtn").click(addTimeRecord);

            $("#delbtn").click(delTimeRecord);

        })
    </script>

</body>


</html>
