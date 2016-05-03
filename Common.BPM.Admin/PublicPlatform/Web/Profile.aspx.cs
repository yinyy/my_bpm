using BPM.Core.Bll;
using BPM.Core.Model;
using Senparc.Weixin;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Washer.Bll;
using Washer.Model;

namespace BPM.Admin.PublicPlatform.Web
{
    public partial class Profile : System.Web.UI.Page
    {
        public WeixinUserInfoResult userInfo = new WeixinUserInfoResult();
        public WasherWeChatConsumeModel wxconsume;
        public WasherConsumeModel consume;

        protected void Page_Load(object sender, EventArgs e)
        {
            //string code = Request.Params["code"];
            //int deptId = Convert.ToInt32(Request.Params["state"]);

            //Department dept = DepartmentBll.Instance.Get(deptId);
            //OAuthAccessTokenResult result = OAuthApi.GetAccessToken(dept.Appid, dept.Secret, code);
            //if (result.errcode != ReturnCode.请求成功)
            //{
            //    Response.Write(result.errmsg);
            //}
            //else
            //{
            //    userInfo = CommonApi.GetUserInfo(AccessTokenContainer.GetAccessToken(dept.Appid), result.openid);
            //    wxconsume = WasherWeChatConsumeBll.Instance.Get(deptId, userInfo.openid);
            //    if (wxconsume != null)
            //    {
            //        consume = WasherConsumeBll.Instance.Get(deptId, wxconsume.KeyId);
            //    }
            //}

            wxconsume = WasherWeChatConsumeBll.Instance.Get(69, "oIC6ejjI1izi372jxZ0R7LnD8p5o");
            if (wxconsume != null)
            {
                consume = WasherConsumeBll.Instance.Get(69, wxconsume.KeyId);
            }
        }
    }
}