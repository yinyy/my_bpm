using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPM.Common.Data;
using BPM.Core.Model;
using BPM.Common.Provider;

namespace BPM.Core.Dal
{
    public class DepartmentDal:BaseRepository<Department>
    {
        public static DepartmentDal Instance
        {
            get { return SingletonProvider<DepartmentDal>.Instance; }
        }

        public IEnumerable<Department> GetChildren(int parentid=0)
        {
            return GetAll().Where(d => d.ParentId == parentid);
        }
    }
}
