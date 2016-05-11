using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPM.BoardListenerLib
{
    public abstract class ReplyMessageBase : MessageBase
    {
        protected byte[] buffer;

        public abstract byte[] ToByteArray();

        protected void InitBuffer(bool withDeviceId = true)
        {
            buffer = new byte[Length + 7];

            long ticks = DateTime.Now.Ticks;

            buffer[0] = (byte)((ticks >> 24) & 0xff);
            buffer[1] = (byte)(((ticks >> 16) & 0xff));
            buffer[2] = (byte)(((ticks >> 8) & 0xff));
            buffer[3] = (byte)(ticks & 0xff);

            buffer[4] = (byte)(((int)Command >> 8) & 0xff);
            buffer[5] = (byte)((int)Command & 0xff);

            buffer[6] = (byte)Length;

            if (withDeviceId)
            {
                long deviceID = Convert.ToInt32(BoardNumber);

                buffer[7] = (byte)((deviceID >> 24) & 0xff);
                buffer[8] = (byte)((deviceID >> 16) & 0xff);
                buffer[9] = (byte)((deviceID >> 8) & 0xff);
                buffer[10] = (byte)(deviceID & 0xff);
            }
        }
    }

    public class ReplyTimeSyncMessage : ReplyMessageBase
    {
        public override byte[] ToByteArray()
        {
            Length = 6;
            InitBuffer(false);

            DateTime now = DateTime.Now;
            buffer[7] = (byte)(now.Year - 2000);
            buffer[8] = (byte)now.Month;
            buffer[9] = (byte)now.Day;
            buffer[10] = (byte)now.Hour;
            buffer[11] = (byte)now.Minute;
            buffer[12] = (byte)now.Second;

            return buffer;
        }
    }

    public class ReplyReadSettingMessage : ReplyMessageBase
    {
        public int ErrorCode = 0;
        public int[] Values = new int[32];

        public override byte[] ToByteArray()
        {
            if (ErrorCode == 0)
            {
                Length = 68;

                InitBuffer();

                for (int i = 0; i < Values.Length; i++)
                {
                    buffer[i * 2 + 11] = (byte)((Values[i] >> 8) & 0xff);
                    buffer[i * 2 + 11 + 1] = (byte)(Values[i] & 0xff);
                }
            }
            else
            {
                Length = 1;

                InitBuffer(false);

                buffer[7] = (byte)ErrorCode;
            }

            return buffer;
        }
    }

    public class ReplyUploadStatusMessage : ReplyMessageBase
    {
        public int Status;

        public override byte[] ToByteArray()
        {
            Length = 1;
            InitBuffer(false);

            buffer[7] = (byte)Status;

            return buffer;
        }
    }

    public class ReplyAccountMessage : ReplyMessageBase
    {
        public int CardId;
        public int Payment;

        public override byte[] ToByteArray()
        {
            Length = 12;
            InitBuffer();

            buffer[11] = (byte)((CardId >> 24) & 0xff);
            buffer[12] = (byte)((CardId >> 16) & 0xff);
            buffer[13] = (byte)((CardId >> 8) & 0xff);
            buffer[14] = (byte)(CardId & 0xff);

            buffer[15] = (byte)((Payment >> 24) & 0xff);
            buffer[16] = (byte)((Payment >> 16) & 0xff);
            buffer[17] = (byte)((Payment >> 8) & 0xff);
            buffer[18] = (byte)(Payment & 0xff);

            return buffer;
        }
    }

    public class ReplyValidateMessage : ReplyMessageBase
    {
        public int CardId;
        public CardKind Kind;
        public CardStatus Status;
        public int Money;

        public override byte[] ToByteArray()
        {
            Command = CommandType.ValidateCardAndPassword;//不接受其它的返回值

            if (Status == CardStatus.Regular)
            {
                Length = 14;
                InitBuffer();

                buffer[11] = (byte)((CardId >> 24) & 0xff);
                buffer[12] = (byte)((CardId >> 16) & 0xff);
                buffer[13] = (byte)((CardId >> 8) & 0xff);
                buffer[14] = (byte)(CardId & 0xff);

                buffer[15] = (byte)Kind;
                buffer[16] = (byte)Status;

                buffer[17] = (byte)((Money >> 24) & 0xff);
                buffer[18] = (byte)((Money >> 16) & 0xff);
                buffer[19] = (byte)((Money >> 8) & 0xff);
                buffer[20] = (byte)(Money & 0xff);
            }
            else
            {
                Length = 10;
                InitBuffer();

                buffer[11] = (byte)((CardId >> 24) & 0xff);
                buffer[12] = (byte)((CardId >> 16) & 0xff);
                buffer[13] = (byte)((CardId >> 8) & 0xff);
                buffer[14] = (byte)(CardId & 0xff);

                buffer[15] = (byte)Kind;
                buffer[16] = (byte)Status;
            }

            return buffer;
        }
    }

    public class ReplyHeartBeatMessage : ReplyMessageBase
    {
        public override byte[] ToByteArray()
        {
            buffer = new byte[1];
            buffer[0] = 0;

            return buffer;
        }
    }

    public class ReplyOperationMessage : ReplyMessageBase
    {
        public int Status;

        public override byte[] ToByteArray()
        {
            Length = 5;
            InitBuffer();

            buffer[11] = (byte)Status;

            return buffer;
        }
    }
}
