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
    }
}
