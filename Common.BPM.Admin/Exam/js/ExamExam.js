var staffInvigilators;

$(function () {
    var size = { width: $(window).width(), height: $(window).height() };
    mylayout.init(size);

    autoResize({ dataGrid: '#examGrid', gridType: 'datagrid', callback: ExamGrid.bind, height: 0, width: 450 });
    autoResize({ dataGrid: '#examSectionGrid', gridType: 'datagrid', callback: ExamSectionGrid.bind, height: 0 });
    autoResize({ dataGrid: '#examSectionItemGrid', gridType: 'datagrid', callback: ExamSectionItemGrid.bind, height: 0, width: 700 });
    
    $(window).resize(function () {
        size = { width: $(window).width(), height: $(window).height() };
        mylayout.resize(size);
    });

    $('#a_addExam').click(ExamCrud.add);
    $('#a_editExam').click(ExamCrud.edit);
    $('#a_delExam').click(ExamCrud.del);
    $('#a_freezeExam').click(ExamCrud.freeze);

    $('#a_addExamSection').click(ExamSectionCrud.add);
    $('#a_editExamSection').click(ExamSectionCrud.edit);
    $('#a_delExamSection').click(ExamSectionCrud.del);
    $('#a_autoArrange').click(ExamSectionCrud.autoArrange);
    $('#a_fillArrange').click(ExamSectionCrud.fillArrange);
    
    $('#a_addExamSectionItem').click(ExamSectionItemCrud.add);
    $('#a_editExamSectionItem').click(ExamSectionItemCrud.edit);
    $('#a_delExamSectionItem').click(ExamSectionItemCrud.del);
    $('#a_importExamSectionItem').click(ExamSectionItemCrud.import);
    $('#a_manualArrange').click(ExamSectionItemCrud.manual);
});

var mylayout = {
    init: function (size) {
        $('#layout').width(size.width).height(size.height).layout();
        var west = $('#layout').layout('panel', 'west');
        var center = $('#layout').layout('panel', 'center');
        var east = $('#layout').layout('panel', 'east');

        west.panel({
            onResize: function (w, h) {
                $('#examGrid').datagrid('resize', { width: w - 8, height: h});
            }
        });
        center.panel({
            onResize: function (w, h) {
                $('#examSectionGrid').datagrid('resize', { width: w, height: h});
            }
        });
        east.panel({
            onResize: function (w, h) {
                $('#examSectionItemGrid').datagrid('resize', { width: w - 8, height: h});
            }
        });
    },
    resize: function (size) {
        mylayout.init(size);
        $('#layout').layout('resize');
    }
};

var ExamGrid = {
    bind: function (winSize) {
        $('#examGrid').datagrid({
            url: ExamCrud.handler,
            toolbar: '#examGridToolbar',
            title: "考试列表",
            iconCls: 'icon icon-list',
            width: 450 - 8,
            height: winSize.height,
            nowrap: false, //折行
            rownumbers: true, //行号
            striped: true, //隔行变色
            idField: 'KeyId',//主键
            singleSelect: true, //单选
            frozenColumns: [[]],
            columns: [[
                { title: '考试名称', field: 'Title', width: 350, align: 'center' },
                {
                    title: '是否归档', field: 'Freezed', width: 80, align: 'center', formatter: function (v, r, idx) {
                        if (r.Freezed == 1) {
                            return '是';
                        } 

                        return '<span style="color:#ff0000;">否</span>';
                    }
                },
                {
                    title: '考试时间', field: 'Started', width: 200, align: 'center', formatter: function (v, r, idx) {
                        if (r.Started == r.Ended) {
                            return r.Started.substring(0, 10);
                        }

                        return v.substring(0, 10) + ' 至 ' + r.Ended.substring(0, 10);
                    }
                },
                {
                    title: '预约时间', field: 'SecKillStarted', width: 300, align: 'center', formatter: function (v, r, idx) {
                        if (r.SecKillStarted) {
                            return r.SecKillStarted.substring(0, 16) + ' 至 ' + r.SecKillEnded.substring(0, 16);
                        }

                        return '';
                    }
                }
            ]],
            pagination: true,
            pageSize: PAGESIZE,
            pageList: [20, 40, 50],
            sortName: 'Started',
            sortOrder: 'desc',
            onSelect: function (rowIndex, rowData) {
                ExamSectionGrid.reload();
                ExamSectionItemGrid.reload();
            }
        });
    },
    getSelectedRow: function () {
        return $('#examGrid').datagrid('getSelected');
    },
    reload: function () {
        $('#examGrid').datagrid('clearSelections').datagrid('reload', { filter: '' });
    }
}

