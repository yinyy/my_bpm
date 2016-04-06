using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WeChat.Utils;
using Newtonsoft.Json;

namespace BPM.Admin.PublicPlatform.Web
{
    public partial class Promote : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                KeyLabel.Text = Request.Params["k"];

                string message = WeChatToolkit.SendCommand("https://api.weixin.qq.com/cgi-bin/qrcode/create?access_token=" + WeChatToolkit.AccessToken,
                    "{\"expire_seconds\": 300, \"action_name\": \"QR_SCENE\", \"action_info\": {\"scene\": {\"scene_id\": " + Request.Params["k"] + "}}}");
                if (!string.IsNullOrWhiteSpace(message))
                {
                    try
                    {
                        var msg = new { ticket = "", expire_seconds = 0, url = "" };
                        msg = JsonConvert.DeserializeAnonymousType(message, msg);

                        CodeImage.ImageUrl = "https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket=" + msg.ticket;
                    }
                    catch (Exception ex)
                    {
                        Response.Write("获取二维码错误。" + ex.Message);
                    }
                }
            }
        }
    }
}