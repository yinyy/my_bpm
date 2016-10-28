<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Outlets.aspx.cs" Inherits="BPM.Admin.PublicPlatform.Web.Outlets" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta name="viewport" content="width=device-width,initial-scale=1,user-scalable=no">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>洗车机网点</title>
    <link href="css/WeUI/style/weui.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <%if (devices == null){ %>
            <div class="weui_panel">
                <div class="weui_panel_hd">洗车机工作网点</div>
                <div class="weui_panel_bd">
                    <div class="weui_media_box weui_media_text">
                        <h4 class="weui_media_title">未安装</h4>
                        <p class="weui_media_desc">运营商还没有安装洗车机！请耐心等一等！</p>
                    </div>
                </div>
            </div>
        <%}else { %>
            <div class="weui_panel weui_panel_access">
                <div class="weui_panel_hd">洗车机工作网点</div>
                <div class="weui_panel_bd">
                    <%foreach (var device in devices)
                        {%>
                        <div class="weui_media_box weui_media_text" <%=(string.IsNullOrWhiteSpace(device.Coordinate)?"":"onclick=\"document.location.href='http://api.map.baidu.com/marker?location="+string.Format("{0},{1}",device.Coordinate.Split(',')[1], device.Coordinate.Split(',')[0])+"&title=洗车机位置&content="+device.Address+"&output=html'\"") %>>
                            <h4 class="weui_media_title"><%=device.Address %></h4>
                            <p class="weui_media_desc"><%=string.Format("{0} {1} {2}", device.Province, device.City, device.Region) %></p>
                            <ul class="weui_media_info">
                                <li class="weui_media_info_meta"><%=string.Format("设备数量：{0} 台", device.Count) %></li>
                                <li class="weui_media_info_meta weui_media_info_meta_extra"><%=string.Format("更新时间：{0:yyyy年MM月dd日 HH:mm:ss}", device.Update) %></li>
                            </ul>
                        </div>
                    <%} %>
                 </div>
                <a href="javascript:void(0);" class="weui_panel_ft">查看更多</a>
            </div>
        <%} %>
    </form>
</body>
</html>
