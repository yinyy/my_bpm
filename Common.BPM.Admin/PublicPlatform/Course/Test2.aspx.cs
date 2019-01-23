using Course.Common.Bll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BPM.Admin.PublicPlatform.Course
{
    public partial class Test2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var staff = CommonStaffBll.Instance.Get("9101", "student");
                Session["staff"] = staff;

                Response.Redirect("./Branch.aspx");
            }
        }
    }
}