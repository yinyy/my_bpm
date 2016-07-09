var actionUrl= '/PublicPlatform/Web/handler/ProfileHandler.ashx';

var countdownvalue = 30;

$(document).ready(function () {
    //进行身份验证
    if (Authorize.authorize() == null) {
        return;
    }

    //$.ajax('/PublicPlatform/Web/handler/Test.ashx',
    //    {
    //        async: false
    //    });
    
    $.post(actionUrl, function (res) {
        if (res.Success == false) {
            $('div#not_subscribe_region').show();
            $('div#bind_region, div#info_region').hide();
        } else if(res.Binded==false){
            $('div#bind_region').show();
            $('div#not_subscribe_region, div#info_region').hide();
        } else {
            $('#user_photo').attr('src', res.Image);
            $('#nick_name').text(res.Nickname);

            $('div#info_region').show();
            $('div#not_subscribe_region, div#bind_region').hide();
        }
    }, 'json');
    
    //绑定用户
    $('a#bind').click(function () {
        var name = $.trim($('input#Name').val());
        var gender = $('input#Gender').prop('checked') == true ? '男' : '女';
        var telphone = $.trim($('input#Telphone').val());
        //var vcode = $.trim($('input#Vcode').val());
        var password = $.trim($('input#Password').val());
        var repassword = $.trim($('input#Repassword').val());

        if (name == '') {
            $('input#Name').focus();
            alert('请输入姓名。');

            return;
        }

        if (telphone == '') {
            $('input#Telphone').focus();
            alert('请输入电话。')

            return;
        }

        //if (vcode == '') {
        //    $('input#Vcode').focus();
        //    alert('请输入验证码。')

        //    return;
        //}

        if (password == '') {
            $('input#Password').focus();
            alert('请输入密码。');

            return;
        }

        if (password != repassword) {
            $('input#Repassword').focus();
            alert('两次输入的密码不一致。请重新输入。');

            return;
        }

        $.getJSON(actionUrl, {
            action: 'bind',
            name: name,
            gender: gender,
            telphone: telphone,
            //Vcode: vcode,
            password: password
        }, function (json) {
            if (json.Success) {
                alert('用户绑定成功。');

                location.reload();
            }else{
                alert(json.Message);
            }
        });
    });

    //解除绑定
    $('a#unbind').click(function () {
        if (confirm('确认解除用户绑定吗？')) {
            $.getJSON(actionUrl, { action: 'unbind' }, function (json) {
                if (json.Success) {
                    alert('已经解除绑定。');

                    $('div#bind_region').show();
                    $('div#not_subscribe_region, div#info_region').hide();
                } else {
                    alert('解除绑定时发生错误。！');
                }
            });
        }
    });






    //$('input#Telphone').change(validateTelphone);

    
});

function validateTelphone() {
    var tel = $.trim($('input#Telphone').val());
    if (tel.length == 11) {
        $('a#GetVcode').removeClass('weui_btn_disabled');
        $('a#GetVcode').one('click', getVcode);
    } else {
        $('a#GetVcode').addClass('weui_btn_disabled');
        $('a#GetVcode').unbind('click');
    }
}

function getVcode() {
    $('a#GetVcode').addClass('weui_btn_disabled');

    $.getJSON(actionURL, { action: 'vcode', Telphone: $.trim($('input#Telphone').val()) }, function (json) {
        if (json.Success == 0) {
            countdownvalue = 30;
            countdown();

            alert('验证码已发送至您的手机。请注意查收。');
        } else {
            alert('获取验证码错误！');
        }
    });
}

function countdown() {
    if (countdownvalue == 0) {
        $('a#GetVcode').html('获取验证码');
        
        validateTelphone();
    } else {
        $('a#GetVcode').html(countdownvalue + '秒后重新获取');
        countdownvalue--;

        setTimeout('countdown()', 1000);
    }
}