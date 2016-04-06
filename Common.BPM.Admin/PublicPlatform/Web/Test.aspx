<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Test.aspx.cs" Inherits="BPM.Admin.PublicPlatform.Web.Test" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width,initial-scale=1,user-scalable=no">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <link href="css/WeUI/style/weui.min.css" rel="stylesheet" />
    <link href="css/card.css?d=<%=DateTime.Now %>" rel="stylesheet" />
    <script src="js/jquery-2_2_1_min.js"></script>
    <script src="js/card.js"></script>
    <title>我的洗车卡</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:Repeater ID="cardRepeater" runat="server">
        <ItemTemplate>
            <a href="./CardDetail.aspx?cid=<%#Eval("KeyId") %>">
                <div class="card_container">
                    <div class="card"">
                        <img src='<%#Eval("Logo") %>' />
                        <p><%#Eval("Name") %></p>
                    </div>
                    <div class="bottom">
                        <p class="left">余额：<%#Eval("Coins") %> 元</p>
                        <p class="right">积分：<%#Eval("Points") %> 点</p>
                    </div>
                </div>
            </a>
        </ItemTemplate>
    </asp:Repeater>
    </form>
</body>
</html>
