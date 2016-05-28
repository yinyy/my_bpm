using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BPM.BoardListener
{
    public class BoardClient
    {
        private Socket socket;

        public BoardClient(Socket socket)
        {
            this.socket = socket;
        }

        public void Start()
        {
            new Thread(new ThreadStart(()=>
            {
                //socket.rece
            })).Start();
        }
    }
}
