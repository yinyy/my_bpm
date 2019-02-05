var handler = '/PublicPlatform/Exam/ashx/ExamHandler.ashx';

$(function () {
    $.getJSON('/PublicPlatform/ashx/OAuth2Handler.ashx', { 'action': 'openid' }, function (data) {
        if (!data.Success) {
            //获取微信身份认证的链接
            $.getJSON('/PublicPlatform/ashx/OAuth2Handler.ashx', { 'nextUrl': 'http://course.dyzyxyydwlwsys.cc/PublicPlatform/Exam/InvigilateCalendar.aspx' }, function (data) {
                if (data.Success) {
                    $('.weui-tab__panel').html('');
                    var html = template('tpl3', data);
                    $('.weui-tab__panel').html(html);

                    setInterval(function () {
                        var cd = parseInt($('#countdown').text());
                        cd = cd - 1;
                        if (cd == 0) {
                            clearInterval();

                            document.location.href = data.Data;
                        } else {
                            $('#countdown').text(cd);
                        }
                    }, 1000);
                } else {
                    weui.alert('获取数据失败。', {
                        title: '提示'
                    });
                }
            });
        } else {
            loadData();
        }
    });
});

//加载监考记录
function loadData() {
    $.getJSON(handler, { 'action': 'calendar' }, function (data) {
        if (data.Success) {
            if (data.Data.length == 0) {
                $('.weui-tab__panel').html('');
                var html = template('tpl2', {});
                $('.weui-tab__panel').html(html);
            } else {
                $('.weui-tab__panel').html('');
                var html = template('tpl', data);
                $('.weui-tab__panel').html(html);
            }
        } else {
            weui.alert('获取数据失败。', {
                title: '提示'
            });
        }
    });
}

function confirmInvigilate(id) {
    var loading = weui.loading('确认中...');
    $.getJSON(handler, { 'action': 'confirm', 'id': id }, function (data) {
        loading.hide();

        if (data.Success) {
            weui.toast('监考已确认。',
                function () {
                    loadData();
                },
                {
                    duration: 3000,
                    className: "bears"
                });    
        } else {
            weui.alert('确认监考信息失败。', {
                title: '提示'
            });
        }
    });
}