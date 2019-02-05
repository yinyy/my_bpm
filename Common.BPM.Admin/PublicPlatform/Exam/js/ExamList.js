var handler = '/PublicPlatform/Exam/ashx/ExamHandler.ashx';

$(function () {
    $.getJSON(handler, { 'action': 'exam_list' }, function (data) {
        if (!data.Success) {
            weui.alert('获取数据失败。', {
                title: '提示'
            });
        } else {
            data = data.Data;

            if (data.length > 0) {
                $('.weui-tab__panel').html('');

                $.each(data, function (idx, d) {
                    var html = template('tpl', d);
                    $('.weui-tab__panel').html($('.weui-tab__panel').html() + html);
                });
            }
        }
    })
});