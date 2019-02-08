$(function () {
    autoResize({ dataGrid: '#list', gridType: 'datagrid', callback: grid.bind, height: 0 });

    //高级查询
    $('#a_search').click(function () {
        search.go('list');
    });

    $('#a_notice').click(CRUD.notice);
});

var grid = {
    bind: function (winSize) {
        $('#list').datagrid({
            url: CRUD.handler,
            toolbar: '#toolbar',
            title: "监考确认列表",
            iconCls: 'icon icon-list',
            width: winSize.width,
            height: winSize.height,
            nowrap: false, //折行
            rownumbers: true, //行号
            striped: true, //隔行变色
            idField: 'ExamId',//主键
            singleSelect: true, //单选
            frozenColumns: [[]],
            columns: [[
                { title: '考试名称', field: 'ExamTitle', width: 300, align: 'center' },
                {
                    title: '未确认监考名单', field: 'Data', width: 0, align: 'center', formatter(v, r, idx) {
                        if (v == null || v == '') {
                            return '无';
                        }

                        var ds = [];
                        var datas = v.split('；');
                        $(datas).each(function (idx, d) {
                            ds[ds.length] = '<div style="float: left;padding:5px 10px;">' + d + '</div>';
                        });
                        return ds.join('');
                    }
                }
            ]],
            pagination: true,
            pageSize: PAGESIZE,
            pageList: [20, 40, 50],
            sortName: 'ExamId',
            sortOrder: 'desc'
        });
    },
    getSelectedRow: function () {
        return $('#list').datagrid('getSelected');
    },
    reload: function () {
        $('#list').datagrid('clearSelections').datagrid('reload', { filter: '' });
    }
}

var CRUD = {
    handler: '/Exam/ashx/ExamConfirmHandler.ashx',
    forms: {
        confirm: '/Exam/html/ExamConfirmView.html'
    },
    notice: function () {
        var row = grid.getSelectedRow();
        if (row) {
            if (row.Data == null || row.Data == '') {
                msg.ok('本次考试无需发送监考通知！');
                return;
            }
            
            jQuery.ajaxjson(CRUD.handler, createParam('notice', row.ExamId), function (d) {
                if (d.Success) {
                    var datas = d.Sended.split(',');
                    if (datas.length > 0) {
                        msg.ok('已向 ' + datas.length + ' 位教师发送监考通知。详情请查看日志');
                    } else {
                        msg.warning('消息发送可能存在错误，请查看发送日志。');
                    }

                    var hDialog = top.jQuery.hDialog({
                        title: '消息发送日志', width: 1000, height: 412, href: CRUD.forms.confirm, iconCls: 'icon-list',
                        onLoad: function () {
                            if (d.Sended != '') {
                                $(d.Sended.split(',')).each(function (idx, o) {
                                    var span = $('<span></span>');
                                    span.addClass('person');
                                    span.html(o);

                                    top.$('#Sended').append(span);
                                });
                            }

                            if (d.NotBinded != '') {
                                $(d.NotBinded.split(',')).each(function (idx, o) {
                                    var span = $('<span></span>');
                                    span.addClass('person');
                                    span.html(o);

                                    top.$('#NotBinded').append(span);
                                });
                            }

                            if (d.Errored != '') {
                                $(d.Errored.split(',')).each(function (idx, o) {
                                    var span = $('<span></span>');
                                    span.addClass('person');
                                    span.html(o);

                                    top.$('#Errored').append(span);
                                });
                            }
                        },
                        buttons: [
                            {
                                text: '关闭',
                                iconCls: 'icon-cancel',
                                handler: function () {
                                    hDialog.dialog('close');
                                }
                            }
                        ]
                    });
                } else {
                    msg.warning('监考通知发送失败。')
                }
            });
        } else {
            msg.warning('请选择要发送通知的考试。');
        }
    }
}

function createParam(action, keyid) {
    var o = {};
    var query = top.$('#uiform').serializeArray();
    query = convertArray(query);
    o.jsonEntity = JSON.stringify(query);
    o.action = action;
    o.keyid = keyid;
    return "json=" + JSON.stringify(o);
}