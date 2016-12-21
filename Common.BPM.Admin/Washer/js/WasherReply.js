var actionUrl = '/Washer/ashx/WasherReplyHandler.ashx';
var formUrl = '/Washer/html/WasherReply.html';
var currentImage;

$(function () {
    autoResize({ dataGrid: '#tabs', gridType: 'tabs', callback: tab.bind, height: 0 });

    var uploader = new qq.FineUploader({
        debug: true,
        element: document.getElementById('fine-uploader'),
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
                currentImage.attr('src', json.url);

                msg.ok('上传成功。');
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
});

function createParam(action, keyid) {
    var o = {};
    var query = top.$('#uiform').serializeArray();
    query = convertArray(query);
    o.jsonEntity = JSON.stringify(query);
    o.action = action;
    o.keyid = keyid;
    return "json=" + JSON.stringify(o);
}

var tab = {
    bind: function (winSize) {
        $('#tabs').tabs({
            onSelect: function (title, index) {
                var tab = $('#tabs').tabs('getSelected');

                tab.html('');

                createTabContent(tab, index);
                initTabContent(tab, index);
            },
            height: winSize.height
        });
    }
};

function addRow(tab) {
    var divControl = tab.children().children('div.control');
    
    var div = $('<div class="little"></div>');
    divControl.before(div);

    div.append('<hr/>');
    var table = $('<table><tr><td class="title">标题</td><td style="height:70px;width:70px"><img class="little" src="/images/little.png" /></td></tr></table>');
    div.append(table);

    var divUrl = $('<div class="url"></div>');
    div.append(divUrl);

    var divDesc = $('<div class="desc"></div>');
    div.append(divDesc);

    table.children().children().children(':first').dblclick(function () {
        editTitle($(this), divUrl, divDesc);
    });

    table.children().children().children().children('img').dblclick(function () {
        editPicture($(this));
    });


    return div;
}

function createTabContent(tab, index) {
    var divSetting = $('<div class="setting"></div>');
    tab.append(divSetting);

    var divBig = $('<div class="big"></div>');
    divSetting.append(divBig);

    var img = $('<img src="/images/empty.jpg" class="big" alt="封面"/>');
    divBig.append(img);
    img.dblclick(function () {
        editPicture($(this));
    });

    var divUrl = $('<div class="url"></div>');
    divBig.append(divUrl);

    var divDesc = $('<div class="desc"></div>');
    divBig.append(divDesc);

    var divTitle = $('<div class="title">标题</div>');
    divBig.append(divTitle);
    divTitle.dblclick(function () {
        editTitle($(this), divUrl, divDesc);
    });

    var divControl = $('<div class="control"></div>');
    divSetting.append(divControl);

    var btnAdd = $('<a id="add" href="#">增加</a>');
    divControl.append(btnAdd);

    var spanSpace = $("<span>&nbsp;&nbsp;</span>");
    divControl.append(spanSpace);

    var btnSave = $('<a id="save" href="#">保存</a>');
    divControl.append(btnSave);

    //-------------------end---------------------//

    btnAdd.linkbutton({
        iconCls: 'icon-add'
    });
    btnAdd.click(function () {
        addRow(tab);
    });

    btnSave.linkbutton({
        iconCls: 'icon-save'
    });
    btnSave.click(function () {
        var os = [];
        var o = { title: divBig.children('div.title').text(), url: divBig.children('div.url').text(), description: divBig.children('div.desc').text(), picture: divBig.children('img').attr('src') };
        os[os.length] = o;

        divSetting.children('div.little').each(function () {
            o = {
                title: $(this).children('table').children().children().children('td.title').text(),
                url: $(this).children('div.url').text(),
                description: $(this).children('div.desc').text(),
                picture: $(this).children('table').children().children().children('td').children('img').attr('src')
            };

            os[os.length] = o;
        });
        
        var t = { Kind: '', Body: JSON.stringify(os) };
        if (index == 0) {
            t.Kind = 'SCAN';
        } else if (index == 1) {
            t.Kind = 'SUBSCRIBE';
        } else if (index == 2) {
            t.Kind = 'OTHER';
        }

        var query = "json=" + JSON.stringify({ jsonEntity: JSON.stringify(t), action: 'update' }); 
        jQuery.ajaxjson(actionUrl, query, function (d) {
            if (parseInt(d) > 0) {
                msg.ok('保存成功！');
            } else {
                msg.warning('保存失败！');
            }
        });
    });
}

function initTabContent(tab, index) {
    var t = { Kind: '' };
    if (index == 0) {
        t.Kind = 'SCAN';
    } else if (index == 1) {
        t.Kind = 'SUBSCRIBE';
    } else if (index == 2) {
        t.Kind = 'OTHER';
    }

    jQuery.ajaxjson(actionUrl, "json=" + JSON.stringify({ jsonEntity: JSON.stringify(t) }), function (d) {
        if (d != null) {
            var ns = eval("(" + d.Body + ")");
            
            tab.children().children('div.big').children('div.title').text(ns[0].title);
            tab.children().children('div.big').children('div.url').text(ns[0].url);
            tab.children().children('div.big').children('div.desc').text(ns[0].description);
            tab.children().children('div.big').children('img').attr('src', ns[0].picture);
            
            for (var i = 1; i < ns.length; i++) {
                var div = addRow(tab);
                var tr = div.children('table').children().children();
                tr.children('td:first').text(ns[i].title);
                div.children('div.url').text(ns[i].url);
                div.children('div.desc').text(ns[i].description);
                tr.children('td:last').children('img').attr('src', ns[i].picture);
            }
        }
    });
}

function editTitle(title, url, desc) {
    var hDialog = top.jQuery.hDialog({
        title: '参数设置', width: 450, height: 255, href: formUrl, iconCls: 'icon-edit', submit: function () {
            return false;
        }, onLoad: function () {
            top.$('#txt_Title').val(title.text());
            top.$('#txt_Description').val(desc.text());
            
            if (url.text() == '#Coin') {
                top.$('#rb_Coin').attr('checked', 'checked');
                top.$('#txt_Url').attr('readonly', 'readonly');
            } else if (url.text() == '#Pay') {
                top.$('#rb_Pay').attr('checked', 'checked');
                top.$('#txt_Url').attr('readonly', 'readonly');
            } else {
                top.$('#rb_Normal').attr('checked', 'checked');
                top.$('#txt_Url').val(url.text());
                top.$('#txt_Url').removeAttr('readonly');
            }

            top.$('#rb_Pay, #rb_Coin').click(function () {
                top.$('#txt_Url').attr('readonly', 'readonly');
            });

            top.$('#rb_Normal').click(function () {
                top.$('#txt_Url').removeAttr('readonly');
            });
        }, buttons: [{
            text: '保存',
            iconCls: 'icon-ok',
            handler: function () {
                title.text(top.$('#txt_Title').val());
                desc.text(top.$('#txt_Description').val());

                if (top.$('#rb_Coin').attr('checked') == 'checked') {
                    url.text('#Coin');
                } else if (top.$('#rb_Pay').attr('checked') == 'checked') {
                    url.text('#Pay');
                } if (top.$('#rb_Normal').attr('checked') == 'checked') {
                    url.text(top.$('#txt_Url').val());
                }

                hDialog.dialog('close');
            }
        }, {
            text: '删除',
            iconCls: 'icon-delete',
            handler: function () {
                var index = $('div.big > div.title, div.little td.title').index(title);
                if (index == 0) {
                    alert('不能删除封页。');
                } else {
                    if (confirm('确认删除 ' + title.text() + ' 吗？')) {
                        $($('div#setting > div').get(index)).remove();
                    }
                }
            }
        }, {
            text: '关闭',
            iconCls: 'icon-cancel',
            handler: function () {
                hDialog.dialog('close');
            }
        }]
    });
}

function editPicture(img) {
    currentImage = img;
    document.getElementsByName('qqfile')[0].click();
}