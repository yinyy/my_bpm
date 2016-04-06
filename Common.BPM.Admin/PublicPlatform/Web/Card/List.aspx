<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="BPM.Admin.PublicPlatform.Web.Card.List" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width,initial-scale=1,user-scalable=no">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <link href="../css/WeUI/style/weui.min.css" rel="stylesheet" />
    <link href="../css/card.css?d=<%=DateTime.Now %>" rel="stylesheet" />
    <title>我的洗车卡</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:Repeater ID="cardRepeater" runat="server">
        <ItemTemplate>
            <a href="./Detail.aspx?cid=<%#Eval("KeyId") %>">
                <div class="card_container">
                    <div class="card" style="background-color:<%#Eval("CardColor")%>">
                        <img src='<%#Eval("Logo") %>' />
                        <p><%#Eval("Brand") %></p>
                    </div>
                    <div class="bottom">
                        <p class="left">余额：<%#Eval("Coins") %>元</p>
                        <p class="right">积分：<%#Eval("Points") %>点</p>
                    </div>
                </div>
            </a>
        </ItemTemplate>
    </asp:Repeater>
    </form>
</body>
</html>
