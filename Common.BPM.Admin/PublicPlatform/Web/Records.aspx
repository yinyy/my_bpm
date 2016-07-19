<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Records.aspx.cs" Inherits="BPM.Admin.PublicPlatform.Web.Records" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width,initial-scale=1,user-scalable=no">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <link href="./css/WeUI/style/weui.min.css" rel="stylesheet" />
    <link href="./css/Records.css?v=0.0.2" rel="stylesheet"/>
    <script src="./js/jquery-2_2_1_min.js"></script>
    <script src="./js/Records.js?t=<%=DateTime.Now %>"></script>
    <title>洗车记录</title>
</head>
<body>
    <form id="form1" runat="server">
        <div id="record_list" style="display:none;">
            <div class="weui_cells_title">洗车记录</div>
            <div class="weui_cells weui_cells_access">
                
            </div>
            <div class="weui_btn_area">
                <ul>
                    <li><a class="weui_btn weui_btn_primary weui_btn_disabled" href="javascript:" id="previous">上一页</a></li>
                    <li></li>
                    <li><a class="weui_btn weui_btn_primary weui_btn_disabled" href="javascript:" id="next">下一页</a></li>
                </ul>
            </div>

        </div>

        <div id="no_record" style="display:none;">
            没有找到洗车记录。
        </div>
    </form>
</body>
</html>
