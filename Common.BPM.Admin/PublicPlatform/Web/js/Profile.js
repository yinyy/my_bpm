var actionUrl= '/PublicPlatform/Web/handler/ProfileHandler.ashx';

var vcode;

$(document).ready(function () {
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
        var vcode = $.trim($('input#Vcode').val());
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

        if (vcode == '') {
            $('input#Vcode').focus();
            alert('请输入验证码。')

            return;
        }

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
            vcode: vcode,
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

    vcode = new Vcode($('a#GetVcode'), $('input#Telphone'));
});