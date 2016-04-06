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
    public class WasherOrderDal : BaseRepository<WasherOrderModel>
    {
        public static WasherOrderDal Instance
        {
            get { return SingletonProvider<WasherOrderDal>.Instance; }
        }
    }
}
