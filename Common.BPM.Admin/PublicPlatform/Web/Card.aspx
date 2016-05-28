<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Card.aspx.cs" Inherits="BPM.Admin.PublicPlatform.Web.Card" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta name="viewport" content="width=device-width,initial-scale=1,user-scalable=no">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>我的洗车卡</title>
    <link href="css/WeUI/style/weui.css" rel="stylesheet" />
    <link href="css/Card.css" rel="stylesheet" />
    <script src="js/jquery-2_2_1_min.js"></script>
    <script src="js/Card.js?d=<%=DateTime.Now.Ticks %>"></script>
</head>
<body>
    <form id="uiform" runat="server">
        <input type="hidden" id="wxid" value='<%=Request.Params["wxid"] %>' />

        <div id="card_list">
        <%
            if (cards == null || cards.Count == 0)
            {
        %>
            <i class="weui_icon_msg weui_icon_warn"></i>
            <div>没有绑定洗车卡或洗车卡已经过期。</div>
         <%
            }
            else
            {
        %>
            <div class="weui_panel">
                <div class="weui_panel_hd">已绑定的洗车卡</div>
                <div class="weui_panel_bd">                
                    <%
                        foreach (var card in cards)
                        {
                    %>
                    <a href="javascript:void(0);" class="weui_media_box weui_media_appmsg">
                        <div class="weui_media_hd">
                            <img class="weui_media_appmsg_thumb" src="./images/icon_card.png" alt="">
                        </div>
                        <div class="weui_media_bd">
                            <h4 class="weui_media_title"><%=card.CardNo %></h4>
                            <p class="weui_media_desc"><%=string.Format("洗车币：{0:#####.00}<br/>有效期：{1:yyyy年MM月dd日} - {2:yyyy年MM月dd日}", card.Coins / 100.0, card.ValidateFrom, card.ValidateEnd) %></p>
                        </div>
                    </a>
                    <%
                        }
                    %>
                </div>
            </div>
        <%
            }
        %>          
            <div class="weui_btn_area">
                <a class="weui_btn weui_btn_primary" href="javascript:" id="prebind">绑定洗车卡</a>
            </div>
        </div>

        <div id="bind_card" style="display:none;">
            <div class="weui_cells_title">绑定洗车卡</div>
            <div class="weui_cells weui_cells_form">
                <div class="weui_cell">
                    <div class="weui_cell_hd"><label class="weui_label">卡号</label></div>
                    <div class="weui_cell_bd weui_cell_primary">
                        <input id="CardNo" class="weui_input" type="text" placeholder="请输入卡号"/>
                    </div>
                </div>
            
                <div class="weui_cell">
                    <div class="weui_cell_hd"><label class="weui_label">密码</label></div>
                    <div class="weui_cell_bd weui_cell_primary">
                        <input id="Password" class="weui_input" type="password" placeholder="请输入密码"/>
                    </div>
                </div>
            </div>
            <div class="weui_btn_area">
                <a class="weui_btn weui_btn_primary" href="javascript:" id="bind">确定</a>
            </div>
        </div>
    </form>
</body>
</html>
