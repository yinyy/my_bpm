<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site1.Master" CodeBehind="SanitationWorkRegion.aspx.cs" Inherits="BPM.Admin.Sanitation.SanitationWorkRegion" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- 工具栏按钮 -->
    <div id="toolbar"><%= base.BuildToolbar()%></div>
    
    <div id="map_container">  
        
    </div>


    <script src="js/SanitationMapData.js?d=<%=DateTime.Now %>"></script>
    <script src="js/SanitationWorkRegion.js?d=<%=DateTime.Now %>"></script>
    <script type="text/javascript" src="http://api.map.baidu.com/api?v=1.5&ak=FBddae84e942f0b6b28aa762786b00f8">
    //v1.5版本的引用方式：src="http://api.map.baidu.com/api?v=1.5&ak=您的密钥"
    //v1.4版本及以前版本的引用方式：src="http://api.map.baidu.com/api?v=1.4&key=您的密钥&callback=initialize"
    </script>
    <script type="text/javascript" src="http://developer.baidu.com/map/jsdemo/demo/convertor.js"></script>
</asp:Content>



