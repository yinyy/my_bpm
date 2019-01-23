var handler = "ashx/BranchHandler.ashx";

$(function () {
    $.getJSON(handler, { 'action': 'list' }, function (data) {
        if (!data.Success) {
            weui.alert('获取数据失败。', {
                title: '提示'
            });
        } else {
            $('#list').html('');
            data = data.Data;
            $.each(data, function (idx, d) {
                var html = template('tpl', d);
                $('#list').html($('#list').html() + html);
            });
        }
    })
});

////检查授权
//    $.getJSON(handler, {'action': 'oauth'}, function (data) {
//        if (!data.Success) {
//            //没有授权，则请求授权
//            document.location.href = data.Url;
//        } else {
//            //获得全部专业方向并显示
//            loadBranches();
//        }
//    })
function loadBranches() {
    
}