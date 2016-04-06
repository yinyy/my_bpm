using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using BPM.Common;
using BPM.Core.Model;
using BPM.Core.Bll;

namespace BPM.Admin.Washer.ashx
{
    /// <summary>
    /// WasherCityHandler 的摘要说明
    /// </summary>
    public class WasherDistrictHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            switch (context.Request.Params["action"])
            {
                case "province":
                    context.Response.Write(JSONhelper.ToJson(DistrictBll.Instance.GetDistricts().Select(d => new { KeyId = d.KeyId + "_" + d.Name, Title = d.Name })));
                    break;
                case "city":
                    int pid = Convert.ToInt32(context.Request.Params["pid"]);
                    context.Response.Write(JSONhelper.ToJson(DistrictBll.Instance.GetDistricts(pid).Select(c => new { KeyId = c.KeyId + "_" + c.Name, Title = c.Name })));
                    break;
                case "region":
                    int cid = Convert.ToInt32(context.Request.Params["cid"]);
                    context.Response.Write(JSONhelper.ToJson(DistrictBll.Instance.GetDistricts(cid).Select(a => new { KeyId = a.KeyId + "_" + a.Name, Title = a.Name })));
                    break;
                default:
                    //JArray ps, cs, ars;
                    //using (Stream stream = new FileStream(context.Server.MapPath("/Washer/js/Province.data"), FileMode.Open))
                    //{
                    //    ps  = JArray.Load(new JsonTextReader(new StreamReader(stream)));
                    //}
                    //using (Stream stream = new FileStream(context.Server.MapPath("/Washer/js/City.data"), FileMode.Open))
                    //{
                    //    cs = JArray.Load(new JsonTextReader(new StreamReader(stream)));
                    //}
                    //using (Stream stream = new FileStream(context.Server.MapPath("/Washer/js/Area.data"), FileMode.Open))
                    //{
                    //    ars = JArray.Load(new JsonTextReader(new StreamReader(stream)));
                    //}

                    //foreach(var o1 in ps)
                    //{
                    //    District d1 = new District();
                    //    d1.Memo = Convert.ToString(o1["ProRemark"]);
                    //    d1.Name = Convert.ToString(o1["name"]);
                    //    d1.ParentId = 0;
                    //    d1.Sort = Convert.ToInt32(o1["ProSort"]);
                    //    d1.KeyId = DistrictBll.Instance.Add(d1);

                    //    foreach(var o2 in cs.Where(c => Convert.ToInt32(c["ProID"])==Convert.ToInt32(o1["ProID"])))
                    //    {
                    //        District d2 = new District();
                    //        d2.Memo = "";
                    //        d2.Name = Convert.ToString(o2["name"]);
                    //        d2.ParentId = d1.KeyId;
                    //        d2.Sort = Convert.ToInt32(o2["CitySort"]);
                    //        d2.KeyId = DistrictBll.Instance.Add(d2);

                    //        foreach (var o3 in ars.Where(a => Convert.ToInt32(a["CityID"]) == Convert.ToInt32(o2["CityID"])))
                    //        {
                    //            District d3 = new District();
                    //            d3.Memo = "";
                    //            d3.Name = Convert.ToString(o3["DisName"]);
                    //            d3.ParentId = d2.KeyId;
                    //            d3.Sort = 0;
                    //            DistrictBll.Instance.Add(d3);
                    //        }
                    //    }
                    //}
                    string provinceName = context.Request.Params["province"];
                    string cityName = context.Request.Params["city"];
                    string regionName = context.Request.Params["region"];

                    District province = DistrictBll.Instance.GetDistrict(provinceName);
                    District city = DistrictBll.Instance.GetDistrict(cityName, province);
                    District region = DistrictBll.Instance.GetDistrict(regionName, city);

                    context.Response.Write(JSONhelper.ToJson(new { pid = province == null ? -1 : province.KeyId, cid = city == null ? -1 : city.KeyId, rid = region == null ? -1 : region.KeyId }));
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
    }
}