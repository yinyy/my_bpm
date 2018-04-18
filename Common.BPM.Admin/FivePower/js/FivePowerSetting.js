var handler = 'ashx/FivePowerSettingHandler.ashx';

//系统全局设置
$(function () {
    $('#btnok').click(function () {
        var json = {};
        json.Appid = $('#txt_Appid').val();
        json.Secret = $('#txt_Secret').val();
        json.Aeskey = $('#txt_Aeskey').val();
        json.Token = $('#txt_Token').val();
        json.Address = $('#txt_Address').val();

        json.Setting = JSON.stringify(json.Setting);

        $.post(handler, json, function (d) {
            if (d == 1)
                msg.ok('参数设置保存成功。');
            else
                alert(d);
        });
    });
    
    //显示原来的数据
    $('#txt_Appid').val(json.Appid);
    $('#txt_Secret').val(json.Secret);
    $('#txt_Aeskey').val(json.Aeskey);
    $('#txt_Token').val(json.Token);
    $('#txt_Address').val(json.Address);
   
    $('body').css('overflow', 'auto');
});