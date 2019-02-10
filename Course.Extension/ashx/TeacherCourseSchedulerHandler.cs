using Course.Extension.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

namespace Course.Extension.ashx
{
    /// <summary>
    /// 在教学质量监控系统部署时，使用5.0.8的newtonsoft
    /// </summary>
    public class TeacherCourseScheduleHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            string connStr = ConfigurationManager.ConnectionStrings["DB.DbConnection"].ConnectionString;

            if (string.IsNullOrEmpty(connStr))
            {
                context.Response.Write(JsonConvert.SerializeObject(new { Success = false, Message = "缺少数据库连接字符串。" }));
                return;
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                DateTime termStart, termEnd;
                int termId;

                using (SqlCommand cmd = new SqlCommand("select * from TM_Term where Status = @Status", conn))
                {
                    cmd.Parameters.AddWithValue("@Status", "当前学期");
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);

                        DataView dv = ds.Tables[0].DefaultView;
                        if (dv.Count == 0)
                        {
                            context.Response.Write(JsonConvert.SerializeObject(new { Success = false, Message = "没有找到当前学期。" }));
                            return;
                        }

                        termId = Convert.ToInt32(dv[0]["KeyId"]);
                        termStart = Convert.ToDateTime(dv[0]["TermStart"]);
                        termEnd = Convert.ToDateTime(dv[0]["TermEnd"]);
                    }
                }

                DateTime today;
                if (string.IsNullOrEmpty(context.Request["date"]))
                {
                    today = DateTime.Today;
                }
                else
                {
                    today = DateTime.ParseExact(context.Request["date"], "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                }

                if (termStart > today)
                {
                    context.Response.Write(JsonConvert.SerializeObject(new { Success = false, Message = "当前学期未开始。" }));
                    return;
                }
                if (termEnd < today)
                {
                    context.Response.Write(JsonConvert.SerializeObject(new { Success = false, Message = "当前学期已结束。" }));
                    return;
                }

                //把termStart对齐到周一
                while (termStart.DayOfWeek != DayOfWeek.Monday)
                {
                    termStart = termStart.AddDays(1);
                }

                int days = (today - termStart).Days;
                int week = days / 7 + 1;
                string[] dayOfWeek = { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };

                List<TeacherModel> teacherList = new List<TeacherModel>();
                using (SqlCommand cmd = new SqlCommand("select KeyId, Remark from sys_users", conn))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);

                        DataView dv = ds.Tables[0].DefaultView;
                        var obj = new { Serial = "", OpenId = "" };
                        foreach (DataRowView drv in dv)
                        {
                            string remark = drv["Remark"] == null ? "" : drv["Remark"].ToString();
                            if (string.IsNullOrEmpty(remark))
                            {
                                continue;
                            }

                            try
                            {
                                obj = JsonConvert.DeserializeAnonymousType(remark, obj);
                            }
                            catch
                            {
                                continue;
                            }

                            TeacherModel teacher = new TeacherModel();
                            teacher.KeyId = Convert.ToInt32(drv["KeyId"]);
                            //staff.Name = drv["TrueName"].ToString();
                            teacher.Serial = obj.Serial;
                            teacherList.Add(teacher);
                        }
                    }
                }

                using (SqlCommand cmd = new SqlCommand("select TeacherID, CourseTime, " +
                    "VClassDescription, Title, CourseName " +
                    "from V_TM_TeachCourseInfo " +
                    "where TermID = @TermID " +
                    "and CourseWeekStart <= @Week and CourseWeekEnd >= @Week " +
                    "and CourseDay = @DayOfWeek " +
                    "order by TeacherId, CourseId, CourseTime", conn))
                {
                    cmd.Parameters.AddWithValue("@TermID", termId);
                    cmd.Parameters.AddWithValue("@Week", week);
                    cmd.Parameters.AddWithValue("@DayOfWeek", dayOfWeek[(int)today.DayOfWeek]);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);

                        List<CourseSchedulerModel> courseSchedulerList = new List<CourseSchedulerModel>();
                        DataView dv = ds.Tables[0].DefaultView;
                        foreach (DataRowView drv in dv)
                        {
                            CourseSchedulerModel scheduler = new CourseSchedulerModel();
                            scheduler.TeacherSerial = teacherList.Where(t => t.KeyId == Convert.ToInt32(drv["TeacherID"])).Select(t => t.Serial).FirstOrDefault();
                            scheduler.CourseTitle = drv["CourseName"].ToString();
                            scheduler.RoomTitle = drv["Title"].ToString();
                            scheduler.ClassTitle = drv["VClassDescription"].ToString();
                            scheduler.Section = drv["CourseTime"].ToString();
                            courseSchedulerList.Add(scheduler);
                        }

                        List<CourseSchedulerModel> courseSchedulerList2 = new List<CourseSchedulerModel>();
                        foreach (var g in courseSchedulerList.GroupBy(m => new {
                            TeacherSerial = m.TeacherSerial,
                            ClassTitle = m.ClassTitle,
                            CourseTitle = m.CourseTitle,
                            RoomTitle = m.RoomTitle
                        }))
                        {
                            CourseSchedulerModel scheduler = new CourseSchedulerModel();
                            scheduler.ClassTitle = g.Key.ClassTitle;
                            scheduler.CourseTitle = g.Key.CourseTitle;
                            scheduler.RoomTitle = g.Key.RoomTitle;
                            scheduler.TeacherSerial = g.Key.TeacherSerial;

                            int[] section = new int[10];
                            foreach (var d in g)
                            {
                                string s = d.Section;
                                if (string.IsNullOrEmpty(s))
                                {
                                    continue;
                                }

                                foreach (var d1 in d.Section.Split(','))
                                {
                                    if (d1.IndexOf('-') >= 0)
                                    {
                                        for (int i = Convert.ToInt32(d1.Substring(0, d1.IndexOf('-'))); i <= Convert.ToInt32(d1.Substring(d1.IndexOf('-') + 1)); i++)
                                        {
                                            section[i - 1] = i;
                                        }
                                    }
                                    else
                                    {
                                        section[Convert.ToInt32(d1) - 1] = Convert.ToInt32(d1);
                                    }
                                }
                            }

                            string str = "";
                            while (section.Count() > 0)
                            {
                                section = section.SkipWhile(n => n == 0).ToArray();
                                if (section.Count() == 0)
                                {
                                    break;
                                }

                                var q1 = section.TakeWhile((n, i) => n != 0);
                                int a = q1.First(), b = q1.Last();
                                if (string.IsNullOrEmpty(str))
                                {
                                    if (a == b)
                                    {
                                        str += string.Format("{0}", a);
                                    }
                                    else
                                    {
                                        str += string.Format("{0}-{1}", a, b);
                                    }
                                }
                                else
                                {
                                    if (a == b)
                                    {
                                        str += string.Format("，{0}", a);
                                    }
                                    else
                                    {
                                        str += string.Format("，{0}-{1}", a, b);
                                    }
                                }
                                section = section.Skip(q1.Count()).ToArray();
                            }

                            scheduler.Section = string.IsNullOrEmpty(str) ? "未指定" : str;

                            courseSchedulerList2.Add(scheduler);
                        }

                        context.Response.Write(JsonConvert.SerializeObject(new { Success = true, Data = courseSchedulerList2.OrderBy(m => m.Section) }));
                    }
                }
            }
        }
    }
}
