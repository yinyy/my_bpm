<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="ShoppingCommodity.aspx.cs" Inherits="BPM.Admin.Shopping.ShoppingCommodity" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        img.Commodity_Picture{
            width: 120px;
            height: 90px;
            margin: 10px;
        }
    </style>
    <link href="../scripts/fine-uploader/fine-uploader.min.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <!-- 工具栏按钮 -->
    <div id="toolbar"><%= base.BuildToolbar()%></div>

    <!-- datagrid 列表 -->
    <table id="list" ></table>  

    <!-- 引入多功能查询js -->
    <script src="../../scripts/Business/Search.js"></script>

    <script type="text/javascript" charset="utf-8" src="/Editor/xhEditor/xheditor-1.2.1.min.js"></script>
    <script type="text/javascript" charset="utf-8" src="/Editor/xhEditor/xheditor_lang/zh-cn.js"></script>

    <script src="../scripts/fine-uploader/fine-uploader.min.js"></script>

    <!-- 引入js文件 -->
    <script src="js/ShoppingCommodity.js?d=<%=DateTime.Now.Ticks %>"></script>
</asp:Content>
