using Senparc.Weixin.MP.CommonAPIs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WeChat.Utils;

namespace BPM.Admin.PublicPlatform
{
    public partial class Test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //string[] openids =
            //{
            //"oiVK2uH3zgJLC6iGMoB6iuDKDW1M",
            //"oiVK2uNX800Hv--bYeKIpz7_Uo4E",
            //"oiVK2uJF9u4F-_7jfU5OpB_7OY9Y",
            //"oiVK2uBw07c2SKdr2xrPIpiR0nOM",
            //"oiVK2uJ1jbjLpDgDC5QnJUU_PxUw",
            //"oiVK2uCUcO80LBEMK-aH7Xv1n3_s",
            //"oiVK2uMNfDkppU5vVRyr-0IYklIQ",
            //"oiVK2uFJBMOmyBK0t384AIzQuQAc",
            //"oiVK2uFa5WbRIT-jWQ1cr-D4_JpQ",
            //"oiVK2uEA0GvKVVsxjUDRSjSQ0CHs",
            //"oiVK2uNPvhPq5yZHqnBAR9NCHXkg",
            //"oiVK2uKUQQTxzBTCJtn_4SSHXzmY",
            //"oiVK2uNPE9RfsRIqsT_2_lnu8JTw",
            //"oiVK2uEagqJ95IIdtbf3XAZViqFg",
            //"oiVK2uDQuoVD3xQ9rE-R5gI6YwBk",
            //"oiVK2uBMLWqGlmeKrgk7MzXzvgM0",
            //"oiVK2uIFB7IneSemv6nWNbyYX63c",
            //"oiVK2uIttB2WiY0Jl6NFQG1WSMBs",
            //"oiVK2uFzQ2T5X27UjOtcbZXFs-IQ",
            //"oiVK2uByl5vaFddyjUjdHoOz96zM",
            //"oiVK2uB5qOz8bS_JBlkZxynsCTMk",
            //"oiVK2uOBNxqk0mN3zh_fYyNlk0wU",
            //"oiVK2uI2mtVrvSsZNqK0-3mflVKs",
            //"oiVK2uFvIOGcDQUBBj-f0ih6M9OQ",
            //"oiVK2uBwQciYFO8Td1JRPtFez01Q",
            //"oiVK2uP0xdYSKdOi2MdyoSuZ1gm8",
            //"oiVK2uAnKxDH5Az8VrPxNxZNkJLE",
            //"oiVK2uLKi826Mn8iVMsLcAGNuqFM",
            //"oiVK2uMwCeIhaS3AHiy_cWbUio4o",
            //"oiVK2uLKFrMuLzq02dnzWYGN0TI0",
            //"oiVK2uNKANmYCam_qUwlby8_EWuQ",
            //"oiVK2uHsU3wRfDKGZ9husJYH-K6g",
            //"oiVK2uKQ0QMavYzLnepotaMF73BQ",
            //"oiVK2uDn9YPfb3160EMP5kLyY7bs",
            //"oiVK2uDb5foRb-w3SGMFXnU-asBQ",
            //"oiVK2uDNR_MoejilIQ_qCpJRZdH0",
            //"oiVK2uLNF7RltKfwY80MvJNlmQ20",
            //"oiVK2uElzQF5a5YpaEZ1L4jN_Ev4",
            //"oiVK2uA-AqZs622DcqM5sYmfjeP4",
            //"oiVK2uNAXATzHLmOopD2Q_-fPPwA",
            //"oiVK2uFbcIh3zDYaCEvT8OjlgeHQ",
            //"oiVK2uKj1UK1vEpBIhBzz2mX57mo",
            //"oiVK2uNshx2w1zUvrh0B41YleN5c",
            //"oiVK2uG0HfskId-4WGWp5JrTuSss",
            //"oiVK2uFeYlBH2IcNqltuvcG4mrEw",
            //"oiVK2uKg57Lv9hLD1Vear2jY_Luc",
            //"oiVK2uDr_FINemcAasHp4tx1vkcY",
            //"oiVK2uMWt7O3BWfcGmKzgzgxEmwk",
            //"oiVK2uFlHquOSBMR4zzn62z4UKlY",
            //"oiVK2uIO4qvULzY2r9vUPIHP05vI",
            //"oiVK2uN36XwSulaHGtLrHl5Ead6I",
            //"oiVK2uD68xadRT3QBkQdPhuNlLXg",
            //"oiVK2uMamOYXbLfeU_MU36fTlHK0",
            //"oiVK2uDnnnr03Ok3lC96NWAshWu8",
            //"oiVK2uM88kiWdiIQXbW2W96_DLIs",
            //"oiVK2uCDUOpuYzWmUQmM-8JTderg",
            //"oiVK2uHVcY0lZUaL4ZH57YTa9-nY",
            //"oiVK2uIkNVmABxg2T89iSkuv2boc",
            //"oiVK2uOqHXhV1ckaMT3W7h7QaV7s",
            //"oiVK2uGwEk1gE_RTjgr0gVRwxgAs",
            //"oiVK2uBAtIQuVWJBl9h9gRF8lfb8",
            //"oiVK2uLlFXrrarOzj30VDOww_nNI",
            //"oiVK2uAyJn6VlhBel2_K81StEJOQ",
            //"oiVK2uP_WlfKXTzqyIF292IEHYSc",
            //"oiVK2uMyCJjjYPvpfi-Gp4I-P7Bk",
            //"oiVK2uJ4jXdWNZvHJqqVe2Nv9Jng",
            //"oiVK2uB9pOTLJhLFn6-DHfp1uMsU",
            //"oiVK2uMAl8EuuhoOt3R-7W5oLrmU",
            //"oiVK2uD3HAjZXb-D7jtUMAhZ5HMc",
            //"oiVK2uK3WQPZZuU2XfCYsaVyOlMI",
            //"oiVK2uDW_7-5BIqWjsnUuaWPgbuA",
            //"oiVK2uHHnb-YbgD3MxJksxkuVssE",
            //"oiVK2uJzX1ar2IsGkJ2Lh3xbUoAA",
            //"oiVK2uIPPFP0n01aLKXjwxKH8xYQ",
            //"oiVK2uG_H4qPTKKnfJiAn5ucRo3c",
            //"oiVK2uMs-pIT9GTyW7bi__7L1IPo",
            //"oiVK2uL4jfGwZjoI4TqVKIcHhj1w",
            //"oiVK2uEakkOiQzgmXM2cK5mHDCbk",
            //"oiVK2uOXZ09D_Zs-lD2DCsnjtF9o",
            //"oiVK2uClHr9l0TGpqQl5rtXAn9Bs",
            //"oiVK2uGRA67IJB97AUyQrogf9Ka0",
            //"oiVK2uEuo-PGJUNQWJbqkqu9Qk9c",
            //"oiVK2uKhgjvNtYLv1IAr9_jrFKsk",
            //"oiVK2uOUkpDpnrh9__aRsKJM8Utc",
            //"oiVK2uJxrfJWGihnM7SOP2M0VpEg",
            //"oiVK2uNBgwGvsX4F_Ry9YDdiEpls",
            //"oiVK2uBE1gKulkYJyMIpRwcerCF8",
            //"oiVK2uAZ7PckOVKl_qYtjLZBK47I",
            //"oiVK2uFu5UvJffwGvao82xP-QN5I",
            //"oiVK2uBOMU6LY6O3eWHqDNqm8FdU",
            //"oiVK2uJdlwrtY-3IieDPz27cAzWE",
            //"oiVK2uCDaf13fwPUgjQ6P8qf85p0",
            //"oiVK2uMc0vL3v4WaNuMFXPFSM4y0",
            //"oiVK2uIVSRICq39Afws97Gda59cI",
            //"oiVK2uJ85nhloWCUDIdq48UbI5FM",
            //"oiVK2uPI-Kn6PLK9OgvvFzYUqXUQ",
            //"oiVK2uFpm8UW4I4-jzQXC_Xrk3RA",
            //"oiVK2uE30SanNqj8iU9VlniRfvJE",
            //"oiVK2uFEPV2rkGARfJKCHB1weRI0",
            //"oiVK2uCZ1lLbi2Fhwp80NceYCGk4",
            //"oiVK2uJ_1maRbKtmYzp4ZzbApa2A",
            //"oiVK2uNjRKIgSToGes6M2USxTHpo",
            //"oiVK2uGWMjkSrOUNOcQ0ev_sE42Y",
            //"oiVK2uBIAlFYozfzB8T_OPQQeI-Y",
            //"oiVK2uKtD7CRPuHg7jLyU6FU-V2w",
            //"oiVK2uKchBJpbIkr8boYJcLtu6Qc",
            //"oiVK2uAQZZkH9CqiZ1LWBYUBzE0E",
            //"oiVK2uEm1SgCiAbZ_6IOvjJUhWc8",
            //"oiVK2uG1f0AEvaMI8QCMgNyLFMYk",
            //"oiVK2uIGo1dsju4QgpoPep0q_Pnk",
            //"oiVK2uF3OFVrWeGx4JyOJ1LNOvrA",
            //"oiVK2uLqm1kOXrgjkQrX8wYi5ILo",
            //"oiVK2uIS7kfb-YjoziJutcr2DMeI",
            //"oiVK2uNiTKUPExauCoYQKV_WvNvo",
            //"oiVK2uHmY53Pj3obn60dcCUMiApo",
            //"oiVK2uMVPImHB-T3xiCEXKinkI3w",
            //"oiVK2uL22DXNWglRI6V8xUKoMHho",
            //"oiVK2uKcFp1V2jHbyLMHuBc-emXc",
            //"oiVK2uNR7d-5Hm_vqig2oF74HmA4",
            //"oiVK2uOSRrtY1sZaRLwax71mFJZQ",
            //"oiVK2uPCb0arcl4UGEE028Twt4Uw"
            //};

            string accessToken = AccessTokenContainer.TryGetAccessToken("wx2d8bcab64b53be3a", "fca041d5cd06bb434a6aee37209d5ea7");


            //using (StreamWriter writer = new StreamWriter("D:/123.txtxt"))
            //{
            //    foreach (string id in openids)
            //    {
            //        var info = CommonApi.GetUserInfo(accessToken, id);
            //        writer.WriteLine(string.Format("{0}===={1}", id, info.nickname));
            //    }
            //}
        }

        protected void 上传素材_Click(object sender, EventArgs e)
        {

        }

        protected void 创建菜单_Click(object sender, EventArgs e)
        {
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/menu/create?access_token={0}", WeChatToolkit.AccessToken);
            string menu;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/PublicPlatform/data/menu.txt")))
            {
                menu = reader.ReadToEnd();
            }

            string message = WeChatToolkit.SendCommand(url, menu);
            返回信息.Text = message;
        }
    }
}