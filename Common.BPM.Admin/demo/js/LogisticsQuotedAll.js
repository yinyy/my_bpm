var actionURL = '/demo/ashx/LogisticsQuotedHandler.ashx';

$(function () {

    autoResize({ dataGrid: '#list', gridType: 'datagrid', callback: grid.bind, height: 0 });
    $('#a_export').click(function () {
        var ee = new ExportExcel('list', "V_Quoted_Detail");
        ee.go();
    });
    //高级查询
   $('#a_search').click(function () {
        search.go('list');
   });

   $('#a_detail').click(CRUD.view);

});

var grid = {
    bind: function (winSize) {
        $('#list').datagrid({
            url: actionURL + '?' + createParam('all', 0),
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
			{ title: '询价人', field: 'Inquirer_Title', width: 90, align: 'center' },
		    {
		        title: '询价截止时间', field: 'Ended', width: 150, align: 'center'},
		    { title: '货物名称', field: 'Cargo', width: 120 },
		    { title: '柜数', field: 'Amount', width: 45, align: 'right' },
		    { title: '单柜毛重', field: 'Weight', width: 60, align: 'right', formatter: function (value, row, index) { return value + 'KG'; } },
		    { title: '目的港', field: 'Port', width: 150 },
		    { title: 'ETD', field: 'ETD', width: 90, align: 'center'},
		    { title: 'ETA', field: 'ETA', width: 90, align: 'center' },
		    { title: '报价', field: 'Price', width: 75, align: 'right' },
		    { title: '船公司', field: 'Ship', width: 130, align: 'left' },
            { title: 'ETD', field: 'FeedbackETD', width: 90, align: 'center' },
            { title: 'ETA', field: 'FeedbackETA', width: 90, align: 'center' },
            { title: '备注', field: 'FeedbackMemo', width: 200, align: 'left' }
            ]],
            pagination: true,
            pageSize: PAGESIZE,
            pageList: [20, 40, 50],
            sortName: 'Published',
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
    view: function () {
        var row = grid.getSelectedRow();

        if (row) {
            var hDialog = top.jQuery.hDialog({
                title: '查看', width: 700, height: 460, href: '/demo/html/LogisticsInquiryView.html' + '?d=' + (new Date()), iconCls: 'icon-save',
                buttons: [{ text: '关闭', handler: function () { hDialog.dialog('close'); }, iconCls: 'icon-delete2' }],
                onLoad: function () {
                    top.$('#txt_Inquirer').text(row.Inquirer_Title);
                    top.$('#txt_Published').text(row.Published);
                    top.$('#txt_Cargo').text(row.Cargo);
                    top.$('#txt_Amount').text(row.Amount);
                    top.$('#txt_Packing').text(row.Packing_Title);
                    top.$('#txt_Weight').text(row.Weight);
                    top.$('#txt_Port').text(row.Port);
                    top.$('#txt_Etd').text(row.ETD);
                    top.$('#txt_Eta').text(row.ETA);
                    top.$('#txt_Kind').text(row.Kind_Title);
                    top.$('#txt_Ended').text(row.Ended);
                    top.$('#txt_Freebox').text(row.Freebox);
                    top.$('#txt_Memo').text(row.Memo);
                    if ($.trim(row.Attachment) != '') {
                        var fn = row.Attachment.substring(row.Attachment.lastIndexOf('/') + 1);
                        top.$('#txt_Attachment').html('<a href=\"' + row.Attachment + '\" target=\"_blank\">' + fn.substring(fn.indexOf('_') + 1) + '</a>');
                    }
                    top.$('#txt_SupplyNames').parent().parent().hide();
                },
                submit: function () {
                    hDialog.dialog('close');
                    return false;
                }
            });
        } else {
            msg.warning('请选择要查看详情的记录。');
        }
    }
}