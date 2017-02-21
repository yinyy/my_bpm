//系统全局设置
$(function () {
    $("input#txt_Color").spectrum({
        color: "#ECC",
        showInput: true,
        className: "full-spectrum",
        showInitial: true,
        showPalette: true,
        showSelectionPalette: true,
        maxSelectionSize: 10,
        preferredFormat: "hex",
        localStorageKey: "spectrum.demo",
        move: function (color) {

        },
        show: function () {

        },
        beforeShow: function () {

        },
        hide: function () {

        },
        change: function () {

        },
        palette: [
            ["rgb(0, 0, 0)", "rgb(67, 67, 67)", "rgb(102, 102, 102)",
            "rgb(204, 204, 204)", "rgb(217, 217, 217)", "rgb(255, 255, 255)"],
            ["rgb(152, 0, 0)", "rgb(255, 0, 0)", "rgb(255, 153, 0)", "rgb(255, 255, 0)", "rgb(0, 255, 0)",
            "rgb(0, 255, 255)", "rgb(74, 134, 232)", "rgb(0, 0, 255)", "rgb(153, 0, 255)", "rgb(255, 0, 255)"],
            ["rgb(230, 184, 175)", "rgb(244, 204, 204)", "rgb(252, 229, 205)", "rgb(255, 242, 204)", "rgb(217, 234, 211)",
            "rgb(208, 224, 227)", "rgb(201, 218, 248)", "rgb(207, 226, 243)", "rgb(217, 210, 233)", "rgb(234, 209, 220)",
            "rgb(221, 126, 107)", "rgb(234, 153, 153)", "rgb(249, 203, 156)", "rgb(255, 229, 153)", "rgb(182, 215, 168)",
            "rgb(162, 196, 201)", "rgb(164, 194, 244)", "rgb(159, 197, 232)", "rgb(180, 167, 214)", "rgb(213, 166, 189)",
            "rgb(204, 65, 37)", "rgb(224, 102, 102)", "rgb(246, 178, 107)", "rgb(255, 217, 102)", "rgb(147, 196, 125)",
            "rgb(118, 165, 175)", "rgb(109, 158, 235)", "rgb(111, 168, 220)", "rgb(142, 124, 195)", "rgb(194, 123, 160)",
            "rgb(166, 28, 0)", "rgb(204, 0, 0)", "rgb(230, 145, 56)", "rgb(241, 194, 50)", "rgb(106, 168, 79)",
            "rgb(69, 129, 142)", "rgb(60, 120, 216)", "rgb(61, 133, 198)", "rgb(103, 78, 167)", "rgb(166, 77, 121)",
            "rgb(91, 15, 0)", "rgb(102, 0, 0)", "rgb(120, 63, 4)", "rgb(127, 96, 0)", "rgb(39, 78, 19)",
            "rgb(12, 52, 61)", "rgb(28, 69, 135)", "rgb(7, 55, 99)", "rgb(32, 18, 77)", "rgb(76, 17, 48)"]
        ]
    });

    $('input#rb_Kind_Point').click(function () {
        $(this).parent().parent().prev().children('li').children('label:odd').html('&nbsp;&nbsp;积分');
    });

    $('input#rb_Kind_Percent').click(function () {
        $(this).parent().parent().prev().children('li').children('label:odd').html('&nbsp;&nbsp;%的充值积分');
    });

    var uploader = new qq.FineUploader({
        debug: true,
        button: document.getElementById('Upload'),
        element: document.getElementById('fine-uploader'),
        request: {
            endpoint: '/ashx/UploadHandler.ashx'
        },
        multiple: false,
        allowedExtensions: ['.png', '.jpg'],
        retry: {
            enableAuto: false
        },
        callbacks: {
            onComplete: function (id, name, json, xhr) {
                $('#Logo').attr('src', json.url);
                $('#txt_Logo').val(json.url);
                
                msg.ok('上传成功。');
            },
            onError: function (id, name, reason, xhr) {
                msg.warning('上传失败！');
            },
            onSubmit: function (id, name) {
                //alert('submit');
            },
            onSubmitted: function (id, name) {
                //alert('submitted');
            }
        }
    });

    //var editor = $('#txt_Intro').xheditor({
    //    tools: 'Cut,Copy,Paste,Pastetext,|,Blocktag,|,Source,Preview'
    //});

    $('#btnok').click(function () {
        var json = {};
        json.Appid = $('#txt_Appid').val();
        json.Secret = $('#txt_Secret').val();
        json.Aeskey = $('#txt_Aeskey').val();
        json.Token = $('#txt_Token').val();
        json.Brand = $('#txt_Brand').val();
        json.Logo = $('#txt_Logo').val();
        json.Setting = {};

        //处理微信支付选项
        var vs = [];
        if($.trim($('#txt_WxPayOption').val())!=''){
            $($.trim($('#txt_WxPayOption').val()).split(',')).each(function (i, v) {
                vs[vs.length] = parseInt(v);
            });
            json.Setting.WxPayOption = vs;
        }

        //处理短信接口参数
        json.Setting.Sms = {
            Cid: $('#txt_SmsCid').val(),
            Uid: $('#txt_SmsUid').val(),
            Pas: $('#txt_SmsPas').val(),
            Url: $('#txt_SmsUrl').val()
        };

        //处理购买洗车卡选项
        vs = [];
        if ($.trim($('#txt_Buy_Card_Option').val()) != '') {
            $($.trim($('#txt_Buy_Card_Option').val()).split(';')).each(function (i, o) {
                o = $.trim(o);
                o = o.split(',');

                vs[vs.length] = { Product: o[0], Value: parseInt(o[1]), Price: parseFloat(o[2]), Day: parseInt(o[3]), Score: parseInt(o[4]) };
            });
            json.Setting.BuyCardOption = vs;
        }

        //处理推荐奖励
        vs = [];
        if($.trim($('#txt_GiftLevel').val())!=''){
            $($.trim($('#txt_GiftLevel').val()).split(',')).each(function (i, o) {
                vs[vs.length] = parseInt(o);
            });
            json.Setting.GiftLevel = vs
        }

        //处理注册赠送
        json.Setting.Register = {
            Coupon: parseInt($('#txt_RegisterCoupon').val()), CouponDay: parseInt($('#txt_RegisterCouponDay').val()), Point: parseInt($('#txt_RegisterPoint').val())
        };

        //处理洗车积分
        json.Setting.PayWashCar = {
            Wx: parseInt($('#txt_Pay_Wash_Card_Wx').val()), Vip: parseInt($('#txt_Pay_Wash_Card_Vip').val()), Coupon: 0
        };

        //处理文章转发
        json.Setting.Relay = {
            Friend: parseInt($('#txt_Relay_Friend').val()),
            Moment: parseInt($('#txt_Relay_Moment').val())
        };

        json.Setting = JSON.stringify(json.Setting);

        $.post('ashx/WasherSettingHandler.ashx', json, function (d) {
            if (d == 1)
                msg.ok('参数设置保存成功。');
            else
                alert(d);
        });
    });

    $('#btnmenu').click(function () {
        if (confirm('创建菜单将会覆盖已有菜单。确认创建菜单吗？')) {
            $.post('ashx/WasherSettingHandler.ashx?action=menu', function (json) {
                if (json.Success == true) {
                    msg.ok('菜单创建成功。');
                } else {
                    alert(json.Message);
                }
            });
        }
    });

    //显示原来的数据
    $('#txt_Appid').val(json.Appid);
    $('#txt_Secret').val(json.Secret);
    $('#txt_Aeskey').val(json.Aeskey);
    $('#txt_Token').val(json.Token);
    $('#txt_Brand').val(json.Brand);
    $('#txt_Logo').val(json.Logo);
    $('#Logo').attr('src', json.Logo);
    if (json.Setting != null) {
        var setting = eval("(" + json.Setting + ")");
        if (setting.WxPayOption != null) {
            //处理微信支付选项
            var vs = [];
            $(setting.WxPayOption).each(function (i, v) {
                vs[vs.length] = parseInt(v); 
            });
            $('#txt_WxPayOption').val(vs.join(','));
        }

        if (setting.Sms != null) {
            //处理短信接口参数
            $('#txt_SmsCid').val(setting.Sms.Cid);
            $('#txt_SmsUid').val(setting.Sms.Uid);
            $('#txt_SmsPas').val(setting.Sms.Pas);
            $('#txt_SmsUrl').val(setting.Sms.Url);
        }

        if (setting.BuyCardOption != null) {
            //处理购买洗车卡选项
            vs = [];
            $(setting.BuyCardOption).each(function (i, o) {
                var v = [o.Product, o.Value, o.Price, o.Day, o.Score];
                vs[vs.length] = v.join(',');
            });
            $('#txt_Buy_Card_Option').val(vs.join(';\r\n'));
        }

        if (setting.GiftLevel != null) {
            //处理推荐奖励
            vs = [];
            $(setting.GiftLevel).each(function (i, o) {
                vs[vs.length] = parseInt(o);
            });
            $('#txt_GiftLevel').val(vs.join(','));
        }

        if (setting.Register != null) {
            //处理注册赠送
            $('#txt_RegisterCoupon').val(setting.Register.Coupon);
            $('#txt_RegisterCouponDay').val(setting.Register.CouponDay);
            $('#txt_RegisterPoint').val(setting.Register.Point);
        }

        if (setting.PayWashCar != null) {
            //处理洗车积分
            $('#txt_Pay_Wash_Card_Wx').val(setting.PayWashCar.Wx);
            $('#txt_Pay_Wash_Card_Vip').val(setting.PayWashCar.Vip);
        }

        if (setting.Relay != null) {
            //处理文章转发
            $('#txt_Relay_Friend').val(setting.Relay.Friend);
            $('#txt_Relay_Moment').val(setting.Relay.Moment);
        }
    }

    $('body').css('overflow', 'auto');
});