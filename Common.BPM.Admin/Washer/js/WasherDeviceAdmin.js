var actionURL = '/Washer/ashx/WasherDeviceHandler.ashx';
var formurl = '/Washer/html/WasherDeviceAdd.html';

$(function () {

    autoResize({ dataGrid: '#list', gridType: 'datagrid', callback: grid.bind, height: 0 });

    $('#a_add').click(CRUD.add);
    $('#a_edit').click(CRUD.edit);
    $('#a_delete').click(CRUD.del);

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
            title: "设备列表",
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
		    {title: '序列号', field: 'Serial', width: 150, align: 'center'},
		    {title: '所属客户', field: 'DepartmentName', width: 200, align: 'center'},
		    {title: '备注', field: 'Memo', width: 500, align: 'left'}
            ]],
            pagination: true,
            pageSize: PAGESIZE,
            pageList: [20, 40, 50],
            sortName: 'Serial',
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
            title: '添加', width: 450, height: 248, href: formurl, iconCls: 'icon-add', submit: function () {
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
                top.$('#txt_BelongTo').combobox({
                    url: actionURL + '?' + createParam('dept', 0),
                    valueField: 'KeyId',
                    textField: 'DepartmentName'
                });
            }
        });

        top.$('#uiform').validate();
    },
    edit: function () {
        var row = grid.getSelectedRow();
        if (row) {
            var hDialog = top.jQuery.hDialog({
                title: '编辑', width: 450, height: 248, href: formurl, iconCls: 'icon-save',
                onLoad: function () {
                    top.$('#txt_BelongTo').combobox({
                        url: actionURL + '?' + createParam('dept', 0),
                        valueField: 'KeyId',
                        textField: 'DepartmentName'
                    });

                    top.$('#txt_Serial').val(row.Serial);
                    top.$('#txt_BelongTo').combobox('setValue', row.DepartmentId);
                    top.$('#txt_Memo').val(row.Memo);
                },
                submit: function () {
                    if (top.$('#uiform').form('validate')) {
                        var query = createParam('edit', row.KeyId);;
                        jQuery.ajaxjson(actionURL, query, function (d) {
                            if (parseInt(d) > 0) {
                                msg.ok('修改成功！');
                                hDialog.dialog('close');
                                grid.reload();
                            } else {
                                MessageOrRedirect(d);
                            }
                        });
                    }
                    return false;
                }
            });

        } else {
            msg.warning('请选择要修改的行。');
        }
    },
    del: function () {
        var row = grid.getSelectedRow();
        if (row) {
            if (confirm('确认要执行删除操作吗？')) {
                var rid = row.KeyId;
                jQuery.ajaxjson(actionURL, createParam('del', rid), function (d) {
                    if (parseInt(d) > 0) {
                        msg.ok('删除成功！');
                        grid.reload();
                    } else {
                        MessageOrRedirect(d);
                    }
                });
            }
        } else {
            msg.warning('请选择要删除的行。');
        }
    }
};