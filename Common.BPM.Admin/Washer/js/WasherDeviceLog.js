var actionURL = '/Washer/ashx/WasherDeviceLogHandler.ashx';
var balanceUrl = '/Washer/html/WasherBalance.html';


//var searchForm = '<form id="uiform"><table class="grid">' +
//    '<tr>' +
//        '<td>开始日期：</td>' +
//        '<td><input type="text" id="txt_Time_Start" name="TimeStart" class="easyui-datebox" style="width: 186px;" /></td>' +
//    '</tr>' +
//    '<tr>' +
//        '<td>结束时间：</td>' +
//        '<td><input type="text" id="txt_Time_End" name="TimeEnd" class="easyui-datebox" style="width: 186px;" /></td>' +
//    '</tr>' +
//'</table></form>';

$(function () {

    autoResize({ dataGrid: '#list', gridType: 'datagrid', callback: grid.bind, height: 0 });

    //高级查询
    $('#a_search').click(function () {
        search.go('list');
    });

    $('#a_export').click(function () {
        var o = { action: 'export', keyid: 0 };
        var query = "json=" + JSON.stringify(o);

        if ($('body').data('where') != null && $('body').data('where') != '') {
            query = query + "&filter=" + $('body').data('where');
        }

        window.open(actionURL + '?' + query);
    });

    $('#a_balance').click(function () {
        CRUD.balance();
    });

    $('#a_hide').click(function () {
        CRUD.hide();
    });
});

var grid = {
    bind: function (winSize) {
        $('#list').datagrid({
            url: actionURL,
            toolbar: '#toolbar',
            title: "洗车机流水记录",
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
                title: '消费者', field: 'ConsumeName', width: 100, align: 'center', formatter(v, r, i) {
                    if (v == null) {
                        return '未注册用户';
                    } else {
                        return v;
                    }
                }
            },
            {
                title: '开始时间', field: 'Started', width: 150, align: 'center', formatter(v, r, i) {
                    if (v == null) {
                        return '';
                    }

                    var str = v.substring(0, 19);
                    return str;
                }
            },
            {
                title: '结束时间', field: 'Ended', width: 70, align: 'center', formatter(v, r, i) {
                    var str = '';
                    if (v == null) {
                        str = '<span style="color:red;">未结算</span>';
                    } else  {
                        str = v.substring(11, 19);
                    } 
                    return str;
                }
            },
		    {
		        title: '编号', field: 'SerialNumber', width: 150, align: 'left', formatter: function (v, r, i) {
		            return '序列号：' + r.SerialNumber + '<br/>' + '主板号：' + r.BoardNumber;
		        }
		    },
            {
                title: '洗车地点', field: 'Address', width: 220, align: 'center', formatter: function (v, r, i) {
                    if (v == null) {
                        return '';
                    }

                    if (v == '') {
                        return '未安装';
                    } else {
                        return r.Province + ' - ' + r.City + ' - ' + r.Region + '<br/>' + r.Address;
                    }
                }
            },
            {
                title: '初始洗车币', field: 'RemainCoins', width: 70, align: 'right', formatter: function (v, r, i) {
                    return (v / 100.0).toFixed(2);
                }
            },
            {
                title: '结算洗车币', field: 'PayCoins', width: 70, align: 'right', formatter: function (v, r, i) {
                    if (v == null) {
                        return '';
                    } else {
                        return (v / 100.0).toFixed(2);
                    }
                }
            },
            { title: '支付方式', field: 'Kind', width: 100, align: 'center' },
            { title: '支付卡号', field: 'CardNo', width: 150, align: 'center' },
            { title: '支付凭证', field: 'Ticks', width: 150, align: 'center' },
            { title: '所属客户', field: 'DepartmentName', width: 200, align: 'center' },
            {
                title: '实时消费', field: 'TempTime', width: 200, align: 'left', formatter(v, r, i) {
                    if (v == null) {
                        return '';
                    }

                    var str = '时&nbsp;&nbsp;&nbsp;间：' + r.TempTime.substring(0, 19) + '<br/>洗车币：' + (r.TempCoins / 100.0).toFixed(2);

                    return str;
                }
            },
            { title: '备注', field: 'Memo', width: 300, align: 'left' },
            ]],
            pagination: true,
            pageSize: PAGESIZE,
            pageList: [20, 40, 50],
            sortName: 'KeyId',
            sortOrder: 'desc',
            /*showFooter: true,*/
            onLoadSuccess: function () {
                var rows = $('#list').datagrid('getRows');
                var amount = 0.0;

                for (var i = 0; i < rows.length; i++) {
                    amount += rows[i].PayCoins;
                }

                var rowIndex = rows.length;

                $('#list').datagrid('appendRow', {
                    ConsumeName: '<b>消费合计</b>',
                    PayCoins: amount,
                    IsShow: true
                });
                $('#list').datagrid('mergeCells', { index: rowIndex, field: 'ConsumeName', rowspan: 1, colspan: 5 });
            },
            rowStyler: function (index, row) {
                if (!row.IsShow) {
                    return 'color:#ff00ff;';
                }
            }
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
    balance: function () {
        var row = grid.getSelectedRow();
        if (row) {
            var isEnd = row.Ended != null;
            if (isEnd) {
                msg.warning('当前选中的行已经完成结算。');
            } else {
                var hDialog = top.jQuery.hDialog({
                    title: '手动结算', width: 300, height: 125, href: balanceUrl, iconCls: 'icon-coins', submit: function () {
                        if (top.$('#uiform').form('validate')) {
                            var query = createParam('balance', row.KeyId);
                            jQuery.ajaxjson(actionURL, query, function (d) {
                                if (parseInt(d) > 0) {
                                    msg.ok('手动结算成功！');
                                    hDialog.dialog('close');
                                    grid.reload();
                                } else {
                                    MessageOrRedirect(d);
                                }
                            });
                        }
                        return false;
                    }, onLoad: function () {
                        if (row.TempCoins != null) {
                            top.$('#txt_PayCoins').numberbox('setValue', row.TempCoins);
                        } else {
                            top.$('#txt_PayCoins').numberbox('setValue', row.RemainCoins > 500 ? 500 : row.RemainCoins);
                        }
                    }
                });

                top.$('#uiform').validate();
            }
        } else {
            msg.warning('请选择要结算的行。');
        }
    },
    hide: function () {
        var row = grid.getSelectedRow();
        if (row) {
            if (confirm(row.IsShow ? "其他用户也可能看到当前记录。是否隐藏？" : "其他用户不能查看当前记录。是否显示？")) {
                var query = createParam('show', row.KeyId);
                jQuery.ajaxjson(actionURL, query, function (d) {
                    if (parseInt(d) > 0) {
                        msg.ok('操作成功！');
                        grid.reload();
                    } else {
                        MessageOrRedirect(d);
                    }
                });
            }
        } else {
            msg.warning('请选择要操作的行。');
        }
    }
}