var actionURL = '/Sanitation/ashx/SanitationDispatchHandler.ashx';
var formurl   = '/Sanitation/html/SanitationDispatch.html';

$(function () {

    autoResize({ dataGrid: '#list', gridType: 'treegrid', callback: grid.bind, height: 0 });

    $('#a_add').click(CRUD.add);
    $('#a_edit').click(CRUD.edit);
    $('#a_delete').click(CRUD.del);
    $('#a_enabled').click(CRUD.enable);
    $('#a_card').click(CRUD.card);
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
		    {
		        title: '日期', field: 'Time', width: 120, align: 'center', formatter: function (v, r, i) {
		            return v.substring(0, 10);
		        }
		    },
		    {
		        title: '姓名', field: 'Name', width: 120, formatter: function (v, r, i) {
		            return v + '[' + r.Code + ']';
		        }
		    },
		    {title:'车辆',field:'Plate',width:120, align:'center'},
		    { title: '次数', field: 'Workload', width: 80, align: 'right' },
            {
                title: '是否生效', field: 'Enabled', width: 80, align: 'center', formatter: function (v, r, i) {
                    if (v=='是') {
                        return '<img src="/css/icon/16/bullet_tick.png"/>';
                    } else {
                        return '<img src="/css/icon/16/bullet_minus.png"/>';
                    }
                }
            },
		    {title:'备注',field:'Memo',width:400}               
            ]],
            pagination: true,
            pageSize: PAGESIZE,
            pageList: [20, 40, 50]
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
            title: '添加', width: 400, height: 353, href:formurl, iconCls: 'icon-add', submit: function () {
                if (top.$('#uiform').form('validate')) {
                    var query = createParam('add', '0');
                    jQuery.ajaxjson(actionURL, query, function (d) {
                        if (d == '0') {
                            alert('驾驶员或车辆已经被安排了。');
                        } else {
                            if (parseInt(d) > 0) {
                                msg.ok('添加成功！');
                                hDialog.dialog('close');
                                grid.reload();
                            } else {
                                MessageOrRedirect(d);
                            }
                        }
                    });
                }
                return false;
            }, onLoad: function () {
                var n = new Date();
                top.$('#txt_Time').datebox('setValue', n.getFullYear() + '-' + (n.getMonth() + 1) + '-' + n.getDate());
            }
        });
       
        top.$('#uiform').validate();
    },
    edit: function () {
        var row = grid.getSelectedRow();
        if (row) {
            if (row.Enabled == '是') {
                msg.warning('该行已经生效。不能编辑！');
            } else {
                var hDialog = top.jQuery.hDialog({
                    title: '编辑', width: 400, height: 353, href: formurl, iconCls: 'icon-save',
                    onLoad: function () {
                        top.$('#txt_Time').datebox('setValue', row.Time.substring(0, 10));
                        top.$('#txt_DriverId').combobox('setValue', row.DriverId);
                        top.$('#txt_TrunkId').combobox('setValue', row.TrunkId);
                        top.$('#txt_Workload').numberbox('setValue', row.Workload);
                        if (row.Enabled == '是') {
                            top.$('#txt_Enabled').attr('checked', 'checked');
                        }
                        top.$('#txt_Memo').val(row.Memo);
                    },
                    submit: function () {
                        if (top.$('#uiform').form('validate')) {
                            var query = createParam('edit', row.KeyId);;
                            jQuery.ajaxjson(actionURL, query, function (d) {
                                if (d == '0') {
                                    alert('驾驶员或车辆已经被安排了。');
                                } else {
                                    if (parseInt(d) > 0) {
                                        msg.ok('修改成功！');
                                        hDialog.dialog('close');
                                        grid.reload();
                                    } else {
                                        MessageOrRedirect(d);
                                    }
                                }
                            });
                        }
                        return false;
                    }
                });
            }
        } else {
            msg.warning('请选择要修改的行。');
        }
    },
    del: function () {
        var row = grid.getSelectedRow();
        if (row) {
            if (row.Enabled == '是') {
                msg.warning('该行已经生效。不能删除！');
            } else {
                if (confirm('确认要执行删除操作吗？')) {
                    var rid = row.KeyId;
                    jQuery.ajaxjson(actionURL, createParam('delete', rid), function (d) {
                        if (parseInt(d) > 0) {
                            msg.ok('删除成功！');
                            grid.reload();
                        } else {
                            MessageOrRedirect(d);
                        }
                    });
                }
            }
        } else {
            msg.warning('请选择要删除的行。');
        }
    },
    enable: function () {
        var row = grid.getSelectedRow();
        if (row) {
            if (row.Enabled == '是') {
                msg.warning('该行已经生效。');
            } else {
                if (confirm('生效后不能更改。确认要生效吗？')) {
                    var rid = row.KeyId;
                    jQuery.ajaxjson(actionURL, createParam('enable', rid), function (d) {
                        if (d == '0') {
                            alert('驾驶员或车辆已经被安排了。');
                        } else {
                            if (parseInt(d) > 0) {
                                msg.ok('已经生效！');
                                grid.reload();
                            } else {
                                MessageOrRedirect(d);
                            }
                        }
                    });
                }
            }
        } else {
            msg.warning('请选择行。');
        }
    },
    card: function () {
        var row = grid.getSelectedRow();
        if (row) {
            if (row.Enabled == '是') {
                if (confirm('请将磁卡放在设备上，并点击确定写卡。')) {
                    alert('写卡成功。' + row.Time.substring(0, 10) + ',' + row.KeyId + ',' + row.DriverId + '[' + row.Code + '],' + row.TrunkId + '[' + row.Plate.substring(1) + '],' + row.Workload);
                } 
            } else {
                msg.warning('该行还未生效，不能执行写卡操作。');
            }
        } else {
            msg.warning("请选择行。");
        }
    }
};

