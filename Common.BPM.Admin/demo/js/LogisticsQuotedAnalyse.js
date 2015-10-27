var actionURL = '/demo/ashx/LogisticsQuotedAnalyseHandler.ashx';

$(function () {

    autoResize({ dataGrid: '#list', gridType: 'datagrid', callback: grid.bind, height: 0 });

    $('#a_export').click(function () {
        var ee = new ExportExcel('list', "V_Quoted_Analyse",
            [
            { title: '货代公司', field: 'TrueName' },
            { title: '询盘次数', field: 'Sended' },
            { title: '报价次数', field: 'Replied' },
            { title: '中标次数', field: 'Bidded' }
            ]);
        ee.go();
    });
    $('#a_search').click(function () {
        
    });
});

var grid = {
    bind: function (winSize) {
        $('#list').datagrid({
            url: actionURL,
            toolbar: '#toolbar',
            title: "数据列表",
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
			{ title: '货代公司', field: 'TrueName', width: 300, align: 'left' },
		    {
		        title: '询盘次数', field: 'Sended', width: 100, align: 'center'
		    },
		    { title: '报价次数', field: 'Replied', width: 100, align: 'center' },
            { title: '中标次数', field: 'Bidded', width: 100, align: 'center' }]],
            sortName: 'TrueName',
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