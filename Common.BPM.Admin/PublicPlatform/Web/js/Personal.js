var actionUrl = '/PublicPlatform/Web/handler/Personal.ashx';

$(document).ready(function () {
    $.getJSON(actionUrl, function (json) {
        $('#max_pay_coins').val((json.MaxPayCoins / 100).toFixed(0));
    });

    $('#save_button').click(function () {
        var maxPayCoins = parseInt($('#max_pay_coins').val());
        if (maxPayCoins < 0) {
            $('#dialog2 div.weui_dialog div.weui_dialog_bd').text('支付限额必须大于等于0。');
            $('#dialog2').show();

            return;
        }

        $.getJSON(actionUrl, { action: 'save', max_pay_coins: maxPayCoins }, function (data) {
            if (data > 0) {
                $('#dialog2 div.weui_dialog div.weui_dialog_bd').text('设置保存成功。');
                $('#dialog2').show();
            } else {
                $('#dialog2 div.weui_dialog div.weui_dialog_bd').text('保存失败。');
                $('#dialog2').show();
            }
        });
    });
});