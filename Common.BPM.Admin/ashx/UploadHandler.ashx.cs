using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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

            string folder = string.Format("/upload/{0}/{1}/{2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(context.Server.MapPath(folder));
            }

            string source = context.Request.Params["source"];
            if (source == "xheditor")
            {
                context.Response.Charset = "UTF-8";

                // 初始化一大堆变量
                string inputname = "filedata";//表单文件域name
                int maxattachsize = 10 * 1024 * 1024;     // 最大上传大小，默认是2M
                byte[] file;                     // 统一转换为byte数组处理
                string localname = "";
                string disposition = context.Request.ServerVariables["HTTP_CONTENT_DISPOSITION"];

                string err = "";
                string msg = "''";

                if (disposition != null)
                {
                    // HTML5上传
                    file = context.Request.BinaryRead(context.Request.TotalBytes);
                    localname = context.Server.UrlDecode(Regex.Match(disposition, "filename=\"(.+?)\"").Groups[1].Value);// 读取原始文件名
                }
                else
                {
                    HttpFileCollection filecollection = context.Request.Files;
                    HttpPostedFile postedfile = filecollection.Get(inputname);

                    // 读取原始文件名
                    localname = postedfile.FileName;
                    // 初始化byte长度.
                    file = new Byte[postedfile.ContentLength];

                    // 转换为byte类型
                    System.IO.Stream stream = postedfile.InputStream;
                    stream.Read(file, 0, postedfile.ContentLength);
                    stream.Close();

                    filecollection = null;
                }

                if (file.Length == 0)
                {
                    err = "无数据提交";
                }
                else
                {
                    if (file.Length > maxattachsize)
                    {
                        err = "文件大小超过" + maxattachsize + "字节";
                    }
                    else
                    {
                        string ext = localname.Substring(localname.LastIndexOf('.') + 1).ToLower();
                        string url = string.Format("{0}/{1}.{2}", folder, Convert.ToString(DateTime.Now.Ticks, 16), ext);

                        try
                        {
                            FileStream fs = new FileStream(context.Server.MapPath(url), FileMode.Create, FileAccess.Write);
                            fs.Write(file, 0, file.Length);
                            fs.Flush();
                            fs.Close();
                        }
                        catch (Exception ex)
                        {
                            err = ex.Message.ToString();
                        }

                        msg = "{'url':'" + url + "','localname':'" + jsonString(localname) + "','id':'1'}";
                    }
                }

                context.Response.Write("{'err':'" + jsonString(err) + "','msg':" + msg + "}");
            }
            else
            {
                string fileName = context.Request["qqfilename"];
                string ext = fileName.Substring(fileName.LastIndexOf('.') + 1);

                string url = string.Format("{0}/{1}.{2}", folder, Convert.ToString(DateTime.Now.Ticks, 16), ext);
                fileName = context.Server.MapPath(url);
                folder = context.Server.MapPath(folder);

                try
                {
                    using (var inputStream = context.Request.Files.Count > 0 ? context.Request.Files[0].InputStream : context.Request.InputStream)
                    {
                        using (var flieStream = new FileStream(fileName, FileMode.Create))
                        {
                            inputStream.CopyTo(flieStream);
                        }
                    }

                    context.Response.Write(JsonConvert.SerializeObject(new { success = true, url = url }));//{"success": false, "error": "error message to display", "reset": true}  "preventRetry": true
                }
                catch (Exception exp)
                {
                    context.Response.Write(JsonConvert.SerializeObject(new { success = false, error = exp.Message }));
                }
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    
        private string jsonString(string str)
        {
            str = str.Replace("\\", "\\\\");
            str = str.Replace("/", "\\/");
            str = str.Replace("'", "\\'");
            return str;
        }

    }
}