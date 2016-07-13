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
    public enum WasherVcodeResult
    {
        验证码正确,
        请先获取验证码,
        验证码已过期,
        验证码错误
    }

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

        public WasherVcodeModel Create(string telphone)
        {
            WasherVcodeModel vcode = WasherVcodeBll.Instance.Get(telphone);
            if (vcode == null || vcode.Validated != null || vcode.Created.AddMinutes(3) <= DateTime.Now)
            {
                vcode = new WasherVcodeModel();
                vcode.Created = DateTime.Now;
                vcode.Memo = "";
                vcode.Validated = null;
                vcode.Telphone = telphone;
                vcode.Vcode = string.Format("{0:000000}", DateTime.Now.Ticks % 1000000);

                if (WasherVcodeBll.Instance.Save(vcode) > 0)
                {
                    return vcode;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return vcode;
            }
        }

        public int Save(WasherVcodeModel vcode)
        {
            return WasherVcodeDal.Instance.Insert(vcode);
        }

        public int Update(WasherVcodeModel model)
        {
            return WasherVcodeDal.Instance.Update(model);
        }
    }
}
