//http://www.tuicool.com/articles/beEN3m7
//https://pay.weixin.qq.com/wiki/doc/api/jsapi.php?chapter=7_4

var commonActionURL = "/Washer/ashx/WasherHandler.ashx";
var actionURL = "/PublicPlatform/Web/PaymentHandler.ashx";
var pi;

function onBridgeReady() {
    //alert(pi.AppID + "\t" + pi.Timestamp + "\t" + pi.Noncestr + "\t" + pi.Package + "\t" + pi.PaySign);
    WeixinJSBridge.invoke(
        'getBrandWCPayRequest', {
            "appId": pi.AppID,     //公众号名称，由商户传入     
            "timeStamp": pi.Timestamp,         //时间戳，自1970年以来的秒数     
            "nonceStr": pi.Noncestr, //随机串     
            "package": pi.Package,
            "signType": "MD5",         //微信签名方式：     
            "paySign": pi.PaySign //微信签名 
        },
        function (res) {
            //alert(res.err_code + "\r\n" + res.err_desc + "\r\n" + res.err_msg);
            if (res.err_msg == "get_brand_wcpay_request:ok") {
                //alert('dfadsfasdf');
                // 使用以上方式判断前端返回,微信团队郑重提示：res.err_msg将在用户支付成功后返回ok，但并不保证它绝对可靠。 

                //判断确实支付成功了，然后再开启设备
                alert(pi.Serial);
                $.post(actionURL, { action: 'validate_payment', serial: '2016052300007082407' }, function (res) {
                    if (res.Success) {
                        $.post(commonActionURL, { action: 'send_command', BalanceId: res.BalanceId, Money: res.Money, BoardNumber: '100007' }, function (res1) {
                            if (res1.Success) {
                                WeixinJSBridge.invoke('closeWindow', {}, function (res) {
                                    alert(res.err_msg);
                                });
                            } else {
                                alert('命令发送失败。');
                            }
                        }, 'json');
                    } else {
                        alert('支付失败。');
                    }
                }, 'json');
            }else if(res.err_msg=='get_brand_wcpay_request:cancel'){
                alert('用户取消');
            }
        }
    );
}

$(document).ready(function () {
    $('#pay_button').click(payFunction);
});

function payFunction() {
    $.post(commonActionURL, { action: 'send_command', BalanceId: 38}, function (res1) {
        if (res1.Success) {
            WeixinJSBridge.invoke('closeWindow', {}, function (res) {
                alert(res.err_msg);
            });
        } else {
            alert('命令发送失败。');
        }
    }, 'json');


    return;



    var money = $.trim($('#pay_money').val());
    var reg = new RegExp("^[0-9]+[\.]?[0-9]*$");
    if (!reg.test(money)) {
        alert('支付金额格式不正确。');
        return false;
    }

    $(this).addClass('weui_btn_disabled');
    $(this).unbind('click');

    $.post(actionURL, { action: 'create_pay_params', body: '洗车费用', pay: parseInt(parseFloat(money) * 100), wxid: ps.wxid, attach: ps.board }, function (res) {
        if (res.Success == true) {
            pi = eval('(' + res.PrepayInfo + ')');

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
        }
    }, 'json');
}
