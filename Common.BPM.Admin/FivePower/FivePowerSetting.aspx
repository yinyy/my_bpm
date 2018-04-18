<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="FivePowerSetting.aspx.cs" Inherits="BPM.Admin.FivePower.FivePowerSetting" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../scripts/Colorpicker/spectrum.css" rel="stylesheet" />
    <link href="../scripts/fine-uploader/fine-uploader-new.min.css" rel="stylesheet" />
    <style type="text/css">
        div.c ul:first-child li label:first-child{
            text-align: right;
            width: 100px;

            display: inline-block;
        }

        div.c ul:first-child li img#Logo{
            width: 48px;
            height: 48px;

            border: 1px solid #ffffff;
        }

        .full-spectrum .sp-palette {
            max-width: 200px;
        }

        div.c ul:first-child li ul {
            display: inline-block;
            vertical-align: middle;
        }

        div.c ul:first-child li ul li label:first-child{
            width: 60px;
            text-align: right;
        }

        div.c ul:first-child li ul li input{
            width: 60px;
            text-align: center;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="sysconfig" style="margin:10px;">
        <h1>微信参数设置</h1>
        <div class="c">
            <ul>
                <li><label>appid：</label><input type="text" id="txt_Appid" name="Appid" style="width: 500px;"/></li>
                <li><label>secret：</label><input type="text" id="txt_Secret" name="Secret" style="width: 500px;"/></li>
                <li><label>aeskey：</label><input type="text" id="txt_Aeskey" name="Aeskey" style="width: 500px;"/></li>
                <li><label>Token：</label><input type="text" id="txt_Token" name="Token" style="width: 500px;"/></li>
           </ul>
        </div>
        <h1>安装地点参数设置</h1>
        <div class="c">
            <ul>
                <li>
                    <label for="txt_Address">安装地点：</label><textarea id="txt_Address" rows="10" cols="100" style="display: inline-block;vertical-align: middle;"></textarea>
                </li>
                <li><label>&nbsp;</label>格式：多个安装地点之间用英文分号分隔。</li>
           </ul>
        </div>
    </div>


    <div style="margin:140px;width:160px; margin-top:40px; font-family:'Microsoft YaHei'">
        <a id="btnok" href="javascript:;" class="alertify-button alertify-button-ok">保存设置</a>
        <%--<a id="btnmenu" href="javascript:;" class="alertify-button alertify-button-ok">创建菜单</a>--%>
    </div>


    
    <script src="ashx/FivePowerSettingHandler.ashx?action=js" type="text/javascript"></script>
    <!-- 引入js文件 -->
    <script src="../scripts/Colorpicker/spectrum.js?d=<%=DateTime.Now %>"></script>
    <script src="../scripts/fine-uploader/fine-uploader.min.js"></script>
    <script src="js/FivePowerSetting.js?d=<%=DateTime.Now %>"></script>
    <script src="../Editor/xhEditor/xheditor-1.2.1.min.js"></script>
    <script src="../Editor/xhEditor/xheditor_lang/zh-cn.js"></script>
</asp:Content>


