var Authorize = {
    authorize: function () {
        var access_code = Common.getQueryString('code');
        var appid = Common.getQueryString('appid');
        if (appid == null) {
            location.href = '/PublicPlatform/Web/Error.html?c=1';

            return null;
        }

        if (access_code == null) {
            var fromurl = location.href;//获取授权code的回调地址，获取到code，直接返回到当前页  
            var url = 'https://open.weixin.qq.com/connect/oauth2/authorize?appid=' + appid + '&redirect_uri=' + encodeURIComponent(fromurl) + '&response_type=code&scope=snsapi_base&state=0#wechat_redirect';
            location.href = url;

            return null;
        } else {
            var o = eval('(' + $.ajax('/PublicPlatform/Web/handler/AuthorizeHandler.ashx',
                {
                    async: false,
                    data: { code: access_code, appid: appid },
                    dataType: 'json'
                }).responseText + ')');

            if (o.Success == true) {
                location.href = Common.createNextUrl();

                return true;
            } else {
                return null;
            }
        }
    },
    test: function () {
        $.ajax('/PublicPlatform/Web/handler/AuthorizeTestHandler.ashx',
                {
                    async: false,
                    dataType: 'json'
                });
        location.href = Common.createNextUrl();
    }
}