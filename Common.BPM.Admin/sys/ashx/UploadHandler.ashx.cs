using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BPM.Admin.sys.ashx
{
    /// <summary>
    /// UploadHandler 的摘要说明
    /// </summary>
    public class UploadHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            HttpPostedFile upfile = context.Request.Files["fileToUpload"];
            if (upfile == null)
            {
                context.Response.Write("1");
            }
            else if (upfile.ContentLength > 1024 * 1024 * 5)//不能大于5M
            {
                context.Response.Write("2");
            }
            else
            {
                DateTime now = DateTime.Now;
                string path = string.Format("~/upload/{0}/{1}/{2}", now.Year, now.Month, now.Day);
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(path));

                string filename = string.Format("{0}/{1}_{2}", path, now.Ticks.ToString("x8"), upfile.FileName.Substring(upfile.FileName.LastIndexOf('\\') + 1));

                upfile.SaveAs(HttpContext.Current.Server.MapPath(filename));

                context.Response.Write("3," + filename);
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}