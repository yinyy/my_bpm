var actionURL = '/demo/ashx/LogisticsInquiryHandler.ashx';
var uploadURL = '/sys/ashx/UploadHandler.ashx';
var formurl = '/demo/html/LogisticsInquiry.html';
var viewurl = '/demo/html/LogisticsInquiryView.html';

$(function () {

    autoResize({ dataGrid: '#list', gridType: 'datagrid', callback: grid.bind, height: 0 });

    $('#a_add').click(CRUD.add);
    $('#a_edit').click(CRUD.edit);
    $('#a_delete').click(CRUD.del);
    $('#a_send').click(CRUD.send);
    $('#a_detail').click(CRUD.detail);
    $('#a_copy').click(CRUD.copy);
    $('#a_export').click(function () {
        if ($('#list').datagrid('options').queryParams != null) {
            var f = '{"groupOp": "AND", "rules": [{ "field": "NewIndex", "op": "eq", "data": 1 }], "groups": [' + $('#list').datagrid('options').queryParams['filter'] + "]}";
            $('body').data('where', f);
        };

        //$('body').data('where', $('#list').datagrid('options').queryParams['filter'])
        //for (var p in $('#list').datagrid('options').queryParams)
        //    alert(p+'  '+$('#list').datagrid('options').queryParams[p]);

        var ee = new ExportExcel('list', "V_Inquiry_Detail",
            [
            { title: '询价人', field: 'Inquirer_Title' },
            { title: '询价时间', field: 'Published' },
            { title: '截止日期', field: 'Ended' },
            { title: '货物名称', field: 'Cargo' },
            { title: '柜数', field: 'Amount' },
            { title: '包装', field: 'Packing_Title' },
            { title: '吨数', field: 'Weight' },
            { title: '目的港', field: 'Port' },
            { title: 'ETD', field: 'ETD' },
            { title: 'ETA', field: 'ETA' },
            { title: '类型', field: 'Kind_Title' },
            { title: '免费箱使', field: 'Freebox' },
            { title: '备注', field: 'Memo' },
            { title: '附件', field: 'Attachment'},
            { title: '航线', field: 'SupplyGroupTitle' },
            { title: '发送给', field: 'SupplyNames' },
            { title: '当前状态', field: 'Status' },
            { title: '中标公司', field: 'Bidder'},
            { title: '价格', field: 'BidderPrice'},
            { title: '船公司', field: 'BidderShip' },
            { title: 'ETD', field: 'BidderETD' },
            { title: 'ETA', field: 'BidderETA'},
            { title: '备注', field: 'BidderMemo'}
        ]);
        ee.go();
    });
    $('#a_admin_delete').click(CRUD.admin_delete);

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
			{ title: '询价人', field: 'Inquirer_Title', width: 90, align: 'center' },
		    { title: '询价时间', field: 'Published', width: 150, align: 'center' },
		    { title: '截止日期', field: 'Ended', width: 150, align: 'center' },
		    { title: '货物名称', field: 'Cargo', width: 160 },
		    { title: '目的港', field: 'Port', width: 120 },
            { title: 'ETD', field: 'ETD', width: 90, align: 'center' },
            { title: 'ETA', field: 'ETA', width: 90, align: 'center' },
		    {
                title: '附件', field: 'Attachment', width: 60, align: 'center',
                formatter: function (value, row, index) {
                    if (value == '') {
                        return '';
                    } else {
                        return '<a href="' + value + '" target="_blank">下载</a>';
                    }
                }
            },
            { title: '发送给', field: 'SupplyNames', width: 150 },
		    {
		        title: '当前状态', field: 'Status', width: 70, align: 'center',
		        styler: function (value, row, index) {
		            if (value == '未发送') {
		                return 'color: #000000';
		            } else if (value == '已发送') {
		                return 'color: #ff00ff';
		            } else if (value == '已完成') {
		                return 'color: #0000ff';
		            }
		        }
		    },
		    {
		        title: '中标公司', field: 'Bidder', width: 150, formatter: function (value, row, index) {
		            if (row.Status == '已完成') {
		                return row.Bidder;
		            } else {
		                return '';
		            }
		        }
		    },
		    {
		        title: '价格', field: 'BidderPrice', width: 75, align: 'right', formatter: function (value, row, index) {
		            if (row.Status == '已完成') {
		                return row.BidderPrice;
		            } else {
		                return '';
		            }
		        }
		    },
		    {
		        title: '船公司', field: 'BidderShip', width: 150, align: 'left', formatter: function (value, row, index) {
		            if (row.Status == '已完成') {
		                return row.BidderShip;
		            } else {
		                return '';
		            }
		        }
		    },
            {
                title: 'ETD', field: 'BidderETD', width: 150, align: 'left', formatter: function (value, row, index) {
                    if (row.Status == '已完成') {
                        return row.BidderVoyage;
                    } else {
                        return '';
                    }
                }
            },
            {
                title: 'ETA', field: 'BidderETA', width: 90, align: 'center', formatter: function (value, row, index) {
                    if (row.Status == '已完成') {
                        return row.BidderRange;
                    } else {
                        return '';
                    }
                }
            },
            {
                title: '备注', field: 'BidderMemo', width: 200, align: 'left', formatter: function (value, row, index) {
                    if (row.Status == '已完成') {
                        return row.BidderMemo;
                    } else {
                        return '';
                    }
                }
            }
            ]],
            pagination: true,
            pageSize: PAGESIZE,
            pageList: [20, 40, 50],
            sortName: 'Published',
            sortOrder: 'desc'
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
    bindCtrl: function () {
        top.$('#txt_Inquirer').combobox({
            url: actionURL + "?" + createParam('dics', 15),
            textField: 'Title',
            valueField: 'KeyId',
            editable: false,
            required: true
        });
        top.$('#txt_Cargo').combobox({
            url: actionURL + "?" + createParam('dics', 18),
            textField: 'Title',
            valueField: 'Title',
            editable: true,
            required: true
        });
        top.$('#txt_Kind').combobox({
            url: actionURL + "?" + createParam('dics', 16),
            textField: 'Title',
            valueField: 'KeyId',
            editable: false,
            required: true
        });
        top.$('#txt_Packing').combobox({
            url: actionURL + "?" + createParam('dics', 19),
            textField: 'Title',
            valueField: 'KeyId',
            editable: false,
            required: true
        });
        top.$('#txt_Port').combobox({
            url: actionURL + "?" + createParam('dics', 21),
            textField: 'Title',
            valueField: 'Title',
            editable: true,
            required: true
        });

        top.$('#txt_Amount').combobox({
            url: '/sys/ashx/DicHandler.ashx?categoryId=22',
            valueField: 'Title',
            textField: 'Title',
            editable: true,
            required: true,
            required: true
        });
        top.$('#txt_Freebox').combobox({
            url: '/sys/ashx/DicHandler.ashx?categoryId=17',
            valueField: 'Title',
            textField: 'Title',
            editable: true,
            required: true
        });
        top.$('#txt_Weight').numberbox({
            min: 0,
            precision: 2,
            required: true
        });

        top.$('#uploadButton').click(function () {
            CRUD.upload();
        });

        top.$('#deleteAttachmentButton').click(function () {
            CRUD.delete_attachment();
        });

        var time = new Date();
        top.$('#txt_Published').val(time.getFullYear() + '-' +
            (time.getMonth() + 1) + '-' + time.getDate() + ' ' +
            time.getHours() + ':' + time.getMinutes());

        top.$('#txt_Ended').datetimebox({
            required: true,
            showSeconds: false,
            editable: false
        });

        top.$('#txt_Etd').datebox({
            required: true,
            editable: false
        });

        top.$('#txt_Eta').datebox({
            editable: true
        });

        top.$('#sendto').click(function () {
            myDeptTree.create();
            myDeptTree.init();
        });

        top.$('#txt_Status').val('未发送');
    },
    add: function () {
        var hDialog = top.jQuery.hDialog({
            title: '添加', width: 700, height: 550, href: formurl + '?d=' + (new Date()), iconCls: 'icon-add', buttons: [{
                text: '保存', handler: function () {
                    if (top.$('#uiform').form('validate')) {
                        //判断时间问题
                        var t1 = Date.parse(top.$('#txt_Published').val().replace(/-/g, '/'));
                        var t2 = Date.parse(top.$('#txt_Ended').datetimebox('getValue').replace(/-/g, '/'));

                        if ((t2 - t1) / 1000 / 60 / 60 < 4) {
                            alert('截止时间必须超过4个小时。');
                            return false;
                        }

                        if ($.trim(top.$('#txt_SupplyIds').val()) == '') {
                            alert('请选择航线。')
                            return false;
                        }

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
                },
                iconCls: 'icon-disk_black_magnify'
            },
            {
                text: '保存并发送',
                handler: function () {
                    if (top.$('#uiform').form('validate')) {
                        //判断时间问题
                        var t1 = Date.parse(top.$('#txt_Published').val().replace(/-/g, '/'));
                        var t2 = Date.parse(top.$('#txt_Ended').datetimebox('getValue').replace(/-/g, '/'));

                        if ((t2 - t1) / 1000 / 60 / 60 < 4) {
                            alert('截止时间必须超过4个小时。');
                            return false;
                        }

                        if ($.trim(top.$('#txt_SupplyIds').val()) == '') {
                            alert('请选择航线。')
                            return false;
                        }

                        var query = createParam('add', '0');
                        jQuery.ajaxjson(actionURL, query, function (d) {
                            if (parseInt(d) > 0) {
                                hDialog.dialog('close');

                                var rid = parseInt(d);
                                jQuery.ajaxjson(actionURL, createParam('send', rid), function (d) {
                                    if (d == 1) {
                                        msg.ok('已经发送询盘信息。')
                                        grid.reload();
                                    } else {
                                        MessageOrRedirect('发送失败！');
                                    }
                                });
                            } else {
                                MessageOrRedirect(d);
                            }
                        });
                    }
                    return false;
                },
                iconCls: 'icon-lightning_go'
            },
            {
                text: '关闭',
                handler: function () {
                    hDialog.dialog('close');
                },
                iconCls: 'icon-delete2'
            }
            ],
            onLoad: function () {
                CRUD.bindCtrl();

                var t = new Date();
                t.setTime(t.getTime() + 24 * 60 * 60 * 1000);
                top.$('#txt_Ended').datetimebox('setValue', t.getFullYear() + '-' + (t.getMonth() + 1) + '-' + t.getDate() + ' ' + t.getHours() + ':' + t.getMinutes());
                top.$('#deleteAttachmentButton').hide(); 
            }
        });

        top.$('#uiform').validate();
    },
    edit: function () {
        var row = grid.getSelectedRow();
        if (row) {
            if (row.Status == '未发送') {
                var hDialog = top.jQuery.hDialog({
                    title: '编辑', width: 700, height: 550, href: formurl + '?d=' + (new Date()), iconCls: 'icon-save',
                    onLoad: function () {
                        CRUD.bindCtrl();

                        top.$('#txt_Inquirer').combobox('setValue', row.Inquirer);
                        top.$('#txt_Published').val(row.Published);
                        top.$('#txt_Cargo').combobox('setValue', row.Cargo);
                        top.$('#txt_Amount').combobox('setValue', row.Amount);
                        top.$('#txt_Packing').combobox('setValue', row.Packing);
                        top.$('#txt_Weight').numberbox('setValue', row.Weight);
                        top.$('#txt_Port').combobox('setValue', row.Port);
                        top.$('#txt_Etd').datebox('setValue', row.ETD);
                        top.$('#txt_Eta').datebox('setValue', row.ETA);
                        top.$('#txt_Kind').combobox('setValue', row.Kind);
                        top.$('#txt_Ended').datetimebox('setValue', row.Ended);
                        top.$('#txt_Freebox').combobox('setValue', row.Freebox);
                        top.$('#txt_Memo').val(row.Memo);
                        top.$('#txt_Attachment').val(row.Attachment);
                        if ($.trim(row.Attachment) != '') {
                            var fn = row.Attachment.substring(row.Attachment.lastIndexOf('/') + 1);
                            top.$('#span_Attachment').text(fn.substring(fn.indexOf('_') + 1));
                            top.$('#deleteAttachmentButton').show();
                        } else {
                            top.$('#deleteAttachmentButton').hide();
                        }
                        top.$('#txt_SupplyIds').val(row.SupplyIds);
                        top.$('#txt_SupplyNames').val(row.SupplyNames);
                        top.$('#span_SupplyNames').text(row.SupplyNames);
                        top.$('#txt_Status').text(row.Status);
                    }, buttons: [{
                        text: '保存', handler: function () {
                            if (top.$('#uiform').form('validate')) {
                                //判断时间问题
                                var t1 = Date.parse(top.$('#txt_Published').val().replace(/-/g, '/'));
                                var t2 = Date.parse(top.$('#txt_Ended').datetimebox('getValue').replace(/-/g, '/'));

                                if ((t2 - t1) / 1000 / 60 / 60 < 4) {
                                    alert('截止时间必须超过4个小时。');
                                    return false;
                                }

                                if ($.trim(top.$('#txt_SupplyIds').val()) == '') {
                                    alert('请选择航线。')
                                    return false;
                                }

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
                        },
                        iconCls: 'icon-disk_black_magnify'
                    },
                    {
                        text: '保存并发送',
                        handler: function () {
                            if (top.$('#uiform').form('validate')) {
                                //判断时间问题
                                var t1 = Date.parse(top.$('#txt_Published').val().replace(/-/g, '/'));
                                var t2 = Date.parse(top.$('#txt_Ended').datetimebox('getValue').replace(/-/g, '/'));

                                if ((t2 - t1) / 1000 / 60 / 60 < 4) {
                                    alert('截止时间必须超过4个小时。');
                                    return false;
                                }

                                if ($.trim(top.$('#txt_SupplyIds').val()) == '') {
                                    alert('请选择航线。')
                                    return false;
                                }

                                var query = createParam('edit', row.KeyId);;
                                jQuery.ajaxjson(actionURL, query, function (d) {
                                    if (parseInt(d) > 0) {
                                        hDialog.dialog('close');

                                        var rid = row.KeyId;
                                        jQuery.ajaxjson(actionURL, createParam('send', rid), function (d) {
                                            if (d == 1) {
                                                msg.ok('已经发送询盘信息。')
                                                grid.reload();
                                            } else {
                                                MessageOrRedirect('发送失败！');
                                            }
                                        });
                                    } else {
                                        MessageOrRedirect(d);
                                    }
                                });
                            }
                            return false;
                        },
                        iconCls: 'icon-lightning_go'
                    },
                    {
                        text: '关闭',
                        handler: function () {
                            hDialog.dialog('close');
                        },
                        iconCls: 'icon-delete2'
                    }
                    ]
                });
            }else{
                var hDialog = top.jQuery.hDialog({
                    title: '查看', width: 700, height: 550, href: viewurl + '?d=' + (new Date()), iconCls: 'icon-save',
                    onLoad: function () {
                        top.$('#txt_Inquirer').text(row.Inquirer_Title);
                        top.$('#txt_Published').text(row.Published);
                        top.$('#txt_Cargo').text(row.Cargo);
                        top.$('#txt_Amount').text(row.Amount);
                        top.$('#txt_Packing').text(row.Packing_Title);
                        top.$('#txt_Weight').text(row.Weight);
                        top.$('#txt_Port').text(row.Port);
                        top.$('#txt_Etd').text(row.ETD);
                        top.$('#txt_Eta').text(row.ETA);
                        top.$('#txt_Kind').text(row.Kind_Title);
                        top.$('#txt_Ended').text(row.Ended);
                        top.$('#txt_Freebox').text(row.Freebox);
                        top.$('#txt_Memo').text(row.Memo);
                        if ($.trim(row.Attachment) != '') {
                            var fn = row.Attachment.substring(row.Attachment.lastIndexOf('/') + 1);
                            top.$('#txt_Attachment').html('<a href=\"' + row.Attachment + '\" target=\"_blank\">' + fn.substring(fn.indexOf('_') + 1) + '</a>');
                        }
                        top.$('#txt_SupplyNames').text(row.SupplyNames);
                    },
                    submit: function () {
                        hDialog.dialog('close');
                        return false;
                    }
                });
            }
        } else {
            msg.warning('请选择要修改的行。');
        }
    },
    del: function () {
        var row = grid.getSelectedRow();
        if (row) {
            if (row.Status == '未发送') {
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
                msg.warning('不能删除当前询盘信息！');
            }
        } else {
            msg.warning('请选择要删除的行。');
        }
    },
    send: function () {
        var row = grid.getSelectedRow();
        if (row) {
            if (row.Status == '未发送') {
                if (row.SupplyIds != '') {
                    //可以发送
                    if (confirm('确认发送询盘信息吗？')) {
                        var rid = row.KeyId;
                        jQuery.ajaxjson(actionURL, createParam('send', rid), function (d) {
                            if (d == 1) {
                                msg.ok('已经发送询盘信息。')
                                grid.reload();
                            } else {
                                MessageOrRedirect('发送失败！');
                            }
                        });
                    }
                } else {
                    msg.warning('请选择发送对象！');
                }
            } else {
                msg.warning('该询盘信息已经发送。');
            }
        } else {
            msg.warning('请选择要发送的询盘记录。');
        }
    },

    upload: function () {
        if (top.$('#fileToUpload')[0].files[0] == null) {
            msg.warning('请选择上传文件。');
            return;
        }

        var fd = new FormData();
        fd.append('fileToUpload', top.$('#fileToUpload')[0].files[0]);
        var xhr = new XMLHttpRequest();
        xhr.addEventListener('load', CRUD.upload_completed, false);
        xhr.addEventListener('error', CRUD.uploadFailed, false);
        xhr.addEventListener('abort', CRUD.uploadCanceled, false);
        xhr.upload.addEventListener('progress', CRUD.uploadProgress, false);
        xhr.open('POST', uploadURL);
        xhr.send(fd);
    },

    uploadProgress: function(evt){
        //if (evt.lengthComputable) {
        //    var percentComplete = Math.round(evt.loaded * 100 / evt.total);
        //    alert(percentComplete.toString() + '%');
        //}
        //else {
        //    alert('unable to compute');
        //}
    },

    delete_attachment: function (evt) {
        if (!confirm('确认删除附件吗？')) {
            return;
        }

        top.$('#txt_Attachment').val('');
        top.$('#span_Attachment').text('');
        top.$('#deleteAttachmentButton').hide();

        msg.ok('请保存，以删除附件。');
    },

    upload_completed: function (evt) {
        var ans = evt.target.responseText;
        if (ans.substring(0, 2) == '3,') {
            top.$('#txt_Attachment').val(ans.substring(3));

            var fn = ans.substring(ans.lastIndexOf('/') + 1);
            top.$('#span_Attachment').text(fn.substring(fn.indexOf('_') + 1));
            top.$('#deleteAttachmentButton').show();

            msg.ok('附件上传成功。');
        } else {
            msg.warning('文件上传失败！');
        }
    },

    upload_failed: function () {
        msg.warning('操作失败！');
    },

    upload_canceled: function () {
        msg.warning('操作取消！');
    },

    admin_delete: function () {
        var row = grid.getSelectedRow();
        if (row) {
            if (confirm('确认彻底删除当前的记录吗？这将删除询盘以及与之相关的报价信息。')) {
                var rid = row.KeyId;
                jQuery.ajaxjson(actionURL, createParam('admin_delete', rid), function (d) {
                    if (parseInt(d) > 0) {
                        msg.ok('删除成功！');
                        grid.reload();
                    } else {
                        MessageOrRedirect(d);
                    }
                });
            }
        } else {
            msg.warning('请选择要彻底删除的记录。');
        }
    },

    detail: function () {
        var row = grid.getSelectedRow();
        if (row) {
            if (row.Status == '已完成') {
                var hDialog = top.jQuery.hDialog({
                    title: '报价详情', width: 830, height: 480,
                    href: '/demo/html/LogisticsInquiryDetail.html',
                    iconCls: 'icon-table', buttons: [{
                        text: '关闭', handler: function () {
                            hDialog.dialog('close');
                            return false;
                        }, iconCls: 'icon-delete2'
                    }], cache: false,
                    onLoad: function () {
                        top.$('#left_btns').datagrid({
                            url: actionURL + '?' + createParam('detail', row.KeyId),
                            nowrap: false, //折行
                            fit: true,
                            rownumbers: false, //行号
                            striped: true, //隔行变色
                            idField: 'KeyId',//主键
                            singleSelect: true, //单选
                            frozenColumns: [[]],
                            columns: [[
                                { title: '排名', field: 'NewIndex', width: 40, align: 'center' },
                                { title: '报价公司', field: 'Truename', width: 200, align: 'left' },
                                { title: '报价', field: 'Price', width: 75, align: 'right' },
		                        { title: '船公司', field: 'Ship', width: 130, align: 'left' },
                                { title: 'ETD', field: 'ETD', width: 90, align: 'center' },
                                { title: 'ETA', field: 'ETA', width: 90, align: 'center' },
                                { title: '备注', field: 'Memo', width: 150, align: 'left' }
                            ]]
                        });
                    }
                });
            } else {
                msg.warning('询盘还未完成。');
            }
        } else {
            msg.warning('请选择记录。');
        }
    },
    copy: function () {
        var row = grid.getSelectedRow();
        if (row) {
            var hDialog = top.jQuery.hDialog({
                title: '增加', width: 700, height: 550, href: formurl + '?d=' + (new Date()), iconCls: 'icon-save',
                onLoad: function () {
                    CRUD.bindCtrl();

                    top.$('#txt_Inquirer').combobox('setValue', row.Inquirer);
                    top.$('#txt_Cargo').combobox('setValue', row.Cargo);
                    top.$('#txt_Amount').combobox('setValue', row.Amount);
                    top.$('#txt_Packing').combobox('setValue', row.Packing);
                    top.$('#txt_Weight').numberbox('setValue', row.Weight);
                    top.$('#txt_Port').combobox('setValue', row.Port);
                    top.$('#txt_Etd').datebox('setValue', row.ETD);
                    top.$('#txt_Eta').datebox('setValue', row.ETA);
                    top.$('#txt_Kind').combobox('setValue', row.Kind);
                    top.$('#txt_Freebox').combobox('setValue', row.Freebox);
                    top.$('#txt_Memo').val(row.Memo);
                    top.$('#txt_Attachment').val(row.Attachment);
                    if ($.trim(row.Attachment) != '') {
                        var fn = row.Attachment.substring(row.Attachment.lastIndexOf('/') + 1);
                        top.$('#span_Attachment').text(fn.substring(fn.indexOf('_') + 1));
                        top.$('#deleteAttachmentButton').show();
                    } else {
                        top.$('#deleteAttachmentButton').hide();
                    }
                    top.$('#txt_SupplyIds').val(row.SupplyIds);
                    top.$('#txt_SupplyNames').val(row.SupplyNames);
                    top.$('#span_SupplyNames').text(row.SupplyNames);

                    var t = new Date();
                    t.setTime(t.getTime() + 24 * 60 * 60 * 1000);
                    top.$('#txt_Ended').datetimebox('setValue', t.getFullYear() + '-' + (t.getMonth() + 1) + '-' + t.getDate() + ' ' + t.getHours() + ':' + t.getMinutes());
                }, buttons: [{
                    text: '保存', handler: function () {
                        if (top.$('#uiform').form('validate')) {
                            //判断时间问题
                            var t1 = Date.parse(top.$('#txt_Published').val().replace(/-/g, '/'));
                            var t2 = Date.parse(top.$('#txt_Ended').datetimebox('getValue').replace(/-/g, '/'));

                            if ((t2 - t1) / 1000 / 60 / 60 < 4) {
                                alert('截止时间必须超过4个小时。');
                                return false;
                            }

                            if ($.trim(top.$('#txt_SupplyIds').val()) == '') {
                                alert('请选择航线。')
                                return false;
                            }

                            var query = createParam('add', row.KeyId);;
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
                    },
                    iconCls: 'icon-disk_black_magnify'
                },
                {
                    text: '保存并发送',
                    handler: function () {
                        if (top.$('#uiform').form('validate')) {
                            //判断时间问题
                            var t1 = Date.parse(top.$('#txt_Published').val().replace(/-/g, '/'));
                            var t2 = Date.parse(top.$('#txt_Ended').datetimebox('getValue').replace(/-/g, '/'));

                            if ((t2 - t1) / 1000 / 60 / 60 < 4) {
                                alert('截止时间必须超过4个小时。');
                                return false;
                            }

                            if ($.trim(top.$('#txt_SupplyIds').val()) == '') {
                                alert('请选择航线。')
                                return false;
                            }

                            var query = createParam('add', row.KeyId);
                            jQuery.ajaxjson(actionURL, query, function (d) {
                                if (parseInt(d) > 0) {
                                    hDialog.dialog('close');
                                    
                                    var rid = parseInt(d);
                                    jQuery.ajaxjson(actionURL, createParam('send', rid), function (d) {
                                        if (d == 1) {
                                            msg.ok('已经发送询盘信息。')
                                            grid.reload();
                                        } else {
                                            MessageOrRedirect('发送失败！');
                                        }
                                    });
                                } else {
                                    MessageOrRedirect(d);
                                }
                            });
                        }
                        return false;
                    },
                    iconCls: 'icon-lightning_go'
                },
                {
                    text: '关闭',
                    handler: function () {
                        hDialog.dialog('close');
                    },
                    iconCls: 'icon-delete2'
                }
                ]
            });
        } else {
            msg.warning('请选择需要复制的行。');
        }
    }
};


var myDeptTree = {
    create: function(){
        var hDialog = top.jQuery.hDialog({
            title: '发送给', width: 258, height: 480, iconCls: 'icon-cog',
            submit: function () {
                //只允许选择一个根节点
                var root = top.$('#depTree').tree('getRoots');
                var count = 0;
                for (var i = 0; i < root.length; i++) {
                    if (root[i].checked) {
                        count++;
                    }
                }

                if (count == 0) {
                    alert('请选择货代公司。');
                    return false;
                }

                if (count > 1) {
                    alert('只能选择一条线路。');
                    return false;
                }

                for (var i = 0; i < root.length; i++) {
                    if (root[i].checked) {
                        root = root[i];
                        break;
                    }
                }

                top.$('#txt_SupplyIds').val(root.id);

                var names = [];
                var child = top.$('#depTree').tree('getChildren', root.target);
                for (var i = 0; i < child.length; i++) {
                    names[names.length] = child[i].text;
                }

                top.$('#span_SupplyNames').text(names.join(','));
                top.$('#txt_SupplyNames').val(names.join(','));

                hDialog.dialog('close');
                return false;
            },
            content: '<ul id="depTree"></ul>'
        });
    },

    init: function () {
        top.$('#depTree').tree({
            cascadeCheck: true, //是否联动选中节点
            lines: true, //显示连接线
            checkbox: true, //显示筛选框
            url: actionURL + '?' + createParam("depart_tree", 0),
            onLoadSuccess: function (node, data) {
                var id = top.$('#txt_SupplyIds').val();
                if (id != '') {
                    var n = top.$('#depTree').tree('find', id);
                    top.$('#depTree').tree('check', n.target);
                }
            }
        });
    }
};