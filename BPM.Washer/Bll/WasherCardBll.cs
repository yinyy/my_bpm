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
            return WasherCardDal.Instance.GetWhere(new { DepartmentId = departmentId, Coins = value, Kind = "Sale"}).Where(a => a.BinderId == null && a.ValidateEnd.CompareTo(DateTime.Now) > 0).Count();
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
                if (card.Coins >= cost)
                {
                    card.Coins -= cost;
                    cost = 0;
                }
                else {
                    cost -= card.Coins;
                    card.Coins = 0;
                }
                //card.Memo = string.Format("{0}", ticks);
                WasherCardBll.Instance.Update(card);
            }

            if (cost > 0 && card.BinderId!=null)
            {
                var cards = WasherCardBll.Instance.GetValidCards(card.BinderId.Value).Where(a => a.Coins > 0).OrderBy(a => a.ValidateEnd);
                foreach (WasherCardModel c in cards)
                {
                    if (c.Coins >= cost)
                    {
                        c.Coins -= cost;
                        cost = 0;

                        c.Memo = string.Format("{0}", ticks);
                        WasherCardBll.Instance.Update(c);

                        break;
                    }
                    else
                    {
                        c.Coins = 0;
                        cost -= c.Coins;

                        c.Memo = string.Format("{0}", ticks);
                        WasherCardBll.Instance.Update(c);
                    }
                }
            }

            return cost1;
        }

        public bool Bind(WasherConsumeModel consume, int value)
        {
            WasherCardModel card = WasherCardDal.Instance.GetWhere(new { DepartmentId = consume.DepartmentId, Coins = value, Kind = "Sale"}).Where(a => a.BinderId == null && a.ValidateEnd.CompareTo(DateTime.Now) > 0).FirstOrDefault();
            if (card == null)
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

        public static string GetNextCouponCardNo(int deptId)
        {
            return string.Format("Coupon_{0}_{1:x}", deptId, DateTime.Now.Ticks);
        }

        public int GetValidCoins(int consumeId)
        {
            return GetValidCards(consumeId).Select(a => a.Coins).Sum();
        }
    }
}
