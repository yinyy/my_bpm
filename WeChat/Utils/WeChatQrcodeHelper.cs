using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WeChat.Utils
{
    public class WeChatQrcodeHelper
    {
        public static string GetPermanenceCode(int id)
        {
            string ticket = "";

            string message = WeChatToolkit.SendCommand(string.Format("https://api.weixin.qq.com/cgi-bin/qrcode/create?access_token={0}", WeChatToolkit.AccessToken),
                string.Format("{{\"action_name\": \"QR_LIMIT_STR_SCENE\", \"action_info\": {{\"scene\": {{\"scene_str\": \"channel{0}\"}}}}}}", id));
            if (message.IndexOf("errcode") == -1)
            {
                //{"ticket":"gQH47joAAAAAAAAAASxodHRwOi8vd2VpeGluLnFxLmNvbS9xL2taZ2Z3TVRtNzJXV1Brb3ZhYmJJAAIEZ23sUwMEmm3sUw==","expire_seconds":60,"url":"http:\/\/weixin.qq.com\/q\/kZgfwMTm72WWPkovabbI"}
                var msg = new { ticket = "", url = "" };
                msg = JsonConvert.DeserializeAnonymousType(message, msg);

                ticket = msg.ticket;
            }

            return ticket;
        }
    }
}
