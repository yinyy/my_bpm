var actionUrl = '/PublicPlatform/Web/handler/CardHandler.ashx';
var vcode;

function hideAll() {
    $('div#card_list_region').hide();
    $('div#bind_card_region').hide();
    $('div#shopping_region').hide();
}

var Loading = {
    show: function () {
        $('div#loading_region').show();
    },
    hide: function () {
        $('div#loading_region').hide();
    }
};

var Button = {
    bind: function () {
        List.button();
        Bind.button();

    }
}

var List = {
    init: function(){
        $('div#button_region > a').each(function (index, obj) {
            if (index == 0) {
                $(obj).click(function () {
                    Buy.show();
                });
            } else if (index == 1) {
                $(obj).click(function () {
                    Bind.show();
                });
            }
        });
    },
    show: function () {
        hideAll();

        $('div#button_region > a').removeClass('weui_btn_disabled');

        $('div#card_list_region').show();

        Loading.show();
        $.getJSON(actionUrl, { action: 'list' }, function (json) {
            Loading.hide();

            if (json.count == 0) {
                $('div#no_cards_region').show();
            } else {
                $('div#show_cards_region > div.weui_panel_bd').empty();
                $(json.data).each(function (idx, obj) {
                    var s = '   <a href="javascript:void(0);" class="weui_media_box weui_media_appmsg">\
                                <div class="weui_media_hd">\
                                    <img class="weui_media_appmsg_thumb" src="./images/icon_card.png" alt="">\
                                </div>\
                                <div class="weui_media_bd">\
                                    <h4 class="weui_media_title">' + (obj.No.indexOf('Coupon') == 0 ? '优惠券' : obj.No) + '</h4>\
                                    <p class="weui_media_desc">洗车币：'+ (obj.Coins / 100.0).toFixed(2) + '<br/>有效期：' + formatDate(obj.ValidateFrom) + ' - ' + formatDate(obj.ValidateEnd) + '</p>\
                                </div>\
                            </a>';

                    $('div#show_cards_region > div.weui_panel_bd').append($(s));
                });

                $('div#show_cards_region').show();
            }

            $('div#button_region').show();
        });
    }
}

var Buy = {
    selected: null,
    init: function () {
        $.ajax({url: actionUrl, type: 'GET', async: 'false', data: {action: 'query'}, dataType: 'json', success: function(d){
            if (d.Success == true) {
                var ul;

                $(d.Data).each(function (i, v) {
                    if (i % 3 == 0) {
                        ul = $('<ul class="card_kind_list"></ul>');
                        $('div#shopping_region > div.card_kind_area').append(ul);
                    }

                    var o = $('<li><p class="value">' + v.Value + '元</p><p class="price">售价' + v.Price.toFixed(2) + '元</p><p class="remain">剩余' + v.Remain + '张</p></li>');
                    o.attr('Product', v.Product);
                    o.attr('Remain', v.Remain);
                    o.attr('Price', v.Price);
                    o.attr('Value', v.Value);
                    ul.append(o);

                    //购买洗车卡：不同金额
                    o.click(function () {
                        $('div#shopping_region > div.card_kind_area > ul.card_kind_list > li.selected').removeClass('selected');
                        $(this).addClass('selected');
                        Buy.selected = $(this);

                        if (parseInt($(this).attr('Remain')) <= 0) {
                            Buy.buyButton.disabled();
                        } else {
                            Buy.buyButton.enabled();
                        }

                        ////检查该面值洗车卡数量和实际价格
                        //Buy.selected.index = $('div#shopping_region > div.card_kind_area > ul.card_kind_list > li').index($(this));
                        //Buy.selected.value = v.title; //Buy.selected.index == 0 ? 50 : Buy.selected.index == 1 ? 100 : Buy.selected.index == 2 ? 200 : Buy.selected.index == 3 ? 300 : 0;
                        //$.getJSON(actionUrl, { action: 'query', value: Buy.selected.value }, function (json) {
                        //    if (json.Success == true) {
                        //        Buy.selected.remain = json.Remain;
                        //        Buy.selected.price = json.Price.toFixed(2);
                        //        Buy.selected.product = json.Product;

                        //        Buy.display();

                        //        if (json.Remain > 0) {
                        //            Buy.buyButton.enabled();
                        //        } else {
                        //            Buy.buyButton.disabled();
                        //        }
                        //    } else {
                        //        Buy.selected.index = -1;
                        //        Buy.selected.remain = 0;
                        //        Buy.selected.price = 0.00;
                        //        Buy.selected.product = '';

                        //        Buy.display();

                        //        alert('发生错误。');
                        //    }
                        //});
                    });
                });
            }
        }});
       
        //this.selected = {index: -1, remain: 0, price: 0.00, product: '', value:0};
        this.buyButton = {
            enabled: function () {
                $('div#shopping_region > div.weui_btn_area > a:first').removeClass('weui_btn_disabled');
            },
            disabled: function () {
                $('div#shopping_region > div.weui_btn_area > a:first').addClass('weui_btn_disabled');
            }
        };

        //购买洗车卡：购买和取消
        $('div#shopping_region > div.weui_btn_area > a:first').click(function () {

            if (parseInt(Buy.selected.attr('Remain')) > 0) {
                //锁定洗车卡
                var socket = new WebSocket('ws://139.129.43.203:5500');
                //var socket = new WebSocket('ws://127.0.0.1:5500');
                socket.onerror = function (event) {
                    alert("error:" + event);
                };
                socket.onopen = function (event) {
                    var o = { Action: 'lock_card', Data: JSON.stringify({ CID: cid, Value: parseInt(Buy.selected.attr('Value')) * 100 }) };
                    socket.send(JSON.stringify(o));

                    $('#loading_region').show();

                    socket.onmessage = function (event) {
                        var card = event.data;

                        if (card == '') {
                            $('#loading_region').hide();
                            alert('请重新打开网页购买洗车卡。');
                        } else {
                            Buy.buyButton.disabled();

                            Pay.prepay({ body: '购买洗车卡', pay: parseInt(parseFloat(Buy.selected.attr('Price')) * 100), attach: Buy.selected.attr('Product') },
                               function () {
                                   Buy.buyButton.enabled();

                                   $('#loading_region').hide();

                                   alert('支付成功。');

                                   $.post(actionUrl, { action: 'payBind2', value: Buy.selected.attr('Value'), card: card }, function (res) {
                                       if (res.Success == true) {
                                           List.show();
                                       }
                                       else {
                                           alert('绑定失败。');
                                       }
                                   }, 'json');

                                   //$.post(actionUrl, { action: 'payBind', value: Buy.selected.attr('Value') }, function (res) {
                                   //    if (res.Success == true) {
                                   //        List.show();
                                   //    }
                                   //    else {
                                   //        alert('绑定失败。');
                                   //    }
                                   //}, 'json');
                               },
                               function () {
                                   Buy.buyButton.enabled();
                                   $('#loading_region').hide();

                                   alert('支付失败。');
                               },
                               function () {
                                   $('#loading_region').hide();
                                   Buy.buyButton.enabled();
                               });
                        }
                    };
                };
            }
        });
        $('div#shopping_region > div.weui_btn_area > a:last').click(function () {
            $(this).click(function () {
                List.show();
            });
        });
    },

    //clear: function(){
    //    $('div#shopping_region > div.card_kind_area > ul.card_kind_list > li.selected').removeClass('selected');

    //    Buy.selected.index = -1;
    //    Buy.selected.remain = 0;
    //    Buy.selected.price = 0.00;
    //    Buy.selected.product = '';

    //    //this.display();
    //},

    show: function () {
        hideAll();

        $('div#shopping_region > div.weui_btn_area > a').removeClass('weui_btn_disabled');

        //this.clear();
        $('div#shopping_region').show();
        this.buyButton.disabled();
    }
};

