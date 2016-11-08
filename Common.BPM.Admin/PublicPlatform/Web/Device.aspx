<%@ Page Language="C#" AutoEventWireup="true" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta name="viewport" content="width=device-width,initial-scale=1,user-scalable=no">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>设备列表</title>
    <link href="css/WeUI/style/weui.min.css" rel="stylesheet" />
    <link href="css/style.css" rel="stylesheet" />
    <script src="../../scripts/jquery-1.8.3.min.js"></script>
    <script src="js/Device.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div id="no_device" style="text-align:center;padding:5em 0em;display:none;" class="weui_icon_msg weui_icon_info">
            还没有关注设备。请先通过扫码关注。
        </div>
        <div id="has_device" style="display:none;" class="weui_panel weui_panel_access">
            <div class="weui_panel_hd">已关注的设备</div>
            <div class="weui_panel_bd">
            </div>
            <a class="weui_panel_ft" href="javascript:void(0);">查看更多</a>
        </div>

        <div class="weui_dialog_confirm" id="dialog1" style="display:none;">
            <div class="weui_mask"></div>
            <div class="weui_dialog">
                <div class="weui_dialog_hd"><strong class="weui_dialog_title">取消关注</strong></div>
                <div class="weui_dialog_bd">取消关注后，您将不能收到设备状态发生变化时发送给您的信息。</div>
                <div class="weui_dialog_ft">
                    <a href="javascript:;" class="weui_btn_dialog default">取消</a>
                    <a href="javascript:;" class="weui_btn_dialog primary">确定</a>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
