using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BPM.Core;
using BPM.Core.Model;
using BPM.Common;
using BPM.Core.BasePage;

namespace BPM.Admin
{
    public partial class Default : BpmBasePage
    {
        protected string NavContent = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            string configData = SysVisitor.Instance.CurrentUser.ConfigJson;

            string themePath = Server.MapPath("theme/navtype/");
            NVelocityHelper vel = new NVelocityHelper(themePath);
            vel.Put("username", UserName);
            string navHTML = "Accordion.html";
            if (!string.IsNullOrEmpty(configData))
            {
                ConfigModel sysconfig = JSONhelper.ConvertToObject<ConfigModel>(configData);
                if (sysconfig != null)
                {

                    switch (sysconfig.ShowType)
                    {
                        case "menubutton":
                            navHTML = "menubutton.html";
                            break;
                        case "tree":
                            navHTML = "tree.html";
                            break;
                        case "menuAccordion":
                        case "menuAccordion2":
                        case "menuAccordionTree":
                            navHTML = "topandleft.html";
                            break;
                        default:
                            navHTML = "Accordion.html";
                            break;
                    }

                }
            }
            
            NavContent = vel.FileToString(navHTML);
        }
    }
}