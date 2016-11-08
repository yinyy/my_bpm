using BPM.Agriculture.Dal;
using BPM.Agriculture.Model;
using BPM.Common.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPM.Agriculture.Bll
{
    public class AgricultureSubscriberBll
    {
        public static AgricultureSubscriberBll Instance
        {
            get { return SingletonProvider<AgricultureSubscriberBll>.Instance; }
        }

        public string JsonDataForEasyUIdataGrid(int pageindex, int pagesize, string filter, string sort = "KeyId", string order = "asc")
        {
            return AgricultureSubscriberDal.Instance.JsonDataForEasyUIdataGrid(pageindex, pagesize, filter, sort, order);
        }

        public int Delete(int keyId)
        {
            return AgricultureSubscriberDal.Instance.Delete(keyId);
        }

        public int Add(AgricultureSubscriberModel m)
        {
            return AgricultureSubscriberDal.Instance.Insert(m);
        }

        public string JsonDataForSubscriber(string openid)
        {
            return AgricultureSubscriberDal.Instance.JsonDataForSubscriber(openid);
        }

        public object Get(string openid)
        {
            throw new NotImplementedException();
        }

        public AgricultureSubscriberModel Get(string weixinOpenId, int deviceId)
        {
            return AgricultureSubscriberDal.Instance.GetWhere(new { OpenId = weixinOpenId, DeviceId = deviceId }).FirstOrDefault();
        }

        public List<AgricultureSubscriberModel> GetByDeviceId(int deviceId)
        {
            return AgricultureSubscriberDal.Instance.GetWhere(new { DeviceId = deviceId }).ToList();
        }
    }
}
