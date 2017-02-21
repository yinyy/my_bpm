<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Test.aspx.cs" Inherits="BPM.Admin.PublicPlatform.Test" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <script src="Web/js/jquery-2_2_1_min.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <label>ACCESS_TOKEN：</label><label><%=WeChat.Utils.WeChatToolkit.AccessToken%></label><br />
    <asp:Button ID="上传素材" runat="server" Text="上传素材" OnClick="上传素材_Click" /><br />
        <asp:Button ID="创建菜单" runat="server" Text="创建菜单" OnClick="创建菜单_Click" /><br />
        <asp:TextBox ID="返回信息" runat="server" TextMode="MultiLine" Columns="150" Rows="30"></asp:TextBox>
    </div>

        <textarea id="setting" rows="10" cols="100"></textarea><br />
        {"WxPayOption":[600,800,1000],"Sms":{"Cid":"PnH65DWeULNh","Uid":"nEh8rcyJWEl4","Pas":"99795jwx","Url":"http://api.weimi.cc/2/sms/send.html"},"BuyCardOption":[{"Product":"50元洗车卡","Value":50,"Price":50,"Day":730,"Score":50},{"Product":"100元洗车卡","Value":100,"Price":100,"Day":730,"Score":100},{"Product":"200元洗车卡","Value":200,"Price":200,"Day":730,"Score":200},{"Product":"300元洗车卡","Value":300,"Price":300,"Day":730,"Score":300}],"GiftLevel":[50,10,1,0,0],"Register":{"Coupon":5,"CouponDay":7,"Point":50},"PayWashCar":{"Wx":20,"Vip":50,"Coupon":0},"Relay":{"Friend":1,"Moment":2}}
        <br /><textarea id="setting2" rows="10" cols="100"></textarea><br />
        <input type="button" id="btn" value="转换"/>

        <script type="text/javascript">
            $('#btn').click(function () {
                var os = eval('(' + $('#setting').val() + ')');

                var ns = {
                    WxPayOption: os.PayWash==null?[]:os.PayWash,
                    Sms: os.Sms,
                    BuyCardOption: os.Buy,
                    GiftLevel: os.Point.Referers.Level,
                    Register: { Coupon: os.Coupon.Coins, CouponDay: os.Coupon.Time, Point: 0 },
                    PayWashCar: { Wx: 0, Vip: 0, Coupon: 0 },
                    Relay: { Friend: 0, Moment: 0 }
                };

                $('#setting2').val(JSON.stringify(ns));

                return false;
            });
        </script>
    </form>
</body>
</html>
