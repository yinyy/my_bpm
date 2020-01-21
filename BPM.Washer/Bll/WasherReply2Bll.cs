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
    public class WasherReply2Bll
    {
        public static WasherReply2Bll Instance
        {
            get { return SingletonProvider<WasherReply2Bll>.Instance; }
        }

        public string GetJson(int pageindex, int pagesize, string filterJson, string sort = "Keyid", string order = "asc")
        {
            return WasherReply2Dal.Instance.GetJson(pageindex, pagesize, filterJson, sort, order);
        }

        public int Add(WasherReply2Model model)
        {
            return WasherReply2Dal.Instance.Insert(model);
        }

        public int Update(WasherReply2Model model)
        {
            return WasherReply2Dal.Instance.Update(model);
        }

        public int Delete(int keyId)
        {
            return WasherReply2Dal.Instance.Delete(keyId);
        }

        public WasherReply2Model Get(int deptId, string eventKey)
        {
            return WasherReply2Dal.Instance.GetWhere(new
            {
                DepartmentId = deptId,
                MenuKey = eventKey
            }).FirstOrDefault();
        }
    }
}
