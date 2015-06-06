var actionURL = '/Washer/ashx/WasherDeviceHandler.ashx';
var formurl = '/Washer/html/WasherDevice.html';

$(function () {

    autoResize({ dataGrid: '#list', gridType: 'datagrid', callback: grid.bind, height: 0 });

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
            url: actionURL+'?'+createParam('list2',0),
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
		    { title: '序列号', field: 'Serial', width: 150, align: 'center' },
		    { title: '名称', field: 'Title', width: 250, align: 'center' },
		    {
		        title: '地址', field: 'Address', width: 250, align: 'center', formatter: function (v, r, i) {
		            return '<a href="">' + v + '</a>';
		        }},
		    {
		        title: '状态', field: 'Status', width: 80, align: 'center', formatter: function (v, r, i) {
		            if (v == 1) {
		                return '工作';
		            } else if (v == 2) {
		                return '空闲';
		            } else {
		                return '未知';
		            }
		        }},
		    {
		        title: '更新时间', field: 'Updated', width: 170, align: 'center', formatter: function (v, r, i) {
		            return v.substring(0, 10) + '&nbsp;&nbsp;' + v.substring(11);
		        }},
            { title: '备注', field: 'Memo2', width: 350, align: 'left' }]],
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
    edit: function () {
        var row = grid.getSelectedRow();
        if (row) {
            var hDialog = top.jQuery.hDialog({
                title: '编辑', width: 450, height: 248, href: formurl, iconCls: 'icon-save',
                onLoad: function () {
                    top.$('#txt_Title').val(row.Title);
                    top.$('#txt_Address').val(row.Address);
                    top.$('#txt_Memo2').val(row.Memo2);
                },
                submit: function () {
                    if (top.$('#uiform').form('validate')) {
                        var query = createParam('edit2', row.KeyId);;
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
                jQuery.ajaxjson(actionURL, createParam('del2', rid), function (d) {
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