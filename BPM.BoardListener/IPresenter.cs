using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPM.BoardListener
{
    public interface IPresenter
    {
        void PrintDebug(string message, bool saveLog);
        void AddDevice(string serial, string board, string ip, string department, string address);
        void RemoveDevice(string ip);
    }
}
