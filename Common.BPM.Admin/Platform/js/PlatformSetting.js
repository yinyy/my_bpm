var handler = 'ashx/PlatformSettingHandler.ashx';

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
    var query = createParam('js', 0);
    $.getJSON(handler, query, function (data) {
        if (data.Success) {
            $(data.Data).each(function (idx, d) {
                $('#txt_' + d.Keyword).val(d.Value);
            });
        }
    });

    $('#btnok').click(function () {
        var datas = [];
        $('div.c > ul > li > input:text').each(function (idx, d) {
            datas[datas.length] = {
                Keyword: $(d).attr('name'),
                Value: $.trim($(d).val())
            };
        });
        $('div.c > ul > li > textarea').each(function (idx, d) {
            datas[datas.length] = {
                Keyword: $(d).attr('name'),
                Value: $.trim($(d).val())
            };
        });

        var query = createParam('save', 0, datas);
        $.post(handler, query, function (data) {
            if (data.Success) {
                msg.ok('设置保存成功。');
            } else {
                msg.warning('设置保存失败。');
            }
        }, 'json');
    });
});