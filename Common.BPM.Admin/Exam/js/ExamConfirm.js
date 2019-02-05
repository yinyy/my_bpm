$(function () {
    autoResize({ dataGrid: '#list', gridType: 'datagrid', callback: grid.bind, height: 0 });

    //高级查询
    $('#a_search').click(function () {
        search.go('list');
    });
});

var grid = {
    bind: function (winSize) {
        $('#list').datagrid({
            url: CRUD.handler,
            toolbar: '#toolbar',
            title: "监考确认列表",
            iconCls: 'icon icon-list',
            width: winSize.width,
            height: winSize.height,
            nowrap: false, //折行
            rownumbers: true, //行号
            striped: true, //隔行变色
            idField: 'ExamId',//主键
            singleSelect: true, //单选
            frozenColumns: [[]],
            columns: [[
                { title: '考试名称', field: 'ExamTitle', width: 300, align: 'center' },
                {
                    title: '未确认监考名单', field: 'Data', width: 0, align: 'center', formatter(v, r, idx) {
                        if (v == null || v == '') {
                            return '无';
                        }

                        var ds = [];
                        var datas = v.split('；');
                        $(datas).each(function (idx, d) {
                            ds[ds.length] = '<div style="float: left;padding:5px 10px;">' + d + '</div>';
                        });
                        return ds.join('');
                    }
                },
            ]],
            pagination: true,
            pageSize: PAGESIZE,
            pageList: [20, 40, 50],
            sortName: 'ExamId',
            sortOrder: 'desc'
        });
    },
    getSelectedRow: function () {
        return $('#list').datagrid('getSelected');
    },
    reload: function () {
        $('#list').datagrid('clearSelections').datagrid('reload', { filter: '' });
    }
}

var CRUD = {
    handler: '/Exam/ashx/ExamConfirmHandler.ashx',
}

function createParam(action, keyid) {
    var o = {};
    var query = top.$('#uiform').serializeArray();
    query = convertArray(query);
    o.jsonEntity = JSON.stringify(query);
    o.action = action;
    o.keyid = keyid;
    return "json=" + JSON.stringify(o);
}