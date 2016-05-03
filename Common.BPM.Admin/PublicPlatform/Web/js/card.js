var actionURL = '/PublicPlatform/Web/handler/CardHandler.ashx';
var countdownvalue = 30;

$(document).ready(function () {
    $('a#prebind').click(function () {
        $('div#not_exist').hide();
        $('div#bind_card').show();
    });

    $('input#CardNo').change(validateCardNo);

    $('a#bind').click(function () {
        var cardNo = $.trim($('input#CardNo').val());
        var password = $.trim($('input#Password').val());
        var departmentId = parseInt($('input#DepartmentId').val());
        var binderId = parseInt($('input#ConsumeId').val());

        if (cardNo == '') {
            alert('请输入卡号。');
            $('input#CardNo').focus();
            return;
        }

        if (password == '') {
            alert('请输入卡密码。');
            $('input#Password').focus();
            return;
        }

        var o = {
            action: 'bind',
            keyId: 0,
            jsonEntity: JSON.stringify({
                CardNo: cardNo,
                DepartmentId: departmentId,
                BinderId: binderId,
                Password: password
            })
        };
        o = "json=" + JSON.stringify(o);
        $.getJSON(actionURL + '?' + o, function (json) {
            if (json.success == 0) {
                alert('已绑定洗车卡。');

                document.forms[0].submit();
            } else if (json.success == 1) {
                alert('卡号错误。请重新输入。');
            } else if (json.success == 2) {
                alert('密码错误。请重新输入。');
            } else if (json.success == 3) {
                alert('您输入的卡号已经被其它用户绑定。');
            } else if (json.success == 4) {
                alert('该洗车卡已过期。');
            } else {
                alert('其它错误。');
            }
        });
    });

    $('a#unbind').click(function () {
        if (confirm('确认解除与这张洗车卡的绑定吗？')) {
            var o = {
                action: 'unbind',
                keyId: parseInt($('input#CardId').val())
            };
            o = "json=" + JSON.stringify(o);
            $.getJSON(actionURL + '?' + o, function (json) {
                if (json.success == 0) {
                    alert('已解除与洗车卡的绑定');
                    document.forms[0].submit();
                } else {
                    alert('解除绑定时发生错误。');
                }
            });
        }
    });
});

function validateCardNo() {
    var cno = $.trim($('input#CardNo').val());
    if (cno.length > 0) {
        $('a#GetPassword').removeClass('weui_btn_disabled');
        $('a#GetPassword').unbind('click');
        $('a#GetPassword').one('click', getPassword);
    } else {
        $('a#GetPassword').addClass('weui_btn_disabled');
        $('a#GetPassword').unbind('click');
    }
}

function getPassword() {
    var o = {
        action: 'password',
        keyId: 0,
        jsonEntity: JSON.stringify({
            CardNo: $.trim($('input#CardNo').val()),
            DepartmentId: parseInt($.trim($('input#DepartmentId').val())),
            BinderId: parseInt($.trim($('input#ConsumeId').val()))
        })
    };
    o = "json=" + JSON.stringify(o);
    $.getJSON(actionURL + '?' + o, function (json) {
        if (json.success == 0) {
            countdownvalue = 30;
            countdown();

            alert('密码已发送至您的手机。请注意查收。');

            $('a#GetPassword').addClass('weui_btn_disabled');

            return;
        } else if (json.success == 1) {
            alert('卡号错误。请重新输入。');
        } else if (json.success == 2) {
            alert('您输入的卡号已经被其它用户绑定。');
        } else if (json.success == 3) {
            alert('该洗车卡已过期。');
        } else {
            alert('其它错误。');
        }

        validateCardNo();
    });
}

function countdown() {
    if (countdownvalue == 0) {
        $('a#GetPassword').html('获取验证码');

        validateCardNo();
    } else {
        $('a#GetPassword').html(countdownvalue + '秒后重新获取');
        countdownvalue--;

        setTimeout('countdown()', 1000);
    }
}