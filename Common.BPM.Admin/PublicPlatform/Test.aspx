<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Test.aspx.cs" Inherits="BPM.Admin.PublicPlatform.Test" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <label>ACCESS_TOKEN：</label><label><%=WeChat.Utils.WeChatToolkit.AccessToken%></label><br />
    <asp:Button ID="上传素材" runat="server" Text="上传素材" OnClick="上传素材_Click" /><br />
        <asp:Button ID="创建菜单" runat="server" Text="创建菜单" OnClick="创建菜单_Click" /><br />
        <asp:TextBox ID="返回信息" runat="server" TextMode="MultiLine" Columns="150" Rows="30"></asp:TextBox>
    </div>
    </form>
</body>
</html>
