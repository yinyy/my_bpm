var Common = {
    getQueryString: function (name) {
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
        var r = location.search.substr(1).match(reg);
        if (r != null) {
            return unescape(decodeURI(r[2]));
        }

        return null;
    },

    createNextUrl: function () {
        var url = location.pathname;
        url = url.substr(0, url.lastIndexOf('/'));

        var s = location.search.substr(1);
        var reg = new RegExp("(^|&)code=([^&]*)(&|$)", "i");
        var r = s.match(reg);
        if (r != null) {
            if (r[0].indexOf('&') == 0) {
                s = s.replace(r[0], '&');
            } else {
                s = s.replace(r[0], '');
            }
        }

        reg = new RegExp("(^|&)appid=([^&]*)(&|$)", "i");
        r = s.match(reg);
        if (r != null) {
            if (r[0].indexOf('&') == 0) {
                s = s.replace(r[0], '&');
            } else {
                s = s.replace(r[0], '');
            }
        }

        reg = new RegExp("(^|&)next=([^&]*)(&|$)", "i");
        r = s.match(reg);
        if (r != null) {
            url += '/' + r[2] + '?';

            if (r[0].indexOf('&') == 0) {
                s = s.replace(r[0], '&');
            } else {
                s = s.replace(r[0], '');
            }
        }

        s = $.trim(s);
        if (s != '') {
            if (s.indexOf('&') == 0) {
                url += s.substr(1);
            } else {
                url += s;
            }
        }

        return url;
    }
}