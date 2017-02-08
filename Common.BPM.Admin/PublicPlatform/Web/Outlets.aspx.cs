using BPM.Core.Bll;
using BPM.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Washer.BasePage;
using Washer.Bll;
using Washer.Extension;
using Washer.Model;

namespace BPM.Admin.PublicPlatform.Web
{
    public partial class Outlets : AuthorizeBasePage
    {
        public class A
        {
            public string Province;
            public string City;
            public string Region;
            public string Address;
            public string Coordinate;
            public int Count;
            public DateTime Update;
            public double Distance;
        }

        private class B
        {
            public double lat;
            public double lng;
        }

        public List<A> devices = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string deptId = Session["deptId"].ToString();
                string openid = Session["openid"].ToString();

                WasherWeChatConsumeModel wxconsume = WasherWeChatConsumeBll.Instance.Get(Convert.ToInt32(deptId), openid);
                B b = null;
                if (!string.IsNullOrEmpty(wxconsume.Coordinate))
                {
                    b = Convert_GCJ02_To_BD09(Convert.ToDouble(wxconsume.Coordinate.Substring(0, wxconsume.Coordinate.IndexOf(','))),
                        Convert.ToDouble(wxconsume.Coordinate.Substring(wxconsume.Coordinate.IndexOf(',') + 1)));
                }

                Department dept = DepartmentBll.Instance.Get(Convert.ToInt32(deptId));
                if (dept != null)
                {
                    devices = WasherDeviceBll.Instance.GetByDepartment(dept.KeyId).
                        Where(d => !string.IsNullOrWhiteSpace(d.Address) && !string.IsNullOrWhiteSpace(d.IpAddress) && d.UpdateTime != null).
                        GroupBy(d => new { Province = d.Province, City = d.City, Region = d.Region, Address = d.Address, Coordinate = d.Coordinate }).
                        Select(g => new A
                        {
                            Province = g.Key.Province,
                            City = g.Key.City,
                            Region = g.Key.Region,
                            Address = g.Key.Address,
                            Count = g.Count(),
                            Update = g.Select(d => d.UpdateTime).OrderByDescending(t => t).FirstOrDefault(),
                            Coordinate = g.Key.Coordinate,
                            Distance = 0
                        }).OrderBy(d => d.Address).ToList();

                    foreach (var d in devices)
                    {
                        if (b == null || string.IsNullOrEmpty(d.Coordinate))
                        {
                            d.Distance = int.MaxValue;
                        }
                        else
                        {
                            string lng = d.Coordinate.Substring(0, d.Coordinate.IndexOf(','));
                            string lat = d.Coordinate.Substring(d.Coordinate.IndexOf(',') + 1);

                            d.Distance = GetDistance(b.lng, b.lat, Convert.ToDouble(lng), Convert.ToDouble(lat));
                        }
                    }

                    devices = devices.OrderBy(d => d.Distance).ToList();
                }
            }
        }

        /// <summary>
        /// 腾讯地图坐标转百度地图坐标
        /// </summary>
        /// <param name="">lng 腾讯地图坐标的经度</param>
        /// <param name="">lat 腾讯地图坐标的纬度</param>
        /// <returns>纬度经度</returns>
        private B Convert_GCJ02_To_BD09(double lng, double lat)
        {
            double pi = 3.14159265358979324 * 3000.0 / 180.0;
            double z = Math.Sqrt(lng * lng + lat * lat) + 0.00002 * Math.Sin(lat * pi);
            double theta = Math.Atan2(lat, lng) + 0.000003 * Math.Cos(lng * pi);
            lng = z * Math.Cos(theta) + 0.0065;
            lat = z * Math.Sin(theta) + 0.006;
            return new B() { lng = lng, lat = lat };
        }

        /// <summary>
        /// 百度地图坐标计算
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private double Rad(double value)
        {
            return value * 3.1415926535898 / 180.0;
        }

        private double GetDistance(double lng1, double lat1, double lng2, double lat2)
        {
            double EARTH_RADIUS = 6378.137;//地球的半径
            double radLat1 = Rad(lat1);
            double radLat2 = Rad(lat2);
            double a = radLat1 - radLat2;

            double b = Rad(lng1) - Rad(lng2);

            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
            s = s * EARTH_RADIUS;
            s = Math.Round(s * 10000) / 10000;
            s = s * 1000;
            return Math.Ceiling(s);
        }
    }
}