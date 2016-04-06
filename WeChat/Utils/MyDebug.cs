using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WeChat.Utils
{
    public class MyDebug
    {
        public static void debug(string msg)
        {
            using (StreamWriter writer = new StreamWriter(HttpContext.Current.Server.MapPath(string.Format("~/{0}.txt", DateTime.Now.Ticks.ToString()))))
            {
                writer.WriteLine(msg);
            }
        }
    }
}
