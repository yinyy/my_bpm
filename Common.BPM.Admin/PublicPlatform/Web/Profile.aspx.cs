using BPM.Common;
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
        protected void Page_Load(object sender, EventArgs e)
        {
            //string action = Request.Params["action"];
            //if (string.IsNullOrWhiteSpace(action))
            //{
            //    string code = Request.Params["code"];
            //    int deptId = Convert.ToInt32(Request.Params["state"]);

            //    Department dept = DepartmentBll.Instance.Get(deptId);
            //    OAuthAccessTokenResult result = OAuthApi.GetAccessToken(dept.Appid, dept.Secret, code);
            //    if (result.errcode != ReturnCode.请求成功)
            //    {
            //        Response.Write(result.errmsg);
            //    }
            //    else
            //    {
            //        userInfo = CommonApi.GetUserInfo(AccessTokenContainer.GetAccessToken(dept.Appid), result.openid);
            //        wxconsume = WasherWeChatConsumeBll.Instance.Get(deptId, userInfo.openid);
            //        if (wxconsume != null)
            //        {
            //            consume = WasherConsumeBll.Instance.GetByBinder(wxconsume);
            //            Session["Consume"] = consume;
            //        }

            //        Session["UserInfo"] = userInfo;
            //        Session["WxConsume"] = wxconsume;

            //        Session["wxid"] = wxconsume.KeyId;
            //    }

            //    //wxconsume = WasherWeChatConsumeBll.Instance.Get(69, "oIC6ejjI1izi372jxZ0R7LnD8p5o");
            //    //if (wxconsume != null)
            //    //{
            //    //    consume = WasherConsumeBll.Instance.Get(69, wxconsume.KeyId);
            //    //}
            //}
            //else if (action == "vcode")
            //{
            //    string telphone = Request.Params["Telphone"];
            //    //获得有效的验证码
            //    WasherVcodeModel vcode = WasherVcodeBll.Instance.Get(telphone);
            //    if (vcode == null || vcode.Validated != null || vcode.Created.AddMinutes(3) <= DateTime.Now)
            //    #region 没有该手机号对应的验证码，或验证码已经过期
            //    {
            //        vcode = new WasherVcodeModel();
            //        vcode.Created = DateTime.Now;
            //        vcode.Memo = "";
            //        vcode.Validated = null;
            //        vcode.Telphone = telphone;
            //        vcode.Vcode = string.Format("{0:000000}", DateTime.Now.Ticks % 1000000);

            //        if (WasherVcodeBll.Instance.Save(vcode) > 0)
            //        {
            //            Response.Write(JSONhelper.ToJson(new { Success = 0 }));

            //            //TODO 发送验证码到手机号
            //            #region 发送验证码到手机号
            //            #endregion
            //        }
            //        else
            //        {
            //            Response.Write(JSONhelper.ToJson(new { Success = -1 }));
            //        }
            //    }
            //    #endregion
            //    else
            //    #region 发送已有的验证码
            //    {
            //        Response.Write(JSONhelper.ToJson(new { Success = 0 }));

            //        //TODO 发送验证码到手机号
            //        #region 发送验证码到手机号
            //        #endregion
            //    }
            //    #endregion

            //    Response.End();
            //}
            //else if (action == "bind")
            //{
            //    int binderId = Convert.ToInt32(Request.Params["BinderId"]);
            //    int departmentId = Convert.ToInt32(Request.Params["DepartmentId"]);
            //    string name = Request.Params["name"];
            //    string gender = Request.Params["Gender"];
            //    string telphone = Request.Params["Telphone"];
            //    string vcode = Request.Params["Vcode"];
            //    string password = Request.Params["Password"];

            //    WasherVcodeResult code = WasherVcodeBll.Instance.Validate(telphone, vcode, 3);
            //    if (code != WasherVcodeResult.验证码正确)
            //    {
            //        Response.Write(JSONhelper.ToJson(new { Success = -1 }));
            //    }
            //    else
            //    {
            //        WasherConsumeModel consume = WasherConsumeBll.Instance.Get(departmentId, telphone);
            //        if (consume == null)
            //        {
            //            consume = new WasherConsumeModel()
            //            {
            //                BinderId = binderId,
            //                Bindered = DateTime.Now,
            //                DepartmentId = departmentId,
            //                Gender = gender,
            //                Memo = "",
            //                Name = name,
            //                Password = password,
            //                Points = 0,
            //                Telphone = telphone
            //            };

            //            if ((consume.KeyId = WasherConsumeBll.Instance.Add(consume)) > 0)
            //            {
            //                Response.Write(JSONhelper.ToJson(new { Success = 0, keyId = consume.KeyId }));
            //            }
            //            else
            //            {
            //                Response.Write(JSONhelper.ToJson(new { Success = -2 }));
            //            }
            //        }
            //        else if (consume.BinderId != null)
            //        {
            //            Response.Write(JSONhelper.ToJson(new { Success = -3 }));
            //        }
            //        else
            //        {
            //            consume.Name = name;
            //            consume.Password = password;
            //            consume.Gender = gender;
            //            consume.Bindered = DateTime.Now;
            //            consume.BinderId = binderId;

            //            if (WasherConsumeBll.Instance.Update(consume) > 0)
            //            {
            //                Response.Write(JSONhelper.ToJson(new { Success = 0, keyId = consume.KeyId }));
            //            }
            //            else
            //            {
            //                Response.Write(JSONhelper.ToJson(new { Success = -2 }));
            //            }
            //        }
            //    }

            //    Response.End();
            //}
            //else if (action == "unbind")
            //{
            //    int consumeId = Convert.ToInt32(Request.Params["ConsumeId"]);
            //    consume = WasherConsumeBll.Instance.Get(consumeId);
            //    if (consume != null)
            //    {
            //        consume.BinderId = null;
            //        consume.Bindered = null;

            //        if (WasherConsumeBll.Instance.Update(consume) > 0)
            //        {
            //            Response.Write(JSONhelper.ToJson(new { Success = 1 }));
            //        }
            //        else
            //        {
            //            Response.Write(JSONhelper.ToJson(new { Success = 0 }));
            //        }
            //    }
            //    else
            //    {
            //        Response.Write(JSONhelper.ToJson(new { Success = -1 }));
            //    }

            //    Response.End();
            //}
        }
    }
}