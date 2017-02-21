using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Washer.Model
{
    public class WasherDepartmentSetting
    {
        public int[] WxPayOption { get; set; }
        public WasherDepartmentSettingSms Sms { get; set; }
        public WasherDepartmentSettingBuyCardOption[] BuyCardOption { get; set; }
        public int[] GiftLevel { get; set; }
        public WasherDepartmentSettingRegister Register { get; set; }

        public WasherDepartmentSettingPayWashCar PayWashCar{get;set;}
        public WasherDepartmentSettingRelay Relay { get; set; }

        private WasherDepartmentSetting()
        {

        }

        public static WasherDepartmentSetting Instance
        {
            get
            {
                WasherDepartmentSetting setting = new WasherDepartmentSetting();
                setting.WxPayOption = new int[] { };
                setting.Sms = new WasherDepartmentSettingSms {Cid="", Pas="", Uid="", Url="" };
                setting.BuyCardOption = new WasherDepartmentSettingBuyCardOption[] { };
                setting.GiftLevel = new int[] { };
                setting.Register = new WasherDepartmentSettingRegister {Coupon=0, CouponDay=0, Point=0 };
                setting.PayWashCar = new WasherDepartmentSettingPayWashCar { Coupon=0, Vip=0, Wx=0};
                setting.Relay = new WasherDepartmentSettingRelay() { Friend = 0, Moment = 0 };

                return setting;
            }
        }
    }

    public class WasherDepartmentSettingSms
    {
        public string Cid { get; set; }
        public string Uid { get; set; }
        public string Pas { get; set; }
        public string Url { get; set; }
    }

    public class WasherDepartmentSettingRegister
    {
        public int Coupon { get; set; }
        public int CouponDay { get; set; }
        public int Point { get; set; }
    }

    public class WasherDepartmentSettingBuyCardOption
    {
        public int Value { get; set; }
        public float Price { get; set; }
        public int Day { get; set; }
        public string Product { get; set; }
        public int Score { get; set; }
    }

    public class WasherDepartmentSettingPayWashCar
    {
        public int Wx { get; set; }
        public int Vip { get; set; }
        public int Coupon { get; set; }
    }

    public class WasherDepartmentSettingRelay
    {
        public int Friend { get; set; }
        public int Moment { get; set; }
    }
}