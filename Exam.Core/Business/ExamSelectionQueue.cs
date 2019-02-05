using BPM.Common.Provider;
using Course.Common.Bll;
using Course.Common.Model;
using Exam.Core.Bll;
using Exam.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exam.Core.Business
{
    public class ExamSelectionQueue
    {
        public class ExamStaffData
        {
            public int ExamSectionId { get; set; }
            public int StaffId { get; set; }
            public Action<int> ResultAction { get; set; }
            public bool Select { get; set; }
        }

        private static readonly object SyncObject = new object();
        private Queue<ExamStaffData> dataQueue = null;


        public static ExamSelectionQueue Instance
        {
            get { return SingletonProvider<ExamSelectionQueue>.Instance; }
        }

        public void AddTask(int examSectionId, int staffId, bool select, Action<int> action)
        {
            ExamStaffData data = new ExamStaffData();
            data.ExamSectionId = examSectionId;
            data.StaffId = staffId;
            data.ResultAction = action;
            data.Select = select;

            lock (SyncObject)
            {
                if(dataQueue == null)
                {
                    dataQueue = new Queue<ExamStaffData>();
                }
                dataQueue.Enqueue(data);
            }
        }

        public void Start()
        {
            lock (SyncObject)
            {
                while(dataQueue!=null && dataQueue.Count() > 0)
                {
                    ExamStaffData data = dataQueue.Dequeue();
                    int res = 0;

                    if (data.Select)
                    {
                        if (ExamExamSectionStaffBll.Instance.HasSelected(data.ExamSectionId, data.StaffId))
                        {
                            //记录是否已经存在
                            res = 100;//0表示已经选择，本次无需选择
                        }
                        else
                        {
                            ExamExamSectionModel ees = ExamExamSectionBll.Instance.Get(data.ExamSectionId);
                            CommonStaffModel staff = CommonStaffBll.Instance.Get(data.StaffId);

                            //预约的其它场次的监考时间是否与当前场次的监考时间有冲突
                            var eesvs = ExamExamSectionViewBll.Instance.GetList(staff);
                            eesvs = eesvs.Where(m => m.ExamSectionId != ees.KeyId).OrderBy(m=>m.Started);

                            bool conflict = false;
                            foreach(var eesv in eesvs)
                            {
                                //考试时间有交错
                                if ((eesv.Started >= ees.Started && eesv.Ended < ees.Ended) || (eesv.Ended >= ees.Started && eesv.Ended <= ees.Ended)
                                    || (ees.Started >= eesv.Started && ees.Started <= eesv.Ended) || (ees.Ended >= eesv.Started && ees.Ended <= eesv.Ended))
                                {
                                    conflict = true;
                                    break;
                                }
                            }

                            if (conflict)
                            {
                                res = 102;//表示本次预约的场次与其它预约的场次有冲突
                            }
                            else
                            {
                                //是否已经在该时间段安排了监考任务
                                var esidvs = ExamStaffInvigilateDetailViewBll.Instance.GetList(staff);
                                //esidvs = esidvs.Where(m => m.ExamSectionId != ees.KeyId);
                                foreach(var esidv in esidvs)
                                {
                                    //考试时间有交错
                                    if ((esidv.Started >= ees.Started && esidv.Ended < ees.Ended) || (esidv.Ended >= ees.Started && esidv.Ended <= ees.Ended)
                                        || (ees.Started >= esidv.Started && ees.Started <= esidv.Ended) || (ees.Ended >= esidv.Started && ees.Ended <= esidv.Ended))
                                    {
                                        conflict = true;
                                        break;
                                    }
                                }

                                if (conflict)
                                {
                                    res = 103;//表示本次预约的场次与其它已经安排的监考冲突
                                }
                                else
                                {
                                    //本场考试需要的监考人数
                                    int requires = ExamExamSectionItemBll.Instance.CountRequired(data.ExamSectionId);
                                    //本场考试已经报名的监考人数
                                    int selected = ExamExamSectionStaffBll.Instance.CountSelected(data.ExamSectionId);
                                    if (selected < requires)
                                    {
                                        ExamExamSectionStaffModel model = new ExamExamSectionStaffModel();
                                        model.Created = DateTime.Now;
                                        model.ExamSectionId = data.ExamSectionId;
                                        model.StaffId = staff.KeyId;

                                        ExamExamSectionStaffBll.Instance.Insert(model);

                                        res = 1;//表示成功选择本场次监考
                                    }
                                    else
                                    {
                                        res = 101;//101表示本场监考人数已满
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        ExamExamSectionStaffBll.Instance.Delete(data.ExamSectionId, data.StaffId);
                        res = 2;//表示成功取消本场次监考
                    }

                    data.ResultAction(res);
                }
            }
        }
    }
}
