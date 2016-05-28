using BPM.Common.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Washer.Model;

namespace Washer.Bll
{
    public class WasherValidatorBll
    {
        public static WasherValidatorBll Instance
        {
            get { return SingletonProvider<WasherValidatorBll>.Instance; }
        }

        public WasherDeviceLogModel ValidateCardAndPassword(string boardNumber, string cardNo, string password=null)
        {
            WasherDeviceModel device = WasherDeviceBll.Instance.GetByBoardNumber(boardNumber);
            if (device == null)
            {
                return null;
            }

            WasherCardModel card = WasherCardBll.Instance.Get(device.DepartmentId, cardNo);
            if (card == null || card.BinderId == null || card.ValidateEnd.Date.CompareTo(DateTime.Now.Date) <= 0 || card.Coins <= 0)
            {
                return null;
            }

            WasherConsumeModel consume = WasherConsumeBll.Instance.Get(card.BinderId.Value);
            if (consume == null || (password!=null && consume.Password!=password))
            {
                return null;
            }

            WasherDeviceLogModel balance = new WasherDeviceLogModel();
            balance.CardId = card.KeyId;
            balance.ConsumeId = consume.KeyId;
            balance.DeviceId = device.KeyId;
            balance.Kind = password == null ? "刷卡" : "卡号密码";
            balance.Memo = "";
            balance.Started = DateTime.Now;
            balance.RemainCoins = card.Coins;
            balance.PayCoins = 0;
            
            if((balance.KeyId = WasherDeviceLogBll.Instance.Add(balance)) > 0)
            {
                return balance;
            }

            return null;
        }

        public WasherDeviceLogModel ValidatePhoneAndPassword(string boardNumber, string phone, string password)
        {
            WasherDeviceModel device = WasherDeviceBll.Instance.GetByBoardNumber(boardNumber);
            if (device == null) {
                return null;
            }

            WasherConsumeModel consume = WasherConsumeBll.Instance.Get(device.DepartmentId, phone);
            if (consume == null || consume.Password!=password)
            {
                return null;
            }

            int coins = WasherConsumeBll.Instance.GetValidCoins(consume.KeyId);
            if (coins <= 0)
            {
                return null;
            }

            WasherDeviceLogModel deviceLog = new WasherDeviceLogModel();
            deviceLog.CardId = WasherCardBll.Instance.GetValidCards(consume.KeyId).OrderBy(a=>a.ValidateEnd).FirstOrDefault().KeyId;
            deviceLog.ConsumeId = consume.KeyId;
            deviceLog.DeviceId = device.KeyId;
            deviceLog.Kind = "电话密码";
            deviceLog.Memo = "";
            deviceLog.PayCoins = 0;
            deviceLog.RemainCoins = coins;
            deviceLog.Started = DateTime.Now;
            
            if((deviceLog.KeyId = WasherDeviceLogBll.Instance.Add(deviceLog)) > 0)
            {
                return deviceLog;
            }

            return null;
        }
    }
}
