using BPM.Core.Bll;
using BPM.Core.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.CommonAPIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Washer.Bll;
using Washer.Model;
using WebSocket4Net;

namespace Washer.Toolkit
{
    public class LockCardInfo
    {
        public bool Finished { get; internal set; }
        public int ConsumeId { get; set; }
        public int CardValue { get; set; }

        public LockCardInfo(int consumeId, int cardValue)
        {
            this.ConsumeId = consumeId;
            this.CardValue = cardValue;
            this.Finished = false;
        }

        public string CardNumber { get; set; }
    }

    public class LoopThread
    {
        private static LoopThread CurrentLoopThread;

        private ManualResetEvent resetEvent;
        private List<LockCardInfo> lockCardList;

        private bool isRunning;

        private LoopThread()
        {
            isRunning = false;
            lockCardList = new List<LockCardInfo>();

            resetEvent = new ManualResetEvent(false);
        }

        public static void Start()
        {
            if (CurrentLoopThread == null)
            {
                CurrentLoopThread = new LoopThread();
                CurrentLoopThread.Execute();
            }
        }

        public void Execute() { 
            isRunning = true;

            new Thread(() =>
            {
                resetEvent.Reset();

                while (true)
                {
                    if (lockCardList.Count == 0)
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
                            lci.CardNumber = Aes.Encrypt(cardNo);
                        }
                        else
                        {
                            lci.CardNumber = "";
                        }

                        lockCardList.Remove(lci);
                        lci.Finished = true;
                    }                    
                }
            }).Start();
        }

        public static void Add(LockCardInfo lci)
        {
            CurrentLoopThread.lockCardList.Add(lci);
            CurrentLoopThread.resetEvent.Set();
        }

        public static void Stop()
        {
            CurrentLoopThread.isRunning = false;
            CurrentLoopThread.resetEvent.Set();
        }
    }
}