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
    <script src="js/Card.js"></script>
</head>
<body>
    <form id="uiform" runat="server">
        <%
            if (card == null)
            {
        %>

        <input type="hidden" value="<%=consume.DepartmentId %>" id="DepartmentId" />
        <input type="hidden" value="<%=consume.KeyId %>" id="ConsumeId" />

        <div id="not_exist">
            <div>没有绑定洗车卡或洗车卡已经过期。</div>
            <div class="weui_btn_area">
                <a id="prebind" class="weui_btn weui_btn_primary">绑定洗车卡</a>
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
                    <div class="weui_cell_ft">
                        <a id="GetPassword" class="weui_btn weui_btn_mini weui_btn_default weui_btn_disabled">获取密码</a>
                    </div>
                </div>
            
                <div class="weui_cell">
                    <div class="weui_cell_hd"><label class="weui_label">密码</label></div>
                    <div class="weui_cell_bd weui_cell_primary">
                        <input id="Password" class="weui_input" type="password" placeholder="请输入密码"/>
                    </div>
                </div>
            </div>
            <div class="weui_cells_tips">
                <p>1、绑定虚拟卡时，输入卡号后点击“获取密码”按钮，系统将卡密码发送至您的手机号。</p>
                <p>2、绑定实体卡时，直接输入卡片背面的卡密码即可。</p>
            </div>
            <div class="weui_btn_area">
                <a class="weui_btn weui_btn_primary" href="javascript:" id="bind">绑定洗车卡</a>
            </div>
        </div>
        <%
            }
            else
            {
        %>
        <input type="hidden" id="CardId" value="<%=card.KeyId %>" />
        <div class="weui_panel weui_panel_access">
            <div class="weui_panel_hd">我的洗车卡</div>
            <div class="weui_panel_bd">
                <div class="weui_media_box weui_media_text">
                    <h4 class="weui_media_title">卡号</h4>
                    <p class="weui_media_desc"><%=card.CardNo %></p>
                </div>
                <div class="weui_media_box weui_media_text">
                    <h4 class="weui_media_title">卡类型</h4>
                    <p class="weui_media_desc"><%=card.Kind=="permanent"?"永久卡":card.Kind=="temporary"?"临时卡":"未知类型" %></p>
                </div>
                <%
                    if (card.Kind == "temporary")
                    {
                %>
                <div class="weui_media_box weui_media_text">
                    <h4 class="weui_media_title">有效期</h4>
                    <p class="weui_media_desc"><%=string.Format("{0:yyyy年MM月dd日} - {1:yyyy年MM月dd日}", card.ValidateFrom, card.ValidateEnd) %></p>
                </div>
                <%
                    }
                %>
                <div class="weui_media_box weui_media_text">
                    <h4 class="weui_media_title">余额（洗车币）</h4>
                    <p class="weui_media_desc"><%=card.Coins %></p>
                </div>
            </div>
        </div>
        <div class="weui_btn_area">
            <a class="weui_btn weui_btn_warn" href="javascript:" id="unbind">取消绑定</a>
        </div>
        <%
            }
        %>
    </form>
</body>
</html>
