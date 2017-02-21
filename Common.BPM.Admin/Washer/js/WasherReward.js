var actionURL = '/Washer/ashx/WasherRewardHandler.ashx';

$(function () {
    autoResize({ dataGrid: '#list', gridType: 'datagrid', callback: grid.bind, height: 0 });

    $('#a_delete').click(CRUD.del);
    $('#a_export').click(function () {
        var o = { action: 'export', keyid: 0 };
        var query = "json=" + JSON.stringify(o);

        if ($('body').data('where') != null && $('body').data('where') != '') {
            query = query + "&filter=" + $('body').data('where');
        }

        window.open(actionURL + '?' + query);
    });

    //高级查询
    $('#a_search').click(function () {
        search.go('list');
    });
});

var grid = {
    bind: function (winSize) {
        $('#list').datagrid({
            url: actionURL,
            toolbar: '#toolbar',
            title: "积分列表",
            iconCls: 'icon icon-list',
            width: winSize.width,
            height: winSize.height,
            nowrap: false, //折行
            rownumbers: true, //行号
            striped: true, //隔行变色
            idField: 'KeyId',//主键
            singleSelect: true, //单选
            frozenColumns: [[]],
            columns: [[
		    { title: '会员姓名', field: 'Name', width: 120, align: 'center' },
            { title: '积分类型', field: 'Kind', width: 200, align: 'center' },
            { title: '积分', field: 'Points', width: 60, align: 'right' },
            { title: '获取时间', field: 'Time', width: 150, align: 'center' },
            { title: '已用积分', field: 'Used', width: 60, align: 'right' },
            { title: '详细信息', field: 'Memo', width: 600, align: 'left' },
            { title: '运营商', field: 'DepartmentName', width: 100, align: 'center' }
            ]],
            pagination: true,
            pageSize: PAGESIZE,
            pageList: [20, 40, 50],
            sortName: 'KeyId',
            sortOrder: 'desc'
        });
    },
    getSelectedRow: function () {
        return $('#list').datagrid('getSelected');
    },
    reload: function () {
        $('#list').datagrid('clearSelections').datagrid('reload', { filter: '' });
    }
};

function createParam(action, keyid) {
    var o = {};
    var query = top.$('#uiform').serializeArray();
    query = convertArray(query);
    o.jsonEntity = JSON.stringify(query);
    o.action = action;
    o.keyid = keyid;
    return "json=" + JSON.stringify(o);
}


var CRUD = {
    del: function(){
        var row = grid.getSelectedRow();
        if (row) {
            if (confirm('确认删除选中的会员积分吗？')) {
                var rid = row.KeyId;
                jQuery.ajaxjson(actionURL, createParam('del', rid), function (d) {
                    if (parseInt(d) > 0) {
                        msg.ok('删除成功！');
                        grid.reload();
                    } else {
                        MessageOrRedirect(d);
                    }
                });
            }
        } else {
            msg.warning('请选择记录。');
        }
    }
};