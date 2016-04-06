using BPM.Common.Data;
using BPM.Common.Provider;
using BPM.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPM.Core.Dal
{
   public  class DistrictDal : BaseRepository<District>
    {
        public static DistrictDal Instance
        {
            get { return SingletonProvider<DistrictDal>.Instance; }
        }
    }
}