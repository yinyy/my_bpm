using Course.Common.Bll;
using Course.Common.Model;
using Course.Core.Bll;
using Course.Core.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace BPM.Admin.PublicPlatform.Course.ashx
{
    /// <summary>
    /// StaffBranchHandler 的摘要说明
    /// </summary>
    public class StaffBranchHandler : IHttpHandler,IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            string action = context.Request["action"];

            if (action == "save")
            {
                string openid = context.Session["openid"].ToString();
                CommonStaffModel staff = CommonStaffBll.Instance.Get(openid);
                string introduction = context.Request["introduction"];
                string branchStr = context.Request["branches"];
                string[] branches = branchStr.Split(',');

                for (int i = 1; i <= 3; i++)
                {
                    //先检查有没有，有则更新，无则增加
                    CourseStaffBranchModel csbm = CourseStaffBranchBll.Instance.Get(staff.KeyId, i);
                    if (csbm == null || csbm.BranchId != Convert.ToInt32(branches[i - 1]))
                    {
                        if (csbm != null)
                        {
                            //数据库中的记录与当前选择的方向不相同，则删除
                            CourseStaffBranchBll.Instance.Delete(csbm.KeyId);
                        }

                        //新增
                        csbm = new CourseStaffBranchModel();
                        csbm.StaffId = staff.KeyId;
                        csbm.BranchId = Convert.ToInt32(branches[i - 1]);
                        csbm.Sorted = i;
                        if (i == 1)
                        {
                            csbm.Introduction = introduction;
                        }
                        else
                        {
                            csbm.Introduction = "";
                        }
                        csbm.Accepted = 0;
                        csbm.ChooseTime = DateTime.Now;

                        CourseStaffBranchBll.Instance.Insert(csbm);
                    }
                    else
                    {
                        //相同，则更新时间
                        csbm.ChooseTime = DateTime.Now;
                        if (i == 1)
                        {
                            csbm.Introduction = introduction;
                        }
                        CourseStaffBranchBll.Instance.Update(csbm);
                    }
                }

                context.Response.Write(JsonConvert.SerializeObject(new { Success = true }));
            }
            else if (action == "list")
            {
                string openid = context.Session["openid"].ToString();
                CommonStaffModel staff = CommonStaffBll.Instance.Get(openid);
                context.Response.Write(JsonConvert.SerializeObject(new { Success = true, Data = CourseStaffBranchBll.Instance.Get(staff) }));
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