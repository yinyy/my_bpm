var actionURL = '/Shopping/ashx/ShoppingCommodityHandler.ashx';
var formURL = '/Shopping/html/ShoppingCommodity.html';
var uploadURL = '/ashx/UploadHandler.ashx';

var currentImage;
var editor;

$(function () {
	autoResize({ dataGrid: '#list', gridType: 'datagrid', callback: grid.bind, height: 0 });

	$('#a_add').click(CRUD.add);
	$('#a_edit').click(CRUD.edit);
	$('#a_delete').click(CRUD.del);
	$('#a_export').click(CRUD.exp);

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
			title: "商品列表",
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
		        title: '图片', field: 'Picture', width: 150, align: 'center', formatter: function (v, r, i) {
		            return '<img class="Commodity_Picture" src="..' + v + '"/>';
		        }},
            { title: '编码', field: 'Code', width: 150, align: 'center' },
            { title: '名称', field: 'Title', width: 400, align: 'center' },
            { title: '售价', field: 'Price', width: 90, align: 'right', formatter: function (v, r, i) { return '￥'+(v/100.0).toFixed(2)} },
            { title: '积分可抵', field: 'Point', width: 90, align: 'right', formatter: function (v, r, i) { return v + '%'; } },
            { title: '排序', field: 'Sorting', width: 100, align: 'center' },
			{ title: '详情', field: 'Details', width: 100, align: 'center', formatter: function (v, r, i) { return '<a href="#">商品详情</a>';} },
            { title: '图库', field: 'Gallery', width: 100, align: 'center', formatter: function (v, r, i) { return '<a href="#">商品图库</a>'; } }
            ]],
			pagination: true,
			pageSize: PAGESIZE,
			pageList: [20, 40, 50],
			sortName: 'KeyId',
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
    add: function () {
        var wd = $(document).width();
        var ht = $(document).height();

		var hDialog = top.jQuery.hDialog({
			title: '添加商品', width: wd, height: ht, href: formURL, iconCls: 'icon-add', submit: function () {
			    if (top.$('#uiform').form('validate')) {
			        var price = parseInt($.trim(top.$('#Commodity_Price').val()));
                    if (isNaN(price)) {
			            alert('价格请填写数字。');
			            return false;
			        }

			        var point = parseInt(top.$('#Commodity_Point').val());
			        if(isNaN(point)){
			            alert('积分抵扣请填写数字。')
			            return false;
			        }

			        var gs = [];
			        top.$('ul.gallery > li > img').each(function(){
			            gs[gs.length] = $(this).attr('src');
			        });

				    var query = 'json=' + JSON.stringify({
				        action: 'add', keyid: 0, jsonEntity: JSON.stringify({
				            Code: top.$('#Commodity_Code').val(),
				            Title: $.trim(top.$('#Commodity_Title').val()),
				            Price: price,
				            Point: point,
				            Picture: top.$('#Commodity_Picture').attr('src'),
				            Gallery: gs,
                            Sorting: top.$('input[name="Sorting"]:checked').val(),
                            Details: editor.getSource()
				        })
				    });

				    jQuery.ajaxjson(actionURL, query, function (d) {
						if (d.Success) {
							msg.ok('添加成功！');
							hDialog.dialog('close');
							grid.reload();
						} else {
							MessageOrRedirect(d.Message);
						}
					});
				}
				return false;
			}, onLoad: function () {
			    editor = top.$('#Commodity_Details').xheditor(
                    {
                        upLinkUrl: uploadURL+'?source=xheditor',
                        upImgUrl: uploadURL + '?source=xheditor',
                        upFlashUrl: uploadURL + '?source=xheditor',
                        upMediaUrl: uploadURL + '?source=xheditor',
                        remoteImgSaveUrl: uploadURL + '?source=xheditor'
                    });

			    new qq.FineUploader({
			        debug: true,
			        element: top.$('#fine-uploader').get(0),
			        template: top.$('#qq-template').get(0),
			        request: {
			            endpoint: '/ashx/UploadHandler.ashx'
			        },
			        multiple: false,
			        allowedExtensions: ['.png', '.jpg'],
			        retry: {
			            enableAuto: false
			        },
			        callbacks: {
			            onComplete: function (id, name, json, xhr) {
			                if (currentImage != null) {
			                    currentImage.attr('src', json.url);
			                } else {
			                    var li = $('<li></li>');
			                    var img = $('<img src="' + json.url + '" width="160" height="120"/>');
			                    img.dblclick(function () {
			                        if (confirm('确定删除图片吗？')) {
			                            img.parent().remove();
			                        }
			                    })
			                    li.append(img);
			                    top.$('#More_Picture').parent().before(li);
			                }

			                currentImage = null;
			            },
			            onError: function (id, name, reason, xhr) {
			                msg.warning('上传失败！');
			            },
			            onSubmit: function (id, name) {
			                //alert('submit');
			            },
			            onSubmitted: function (id, name) {
			                //alert('submitted');
			            }
			        }
			    });

			    top.$('#Commodity_Picture').dblclick(function () {
			        currentImage = $(this);
			        top.document.getElementsByName('qqfile')[0].click();
			    });
			    top.$('#More_Picture').click(function () {
			        currentImage = null;
			        top.document.getElementsByName('qqfile')[0].click();
			    });
			}
		});

		top.$('#uiform').validate();
	},
	del: function () {
		var row = grid.getSelectedRow();
		if (row) {
			if (confirm('确认删除选中的洗车卡吗？')) {
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
			msg.warning('请选择记录。');
		}
	},
	edit: function () {
		var row = grid.getSelectedRow();
		if (row) {
		    var wd = $(document).width();
		    var ht = $(document).height();

			var hDialog = top.jQuery.hDialog({
				title: '编辑', width: wd, height: ht, href: formURL, iconCls: 'icon-edit', submit: function () {
					if (top.$('#uiform').form('validate')) {
					    var price = parseInt($.trim(top.$('#Commodity_Price').val()));
					    if (isNaN(price)) {
					        alert('价格请填写数字。');
					        return false;
					    }

					    var point = parseInt(top.$('#Commodity_Point').val());
					    if(isNaN(point)){
					        alert('积分抵扣请填写数字。')
					        return false;
					    }

					    var gs = [];
					    top.$('ul.gallery > li > img').each(function(){
					        gs[gs.length] = $(this).attr('src');
					    });

					    var query = 'json=' + JSON.stringify({
					        action: 'edit', keyid: row.KeyId, jsonEntity: JSON.stringify({
					            Code: top.$('#Commodity_Code').val(),
					            Title: $.trim(top.$('#Commodity_Title').val()),
					            Price: price,
					            Point: point,
					            Picture: top.$('#Commodity_Picture').attr('src'),
					            Gallery: gs,
					            Sorting: top.$('input[name="Sorting"]:checked').val(),
					            Details: editor.getSource()
					        })
					    });

					    jQuery.ajaxjson(actionURL, query, function (d) {
					        if (d.Success) {
					            msg.ok('保存成功！');
					            hDialog.dialog('close');
					            grid.reload();
					        } else {
					            MessageOrRedirect(d.Message);
					        }
					    });
					}
				    return false;
				}, onLoad: function () {
				    editor = top.$('#Commodity_Details').xheditor(
                    {
                        upLinkUrl: 'demos/upload.php?immediate=1',
                        upImgUrl: 'demos/upload.php?immediate=1',
                        upFlashUrl: 'demos/upload.php?immediate=1',
                        upMediaUrl: 'demos/upload.php?immediate=1',
                        remoteImgSaveUrl: 'demos/saveremoteimg.php'
                    });

				    new qq.FineUploader({
				        debug: true,
				        element: top.$('#fine-uploader').get(0),
				        template: top.$('#qq-template').get(0),
				        request: {
				            endpoint: '/ashx/UploadHandler.ashx'
				        },
				        multiple: false,
				        allowedExtensions: ['.png', '.jpg'],
				        retry: {
				            enableAuto: false
				        },
				        callbacks: {
				            onComplete: function (id, name, json, xhr) {
				                if (currentImage != null) {
				                    currentImage.attr('src', json.url);
				                } else {
				                    var li = $('<li></li>');
				                    var img = $('<img src="' + json.url + '" width="160" height="120"/>');
				                    img.dblclick(function () {
				                        if (confirm('确定删除图片吗？')) {
				                            img.parent().remove();
				                        }
				                    })
				                    li.append(img);
				                    top.$('#More_Picture').parent().before(li);
				                }

				                currentImage = null;
				            },
				            onError: function (id, name, reason, xhr) {
				                msg.warning('上传失败！');
				            },
				            onSubmit: function (id, name) {
				                //alert('submit');
				            },
				            onSubmitted: function (id, name) {
				                //alert('submitted');
				            }
				        }
				    });

				    top.$('#Commodity_Picture').dblclick(function () {
				        currentImage = $(this);
				        top.document.getElementsByName('qqfile')[0].click();
				    });
				    top.$('#More_Picture').click(function () {
				        currentImage = null;
				        top.document.getElementsByName('qqfile')[0].click();
				    });

				    top.$('#Commodity_Code').val(row.Code);
				    top.$('#Commodity_Title').val(row.Title);
				    top.$('#Commodity_Price').val(row.Price);
				    top.$('#Commodity_Point').val(row.Point);
				    top.$('#Commodity_Picture').attr('src', row.Picture);
				    if (row.Sorting == 100) {
				        top.$('#Commodity_High').checked();
				    }
				}
			});

			top.$('#uiform').validate();
		} else {
			msg.warning('请选择洗车卡。');
		}
	},
	consume: function () {
		var row = grid.getSelectedRow();
		if (row) {
			var hDialog = top.jQuery.hDialog({
				title: '记录', width: 550, height: 655, content: '<div id="tt"></div>', iconCls: 'icon-list'
			});

			top.$('#tt').tabs({
				border: false,
				width: 530,
				height: 578
			});
			top.$('#tt').tabs('add', {
				title: '消费记录',
				content: '<table id="dg1"></table>',
				closable: false
			});
			top.$('#tt').tabs('add', {
				title: '充值记录',
				content: '<table id="dg2"></table>',
				closable: false
			});
			top.$('#tt').tabs('select', 0);

			top.$('#dg1').datagrid({
				url: actionURL + '?' + "json=" + JSON.stringify({ action: 'consume', keyid: row.KeyId }),
				width: 530,
				height: 549,
				nowrap: false, //折行
				rownumbers: true, //行号
				striped: true, //隔行变色
				idField: 'KeyId',//主键
				singleSelect: true, //单选
				columns: [[
                    {
                    	field: 'Time', title: '消费时间', align: 'center', width: 170, formatter(v, r, i) {
                    		return v.substring(0, 19);
                    	}
                    },
                {
                	field: 'Coins', title: '消费洗车币', width: 100, align: 'right', formatter(v, r, i) {
                		return '￥' + (v * (-1)).toFixed(2);
                	}
                }
				]],
				pagination: true,
				pageSize: PAGESIZE,
				pageList: [20, 40, 50],
				sortName: 'Time',
				sortOrder: 'desc'
			});

			top.$('#dg2').datagrid({
				url: actionURL + '?' + "json=" + JSON.stringify({ action: 'recharge', keyid: row.KeyId }),
				width: 530,
				height: 549,
				nowrap: false, //折行
				rownumbers: true, //行号
				striped: true, //隔行变色
				idField: 'KeyId',//主键
				singleSelect: true, //单选
				columns: [[
                    {
                    	field: 'Time', title: '消费时间', align: 'center', width: 170, formatter(v, r, i) {
                    		return v.substring(0, 19);
                    	}
                    },
                {
                	field: 'Coins', title: '消费洗车币', width: 100, align: 'right', formatter(v, r, i) {
                		return '￥' + v.toFixed(2);
                	}
                }
				]],
				pagination: true,
				pageSize: PAGESIZE,
				pageList: [20, 40, 50],
				sortName: 'Time',
				sortOrder: 'desc'
			});
		} else {
			msg.warning('请选择设备。');
		}
	},
	test: function () {
		var query = createParam('coupon', '0');
		jQuery.ajaxjson(actionURL, query, function (d) {
			if (parseInt(d) > 0) {
				msg.ok('添加成功！');
				grid.reload();
			} else {
				msg.warning('添加失败！');
				MessageOrRedirect(d);
			}
		});
	},
	batch: function () {
		var hDialog = top.jQuery.hDialog({
			title: '添加', width: 450, height: 290, href: batchFormURL, iconCls: 'icon-add', submit: function () {
				var start = top.$('#txt_Start').numberbox('getValue');
				var end = top.$('#txt_End').numberbox('getValue');
				if (start >= end) {
					alert('起止卡号错误。');
					return false;
				}

				start = top.$('#txt_ValidateFrom').datebox('getValue');
				end = top.$('#txt_ValidateEnd').datebox('getValue');
				if (start >= end) {
					alert('有效时间错误。');
					return false;
				}

				if (top.$('#uiform').form('validate')) {
					var query = createParam('batch', '0');
					jQuery.ajaxjson(actionURL, query, function (d) {
						if (parseInt(d) >= 0) {
							msg.ok('批量生成成功！');
							hDialog.dialog('close');
							grid.reload();

							alert('批量生成洗车卡：' + d + '张。');
						} else if (parseInt(d) < 0) {
							msg.warning('批量生成失败！');
						} else {
							MessageOrRedirect(d);
						}
					});
				}
				return false;
			}, onLoad: function () {
				top.$('#txt_ValidateFrom').datebox({
					required: true,
					editable: false
				});
				top.$('#txt_ValidateEnd').datebox({
					required: true,
					editable: false
				});
			}
		});

		top.$('#uiform').validate();
	},
	exp: function () {
		var o = { action: 'export', keyid: 0 };
		var query = "json=" + JSON.stringify(o);

		if ($('body').data('where') != null && $('body').data('where') != '') {
			query = query + "&filter=" + $('body').data('where');
		}

		window.open(actionURL + '?' + query);
	}
};