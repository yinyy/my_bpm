var actionURL = '/Washer/ashx/WasherCardLogHandler.ashx';

$(function () {

    autoResize({ dataGrid: '#list', gridType: 'datagrid', callback: grid.bind, height: 0 });

    //高级查询
    $('#a_search').click(function () {
        search.go('list');
    });
    //导出
    $('#a_export').click(function () {
        var o = { action: 'export', keyid: 0 };
        var query = "json=" + JSON.stringify(o);

        if ($('body').data('where') != null && $('body').data('where') != '') {
            query = query + "&filter=" + $('body').data('where');
        }

        window.open(actionURL + '?' + query);
    });
});

var grid = {
    bind: function (winSize) {
        $('#list').datagrid({
            url: actionURL + '?' + "json=" + JSON.stringify({ action: 'cost'}),
            toolbar: '#toolbar',
            title: "洗车卡消费记录",
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
		    {
		        title: '刷卡时间', field: 'Time', width: 180, align: 'center', formatter(v, r, i) {
		            return v.substring(0, 19);
		        }},
            { title: '持卡人', field: 'Name', width: 120, align: 'center' },
            { title: '卡号', field: 'CardNo', width: 200, align: 'center' },
            { title: '卡类型', field: 'Kind', width: 150, align: 'center'},
            {
                title: '消费（洗车币）', field: 'Coins', width: 105, align: 'right', formatter(v, r, i) {
                    return (v / 100.0).toFixed(2);
                }
            },
            {
                title: '支付凭证', field: 'Memo', width: 150, align: 'center'
            },
            { title: '所属客户', field: 'DepartmentName', width: 200, align: 'center' }
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