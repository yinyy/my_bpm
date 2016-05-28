<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Records.aspx.cs" Inherits="BPM.Admin.PublicPlatform.Web.Records" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width,initial-scale=1,user-scalable=no">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <link href="./css/WeUI/style/weui.min.css" rel="stylesheet" />
    <link href="./css/Records.css?d=<%=DateTime.Now %>" rel="stylesheet" />
    <script src="./js/jquery-2_2_1_min.js"></script>
    <script src="./js/Records.js?d=<%=DateTime.Now %>"></script>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div class="weui_cells_title">消费记录</div>
        <div class="weui_cells weui_cells_access">
            <a class="weui_cell" href="javascript:;">
                <div class="weui_cell_bd weui_cell_primary">
                    <p>2014年11月12日 08:09:33</p>
                </div>
                <div>
                    ￥9.00
                </div>
            </a>
            <a class="weui_cell" href="javascript:;">
                <div class="weui_cell_bd weui_cell_primary">
                    <p>2014年11月12日 08:09:33</p>
                </div>
                <div>
                    ￥8.78
                </div>
            </a>
        </div>
    </form>
</body>
</html>
