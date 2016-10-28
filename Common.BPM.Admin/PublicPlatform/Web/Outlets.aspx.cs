using BPM.Core.Bll;
using BPM.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Washer.Bll;
using Washer.Extension;
using Washer.Model;

namespace BPM.Admin.PublicPlatform.Web
{
    public partial class Outlets : System.Web.UI.Page
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
        }

        public List<A> devices = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string deptId = Request.Params["appid"];
                if (!string.IsNullOrEmpty(deptId))
                {
                    Department dept = DepartmentBll.Instance.Get(Convert.ToInt16(deptId));
                    if (dept != null)
                    {
                        devices = WasherDeviceBll.Instance.GetByDepartment(dept.KeyId).
                            Where(d => !string.IsNullOrWhiteSpace(d.Address) && !string.IsNullOrWhiteSpace(d.IpAddress) && d.UpdateTime != null).
                            GroupBy(d => new { Province = d.Province, City = d.City, Region = d.Region, Address = d.Address,Coordinate=d.Coordinate }).
                            Select(g => new A
                            {
                                Province = g.Key.Province,
                                City = g.Key.City,
                                Region = g.Key.Region,
                                Address = g.Key.Address,
                                Count = g.Count(),
                                Update = g.Select(d => d.UpdateTime).OrderByDescending(t => t).FirstOrDefault(),
                                Coordinate = g.Key.Coordinate
                            }).OrderBy(d=>d.Address).ToList();
                        
                    }
                }
            }
        }
    }
}