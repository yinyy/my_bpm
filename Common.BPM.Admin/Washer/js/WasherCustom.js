var actionURL = '/Washer/ashx/WasherCustomHandler.ashx';
var formurl = '/Washer/html/WasherCustom.html';

$(function () {

    autoResize({ dataGrid: '#list', gridType: 'datagrid', callback: grid.bind, height: 0 });

    $('#a_add').click(CRUD.add);
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
            title: "客户列表",
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
		        title: '姓名', field: 'Name', width: 150, align: 'center'
		    },
		    { title: '性别', field: 'Gender', width: 60, align: 'center'},
		    {
		        title: '身份证号', field: 'Card', width: 200, align: 'center', formatter: function (v, r, index) {
		            if (v != '') {
		                return v.substring(0, 6) + '********' + v.substring(v.length - 4);
		            }

		            return v;
		        }},
            {
                title: '记录', field: 'KeyId', width: 150, align: 'center', formatter: function (v, r, i) {
                    return '<a href="javascript:void(0);" onclick="CRUD.charge(' + r.KeyId + ');">充值记录</a>&nbsp;&nbsp;<a href="javascript:void(0);" onclick="CRUD.consume(' + r.KeyId + ');">消费记录</a>';
                }
            },
		    {
		        title: '备注', field: 'Memo', width: 500, align: 'left'
		    }
            ]],
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
    add: function () {
        var hDialog = top.jQuery.hDialog({
            title: '添加', width: 450, height: 283, href: formurl, iconCls: 'icon-add', submit: function () {
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
                top.$('#txt_Gender').combobox({   
                    url:'/sys/ashx/DicHandler.ashx?action=code&code=gender',   
                    valueField:'Title',   
                    textField:'Title'  
                });
            }
        });

        top.$('#uiform').validate();
    },
    edit: function () {
        var row = grid.getSelectedRow();
        if (row) {
            var hDialog = top.jQuery.hDialog({
                title: '编辑', width: 450, height: 283, href: formurl, iconCls: 'icon-save',
                onLoad: function () {
                    top.$('#txt_Gender').combobox({
                        url: '/sys/ashx/DicHandler.ashx?action=code&code=gender',
                        valueField: 'Title',
                        textField: 'Title'
                    });

                    top.$('#txt_Name').val(row.Name);
                    top.$('#txt_Gender').combobox('setValue', row.Gender);
                    top.$('#txt_Card').val(row.Card);
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
    charge: function (rid) {

    },
    consume: function (rid) {
        $('#win').window({
            width: 660,
            height: 600,
            modal: true,
            collapsible: false,
            minimizable: false,
            maximizable: false,
            title: '客户消费记录'
        });

        $('#tt').treegrid({
            url: actionURL + '?' + createParam('consume', rid),
            idField: 'KeyId',
            treeField: 'CardSerial',
            animate: true,
            maximized: true,
            columns: [[
                { title: '卡号', field: 'CardSerial', width: 150 },
                { field: 'Time', title: '消费时间', width: 160, align: 'center' },
                { field: 'DeviceAddress', title: '消费地点', width: 200, align: 'center' },
                {
                    field: 'Money', title: '消费金额', width: 100, align: 'right', formatter: function (v, r, i) {
                        return '￥' + v.toFixed(2);
                    }
                }
            ]]
        });
    }
};