var ExamSectionGrid = {
    bind: function (winSize) {
        $('#examSectionGrid').datagrid({
            url: ExamSectionCrud.handler,
            toolbar: '#examSectionGridToolbar',
            title: "场次列表",
            iconCls: 'icon icon-list',
            width: winSize.width - 400 - 700 + 8,
            height: winSize.height,
            nowrap: false, //折行
            rownumbers: true, //行号
            striped: true, //隔行变色
            idField: 'KeyId',//主键
            singleSelect: true, //单选
            frozenColumns: [[]],
            columns: [[
                { title: '场次名称', field: 'Title', width: 180, align: 'center' },
                {
                    title: '本场次时间', field: 'Started', width: 140, align: 'center', formatter: function (v, r, idx) {
                        return v.substring(0, 10) + '<br/>' + v.substring(11, 16) + ' 至 ' + r.Ended.substring(11, 16);
                    }
                },
                {
                    title: '考场/监考', field: 'ItemCount', width: 80, align: 'center', formatter: function (v, r, i) {
                        if (v == null || r.TeacherRequired == null) {
                            return '0 / 0';
                        }

                        return v + ' / ' + r.TeacherRequired;
                    }
                },
                {
                    title: '已选人员', field: 'TeacherNames', width: 400, align: 'left', formatter: function (v, r, i) {
                        if (r.TeacherSelected == null) {
                            return '无';
                        }

                        var names = [];
                        if (v != null) {
                            var datas = v.split(';');
                            $(datas).each(function (idx, d) {
                                names[names.length] = d.substring(d.indexOf(',') + 1);
                            });
                        }

                        if (v == null) {
                            return r.TeacherSelected + '人。';
                        } else {
                            return r.TeacherSelected + '人。' + names.join('，') + '。';
                        }
                    }
                },
            ]],
            pagination: true,
            pageSize: PAGESIZE,
            pageList: [20, 40, 50],
            sortName: 'Started',
            sortOrder: 'asc',
            onSelect: function (rowIndex, rowData) {
                ExamSectionItemGrid.reload();
            }
        });
    },
    getSelectedRow: function () {
        return $('#examSectionGrid').datagrid('getSelected');
    },
    reload: function () {
        var row = ExamGrid.getSelectedRow();
        if (row) {
            var filter = "{\"groupOp\":\"AND\",\"rules\":[{\"field\":\"ExamId\",\"op\":\"eq\",\"data\":\"" + row.KeyId + "\"}],\"groups\":[]}";
            $('#examSectionGrid').datagrid('clearSelections').datagrid('reload', { filter: filter });
        } else {
            var filter = "{\"groupOp\":\"AND\",\"rules\":[{\"field\":\"ExamId\",\"op\":\"eq\",\"data\":\"0\"}],\"groups\":[]}";
            $('#examSectionGrid').datagrid('clearSelections').datagrid('reload', { filter: filter });
        }
    }
};