var Bind = {
    init:function(){
        this.button = { ok: $('div#bind_card_region > div.weui_btn_area > a:first'), cancel: $('div#bind_card_region > div.weui_btn_area > a:last') };
        this.controller = { number: $('input#CardNo'), password: $('input#Password'), vcode: $('input#Vcode') };

        this.button.ok.click(function () {
            var cardNo = $.trim(Bind.controller.number.val());
            var password = $.trim(Bind.controller.password.val());
            var vc = $.trim(Bind.controller.vcode.val());

            if (cardNo == '') {
                alert('请输入卡号。');
                Bind.controller.number.focus();
                return;
            }

            if (password == '') {
                alert('请输入卡密码。');
                Bind.controller.password.focus();
                return;
            }

            //if (vc == '') {
            //    alert('请输入验证码。');
            //    Binder.controller.Vcode.focus();
            //    return;
            //}

            Loading.show();
            $.getJSON(actionUrl, { action: 'bind', card: cardNo, password: password, vcode: vc, telphone: $('input#Telphone').val() }, function (json) {
                Loading.hide();

                if (json.Success) {
                    alert('洗车卡绑定成功。');

                    List.show();
                } else {
                    alert(json.Message);
                }
            });
        });

        this.button.cancel.click(function () {
            List.show();
        });
    },

    clear: function(){
        this.controller.number.val('');
        this.controller.password.val('');
        this.controller.vcode.val('');
    },

    show: function () {
        hideAll();

        $('div#bind_card_region > div.weui_btn_area > a').removeClass('weui_btn_disabled');

        this.clear();
        $('div#bind_card_region').show();
    }
}


$(document).ready(function () {
    if (cid == null) {
        location.href = 'Profile.aspx';
    } else {
        vcode = new Vcode($('a#GetVcode'), $('input#Telphone'));
        $.getJSON(actionUrl, { action: 'telphone' }, function (json) {
            if (json.Binded) {
                $('input#Telphone').val(json.Message);

                List.init();
                Buy.init();
                Bind.init();

                List.show();
            }
        });
    }
});

function formatDate(d) {
    return d.substring(0, 4) + '年' + d.substring(5, 7) + '月' + d.substring(8, 10) + '日';
}