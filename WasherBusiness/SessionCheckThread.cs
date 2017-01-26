using SuperSocket.SocketBase;
using SuperSocket.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WasherBusiness
{
    public class SessionCheckThread
    {
        private List<BoardAppServer> appServers;
        private List<WebSocketServer> webServers;

        private bool isRunnng = false;

        public SessionCheckThread()
        {
            this.appServers = new List<BoardAppServer>();
            this.webServers = new List<WebSocketServer>();
        }

        public void Add(BoardAppServer server)
        {
            this.appServers.Add(server);
        }

        public void Add(WebSocketServer server)
        {
            this.webServers.Add(server);
        }

        public void Start()
        {
            isRunnng = true;

            new Thread(() =>
            {
                while (isRunnng)
                {
                    try
                    {
                        foreach (var svr in this.webServers)
                        {
                            foreach (var sn in svr.GetAllSessions())
                            {
                                if (!sn.Connected)
                                {
                                    sn.Close();
                                }
                            }
                        }

                        foreach (var svr in this.appServers)
                        {
                            foreach (var sn in svr.GetAllSessions())
                            {
                                if (!sn.Connected)
                                {
                                    sn.Close();
                                }
                            }
                        }
                    }
                    catch
                    {

                    }

                    Thread.Sleep(60 * 1000);
                }
            }).Start();
        }

        public void Stop()
        {
            this.isRunnng = false;
        }
    }
}
