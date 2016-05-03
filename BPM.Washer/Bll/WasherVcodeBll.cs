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
    public class WasherVcodeBll
    {
        public static WasherVcodeBll Instance
        {
            get { return SingletonProvider<WasherVcodeBll>.Instance; }
        }

        public WasherVcodeModel Get(string telphone)
        {
            return WasherVcodeDal.Instance.Get(telphone);
        }

        public int Save(WasherVcodeModel vcode)
        {
            return WasherVcodeDal.Instance.Insert(vcode);
        }

        public int Validate(string telphone, string vcode, int m)
        {
            WasherVcodeModel model = Get(telphone);
            if (model == null)
            {
                //还没有获取验证码
                return 1;
            }
            else if (model.Created.AddMinutes(m) <= DateTime.Now)
            {
                //验证码超时了
                return 2;
            }
            else if (vcode != model.Vcode)
            {
                return 3;
            }

            model.Validated = DateTime.Now;
            Update(model);

            return 0;
        }

        public int Update(WasherVcodeModel model)
        {
            return WasherVcodeDal.Instance.Update(model);
        }
    }
}
