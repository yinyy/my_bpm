var actionURL = 'ashx/SanitationDriverHandler.ashx';
var formurl   = '/Sanitation/html/SanitationDriver.html';

$(function () {

    autoResize({ dataGrid: '#list', gridType: 'datagrid', callback: grid.bind, height: 0 });

    $('#a_add').click(CRUD.add);
    $('#a_edit').click(CRUD.edit);
    $('#a_delete').click(CRUD.del);
    $('#a_read_card').click(CARD.read);
    $('#a_write_card').click(CARD.write);
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
		    { title: '姓名', field: 'Name', width: 200 },
		    { title: '编号', field: 'Code', width: 100, align:'center' },
		    { title: '性别', field: 'Gender', width: 80, align: 'center' },
		    {title:'电话',field:'Telphone',width:120},
		    {title:'备注',field:'Memo',width:400}               
            ]],
            pagination: true,
            pageSize: PAGESIZE,
            pageList: [20, 40, 50],
            sortName: 'Name'
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
            title: '添加', width: 400, height: 318, href:formurl, iconCls: 'icon-add', submit: function () {
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
                    url: '/sys/ashx/DicHandler.ashx?action=code&code=gender',
                    valueField: 'Title',
                    textField: 'Title',
                    required: true,
                    editable: false,
                    missingMessage: '请选择性别。'
                });
            }
        });
       
        top.$('#uiform').validate();
    },
    edit: function () {
        var row = grid.getSelectedRow();
        if (row) {
            var hDialog = top.jQuery.hDialog({
                title: '编辑', width: 400, height: 318, href: formurl, iconCls: 'icon-save',
                onLoad: function () {
                    top.$('#txt_KeyId').val(row.KeyId);
                    top.$('#txt_Name').val(row.Name);
                    top.$('#txt_Code').val(row.Code);

                    top.$('#txt_Gender').combobox({
                        url: '/sys/ashx/DicHandler.ashx?action=code&code=gender',
                        valueField: 'Title',
                        textField: 'Title',
                        required: true,
                        editable: false,
                        missingMessage: '请选择性别。'
                    });
                    top.$('#txt_Gender').combobox('setValue', row.Gender);

                    top.$('#txt_Telphone').val(row.Telphone);
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
                jQuery.ajaxjson(actionURL, createParam('delete', rid), function (d) {
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

var CARD = {
    write: function () {
        var row = grid.getSelectedRow();
        if (row) {
            var epc = CARDClass.makeDriverCard(row.KeyId, row.Code.length > 4 ? row.Code(0, 4) : row.Code);
            CARDClass.writeEPC(epc, function (cardno) {
                alert('写卡成功。卡号：' + cardno.substring(0, 16) + '。');
            });
        } else {
            msg.warning("请选择行。");
        }
    },

    read: function () {
        CARDClass.readEPC(function (data) {
            var cardno = data.substring(0, 16);
            if (!CARDClass.isDriverCard(cardno)) {
                alert('卡片错误。请使用人员卡。');
                return;
            }

            $.getJSON(actionURL, { json: JSON.stringify({ action: 'get', keyid: CARDClass.parseDriverId(cardno) }) }, function (d) {
                if (d == null) {
                    alert('查无此人。');
                } else {
                    var s = '卡号：' + cardno + '\n' +
                   '姓名：' + d.Name + '\n' +
                   '编号：' + d.Code;

                    alert(s);
                }
            });
        }, null);
    }
};