$(function () {
    autoResize({ dataGrid: '#list', gridType: 'datagrid', callback: grid.bind, height: 0 });

    $('#a_init').click(CRUD.init);
    $('#a_reset').click(CRUD.reset);
});

var grid = {
    bind: function (winSize) {
        $('#list').datagrid({
            url: CRUD.handler,
            toolbar: '#toolbar',
            title: "监考信息",
            iconCls: 'icon icon-list',
            width: winSize.width,
            height: winSize.height,
            nowrap: false, //折行
            rownumbers: true, //行号
            striped: true, //隔行变色
            idField: 'StaffId',//主键
            singleSelect: true, //单选
            frozenColumns: [[]],
            columns: [[
                { title: '工号', field: 'Serial', width: 150, align: 'center' },
                { title: '姓名', field: 'Name', width: 150, align: 'center' },
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
                    } },
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
        edit: '/Exam/html/ExamStaffInvigilateEdit.html'
    },
    init: function () {
        if (confirm('初始化操作将清空全部监考员的监考次数。确定执行初始化操作吗？')) {
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
    reset: function () {
        var row = grid.getSelectedRow();
        if (row) {
            var hDialog = top.jQuery.hDialog({
                title: '重置监考次数', width: 400, height: 185, href: CRUD.form.edit, iconCls: 'icon-edit',
                onLoad: function () {
                    top.$('#txt_Serial').html(row.Serial);
                    top.$('#txt_Name').html(row.Name);
                    top.$('#txt_Invigilated').val(row.Invigilated);
                },
                submit: function () {
                    if (top.$('#uiform').form('validate')) {
                        var query = createParam('reset', row.StaffId);
                        jQuery.ajaxjson(CRUD.handler, query, function (d) {
                            if (parseInt(d) > 0) {
                                msg.ok('监考信息重置成功！');
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
            msg.warning('请选择要重置的记录。');
        }
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
