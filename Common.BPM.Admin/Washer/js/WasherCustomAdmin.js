var actionURL = '/Washer/ashx/WasherCustomHandler.ashx';
var formurl = '/Washer/html/WasherCustom.html';

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
            url: actionURL,
            toolbar: '#toolbar',
            title: "客户列表",
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
		        title: '姓名', field: 'Name', width: 150, align: 'center'
		    },
		    { title: '性别', field: 'Gender', width: 60, align: 'center'},
		    {
		        title: '身份证号', field: 'Card', width: 200, align: 'center'
		    },
            {
                title: '记录', field: 'KeyId', width: 150, align: 'center', formatter: function (v, r, i) {
                    return '<a href="javascript:void(0);" onclick="CRUD.charge(' + r.KeyId + ');">充值记录</a>&nbsp;&nbsp;<a href="javascript:void(0);" onclick="CRUD.consume(' + r.KeyId + ');">消费记录</a>';
                }
            },
		    {
		        title: '备注', field: 'Memo', width: 500, align: 'left'
		    }
            ]],
            pagination: true,
            pageSize: PAGESIZE,
            pageList: [20, 40, 50],
            sortName: 'Name',
            sortOrder: 'asc'
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
    charge: function (rid) {
        $('#win').window({
            width: 780,
            height: 600,
            modal: true,
            collapsible: false,
            minimizable: false,
            maximizable: false,
            title: '客户充值记录'
        });

        $('#tt').treegrid({
            url: actionURL + '?' + createParam('recharge', rid),
            idField: 'KeyId',
            treeField: 'CardSerial',
            animate: true,
            maximized: true,
            columns: [[
                { title: '卡号', field: 'CardSerial', width: 150 },
                { field: 'Time', title: '充值时间', width: 160, align: 'center' },
                { field: 'Serial', title: '订单编号', width: 200, align: 'center' },
                {
                    field: 'Money', title: '充值金额', width: 100, align: 'right', formatter: function (v, r, i) {
                        return '￥' + v.toFixed(2);
                    }
                },
                { field: 'Way', title: '充值途径', width: 120, align: 'center' },
            ]]
        });
    },
    consume: function (rid) {
        $('#win').window({
            width: 660,
            height: 600,
            modal: true,
            collapsible: false,
            minimizable: false,
            maximizable: false,
            title: '客户消费记录'
        });

        $('#tt').treegrid({
            url: actionURL + '?' + createParam('consume', rid),
            idField: 'KeyId',
            treeField: 'CardSerial',
            animate: true,
            maximized: true,
            columns: [[
                { title: '卡号', field: 'CardSerial', width: 150 },
                { field: 'Time', title: '消费时间', width: 160, align: 'center' },
                { field: 'DeviceAddress', title: '消费地点', width: 200, align: 'center' },
                {
                    field: 'Money', title: '消费金额', width: 100, align: 'right', formatter: function (v, r, i) {
                        return '￥' + v.toFixed(2);
                    }}
            ]]
        });
    }
}