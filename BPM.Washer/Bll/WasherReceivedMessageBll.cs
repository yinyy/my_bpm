using BPM.Common.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Washer.Dal;
using Washer.Model;

namespace Washer.Bll
{
    public class WasherReceivedMessageBll
    {
        public static WasherReceivedMessageBll Instance
        {
            get { return SingletonProvider<WasherReceivedMessageBll>.Instance; }
        }

        public int Add(WasherReceivedMessageModel model)
        {
            return WasherReceivedMessageDal.Instance.Insert(model);
        } 
    }
}
