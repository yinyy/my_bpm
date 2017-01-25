using Newtonsoft.Json;
using SuperSocket.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Washer.Bll;
using Washer.Model;

namespace WasherBusiness
{
    public class LockCardInfo
    {
        public WebSocketSession Session { get; set; }
        public int ConsumeId { get; set; }
        public int CardValue { get; set; }

        public LockCardInfo(WebSocketSession session, int consumeId, int cardValue)
        {
            this.Session = session;
            this.ConsumeId = consumeId;
            this.CardValue = cardValue;
        }
    }

    public class ValidatePayInfo
    {
        public WebSocketSession Session { get; set; }
        public string Serial { get; set; }

        public int CheckedCount { get; set; }

        public DateTime LastChecked { get; set; }

        public ValidatePayInfo(WebSocketSession session, string serial)
        {
            this.Session = session;
            this.Serial = serial;

            CheckedCount = 0;
            LastChecked = DateTime.Now.AddMinutes(-1);
        }
    }

    public class LoopThread
    {
        private ManualResetEvent resetEvent;
        private List<LockCardInfo> lockCardList;
        private List<ValidatePayInfo> validatePayList;


        private bool isRunning;

        public LoopThread()
        {
            isRunning = false;
            lockCardList = new List<LockCardInfo>();
            validatePayList = new List<ValidatePayInfo>();

            resetEvent = new ManualResetEvent(false);
        }

        public void Start()
        {
            isRunning = true;

            new Thread(() =>
            {
                resetEvent.Reset();

                while (true)
                {
                    if (lockCardList.Count == 0 && validatePayList.Count == 0)
                    {
                        if (!isRunning)
                        {
                            break;
                        }

                        resetEvent.Reset();
                        resetEvent.WaitOne();
                    }

                    LockCardInfo lci = lockCardList.FirstOrDefault();
                    if (lci != null)
                    {
                        WasherConsumeModel consume = WasherConsumeBll.Instance.Get(lci.ConsumeId);
                        string cardNo = WasherCardBll.Instance.Lock(consume.DepartmentId, lci.CardValue);
                        if (!string.IsNullOrEmpty(cardNo))
                        {
                            lci.Session.Send(cardNo);
                        }
                        else
                        {
                            lci.Session.Send("");
                        }

                        lockCardList.Remove(lci);
                    }

                    ValidatePayInfo vpi = validatePayList.Where(a => a.LastChecked.AddSeconds(2).CompareTo(DateTime.Now) < 0).FirstOrDefault();
                    if (vpi != null)
                    {
                        if (vpi.CheckedCount > 15)
                        {
                            //超过30秒还没有检测到付款信息
                            vpi.Session.Send("pay_overtime");
                            validatePayList.Remove(vpi);
                        }
                        else
                        {
                            vpi.CheckedCount++;
                            vpi.LastChecked = DateTime.Now;

                            //检查付款信息
                            WasherOrderModel order = WasherOrderBll.Instance.Get(vpi.Serial);
                            if (order != null && order.Status == "已支付")
                            {
                                vpi.Session.Send("pay_success");
                                validatePayList.Remove(vpi);
                            }
                        }
                    }
                }
            }).Start();
        }

        public void Add(LockCardInfo lci)
        {
            lockCardList.Add(lci);
            resetEvent.Set();
        }

        public void Add(ValidatePayInfo vpi)
        {
            validatePayList.Add(vpi);
            resetEvent.Set();
        }

        public void Stop()
        {
            isRunning = false;
            resetEvent.Set();
        }
    }
}