using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace Course.Extension.Staff
{
    public class CommonStaff : IHttpHandler
    {
        /// <summary>
        /// 您将需要在网站的 Web.config 文件中配置此处理程序 
        /// 并向 IIS 注册它，然后才能使用它。有关详细信息，
        /// 请参阅以下链接: https://go.microsoft.com/?linkid=8101007
        /// </summary>
        #region IHttpHandler Members

        public bool IsReusable
        {
            // 如果无法为其他请求重用托管处理程序，则返回 false。
            // 如果按请求保留某些状态信息，则通常这将为 false。
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            string connStr = ConfigurationManager.ConnectionStrings["DB.DbConnection"].ConnectionString;
            string serial = context.Request["serial"];
            string type = context.Request["type"];
            string password = context.Request["password"];

            if (string.IsNullOrEmpty(connStr) ||
                string.IsNullOrEmpty(serial) ||
                string.IsNullOrEmpty(type))
            {
                context.Response.Write(JsonConvert.SerializeObject(new { Success = false , Message="缺少参数。"}));
                return;
            }

            //根据type不同，获取用户的信息
            if (type == "teacher") {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    using (SqlCommand cmd = new SqlCommand("select * from Sys_Users where Remark like '%\"Serial\":\"" + serial + "\"%'", conn)){
                        using(SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                adapter.Fill(ds);
                                using(DataView dv = ds.Tables[0].DefaultView)
                                {
                                    if (dv.Count == 0)
                                    {
                                        context.Response.Write(JsonConvert.SerializeObject(new { Success = false, Message = "没有找到对象。" }));
                                        return;
                                    }
                                    else
                                    {
                                        //验证密码是否正确 
                                        string savedPasswd = dv[0]["Password"].ToString();
                                        string passSalt = dv[0]["PassSalt"].ToString();

                                        string md5pass = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(password + passSalt, "MD5");
                                        if (savedPasswd.Equals(md5pass, StringComparison.OrdinalIgnoreCase))
                                        {
                                            var obj = new { Serial = "", Gender = "", Description = "" };
                                            obj = JsonConvert.DeserializeAnonymousType(dv[0]["Remark"].ToString(), obj);
                                            
                                            context.Response.Write(JsonConvert.SerializeObject(new
                                            {
                                                Success = true,
                                                Data = new
                                                {
                                                    Serial = obj.Serial,
                                                    Name = dv[0]["TrueName"],
                                                    Gender = obj.Gender,
                                                    Type = type
                                                }
                                            }));
                                        }
                                        else
                                        {
                                            context.Response.Write(JsonConvert.SerializeObject(new
                                            {
                                                Success = false,
                                                Message="密码错误。"
                                            }));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else  if(type=="student"){
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    using (SqlCommand cmd = new SqlCommand("select * from TM_Students where StudentNumber = @Serial", conn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@Serial", serial));
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                adapter.Fill(ds);
                                using (DataView dv = ds.Tables[0].DefaultView)
                                {
                                    if (dv.Count == 0)
                                    {
                                        context.Response.Write(JsonConvert.SerializeObject(new { Success = false, Message = "没有找到对象。" }));
                                        return;
                                    }
                                    else
                                    {
                                        context.Response.Write(JsonConvert.SerializeObject(new
                                        {
                                            Success = true,
                                            Data = new
                                            {
                                                Serial = dv[0]["StudentNumber"],
                                                Name = dv[0]["Name"],
                                                Type = type
                                            }
                                        }));
                                    }
                                }
                            }

                        }
                    }
                }
            }

            context.Response.End();
        }

        #endregion
    }
}
