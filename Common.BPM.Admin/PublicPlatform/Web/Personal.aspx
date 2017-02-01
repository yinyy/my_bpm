<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Personal.aspx.cs" Inherits="BPM.Admin.PublicPlatform.Web.Personal" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width,initial-scale=1,user-scalable=no">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <link href="./css/WeUI/style/weui.min.css" rel="stylesheet" />
    <link href="./css/Personal.css?d=<%=DateTime.Now %>" rel="stylesheet" />
    <script src="./js/jquery-2_2_1_min.js"></script>
    <script src="./js/Personal.js?t=<%=DateTime.Now %>"></script>
    <title>设置</title>
</head>

<body>
    <form id="uiform" runat="server">
        <div class="weui_cells_title">会员卡支付限额</div>
        <div class="weui_cells weui_cells_form">
            <div class="weui_cell">
                <div class="weui_cell_bd weui_cell_primary">
                    <input class="weui_input" type="number" pattern="[0-9]*" placeholder="支付限额" id="max_pay_coins"/>
                </div>
            </div>
        </div>
        <div class="weui_cells_tips">设置会员卡支付洗车时，单次消费的最大额度（单位元）。为了保证账户安全，请设置为大于0的整数。0表示不设置限额。</div>
        <div class="weui_btn_area">
            <a class="weui_btn weui_btn_primary" href="javascript:" id="save_button">确定</a>
        </div>

        <div class="weui_dialog_alert" id="dialog2" style="display: none;">
            <div class="weui_mask"></div>
            <div class="weui_dialog">
                <div class="weui_dialog_hd"><strong class="weui_dialog_title">提示</strong></div>
                <div class="weui_dialog_bd">弹窗内容，告知当前页面信息等</div>
                <div class="weui_dialog_ft">
                    <a href="javascript:;" class="weui_btn_dialog primary" onclick="$('#dialog2').hide();">确定</a>
                </div>
            </div>
        </div>

    </form>
</body>
</html>
