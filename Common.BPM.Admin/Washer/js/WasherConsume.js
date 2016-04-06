var actionURL = '/Washer/ashx/WasherConsumeHandler.ashx';

$(function () {

    autoResize({ dataGrid: '#list', gridType: 'datagrid', callback: grid.bind, height: 0 });

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
            title: "消费者列表",
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
		        title: '昵称', field: 'NickName', width: 130, align: 'center'
		    },
            { title: '性别', field: 'Gender', width: 60, align: 'center' },
            {
                title: '归属地', field: 'Country', width: 150, align: 'center', formatter: function (v, r, i) {
                    return r.Country + ' - ' + r.Province + ' - ' + r.City;
                }
            },
            {
                title: '运营商', field: 'DepartmentName', width: 180, align: 'center'
            },
            //{
            //    title: '卡号', field: 'Card', width: 200, align: 'center'
            //},
            { title: '余额', field: 'Coins', width: 130, align: 'right' },
            {
                title: '积分', field: 'Points', width: 130, align: 'right'
            },
            {
                title: '推荐人', field: 'RefererId', width: 130, align: 'center', formatter: function (v, r, i) {
                    if (r.RefererOpenId == null) {
                        return '无';
                    } else {
                        return r.RefererNickName;
                    }
                }
            },
            {
                title: '记录', field: 'KeyId', width: 100, align: 'center', formatter: function (v, r, i) {
                    return '<a href="javascript:void(0)">消费</a>&nbsp;<a href="javascript:void(0)">充值</a>&nbsp;<a href="javascript:void(0)">积分</a>';
                }
            }
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
            title: '添加', width: 450, height: 352, href: addDeviceUrl, iconCls: 'icon-add', submit: function () {
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
                    url: actionURL + "?json=" + JSON.stringify({action:'users'}),
                    textField: 'Title',
                    valueField: 'KeyId',
                    editable: false,
                    required: true
                });
                top.$('#txt_ProductionTime').datebox({
                    required: true,
                    editable: false
                });
                top.$('#txt_DeliveryTime').datebox({
                    required: true,
                    editable: false
                });
            }
        });

        top.$('#uiform').validate();
    },
    edit: function () {
        var row = grid.getSelectedRow();
        if (row) {
            var hDialog = top.jQuery.hDialog({
                title: '编辑', width: 450, height: 248, href: editDeviceUrl, iconCls: 'icon-save',
                onLoad: function () {
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
            if (confirm('删除设备信息会连带删除该设备的消费情况。\n确认删除该设备吗？')) {
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