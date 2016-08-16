var actionUrl = '/PublicPlatform/Web/handler/RecordHandler.ashx';
var pageindex = 1;
var pagesize = 10;

$(document).ready(function () {
    $('div.weui_panel > a.weui_panel_ft').click(function () {
        pageindex++;
        fillRecords();
    });

    fillRecords();
});

function fillRecords() {
    $.post(actionUrl + '?page=' + pageindex + '&rows=' + pagesize, function (res) {
        if (res.total <= 0) {
            var d = new Date();
            var y = d.getFullYear();
            var m = d.getMonth() + 1;
            if (m < 10) {
                m = '0' + m;
            }
            var date = d.getDate();
            if (date < 10) {
                date = '0' + date;
            }
            var h = d.getHours();
            if (h < 10) {
                h = '0' + h;
            }
            var mm = d.getMinutes();
            if (mm < 10) {
                mm = '0' + mm;
            }
            var s = d.getSeconds();
            if (s < 10) {
                s = '0' + s;
            }

            var o = $('<div class="weui_media_box weui_media_text"><h4 class="weui_media_title">没有找到洗车记录。快去给爱车洗个澡吧！</h4><p class="weui_media_desc">想知道哪里有自助洗车机吗？微信公众号下“我要”-“查看网点”即可找到。</p><ul class="weui_media_info"><li class="weui_media_info_meta">更新时间：' + (y + '年' + m + '月' + date + '日 ' + h + ':' + mm + ':' + s) + '</li></ul></div>');
            $('div.weui_panel > div.weui_panel_bd').append(o);

            $('div.weui_panel > a.weui_panel_ft').hide();
        } else {
            $(res.rows).each(function (i, r) {
                var o = $('<div class="weui_media_box weui_media_text"><h4 class="weui_media_title">'+r.Address + '</h4><ul class="weui_media_info"><li class="weui_media_info_meta">洗车时间：'+r.Started + '</li><li class="weui_media_info_meta weui_media_info_meta_extra">消费洗车币：'+r.PayCoins.toFixed(2) +'</li></ul></div>');
                $('div.weui_panel > div.weui_panel_bd').append(o);
            });

            var displayed = $('div.weui_panel > div.weui_panel_bd > div').size();
            if (displayed >= res.total) {
                $('div.weui_panel > a.weui_panel_ft').hide();
            }
        }
    }, 'json');
}