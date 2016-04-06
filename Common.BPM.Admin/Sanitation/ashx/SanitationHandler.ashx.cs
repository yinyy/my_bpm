using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Sanitation.Bll;
using Sanitation.Model;
using BPM.Core.Model;
using BPM.Core.Bll;
using BPM.Core.Dal;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace BPM.Admin.Sanitation.ashx
{
    /// <summary>
    /// SanitationHandler 的用途是：
    /// 1、SIM900A模块获取调度信息
    /// 2、保存加水信息
    /// </summary>
    public class SanitationHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string action = context.Request.Params["action"];
            string code=null;
            string plate=null;
            double lng=0;
            double lat=0;
            int pipe=0;
            float volumn = 0;
            string address = null;
            int kind = 0;
            int potency = 0;

            SanitationDispatchModel dispatch;

            string log_dir = context.Server.MapPath("~/logs");
            if(!Directory.Exists(log_dir)){
            Directory.CreateDirectory(log_dir);
            }

            switch (action)
            {
                case "save":
                    try
                    {
                        code = context.Request.Params["driver"];
                        plate = context.Request.Params["trunk"];
                        volumn = Convert.ToSingle(context.Request.Params["volumn"]);
                        address = context.Request.Params["address"];
                        kind = Convert.ToInt32(context.Request.Params["kind"]);
                        potency = Convert.ToInt32(context.Request.Params["potency"]);

                        dispatch = new SanitationDispatchModel();
                        dispatch.DriverId = SanitationDriverBll.Instance.GetByCode(code).KeyId;
                        dispatch.Kind = kind;
                        dispatch.Potency = potency;
                        dispatch.Status = 0;
                        dispatch.TrunkId = SanitationTrunkBll.Instance.GetByPlate(plate).KeyId;
                        dispatch.Time = DateTime.Now;
                        dispatch.Address = DicDal.Instance.GetWhere(new { Code = address }).FirstOrDefault().Title;
                        dispatch.Volumn = volumn;

                        context.Response.Write("SavedSuccess:" + SanitationDispatchBll.Instance.Add(dispatch));
                    }
                    catch (Exception e)
                    {
                        string log_file = string.Format("{0}\\{1}.txt", log_dir, Convert.ToString(DateTime.Now.Ticks, 16));
                        using (StreamWriter writer = new StreamWriter(new FileStream(log_file, FileMode.CreateNew)))
                        {
                            writer.WriteLine(string.Format("code={0},plate={1},volumn={2},address={3},kind={4},potency={5}", code, plate, volumn, address, kind, potency));
                            writer.WriteLine(e.Message);
                            writer.WriteLine(e.StackTrace);
                        }

                        context.Response.Write("error");
                    }
                    finally
                    {
                        context.Response.Flush();
                    }
                    break;
                case "current"://车载设备获得当前任务的方法
                    code = context.Request.Params["code"];
                    plate = context.Request.Params["plate"];

                    dispatch = SanitationDispatchBll.Instance.Current(code, plate);

                    context.Response.Write(string.Format("current:{0}", dispatch == null ? 0 : dispatch.KeyId));
                    context.Response.Flush();
                    break;
                case "sign"://车载设备签到的方法
                    code = context.Request.Params["code"];
                    plate = context.Request.Params["plate"];
                    lng=Convert.ToDouble( context.Request.Params["lng"]);
                    lng /= 100.0;
                    lat = Convert.ToDouble(context.Request.Params["lat"]);
                    lat /= 100.0;
                    pipe = Convert.ToInt32(context.Request.Params["pipe"]);

                    dispatch = SanitationDispatchBll.Instance.Current(code, plate);
                    dispatch.Signed = DateTime.Now;
                    dispatch.Destination = string.Format("{0},{1}",lng,lat);
                    dispatch.Working = pipe;
                    dispatch.Region = isInRegion(lng, lat);//TODO:这个将来需要通过计算验证是在区域外签到还是区域内签到的
                    dispatch.Status = 1;

                    context.Response.Write(string.Format("signed:{0}", SanitationDispatchBll.Instance.Update(dispatch)));
                    context.Response.Flush();
                    break;
                default:
                    string result = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (string.IsNullOrEmpty(result))
                    {
                        result = context.Request.ServerVariables["REMOTE_ADDR"];
                    }
                    if (string.IsNullOrEmpty(result))
                    {
                        result = context.Request.UserHostAddress;
                    }
                    if (string.IsNullOrEmpty(result))
                    {
                        result = "0.0.0.0";
                    }

                    context.Response.Write(result);
                    context.Response.Flush();
                    break;
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private int isInRegion(double lng, double lat)
        {
            lng = (int)lng + (lng - (int)lng) * 100 / 60;
            lat = (int)lat + (lat - (int)lat) * 100 / 60;

            string url = string.Format("http://api.map.baidu.com/geoconv/v1/?ak=FBddae84e942f0b6b28aa762786b00f8&coords={0},{1}", lng, lat);
            WebRequest wq = WebRequest.Create(url);
            WebResponse ws = wq.GetResponse();
            Stream stream = ws.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            string json = reader.ReadToEnd();

            A a = JsonConvert.DeserializeObject<A>(json);
            if (a.status == 0)
            {
                double x = a.result[0].x;
                double y = a.result[0].y;

                reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Sanitation/js/SanitationMapData.js"));
                json = reader.ReadToEnd();
                json = json.Substring(json.IndexOf('['), json.IndexOf(']') - json.IndexOf('[') + 1);

                B[] region = JsonConvert.DeserializeObject<B[]>(json);
                int littlePoints = 0;
                int largePoints = 0;
                for (int i = 0; i < region.Length; i++)
                {
                    B b1 = region[i];
                    B b2 = region[(i + 1) % region.Length];

                    if (y >= Math.Min(b1.y, b2.y) && y <= Math.Max(b1.y, b2.y))
                    {
                        if ((b2.y == b1.y) && (y == b2.y))
                        {//水平线
                            littlePoints = 1;
                            largePoints = 1;
                            break;
                        }
                        else if (b2.x == b1.x)
                        {//垂直线
                            if (x < b2.x)
                            {
                                largePoints++;
                            }
                            else if (x > b2.x)
                            {
                                littlePoints++;
                            }
                            else
                            {
                                largePoints = 1;
                                littlePoints = 1;
                                break;
                            }
                        }
                        else
                        {
                            double k = (b2.y - b1.y) / (b2.x - b1.x);
                            double b = b2.y - k * b2.x;

                            double tx = (y - b) / k;
                            if (x < tx)
                            {
                                largePoints++;
                            }
                            else if (x > tx)
                            {
                                littlePoints++;
                            }
                            else
                            {
                                largePoints = 1;
                                littlePoints = 1;
                                break;
                            }
                        }
                    }
                }

                if ((littlePoints % 2 == 1) && (largePoints % 2 == 1))
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }

            return 0;
        }
    }
    class A
    {
        public int status { get; set; }
        public B[] result { get; set; }
    }

    class B
    {
        public double x { get; set; }
        public double y { get; set; }
    }
}