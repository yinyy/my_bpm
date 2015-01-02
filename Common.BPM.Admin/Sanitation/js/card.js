var CARDClass = {
    //写入EPC号的时候，是从0002开始写入的。
    writeEPC: function (data, success_handler, error_handler) {
        var o = { action: 'write', mem: 1, start: '0002', data: data };
        var command = JSON.stringify(o);

        invockWebSocket(command, success_handler, error_handler);
    },

    writeEPCWithoutTips: function (data, success_handler, error_handler) {
        var o = { action: 'write', mem: 1, start: '0002', data: data };
        var command = JSON.stringify(o);

        invockWebSocket(command, success_handler, error_handler, true);
    },

    readEPC: function (success_handler, error_handler) {
        var o = { action: 'read', mem: 1, start: '0002'};
        var command = JSON.stringify(o);

        invockWebSocket(command, success_handler, error_handler);
    },

    prefix:function(card){
        return String.fromCharCode(parseInt(card.substring(0, 2), 16)) + String.fromCharCode(parseInt(card.substring(2, 4), 16));
    },

    isTrunkCard: function (card) {
        return CARDClass.prefix(card) == 'TK';
    },

    isDriverCard: function (card) {
        return CARDClass.prefix(card) == 'PP';
    },

    parseTrunkId: function (card) {
        var b1 = parseInt(card.substring(4, 6), 16);
        var b0 = parseInt(card.substring(6, 8), 16);

        return keyid = (b1 << 8) + b0;
    },

    parseDriverId: function (card) {
        var b1 = parseInt(card.substring(4, 6), 16);
        var b0 = parseInt(card.substring(6, 8), 16);

        return (b1 << 8) + b0;
    },

    parseDispatchId:function(card){
        var b3 = parseInt(card.substring(16, 18), 16);
        var b2 = parseInt(card.substring(18, 20), 16);
        var b1 = parseInt(card.substring(20, 22), 16);
        var b0 = parseInt(card.substring(22, 24), 16);

        return (b3 << 24) + (b2 << 16) + (b1 << 8) + b0;
    },

    //人员前缀为PP（第1字），后面跟着KeyId（第2Word，最大表示65535左右），再接上人员编号D001（第3、4字），再接上任务KeyId（第5、6字）。
    makeDriverCard: function (rid, code) {
        var data = 'PP';
        var b0 = rid & 0xFF;
        var b1 = (rid >> 8) & 0xFF;

        data += String.fromCharCode(b1) + String.fromCharCode(b0);
        data += code;
        data += '\0\0\0\0';

        return data;
    },

    //车辆前缀为TK（第1字），后面跟着KeyId（第2Word，最大表示65535左右），再接上车牌号E12345（第3、4、5字）。
    makeTrunkCard: function (rid, plate) {
        var data = 'TK';
        var b0 = rid & 0xFF;
        var b1 = (rid >> 8) & 0xFF;

        data += String.fromCharCode(b1) + String.fromCharCode(b0);
        data += plate;
        data += '\0\0';

        return data;
    },

    makeDispatchCard: function (driverCard, did) {
        var data = driverCard.substring(0, 8);
        var b0 = did & 0xFF;
        var b1 = (did >> 8) & 0xFF;
        var b2 = (did >> 16) & 0xFF;
        var b3 = (did >> 24) & 0xFF;

        data += String.fromCharCode(b3) + String.fromCharCode(b2) + String.fromCharCode(b1) + String.fromCharCode(b0);
        
        return data;
    }
};

function invockWebSocket(command, success_handler, error_handler, notips) {
    window.WebSocket = window.WebSocket || window.MozWebSocket;
    if (!window.WebSocket) {
        alert("你的浏览器不支持WebSocket关键技术。请使用Google Chrome、Firefox浏览器，或升级IE至版本10以上。");
        return;
    }

    var cfm = true;
    if (!notips) {
        cfm = confirm('请将磁卡放在设备上，并点击确定写卡。');
    }
    if (cfm) {
        var wsUri = "ws://localhost:8123";

        var websocket = new WebSocket(wsUri);
        websocket.onopen = function (evt) {
            websocket.send(command);
        };
        websocket.onclose = function (evt) {

        };
        websocket.onmessage = function (evt) {
            var msg = evt.data.toString();

            if (msg == 'error_connect') {
                alert('请选连接读写设备。');
            } else if (msg == 'error_recognize') {
                alert('识别卡错误。');
            } else if (evt.data == 'error_write') {
                alert('写卡错误。');
            } else if (evt.data == 'error_read') {
                alert('读卡错误。');
            } else if (evt.data == 'error_length') {
                alert('数据错误。');
            } else if (msg.indexOf('success') != -1) {
                if (success_handler != null) {
                    success_handler(msg.substring(msg.indexOf('_') + 1));
                }
            } else {
                alert('其它错误：' + msg);
            }
        };
        websocket.onerror = function (evt) {
            alert('WebSocket错误：' + evt.data);
            if (error_handler != null) {
                error_handler(evt);
            }
        };
    }
}