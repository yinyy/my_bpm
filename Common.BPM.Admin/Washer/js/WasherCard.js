var actionURL = '/Washer/ashx/WasherCardHandler.ashx';
var formURL = '/Washer/html/WasherCard.html';

function showPassword(o) {
    if (window.event.ctrlKey == true) {
        alert($(o).attr('p'));
    }
}

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
		    { title: '运营商', field: 'DepartmentName', width: 200, align: 'center' },
            { title: '卡号', field: 'CardNo', width: 200, align: 'center' },
            {
                title: '密码', field: 'Password', width: 80, align: 'center', formatter(v, r, i) {
                    return '<span onclick="showPassword(this);" p="' + v + '">******</span>';
                }
            },
            {
                title: '有效期', field: 'ValidateFrom', width: 200, align: 'center', formatter(v, r, i) {
                    return r.ValidateFrom.substring(0, 10) + ' 至 ' + r.ValidateEnd.substring(0, 10);
                }
            },
            {
                title: '可用余额', field: 'Coins', width: 80, align: 'right', formatter(v, r, i) {
                    return v.toFixed(2);
                }
            },
            {
                title: '持卡人', field: 'BinderId', width: 80, align: 'center', formatter(v, r, i) {
                    if (v == null) {
                        return '未绑定';
                    } else {
                        return r.Name;
                    }
                }
            },
            {title: '状态', field: 'Status', width: 80, align: 'center'}
            ]],
            pagination: true,
            pageSize: PAGESIZE,
            pageList: [20, 40, 50],
            sortName: 'KeyId',
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
            title: '添加', width: 450, height: 252, href: formURL, iconCls: 'icon-add', submit: function () {
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
                top.$('#txt_ValidateFrom').datebox({
                    required: true,
                    editable: false
                });
                top.$('#txt_ValidateEnd').datebox({
                    required: true,
                    editable: false
                });
            }
        });

        top.$('#uiform').validate();
    }
};