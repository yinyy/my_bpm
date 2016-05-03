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
        public WasherConsumeModel consume;
        public WasherCardModel card;

        protected void Page_Load(object sender, EventArgs e)
        {
            string wxconsumeId = Request.Params["id"];

            WasherWeChatConsumeModel wxconsume = WasherWeChatConsumeBll.Instance.Get(Convert.ToInt32(wxconsumeId));
            consume = WasherConsumeBll.Instance.Get(wxconsume);
            card = WasherCardBll.Instance.Get(consume);
        }
    }
}