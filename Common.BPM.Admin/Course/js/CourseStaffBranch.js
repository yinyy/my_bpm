var actionURL = '/Course/ashx/CourseStaffBranchHandler.ashx';
var auditForm = '/Course/html/CourseStaffBranchAudit.html';

$(function () {
    autoResize({ dataGrid: '#list', gridType: 'datagrid', callback: grid.bind, height: 0 });

    $('#a_audit').click(CRUD.audit);

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
            title: "学生分流",
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
                { title: '学号', field: 'StaffSerial', width: 150, align: 'center' },
                { title: '姓名', field: 'StaffName', width: 200, align: 'center' },
                {
                    title: '第一专业方向', field: 'Branch1', width: 250, align: 'center', formatter: function (v, r, idx) {
                        if (v == null || v=='') {
                            return;
                        }

                        var values = v.split('|');
                        if (values[1] == 1) {
                            return '<span style="font-weight:bolder;color:#0000ff;">' + values[3] + '</span>';
                        } else {
                            return values[3];
                        }
                    }
                },
                {
                    title: '备选专业方向2', field: 'Branch2', width: 250, align: 'center', formatter: function (v, r, idx) {
                        if (v == null || v == '') {
                            return;
                        }

                        var values = v.split('|');
                        if (values[1] == 1) {
                            return '<span style="font-weight:bolder;color:#0000ff;">' + values[3] + '</span>';
                        } else {
                            return values[3];
                        }
                    }
                },
                {
                    title: '备选专业方向2', field: 'Branch3', width: 250, align: 'center', formatter: function (v, r, idx) {
                        if (v == null || v == '') {
                            return;
                        }

                        var values = v.split('|');
                        if (values[1] == 1) {
                            return '<span style="font-weight:bolder;color:#0000ff;">' + values[3] + '</span>';
                        } else {
                            return values[3];
                        }
                    }
                },
                { title: '自我评价', field: 'Introduction', width: 500, align: 'center' },
            ]],
            pagination: true,
            pageSize: PAGESIZE,
            pageList: [20, 40, 50],
            sortName: 'StaffSerial',
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
    audit: function () {
        var row = grid.getSelectedRow();
        if (row) {
            var hDialog = top.jQuery.hDialog({
                title: '审核', width: 460, height: 375, href: auditForm, iconCls: 'icon-audit',
                onLoad: function () {
                    top.$('#txt_Serial').text(row.StaffSerial);
                    top.$('#txt_Name').text(row.StaffName);
                    top.$('#txt_Introduction').text(row.Introduction);

                    $.each([row.Branch1, row.Branch2, row.Branch3], function (idx, v) {
                        if (v == null || v == '') {
                            //
                        } else {
                        var values = v.split('|');

                        var div = $('<div></div>');
                        var input = $('<input type="radio" name="choose" value="' + values[2] + '" id="choose' + idx + '"/>')
                        if (values[1] == 1) {
                            $(input).prop('checked', 'checked');
                        }
                        var label = $('<label for="choose' + idx + '">' + values[3] + '</label > ');

                        $(div).append(input);
                        $(div).append(label);

                        top.$('#branch_container').append(div);
                        }
                    });
                },
                submit: function () {
                    var o = {};
                    o.jsonEntity = JSON.stringify({ 'StaffId': row.StaffId, 'BranchId': top.$('input:checked').val() });
                    o.action = 'audit';
                    o.keyid = 0;
                    var query = "json=" + JSON.stringify(o);

                    jQuery.ajaxjson(actionURL, query, function (d) {
                        if (parseInt(d) > 0) {
                            msg.ok('审核成功！');
                            hDialog.dialog('close');
                            grid.reload();
                        } else {
                            MessageOrRedirect(d);
                        }
                    });
                }
            });
        } else {
            msg.warning('请选择要修改的行。');
        }
    }
}