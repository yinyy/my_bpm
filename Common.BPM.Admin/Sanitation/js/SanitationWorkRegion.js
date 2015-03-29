var actionUrl = '/Sanitation/ashx/SanitationWorkRegionHandler.ashx';
var map;
var polygon;

$(function () {
    map = new BMap.Map('map_container');          // 创建地图实例
    map.addControl(new BMap.NavigationControl());
    map.centerAndZoom('东营市', 13);
    map.enableScrollWheelZoom(true);

    autoResize({ dataGrid: '#map_container', gridType: 'treegrid', callback: Map.bind, height: 0 });

    $('#a_save').click(Map.save);
});

var Map = {
    bind: function (winSize) {
        $('#map_container').panel({
            width: winSize.width,
            height: winSize.height,
            tools: [{
                iconCls: 'icon-add',
                handler: function () { alert('new') }
            }]
        });

        var wrp = [];
        for (var i = 0; i < workRegionPoints.length; i++) {
            wrp[wrp.length] = new BMap.Point(workRegionPoints[i].x, workRegionPoints[i].y);
        }

        polygon = new BMap.Polygon(wrp, { strokeColor: "blue", strokeWeight: 2, strokeOpacity: 0.5, fillColor: 'red', fillOpacity: 0.05 });  //创建多边形
        map.addOverlay(polygon);   //增加多边形
        polygon.enableEditing();
    },

    save: function () {
        var ps = polygon.getPath();
        var o = [];
        for (var i = 0; i < ps.length; i++) {
            o[o.length] = { x: ps[i].lng, y: ps[i].lat };
        }

        $.post(actionUrl, { action: 'save', path: JSON.stringify(o) }, function (data) {
            if (data == 'success') {
                msg.ok('工作区域保存成功。');
            } else {
                msg.waring('工作区域保存失败！');
            }
        }, 'text');
    }
};