
$(function () {

    autoResize({ dataGrid: '#list', gridType: 'datagrid', callback: Grid.bind, height: 0 });

    $('#a_add').click(GridCrud.add);
    $('#a_edit').click(GridCrud.edit);
    $('#a_delete').click(GridCrud.del);

    //高级查询
    $('#a_search').click(function () {
        search.go('list');
    });
});

var Grid = {
    bind: function (winSize) {
        $('#list').datagrid({
            url: GridCrud.handler,
            toolbar: '#toolbar',
            title: "专业方向",
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
                { title: '用户标签ID', field: 'id', width: 150, align: 'center' },
                { title: '用户标签名称', field: 'name', width: 400, align: 'center' },
                { title: '此标签下的粉丝数据', field: 'count', width: 200, align: 'center' }
            ]],
            pagination: true,
            pageSize: PAGESIZE,
            pageList: [20, 40, 50],
            sortName: 'name',
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


var GridCrud = {
    handler: '/Platform/ashx/PlatformUserTagHandler.ashx',
    forms: {
        edit: '/Platform/html/PlatformUserTagEdit.html'
    },
    add: function () {
        var hDialog = top.jQuery.hDialog({
            title: '添加', width: 460, height: 120, href: GridCrud.forms.edit, iconCls: 'icon-add', submit: function () {
                if (top.$('#uiform').form('validate')) {
                    var query = createParam('add', '0');
                    jQuery.ajaxjson(GridCrud.handler, query, function (d) {
                        if (parseInt(d) > 0) {
                            msg.ok('添加成功！');
                            hDialog.dialog('close');
                            Grid.reload();
                        } else {
                            MessageOrRedirect(d);
                        }
                    });
                }
                return false;
            }, onLoad: function () {
            }
        });

        top.$('#uiform').validate();
    },
    edit: function () {
        var row = Grid.getSelectedRow();
        if (row) {
            var hDialog = top.jQuery.hDialog({
                title: '编辑', width: 460, height: 120, href: GridCrud.forms.edit, iconCls: 'icon-edit',
                onLoad: function () {
                    top.$('#txt_Name').val(row.name);
                },
                submit: function () {
                    if (top.$('#uiform').form('validate')) {
                        var query = createParam('edit', row.id);;
                        jQuery.ajaxjson(GridCrud.handler, query, function (d) {
                            if (parseInt(d) > 0) {
                                msg.ok('修改成功！');
                                hDialog.dialog('close');
                                Grid.reload();
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
        var row = Grid.getSelectedRow();
        if (row) {
            if (confirm('确认删除当前的用户标签吗？')) {
                jQuery.ajaxjson(GridCrud.handler, createParam('del', row.id), function (d) {
                    if (parseInt(d) > 0) {
                        msg.ok('删除成功！');
                        Grid.reload();
                    } else {
                        MessageOrRedirect(d);
                    }
                });
            }
        } else {
            msg.warning('请选择要删除的行。');
        }
    }
}