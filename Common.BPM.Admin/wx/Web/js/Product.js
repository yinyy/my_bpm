var handler = '/wx/Web/ashx/ProductHandler.ashx';

function is_wechat_client() {
    var ua = navigator.userAgent.toLowerCase();
    if (ua.match(/MicroMessenger/i) == "micromessenger") {
        return true;
    } else {
        return false;
    }
}  

$(document).ready(function () {
    if (is_wechat_client()) {
        $('div#registe_product').removeClass('hidden');
    } else {
        $('div#not_in_wechat').removeClass('hidden');

        return;
    }

    //获得安装地点信息
    var o = {};
    o.action = 'params';
    o.keyid = parseInt($.trim($('input#deptId').val()));
    var json = 'json=' + JSON.stringify(o);

    $.post(handler, json, function (data) {
        $('select#Address').empty();
        var opt = $('<option value="">请选择</option>');
        $('select#Address').append(opt);
        $(data.Addresses).each(function (index, d) {
            var opt = $('<option value="' + d + '">' + d + '</option>');
            $('select#Address').append(opt);
        });

        $('select#Model').empty();
        var opt = $('<option value="">请选择</option>');
        $('select#Model').append(opt);
        $(data.Models).each(function (index, d) {
            var opt = $('<option value="' + d.Code + '">' + d.Title + '</option>');
            $('select#Model').append(opt);
        });
    }, 'json');


    $('a#saveButton').click(function () {
        var model = $.trim($('#Model').val());
        var serial = $.trim($('#Serial').val());
        var address = $.trim($('#Address').val());
        var type = $.trim($('#Type').val());
        var plate = $.trim($('#Plate').val());
        var trouble = $('#Trouble').prop('checked');
        var detail = $.trim($('#Detail').val());
        var owner = $.trim($('#Owner').val());
        var phone = $.trim($('#Phone').val());
        var driving = $.trim($('#Driving').val());

        //验证是否都填写了数据
        if (model == '') {
            $.alert("请选择所安装产品的型号。", "提示");
            return;
        }
        if (serial == '') {
            $.alert("请输入所安装产品的序列号。", "提示");
            return;
        }
        if (address == '') {
            $.alert("请选产品安装地点。", "提示");
            return;
        }

        if (type == '') {
            $.alert("请输入安装车辆的型号。", "提示");
            return;
        }
        if (plate == '') {
            $.alert("请输入安装车辆的车牌号。", "提示");
            return;
        }
        if (driving == '') {
            $.alert("请输入车辆行驶里程。", "提示");
            return;
        }
        if (isNaN(driving)) {
            $.alert("车辆行驶里程为纯数字。请重新输入。", "提示");
            return;
        }
        if (trouble && detail == '') {
            $.alert("请详细描述车辆目前存在的故障。", "提示");
            return;
        }

        if (owner == '') {
            $.alert("请输入车主姓名。", "提示");
            return;
        }
        if (phone == '') {
            $.alert("请输入车主联系方式。", "提示");
            return;
        }

        var query = {
            DepartmentId: $('input#deptId').val(),
            Model: model,
            Serial: serial,
            Address: address,
            Type: type,
            Plate: plate,
            Trouble: trouble,
            Detail: detail,
            Owner: owner,
            Phone: phone,
            Driving: parseInt(driving)
        };

        var o = {};
        o.jsonEntity = JSON.stringify(query);
        o.action = 'registe';
        var json = 'json=' + JSON.stringify(o);
        
        $.post(handler, json, function (d) {
            if (d.Success == true) {
                $('div#done_result > div > i').removeClass('weui-icon-warn');
                $('div#done_result > div > i').addClass('weui-icon-success');
                
                $('div#done_result > div > h2').text('操作成功');
                $('div#done_result > div > p').text('成功登记产品信息');

                $('div#registe_product').addClass('hidden');
                $('div#done_result').removeClass('hidden');
            } else {
                var msg = d.Message;

                $('div#done_result > div > i').removeClass('weui-icon-success');
                $('div#done_result > div > i').addClass('weui-icon-warn');

                $('div#done_result > div > h2').text('操作失败');

                if (msg == 'product_has_registed') {
                    $('div#done_result > div > p').text('产品序列号已经被登记过。请核对产品序列号。');
                } else if(msg=='saved_error') {
                    $('div#done_result > div > p').text('登记产品信息时发生错误，请稍候重试。');
                }

                $('div#registe_product').addClass('hidden');
                $('div#done_result').removeClass('hidden');
            }
        }, 'json');
    });
});