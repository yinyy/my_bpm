var handler = 'ashx/StaffHandler.ashx';
var oauth2Handler = '/PublicPlatform/ashx/OAuth2Handler.ashx';

$(function () {
    //先要拿到当前用户的openid
    $.getJSON(oauth2Handler, { 'action': 'openid' }, function (data) {
        if (data.Success) {
            //能够拿到openid，则根据openid来找staff
            $.getJSON(handler, { 'action': 'get' }, function (data) {
                if (data.Success) {
                    //用户已经完成了注册
                    $('#page1').addClass('hidden');
                    $('#page2').removeClass('hidden');

                    var html = template('tpl', {
                        'Type': data.Data.Type == 'student' ? '学生' : '教工',
                        'Serial': data.Data.Serial,
                        'Name': data.Data.Name
                    });
                    $('#page2 > .page_bd').html(html);
                } else {
                    $('#page1').removeClass('hidden');
                    $('#page2').addClass('hidden');
                }
            });
        } else {
            //没拿到openid，则OAuth2验证
            var returnUrl = 'http://course.dyzyxyydwlwsys.cc/PublicPlatform/Register.aspx';
            if (nextUrl != null) {
                returnUrl += '?nextUrl=' + nextUrl;
            }
            $.getJSON(oauth2Handler, { 'nextUrl': returnUrl }, function (data) {
                if (data.Success) {
                    document.location.href = data.Data;
                }
            });
        }
    });

    $('#bindButton').click(function () {
        var type = $('#type').val();
        var serial = $.trim($('#serial').val());
        var password = $.trim($('#password').val());

        if (type == 'none') {
            weui.alert('请选择用户类型。', {
                title: '提示'
            });
            return;
        }

        if (serial == '') {
            weui.alert('请输入学生学号或教工编号。', {
                title: '提示'
            });
            return;
        }

        //从教学质量监控系统获取用户的信息
        $.getJSON(handler, {
            'action': 'check',
            'serial': serial,
            'type': type,
            'password': password
        }, function (data) {
            if (data.Success) {
                var p = data.Data;

                //询问用户是否使用当前微信号绑定学号工号和姓名
                weui.confirm('将当前微信号与 ' + p.Name + ' 绑定吗？', function () {
                    $.post(handler, {
                        'action': 'bind',
                        'serial': p.Serial,
                        'name': p.Name,
                        'type': p.Type,
                        'gender': p.Gender
                    }, function (data) {
                        if (data.Success) {
                            weui.toast('绑定成功。', function () {
                                document.location.href = './Register.aspx';
                            }, {
                                    duration: 3000,
                                    className: "bears"
                                });
                        } else {
                            weui.alert('用户绑定失败，请稍后重试。', {
                                title: '提示'
                            });
                            return;
                        }
                    }, 'json');
                }, {
                        title: '提示'
                    });
            } else {
                weui.alert('未查询到学号或工号所对应的记录。请确认了正确的信息（学号、工号、密码等）。', {
                    title: '提示'
                });
                return;
            }
        });
    });

    $('#unbindButton').click(function () {
        //迅为用户是否解除绑定
        weui.confirm('确定解除绑定吗？', function () {
            $.getJSON(handler, { 'action': 'unbind' }, function (data) {
                if (data.Success) {
                    weui.toast('绑定成功。', function () {
                        document.location.href = './Register.aspx';
                    }, {
                            duration: 3000,
                            className: "bears"
                        });
                } else {
                    weui.alert('解除绑定失败，请稍后重试。', {
                        title: '提示'
                    });
                }
            });
        }, {
                title: '提示'
            });
    });
});
