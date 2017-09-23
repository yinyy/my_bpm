var actionURL = '/Washer/ashx/WasherCardHandler.ashx';
var formURL = '/Washer/html/WasherCard.html';
var batchFormURL = '/Washer/html/WasherCardBatch.html';

function showPassword(o) {
    if (window.event.ctrlKey == true) {
        alert($(o).attr('p'));
    }
}

$(function () {

    autoResize({ dataGrid: '#list', gridType: 'datagrid', callback: grid.bind, height: 0 });

    $('#a_add').click(CRUD.add);
    $('#a_edit').click(CRUD.edit);
    $('#a_delete').click(CRUD.del);
    $('#a_batch_create').click(CRUD.batch);
    $('#a_export').click(CRUD.exp);

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
                title: '密码', field: 'Password', width: 80, align: 'center'
            },
            {
                title: '卡类型', field: 'Kind', width: 100, align: 'center', formatter(v, r, i) {
                    if (v == 'Inner') {
                        return '内部发行卡';
                    } else if (v == 'Sale') {
                        return '售卖卡';
                    } else if (v == 'Coupon') {
                        return '优惠卡';
                    } else if (v == 'Convertor') {
                        return '兑换卡';
                    } else {
                        return '未知类型';
                    }
                }
            },
            {
                title: '有效期', field: 'ValidateFrom', width: 200, align: 'center', formatter(v, r, i) {
                    return r.ValidateFrom.substring(0, 10) + ' 至 ' + r.ValidateEnd.substring(0, 10);
                }
            },
            {
                title: '洗车币', field: 'Coins', width: 80, align: 'right', formatter(v, r, i) {
                    return (v/100.0).toFixed(2);
                }
            },
            {
                title: '持卡人', field: 'Name', width: 100, align: 'center', formatter(v, r, i) {
                    if (v == null) {
                        return '';
                    } else {
                        return r.Name;
                    }
                }
            },
            { title: '状态', field: 'Status', width: 80, align: 'center' }
            ]],
            pagination: true,
            pageSize: PAGESIZE,
            pageList: [20, 40, 50],
            sortName: 'CardNo',
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
            title: '添加', width: 450, height: 290, href: formURL, iconCls: 'icon-add', submit: function () {
                if (top.$('#uiform').form('validate')) {
                    var query = createParam('add', '0');
                    jQuery.ajaxjson(actionURL, query, function (d) {
                        if (parseInt(d) > 0) {
                            msg.ok('添加成功！');
                            //hDialog.dialog('close');
                            grid.reload();
                        } else if (parseInt(d) ==-1) {
                            msg.warning('卡号已经存在！');
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
    },
    del: function(){
        var row = grid.getSelectedRow();
        if (row) {
            if (confirm('确认删除选中的洗车卡吗？')) {
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
            msg.warning('请选择记录。');
        }
    },
    edit: function () {
        var row = grid.getSelectedRow();
        if (row) {
            var hDialog = top.jQuery.hDialog({
                title: '编辑', width: 450, height: 219, href: formURL, iconCls: 'icon-add', submit: function () {
                    if (top.$('#uiform').form('validate')) {
                        var query = createParam('edit', row.KeyId);
                        jQuery.ajaxjson(actionURL, query, function (d) {
                            if (parseInt(d) > 0) {
                                msg.ok('更新成功！');
                                hDialog.dialog('close');
                                grid.reload();
                            } else if (parseInt(d) == -1) {
                                msg.warning('更新失败！');
                            } else {
                                MessageOrRedirect(d);
                            }
                        });
                    }
                    return false;
                }, onLoad: function () {
                    top.$('#txt_CardNo').val(row.CardNo);
                    top.$('#txt_CardNo').parent().parent().hide();

                    top.$('#txt_Password').val(row.Password);
                    top.$('#txt_Password').parent().parent().hide();

                    top.$('#txt_Coins').numberbox('setValue', row.Coins);
                    //top.$('#txt_Coins').parent().parent().hide();

                    top.$('#txt_ValidateFrom').datebox({
                        required: true,
                        editable: false,
                        value: row.ValidateFrom.substring(0, 10)
                    });
                    top.$('#txt_ValidateEnd').datebox({
                        required: true,
                        editable: false,
                        value: row.ValidateEnd.substring(0, 10)
                    });

                    top.$('#txt_Kind').val(row.Kind);
                }
            });

            top.$('#uiform').validate();
        } else {
            msg.warning('请选择洗车卡。');
        }
    },
    consume: function () {
        var row = grid.getSelectedRow();
        if (row) {
            var hDialog = top.jQuery.hDialog({
                title: '记录', width: 550, height: 655, content: '<div id="tt"></div>', iconCls: 'icon-list'
            });

            top.$('#tt').tabs({
                border: false,
                width: 530,
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
                width: 530,
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
                    field: 'Coins', title: '消费洗车币', width: 100, align: 'right', formatter(v, r, i) {
                        return '￥' + (v*(-1)).toFixed(2);
                    }
                }
                ]],
                pagination: true,
                pageSize: PAGESIZE,
                pageList: [20, 40, 50],
                sortName: 'Time',
                sortOrder: 'desc'
            });

            top.$('#dg2').datagrid({
                url: actionURL + '?' + "json=" + JSON.stringify({ action: 'recharge', keyid: row.KeyId }),
                width: 530,
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
                    field: 'Coins', title: '消费洗车币', width: 100, align: 'right', formatter(v, r, i) {
                        return '￥' + v.toFixed(2);
                    }
                }
                ]],
                pagination: true,
                pageSize: PAGESIZE,
                pageList: [20, 40, 50],
                sortName: 'Time',
                sortOrder: 'desc'
            });
        } else {
            msg.warning('请选择设备。');
        }
    },
    test: function () {
        var query = createParam('coupon', '0');
        jQuery.ajaxjson(actionURL, query, function (d) {
            if (parseInt(d) > 0) {
                msg.ok('添加成功！');
                grid.reload();
            } else {
                msg.warning('添加失败！');
                MessageOrRedirect(d);
            }
        });
    },
    batch: function () {
        var hDialog = top.jQuery.hDialog({
            title: '添加', width: 450, height: 290, href: batchFormURL, iconCls: 'icon-add', submit: function () {
                var start = top.$('#txt_Start').numberbox('getValue');
                var end = top.$('#txt_End').numberbox('getValue');
                if (start >= end) {
                    alert('起止卡号错误。');
                    return false;
                }

                start = top.$('#txt_ValidateFrom').datebox('getValue');
                end = top.$('#txt_ValidateEnd').datebox('getValue');
                if (start >= end) {
                    alert('有效时间错误。');
                    return false;
                }

                if (top.$('#uiform').form('validate')) {
                    var query = createParam('batch', '0');
                    jQuery.ajaxjson(actionURL, query, function (d) {
                        if (parseInt(d) >= 0) {
                            msg.ok('批量生成成功！');
                            hDialog.dialog('close');
                            grid.reload();

                            alert('批量生成洗车卡：' + d + '张。');
                        } else if (parseInt(d) < 0) {
                            msg.warning('批量生成失败！');
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
    },
    exp: function () {
        var o = { action: 'export', keyid: 0 };
        var query = "json=" + JSON.stringify(o);

        if ($('body').data('where') != null && $('body').data('where') != '') {
            query = query + "&filter=" + $('body').data('where');
        }

        window.open(actionURL + '?' + query);
    }
};