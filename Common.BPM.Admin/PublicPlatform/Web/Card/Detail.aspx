<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="BPM.Admin.PublicPlatform.Web.Card.Detail" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width,initial-scale=1,user-scalable=no">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <link href="../css/WeUI/style/weui.min.css" rel="stylesheet" />
    <link href="../css/card.css?d=<%=DateTime.Now %>" rel="stylesheet" />
    <script src="../js/jquery-2_2_1_min.js"></script>
    <script src="../js/card.js"></script>
    <title>我的洗车卡</title>
</head>
<body>
    <form id="form1" runat="server">
        <div class="page">
            <div class="bd" style="height: 100%;">
                <div class="weui_tab">
                    <div class="weui_tab_bd">
                        <div id="consume_right">
                            <article class="weui_article">
                                <section>
                                    <%=_Introduction %>
                                </section>
                            </article>
                        </div>
                        <div id="machine_network">
                            <div class="weui_cells_title">洗车网点</div>
                            <div class="weui_cells">
                                <div class="weui_cell">
                                    <div class="weui_cell_hd"><img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAC4AAAAuCAMAAABgZ9sFAAAAVFBMVEXx8fHMzMzr6+vn5+fv7+/t7e3d3d2+vr7W1tbHx8eysrKdnZ3p6enk5OTR0dG7u7u3t7ejo6PY2Njh4eHf39/T09PExMSvr6+goKCqqqqnp6e4uLgcLY/OAAAAnklEQVRIx+3RSRLDIAxE0QYhAbGZPNu5/z0zrXHiqiz5W72FqhqtVuuXAl3iOV7iPV/iSsAqZa9BS7YOmMXnNNX4TWGxRMn3R6SxRNgy0bzXOW8EBO8SAClsPdB3psqlvG+Lw7ONXg/pTld52BjgSSkA3PV2OOemjIDcZQWgVvONw60q7sIpR38EnHPSMDQ4MjDjLPozhAkGrVbr/z0ANjAF4AcbXmYAAAAASUVORK5CYII=" alt="" style="width:20px;margin-right:5px;display:block"></div>
                                    <div class="weui_cell_bd weui_cell_primary">
                                        <p>标题文字1</p>
                                    </div>
                                </div>
                                <div class="weui_cell">
                                    <div class="weui_cell_hd"><img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAC4AAAAuCAMAAABgZ9sFAAAAVFBMVEXx8fHMzMzr6+vn5+fv7+/t7e3d3d2+vr7W1tbHx8eysrKdnZ3p6enk5OTR0dG7u7u3t7ejo6PY2Njh4eHf39/T09PExMSvr6+goKCqqqqnp6e4uLgcLY/OAAAAnklEQVRIx+3RSRLDIAxE0QYhAbGZPNu5/z0zrXHiqiz5W72FqhqtVuuXAl3iOV7iPV/iSsAqZa9BS7YOmMXnNNX4TWGxRMn3R6SxRNgy0bzXOW8EBO8SAClsPdB3psqlvG+Lw7ONXg/pTld52BjgSSkA3PV2OOemjIDcZQWgVvONw60q7sIpR38EnHPSMDQ4MjDjLPozhAkGrVbr/z0ANjAF4AcbXmYAAAAASUVORK5CYII=" alt="" style="width:20px;margin-right:5px;display:block"></div>
                                    <div class="weui_cell_bd weui_cell_primary">
                                        <p>标题文字1</p>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div id="consume_record">
                            <div class="weui_cells_title">消费记录</div>
                            <div class="weui_cells">
                                <div class="weui_cell">
                                    <div class="weui_cell_bd weui_cell_primary">
                                        <p>消费1</p>
                                    </div>
                                    <div class="weui_cell_ft">说明文字</div>
                                </div>
                                <div class="weui_cell">
                                    <div class="weui_cell_bd weui_cell_primary">
                                        <p>消费2</p>
                                    </div>
                                    <div class="weui_cell_ft">说明文字</div>
                                </div>
                                <div class="weui_cell">
                                    <div class="weui_cell_bd weui_cell_primary">
                                        <p>消费3</p>
                                    </div>
                                    <div class="weui_cell_ft">说明文字</div>
                                </div>
                            </div>
                            <div class="weui_cells_title">积分记录</div>
                            <div class="weui_cells">
                                <div class="weui_cell">
                                    <div class="weui_cell_bd weui_cell_primary">
                                        <p>积分1</p>
                                    </div>
                                    <div class="weui_cell_ft">说明文字</div>
                                </div>
                                <div class="weui_cell">
                                    <div class="weui_cell_bd weui_cell_primary">
                                        <p>积分2</p>
                                    </div>
                                    <div class="weui_cell_ft">说明文字</div>
                                </div>
                                <div class="weui_cell">
                                    <div class="weui_cell_bd weui_cell_primary">
                                        <p>积分3</p>
                                    </div>
                                    <div class="weui_cell_ft">说明文字</div>
                                </div>
                            </div>
                            <div class="weui_cells_title">充值记录</div>
                            <div class="weui_cells">
                                <div class="weui_cell">
                                    <div class="weui_cell_bd weui_cell_primary">
                                        <p>充值1</p>
                                    </div>
                                    <div class="weui_cell_ft">说明文字</div>
                                </div>
                                <div class="weui_cell">
                                    <div class="weui_cell_bd weui_cell_primary">
                                        <p>充值22</p>
                                    </div>
                                    <div class="weui_cell_ft">说明文字</div>
                                </div>
                                <div class="weui_cell">
                                    <div class="weui_cell_bd weui_cell_primary">
                                        <p>充值3</p>
                                    </div>
                                    <div class="weui_cell_ft">说明文字</div>
                                </div>
                            </div>
                            <div class="weui_cells_title">充值</div>
                            <div class="weui_cells">
                                <div class="weui_cell">
                                    <div class="weui_cell_bd weui_cell_primary">
                                        <ul class="amount_charge">
                                            <li>50元</li>
                                            <li>100元</li>
                                            <li>200元</li>
                                        </ul>
                                        <div style="clear: both; height: 10px;"></div>
                                        <a id="charge_button" href="#" class="weui_btn weui_btn_disabled weui_btn_primary">充值</a>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div id="promote">
                            <img src="../images/default_logo.png" />
                            <p>长按上面的二维码，发送给好友！</p>
                        </div>
                    </div>
                    <div class="weui_tabbar">
                        <a href="javascript:;" class="weui_tabbar_item weui_bar_item_on">
                            <div class="weui_tabbar_icon">
                                <img src="../images/icon_nav_button.png" alt="">
                            </div>
                            <p class="weui_tabbar_label">用卡须知</p>
                        </a>
                        <a href="javascript:;" class="weui_tabbar_item">
                            <div class="weui_tabbar_icon">
                                <img src="../images/icon_nav_msg.png" alt="">
                            </div>
                            <p class="weui_tabbar_label">洗车网点</p>
                        </a>
                        <a href="javascript:;" class="weui_tabbar_item">
                            <div class="weui_tabbar_icon">
                                <img src="../images/icon_nav_article.png" alt="">
                            </div>
                            <p class="weui_tabbar_label">消费积分</p>
                        </a>
                        <a href="javascript:;" class="weui_tabbar_item">
                            <div class="weui_tabbar_icon">
                                <img src="../images/icon_nav_cell.png" alt="">
                            </div>
                            <p class="weui_tabbar_label">推广</p>
                        </a>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
