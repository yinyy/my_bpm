var actionUrl = 'ashx/LogisticsFreightGroupHandler.ashx';

$(function () {
    var size = { width: $(window).width(), height: $(window).height() };
    mylayout.init(size);
    $(window).resize(function () {
        size = { width: $(window).width(), height: $(window).height() };
        mylayout.resize(size);
    });

    DicCategory.bindTree();
    autoResize({ dataGrid: '#userGrid', gridType: 'datagrid', callback: mygrid.databind, height: 4, width: 204 });

    //字典数据
    $('#a_save').click(mygrid.save);
});

var mylayout = {
    init: function (size) {
        $('#layout').width(size.width - 4).height(size.height - 4).layout();
        var center = $('#layout').layout('panel', 'center');
        center.panel({
            onResize: function (w, h) {
                $('#dicGrid').datagrid('resize', { width: w, height: h });
            }
        });
    },
    resize: function (size) {
        mylayout.init(size);
        $('#layout').layout('resize');
    }
};

var DicCategory = {
    bindTree: function () {
        $('#dataDicType').tree({
            url: actionUrl + '?action=hdfz',
            onLoadSuccess: function (node, data) {
                if (data.length == 0) {
                    $('#noCategoryInfo').fadeIn();
                }

                $('body').data('categoryData', data);
            },
            onClick: function (node) {
                var cc = node.id;

                $.getJSON(actionUrl, { action: 'line', id: cc, t: new Date() }, function (data) {
                    mygrid.SetSelectedRows(data);
                });
            }
        });
    },
    reload: function() {
        $('#dataDicType').tree('reload');
    },
    getSelected: function() {
        return $('#dataDicType').tree('getSelected');
    }
};

function createParam(action, keyid) {
    var o = {};
    var query = top.$('#dicForm').serializeArray();
    query = convertArray(query);
    o.jsonEntity = JSON.stringify(query);
    o.action = action;
    o.keyid = keyid;
    return "json=" + JSON.stringify(o);
}

var dicDialog;
var mygrid = {
    databind: function (winSize) {
        $('#dicGrid').datagrid({
            url: actionUrl + '?action=freight',
            toolbar:'#toolbar',
            title: "货代公司",
            iconCls: 'icon icon-list',
            width: winSize.width,
            height: winSize.height,
            nowrap: false, //折行
            rownumbers: true, //行号
            striped: true, //隔行变色
            idField: 'KeyId',//主键
            singleSelect: false, //单选
            frozenColumns: [[]],
            columns: [[
                { title: '货代公司名称', field: 'DepartmentName', width: 300 },
                { title: '备注', field: 'Remark', width: 300 }
            ]],
            pagination: false,
            pageSize: PAGESIZE,
            pageList:[20,40,50]
        });
    },
    GetSelectedRows:function() {
        return $('#dicGrid').datagrid('getSelections');
    },
    SetSelectedRows: function (ids) {
        //先清除选中的
        $('#dicGrid').datagrid('clearSelections');

        var rows = $('#dicGrid').datagrid('getRows');
        for (var i = 0; i < rows.length; i++) {
            var id = rows[i].KeyId;

            for (var j = 0; j < ids.length; j++) {
                if (ids[j] == id) {
                    $('#dicGrid').datagrid('selectRow', i);
                    break;
                }
            }
        }
    },
    save: function () {
        var row = DicCategory.getSelected();
        if (row == null) {
            msg.warning('请选择一个货代分组。');
            return;
        } else {
            var dicId = row.id;
            var departIds = [];
            var rows = mygrid.GetSelectedRows();

            for(var i=0;i<rows.length;i++){
                departIds[departIds.length] = rows[i].KeyId;
            }
            
            $.post(actionUrl, {action:'save', did: dicId, dids: departIds.join(',')}, function (data) {
                if (data == 'success') {
                    msg.ok('分组信息保存成功。');
                } else {
                    msg.warning('分组信息保存失败。');
                }
            }, 'text');
        }
    }
}