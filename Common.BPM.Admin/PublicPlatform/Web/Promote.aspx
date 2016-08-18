<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Promote.aspx.cs" Inherits="BPM.Admin.PublicPlatform.Web.Promote" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width,initial-scale=1,user-scalable=no">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <link href="./css/WeUI/style/weui.min.css" rel="stylesheet" />
    <link href="css/style.css" rel="stylesheet" />
    <link href="./css/Promote.css?d=<%=DateTime.Now %>" rel="stylesheet" />
    <script src="./js/jquery-2_2_1_min.js"></script>
    <title>推广活动</title>
</head>

<body>
    <form id="uiform" runat="server">
        <div><p>&nbsp;</p></div>
        <div id="code_div" runat="server">
            <p>请让好友扫一扫下面的二维码，或者，截图发送给好友。好友关注后，您将得到积分奖励！</p>
            <asp:Image ID="CodeImage" runat="server" Width="200" Height="200" />
            <p>此二维码的有效期为7天。</p>
        </div>
        <div id="nocode_div" runat="server" visible="false">
            <p>创建推广二维码错误！</p>
        </div>
    </form>
</body>
</html>
