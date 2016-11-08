function createInfoWindow(kind, mac) {
    var opts = {
        width: 250,     // 信息窗口宽度    
        height: 80,     // 信息窗口高度    
        title: '<div class="infoTitle">设备状态 - ' + kind + '</div>'  // 信息窗口标题
    }
    var content = $('<div class="center container">当前的土壤湿度为：<span  mac="' + mac + '"></span>%。</div>').get(0);

    infoWindow = new BMap.InfoWindow(content, opts);  // 创建信息窗口对象
}