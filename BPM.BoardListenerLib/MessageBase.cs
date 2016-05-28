using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPM.BoardListenerLib
{
    public class MessageBase
    {
        public enum CommandType
        {
            ValidateCardAndPassword = 101,
            ValidateCard = 102,
            ValidatePhoneAndPassword = 103,
            TimeSync = 201,
            ReaderSetting = 202,
            UploadStatus = 203,
            Account = 205,
            HeartBeat = 1001,
            Operation = 2001
        }

        public enum OperationType
        {
            Start = 1,
            Stop = 2
        }

        public enum CardKind
        {
            Normal = 0x01,
            Super = 0x02
        }

        public enum CardStatus
        {
            Regular = 0x01,
            Unusual = 0x02
        }

        public int Ticks;
        public CommandType Command;
        public int Length;
        public string BoardNumber;
    }
}
