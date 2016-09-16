<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="BPM.Admin.PublicPlatform.Web.ChangePassword" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta name="viewport" content="width=device-width,initial-scale=1,user-scalable=no">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>重置密码</title>
    <link href="css/WeUI/style/weui.css" rel="stylesheet" />
    <link href="css/style.css" rel="stylesheet" />
    <link href="css/Card2.css" rel="stylesheet" />
    <script src="js/jquery-2_2_1_min.js"></script>
    <script src="js/common.js"></script>
    <script src="js/ChangePassword.js?d=<%=DateTime.Now.Ticks %>"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <div class="weui_cells_title">重置密码</div>
            <div class="weui_cells weui_cells_form">
                <div class="weui_cell">
                    <div class="weui_cell_bd weui_cell_primary">
                        <input id="Password" class="weui_input" type="password" pattern="[0-9]*" placeholder="输入新密码"/>
                    </div>
                </div>

                <div class="weui_cell">
                    <div class="weui_cell_bd weui_cell_primary">
                        <input id="RePassword" class="weui_input" type="password" pattern="[0-9]*" placeholder="再次输入新密码"/>
                    </div>
                </div>            
            </div>

            <div class="weui_btn_area">
                <a class="weui_btn weui_btn_primary" href="javascript:">确定</a>
            </div>
        </div>
    </form>
</body>
</html>