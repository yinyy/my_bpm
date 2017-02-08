﻿//http://www.tuicool.com/articles/beEN3m7
//https://pay.weixin.qq.com/wiki/doc/api/jsapi.php?chapter=7_4
//https://mp.weixin.qq.com/wiki 
//JS-SDK

var actionUrl = "/Washer/ashx/WasherHandler.ashx";

function onBridgeReady() {
    var ts = Common.getQueryString('ts');
    $.post(actionUrl, { action: 'Validate', ts: ts == null || ts == '' ? 0 : ts }, function (res) {
        if (res == 1) {
            var useCard = Common.getQueryString('card');
            if (useCard != null) {
                var board = Common.getQueryString('board');

                $.post(actionUrl, { action: 'PayCoins', board: board }, function (res) {
                    if (res.Success == false) {
                        alert('洗车机启动失败！');
                    } else {
                        alert('洗车机已经启动。');
                    }

                    wx.closeWindow();
                }, 'json');
            } else {
                $('#form1').show();

                $('#pay_button').click(function () {
                    if ($('#pay_button.weui_btn_disabled').size() > 0) {
                        return;
                    }

                    var money = $.trim($('#pay_money').val());
                    var reg = new RegExp("^[0-9]+[\.]?[0-9]*$");
                    if (!reg.test(money)) {
                        alert('支付金额格式不正确。');
                        return false;
                    }

                    $(this).addClass('weui_btn_disabled');
                    var board = Common.getQueryString('board');
                    Pay.prepay({ body: '微信支付洗车', pay: parseInt(parseFloat(money) * 100), attach: board },
                                           function (pi) {
                                               $.post(actionUrl, { action: 'PayWash', serial: pi.Serial }, function (res1) {
                                                   if (res1.Success) {
                                                       wx.closeWindow();
                                                   } else {
                                                       alert('命令发送失败。');
                                                   }
                                               }, 'json');

                                               $(this).removeClass('weui_btn_disabled');

                                               alert('支付成功。');

                                               //$.post(actionUrl, { action: 'payBind', value: Buy.selected.value }, function (res) {
                                               //    if (res.Success == true) {
                                               //        List.show();
                                               //    }
                                               //    else {
                                               //        alert('绑定失败。');
                                               //    }
                                               //}, 'json');
                                           },
                                           function () {
                                               $(this).removeClass('weui_btn_disabled');

                                               alert('支付失败。');
                                           },
                                           function () {
                                               $(this).removeClass('weui_btn_disabled');
                                           });
                });
            }
        } else {
            alert('请求已过期，请重新扫码。');

            wx.closeWindow();
        }
    }, 'json');
};

$(document).ready(function () {
    if (typeof WeixinJSBridge == "undefined") {
        if (document.addEventListener) {
            document.addEventListener('WeixinJSBridgeReady', onBridgeReady, false);
        } else if (document.attachEvent) {
            document.attachEvent('WeixinJSBridgeReady', onBridgeReady);
            document.attachEvent('onWeixinJSBridgeReady', onBridgeReady);
        }
    } else {
        onBridgeReady();
    }
});