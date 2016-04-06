using BPM.Common.Data;
using BPM.Common.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Washer.Model;

namespace Washer.Dal
{
    public class WasherRewardDal : BaseRepository<WasherRewardModel>
    {
        public static WasherRewardDal Instance
        {
            get { return SingletonProvider<WasherRewardDal>.Instance; }
        }
    }
}
