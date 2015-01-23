using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPM.Common;
using BPM.Common.Data;
using BPM.Common.Provider;
using BPM.Logistics.Dal;
using BPM.Logistics.model;

namespace BPM.Logistics.Bll
{
    public class LogisticsFreightGroupBll
    {
        public static LogisticsFreightGroupBll Instance
        {
            get { return SingletonProvider<LogisticsFreightGroupBll>.Instance; }
        }

        public int Add(LogisticsFreightGroupModel model)
        {
            return LogisticsFreightGroupDal.Instance.Insert(model);
        }

        public int Update(LogisticsFreightGroupModel model)
        {
            return LogisticsFreightGroupDal.Instance.Update(model);
        }

        public int Delete(int keyid)
        {
            return LogisticsFreightGroupDal.Instance.Delete(keyid);
        }

        public string GetJson(int pageindex, int pagesize, string filterJson, string sort = "Keyid", string order = "asc")
        {
            return LogisticsFreightGroupDal.Instance.GetJson(pageindex, pagesize, filterJson, sort, order);
        }

        public string GetFreightByDic(int dicId)
        {
            return JSONhelper.ToJson(LogisticsFreightGroupDal.Instance.GetWhere(new { DicId = dicId }).Select(o => o.DepartmentId).ToArray());
        }

        public bool DeleteGroup(int dicId)
        {
            return DbUtils.DeleteWhere<LogisticsFreightGroupModel>(new { DicId = dicId }) >= 0;
        }

        public List<LogisticsFreightGroupModel> GetListByDic(int dicId)
        {
            return LogisticsFreightGroupDal.Instance.GetWhere(new { DicId = dicId }).ToList();
        }
    }
}
