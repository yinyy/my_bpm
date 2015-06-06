var actionURL = '/Washer/ashx/WasherCardHandler.ashx';
var formurl = '/Washer/html/WasherCardEdit.html';

$(function () {

    autoResize({ dataGrid: '#list', gridType: 'datagrid', callback: grid.bind, height: 0 });

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
		    { title: '发卡人', field: 'TrueName', width: 150, align: 'center' },
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
                }},
            {
                title: '操作', field: 'KeyId', width: 80, align: 'center', formatter: function (v, r, i) {
                    if (r.Status == 0) {
                        return '<a href="javascript:void(0);" onclick="CRUD.loss(' + r.KeyId + ');">挂失</a>';
                    } else {
                        return '<a href="javascript:void(0);" onclick="CRUD.relieve(' + r.KeyId + ');">解挂</a>';
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
    /*edit: function () {
        var row = grid.getSelectedRow();
        if (row) {
            var hDialog = top.jQuery.hDialog({
                title: '编辑', width: 450, height: 248, href: formurl, iconCls: 'icon-save',
                onLoad: function () {
                    top.$('#txt_CustomId').combobox({
                        url: actionURL + '?' + createParam('cstm', row.UserId),
                        valueField: 'KeyId',
                        textField: 'Name'
                    });

                    top.$('#txt_Serial').val(row.Serial);
                    top.$('#txt_CustomId').combobox('setValue', row.CustomId);
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
    },*/
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
    },
    loss: function (rid) {
        if (confirm('确定挂失当前洗车卡吗？')) {
            jQuery.ajaxjson(actionURL, createParam('loss', rid), function (d) {
                if (parseInt(d) > 0) {
                    msg.ok('挂失成功！');
                    grid.reload();
                } else {
                    MessageOrRedirect(d);
                }
            });
        }
    },
    relieve: function (rid) {
        if (confirm('确定解挂当前洗车卡吗？')) {
            jQuery.ajaxjson(actionURL, createParam('relieve', rid), function (d) {
                if (parseInt(d) > 0) {
                    msg.ok('解挂成功！');
                    grid.reload();
                } else {
                    MessageOrRedirect(d);
                }
            });
        }
    }
};