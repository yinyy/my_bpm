using BPM.Common.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Washer.Dal;
using Washer.Model;
using System.Web.UI.WebControls;
using System.Data;

namespace Washer.Bll
{
    public class WasherCardBll
    {
        public static WasherCardBll Instance
        {
            get { return SingletonProvider<WasherCardBll>.Instance; }
        }

        public WasherCardModel Get(WasherConsumeModel consume)
        {
            return WasherCardDal.Instance.GetWhere(new { DepartmentId = consume.DepartmentId, BinderId = consume.KeyId }).OrderByDescending(a => a.Binded).FirstOrDefault();
        }

        public WasherCardModel Get(int departmentId, string cardNo)
        {
            return WasherCardDal.Instance.GetWhere(new { DepartmentId = departmentId, CardNo = cardNo }).FirstOrDefault();
        }

        public int GetCardCountByValue(int departmentId, int value)
        {
            int count = 0;
            var q = WasherCardDal.Instance.GetWhere(new { DepartmentId = departmentId, Coins = value, Kind = "Sale" }).Where(a => a.BinderId == null && a.ValidateEnd.CompareTo(DateTime.Now) > 0);
            foreach(WasherCardModel c in q)
            {
                if(c.Locked==null || (DateTime.Now - c.Locked.Value).TotalMinutes > 15)
                {
                    count++;
                }
            }

            //return WasherCardDal.Instance.GetWhere(new { DepartmentId = departmentId, Coins = value, Kind = "Sale"}).Where(a => a.BinderId == null && a.ValidateEnd.CompareTo(DateTime.Now) > 0).Count();
            return count;
        }

        public IEnumerable<WasherCardModel> GetValidCards(int consumeId)
        {
            return WasherCardDal.Instance.GetWhere(new { BinderId = consumeId }).Where(a => a.ValidateEnd.Date.CompareTo(DateTime.Now.Date) >= 0);//.OrderByDescending(a => a.ValidateEnd)
        }

        public WasherCardModel Get(int keyId)
        {
            return WasherCardDal.Instance.Get(keyId);
        }

        public int Update(WasherCardModel card)
        {
            return WasherCardDal.Instance.Update(card);
        }

        public string GetJson(int pageindex, int pagesize, string filterJson, string sort = "Keyid", string order = "asc")
        {
            return WasherCardDal.Instance.GetJson(pageindex, pagesize, filterJson, sort, order);
        }

        public int Add(WasherCardModel model)
        {
            return WasherCardDal.Instance.Insert(model);
        }

        public int Delete(int keyId)
        {
            return WasherCardDal.Instance.Delete(keyId);
        }

        public int Deduction(int cardId, int cost, int ticks)
        {
            #region 计算折扣金额
            int cost1 = cost;// = (int)(cost * 0.9);
            #endregion

            WasherCardModel card = WasherCardBll.Instance.Get(cardId);
            if (card.Coins > 0)
            {
                WasherCardLogModel cl = new WasherCardLogModel()
                {
                    CardId = card.KeyId,
                    Memo = string.Format("{0}", ticks),
                    Time = DateTime.Now                   
                };

                if (card.Coins >= cost)
                {
                    cl.Coins = cost;

                    card.Coins -= cost;
                    cost = 0;
                }
                else {
                    cl.Coins = card.Coins;

                    cost -= card.Coins;
                    card.Coins = 0;
                }

                WasherCardBll.Instance.Update(card);
                WasherCardLogBll.Instance.Insert(cl);
            }

            if (cost > 0 && card.BinderId!=null)
            {
                var cards = WasherCardBll.Instance.GetValidCards(card.BinderId.Value).Where(a => a.Coins > 0).OrderBy(a => a.ValidateEnd);
                foreach (WasherCardModel c in cards)
                {
                    WasherCardLogModel cl = new WasherCardLogModel()
                    {
                        CardId = c.KeyId,
                        Memo = string.Format("{0}", ticks),
                        Time = DateTime.Now
                    };

                    if (c.Coins >= cost)
                    {
                        cl.Coins = cost;

                        c.Coins -= cost;
                        cost = 0;
                    }
                    else
                    {
                        cl.Coins = c.Coins;

                        c.Coins = 0;
                        cost -= c.Coins;
                    }

                    WasherCardLogBll.Instance.Insert(cl);
                    WasherCardBll.Instance.Update(c);

                    if (cost <= 0)
                    {
                        break;
                    }
                }
            }

            return cost1;
        }

