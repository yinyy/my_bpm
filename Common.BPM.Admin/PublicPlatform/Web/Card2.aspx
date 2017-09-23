<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Card2.aspx.cs" Inherits="BPM.Admin.PublicPlatform.Web.Card2" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta name="viewport" content="width=device-width,initial-scale=1,user-scalable=no">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>我的洗车卡</title>
    <link href="css/WeUI/style/weui.css" rel="stylesheet" />
    <link href="css/style.css" rel="stylesheet" />
    <link href="css/Card2.css?d=<%=DateTime.Now.Ticks %>" rel="stylesheet" />
    <script src="js/jquery-2_2_1_min.js"></script>
    <script src="js/Pay.js?d=<%=DateTime.Now.Ticks %>"></script>
    <script src="js/Card2.js?d=<%=DateTime.Now.Ticks %>"></script>
    <script src="js/Vcode.js?t=<%=DateTime.Now.Ticks %>"></script>
    <script type="text/javascript">
        var cid = <%=Session["consumeId"]==null?null:Session["consumeId"].ToString()%>;
        <%
            int deptId=Convert.ToInt32(Session["deptId"].ToString());
            string openid = Session["openid"].ToString();

            BPM.Core.Model.Department dept = BPM.Core.Bll.DepartmentBll.Instance.Get(deptId);
            Washer.Model.WasherWeChatConsumeModel wxconsume = Washer.Bll.WasherWeChatConsumeBll.Instance.Get(dept.KeyId, openid);
            Washer.Model.WasherConsumeModel consume = Washer.Bll.WasherConsumeBll.Instance.GetByBinder(wxconsume);

            Washer.Model.WasherDepartmentSetting setting = Newtonsoft.Json.JsonConvert.DeserializeObject<Washer.Model.WasherDepartmentSetting>(dept.Setting);
        %>

    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div id="loading_region" class="weui_loading_toast" style="display: none;">
            <div class="weui_mask_transparent"></div>
            <div class="weui_toast">
                <div class="weui_loading">
                    <div class="weui_loading_leaf weui_loading_leaf_0"></div>
                    <div class="weui_loading_leaf weui_loading_leaf_1"></div>
                    <div class="weui_loading_leaf weui_loading_leaf_2"></div>
                    <div class="weui_loading_leaf weui_loading_leaf_3"></div>
                    <div class="weui_loading_leaf weui_loading_leaf_4"></div>
                    <div class="weui_loading_leaf weui_loading_leaf_5"></div>
                    <div class="weui_loading_leaf weui_loading_leaf_6"></div>
                    <div class="weui_loading_leaf weui_loading_leaf_7"></div>
                    <div class="weui_loading_leaf weui_loading_leaf_8"></div>
                    <div class="weui_loading_leaf weui_loading_leaf_9"></div>
                    <div class="weui_loading_leaf weui_loading_leaf_10"></div>
                    <div class="weui_loading_leaf weui_loading_leaf_11"></div>
                </div>
                <p class="weui_toast_content">数据加载中...</p>
            </div>
        </div>
        
        <div id="card_list_region" style="display:none">
            <div id="no_cards_region" style="display: none;">
                <i class="weui_icon_msg weui_icon_warn"></i>
                <div>没有绑定洗车卡或洗车卡已经过期。</div>
            </div>

            <div id="show_cards_region" class="weui_panel" style="display: none;">
                <div class="weui_panel_hd">已绑定的洗车卡</div>
                <div class="weui_panel_bd"> 
                </div>
            </div>
                 
            <div id="button_region" class="weui_btn_area">
                <a class="weui_btn weui_btn_primary" href="javascript:">购买洗车卡</a>
                <a class="weui_btn weui_btn_primary" href="javascript:">绑定洗车卡</a>
            </div>
        </div>
        
        <div id="bind_card_region" style="display: none;">
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

                <div class="weui_cell" style="display:none;">
                    <div class="weui_cell_hd"><label class="weui_label">验证码</label></div>
                    <div class="weui_cell_bd weui_cell_primary">
                        <input id="Vcode" class="weui_input" type="number" placeholder="请输入验证码"/>
                    </div>
                    <div class="weui_cell_ft">
                        <input type="hidden" id="Telphone" />
                        <a id="GetVcode" class="weui_btn weui_btn_mini weui_btn_default">获取验证码</a>
                    </div>
                </div>
            </div>
            <div class="weui_btn_area">
                <a class="weui_btn weui_btn_primary" href="javascript:">确定</a>
                <a class="weui_btn weui_btn_warn" href="javascript:">取消</a>
            </div>
        </div>

        <div id="shopping_region" style="display: none;">
            <div>
                <img src="./images/icon_wash_car.png" width="100%" />
            </div>
            <div class="card_kind_area">
                
            </div>
            <div class="cell">
                <p>说明：</p>
                <p>1、洗车卡成功购买后，自动与用户账户绑定。</p>
                <p style="color:red;font-weight: bolder;">2、支付成功后，请在本页面等待确认支付结果。若30秒内无响应，请联系客服。</p>
            </div>
            <div class="weui_btn_area">
                <a class="weui_btn weui_btn_primary" href="javascript:">购买</a>
                <a class="weui_btn weui_btn_warn" href="javascript:">取消</a>
            </div>
        </div>
    </form>
</body>
</html>
