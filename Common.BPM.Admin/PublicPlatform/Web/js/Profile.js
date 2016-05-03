var actionURL = '/PublicPlatform/Web/handler/ProfileHandler.ashx';

var countdownvalue = 30;

$(document).ready(function () {
    $('input#Telphone').change(validateTelphone);

    $('a#bind').click(function () {
        var name = $.trim($('input#Name').val());
        var gender = $('input#Gender').prop('checked')==true ? '男' : '女';
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

        var o = {
            action: 'bind',
            keyId: 0,
            jsonEntity: JSON.stringify({
                BinderId: $.trim($('input#BinderId').val()),
                DepartmentId: parseInt($.trim($('input#DepartmentId').val())),
                Name: name,
                Gender: gender,
                Telphone: telphone,
                Vcode: vcode,
                Password: password
            })
        };
        o = "json=" + JSON.stringify(o);
        $.getJSON(actionURL + '?' + o, function (json) {
            if (json.success == 1) {
                alert('用户信息绑定成功。');

                $('input#Name').val('');
                $('input#Gender').removeProp('checked');
                $('input#Telphone').val('');
                $('input#Vcode').val('');
                $('input#Password').val('');
                $('input#Repassword').val('');

                $('input#ConsumeId').val(json.keyId);

                $('div#content1').hide();
                $('div#content2').show();
            } else {
                alert(json.errmsg);
            }
        });
    });

    $('a#unbind').click(function () {
        if (confirm('确认解除用户绑定吗？')) {
            var o = {
                action: 'unbind',
                keyId: parseInt($.trim($('input#ConsumeId').val()))
            };
            o = "json=" + JSON.stringify(o);
            $.getJSON(actionURL + '?' + o, function (json) {
                if (json.success == 1) {
                    alert('已经解除绑定。');

                    $('div#content1').show();
                    $('div#content2').hide();
                } else {
                    alert('解除绑定时发生错误。！');
                }
            });
        }
    });
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

    var o = {
        action: 'vcode',
        keyId: 0,
        jsonEntity: JSON.stringify({
            Telphone: $.trim($('input#Telphone').val())
        })
    };
    o = "json=" + JSON.stringify(o);
    $.getJSON(actionURL + '?' + o, function (json) {
        if (json.success == 1) {
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