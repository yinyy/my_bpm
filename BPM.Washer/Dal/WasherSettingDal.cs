using BPM.Common.Data;
using BPM.Common.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Washer.Model;

namespace Washer.Dal
{
    public class WasherSettingDal : BaseRepository<WasherSettingModel>
    {
        public static WasherSettingDal Instance
        {
            get { return SingletonProvider<WasherSettingDal>.Instance; }
        }
    }
}