        public bool Exits(string cardNo)
        {
            return WasherCardDal.Instance.GetWhere(new { CardNo = cardNo }).Count() > 0;
        }

        public string Lock(int deptId, int value)
        {
            WasherCardModel card = null; 
            var cards = WasherCardDal.Instance.GetWhere(new { DepartmentId = deptId, Coins = value, Kind = "Sale" }).Where(a => a.BinderId == null && a.ValidateEnd.CompareTo(DateTime.Now) > 0);
            foreach(WasherCardModel c in cards)
            {
                if (c.Locked== null)
                {
                    card = c;
                    break;
                }

                if ((DateTime.Now - c.Locked.Value).TotalMinutes >= 15)
                {
                    card = c;
                    break;
                }
            }

            if (card != null)
            {
                card.Locked = DateTime.Now;
                WasherCardDal.Instance.Update(card);
            }

            return card == null ? null : card.CardNo;
        }

        public bool Bind(WasherConsumeModel consume, int value)
        {
            var card = WasherCardDal.Instance.GetWhere(new { DepartmentId = consume.DepartmentId, Coins = value, Kind = "Sale" }).Where(a => a.BinderId == null && a.ValidateEnd.CompareTo(DateTime.Now) > 0 && a.Locked != null).OrderBy(a => a.Locked).FirstOrDefault();
            if(card==null)
            {
                return false;
            }

            card.Binded = DateTime.Now;
            card.BinderId = consume.KeyId;
            if (WasherCardBll.Instance.Update(card) > 0)
            {
                return true;
            }

            return false;
        }

        public DataTable Export(string filter, string sort = "KeyId", string order = "desc")
        {
            return WasherCardDal.Instance.Export(filter, sort, order);
        }

        public bool Bind(WasherConsumeModel consume, string cardNo)
        {
            var card = WasherCardDal.Instance.Get(consume.DepartmentId, cardNo);
            if (card != null)
            {
                card.Binded = DateTime.Now;
                card.BinderId = consume.KeyId;
                if (WasherCardBll.Instance.Update(card) > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public static string GetNextCouponCardNo(int deptId)
        {
            return string.Format("Coupon_{0}_{1:x}", deptId, DateTime.Now.Ticks);
        }

        public static string GetNextCouponCardNo(string prefix,int deptId, string openid)
        {
            return string.Format("{0}_{1}_{2:x}_{3}", prefix, deptId, DateTime.Now.Ticks, openid);
        }

        public int GetValidCoins(int keyId)
        {
            WasherCardModel card = WasherCardDal.Instance.Get(keyId);
            if (card == null)
            {
                return 0;
            }
            int coins = card.Coins;

            //查看正在使用的，被冻结的洗车币
            int lockedCoins;
            var ls = WasherDeviceLogDal.Instance.GetWhere(new { CardId = keyId }).Where(l => l.Ticks == null);
            if (ls.Count() == 0)
            {
                lockedCoins = 0;
            }
            else
            {
                lockedCoins = ls.Select(a => a.RemainCoins).Aggregate((t, a) => t + a);
            }
            
            coins -= lockedCoins;

            return coins;
        }

        //public bool InUsed(int keyId)
        //{
        //    return WasherDeviceLogDal.Instance.GetWhere(new { CardId = keyId }).Where(l => l.Ticks == null).Count() > 0;
        //}
    }
}
