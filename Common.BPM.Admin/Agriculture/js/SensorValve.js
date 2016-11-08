function createInfoWindow(kind, mac) {
    var opts = {
        width: 250,     // 信息窗口宽度    
        height: 80,     // 信息窗口高度    
        title: '<div class="infoTitle">设备状态 - ' + kind + '</div>'  // 信息窗口标题
    }

    var content = $('<div class="center container">电磁阀开度：<input type="text" name="sensor_value" way="1" class="center" style="width:40px" mac="' + mac + '" value="0"/>%&nbsp;</div>');
    var btn = $('<input type="button" value="设置"/>');
    btn.click(function () {
        var value = $.trim($("[name='sensor_value'][way='1']").val());
        if (isNaN(value)) {
            alert('请输入正确的电磁阀开度。');
            return;
        }

        value = parseInt(value);
        if (value <= 0) {
            value = 0;
        }
        if (value >= 100) {
            value = 100;
        }

        socket.send($("[name='sensor_value']").attr('mac').replace(/:/g, '') + '0002' + (Array(2).join('0') + value.toString(16)).slice(-2));
    });

    content.append(btn);

    infoWindow = new BMap.InfoWindow(content.get(0), opts);  // 创建信息窗口对象
}