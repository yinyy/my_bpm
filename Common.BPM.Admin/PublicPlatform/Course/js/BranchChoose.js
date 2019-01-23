var staffBranchHandler = "ashx/StaffBranchHandler.ashx";
var oauth2Handler = "/PublicPlatform/ashx/OAuth2Handler.ashx";
var staffHandler = "/PublicPlatform/ashx/StaffHandler.ashx";
var branchHandler = 'ashx/BranchHandler.ashx';

$(function () {
    //验证用户是否已经通过oauth
    oauth();

    //保存按钮的点击事件
    $('#updateButton').click(function () {
        var branches = [];
        $.each($('.select__bd > select'), function (idx, s) {
            branches[branches.length] = $(s).val();
        });
        if (branches[0] == '0') {
            weui.alert('请选择第一专业方向。', {
                title: '提示'
            });
            return;
        }

        var introduction = $.trim($('#Introduction').val());
        branches = branches.join(',');

        weui.confirm('确定保存当前选择吗？', function () {
            //保存选择
            $.post(staffBranchHandler, { 'action': 'save', 'branches': branches, 'introduction': introduction }, function (data) {
                if (!data.Success) {
                    weui.alert('专业方向选择保存失败。请稍后重试。', {
                        title: '提示'
                    });
                } else {
                    weui.alert('专业方向选择保存成功。', {
                        title: '提示'
                    });
                }
            }, 'json');
        }, {
            title: '提示'
        });

    });

    //个人说明变动
    $('#Introduction').on('input focus keyup', function (ev) {
        var totalCount = 500;
        var wordCount = $.trim($(this).val()).length;
        var remaining = totalCount - wordCount;
        if (remaining <= 0) {
            $(this).val($(this).val().slice(0, totalCount));
            $('#overage').text(0);
        } else {
            $('#overage').text(remaining);
        }
        ev.stopPropagation();
    });
});

function oauth() {
    $.getJSON(oauth2Handler, { 'action': 'openid' }, function (data) {
        if (data.Success) {
            //有了openid，验证用户（学生）是否已经完成绑定
            checkStaff();
        } else {
            //进行oauth
            var returnUrl = 'http://course.dyzyxyydwlwsys.cc/PublicPlatform/Course/BranchChoose.aspx';
            $.getJSON(oauth2Handler, { 'nextUrl': returnUrl }, function (data) {
                if (data.Success) {
                    document.location.href = data.Data;
                }
            });
        }
    });
}

//获得用户信息
function checkStaff() {
    $.getJSON(staffHandler, { 'action': 'get' }, function (data) {
        if (data.Success) {
            data = data.Data;
            if (data.Type == 'teacher') {
                weui.alert('教师用户不能使用此功能。', function () {
                    window.history.back();
                },{
                    title: '提示'
                });
            } else {
                //加载用户所选择的专业方向
                loadStaffBranches();
            }
        } else {
            //还没有完成绑定，进入绑定界面
            document.location.href = 'http://course.dyzyxyydwlwsys.cc/PublicPlatform/Register.aspx?nextUrl=http://course.dyzyxyydwlwsys.cc/PublicPlatform/Course/BranchChoose.aspx';
        }
    });
}

//加载用户所选择的专业方向
function loadStaffBranches() {
    $.getJSON(staffBranchHandler, { 'action': 'list' }, function (data) {
        if (!data.Success) {
            weui.alert('获取数据失败。请稍后重试。', {
                title: '提示'
            });
        } else {
            var accept = 0;
            staffBranches = data.Data;
            $.each(staffBranches, function (idx, d) {
                accept += d.Accepted;
            });

            if (accept == 0) {
                $('#page1').removeClass('hidden');

                //加载界面元素
                //获取全部的专业方向，填充select
                $.getJSON(branchHandler, { 'action': 'list' }, function (data) {
                    if (!data.Success) {
                        weui.alert('获取数据失败。', {
                            title: '提示'
                        });
                    } else {
                        $('.select__bd').html('');
                        var html = template('tpl1', data);
                        $('.select__bd').html(html);

                        //填充已有数据
                        var selects = $('.select__bd > select');
                        $.each(staffBranches, function (idx, d) {
                            if (idx == 0) {
                                $('#Introduction').text(d.Introduction);
                            }

                            $(selects.get(idx)).val(d.BranchId);
                        });
                    }
                });
            } else {
                //找到录取的那个方向
                var staffBranch = null;
                $.each(staffBranches, function (idx, d) {
                    if (d.Accepted == 1) {//1是被录取
                        staffBranch = d;
                    }
                });

                //获取其详细信息
                $.getJSON(branchHandler, { 'action': 'detail', 'id': staffBranch.BranchId }, function (data) {
                    if (!data.Success) {
                        weui.alert('获取数据失败。', {
                            title: '提示'
                        });
                    } else {
                        $('#page2').html('');
                        var html = template('tpl2', data.Data);
                        $('#page2').html(html);

                        $('#page2').removeClass('hidden');
                    }
                });
            }
        }
    });
}