<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="../Site1.Master" CodeBehind="CourseBranch.aspx.cs" Inherits="BPM.Admin.Course.CourseBranch" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">



</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- 工具栏按钮 -->
    <div id="toolbar"><%= base.BuildToolbar()%></div>

    <!-- datagrid 列表 -->
    <table id="list"></table>  

    <!-- 引入多功能查询js -->
    <script src="../../scripts/Business/Search.js?t=<%=DateTime.Now.Ticks %>"></script>
    <script src="../scripts/Business/Export.js?d=<%=DateTime.Now.Ticks %>"></script>


    <!-- 引入js文件 -->
    <script src="../scripts/ckeditor5-build-classic/ckeditor.js"></script>
    <script src="js/CourseBranch.js?d=<%=DateTime.Now %>"></script>
</asp:Content>
