var actionURL = '/PublicPlatform/Web/Card.aspx';

$(document).ready(function () {
    $('a#prebind').click(function () {
        $('div#card_list').hide();
        $('div#bind_card').show();
    });

    $('a#bind').click(function () {
        var cardNo = $.trim($('input#CardNo').val());
        var password = $.trim($('input#Password').val());

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

        $.getJSON(actionURL, { action: 'bind', wxid: $('#wxid').val(), CardNo: cardNo, Password: password }, function (json) {
            switch (json.Success) {
                case 0:
                    alert('已绑定洗车卡。');
                    document.forms[0].submit();
                    break;
                case -1:
                    alert('卡号错误。请重新输入。');
                    break;
                case -2:
                    alert('密码错误。请重新输入。');
                    break;
                case -3:
                    alert('您输入的卡号已经被绑定。');
                    break;
                case -4:
                    alert('该洗车卡已过期。');
                    break;
                default:
                    alert('其它错误。');
                    break;;
            }
        });
    });
});