var ExamSectionItemGrid = {
    bind: function (winSize) {
        $('#examSectionItemGrid').datagrid({
            url: ExamSectionItemCrud.handler,
            toolbar: '#examSectionItemGridToolbar',
            title: "考场列表",
            iconCls: 'icon icon-list',
            width: 700 - 8,
            height: winSize.height,
            nowrap: false, //折行
            rownumbers: true, //行号
            striped: true, //隔行变色
            idField: 'KeyId',//主键
            singleSelect: true, //单选
            frozenColumns: [[]],
            columns: [[
                { title: '考场名称', field: 'Address', width: 200, align: 'center' },
                { title: '考试内容', field: 'Subject', width: 200, align: 'center' },
                {
                    title: '学生/监考', field: 'StudentCount', width: 80, align: 'center', formatter: function (v, r, i) {
                        return v + ' / ' + r.TeacherCount;
                    }
                },
                {
                    title: '已安排', field: 'StaffNames', width: 180, align: 'center', formatter: function (v, r, i) {
                        if (v == null) {
                            return '无';
                        }

                        var names = [];
                        $(v.split(';')).each(function (idx, n) {
                            names[names.length] = n.substring(n.indexOf(',') + 1);
                        });
                        
                        return names;
                    }
                },
            ]],
            pagination: true,
            pageSize: PAGESIZE,
            pageList: [20, 40, 50],
            sortName: 'Address',
            sortOrder: 'asc'
        });
    },
    getSelectedRow: function () {
        return $('#examSectionItemGrid').datagrid('getSelected');
    },
    reload: function () {
        var row = ExamSectionGrid.getSelectedRow();
        if (row) {
            var filter = "{\"groupOp\":\"AND\",\"rules\":[{\"field\":\"ExamSectionId\",\"op\":\"eq\",\"data\":\"" + row.KeyId + "\"}],\"groups\":[]}";
            $('#examSectionItemGrid').datagrid('clearSelections').datagrid('reload', { filter: filter });
        } else {
            var filter = "{\"groupOp\":\"AND\",\"rules\":[{\"field\":\"ExamSectionId\",\"op\":\"eq\",\"data\":\"0\"}],\"groups\":[]}";
            $('#examSectionItemGrid').datagrid('clearSelections').datagrid('reload', { filter: filter });
        }
    }
};

var ExamCrud = {
    handler: '/Exam/ashx/ExamExamHandler.ashx',
    form: {
        edit: '/Exam/html/ExamEdit.html'
    },
    add: function () {
        var hDialog = top.jQuery.hDialog({
            title: '添加考试信息', width: 460, height: 350, href: ExamCrud.form.edit, iconCls: 'icon-add', submit: function () {
                if (top.$('#uiform').form('validate')) {
                    var query = createParam('add', '0');
                    jQuery.ajaxjson(ExamCrud.handler, query, function (d) {
                        if (parseInt(d) > 0) {
                            msg.ok('添加成功！');
                            hDialog.dialog('close');
                            ExamGrid.reload();
                        } else {
                            MessageOrRedirect(d);
                        }
                    });
                }
                return false;
            }, onLoad: function () {
            }
        });

        top.$('#uiform').validate();
    },
    edit: function () {
        var row = ExamGrid.getSelectedRow();
        if (row) {
            var hDialog = top.jQuery.hDialog({
                title: '编辑考试信息', width: 460, height: 350, href: ExamCrud.form.edit, iconCls: 'icon-edit',
                onLoad: function () {
                    top.$('#txt_Title').val(row.Title);
                    top.$('#txt_Started').datebox('setValue', row.Started.substring(0, 10));
                    top.$('#txt_Ended').datebox('setValue', row.Ended.substring(0, 10));
                    top.$('#txt_SecKillStarted').datetimebox('setValue', row.SecKillStarted.substring(0, 16));
                    top.$('#txt_SecKillEnded').datetimebox('setValue', row.SecKillEnded.substring(0, 16));
                    top.$('#txt_Memo').val(row.Memo);
                },
                submit: function () {
                    if (top.$('#uiform').form('validate')) {
                        var query = createParam('edit', row.KeyId);
                        jQuery.ajaxjson(ExamCrud.handler, query, function (d) {
                            if (parseInt(d) > 0) {
                                msg.ok('修改成功！');
                                hDialog.dialog('close');
                                ExamGrid.reload();
                            } else {
                                MessageOrRedirect(d);
                            }
                        });
                    }
                    return false;
                }
            });
        } else {
            msg.warning('请选择要修改的考试信息。');
        }
    },
    del: function () {
        var row = ExamGrid.getSelectedRow();
        if (row) {
            if (confirm('删除考试信息会影响相应的场次信息。确认是否删除当前的考试信息吗？')) {
                var rid = row.KeyId;
                jQuery.ajaxjson(ExamCrud.handler, createParam('del', rid), function (d) {
                    if (parseInt(d) > 0) {
                        msg.ok('删除成功！');
                        ExamGrid.reload();
                    } else {
                        MessageOrRedirect(d);
                    }
                });
            }
        } else {
            msg.warning('请选择要删除的考试信息。');
        }
    },
    freeze: function () {
        var row = ExamGrid.getSelectedRow();
        if (row) {
            if (confirm('归档本次考试信息将更新教师的监考次数，建议在考试结束后归档。确认归档当前的考试吗？')) {
                var rid = row.KeyId;
                jQuery.ajaxjson(ExamCrud.handler, createParam('freeze', rid), function (d) {
                    if (parseInt(d) > 0) {
                        msg.ok('归档成功！');
                        ExamGrid.reload();
                    } else {
                        MessageOrRedirect(d);
                    }
                });
            }
        } else {
            msg.warning('请选择要归档的考试信息。');
        }
    }
}

