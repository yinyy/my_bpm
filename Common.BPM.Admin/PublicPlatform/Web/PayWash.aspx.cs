using BPM.Core.Bll;
using BPM.Core.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Washer.Model;

namespace BPM.Admin.PublicPlatform.Web
{
    public partial class PayWash : System.Web.UI.Page
    {
        public int[] PayWashKinds = { };

        protected void Page_Load(object sender, EventArgs e)
        {
            int deptId = Convert.ToInt32(Session["deptId"].ToString());

            Department dept = DepartmentBll.Instance.Get(deptId);
            WasherDepartmentSetting setting = JsonConvert.DeserializeObject<WasherDepartmentSetting>(dept.Setting);
            if (setting.WxPayOption != null)
            {
                PayWashKinds = setting.WxPayOption;
            }
        }
    }
}