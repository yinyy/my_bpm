<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="WasherSetting.aspx.cs" Inherits="BPM.Admin.Washer.WasherSetting" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="sysconfig" style="margin:10px;">

    <h1>基本设置</h1>
    <div class="c">
        <ul>
            <li><div>初始金额：</div><input type="text" id="txt_Money" name="Money" /></li>
            <li><div>其它参数：</div><input type="text" id="txt_Other" name="Other" /></li>
        </ul>
    </div>
</div>

<div style="margin:140px;width:160px; margin-top:40px; font-family:'Microsoft YaHei'">

    <a id="btnok" href="javascript:;" class="alertify-button alertify-button-ok">保存设置</a>

</div>
    
    <script src="ashx/WasherSettingHandler.ashx?action=js" type="text/javascript"></script>
    <!-- 引入js文件 -->
    <script src="js/WasherSetting.js?d=<%=DateTime.Now %>"></script>
</asp:Content>


