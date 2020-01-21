<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="WasherReply2.aspx.cs" Inherits="BPM.Admin.Washer.WasherReply2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- 工具栏按钮 -->
    <div id="toolbar"><%= base.BuildToolbar()%></div>

    <!-- datagrid 列表 -->
    <table id="list"></table>  

    <!-- 引入js文件 -->
    <script src="js/WasherReply2.js?d=<%=DateTime.Now %>"></script>
</asp:Content>
