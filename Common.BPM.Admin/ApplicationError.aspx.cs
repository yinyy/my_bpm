using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BPM.Admin
{
    public partial class ApplicationError : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //显示程序中的错误码
            if (!IsPostBack)
            {
                //显示程序中的错误码
                if (Application["error"] != null)
                {
                    Response.Write(Application["error"].ToString());
                }
            }
        }
    }
}