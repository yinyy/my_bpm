var actionURL = '/FivePower/ashx/FivePowerProductHandler.ashx';

$(function () {
    autoResize({ dataGrid: '#list', gridType: 'datagrid', callback: grid.bind, height: 0 });

    $('#a_add').click(CRUD.add);
    $('#a_edit').click(CRUD.edit);
    $('#a_delete').click(CRUD.del);
    $('#a_batch_create').click(CRUD.batch);
    $('#a_export').click(CRUD.exp);

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
            title: "已安装设备列表",
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
                    title: '安装时间', field: 'InstallTime', width: 150, align: 'center', formatter: function (v, r, i) {
                        return v.substring(0, 4) + '年' + v.substring(5, 7) + '月' + v.substring(8, 10) + '日';
                    }
                },
                { title: '安装地点', field: 'Address', width: 300, align: 'center' },
                { title: '车主', field: 'Owner', width: 120, align: 'center' },
                { title: '联系方式', field: 'Phone', width: 120, align: 'center' },
                {
                    title: '车辆信息', field: 'Type', width: 200, align: 'center', formatter: function (v, r, i) {
                        return r.Type + ' - ' + r.Plate + '<br/>已行驶：' + r.Driving + '公里';
                    }
                },
                {
                    title: '故障信息', field: 'Trouble', width: 300, align: 'center', formatter: function (v, r, i) {
                        return r.Trouble==true?r.Detail:'无';
                    }
                },
                {
                    title: '设备信息', field: 'Model', width: 280, align: 'center', formatter: function (v, r, i) {
                        return r.Model + ' - ' + r.Serial;
                    }
                },
                {
                    title: '质保', field: 'FinishedTime', width: 180, align: 'center', formatter: function (v, r, i) {
                        return r.FinishedTime.substring(0, 4) + '年' + r.FinishedTime.substring(5, 7) + '月' + r.FinishedTime.substring(8, 10) + '日' + '<br/>或<br/>' + r.FinishedDriving + '公里';
                    }
                }
            ]],
            pagination: true,
            pageSize: PAGESIZE,
            pageList: [20, 40, 50],
            sortName: 'InstallTime',
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