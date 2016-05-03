﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="BPM.Admin.PublicPlatform.Web.Profile" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width,initial-scale=1,user-scalable=no">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <link href="./css/WeUI/style/weui.min.css" rel="stylesheet" />
    <link href="./css/Profile.css?d=<%=DateTime.Now %>" rel="stylesheet" />
    <script src="./js/jquery-2_2_1_min.js"></script>
    <script src="./js/Profile.js?d=<%=DateTime.Now %>"></script>
    <title>我的洗车卡</title>
</head>

<body>
    <form id="uiform" runat="server">
    <%
        if (wxconsume == null) //显示绑定的页面        
        {
    %>
        <div>请先关注公众号！</div>
    <%
        }
        else        
        {
    %>

        <input type="hidden" id="BinderId" value="<%=wxconsume.KeyId %>" />
        <input type="hidden" id="DepartmentId" value="<%=wxconsume.DepartmentId %>" />
        <input type="hidden" id="ConsumeId" value="<%=consume==null?0:consume.KeyId%>" />

        <div id="content1" style="display:none;">
            <div class="weui_cells_title">用户注册</div>
            <div class="weui_cells weui_cells_form">
                <div class="weui_cell">
                    <div class="weui_cell_hd"><label class="weui_label">姓名</label></div>
                    <div class="weui_cell_bd weui_cell_primary">
                        <input id="Name" class="weui_input" type="text" placeholder="请输入姓名"/>
                    </div>
                </div>
            
                <div class="weui_cell weui_cell_switch">
                    <div class="weui_cell_hd weui_cell_primary">性别</div>
                    <div class="weui_cell_ft">
                        <input id="Gender" class="weui_switch" type="checkbox"/>
                    </div>
                </div>

                <div class="weui_cell">
                    <div class="weui_cell_hd"><label class="weui_label">电话</label></div>
                    <div class="weui_cell_bd weui_cell_primary">
                        <input id="Telphone" class="weui_input" type="number" pattern="[0-9]*" placeholder="请输入电话号码"/>
                    </div>
                    <div class="weui_cell_ft">
                        <a id="GetVcode" class="weui_btn weui_btn_mini weui_btn_default weui_btn_disabled">获取验证码</a>
                    </div>
                </div>

                <div class="weui_cell">
                    <div class="weui_cell_hd"><label class="weui_label">验证码</label></div>
                    <div class="weui_cell_bd weui_cell_primary">
                        <input id="Vcode" class="weui_input" type="number" placeholder="请输入验证码"/>
                    </div>
                </div>

                <div class="weui_cell">
                    <div class="weui_cell_hd"><label class="weui_label">密码</label></div>
                    <div class="weui_cell_bd weui_cell_primary">
                        <input id="Password" class="weui_input" type="password" placeholder="请输入密码"/>
                    </div>
                </div>

                <div class="weui_cell">
                    <div class="weui_cell_hd"><label class="weui_label">确认密码</label></div>
                    <div class="weui_cell_bd weui_cell_primary">
                        <input id="Repassword" class="weui_input" type="password" placeholder="请输入确认密码"/>
                    </div>
                </div>
            </div>
        
            <div class="weui_btn_area">
                <a class="weui_btn weui_btn_primary" href="javascript:" id="bind">确定</a>
            </div>
        </div>

        <div id="content2" style="display:none;">
            <div class="weui_panel weui_panel_access">
                <div class="weui_panel_bd">
                    <a href="javascript:void(0);" class="weui_media_box weui_media_appmsg">
                        <div class="weui_media_hd">
                            <img class="weui_media_appmsg_thumb" src="<%=userInfo.headimgurl %>" alt="">
                        </div>
                        <div class="weui_media_bd">
                            <h4 class="weui_media_title"><%=userInfo.nickname %></h4>
                            <p class="weui_media_desc">注册用户</p>
                        </div>
                    </a>
                </div>
            </div>
            <div class="weui_cells weui_cells_access">
                <a class="weui_cell" href="./Card.aspx?id=<%=wxconsume.KeyId %>">
                    <div class="weui_cell_hd"><img src="./images/icon_card.png" alt="" style="width:20px;margin-left:5px;margin-right:15px;display:block"></div>
                    <div class="weui_cell_bd weui_cell_primary">
                        <p>我的洗车卡</p>
                    </div>
                    <div class="weui_cell_ft"></div>
                </a>
                <a class="weui_cell" href="./Records.aspx?id=<%=wxconsume.KeyId %>&type=1">
                    <div class="weui_cell_hd"><img src="./images/icon_card.png" alt="" style="width:20px;margin-left:5px;margin-right:15px;display:block"></div>
                    <div class="weui_cell_bd weui_cell_primary">
                        <p>充值记录</p>
                    </div>
                    <div class="weui_cell_ft"></div>
                </a>
                <a class="weui_cell" href="./Records.aspx?id=<%=wxconsume.KeyId %>&type=2">
                    <div class="weui_cell_hd"><img src="./images/icon_card.png" alt="" style="width:20px;margin-left:5px;margin-right:15px;display:block"></div>
                    <div class="weui_cell_bd weui_cell_primary">
                        <p>消费记录</p>
                    </div>
                    <div class="weui_cell_ft"></div>
                </a>
                <a class="weui_cell" href="./Records.aspx?id=<%=wxconsume.KeyId %>&type=3">
                    <div class="weui_cell_hd"><img src="./images/icon_card.png" alt="" style="width:20px;margin-left:5px;margin-right:15px;display:block"></div>
                    <div class="weui_cell_bd weui_cell_primary">
                        <p>积分记录</p>
                    </div>
                    <div class="weui_cell_ft"></div>
                </a>
            </div>

            <div class="weui_btn_area">
                <a id="unbind" class="weui_btn weui_btn_warn">解除绑定</a>
            </div>
        </div>
    <%
        }
    %>
    </form>

    <script type="text/javascript">
        if(<%=consume==null?"true":"false"%>){
            $('div#content1').show();
            $('div#content2').hide();
        }else{
            $('div#content1').hide();
            $('div#content2').show();
        }
    </script>
</body>
</html>
