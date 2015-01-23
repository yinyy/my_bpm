<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LogisticsFreightGroup.aspx.cs" Inherits="BPM.Admin.demo.LogisticsFreightGroup"  MasterPageFile="../Site1.Master"%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <div id="layout">
        <div region="west" style="width:200px;border-right: 0px;" border="true">
            <div class="easyui-panel" title="货代分组" border="false" iconCls="icon-book_red" >
                <div style="padding:5px;">
                    <ul id="dataDicType"></ul>
                </div>
            </div>
            <div id="noCategoryInfo" style="font-family:微软雅黑; font-size: 18px; color:#BCBCBC; padding: 40px 5px;display:none;">
                　　亲，还没有字典类别哦，点击 添加 按钮创建新的类别。
            </div>
        </div>
        <div region="center" border="false" style="overflow: hidden;">
            <div id="toolbar"><%=base.BuildToolbar() %></div>
            <table id="dicGrid"></table>
        </div>
    </div>
    <script type="text/javascript" src="js/LogisticsFreightGroup.js?v=90&d=<%=DateTime.Now %>"></script>
</asp:Content>
