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

        public List<WasherCardModel> GetCards(string openId)
        {
            var cards = WasherCardDal.Instance.GetCards(openId);
                
            //    .Select(c => new WasherCardModel
            //{
            //    Coins = c.Coins,
            //    KeyId = c.KeyId,
            //    Picture = "/PublicPlatform/Web/images/default_card.png",
            //    Points = c.Points,
            //    Serial = c.Card,
            //    Logo = "/PublicPlatform/Web/images/default_logo.png",
            //    Name = "测试的品牌"
            //}).ToList();

            return cards;
        }

        public string GetIntroduction(int keyId)
        {
            return WasherCardDal.Instance.GetIntroduction(keyId);
        }
    }
}
