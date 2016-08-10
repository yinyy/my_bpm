var actionURL = '/Washer/ashx/WasherDeviceLogHandler.ashx';
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
        //var hDialog = top.jQuery.hDialog({
        //    title: '查询',
        //    width: 350,
        //    height: 150,
        //    content: searchForm,
        //    iconCls: 'icon-search',
        //    buttons: [
        //        {
        //            text: '确定', iconCls: 'icon-ok', handler: function () {
        //                if (top.$('#uiform').form('validate')) {
        //                    var filter = '{"groupOp":"AND","rules":[{"field":"Started","op":"ge","data":"' +
        //                        top.$('#txt_Time_Start').datebox('getValue') + '"}, {"field":"Started","op":"le","data":"' +
        //                        top.$('#txt_Time_End').datebox('getValue') + '"}],"groups":[]}';

        //                    $('#list').datagrid('reload', { filter: filter });
        //                    $('body').data('where', filter);//.replace('Time', '加水时间').replace('Name', '姓名').replace('Plate', '车牌号')
        //                    //hDialog.dialog('close');
        //                }
        //                return false;
        //            }
        //        },
        //        {
        //            text: '清空', iconCls: 'icon-clear', handler: function () {
        //                $('#list').datagrid('reload', { filter: '' });
        //                $('body').data('where', '');
        //                //hDialog.dialog('close');
        //            }
        //        },
        //        {
        //            text: '关闭', iconCls: 'icon-cancel', handler: function () {
        //                hDialog.dialog('close');
        //            }
        //        }
        //    ]
        //});

        //top.$('#txt_Time_Start, #txt_Time_End').datebox({
        //    required: true,
        //    editable: false
        //});
        //top.$('#uiform').validate();
    });

    $('#a_export').click(function () {
        var o = { action: 'export', keyid: 0 };
        var query = "json=" + JSON.stringify(o);

        if ($('body').data('where') != null && $('body').data('where') !='') {
            query = query + "&filter=" + $('body').data('where');
        }

        window.open(actionURL + '?' + query);

        //jQuery.ajaxjson(actionURL, query, function (json) {
        //    alert(json.Success);
        //});
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
                title: '洗车时间', field: 'Started', width: 250, align: 'center', formatter(v, r, i) {
                    if (v == null) {
                        return '';
                    }

                    var str = r.Started.substring(0, 19);
                    if (r.Ended != null) {
                        str += ' - ' + r.Ended.substring(11, 19);
                    }

                    return str;
                }
            },
		    { title: '洗车机序列号', field: 'SerialNumber', width: 150, align: 'center' },
            {
                title: '洗车地点', field: 'Address', width: 220, align: 'center', formatter: function (v, r, i) {
                    if (v == null) {
                        return '';
                    }

                    if (v== '') {
                        return '未安装';
                    } else {
                        return r.Province + ' - ' + r.City + ' - ' + r.Region + '<br/>' + r.Address;
                    }
                }
            },
            {
                title: '洗车币', field: 'PayCoins', width: 100, align: 'right', formatter: function (v, r, i) {
                    if (v == null) {
                        return '';
                    } else {
                        return (v/100.0).toFixed(2);
                    }
                }
            },
            { title: '支付方式', field: 'Kind', width: 150, align: 'center' },
            { title: '支付卡号', field: 'CardNo', width: 150, align: 'center' },
            { title: '支付凭证', field: 'Ticks', width: 150, align: 'center' },
            { title: '所属客户', field: 'DepartmentName', width: 200, align: 'center' }
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
                    PayCoins: amount
                }); 
                $('#list').datagrid('mergeCells', {index: rowIndex, field: 'ConsumeName', rowspan: 1, colspan: 4});
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