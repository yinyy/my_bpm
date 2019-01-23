<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="CommonSetting.aspx.cs" Inherits="BPM.Admin.Common.CommonSetting" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="sysconfig" style="margin:10px;">

        <h1>微信公众平台设置</h1>
        <div class="c">
            <ul>
                <li><div>AppId：</div><input type="text" id="txt_AppId" name="AppId" style="width: 400px" /></li>
                <li><div>AppSecret：</div><input type="text" id="txt_Secret" name="Secret" style="width: 400px" /></li>
                <li><div>Token：</div><input type="text" id="txt_Token" name="Token" style="width: 400px" /></li>
                <li><div>EncodingAESKey：</div><input type="text" id="txt_AesKey" name="AesKey" style="width: 400px" /></li>
            </ul>
        </div>
    </div>

    <div style="margin:140px;width:160px; margin-top:40px; font-family:'Microsoft YaHei'">
        <a id="btnok" href="javascript:;" class="alertify-button alertify-button-ok">保存设置</a>
    </div>


    <script src="ashx/CommonSettingHandler.ashx?action=js" type="text/javascript"></script>
    <script src="js/CommonSetting.js?d=<%=DateTime.Now.Ticks %>" type="text/javascript"></script>
</asp:Content>
