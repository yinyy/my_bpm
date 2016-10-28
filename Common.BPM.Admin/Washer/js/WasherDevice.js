var actionURL = '/Washer/ashx/WasherDeviceHandler.ashx';
var addDeviceUrl = '/Washer/html/WasherDeviceAdd.html';
var setDeviceUrl = '/Washer/html/WasherDeviceSet.html';

var province = city = area = null;

$(function () {

    autoResize({ dataGrid: '#list', gridType: 'datagrid', callback: grid.bind, height: 0 });

    $('#a_add').click(CRUD.add);
    $('#a_edit').click(CRUD.edit);
    $('#a_delete').click(CRUD.del);
    $('#a_set').click(CRUD.set);
    $('#a_consume').click(CRUD.consume);
    $('#a_qrcode').click(CRUD.qrcode);
    $('#a_downloadparams').click(CRUD.downloadparams);

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
		    {title: '序列号', field: 'SerialNumber', width: 120, align: 'center'},
		    { title: '主板号', field: 'BoardNumber', width: 120, align: 'center' },
            {
                title: '生产时间', field: 'ProductionTime', width: 90, align: 'center', formatter: function (v, r, i) {
                    if (v == null) {
                        return '';
                    } else {
                        return v.substring(0, 10);
                    }
                }},
            {
                title: '出厂时间', field: 'DeliveryTime', width: 90, align: 'center', formatter: function (v, r, i) {
                    if (v == null) {
                        return '';
                    } else {
                        return v.substring(0, 10);
                    }
                }
            },
            {
                title: '客户名称', field: 'DepartmentId', width: 180, align: 'center', formatter: function (v, r, i) {
                    return r.DepartmentName;
                }
            },
            { title: '备注', field: 'Memo', width: 180, align: 'left' },
            {
                title: '安装地点', field: 'Address', width: 200, align: 'center', formatter: function (v, r, i) {
                    if (r.Province == '') {
                        return '未安装';
                    } else {
                        return r.Province + ' - ' + r.City + ' - ' + r.Region + '<br/>' + r.Address;
                    }
                }},
            {
                title: '地理坐标', field: 'Coordinate', width: 150, align: 'center'},
            {
                title: 'IP地址', field: 'IpAddress', width: 110, align: 'center', formatter: function (v, r, i) {
                    if (v=='') {
                        return '未知';
                    } else {
                        return r.UpdateTime + '<br/>' + v;
                    }
                }},
            {
                title: '参数设置', field: 'Setting', width: 150, align: 'center', formatter: function (v, r, i) {
                    var s = JSON.parse(v);
                    var str = '';

                    if (s.Coin != null) {
                        if (str != '') {
                            str += '<br/>';
                        }
                        str += '单价：' + s.Coin + '元';
                    }

                    return str;
                }
            },
            { title: '客户备注', field: 'Memo2', width: 180, align: 'left' }
            ]],
            pagination: true,
            pageSize: PAGESIZE,
            pageList: [20, 40, 50],
            sortName: 'SerialNumber',
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
            title: '添加', width: 450, height: 387, href: addDeviceUrl, iconCls: 'icon-add', submit: function () {
                if (top.$('#uiform').form('validate')) {
                    top.$('#chk_Enabled').val(top.$('#chk_Enabled').attr('checked')=='checked');

                    var query = createParam('add', '0');
                    jQuery.ajaxjson(actionURL, query, function (d) {
                        if (parseInt(d) > 0) {
                            msg.ok('添加成功！');
                            hDialog.dialog('close');
                            grid.reload();
                        } else if (parseInt(d) == -1) {
                            msg.warning('主板编号重复。');
                        } else if (parseInt(d) == -2) {
                            msg.warning('序列号重复。');
                        } else {
                            MessageOrRedirect(d);
                        }
                    });
                }
                return false;
            }, onLoad: function () {
                top.$('#txt_DepartmentId').combobox({
                    url: actionURL + "?json=" + JSON.stringify({ action: 'dpts' }),
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
                title: '编辑', width: 450, height: 387, href: addDeviceUrl, iconCls: 'icon-edit',
                onLoad: function () {
                    top.$('#txt_DepartmentId').combobox({
                        url: actionURL + "?json=" + JSON.stringify({ action: 'dpts' }),
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

                    top.$('#txt_SerialNumber').val(row.SerialNumber);
                    top.$('#txt_BoardNumber').val(row.BoardNumber);
                    top.$('#txt_DepartmentId').combobox('setValue', row.DepartmentId);
                    top.$('#txt_ProductionTime').datebox('setValue', row.ProductionTime.substring(0, 10));
                    top.$('#txt_DeliveryTime').datebox('setValue', row.DeliveryTime.substring(0, 10));
                    top.$('#txt_Memo').val(row.Memo);
                    if (row.Enabled) {
                        top.$('#chk_Enabled').attr('checked', 'checked');
                    } else {
                        top.$('#chk_Enabled').removeAttr('checked');
                    }
                },
                submit: function () {
                    if (top.$('#uiform').form('validate')) {
                        top.$('#chk_Enabled').val(top.$('#chk_Enabled').attr('checked') == 'checked');

                        var query = createParam('edit', row.KeyId);;
                        jQuery.ajaxjson(actionURL, query, function (d) {
                            if (parseInt(d) > 0) {
                                msg.ok('修改成功！');
                                hDialog.dialog('close');
                                grid.reload();
                            } else if (parseInt(d) == -1) {
                                msg.warning('主板编号重复。');
                            } else if (parseInt(d) == -2) {
                                msg.warning('序列号重复。');
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
    set: function () {
        var row = grid.getSelectedRow();
        if (row) {
            var hDialog = top.jQuery.hDialog({
                title: '设置', width: 800, height: 410, href: setDeviceUrl, iconCls: 'icon-edit',
                onLoad: function () {
                    top.$('#txt_Province').combobox({
                        url: "/Washer/ashx/WasherDistrictHandler.ashx?action=province",
                        textField: 'Title',
                        valueField: 'KeyId',
                        editable: false,
                        onSelect: function (r) {
                            var pid = r.KeyId;
                            pid = pid.substring(0, pid.indexOf('_'));
                            top.$('#txt_City').combobox('clear');
                            top.$('#txt_Region').combobox('clear');

                            top.$('#txt_City').combobox('reload', "/Washer/ashx/WasherDistrictHandler.ashx?action=city&pid=" + pid);
                        }
                    });
                    top.$('#txt_City').combobox({
                        textField: 'Title',
                        valueField: 'KeyId',
                        editable: false,
                        onSelect: function (r) {
                            var pid = top.$('#txt_Province').combobox('getValue');
                            pid = pid.substring(0, pid.indexOf('_'));

                            var cid = r.KeyId;
                            cid = cid.substring(0, cid.indexOf('_'));

                            top.$('#txt_Region').combobox('clear');

                            top.$('#txt_Region').combobox('reload', "/Washer/ashx/WasherDistrictHandler.ashx?action=region&cid=" + cid);
                        }
                    });
                    top.$('#txt_Region').combobox({
                        textField: 'Title',
                        valueField: 'KeyId',
                        editable: false
                    });

                    top.$('#userTab').tabs({
                        onSelect: function () {
                            top.$('.validatebox-tip').remove();
                        }
                    });

                    $.getJSON('/Washer/ashx/WasherDistrictHandler.ashx', { province: row.Province, city: row.City, region: row.Region }, function (data) {
                        top.$('#txt_City').combobox('reload', "/Washer/ashx/WasherDistrictHandler.ashx?action=city&pid=" + data.pid);
                        top.$('#txt_Region').combobox('reload', "/Washer/ashx/WasherDistrictHandler.ashx?action=region&cid=" + data.cid);

                        if (data.pid != -1) {
                            top.$('#txt_Province').combobox('setValue', data.pid + '_' + row.Province);
                        }
                        if (data.cid != -1) {
                            top.$('#txt_City').combobox('setValue', data.cid + '_' + row.City);
                        }
                        if (data.rid != -1) {
                            top.$('#txt_Region').combobox('setValue', data.rid + '_' + row.Region);
                        }
                    });
                    top.$('#txt_Address').val(row.Address);
                    top.$('#txt_Coordinate').val(row.Coordinate);
                    top.$('#txt_Memo2').val(row.Memo2);

                    var setting = eval("(" + row.Setting + ")");
                    var ps = setting.Params;
                    top.$("input[name='params'").each(function (idx) {
                        $(this).val(ps[idx]);
                    });

                    var value = ps[31];
                    if (value%2 == 0) {
                        top.$('#paramsbit_1').attr('checked', 'checked');
                    }
                    value = parseInt(value/2);
                    if ( value%2== 1) {
                        top.$('#paramsbit_2').attr('checked', 'checked');
                    }
                    value = parseInt(value / 2);
                    if (value % 2 == 1) {
                        top.$('#paramsbit_3').attr('checked', 'checked');
                    }

                    if (!row.Enabled) {
                        top.$('#paramsbit_1, #paramsbit_2, #paramsbit_3').attr('disabled', 'disabled');
                    }
                },
                submit: function () {
                    if (top.$('#uiform').form('validate')) {
                        var ps = [];
                        top.$("input[name='params']").each(function (idx) {
                            ps[ps.length] = parseInt($(this).val());
                        });
                        ps[ps.length] = 0;
                        ps[ps.length] = parseInt((top.$('#paramsbit_3').attr('checked') == 'checked' ? "1" : "0") +
                            (top.$('#paramsbit_2').attr('checked') == 'checked' ? "1" : "0") +
                            (top.$('#paramsbit_1').attr('checked') == 'checked' ? "0" : "1"), 2);

                        top.$('#txt_Setting').val(JSON.stringify({ Coin: 0, Params: ps }));
                        var query = createParam('set', row.KeyId);
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
            msg.warning('请选择要设置的行。');
        }
    },
    qrcode: function () {
        var row = grid.getSelectedRow();
        if (row) {
            $.post(actionURL, createParam("qrcode", row.KeyId), function (d) {
                if (d.Success) {
                    var dialog = top.$.hDialog({
                        content: '<img id="qrcodeimage" width="300px" height="300px" alt="二维码"/>',
                        height: 380, width: 320,
                        title: "二维码",
                        iconCls: 'icon-add',
                        buttons: [{
                            text: '关闭',
                            iconCls: 'icon-cancel',
                            handler: function () {
                                dialog.dialog('close');
                            }
                        }]
                    });

                    top.$('#qrcodeimage').attr('src', d.Url);
                } else {
                    msg.warning('生成二维码时发生错误。');
                }
            }, 'json');
        } else {
            msg.warning('请选择设备。');
        }
    },
    consume: function () {
        var row = grid.getSelectedRow();
        if (row) {
            var hDialog = top.jQuery.hDialog({
                title: '消费记录', width: 700, height: 635, content: '<table id="dg2"></table>', iconCls: 'icon-list'
            });
            top.$('#dg2').datagrid({
                url: actionURL + '?' + "json=" + JSON.stringify({ action: 'device_consume', keyid: row.KeyId }),
                width: 680,
                height: 558,
                nowrap: false, //折行
                rownumbers: true, //行号
                striped: true, //隔行变色
                idField: 'KeyId',//主键
                singleSelect: true, //单选
                columns: [[
                    {
                        field: 'Time', title: '消费时间', align: 'center', width: 180, formatter(v, r, i) {
                            return v.substring(0, 19);
                        }
                    },
                    { field: 'Name', title: '姓名', align: 'center', width: 100 },
                    {
                        field: 'Coins', title: '消费洗车币', width: 100, align: 'right', formatter(v, r, i) {
                            return '￥' + v.toFixed(2);
                        }
                    },
                    {
                        field: 'Address', title: '洗车地点', width: 250, align: 'center', formatter(v, r, i) {
                            return r.Province + ' - ' + r.City + ' - ' + r.Region + '<br/>' + r.Address;
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
    downloadparams: function () {
        var row = grid.getSelectedRow();
        if (row) {
            $.get('/Washer/ashx/WasherHandler.ashx', { action: 'SendSetting', keyid: row.KeyId }, function (d) {
                if (d=='success') {
                    msg.ok('设置下载成功。');
                } else {
                    msg.warning('参数下载失败。');
                }
            });
        } else {
            msg.warning('请选择设备。');
        }
    }
}