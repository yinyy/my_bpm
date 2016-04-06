using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace BPM.Admin.ashx
{
    /// <summary>
    /// UploadHandler 的摘要说明
    /// </summary>
    public class UploadHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string fileName = context.Request["qqfilename"];
            string ext = fileName.Substring(fileName.LastIndexOf('.')+1);

            string folder = string.Format("/upload/{0}/{1}/{2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            string url = string.Format("{0}/{1}.{2}", folder, Convert.ToString(DateTime.Now.Ticks, 16), ext);
            fileName = context.Server.MapPath(url);
            folder = context.Server.MapPath(folder);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            
            try {
                using (var inputStream = context.Request.Files.Count > 0 ? context.Request.Files[0].InputStream : context.Request.InputStream)
                {
                    using (var flieStream = new FileStream(fileName, FileMode.Create))
                    {
                        inputStream.CopyTo(flieStream);
                    }
                }

                context.Response.Write(JsonConvert.SerializeObject(new { success = true, url = url }));//{"success": false, "error": "error message to display", "reset": true}  "preventRetry": true
            }
            catch(Exception exp)
            {
                context.Response.Write(JsonConvert.SerializeObject(new { success=false, error=exp.Message}));
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