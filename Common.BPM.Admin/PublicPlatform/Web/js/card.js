$(document).ready(function () {
    $('div#machine_network, div#consume_record, div#promote').hide();

    $('div.weui_tabbar > a').click(function () {
        var idx = $('div.weui_tabbar > a').index($(this));

        var ds = $('div.weui_tab_bd > div');
        var d = ds.get(idx);

        $(ds).hide();
        $(d).show();

        $('div.weui_tabbar > a.weui_bar_item_on').removeClass('weui_bar_item_on');
        $(this).addClass('weui_bar_item_on');
    });

    $('ul.amount_charge > li').click(function () {
        $('ul.amount_charge > li.selected').removeClass('selected');

        $(this).addClass('selected');
        $('a#charge_button').removeClass('weui_btn_disabled');
    });
});