using BPM.Core.Bll;
using BPM.Core.Model;
using Newtonsoft.Json;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.Containers;
using System;
using Washer.Bll;
using Washer.Extension;
using Washer.Model;
using WeChat.Utils;

namespace BPM.Admin.PublicPlatform.Web
{
    public partial class Promote : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int deptId = Convert.ToInt16(Session["deptId"].ToString());
                string openid = Session["openid"].ToString();

                Department dept = DepartmentBll.Instance.Get(deptId);
                WasherWeChatConsumeModel wxconsume = WasherWeChatConsumeBll.Instance.Get(dept.KeyId, openid);

                //生成带微信用户编号的临时二维码，有效时间不超过一周
                string accessToken = AccessTokenContainer.TryGetAccessToken(dept.Appid, dept.Secret);
                var result = QrCodeApi.Create(accessToken, 604800, Convert.ToInt32(string.Format("7{0}", wxconsume.KeyId)), Senparc.Weixin.MP.QrCode_ActionName.QR_SCENE);
                if (result.errcode == Senparc.Weixin.ReturnCode.请求成功)
                {
                    CodeImage.ImageUrl = string.Format("https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket={0}", result.ticket);
                }
                else
                {
                    code_div.Visible = false;
                    nocode_div.Visible = true;
                }

                //string message = WeChatToolkit.SendCommand("https://api.weixin.qq.com/cgi-bin/qrcode/create?access_token=" + AccessTokenContainer.TryGetAccessToken(dept.Appid, dept.Secret) ,
                //    "{\"expire_seconds\": 604800, \"action_name\": \"QR_SCENE\", \"action_info\": {\"scene\": {\"scene_id\": " + string.Format("") + "}}}");
                //if (!string.IsNullOrWhiteSpace(message))
                //{
                //    try
                //    {
                //        var msg = new { ticket = "", expire_seconds = 0, url = "" };
                //        msg = JsonConvert.DeserializeAnonymousType(message, msg);

                //        CodeImage.ImageUrl = 
                //    }
                //    catch (Exception ex)
                //    {
                //        Response.Write("获取二维码错误。" + ex.Message);
                //    }
                //}
            }
        }
    }
}