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

        public float Deduction(int cardId, float money)
        {
            #region 计算折扣金额
            float money1 = money = money * 0.9f;
            #endregion

            WasherCardModel card = WasherCardBll.Instance.Get(cardId);
            if (card.Coins > 0)
            {
                if (card.Coins >= money)
                {
                    money = 0;
                    card.Coins -= money;
                }
                else {
                    money -= card.Coins;
                    card.Coins = 0;
                }
                WasherCardBll.Instance.Update(card);
            }

            if (money > 0)
            {
                var cards = WasherConsumeBll.Instance.GetValidCards(card.BinderId.Value).Where(a=>a.Coins>0);
                foreach(WasherCardModel c in cards)
                {
                    if (c.Coins >= money)
                    {
                        c.Coins -= money;
                        money = 0;
                        WasherCardBll.Instance.Update(c);

                        break;
                    }
                    else
                    {
                        c.Coins = 0;
                        money -= c.Coins;
                        WasherCardBll.Instance.Update(c);
                    }
                }
            }

            return money1 - money;
        }
    }
}
