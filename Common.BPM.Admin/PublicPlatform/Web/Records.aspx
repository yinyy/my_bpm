<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Records.aspx.cs" Inherits="BPM.Admin.PublicPlatform.Web.Records" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width,initial-scale=1,user-scalable=no">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <link href="./css/WeUI/style/weui.min.css" rel="stylesheet" />
    <link href="./css/Records.css?v=0.0.2" rel="stylesheet"/>
    <script src="./js/jquery-2_2_1_min.js"></script>
    <script src="./js/Records.js?t=<%=DateTime.Now %>"></script>
    <title>洗车记录</title>
</head>
<body>
    <form id="form1" runat="server">
        <div class="weui_panel weui_panel_access">
            <div class="weui_panel_hd">洗车记录</div>
            <div class="weui_panel_bd">
                    
            </div>
            <a href="javascript:void(0);" class="weui_panel_ft">查看更多</a>
        </div>
        <br />
    </form>
</body>
</html>
