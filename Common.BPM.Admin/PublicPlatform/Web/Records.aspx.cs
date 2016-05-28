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
    public partial class Records : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string keyId = Request.Params["id"];
            if (!string.IsNullOrWhiteSpace(keyId))
            {
                WasherWeChatConsumeModel wxconsume = WasherWeChatConsumeBll.Instance.Get(Convert.ToInt32(keyId));
                if (wxconsume != null)
                {
                    WasherConsumeModel consume = WasherConsumeBll.Instance.Get(wxconsume);
                    if (consume != null)
                    {

                    }
                }
            }
        }
    }
}