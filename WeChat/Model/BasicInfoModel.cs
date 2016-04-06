using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WeChat.Model
{
    public class BasicInfoModel
    {
        [JsonProperty(PropertyName ="subscribe")]
        public int Subscribe { get; set; }

        [JsonProperty(PropertyName ="openid")]
        public string OpenId { get; set; }

        [JsonProperty(PropertyName ="nickname")]
        public string NickName { get; set; }

        [JsonProperty(PropertyName ="sex")]
        public int Gender { get; set; }

        [JsonProperty(PropertyName ="language")]
        public string Language { get; set; }

        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }

        [JsonProperty(PropertyName ="province")]
        public string Province { get; set; }
        
        [JsonProperty(PropertyName ="country")]
        public string Country { get; set; }

        [JsonProperty(PropertyName ="headimgurl")]
        public string HeadImageUrl { get; set; }

        [JsonProperty(PropertyName ="subscribe_time")]
        public int SubscribeTime { get; set; }

        [JsonProperty(PropertyName ="unionid")]
        public string UnionId { get; set; }

        [JsonProperty(PropertyName ="remark")]
        public string Remark { get; set; }

        [JsonProperty(PropertyName ="groupid")]
        public int GroupId { get; set; }
    }
}
