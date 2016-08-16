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

        //json.CardColor = $("#txt_Color").spectrum("get").toHexString();

        //var intro = editor.getSource();
        //intro.replace('\\', '\\\\');
        //intro.replace('"', '\"');
        //intro.replace("'", "\'");
        //json.Introduction = intro;

        json.Setting = {
            Sms:{
                Cid: $('#txt_SmsCid').val(),
                Uid: $('#txt_SmsUid').val(),
                Pas: $('#txt_SmsPas').val(),
                Url: $('#txt_SmsUrl').val()
            },
            Point: {
                WashCar: parseInt($('#txt_WashCar').val()),
                Subscribe: parseInt($('#txt_Subscribe').val()),
                Recharge: [parseInt($('#txt_Point50').val()), parseInt($('#txt_Point100').val()), parseInt($('#txt_Point200').val())],
                Referers:{
                    Kind: $('#rb_Kind_Point').attr('checked') ? 'Point' : 'Percent',
                    Level: [parseInt($('#txt_Level1').val()), parseInt($('#txt_Level2').val()), parseInt($('#txt_Level3').val()), parseInt($('#txt_Level4').val()), parseInt($('#txt_Level5').val())]
                }
            }, Coin: {
                Exchange: parseInt($('#txt_Exchange').val()),
                Recharge: [parseInt($('#txt_Coin50').val()), parseInt($('#txt_Coin100').val()), parseInt($('#txt_Coin200').val())]
            }, Coupon: {
                Coins: parseInt($('#txt_Coupon').val()),
                Time: parseInt($('#txt_CouponTime').val())
            }, Buy: [
                { Value: 50, Price: parseFloat($('#txt_Card50').val()), Day: parseInt($('#txt_Day50').val()), Product: '50元洗车卡', Score: parseInt($('#txt_Score50').val()) },
                { Value: 100, Price: parseFloat($('#txt_Card100').val()), Day: parseInt($('#txt_Day100').val()), Product: '100元洗车卡', Score: parseInt($('#txt_Score100').val()) },
                { Value: 200, Price: parseFloat($('#txt_Card200').val()), Day: parseInt($('#txt_Day200').val()), Product: '200元洗车卡', Score: parseInt($('#txt_Score200').val()) },
                { Value: 300, Price: parseFloat($('#txt_Card300').val()), Day: parseInt($('#txt_Day300').val()), Product: '300元洗车卡', Score: parseInt($('#txt_Score300').val()) }
            ]
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
    //$("input#txt_Color").spectrum('set', json.CardColor);
    //editor.setSource(json.Introduction);

    if (json.Setting != null) {
        var setting = eval("(" + json.Setting + ")");

        $('#txt_WashCar').val(setting.Point.WashCar);
        $('#txt_Subscribe').val(setting.Point.Subscribe);

        $('#txt_Point50, #txt_Point100, #txt_Point200').each(function (idx, obj) {
            $(this).val(setting.Point.Recharge[idx]);
        });

        if (setting.Sms!=null) {
            $('#txt_SmsCid').val(setting.Sms.Cid);
            $('#txt_SmsUid').val(setting.Sms.Uid);
            $('#txt_SmsPas').val(setting.Sms.Pas);
            $('#txt_SmsUrl').val(setting.Sms.Url);
        }

        if (setting.Point.Referers.Kind == 'Point') {
            $('#rb_Kind_Point').click();
        } else {
            $('#rb_Kind_Percent').click();
        }

        $('#txt_Level1, #txt_Level2, #txt_Level3, #txt_Level4, #txt_Level5').each(function(idx, obj){
            $(this).val(setting.Point.Referers.Level[idx]);
        });

        $('#txt_Exchange').val(setting.Coin.Exchange);
        $('#txt_Coin50, #txt_Coin100, #txt_Coin200').each(function (idx, obj) {
            $(this).val(setting.Coin.Recharge[idx]);
        });

        $('#txt_Coupon').val(setting.Coupon.Coins);
        $('#txt_CouponTime').val(setting.Coupon.Time);

        $('#txt_Card50, #txt_Card100, #txt_Card200, #txt_Card300').each(function (idx, obj) {
            $(this).val(setting.Buy[idx].Price);
        });
        $('#txt_Day50, #txt_Day100, #txt_Day200, #txt_Day300').each(function (idx, obj) {
            $(this).val(setting.Buy[idx].Day);
        });
        $('#txt_Score50, #txt_Score100, #txt_Score200, #txt_Score300').each(function (idx, obj) {
            $(this).val(setting.Buy[idx].Score);
        });
    }

    $('body').css('overflow', 'auto');
});