var actionURL = '/Washer/ashx/WasherOrderHandler.ashx';

$(function () {

    autoResize({ dataGrid: '#list', gridType: 'datagrid', callback: grid.bind, height: 0 });

    //高级查询
    $('#a_search').click(function () {
        search.go('list');
    });

    $('#a_export').click(function () {
        var o = { action: 'export', keyid: 0 };
        var query = "json=" + JSON.stringify(o);

        if ($('body').data('where') != null && $('body').data('where') !='') {
            query = query + "&filter=" + $('body').data('where');;
        }

        window.open(actionURL + '?' + query);
    });
});

var grid = {
    bind: function (winSize) {
        $('#list').datagrid({
            url: actionURL,
            toolbar: '#toolbar',
            title: "订单记录",
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
                title: '时间', field: 'Time', width: 180, align: 'center', formatter(v, r, i) {
                    return v.substring(0, 19);
                }
            },{ title: '订单号', field: 'Serial', width: 180, align: 'center' },            
            { title: '付款方', field: 'Name', width: 100, align: 'center' },
		    { title: '类型', field: 'Kind', width: 100, align: 'center' },
            {
                title: '支付金额', field: 'Money', width: 90, align: 'right', formatter: function (v, r, i) {
                    return '￥' + v.toFixed(2);
                }
            },
            { title: '订单状态', field: 'Status', width: 90, align: 'center' },
            { title: '微信账单', field: 'TransactionId', width: 250, align: 'center' },
            { title: '备注', field: 'Memo', width: 200 },
            { title: '所属客户', field: 'DepartmentName', width: 200, align: 'center' }
            ]],
            pagination: true,
            pageSize: PAGESIZE,
            pageList: [20, 40, 50],
            sortName: 'Time',
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