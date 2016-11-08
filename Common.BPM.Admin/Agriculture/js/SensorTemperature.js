function createInfoWindow(kind, mac) {
    var opts = {
        width: 250,     // 信息窗口宽度    
        height: 110,     // 信息窗口高度    
        title: '<div class="infoTitle">设备状态 - ' + kind + '</div>'  // 信息窗口标题
    }
    var content = $('<div class="center container"><p>当前的环境温度为：<span mac="' + mac + '" kind="t"></span>°C。<p><p>当前的环境湿度为：<span mac="' + mac + '" kind="h"></span>%。<p></div>').get(0);

    infoWindow = new BMap.InfoWindow(content, opts);  // 创建信息窗口对象
}