using BPM.Core.Bll;
using BPM.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Washer.Bll;
using Washer.Extension;
using Washer.Model;

namespace BPM.Admin.PublicPlatform.Web
{
    public partial class Outlets : System.Web.UI.Page
    {
        public List<WasherDeviceModel> devices = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string appid = Request.Params["appid"];
                if (!string.IsNullOrEmpty(appid))
                {
                    Department dept = DepartmentBll.Instance.GetByAppid(appid);
                    if (dept != null)
                    {
                         devices = WasherDeviceBll.Instance.GetByDepartment(dept.KeyId);
                        devices = devices.Where(d => !string.IsNullOrWhiteSpace(d.Address) && !string.IsNullOrWhiteSpace(d.IpAddress) && d.UpdateTime != null).ToList();
                    }
                }
            }
        }
    }
}