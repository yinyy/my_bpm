using BPM.Common.Data;
using BPM.Common.Provider;
using Course.Common.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Course.Common.Dal
{
    public class CommonSettingDal : BaseRepository<CommonSettingModel>
    {

        public static CommonSettingDal Instance
        {
            get { return SingletonProvider<CommonSettingDal>.Instance; }
        }
    }
}