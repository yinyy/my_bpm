var actionUrl = '/Agriculture/ashx/AgricultureMapHandler.ashx';
var map, infoWindow, socket;

function createWebSocket() {
    // 创建一个Socket实例
    socket = new WebSocket('ws://127.0.0.1:8181');
    
    // 打开Socket 
    socket.onopen = function (event) {
        // 发送一个初始化消息
        //socket.send('I am the client and I\'m listening!');

        // 监听消息
        socket.onmessage = function (event) {
            var json = eval('(' + event.data + ')');
            
            if (json.Kind == '02') {
                //多路开关
                var values = eval('([' + json.Status + '])');
                var markers = map.getOverlays();
                for (var i = 0; i < markers.length; i++) {
                    if (markers[i].mac == json.Mac) {
                        var icon= new BMap.Icon((values[0] == 1)?"./images/map/on.png":"./images/map/off.png", new BMap.Size(16, 32));
                        markers[i].setIcon(icon);
                    }
                }
            }
            if (json.Kind == '03') {
                //温湿度传感器
                $("[mac='" + json.Mac + "'][kind='t']").html(json.Temperature);
                $("[mac='" + json.Mac + "'][kind='h']").html(json.Humidity);
            } else if (json.Kind == '04') {
                //土壤湿度
                $("[mac='" + json.Mac + "']").html(json.Humidity);
            }
            else if (json.Kind == '05') {
                //颗粒物
                $("[mac='" + json.Mac + "']").html(json.Value);
            }
            else if (json.Kind == '06') {
                //多路pwm
                var values = eval('([' + json.Values + '])');

                $("[mac='" + json.Mac + "'][way='1']").val(values[0]);
            }

            //$("[mac='" + json.mac + "']").text(json.value);
            //$("[mac='" + json.mac + "']").html(json.value);
            //$("[mac='" + json.mac + "']").val(json.value);
        };

        // 监听Socket的关闭
        socket.onclose = function (event) {
            alert('请注意，与服务器断开连接。');
        };

        // 关闭Socket.... 
        //socket.close() 
    };
}

$(function () {
    createWebSocket();

    map = new BMap.Map("container");          // 创建地图实例  
    var point = new BMap.Point(118.629458, 37.442247);  // 创建点坐标  
    map.centerAndZoom(point, 18);                 // 初始化地图，设置中心点坐标和地图级别

    map.addControl(new BMap.NavigationControl());
    map.addControl(new BMap.ScaleControl());

    //加载设备
    $.getJSON(actionUrl, function (json) {
        $(json).each(function (i, d) {
            var pos = d.Coordinate;
            var title = d.Title;
            var kind = d.Kind;
            var mac = d.Mac;

            var lng = pos.substring(0, pos.indexOf(','));
            var lat = pos.substring(pos.indexOf(',') + 1);
            var point = new BMap.Point(lng, lat);

            //添加label
            var label = new BMap.Label(title);
            label.setPosition(point);
            map.addOverlay(label);

            var marker;
            var icon;
            if (kind == '开关控制器') {
                icon = new BMap.Icon("./images/map/off.png", new BMap.Size(16, 32));
                //setTimeout(function () {
                //    socket.send(mac.replace(/:/g, '') + '0001');
                //}, 200);
            } else if (kind == '温湿度传感器') {
                var icon = new BMap.Icon("./images/map/temperature.png", new BMap.Size(16, 84));
            } else if (kind == '土壤湿度传感器') {
                var icon = new BMap.Icon("./images/map/humidity.png", new BMap.Size(16, 32));
            } else if (kind == '电动阀控制器') {
                var icon = new BMap.Icon("./images/map/water.png", new BMap.Size(16, 32));
            } else if (kind == 'PM2.5传感器') {
                var icon = new BMap.Icon("./images/map/dust.png", new BMap.Size(16, 32));
            } else {
                marker = new BMap.Marker(point);
            }

            // 创建标注对象并添加到地图   
            marker = new BMap.Marker(point, { icon: icon });
            marker.mac = mac;
            map.addOverlay(marker);
            marker.addEventListener("click", function () {
                var url = '';
                if (kind == '温湿度传感器') {
                    url = '/Agriculture/js/SensorTemperature.js';
                } else if (kind == '土壤湿度传感器') {
                    url = '/Agriculture/js/SensorHumidity.js';
                } else if (kind == 'PM2.5传感器') {
                    url = '/Agriculture/js/SensorPm25.js';
                } else if (kind == '电动阀控制器') {
                    url = '/Agriculture/js/SensorValve.js';
                } else if(kind=='开关控制器') {
                    url = '';
                } else {
                    url = '';
                }

                $.get(url, null, function (data) {
                    createInfoWindow(kind, mac);
                    map.openInfoWindow(infoWindow, marker.getPosition());      // 打开信息窗口

                    setTimeout(function () {
                        //查询设备状态
                        socket.send(mac.replace(/:/g, '') + '0001');
                    }, 300);
                }, 'script');
            });
        });
    }); 
});