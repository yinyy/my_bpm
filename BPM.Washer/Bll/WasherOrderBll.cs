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
    public class WasherOrderBll
    {
        public static class Payment
        {
            public const string ScanQrcode = "扫码支付";
            public const string PutCoin = "投币支付";
            public const string Recharge = "充值";
        }

        public static WasherOrderBll Instance
        {
            get { return SingletonProvider<WasherOrderBll>.Instance; }
        }

        public int Add(WasherOrderModel model)
        {
            return WasherOrderDal.Instance.Insert(model);
        }

        public WasherOrderModel Get(string serial)
        {
            return WasherOrderDal.Instance.GetWhere(new { Serial = serial }).FirstOrDefault();
        }

        public int Update(WasherOrderModel order)
        {
            return WasherOrderDal.Instance.Update(order);
        }
    }
}
