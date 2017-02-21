//http://www.tuicool.com/articles/beEN3m7
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

                $('div.card_kind_area > ul.card_kind_list > li').click(function () {
                    $('div.card_kind_area > ul.card_kind_list > li').removeClass('selected');
                    $(this).addClass('selected');
                })

                $('#pay_button').click(function () {
                    if ($('#pay_button.weui_btn_disabled').size() > 0) {
                        return;
                    }

                    var money = 0;

                    if ($('div.card_kind_area > ul.card_kind_list > li').size() == 0) {
                        money = $.trim($('#pay_money').val());
                        var reg = new RegExp("^[0-9]+[\.]?[0-9]*$");
                        if (!reg.test(money)) {
                            alert('支付金额格式不正确。');
                            return false;
                        }
                        money = parseInt(parseFloat(money) * 100);
                    } else {
                        if ($('div.card_kind_area > ul.card_kind_list > li.selected').size() <= 0) {
                            alert('请选择支付金额。');
                            return;
                        }

                        money = parseInt($('div.card_kind_area > ul.card_kind_list > li.selected > p').attr('value'));
                    }

                    $(this).addClass('weui_btn_disabled');
                    $('#loading_region').show();

                    var board = Common.getQueryString('board');
                    Pay.prepay({ body: '微信支付洗车', pay: money, attach: JSON.stringify({ Desc: '微信支付洗车', Board: board }) },
                                           function (pi) {
                                               $(this).removeClass('weui_btn_disabled');

                                               $('#loading_region').hide();

                                               alert('支付成功。');
                                               wx.closeWindow();
                                           },
                                           function () {
                                               $(this).removeClass('weui_btn_disabled');

                                               $('#loading_region').hide();

                                               alert('支付失败。');
                                               wx.closeWindow();
                                           },
                                           function () {
                                               $(this).removeClass('weui_btn_disabled');

                                               $('#loading_region').hide();
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