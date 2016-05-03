using BPM.Common.Data;
using BPM.Common.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Washer.Model;

namespace Washer.Dal
{
    public class WasherCardDal : BaseRepository<WasherCardModel>
    {
        public static WasherCardDal Instance
        {
            get { return SingletonProvider<WasherCardDal>.Instance; }
        }

        public List<WasherCardModel> GetCards(string openId)
        {
            var list = DbUtils.GetList<WasherCardModel>(string.Format("select * from V_Consumes where OpenId = '{0}'", openId), null).ToList();
            
            return list;
        }
    }
}
