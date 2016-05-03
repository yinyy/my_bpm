using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Washer.Bll;
using Washer.Model;
using WeChat.Utils;

namespace BPM.Admin.PublicPlatform.Web
{
    public partial class Test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string code = Request.Params["code"];
            string url = string.Format("https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code",
                WeChatToolkit.AppConfiguration.AppId,
                WeChatToolkit.AppConfiguration.AppSecret,
                code);
            string message = WeChatToolkit.SendCommand(url);
            if (!string.IsNullOrWhiteSpace(message))
            {
                var msg = new { access_token = "", expires_in = 0, refresh_token = "", openid = "", scope = "", errcode = 0, errmsg = "" };

                var o = JsonConvert.DeserializeAnonymousType(message, msg);
                if (o.errcode != 0)
                {
                    Response.Write(o.errmsg);
                }
                else
                {
                    GetCards(o.openid);
                }
            }
            //GetCards("oIC6ejjI1izi372jxZ0R7LnD8p5o");
        }

        private void GetCards(string openId)
        {
            //List<WasherCardModel> cards = WasherCardBll.Instance.GetCards(openId);
            //cardRepeater.DataSource = cards;
            //cardRepeater.DataBind();
        }
    }
}