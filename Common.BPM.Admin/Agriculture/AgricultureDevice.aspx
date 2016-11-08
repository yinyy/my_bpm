<%@ Page Language="C#"  MasterPageFile="../Site1.Master" AutoEventWireup="true" CodeBehind="AgricultureDevice.aspx.cs" Inherits="BPM.Admin.Agriculture.AgricultureDevice" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="toolbar"><%=base.BuildToolbar() %></div>

    <table id="list"></table>
 
    <script type="text/javascript" src="../scripts/Business/Search.js?v=3"></script>
    
    <script src="../scripts/Business/Export.js"></script>
    <script type="text/javascript" src="js/AgricultureDevice.js?v=<%=DateTime.Now.Ticks %>"></script>
</asp:Content>
