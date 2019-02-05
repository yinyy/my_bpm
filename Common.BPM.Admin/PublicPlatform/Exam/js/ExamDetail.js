var handler = '/PublicPlatform/Exam/ashx/ExamHandler.ashx';

$(function () {
    loadData();
});

function loadData() {
    $.getJSON(handler, { 'action': 'section_list', 'id': examId }, function (data) {
        if (!data.Success) {
            weui.alert('获取数据失败。', {
                title: '提示'
            });
        } else {
            $('.page__bd').html('');
            var html = template('tpl', data);
            $('.page__bd').html(html);
        }
    });
}

//Code
//100：表示已经选择，本次无需选择
//101：表示本场监考人数已满
//102：表示本次预约的场次与其它预约的场次有冲突
//103：表示本次预约的场次与其它已经安排的监考冲突
//1：表示成功选择本场次监考
//2：表示成功取消本场次监考
function selectExamSection(examSectionId, action) {
    var loading = weui.loading('保存中');
    $.getJSON(handler, {
        'action':
            action ? 'select' : 'unselect',
        'examSectionId': examSectionId
    }, function (data) {
        loading.hide();
        if (data.Success == true) {
            if (data.Code == 1 || data.Code == 100) {
                weui.toast('监考已预约',
                    function () {
                        loadData();
                    },
                    {
                        duration: 3000,
                        className: "bears"
                    });
            } else if (data.Code == 102) {
                weui.alert('本次预约与其它预约场次有冲突。', {
                    title: '预约失败'
                });
            } else if (data.Code == 103) {
                weui.alert('本次预约与其它已经安排的监考有冲突。', {
                    title: '预约失败'
                });
            } else if (data.Code == 2) {
                weui.toast('监考已取消',
                    function () {
                        loadData();
                    },
                    {
                        duration: 3000,
                        className: "bears"
                    });
            } else if (data.Code == 101) {
                weui.alert('本场次监考人数已满。', {
                    title: '预约失败'
                });
            }
        } else {
            weui.alert(data.Message, {
                title: '操作失败'
            });
        }
    });
};