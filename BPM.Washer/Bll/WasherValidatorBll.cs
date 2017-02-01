﻿using BPM.BoardListenerLib;
using BPM.Common.Provider;
using BPM.Core.Bll;
using BPM.Core.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
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

        public object ValidateCard(int departmentId, string boardNumber, string cardNo, string password = null)
        {
            dynamic dobj = new ExpandoObject();
            if (string.IsNullOrWhiteSpace(boardNumber) || string.IsNullOrWhiteSpace(cardNo))
            {
                dobj.Success = false;
                dobj.Message = "参数错误";
                return dobj;
            }

            WasherDeviceModel device;
            if ((device = WasherDeviceBll.Instance.Get(departmentId, boardNumber)) == null)
            {
                dobj.Success = false;
                dobj.Message = "设备不存在";
                return dobj;
            }

            WasherCardModel card;
            if ((card = WasherCardBll.Instance.Get(departmentId, cardNo)) == null ||
                card.ValidateEnd.Date.CompareTo(DateTime.Now.Date) <= 0 ||
                card.Coins <= 0)
            {
                dobj.Success = false;
                dobj.Message = "洗车卡不存在、洗车卡未绑定、洗车卡超过了有效期、洗车卡余额为零";
                return dobj;
            }

            if (password != null && card.BinderId == null)
            {
                dobj.Success = false;
                dobj.Message = "验证卡号+密码，但是未绑定用户。";
                return dobj;
            }

            if (card.BinderId == null)
            {
                WasherDeviceLogModel balance = new WasherDeviceLogModel();
                balance.CardId = card.KeyId;
                balance.ConsumeId = null;
                balance.DeviceId = device.KeyId;
                balance.Kind = password == null ? "刷卡" : "卡号密码";
                balance.Memo = "";
                balance.Started = DateTime.Now;
                balance.RemainCoins = card.Coins;
                balance.PayCoins = 0;

                if ((balance.KeyId = WasherDeviceLogBll.Instance.Add(balance)) < 0)
                {
                    dobj.Success = false;
                    dobj.Message = "写入设备日志错误";
                    return dobj;
                }

                dobj.Success = true;
                dobj.Money = balance.RemainCoins;
                dobj.BalanceId = balance.KeyId;
                return dobj;
            }
            else
            {
                WasherConsumeModel consume;
                if ((consume = WasherConsumeBll.Instance.Get(card.BinderId.Value)) == null || (password != null && consume.Password != password))
                {
                    dobj.Success = false;
                    dobj.Message = "用户不存在、密码错误";
                    return dobj;
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

                if ((balance.KeyId = WasherDeviceLogBll.Instance.Add(balance)) < 0)
                {
                    dobj.Success = false;
                    dobj.Message = "写入设备日志错误";
                    return dobj;
                }

                dobj.Success = true;
                dobj.Money = balance.RemainCoins;
                dobj.BalanceId = balance.KeyId;
                return dobj;
            }
        }

        /// <summary>
        /// 验证主板号和微信用户的编号
        /// </summary>
        /// <param name="boardNumber">主板号</param>
        /// <param name="wxid">微信用户编号</param>
        /// <returns>返回：BalanceId, RemainConis, ListenerIp, OpenId, DepartmentId</returns>
        public object ValidateWxConsume(int departmentId, string boardNumber, int wxid)
        {
            dynamic dobj = new ExpandoObject();
            if (string.IsNullOrWhiteSpace(boardNumber) || wxid < 0)
            {
                dobj.Success = false;
                dobj.Message = "参数错误";
                return dobj;
            }

            WasherDeviceModel device;
            if ((device = WasherDeviceBll.Instance.Get(departmentId, boardNumber)) == null)
            {
                dobj.Success = false;
                dobj.Message = "设备不存在";
                return dobj;
            }

            WasherWeChatConsumeModel wxconsume;
            if ((wxconsume = WasherWeChatConsumeBll.Instance.Get(wxid)) == null)
            {
                dobj.Success = false;
                dobj.Message = "微信用户不存在";
                return dobj;
            }

            WasherConsumeModel consume;
            if ((consume = WasherConsumeBll.Instance.GetByBinder(wxconsume)) == null)
            {
                dobj.Success = false;
                dobj.Message = "用户不存在";
                return dobj;
            }

            int coins = WasherConsumeBll.Instance.GetValidCoins(consume.KeyId);
            if (coins <= 0)
            {
                dobj.Success = false;
                dobj.Message = "余额小于0";
                return dobj;
            }

            var o = new { MaxPayCoins = 0 };
            o = JsonConvert.DeserializeAnonymousType(consume.Setting, o);

            WasherDeviceLogModel balance = new WasherDeviceLogModel();
            balance.CardId = WasherCardBll.Instance.GetValidCards(consume.KeyId).OrderBy(a => a.ValidateEnd).FirstOrDefault().KeyId;
            balance.ConsumeId = consume.KeyId;
            balance.DeviceId = device.KeyId;
            balance.Kind = "余额洗车";
            balance.Memo = "";
            balance.PayCoins = 0;
            balance.RemainCoins = o.MaxPayCoins == 0 ? coins : Math.Min(o.MaxPayCoins, coins);
            balance.Started = DateTime.Now;

            if ((balance.KeyId = WasherDeviceLogBll.Instance.Add(balance)) < 0)
            {
                dobj.Success = false;
                dobj.Message = "写入设备日志错误";
                return dobj;
            }

            Department dept = DepartmentBll.Instance.Get(device.DepartmentId);
            dobj.Success = true;
            dobj.BalanceId = balance.KeyId;
            dobj.RemainCoins = balance.RemainCoins;
            dobj.ListenerIp = device.ListenerIp;
            dobj.OpenId = wxconsume.OpenId;
            dobj.Appid = dept.Appid;
            dobj.Secret = dept.Secret;
            return dobj;
        }

        public object ValidatePaySerial(string serial)
        {
            dynamic dobj = new ExpandoObject();
            WasherOrderModel order;
            if (string.IsNullOrWhiteSpace(serial) || (order = WasherOrderBll.Instance.Get(serial)) == null)
            {
                dobj.Success = false;
                dobj.Message = "订单不存在";
                return dobj;
            }

            if (order.Status != "已支付")
            {
                dobj.Success = false;
                dobj.Message = "订单未支付";
                return dobj;
            }

            WasherDeviceModel device = WasherDeviceBll.Instance.Get(order.DepartmentId, order.Memo);
            WasherWeChatConsumeModel wxconsume = WasherWeChatConsumeBll.Instance.Get(order.DepartmentId, order.OpenId);
            WasherConsumeModel consume = WasherConsumeBll.Instance.GetByBinder(wxconsume);

            //将支付信息写入设备日志
            WasherDeviceLogModel balance = new WasherDeviceLogModel();
            balance.CardId = 0;
            balance.ConsumeId = consume.KeyId;
            balance.DeviceId = device.KeyId;
            balance.Kind = "微信支付";
            balance.Memo = serial;
            balance.PayCoins = 0;
            balance.RemainCoins = order.Money;
            balance.Started = DateTime.Now;

            if ((balance.KeyId = WasherDeviceLogBll.Instance.Add(balance)) < 0)
            {
                dobj.Success = false;
                dobj.Message = "写入设备日志错误";
                return dobj;
            }

            Department dept = DepartmentBll.Instance.Get(device.DepartmentId);
            dobj.Success = true;
            dobj.BoardNumber = device.BoardNumber;
            dobj.BalanceId = balance.KeyId;
            dobj.RemainCoins = balance.RemainCoins;
            dobj.ListenerIp = device.ListenerIp;
            dobj.OpenId = wxconsume.OpenId;
            dobj.DepartmentId = device.DepartmentId;
            dobj.Appid = dept.Appid;
            dobj.Secret = dept.Secret;
            return dobj;
        }

        public object ValidatePhone(int departmentId, string boardNumber, string phone, string password)
        {
            dynamic dobj = new ExpandoObject();

            if (string.IsNullOrWhiteSpace(boardNumber) || string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(password))
            {
                dobj.Success = false;
                dobj.Message = "参数错误";
                return dobj;
            }

            WasherDeviceModel device;
            if ((device = WasherDeviceBll.Instance.Get(departmentId, boardNumber)) == null)
            {
                dobj.Success = false;
                dobj.Message = "设备不存在";
                return dobj;
            }

            WasherConsumeModel consume;
            if ((consume = WasherConsumeBll.Instance.Get(device.DepartmentId, phone)) == null || consume.Password != password)
            {
                dobj.Success = false;
                dobj.Message = "用户不存在、密码错误";
                return dobj;
            }

            int coins = WasherConsumeBll.Instance.GetValidCoins(consume.KeyId);
            if (coins <= 0)
            {
                dobj.Success = false;
                dobj.Message = "余额不足";
                return dobj;
            }

            WasherDeviceLogModel balance = new WasherDeviceLogModel();
            balance.CardId = WasherCardBll.Instance.GetValidCards(consume.KeyId).OrderBy(a => a.ValidateEnd).FirstOrDefault().KeyId;
            balance.ConsumeId = consume.KeyId;
            balance.DeviceId = device.KeyId;
            balance.Kind = "电话密码";
            balance.Memo = "";
            balance.PayCoins = 0;
            balance.RemainCoins = coins;
            balance.Started = DateTime.Now;

            if ((balance.KeyId = WasherDeviceLogBll.Instance.Add(balance)) < 0)
            {
                dobj.Success = false;
                dobj.Message = "写入设备日志错误";
                return dobj;
            }

            dobj.Success = true;
            dobj.Money = balance.RemainCoins;
            dobj.BalanceId = balance.KeyId;
            return dobj;
        }
    }
}
