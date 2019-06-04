var actionURL = '/Washer/ashx/WasherConsumeHandler.ashx';

$(function () {

    autoResize({ dataGrid: '#list', gridType: 'datagrid', callback: grid.bind, height: 0 });

    $('#a_consume').click(CRUD.consume);

    //高级查询
    $('#a_search').click(function () {
        search.go('list');
    });
    //导出
    $('#a_export').click(function () {
        var o = { action: 'export', keyid: 0 };
        var query = "json=" + JSON.stringify(o);

        if ($('body').data('where') != null && $('body').data('where') != '') {
            query = query + "&filter=" + $('body').data('where');
        }

        window.open(actionURL + '?' + query);
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
		    { title: '姓名', field: 'Name', width: 400, align: 'center' },
            { title: '性别', field: 'Gender', width: 60, align: 'center' },
            {
                title: '微信ID', field: 'OpenId', width: 250, align: 'center', formatter(v, r, i) {
                    var str = r.OpenId;
                    if (r.UnionId != null) {
                        str += '<br/>' + r.UnionId;
                    }
                    return str;
                }},
            {
                title: '昵称', field: 'NickName', width: 130, align: 'center', formatter(v, r, i) {
                    if (v == null) {
                        return '';
                    }

                    return v;
                }},
            { title: '电话', field: 'Telphone', width: 130, align: 'center' },
            { title: '运营商', field: 'DepartmentName', width: 180, align: 'center' },
            {
                title: '归属地', field: 'Country', width: 200, align: 'center', formatter: function (v, r, i) {
                    if (v == null) {
                        return '';
                    }

                    return r.Country + ' - ' + r.Province + ' - ' + r.City;
                }
            },
            {
                title: '可用洗车币', field: 'ValidCoins', width: 80, align: 'right', formatter(v, r, i) {
                    return v == null ? '0.00' : (v/100.0).toFixed(2);
                }},
            {
                title: '即将过期', field: 'WillExpireCoins', width: 80, align: 'right', formatter(v, r, i) {
                    return v == null ? '0.00' : (v/100.0).toFixed(2);
                }},
            {
                title: '积分', field: 'Points', width: 80, align: 'right', formatter(v, r, i) {
                    return v == null ? '0' : v;
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
    },
    consume: function () {
        var row = grid.getSelectedRow();
        if (row) {
            var hDialog = top.jQuery.hDialog({
                title: '记录', width: 750, height: 655, content: '<div id="tt"></div>', iconCls: 'icon-list'
            });

            top.$('#tt').tabs({
                border: false,
                width: 730,
                height: 578
            });
            top.$('#tt').tabs('add', {
                title: '消费记录',
                content: '<table id="dg1"></table>',
                closable: false
            });
            top.$('#tt').tabs('add', {
                title: '充值记录',
                content: '<table id="dg2"></table>',
                closable: false
            });
            top.$('#tt').tabs('select', 0);

            top.$('#dg1').datagrid({
                url: actionURL + '?' + "json=" + JSON.stringify({ action: 'consume', keyid: row.KeyId }),
                width: 730,
                height: 549,
                nowrap: false, //折行
                rownumbers: true, //行号
                striped: true, //隔行变色
                idField: 'KeyId',//主键
                singleSelect: true, //单选
                columns: [[
                    {
                        field: 'Time', title: '消费时间', align: 'center', width: 170, formatter(v, r, i) {
                            return v.substring(0, 19);
                        }
                    },
                    {
                        field: 'Address', title: '洗车地点', width: 200, align: 'center', formatter(v, r, i) {
                            return r.Province + ' - ' + r.City + ' - ' + r.Region + '<br/>' + r.Address;
                        }
                    },
                    { field: 'SerialNumber', title: '设备序列号', align: 'center', width: 120 },
                {
                    field: 'Coins', title: '消费洗车币', width: 100, align: 'right', formatter(v, r, i) {
                        return '￥' + v.toFixed(2);
                    }
                }, { field: 'Kind', title: '消费方式', align: 'center', width: 80 }
                ]],
                pagination: true,
                pageSize: PAGESIZE,
                pageList: [20, 40, 50],
                sortName: 'Time',
                sortOrder: 'desc'
            });

            //top.$('#dg2').datagrid({
            //    url: actionURL + '?' + "json=" + JSON.stringify({ action: 'recharge', keyid: row.KeyId }),
            //    width: 730,
            //    height: 549,
            //    nowrap: false, //折行
            //    rownumbers: true, //行号
            //    striped: true, //隔行变色
            //    idField: 'KeyId',//主键
            //    singleSelect: true, //单选
            //    columns: [[
            //        {
            //            field: 'Time', title: '消费时间', align: 'center', width: 180, formatter(v, r, i) {
            //                return v.substring(0, 19);
            //            }
            //        },
            //        { field: 'Name', title: '姓名', align: 'center', width: 100 },
            //        {
            //            field: 'Coins', title: '消费洗车币', width: 100, align: 'right', formatter(v, r, i) {
            //                return '￥' + v.toFixed(2);
            //            }
            //        },
            //        {
            //            field: 'Address', title: '洗车地点', width: 250, align: 'center', formatter(v, r, i) {
            //                return r.Province + ' - ' + r.City + ' - ' + r.Region + '<br/>' + r.Address;
            //            }
            //        }
            //    ]],
            //    pagination: true,
            //    pageSize: PAGESIZE,
            //    pageList: [20, 40, 50],
            //    sortName: 'Time',
            //    sortOrder: 'desc'
            //});
        } else {
            msg.warning('请选择设备。');
        }
    }
};