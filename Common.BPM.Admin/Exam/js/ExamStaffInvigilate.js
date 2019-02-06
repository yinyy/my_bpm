$(function () {
    autoResize({ dataGrid: '#list', gridType: 'datagrid', callback: grid.bind, height: 0 });

    $('#a_init').click(CRUD.init);
    $('#a_add').click(CRUD.add);
    $('#a_edit').click(CRUD.edit);
    $('#a_inport').click(CRUD.import);
});

var grid = {
    bind: function (winSize) {
        $('#list').datagrid({
            url: CRUD.handler,
            toolbar: '#toolbar',
            title: "监考员信息",
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
                { title: '工号', field: 'Serial', width: 150, align: 'center' },
                { title: '姓名', field: 'Name', width: 150, align: 'center' },
                { title: '性别', field: 'Gender', width: 100, align: 'center' },
                {
                    title: '监考次数', field: 'Invigilated', width: 150, align: 'center', formatter: function (v, r, i) {
                        if (v == null) {
                            return 0;
                        }

                        return v;
                    }
                },
                {
                    title: '初始化时间', field: 'Created', width: 150, align: 'center', formatter: function (v, r, i) {
                        if (v == null) {
                            return '';
                        }

                        return v.substring(0, 16);
                    } },
                {
                    title: '更新时间', field: 'Updated', width: 150, align: 'center', formatter: function (v, r, i) {
                        if (v == null) {
                            return '';
                        }

                        return v.substring(0, 16);
                    }
                },
                {
                    title: '是否自动安排监考', field: 'AutoArranged', width: 150, align: 'center', formatter: function (v, r, i) {
                        if (v == 1) {
                            return '是';
                        } else {
                            return '否';
                        }
                    }
                },
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
}

var CRUD = {
    handler: '/Exam/ashx/ExamStaffInvigilateHandler.ashx',
    form: {
        edit: '/Exam/html/ExamStaffInvigilateEdit.html',
        import: '/Exam/html/ExamStaffInvigilateImport.html'
    },
    init: function () {
        if (confirm('初始化操作将清除全部监考员的监考次数。确定执行初始化操作吗？')) {
            jQuery.ajaxjson(CRUD.handler, createParam('init', 0), function (d) {
                if (parseInt(d) > 0) {
                    msg.ok('初始化成功！');
                    grid.reload();
                } else {
                    MessageOrRedirect(d);
                }
            });
        }
    },
    add: function () {
        var hDialog = top.jQuery.hDialog({
            title: '新增监考员信息', width: 400, height: 290, href: CRUD.form.edit, iconCls: 'icon-add',
            onLoad: function () {
            },
            submit: function () {
                if (top.$('#uiform').form('validate')) {
                    var query = createParam('add', 0);
                    jQuery.ajaxjson(CRUD.handler, query, function (d) {
                        if (parseInt(d) > 0) {
                            msg.ok('新增监考员信息成功！');
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

        top.$('#uiform').validate();
    },
    edit: function () {
        var row = grid.getSelectedRow();
        if (row) {
            var hDialog = top.jQuery.hDialog({
                title: '编辑监考员信息', width: 400, height: 290, href: CRUD.form.edit, iconCls: 'icon-edit',
                onLoad: function () {
                    top.$('#txt_Serial').val(row.Serial);
                    top.$('#txt_Name').val(row.Name);
                    top.$('#txt_Invigilated').val(row.Invigilated);
                    if (row.Gender == '男') {
                        top.$('#rb_Gender_1').prop('checked', 'checked');
                    } else {
                        top.$('#rb_Gender_2').prop('checked', 'checked');
                    }
                    if (row.AutoArranged) {
                        top.$('#rb_AutoArranged_1').prop('checked', 'checked');
                    } else {
                        top.$('#rb_AutoArranged_2').prop('checked', 'checked');
                    }

                    top.$('#txt_Serial').prop('readonly', 'readonly');
                    top.$('#txt_Name').prop('readonly', 'readonly');
                    top.$('#rb_Gender_1').prop('disabled', 'disabled');
                    top.$('#rb_Gender_2').prop('disabled', 'disabled');
                },
                submit: function () {
                    if (top.$('#uiform').form('validate')) {
                        var query = createParam('edit', row.KeyId);
                        jQuery.ajaxjson(CRUD.handler, query, function (d) {
                            if (parseInt(d) > 0) {
                                msg.ok('监考员信息保存成功！');
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
            msg.warning('请选择要编辑的记录。');
        }
    },
    import: function () {
        var hDialog = top.jQuery.hDialog({
            title: '批量导入监考员信息', width: 635, height: 490, href: CRUD.form.import, iconCls: 'icon-import',
            onLoad: function () {
            },
            submit: function () {
                if (top.$('#uiform').form('validate')) {
                    var query = createParam('import', 0);
                    jQuery.ajaxjson(CRUD.handler, query, function (d) {
                        if (parseInt(d) > 0) {
                            msg.ok('监考员信息保存成功！');
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
    }
}

function createParam(action, keyid, obj) {
    var o = {};
    var query = top.$('#uiform').serializeArray();
    query = convertArray(query);
    if (obj != null) {
        for (var p in obj) {
            query[p] = obj[p];
        }
    }

    o.jsonEntity = JSON.stringify(query);
    o.action = action;
    o.keyid = keyid;
    return "json=" + JSON.stringify(o);
}
