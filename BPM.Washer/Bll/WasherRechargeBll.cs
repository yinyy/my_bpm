using BPM.Common.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Washer.Dal;

namespace Washer.Bll
{
    public class WasherRechargeBll
    {
        public static WasherRechargeBll Instance
        {
            get { return SingletonProvider<WasherRechargeBll>.Instance; }
        }

        public string GetRechargeJsonByCustomId(int customId)
        {
            return WasherRechargeDal.Instance.GetRechargeJsonByCustomId(customId);
        }
    }
}
