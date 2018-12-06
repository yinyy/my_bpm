using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.TenPayLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Washer.Model;

namespace BPM.Admin.PublicPlatform.Web
{
    public partial class Payment : System.Web.UI.Page
    {
        public string Params;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Params = string.Format("var ps = {{wxid: {0}, board: '{1}'}};", Request.Params["wxid"], Request.Params["b"]);
            }
        }
    }
}