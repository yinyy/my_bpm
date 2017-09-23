using BPM.Common.Provider;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Washer.Dal;
using Washer.Model;

namespace Washer.Bll
{
    public class WasherConsumeBll
    {
        public static WasherConsumeBll Instance
        {
            get { return SingletonProvider<WasherConsumeBll>.Instance; }
        }

        public int Add(WasherConsumeModel model)
        {
            return WasherConsumeDal.Instance.Insert(model);
        }

        public string GetJson(int pageindex, int pagesize, string filterJson, string sort = "Keyid", string order = "asc")
        {
            return WasherConsumeDal.Instance.GetJson(pageindex, pagesize, filterJson, sort, order);
        }

        public WasherConsumeModel Get(int keyId)
        {
            return WasherConsumeDal.Instance.Get(keyId);
        }

        public WasherConsumeModel GetByBinderId(int binderId)
        {
            return WasherConsumeDal.Instance.GetWhere(new { BinderId = binderId/*, DepartmentId = deptId */}).FirstOrDefault();
        }

        public WasherConsumeModel Get(string unionId, string openId)
        {
            return WasherConsumeDal.Instance.GetWhere(new { UnionId = unionId, OpenId = openId }).FirstOrDefault();
        }

        public WasherConsumeModel Get(int departmentId, string telphone)
        {
            return WasherConsumeDal.Instance.GetWhere(new { DepartmentId = departmentId, Telphone = telphone }).FirstOrDefault();
        }

        public DataTable Export(string filter, string sort="KeyId", string order="asc")
        {
            return WasherConsumeDal.Instance.Export(filter, sort, order);
        }

        public WasherConsumeModel GetByBinder(WasherWeChatConsumeModel wxconsume)
        {
            return WasherConsumeDal.Instance.GetWhere(new { BinderId=wxconsume.KeyId, DepartmentId=wxconsume.DepartmentId }).FirstOrDefault();
        }

        public int Update(WasherConsumeModel consume)
        {
            return WasherConsumeDal.Instance.Update(consume);
        }

        public int GetValidCoins(int consumeId)
        {
            int coins = 0;

            #region 根据消费者编号，获取对应的洗车卡的余额
            var cards = WasherCardBll.Instance.GetValidCards(consumeId);
            if (cards.Count()== 0)
            {
                coins = 0;
            }else
            {
                coins = cards.Select(a => a.Coins).Aggregate((t, a) => { return t + a; });
            }
            #endregion

            //找到该用户尚未完成的订单
            var deviceLogs = WasherDeviceLogBll.Instance.GetByConsumeId(consumeId);
            //计算被冻结的金额
            int lockedCoins;
            if (deviceLogs.Count == 0)
            {
                lockedCoins = 0;
            }
            else
            {
                lockedCoins = deviceLogs.Select(l => l.RemainCoins).Aggregate((t, a) => { return t + a; });
            }

            //可用洗车币
            coins -= lockedCoins;

            return coins;
        }
    }
}
