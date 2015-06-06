//系统全局设置
var _data = {
    money: [{ "title": "0元", "name": "0" },
        { "title": "50元", "name": "50" },
        { "title": "100元", "name": "100" },
        { "title": "200元", "name": "200" },
        { "title": "500元 ", "name": "500" },
        { "title": "1000元", "name": "1000" }]
};

function initCtrl() {
    $('#txt_Money').combobox({
        data: _data.money, panelHeight: 'auto', editable: false, valueField: 'name', textField: 'title'
    });

    if (config) {
        $('#txt_Money').combobox('setValue', config.money);
        $('#txt_Other').val(config.other);
    }
}

$(function () {
    initCtrl();

    $('#btnok').click(saveConfig);
    $('body').css('overflow', 'auto');
});

function saveConfig() {
    var money = $('#txt_Money').combobox('getValue');
    var other = $('#txt_Other').val();

    var configObj = { money: money, other: other };

    var str = JSON.stringify(configObj);

    $.ajaxtext('ashx/WasherSettingHandler.ashx', 'json=' + str, function (d) {
        if (d == 1)
            msg.ok('恭喜，全局设置保存成功,按F5看效果');
        else
            alert(d);
    });
}