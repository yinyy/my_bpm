using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WeChat.Utils;

namespace BPM.Admin.PublicPlatform
{
    public partial class Test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void 上传素材_Click(object sender, EventArgs e)
        {

        }

        protected void 创建菜单_Click(object sender, EventArgs e)
        {
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/menu/create?access_token={0}", WeChatToolkit.AccessToken);
            string menu;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/PublicPlatform/data/menu.txt")))
            {
                menu = reader.ReadToEnd();
            }

            string message = WeChatToolkit.SendCommand(url, menu);
            返回信息.Text = message;
        }
    }
}