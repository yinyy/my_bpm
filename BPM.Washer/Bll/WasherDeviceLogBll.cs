using BPM.Common.Provider;
using System.Collections.Generic;
using System.Linq;
using Washer.Dal;
using Washer.Model;
using System;
using System.Data;

namespace Washer.Bll
{
    public class WasherDeviceLogBll
    {
        public static WasherDeviceLogBll Instance
        {
            get { return SingletonProvider<WasherDeviceLogBll>.Instance; }
        }

        public int Add(WasherDeviceLogModel deviceLog)
        {
            return deviceLog.KeyId = WasherDeviceLogDal.Instance.Insert(deviceLog);
        }

        public int Clearing(int departmentId, int ticks, string boardNumber, int balanceId, int cost)
        {
            WasherDeviceModel device = WasherDeviceBll.Instance.Get(departmentId, boardNumber);
            WasherDeviceLogModel balance = Get(balanceId);
            if (balance == null || device==null || balance.DeviceId!=device.KeyId || balance.Ticks!=null)
            {
                return 0;
            }

            if (balance.CardId == 0)
            {
                balance.PayCoins = cost;
            }else
            {
                balance.PayCoins = WasherCardBll.Instance.Deduction(balance.CardId, cost, ticks);
            }
            balance.Ticks = ticks;
            balance.Ended = DateTime.Now;

            if (WasherDeviceLogDal.Instance.Update(balance) > 0)
            {
                return balance.PayCoins;
            }

            return 0;
        }

        public DataTable Export(string filter, string sort="KeyId", string order="desc")
        {
            return WasherDeviceLogDal.Instance.Export(filter, sort, order);
        }

        public WasherDeviceLogModel Get(int balanceId)
        {
            return WasherDeviceLogDal.Instance.GetWhere(new { KeyId = balanceId }).FirstOrDefault();
        }

        public string GetJson(int pageindex, int pagesize, string filter, string sort, string order, string fields=" * ")
        {
            return WasherDeviceLogDal.Instance.GetJson(pageindex, pagesize, filter, sort, order, fields);
        }

        public int Update(WasherDeviceLogModel balance)
        {
            return WasherDeviceLogDal.Instance.Update(balance);
        }

        public List<WasherDeviceLogModel> GetByConsumeId(int consumeId)
        {
            return WasherDeviceLogDal.Instance.GetWhere(new { ConsumeId = consumeId }).Where(l => l.Ended == null).ToList();
        }
    }
}