var ExamSectionCrud = {
    handler: '/Exam/ashx/ExamExamSectionHandler.ashx',
    form: {
        edit: '/Exam/html/ExamSectionEdit.html'
    },
    add: function () {
        var row = ExamGrid.getSelectedRow();
        if (row) {
            var hDialog = top.jQuery.hDialog({
                title: '添加场次信息', width: 460, height: 355, href: ExamSectionCrud.form.edit, iconCls: 'icon-add', submit: function () {
                    if (top.$('#uiform').form('validate')) {
                        var query = createParam('add', '0', { 'ExamId': row.KeyId });
                        jQuery.ajaxjson(ExamSectionCrud.handler, query, function (d) {
                            if (parseInt(d) > 0) {
                                msg.ok('添加成功！');
                                hDialog.dialog('close');
                                ExamSectionGrid.reload();
                            } else {
                                MessageOrRedirect(d);
                            }
                        });
                    }
                    return false;
                }, onLoad: function () {
                }
            });

            top.$('#uiform').validate();
        } else {
            msg.warning('请选择考试信息！');
        }
    },
    edit: function () {
        var row = ExamSectionGrid.getSelectedRow();
        if (row) {
            var hDialog = top.jQuery.hDialog({
                title: '编辑场次信息', width: 460, height: 355, href: ExamSectionCrud.form.edit, iconCls: 'icon-edit',
                onLoad: function () {
                    top.$('#txt_Title').val(row.Title);
                    top.$('#txt_Started').datetimebox('setValue', row.Started.substring(0, 16));
                    top.$('#txt_Ended').datetimebox('setValue', row.Ended.substring(0, 16));
                    top.$('#txt_Memo').val(row.Memo);
                },
                submit: function () {
                    if (top.$('#uiform').form('validate')) {
                        var query = createParam('edit', row.KeyId, { 'ExamId': ExamGrid.getSelectedRow().KeyId });
                        jQuery.ajaxjson(ExamSectionCrud.handler, query, function (d) {
                            if (parseInt(d) > 0) {
                                msg.ok('修改成功！');
                                hDialog.dialog('close');
                                ExamSectionGrid.reload();
                            } else {
                                MessageOrRedirect(d);
                            }
                        });
                    }
                    return false;
                }
            });
        } else {
            msg.warning('请选择要修改的考试信息。');
        }
    },
    del: function () {
        var row = ExamSectionGrid.getSelectedRow();
        if (row) {
            if (confirm('确认是否删除当前的场次信息吗？')) {
                var rid = row.KeyId;
                jQuery.ajaxjson(ExamSectionCrud.handler, createParam('del', rid), function (d) {
                    if (parseInt(d) > 0) {
                        msg.ok('删除成功！');
                        ExamSectionGrid.reload();
                    } else {
                        MessageOrRedirect(d);
                    }
                });
            }
        } else {
            msg.warning('请选择要删除的场次信息。');
        }
    },
    autoArrange: function () {
        var row = ExamSectionGrid.getSelectedRow();
        if (row) {
            if (confirm('是否对自主选择监考的教师安排监考？')) {
                var rid = row.KeyId;
                jQuery.ajaxjson(ExamSectionCrud.handler, createParam('auto', rid), function (d) {
                    if (parseInt(d) > 0) {
                        msg.ok('自动安排监考成功！');
                        ExamSectionGrid.reload();
                        ExamSectionItemGrid.reload();
                    } else {
                        MessageOrRedirect(d);
                    }
                });
            }
        } else {
            msg.warning('请选择要自动安排监考的的场次。');
        }
    },
    fillArrange: function () {
        var row = ExamSectionGrid.getSelectedRow();
        if (row) {
            if (confirm('是否对未自主选择监考的教师安排监考？')) {
                var rid = row.KeyId;
                jQuery.ajaxjson(ExamSectionCrud.handler, createParam('fill', rid), function (d) {
                    if (parseInt(d) > 0) {
                        msg.ok('自动补全监考成功！');
                        ExamSectionGrid.reload();
                        ExamSectionItemGrid.reload();
                    } else {
                        MessageOrRedirect(d);
                    }
                });
            }
        } else {
            msg.warning('请选择要自动安排监考的的场次。');
        }
    }
}

