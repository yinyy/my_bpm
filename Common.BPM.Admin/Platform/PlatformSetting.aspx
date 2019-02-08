<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="PlatformSetting.aspx.cs" Inherits="BPM.Admin.Platform.PlatformSetting" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="sysconfig" style="margin:10px;">

        <h1>消息模板</h1>
        <div class="c">
            <ul>
                <li>
                    <div>监考通知消息模板ID：</div>
                    <input type="text" id="txt_InvigilateTemplateID" name="InvigilateTemplateID" style="width: 405px" />
                </li>
            </ul>
            <ul>
                <li>
                    <div>监考温馨提示语：</div>
                    <textarea id="txt_InvigilateReminder" name="InvigilateReminder" cols="55" rows="6"></textarea>
                </li>
            </ul>
        </div>
    </div>

    <div style="margin:140px;width:160px; margin-top:40px; font-family:'Microsoft YaHei'">
        <a id="btnok" href="javascript:;" class="alertify-button alertify-button-ok">保存设置</a>
    </div>

    <script src="js/PlatformSetting.js?d=<%=DateTime.Now.Ticks %>" type="text/javascript"></script>
</asp:Content>
