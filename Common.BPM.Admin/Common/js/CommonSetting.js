var handler = 'ashx/CommonSettingHandler.ashx';

function createParam(action, keyid, obj) {
    var o = {};
    if (obj != null) {
        o.jsonEntity = JSON.stringify(obj);
    }
    o.action = action;
    o.keyid = keyid;
    return "json=" + JSON.stringify(o);
}

$(function () {
    var query = createParam('js', '0');
    $.getJSON(handler, query, function (data) {
        if (data.Success) {
            data = data.Data;

            $('#txt_AppId').val(data.AppId);
            $('#txt_AppSecret').val(data.AppSecret);
            $('#txt_Token').val(data.Token);
            $('#txt_EncodingAESKey').val(data.EncodingAESKey);
        }
    });

    $('#btnok').click(function () {
        var o = {
            'AppId': $('#txt_AppId').val(),
            'AppSecret': $('#txt_AppSecret').val(),
            'Token': $('#txt_Token').val(),
            'EncodingAESKey': $('#txt_EncodingAESKey').val()
        };

        var query = createParam('save', '0', o);
        $.post(handler, query, function (data) {
            if (data.Success) {
                msg.ok('设置保存成功。');
            } else {
                msg.warning('设置保存失败。');
            }
        }, 'json');
    });
});