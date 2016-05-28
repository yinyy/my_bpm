using BPM.Common;
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
    public partial class Card : System.Web.UI.Page
    {
        public List<WasherCardModel> cards;

        protected void Page_Load(object sender, EventArgs e)
        {
                string wxid = Request.Params["wxid"];
                string action = Request.Params["action"];

                WasherWeChatConsumeModel wxconsume = WasherWeChatConsumeBll.Instance.Get(Convert.ToInt32(wxid));
                WasherConsumeModel consume = WasherConsumeBll.Instance.Get(wxconsume);

            if (string.IsNullOrWhiteSpace(action))
            {
                cards = WasherCardBll.Instance.GetValidCards(consume.KeyId).OrderBy(a => a.ValidateEnd).ToList();
            }
            else if ("bind" == action)
            {
                string cardNo = Request.Params["CardNo"];
                string password = Request.Params["Password"];
                WasherCardModel card = WasherCardBll.Instance.Get(consume.DepartmentId, cardNo);
                if (card == null)
                {
                    Response.Write(JSONhelper.ToJson(new { Success = -1 }));
                }
                else if (card.Password != password)
                {
                    Response.Write(JSONhelper.ToJson(new { Success = -2 }));
                }
                else if (card.BinderId != null)
                {
                    Response.Write(JSONhelper.ToJson(new { Success = -3 }));
                }
                else if (DateTime.Now < card.ValidateFrom || DateTime.Now > card.ValidateEnd)
                {
                    Response.Write(JSONhelper.ToJson(new { Success = -4 }));
                }
                else
                {
                    card.BinderId = consume.KeyId;
                    card.Binded = DateTime.Now;

                    if (WasherCardBll.Instance.Update(card) > 0)
                    {
                        Response.Write(JSONhelper.ToJson(new { Success = 0 }));
                    }
                    else
                    {
                        Response.Write(JSONhelper.ToJson(new { Success = -5 }));
                    }
                }

                Response.End();
            }
        }
    }
}