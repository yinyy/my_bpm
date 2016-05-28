var actionURL = '/Washer/ashx/WasherDeviceLogHandler.ashx';

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
            title: "洗车卡列表",
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
                title: '消费者', field: 'ConsumeName', width: 100, align: 'center'
            },
            {
                title: '洗车时间', field: 'Started', width: 250, align: 'center', formatter(v, r, i) {
                    var str = r.Started.substring(0, 19);
                    if (r.Ended != null) {
                        str += ' - ' + r.Ended.substring(11, 19);
                    }

                    return str;
                }
            },
		    { title: '洗车机序列号', field: 'SerialNumber', width: 150, align: 'center' },
            {
                title: '洗车地点', field: 'Address', width: 220, align: 'center', formatter: function (v, r, i) {
                    if (r.Province == '') {
                        return '未安装';
                    } else {
                        return r.Province + ' - ' + r.City + ' - ' + r.Region + '<br/>' + r.Address;
                    }
                }
            },
            {
                title: '洗车币', field: 'PayCoins', width: 100, align: 'right', formatter: function (v, r, i) {
                    if (v == null) {
                        return '';
                    } else {
                        return (v/100.0).toFixed(2);
                    }
                }
            },
            { title: '支付方式', field: 'Kind', width: 150, align: 'center' },
            { title: '支付卡号', field: 'CardNo', width: 150, align: 'center' },
            { title: '支付凭证', field: 'Ticks', width: 150, align: 'center' },
            { title: '所属客户', field: 'DepartmentName', width: 200, align: 'center' }
            ]],
            pagination: true,
            pageSize: PAGESIZE,
            pageList: [20, 40, 50],
            sortName: 'Started',
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