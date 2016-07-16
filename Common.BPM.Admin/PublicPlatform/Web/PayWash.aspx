<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PayWash.aspx.cs" Inherits="BPM.Admin.PublicPlatform.Web.PayWash" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta name="viewport" content="width=device-width,initial-scale=1,user-scalable=no">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>洗车支付</title>
    <link href="css/WeUI/style/weui.css" rel="stylesheet" />
    <link href="css/style.css" rel="stylesheet" />
    <link href="css/PayWash.css" rel="stylesheet" />
    <script src="js/jquery-2_2_1_min.js"></script>
    <script src="js/common.js"></script>
    <script src="js/Pay.js?t=<%=DateTime.Now.Ticks %>"></script>
    <script src="js/PayWash.js?t=<%=DateTime.Now.Ticks %>"></script>
</head>
<body>
    <form id="form1" runat="server" style="display:none;">
        <div id="top_logo">
            <img src="./images/icon_wash_car.png" width="100%" />
        </div>

        <div id="money_container">
            <div class="cell"><label>支付金额</label></div>
            <div class="cell">
                <input id="pay_money" class="weui_input" type="number" pattern="[0-9]*" placeholder="请输入支付金额" style="font-size: 3em; text-align: center;"/>
            </div>
            <div class="cell hline"></div>
            <div class="cell">
                <p>说明：</p>
                <p>1、洗车后不退余额，请酌情支付。</p>
                <p>2、洗车时金额不足时，洗车机自动停止。</p>
            </div>
        </div>

        <div id="bottom">
            <a id="pay_button" href="javascript:;" class="weui_btn weui_btn_primary">支付</a>
        </div>

        <div id="msg_ok" class="page" style="display:none">
            <div class="weui_msg">
                <div class="weui_icon_area"><i class="weui_icon_success weui_icon_msg"></i></div>
                <div class="weui_text_area">
                    <h2 class="weui_msg_title">支付成功！</h2>
                    <p class="weui_msg_desc">内容详情，可根据实际需要安排</p>
                </div>
                <div class="weui_opr_area">
                    <p class="weui_btn_area">
                        <a href="javascript:;" id="pay_ok" class="weui_btn weui_btn_primary">确定</a>
                    </p>
                </div>
                <div class="weui_extra_area">
                    <a href="">dasdfadfasdf</a>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
