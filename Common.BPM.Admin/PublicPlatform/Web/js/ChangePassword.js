var actionUrl = '/PublicPlatform/Web/handler/ChangePassword.ashx';

$(document).ready(function () {
    $('.weui_btn_area > .weui_btn:first').click(function () {
        var password = $('input#Password').val();
        var repassword = $('input#RePassword').val();

        if (password == '') {
            $('input#Password').focus();
            alert('请输入密码。');
            return;
        }

        if (password.length < 6) {
            $('input#Password').focus();
            alert('密码长度需大于6位。');
            return;
        }

        if (repassword != password) {
            $('input#Password').focus();
            alert('两次输入的密码不一致。');
            return;
        }

        $.getJSON(actionUrl, { action: 'change', password: password}, function (json) {
            if (json.Success) {
                alert('密码修改成功。');

                document.location.href = './Profile.aspx';
            } else {
                alert(json.Message);
            }
        });
    });
});
