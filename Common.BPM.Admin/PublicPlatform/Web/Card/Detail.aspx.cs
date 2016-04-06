using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Washer.Bll;

namespace BPM.Admin.PublicPlatform.Web.Card
{
    public partial class Detail : System.Web.UI.Page
    {
        protected string _Introduction;

        protected void Page_Load(object sender, EventArgs e)
        {
            int keyId = Convert.ToInt32(Request.Params["cid"]);
            _Introduction = WasherCardBll.Instance.GetIntroduction(keyId);
        }
    }
}