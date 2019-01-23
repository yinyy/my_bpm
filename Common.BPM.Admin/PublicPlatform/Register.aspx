<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="BPM.Admin.PublicPlatform.Register" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
    <head>
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
        <meta name="viewport" content="width=device-width,initial-scale=1,user-scalable=no" />
        <title>用户绑定</title>
        <link href="scripts/weui/weui.css" rel="stylesheet" />
        <link href="css/common.css" rel="stylesheet" />
        <script src="scripts/zepto.js"></script>
        <script src="scripts/weui/weui.js"></script>
        <script src="scripts/template-web.js"></script>
        <script src="scripts/Staff.js?d=<%=DateTime.Now.Ticks %>"></script>
    </head>
<body>
    <div class="container">
        <div class="page hidden" id="page1">
            <div class="page__hd">
                <div class="page__title">
                    用户绑定
                </div>
                <div class="page__desc">
                    输入学号或教工编号，完成绑定，与教学质量监控系统实现数据共享。
                </div>
            </div>
            <div class="page_bd">
                <div class="weui-cells__title">用户信息</div>
                <div class="weui-cells weui-cells_form">
                    <div class="weui-cell weui-cell_select weui-cell_select-after">
                        <div class="weui-cell__hd">
                            <label for="" class="weui-label">用户类型</label>
                        </div>
                        <div class="weui-cell__bd">
                            <select class="weui-select" name="type" id="type">
                                <option value="none">请选择</option>
                                <option value="student">学生</option>
                                <option value="teacher">教师</option>
                            </select>
                        </div>
                    </div>
                    <div class="weui-cell">
                        <div class="weui-cell__hd"><label class="weui-label">学号工号</label></div>
                        <div class="weui-cell__bd">
                            <input class="weui-input" type="text" placeholder="请输入学号或工号" id="serial">
                        </div>
                    </div>
                </div>
            </div>
            <div class="weui-btn-area">
                <a class="weui-btn weui-btn_primary" href="javascript:" id="bindButton">绑定</a>
            </div>
        </div>

        <div class="page hidden" id="page2">
            <div class="page__hd">
                <div class="page__title">
                    完成绑定
                </div>
                <div class="page__desc">
                   当前微信号已完成绑定。
                </div>
            </div>
            <div class="page_bd">
                
            </div>
            <div class="weui-btn-area">
                <a class="weui-btn weui-btn_warn" href="javascript:" id="unbindButton">解除绑定</a>
            </div>
        </div>
    </div>

    <script id="tpl" type="text/html">
        <div class="weui-cells__title">用户信息</div>
        <div class="weui-cells weui-cells_form">
            <div class="weui-cell">
                <div class="weui-cell__hd">
                    <label class="weui-label">用户类型</label>
                </div>
                <div class="weui-cell__bd">
                    <label class="weui-input">{{Type}}</label>
                </div>
            </div>
            <div class="weui-cell">
                <div class="weui-cell__hd"><label class="weui-label">学号工号</label></div>
                <div class="weui-cell__bd">
                    <label class="weui-input">{{Serial}}</label>
                </div>
            </div>
            <div class="weui-cell">
                <div class="weui-cell__hd"><label class="weui-label">姓名</label></div>
                <div class="weui-cell__bd">
                    <label class="weui-input">{{Name}}</label>
                </div>
            </div>
        </div>
    </script>
    <script type="text/javascript">
        var nextUrl = '<%=Request.Params["nextUrl"] %>';
    </script>
</body>
</html>
