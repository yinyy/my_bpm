using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Course.Core.Job
{
    public class NextDayScheduleTask : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            string filename;
            if (root.EndsWith("\\"))
            {
                filename = root + "scheduler.txt";
            }
            else
            {
                filename = root + "\\scheduler.txt";
            }

            Random random = new Random();
            using (StreamWriter writer = new StreamWriter(filename, true, Encoding.UTF8))
            {
                writer.WriteLine(string.Format("[{0:yyyy年MM月dd日 HH:mm:ss}] - {1}", DateTime.Now, random.Next(1000)));
            }
        }
    }
}
