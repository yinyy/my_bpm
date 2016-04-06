using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeChat.Utils
{
    public class WeChatAppConfiguration
    {
        internal string _AppId;
        internal string _AppSecret;
        internal string _Token;
        internal string _EncodingAESKey;

        public string AppId
        {
            get
            {
                return _AppId;
            }
        }

        public string AppSecret
        {
            get
            {
                return _AppSecret;
            }
        }

        public string Token
        {
            get
            {
                return _Token;
            }
        }

        public string EncodingAESKey
        {
            get
            {
                return _EncodingAESKey;
            }
        }
    }
}
