var actionUrl = '/PublicPlatform/Web/handler/RecordHandler.ashx';
var pageindex = 1;
var pagesize = 10;

$(document).ready(function () {
    $('a#previous').click(function () {
        var css = $(this).attr('class');
        if (css.indexOf('weui_btn_disabled') != -1) {
            return;
        }

        pageindex--;
        fillRecords();
    });

    $('a#next').click(function () {
        var css = $(this).attr('class');
        if (css.indexOf('weui_btn_disabled') != -1) {
            return;
        }

        pageindex++;
        fillRecords();
    });

    fillRecords();
});

function fillRecords() {
    $.post(actionUrl + '?page=' + pageindex + '&rows=' + pagesize, function (res) {
        if (res.total <= 0) {
            $('div#no_record').show();
            $('div#record_list').hide();
        } else {
            var container = $('div#record_list > div:eq(1)');
            container.html('');

            $(res.rows).each(function (i, r) {
                var o = $('<a class="weui_cell" href="javascript:;"><div class="weui_cell_bd weui_cell_primary"><p class="omit_text">' + r.Address + '</p><p>' + r.Started + '</p></div><div>￥' + r.PayCoins + '</div></a>');
                container.append(o);
            });

            var pages = Math.floor(res.total / pagesize) + (res.total % pagesize == 0 ? 0 : 1);
            if (pageindex <= 1) {
                $('a#previous').addClass('weui_btn_disabled');
            } else {
                $('a#previous').removeClass('weui_btn_disabled');
            }
            if (pageindex >= pages) {
                $('a#next').addClass('weui_btn_disabled');
            } else {
                $('a#next').removeClass('weui_btn_disabled');
            }

            $('div#record_list').show();
            $('div#no_record').hide();
        }
    }, 'json');
}