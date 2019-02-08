using BPM.Common.Provider;
using Course.Common.Dal;
using Course.Common.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Course.Common.Bll
{
    public class CommonMessageBll
    {
        public static CommonMessageBll Instance
        {
            get { return SingletonProvider<CommonMessageBll>.Instance; }
        }

        public int Insert(CommonMessageModel message)
        {
            return CommonMessageDal.Instance.Insert(message);
        }
    }
}
