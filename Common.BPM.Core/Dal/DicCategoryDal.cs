using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPM.Core.Model;
using BPM.Common.Data;
using BPM.Common.Provider;

namespace BPM.Core.Dal
{
    public class DicCategoryDal : BaseRepository<DicCategory>
    {
        public static DicCategoryDal Instance
        {
            get { return SingletonProvider<DicCategoryDal>.Instance; }
        }

        
    }

}
