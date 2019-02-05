using BPM.Common.Provider;
using Exam.Core.Dal;
using Exam.Core.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exam.Core.Bll
{
    public class ExamExamSectionItemBll
    {
        public static ExamExamSectionItemBll Instance
        {
            get { return SingletonProvider<ExamExamSectionItemBll>.Instance; }
        }

        public int Insert(ExamExamSectionItemModel model)
        {
            return ExamExamSectionItemDal.Instance.Insert(model);
        }

        public string GetJson(int pageindex, int pagesize, string filterJson, string sort = "KeyId", string order = "asc")
        {
            return ExamExamSectionItemDal.Instance.GetJson(pageindex, pagesize, filterJson, sort, order);
        }

        public int Delete(int keyId)
        {
            return ExamExamSectionItemDal.Instance.Delete(keyId);
        }

        public int Update(ExamExamSectionItemModel model)
        {
            return ExamExamSectionItemDal.Instance.Update(model);
        }

        public int CountRequired(int examSectionId)
        {
            return ExamExamSectionItemDal.Instance.GetWhere(new { ExamSectionId = examSectionId }).Select(m => m.TeacherCount).Sum();
        }

        public IEnumerable<ExamExamSectionItemModel> GetList(ExamExamSectionModel ees)
        {
            return ExamExamSectionItemDal.Instance.GetWhere(new { ExamSectionId = ees.KeyId });
        }
    }
}
