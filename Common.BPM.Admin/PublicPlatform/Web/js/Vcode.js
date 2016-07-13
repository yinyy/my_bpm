function Vcode(button, input) {
    this.button = $(button);
    this.input = $(input);
    
    this.input.change(function () {
        if ($(button).text() == '获取验证码') {
            var tel = $.trim($(input).val());
            if (tel.length == 11) {
                $(button).removeClass('weui_btn_disabled');
            } else {
                $(button).addClass('weui_btn_disabled');
            }
        }
    });

    this.button.click(function () {
        var css = $(button).attr("class");
        if (css.indexOf('weui_btn_disabled') >= 0) {
            //不作处理
        } else {
            $(button).toggleClass('weui_btn_disabled');
            $.getJSON('/PublicPlatform/Web/handler/VcodeHandler.ashx', {telphone: $.trim($(input).val())}, function (json) {
                if (json.Success) {
                    alert('验证码已发送至您的手机。请注意查收。');

                    var countdownvalue = 31;
                    function countdown() {
                        countdownvalue--;
                        if (countdownvalue > 0) {
                            $(button).text(countdownvalue + '秒后重新获取');

                            setTimeout(countdown, 1000);
                        } else {
                            $(button).text('获取验证码');

                            var phone = $.trim($(input).val());
                            if (phone.length == 11) {
                                $(button).toggleClass('weui_btn_disabled');
                            }
                        }
                    };
                    countdown();
                } else {
                    $(button).toggleClass('weui_btn_disabled');

                    alert('获取验证码错误！'+json.Message);
                }
            });
        }
    });
}