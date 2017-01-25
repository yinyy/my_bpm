var Pay = {
    url: '/PublicPlatform/Web/handler/UnifyPay.ashx',
    prepay: function (params, success, failure, cancel) {
        Pay.success = success;
        Pay.failure = failure;
        Pay.cancel = cancel;

        $.post(Pay.url, { action: 'prepay', body: params.body, pay: params.pay, attach: params.attach }, function (res) {
            if (res.Success == true) {
                Pay.pi = eval('(' + res.PrepayInfo + ')');

                if (typeof WeixinJSBridge == "undefined") {
                    if (document.addEventListener) {
                        document.addEventListener('WeixinJSBridgeReady', Pay.onBridgeReady, false);
                    } else if (document.attachEvent) {
                        document.attachEvent('WeixinJSBridgeReady', Pay.onBridgeReady);
                        document.attachEvent('onWeixinJSBridgeReady', Pay.onBridgeReady);
                    }
                } else {
                    Pay.onBridgeReady();
                }
            }
        }, 'json');
    },

    onBridgeReady: function () {
        //alert("Appid:"+Pay.pi.AppID + "\tTimestamp:" + Pay.pi.Timestamp + "\tNoncestr:" + Pay.pi.Noncestr + "\tPackage:" + Pay.pi.Package + "\tSign:" + Pay.pi.PaySign);
        WeixinJSBridge.invoke(
            'getBrandWCPayRequest', {
                "appId": Pay.pi.AppID,     //公众号名称，由商户传入     
                "timeStamp": Pay.pi.Timestamp,         //时间戳，自1970年以来的秒数     
                "nonceStr": Pay.pi.Noncestr, //随机串     
                "package": Pay.pi.Package,
                "signType": "MD5",         //微信签名方式：     
                "paySign": Pay.pi.PaySign //微信签名 
            },
            function (res) {
                //alert(res.err_code + "\r\n" + res.err_desc + "\r\n" + res.err_msg);
                if (res.err_msg == "get_brand_wcpay_request:ok") {
                    //alert('dfadsfasdf');
                    // 使用以上方式判断前端返回,微信团队郑重提示：res.err_msg将在用户支付成功后返回ok，但并不保证它绝对可靠。 

                    //判断确实支付成功了，然后再开启设备
                    //alert(pi.Serial);
                    //$.post(Pay.url, { action: 'validate', serial: Pay.pi.Serial }, function (res) {
                    //    if (res.Success) {
                    //        if (Pay.success != null) {
                    //            Pay.success(Pay.pi);
                    //        }
                    //    } else {
                    //        if (Pay.failure != null) {
                    //            Pay.failure();
                    //        }
                    //    }
                    //}, 'json');
                    //var socket = new WebSocket('ws://139.129.43.203:5500');
                    var socket = new WebSocket('ws://127.0.0.1:5500');
                    socket.onerror = function (event) {
                        alert("error at 2.");
                    };
                    socket.onopen = function (event) {
                        var o = { Action: 'wx_pay', Data: JSON.stringify({Serial: Pay.pi.Serial}) };
                        socket.send(JSON.stringify(o));

                        socket.onmessage = function (event) {
                            var code = event.data;
                            if (code == 'pay_success') {
                                //操作成功
                                if (Pay.success != nul) {
                                    Pay.success(Pay.pi);
                                }
                            } else {
                                if (Pay.failure != null) {
                                    Pay.failure();
                                }
                            }
                        };
                    };
                } else if (res.err_msg == 'get_brand_wcpay_request:cancel') {
                    if (Pay.cancel != null) {
                        Pay.cancel();
                    }
                }
            }
        );
    }
}