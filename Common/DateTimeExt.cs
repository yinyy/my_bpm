using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPM.Common
{
    public static class DateTimeExt
    {
        public static string Quarter(this DateTime dt, string formatter)
        {
            DateTime d =dt.AddDays(1 - dt.Day);//本月第一天

            switch (d.Month)
            {
                case 1:
                case 2:
                case 3:
                    if (formatter == "small")
                    {
                        return "Q1";
                    }
                    else if(formatter=="large")
                    {
                        return "第一季度";
                    }
                    break;
                case 4:
                case 5:
                case 6:
                    if (formatter == "small")
                    {
                        return "Q2";
                    }
                    else if (formatter == "large")
                    {
                        return "第二季度";
                    }
                    break;
                case 7:
                case 8:
                case 9:
                    if (formatter == "small")
                    {
                        return "Q3";
                    }
                    else if (formatter == "large")
                    {
                        return "第三季度";
                    }
                    break;
                case 10:
                case 11:
                case 12:
                    if (formatter == "small")
                    {
                        return "Q4";
                    }
                    else if (formatter == "large")
                    {
                        return "第四季度";
                    }
                    break;
                default:
                    return "";
            }

            return "";
        }
    }
}
