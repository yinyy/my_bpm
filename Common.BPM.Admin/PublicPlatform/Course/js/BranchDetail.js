var handler = "ashx/BranchHandler.ashx";

function loadDetail(id) {
    $.getJSON(handler, { 'action': 'detail', 'id': id }, function (data) {
        if (!data.Success) {
            weui.alert('获取数据失败。', {
                title: '提示'
            });
        } else {
            $('#detail').html('');
            data = data.Data; 
            var html = template('tpl', data);
            $('#detail').html(html);
        }
    })
}