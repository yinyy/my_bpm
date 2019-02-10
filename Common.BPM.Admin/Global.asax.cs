using BPM.Admin.Job;
using Combres;
using Course.Common.Bll;
using Quartz;
using Quartz.Impl;
using Senparc.CO2NET;
using Senparc.CO2NET.RegisterServices;
using Senparc.Weixin;
using Senparc.Weixin.Entities;
using Senparc.Weixin.MP.Containers;
using System;
using System.Web.Routing;

namespace BPM.Admin
{
    public class Global : System.Web.HttpApplication
    {
        private IScheduler scheduler = null;

        protected void Application_Start(object sender, EventArgs e)
        {
            RouteTable.Routes.AddCombresRoute("Combres");

            //注册Senparc
            RegisterSenparc();

            //设置计划任务，每晚8:30通知第二天的课程，每早6:45通知当天的课程
            CreateScheduleTask();
        }

        /// <summary>
        /// 一个cron表达式有至少6个（也可能7个）有空格分隔的时间元素。
        /// CronTrigger配置完整格式为： [秒] [分] [小时] [日] [月] [周] [年]
        /// https://www.cnblogs.com/jingmoxukong/p/5647869.html
        /// </summary>
        private void CreateScheduleTask()
        {
            scheduler = StdSchedulerFactory.GetDefaultScheduler();
            //IJobDetail job1 = JobBuilder.Create<TeacherClassSchedulerJobTask>().WithIdentity("TomorrowTeacherScheduleJob", "CourseScheduler").Build();
            //job1.JobDataMap.Put("type", 1);
            //ITrigger trigger1 = TriggerBuilder.Create().WithIdentity("TomorrowTeacherScheduleJobTrigger", "CourseScheduler").StartNow()
            //    .WithSchedule(CronScheduleBuilder.CronSchedule("00 30 20 * * ?")).Build();
            //scheduler.ScheduleJob(job1, trigger1);

            IJobDetail job2 = JobBuilder.Create<TeacherClassSchedulerJobTask>().WithIdentity("TodayTeacherScheduleJob", "CourseScheduler").Build();
            job2.JobDataMap.Put("type", 2);
            ITrigger trigger2 = TriggerBuilder.Create().WithIdentity("TodayTeacherScheduleJob", "CourseScheduler").StartNow()
                .WithSchedule(CronScheduleBuilder.CronSchedule("0 0 7 * * ?")).Build();
            scheduler.ScheduleJob(job2, trigger2);

            scheduler.Start();
        }

        private void RegisterSenparc()
        {
            /* CO2NET 全局注册开始
             * 建议按照以下顺序进行注册
             */

            //设置全局 Debug 状态
            var isGLobalDebug = true;
            var senparcSetting = SenparcSetting.BuildFromWebConfig(isGLobalDebug);

            //CO2NET 全局注册，必须！！
            IRegisterService register = RegisterService.Start(senparcSetting).UseSenparcGlobal(false, null);

            ///* 微信配置开始
            // * 建议按照以下顺序进行注册
            // */

            //设置微信 Debug 状态
            var isWeixinDebug = true;
            var senparcWeixinSetting = SenparcWeixinSetting.BuildFromWebConfig(isWeixinDebug);

            //微信全局注册，必须！！
            register.UseSenparcWeixin(senparcWeixinSetting, senparcSetting);

            //注册当前appid
            AccessTokenContainer.Register(senparcWeixinSetting.WeixinAppId, senparcWeixinSetting.WeixinAppSecret);
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {
            if (scheduler != null)
            {
                scheduler.Shutdown(false);
            }
        }

   
    }
}