var ExamSectionItemCrud = {
    handler: '/Exam/ashx/ExamExamSectionItemHandler.ashx',
    form: {
        edit: '/Exam/html/ExamSectionItemEdit.html',
        import: '/Exam/html/ExamSectionItemImport.html',
        manual: '/Exam/html/ExamStaffInvigilateManual.html'
    },
    add: function () {
        var row = ExamSectionGrid.getSelectedRow();
        if (row) {
            var hDialog = top.jQuery.hDialog({
                title: '添加考场信息', width: 460, height: 398, href: ExamSectionItemCrud.form.edit, iconCls: 'icon-add', submit: function () {
                    if (top.$('#uiform').form('validate')) {
                        var query = createParam('add', '0', { 'ExamSectionId': row.KeyId });
                        jQuery.ajaxjson(ExamSectionItemCrud.handler, query, function (d) {
                            if (parseInt(d) > 0) {
                                msg.ok('添加成功！');
                                hDialog.dialog('close');
                                ExamSectionItemGrid.reload();
                            } else {
                                MessageOrRedirect(d);
                            }
                        });
                    }
                    return false;
                }, onLoad: function () {
                }
            });

            top.$('#uiform').validate();
        } else {
            msg.warning('请选择考试场次信息！');
        }
    },
    edit: function () {
        var row = ExamSectionItemGrid.getSelectedRow();
        if (row) {
            var hDialog = top.jQuery.hDialog({
                title: '编辑考场信息', width: 460, height: 398, href: ExamSectionItemCrud.form.edit, iconCls: 'icon-edit',
                onLoad: function () {
                    top.$('#txt_Address').val(row.Address);
                    top.$('#txt_Subject').val(row.Subject);
                    top.$('#txt_StudentCount').val(row.StudentCount);
                    top.$('#txt_TeacherCount').val(row.TeacherCount);
                    top.$('#txt_Memo').val(row.Memo);
                },
                submit: function () {
                    if (top.$('#uiform').form('validate')) {
                        var query = createParam('edit', row.KeyId, { 'ExamSectionId': ExamSectionGrid.getSelectedRow().KeyId});
                        jQuery.ajaxjson(ExamSectionItemCrud.handler, query, function (d) {
                            if (parseInt(d) > 0) {
                                msg.ok('修改成功！');
                                hDialog.dialog('close');
                                ExamSectionItemGrid.reload();
                            } else {
                                MessageOrRedirect(d);
                            }
                        });
                    }
                    return false;
                }
            });
        } else {
            msg.warning('请选择要修改的考场信息。');
        }
    },
    del: function () {
        var row = ExamSectionItemGrid.getSelectedRow();
        if (row) {
            if (confirm('确认是否删除当前的考场信息吗？')) {
                var rid = row.KeyId;
                jQuery.ajaxjson(ExamSectionItemCrud.handler, createParam('del', rid), function (d) {
                    if (parseInt(d) > 0) {
                        msg.ok('删除成功！');
                        ExamSectionItemGrid.reload();
                    } else {
                        MessageOrRedirect(d);
                    }
                });
            }
        } else {
            msg.warning('请选择要删除的考场信息。');
        }
    },
    import: function () {
        var row = ExamSectionGrid.getSelectedRow();
        if (row) {
            var hDialog = top.jQuery.hDialog({
                title: '导入考场信息', width: 635, height: 485, href: ExamSectionItemCrud.form.import, iconCls: 'icon-import', submit: function () {
                    if (top.$('#uiform').form('validate')) {
                        var query = createParam('import', '0', { 'ExamSectionId': row.KeyId });
                        jQuery.ajaxjson(ExamSectionItemCrud.handler, query, function (d) {
                            if (parseInt(d) > 0) {
                                msg.ok('添加成功！');
                                hDialog.dialog('close');
                                ExamSectionItemGrid.reload();
                            } else if (parseInt(d) == 0) {
                                msg.warning('没有添加任何数据');
                                hDialog.dialog('close');
                            } else {
                                MessageOrRedirect(d);
                            }
                        });
                    }
                    return false;
                }, onLoad: function () {
                }
            });

            top.$('#uiform').validate();
        } else {
            msg.warning('请选择考试场次信息！');
        }
    },
    manual: function () {
        var row = ExamSectionItemGrid.getSelectedRow();
        if (row) {
            var hDialog = top.jQuery.hDialog({
                title: '手动安排监考人员', width: 900, height: 775, href: ExamSectionItemCrud.form.manual, iconCls: 'icon-table_relationship',
                onLoad: function () {
                    top.$('#Address').html(row.Address);
                    top.$('#Subject').html(row.Subject);
                    top.$('#TeacherRequired').html(row.TeacherCount);

                    //加载监考员信息
                    jQuery.ajaxjson(ExamSectionItemCrud.handler, createParam('invigilator', row.KeyId, {
                        'ExamSectionId': ExamSectionGrid.getSelectedRow().KeyId
                    }), function (data) {
                        if (data != null) {
                            //加载监考信息
                            staffInvigilators = data;
                            loadStaffInvigilators();
                        }
                    });
                },
                submit: function () {
                    var ids = [];
                    top.$('#InvigilatorSelected > span').each(function (idx, d) {
                        ids[ids.length] = parseInt($(d).attr('staffId'));
                    });
                    ids = ids.join(',');

                    var query = createParam('manual', row.KeyId, { 'Memo': ids});//借用Memo存储数据
                    jQuery.ajaxjson(ExamSectionItemCrud.handler, query, function (d) {
                        if (parseInt(d) >= 0) {
                            msg.ok('监考员安排成功！');
                            hDialog.dialog('close');
                            ExamSectionItemGrid.reload();
                        } else {
                            MessageOrRedirect(d);
                        }
                    });
                    return false;
                }
            });
        } else {
            msg.warning('请选择要考场记录。')
        }
    }
}

