<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PayWash.aspx.cs" Inherits="BPM.Admin.PublicPlatform.Web.PayWash" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta name="viewport" content="width=device-width,initial-scale=1,user-scalable=no">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>洗车支付</title>
    <link href="css/WeUI/style/weui.css" rel="stylesheet" />
    <link href="css/style.css" rel="stylesheet" />
    <link href="css/Card2.css" rel="stylesheet" />
    <script src="js/jquery-2_2_1_min.js"></script>
    <script src="http://res.wx.qq.com/open/js/jweixin-1.0.0.js"></script>
    <script src="js/common.js"></script>
    <script src="js/Pay.js?t=<%=DateTime.Now.Ticks %>"></script>
    <script src="js/PayWash.js?t=<%=DateTime.Now.Ticks %>"></script>
</head>
<body>
    <form id="form1" runat="server" style="display:none;">
       <div id="loading_region" class="weui_loading_toast" style="display: none;">
            <div class="weui_mask_transparent"></div>
            <div class="weui_toast">
                <div class="weui_loading">
                    <div class="weui_loading_leaf weui_loading_leaf_0"></div>
                    <div class="weui_loading_leaf weui_loading_leaf_1"></div>
                    <div class="weui_loading_leaf weui_loading_leaf_2"></div>
                    <div class="weui_loading_leaf weui_loading_leaf_3"></div>
                    <div class="weui_loading_leaf weui_loading_leaf_4"></div>
                    <div class="weui_loading_leaf weui_loading_leaf_5"></div>
                    <div class="weui_loading_leaf weui_loading_leaf_6"></div>
                    <div class="weui_loading_leaf weui_loading_leaf_7"></div>
                    <div class="weui_loading_leaf weui_loading_leaf_8"></div>
                    <div class="weui_loading_leaf weui_loading_leaf_9"></div>
                    <div class="weui_loading_leaf weui_loading_leaf_10"></div>
                    <div class="weui_loading_leaf weui_loading_leaf_11"></div>
                </div>
                <p class="weui_toast_content">数据加载中...</p>
            </div>
        </div>

        <div id="shopping_region">
            <div>
                <img src="./images/icon_wash_car.png" width="100%" />
            </div>
            <div class="card_kind_area">
                <%
    if (PayWashKinds.Length > 0)
    {
        int i;
        for (i = 0; i < PayWashKinds.Length; i++)
        {
            if (i % 3 == 0)
            {
                %><ul class="card_kind_list"><%
    }
                %><li><p value="<%=PayWashKinds[i] %>"><%=PayWashKinds[i] / 100 %>元</p></li><%
    if (i % 3 == 2)
    {
                %></ul><%
        }
    }

    if (i % 2 != 2)
    {
                %></ul><%
        }
    }
    else
    {
                %><input id="pay_money" class="weui_input" type="number" pattern="[0-9]*" placeholder="请输入支付金额" style="font-size: 2em; text-align: center;margin: 0.3em 0em 0em 0em;"/><%
    }%>
            </div>
            <div class="cell">
                <p>说明：</p>
                <p>1、洗车后不退余额，请酌情支付。</p>
                <p>2、洗车时金额不足时，洗车机自动停止。</p>
                <p style="color:red;font-weight: bolder;">3、支付成功后，请在本页面等待确认支付结果。若30秒内无响应，请联系客服。</p>
            </div>
            <div class="weui_btn_area">
                <a id="pay_button" class="weui_btn weui_btn_primary" href="javascript:">支付</a>
            </div>
        </div>
    </form>
</body>
</html>
