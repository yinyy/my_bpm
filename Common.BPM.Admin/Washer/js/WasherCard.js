var actionURL = '/Washer/ashx/WasherCardHandler.ashx';
var formurl = '/Washer/html/WasherCard.html';
var editformurl = '/Washer/html/WasherCardEdit.html';

$(function () {

    autoResize({ dataGrid: '#list', gridType: 'datagrid', callback: grid.bind, height: 0 });

    $('#a_add').click(CRUD.add);

    //高级查询
    $('#a_search').click(function () {
        search.go('list');
    });
});

var grid = {
    bind: function (winSize) {
        $('#list').datagrid({
            url: actionURL+'?'+createParam('list2',0),
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
		    { title: '卡号', field: 'Serial', width: 150, align: 'center' },
		    {
		        title: '客户姓名', field: 'Name', width: 150, align: 'center', formatter: function (v, r, i) {
		            return v + '[' + r.Card.substring(r.Card.length - 4) + ']';
		        }},
		    {
		        title: '当前余额', field: 'Money', width: 100, align: 'right', formatter: function (v, r, i) {
		            return '￥' + v.toFixed(2);
		        }},
		    {
		        title: '状态', field: 'Status', width: 80, align: 'center', formatter: function (v, r, i) {
		            if (v == 0) {
		                return '<span>正常</span>';
		            } else if (v == 1) {
		                return '<span style="color:#ff0000">已挂失</a>';
		            } else {
		                return '<span>未知</span>';
		            }
		        }
		    },
            { title: '备注', field: 'Memo', width: 500, align: 'left' }]],
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
    add: function () {
        var hDialog = top.jQuery.hDialog({
            title: '添加', width: 450, height: 283, href: formurl, iconCls: 'icon-add', submit: function () {
                if (top.$('#uiform').form('validate')) {
                    var query = createParam('add', '0');
                    jQuery.ajaxjson(actionURL, query, function (d) {
                        if (parseInt(d) > 0) {
                            msg.ok('添加成功！');
                            hDialog.dialog('close');
                            grid.reload();
                        } else {
                            MessageOrRedirect(d);
                        }
                    });
                }
                return false;
            }, onLoad: function () {
                top.$('#txt_CustomId').combobox({   
                    url:actionURL+'?'+createParam('cstm', 0),   
                    valueField:'KeyId',   
                    textField:'Name'  
                });
            }
        });

        top.$('#uiform').validate();
    }
};