var InvigilatorCrud = {
    handler: '/Exam/ashx/ExamInvigilatorHandler.ashx',
}


function createParam(action, keyid, obj) {
    var o = {};
    var query = top.$('#uiform').serializeArray();
    query = convertArray(query);
    if (obj != null) {
        for (var p in obj) {
            query[p] = obj[p];
        }
    }

    o.jsonEntity = JSON.stringify(query);
    o.action = action;
    o.keyid = keyid;
    return "json=" + JSON.stringify(o);
}

function loadStaffInvigilators() {
    top.$('#InvigilatorList').html('');
    top.$('#InvigilatorSelected').html('');

    //先把想要监考的显示出来
    $(staffInvigilators).each(function (idx, d) {
        if (d.Wanted && d.AutoArranged==1) {
            var span = $('<span></span>');
            span.addClass('invigilator');
            span.addClass('invigilator_wanted');
            span.attr('staffId', d.KeyId);
            span.attr('serial', d.Serial);
            span.html(d.Name);
            span.prop('title', '[' + d.Serial + ']' + d.Name);
            top.$('#InvigilatorList').append(span);
        }
    });

    //把其它监考员显示出来
    $(staffInvigilators).each(function (idx, d) {
        if (!d.Wanted && d.AutoArranged==1) {
            var span = $('<span></span>');
            span.addClass('invigilator');
            span.attr('staffId', d.KeyId);
            span.attr('serial', d.Serial);
            span.html(d.Name);
            span.prop('title', '[' + d.Serial + ']' + d.Name);
            top.$('#InvigilatorList').append(span);
        }
    });

    //把不自动分配的监考员显示出来
    $(staffInvigilators).each(function (idx, d) {
        if (d.AutoArranged == 0) {
            var span = $('<span></span>');
            span.addClass('invigilator');
            span.addClass('invigilator_other');
            span.attr('staffId', d.KeyId);
            span.attr('serial', d.Serial);
            span.html(d.Name);
            span.prop('title', '[' + d.Serial + ']' + d.Name);
            top.$('#InvigilatorList').append(span);
        }
    });

    //把本考场已安排的监考员显示在上面的列表中
    $(staffInvigilators).each(function (idx, d) {
        if (d.Current) {
            var old = top.$('#InvigilatorList > span[staffId="' + d.KeyId + '"]');
            var cloned = old.clone();

            old.addClass('hidden');
            cloned.removeClass('hidden');
            cloned.removeClass('invigilator_wanted');
            cloned.removeClass('invigilator_other');
            cloned.addClass('invigilator_selected');

            top.$('#InvigilatorSelected').append(cloned);
        }
    });

    //把本场次已安排的监考员隐藏起来
    $(staffInvigilators).each(function (idx, d) {
        if (d.Arranged && !d.Current) {
            top.$('#InvigilatorList > span[staffId="' + d.KeyId + '"]').addClass('hidden');
        }
    });

    //双击监考员的事件
    top.$('#InvigilatorList > span').dblclick(function () {
        if (top.top.$('#InvigilatorSelected > span').size() >= parseInt(top.$('#TeacherRequired').html())) {
            alert('监考人数已满。');
            return;
        }

        //修改当前对象的状态，然后重新加载
        var staffId = $(this).attr('staffId');
        for (var i = 0; i < staffInvigilators.length; i++) {
            if (staffInvigilators[i].KeyId == staffId) {
                staffInvigilators[i].Current = true;
                staffInvigilators[i].Arranged = true;
                break;
            }
        }

        loadStaffInvigilators();
    });
    top.$('#InvigilatorSelected > span').dblclick(function () {
        //修改当前对象的状态，然后重新加载
        var staffId = $(this).attr('staffId');
        for (var i = 0; i < staffInvigilators.length; i++) {
            if (staffInvigilators[i].KeyId == staffId) {
                staffInvigilators[i].Current = false;
                staffInvigilators[i].Arranged = false;
                break;
            }
        }

        loadStaffInvigilators();
    });
}