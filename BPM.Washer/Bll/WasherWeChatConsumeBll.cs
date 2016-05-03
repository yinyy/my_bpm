using BPM.Common.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Washer.Dal;
using Washer.Model;

namespace Washer.Bll
{
    public class WasherWeChatConsumeBll
    {
        public static WasherWeChatConsumeBll Instance
        {
            get { return SingletonProvider<WasherWeChatConsumeBll>.Instance; }
        }

        public WasherWeChatConsumeModel Get(int keyId)
        {
            return WasherWeChatConsumeDal.Instance.Get(keyId);
        }

        public WasherWeChatConsumeModel Get(int deptId, string weixinOpenId)
        {
            return WasherWeChatConsumeDal.Instance.GetWhere(new { DepartmentId = deptId, OpenId = weixinOpenId }).FirstOrDefault();
        }

        public int Add(WasherWeChatConsumeModel consume)
        {
            return WasherWeChatConsumeDal.Instance.Insert(consume);
        }
    }
}
