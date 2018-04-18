<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FivePowerProduct.aspx.cs" Inherits="BPM.Admin.wx.Web.FivePowerProduct" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width,initial-scale=1,user-scalable=no">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <link href="./css/weui.css" rel="stylesheet" />
    <link href="./css/jquery-weui.css" rel="stylesheet" />
    <link href="./css/Product.css?d=<%=DateTime.Now %>" rel="stylesheet" />
    <script src="./js/jquery-2.1.4.js"></script>
    <script src="./js/jquery-weui.js"></script>
    <script src="./js/Product.js?t=<%=DateTime.Now %>"></script>
    <title>产品登记</title>
</head>

<body>
    <form id="uiform" runat="server">
        <div id="not_in_wechat" class="hidden">
            <i class="weui-icon-warn weui-icon_msg"></i>
            <p>请在微信浏览器中进行操作。</p>
        </div>
        
        <div id="registe_product" class="hidden">
            <input type="hidden" id="deptId" value="<%=Request.Params["tag"] %>" />
            
            <div class="weui-cells__title">产品信息</div>
            <div class="weui-cells weui-cells_form">
                <div class="weui-cell">
                    <div class="weui-cell__hd">
                        <label class="weui-label">型号</label>
                    </div>
                    <div class="weui-cell__bd">
                        <select id="Model" class="weui-select"></select>
                    </div>
                </div>

                <div class="weui-cell">
                    <div class="weui-cell__hd">
                        <label class="weui-label">序列号</label>
                    </div>
                    <div class="weui-cell__bd">
                        <input id="Serial" class="weui-input" type="text" placeholder="请输入产品序列号">
                    </div>
                </div>

                <div class="weui-cell">
                    <div class="weui-cell__hd">
                        <label class="weui-label">安装地点</label>
                    </div>
                    <div class="weui-cell__bd">
                        <select id="Address" class="weui-select"></select>
                    </div>
                </div>
            </div>

            <div class="weui-cells__title">车辆信息</div>
            <div class="weui-cells weui-cells_form">
                <div class="weui-cell">
                    <div class="weui-cell__hd">
                        <label class="weui-label">车型</label>
                    </div>
                    <div class="weui-cell__bd">
                        <input id="Type" class="weui-input" type="text" placeholder="请输入车型">
                    </div>
                </div>

                <div class="weui-cell">
                    <div class="weui-cell__hd">
                        <label class="weui-label">车牌号</label>
                    </div>
                    <div class="weui-cell__bd">
                        <input id="Plate" class="weui-input" type="text" placeholder="请输入车牌号">
                    </div>
                </div>

                <div class="weui-cell">
                    <div class="weui-cell__hd">
                        <label class="weui-label">行驶里程</label>
                    </div>
                    <div class="weui-cell__bd">
                        <input id="Driving" class="weui-input" type="number" placeholder="请输入行驶里程">
                    </div>
                    <div class="weui-cell__ft">
                        <label class="weui-label">公里</label>
                    </div>
                </div>

                <div class="weui-cell weui-cell_switch">
                    <div class="weui-cell__bd">车辆是否存在故障</div>
                    <div class="weui-cell__ft">
                        <input id="Trouble" class="weui-switch" type="checkbox">
                    </div>
                </div>

                <div class="weui-cell">
                    <div class="weui-cell__bd">
                        <textarea id="Detail" class="weui-textarea" placeholder="若存在故障，则详细描述故障现象。" rows="3"></textarea>
                    </div>
                </div>
            </div>

            <div class="weui-cells__title">车主信息</div>
            <div class="weui-cells weui-cells_form">
                <div class="weui-cell">
                    <div class="weui-cell__hd">
                        <label class="weui-label">姓名</label>
                    </div>
                    <div class="weui-cell__bd">
                        <input id="Owner" class="weui-input" type="text" placeholder="请输入车主姓名">
                    </div>
                </div>

                <div class="weui-cell">
                    <div class="weui-cell__hd">
                        <label class="weui-label">联系电话</label>
                    </div>
                    <div class="weui-cell__bd">
                        <input id="Phone" class="weui-input" type="tel" placeholder="请输入手机号">
                    </div>
                </div>
            </div>
        
            <div class="weui-btn-area">
                <a class="weui-btn weui-btn_primary" href="javascript:" id="saveButton">确定</a>
            </div>
        </div>

        <div id="done_result" class="weui-msg hidden">
            <div class="weui-msg__icon-area">
                <i class="weui-icon_msg"></i>
            </div>
            <div class="weui-msg__text-area">
                <h2 class="weui-msg__title"></h2>
                <p class="weui-msg__desc"></p>
            </div>
            <%--<div class="weui-msg__opr-area">
                <p class="weui-btn-area">
                    <a href="javascript:;" class="weui-btn weui-btn_primary">推荐操作</a>
                    <a href="javascript:;" class="weui-btn weui-btn_default">辅助操作</a>
                </p>
            </div>--%>
            <div class="weui-msg__extra-area">
                <div class="weui-footer">
                    <!--p class="weui-footer__links">
                        <a href="javascript:void(0);" class="weui-footer__link">底部链接文本</a>
                    </!p-->
                    <p class="weui-footer__text">五号动力</p>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
