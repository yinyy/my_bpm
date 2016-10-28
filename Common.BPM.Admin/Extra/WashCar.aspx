<%@ Page Language="C#" Debug="true" AutoEventWireup="true" CodeBehind="WashCar.aspx.cs" Inherits="BPM.Admin.Extra.WashCar"%>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta name="viewport" content="width=device-width,initial-scale=1,user-scalable=no">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>人保客户洗车</title>
    <link href="../PublicPlatform/Web/css/WeUI/style/weui.css" rel="stylesheet" />
    <link href="../PublicPlatform/Web/css/style.css" rel="stylesheet" />
    <script src="../PublicPlatform/Web/js/jquery-2_2_1_min.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <%if (validateMessage=="wechat_error")
         {%>
        <div class="weui_msg">
            <div class="weui_icon_area"><i class="weui_icon_warn weui_icon_msg"></i></div>
            <div class="weui_text_area">
                <h2 class="weui_msg_title">用户身份验证失败。</h2>
            </div>
        </div>
        <%}
        else if(validateMessage=="configuration_error"){ %>
        <div class="weui_msg">
            <div class="weui_icon_area"><i class="weui_icon_warn weui_icon_msg"></i></div>
            <div class="weui_text_area">
                <h2 class="weui_msg_title">参数错误。</h2>
            </div>
        </div>      
        <%} else if(validateMessage=="user_not_exist"){ %>
        <div class="weui_msg">
            <div class="weui_icon_area"><i class="weui_icon_warn weui_icon_msg"></i></div>
            <div class="weui_text_area">
                <h2 class="weui_msg_title">不是合法的微信用户。</h2>
            </div>
        </div>      
        <%} else if(validateMessage=="empty_card"){ %>
        <div class="weui_msg">
            <div class="weui_icon_area"><i class="weui_icon_warn weui_icon_msg"></i></div>
            <div class="weui_text_area">
                <h2 class="weui_msg_title">洗车卡已过期。</h2>
            </div>
        </div>      
        <%} else if(validateMessage=="device_error"){ %>
        <div class="weui_msg">
            <div class="weui_icon_area"><i class="weui_icon_warn weui_icon_msg"></i></div>
            <div class="weui_text_area">
                <h2 class="weui_msg_title">未查询到指定的洗车机。</h2>
            </div>
        </div>      
        <%}else if(validateMessage=="success"){ %>
        <div class="weui_msg">
            <div class="weui_icon_area"><i class="weui_icon_success weui_icon_msg"></i></div>
            <div class="weui_text_area">
                <h2 class="weui_msg_title">洗车机开始工作。</h2>
            </div>
        </div>      
        <%} else { %>
        <div class="weui_msg">
            <div class="weui_icon_area"><i class="weui_icon_warn weui_icon_msg"></i></div>
            <div class="weui_text_area">
                <h2 class="weui_msg_title">未知错误。</h2>
                <p class="weui_msg_desc"><%=validateMessage %></p>
            </div>
        </div>      
        <%} %>    
    </form>
</body>
</html>
