using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Washer.Model
{
    public class WasherDepartmentSetting
    {
        public WasherDepartmentSettingPoint Point { get; set; }
        public WasherDepartmentSettingCoin Coin { get; set; }
        public WasherDepartmentSettingCoupon Coupon { get; set; }
        public WasherDepartmentSettingBuy[] Buy { get; set; }
        
        private WasherDepartmentSetting()
        {

        }

        public static WasherDepartmentSetting Instance
        {
            get
            {
                WasherDepartmentSetting setting = new WasherDepartmentSetting();
                setting.Point = new WasherDepartmentSettingPoint()
                {
                    WasherCar = 0,
                    Subscribe = 0,
                    Recharge = new int[3],
                    Referers = new WasherDepartmentSettingPointReferer()
                    {
                        Kind = "",
                        Level = new int[5]
                    }
                };
                setting.Coin = new WasherDepartmentSettingCoin()
                {
                    Exchange = 0,
                    Recharge = new int[3]
                };
                setting.Coupon = new WasherDepartmentSettingCoupon()
                {
                    Coins = 0,
                    Time = 0
                };
                setting.Buy = new WasherDepartmentSettingBuy[4];

                return setting;
            }
        }
    }

    public class WasherDepartmentSettingPoint
    {
        public int WasherCar { get; set; }
        public int Subscribe { get; set; }
        public int[] Recharge { get; set; }
        public WasherDepartmentSettingPointReferer Referers { get; set; }
    }

    public class WasherDepartmentSettingPointReferer
    {
        public string Kind { get; set; }
        public int[] Level { get; set; }
    }

    public class WasherDepartmentSettingCoin
    {
        public int Exchange { get; set; }
        public int[] Recharge { get; set; }
    }

    public class WasherDepartmentSettingCoupon
    {
        public int Coins { get; set; }
        public int Time { get; set; }
    }

    public class WasherDepartmentSettingBuy
    {
        public int Card { get; set; }
        public float Price { get; set; }
        public int Day { get; set; }
        public string Product { get; set; }
        public int Score { get; set; }
    }
}