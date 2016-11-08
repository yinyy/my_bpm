var actionUrl = '/Agriculture/ashx/AgricultureDeviceHandler.ashx';

$(function () {
    autoResize({ dataGrid: '#list', gridType: 'datagrid', callback: grid.bind, height: 0 });

    $('#a_add').click(CRUD.add);
    $('#a_edit').click(CRUD.edit);
    $('#a_delete').click(CRUD.del);
    $('#a_qrcode').click(CRUD.qrcode);
});

var grid = {
    bind: function (winSize) {
        $('#list').datagrid({
            url: actionUrl,
            toolbar: '#toolbar',
            title: "Zigbee传感器",
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
                { title: 'MAC地址', field: 'Mac', width: 250, align: 'center' },
                { title: '设备描述', field: 'Title', width: 300, align: 'center' },
                { title: '设备类型', field: 'Kind', width: 120, align: 'center' },
                { title: '组号', field: 'GroupId', width: 100, align: 'center' },
                { title: '坐标', field: 'Coordinate', width: 250, align: 'center' },
                {
                    title: '报警阈值', field: 'Threshold', width: 300, align: 'center', formatter: function (v, r, i) {
                        try {
                            var threshold = '';
                            var o = eval('(' + v + ')');
                            if (r.Kind == '温湿度传感器') {
                                var t = o[0];
                                var h = o[1];

                                if (t.Low.Enabled && t.High.Enabled) {
                                    threshold  = '温度低于 ' + t.Low.Value + '℃或高于 ' + t.High.Value + '℃';
                                } else if (t.Low.Enabled) {
                                    threshold = '温度低于 ' + t.Low.Value + '℃';
                                } else if (t.High.Enabled) {
                                    threshold = '温度高于 ' + t.High.Value + '℃';
                                }

                                threshold += '  <br/>';

                                if (h.Low.Enabled && h.High.Enabled) {
                                    threshold += '湿度低于 ' + h.Low.Value + '% 或高于 ' + h.High.Value + '%';
                                } else if (h.Low.Enabled) {
                                    threshold += '湿度低于 ' + h.Low.Value + '%';
                                } else if (h.High.Enabled) {
                                    threshold += '湿度高于 ' + h.High.Value + '%';
                                }

                                return $.trim(threshold);
                            } else if (r.Kind == '土壤湿度传感器') {
                                var h = o[0];

                                if (h.Low.Enabled && h.High.Enabled) {
                                    threshold = '低于 ' + h.Low.Value + '% 或高于 ' + h.High.Value + '%';
                                } else if (h.Low.Enabled) {
                                    threshold = '低于 ' + h.Low.Value + '%';
                                } else if (h.High.Enabled) {
                                    threshold = '高于 ' + h.High.Value + '%';
                                }
                                return threshold;
                            } else if (r.Kind == 'PM2.5传感器') {
                                var h = o[0];

                                if (h.Low.Enabled && h.High.Enabled) {
                                    threshold = '低于 ' + h.Low.Value + ' 或高于 ' + h.High.Value;
                                } else if (h.Low.Enabled) {
                                    threshold = '低于 ' + h.Low.Value;
                                } else if (h.High.Enabled) {
                                    threshold = '高于 ' + h.High.Value;
                                }
                                return threshold;
                            }
                        } catch (exp) {
                            return 'error';
                        }
                    }},
                { title: '备注', field: 'Memo', width: 300, align: 'left' }
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
        var hDialog = top.$.hDialog({
            title: '添加Zigbee设备', width: 510, height: 495, href: '/Agriculture/html/AgricultureDevice.html', iconCls: 'icon-add', submit: function () {
                if (top.$('#uiform').validate().form()) {
                    var query = createParam('add', '0');
                    $.ajaxjson(actionUrl, query, function (d) {
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
            }
        });

        top.$('#uiform').validate();
    },
    edit: function () {
        var row = grid.getSelectedRow();
        if (row) {
            var hDialog = top.$.hDialog({
                title: '编辑Zigbee设备', width: 510, height: 495, href: '/Agriculture/html/AgricultureDevice.html', iconCls: 'icon-save',
                submit: function () {
                    if (top.$('#uiform').validate().form()) {
                        var query = createParam('edit', row.KeyId);;
                        $.ajaxjson(actionUrl, query, function (d) {
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
                },
                onLoad: function () {
                    top.$('#txt_Mac').val(row.Mac);
                    top.$('#txt_Title').val(row.Title);
                    top.$('#txt_Kind').combobox('setValue', row.Kind);
                    top.$('#txt_GroupId').combobox('setValue', row.GroupId);
                    top.$('#txt_Coordinate').val(row.Coordinate);
                    top.$('#txt_Memo').val(row.Memo);
                    top.$('#txt_Threshold').val(row.Threshold);
                }
            });

            top.$('#uiform').validate();
        } else {
            msg.warning('请选择要修改的行。');
        }
    },
    del: function () {
        var row = grid.getSelectedRow();
        if (row) {
            if (confirm('确认要执行删除操作吗？')) {
                var rid = row.KeyId;
                $.ajaxjson(actionUrl, createParam('delete', rid), function (d) {
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
    qrcode: function () {
        var row = grid.getSelectedRow();
        if (row) {
            $.post(actionUrl, createParam("qrcode", row.KeyId), function (d) {
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
    }
};

function showInMap(c) {
    alert('在地图查看' + c);
}