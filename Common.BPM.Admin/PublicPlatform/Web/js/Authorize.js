var Authorize = {
    authorize: function () {
        var access_code = Common.getQueryString('code');
        if (access_code == null) {
            var appid = Common.getQueryString('appid');
            var fromurl = location.href;//获取授权code的回调地址，获取到code，直接返回到当前页  
            var url = 'https://open.weixin.qq.com/connect/oauth2/authorize?appid=' + appid + '&redirect_uri=' + encodeURIComponent(fromurl) + '&response_type=code&scope=snsapi_base&state=0#wechat_redirect';
            location.href = url;

            return null;
        } else {
            return eval('(' + $.ajax('/PublicPlatform/Web/handler/AuthorizeHandler.ashx',
                {
                    async: false,
                    data: { code: access_code, appid: Common.getQueryString('appid') },
                    dataType: 'json'
                }).responseText + ')');
        }
    }
}