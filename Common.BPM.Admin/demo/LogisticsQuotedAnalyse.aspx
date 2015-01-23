<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="LogisticsQuotedAnalyse.aspx.cs" Inherits="BPM.Admin.demo.LogisticsQuotedAnalyse" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <!-- datagrid 列表 -->
    <table id="list" ></table>  

    <!-- 引入js文件 -->
      <script src="js/LogisticsQuotedAnalyse.js?d=<%=DateTime.Now.Ticks %>"></script>
</asp:Content>



