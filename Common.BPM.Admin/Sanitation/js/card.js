var CARDClass = {
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
        var o = { action: 'read', mem: 1, words: 6, start: '0002'};
        var command = JSON.stringify(o);

        invockWebSocket(command, success_handler, error_handler);
    },

    isTrunkCard: function (card) {
        var pre = String.fromCharCode(parseInt(card.substring(0, 2), 16)) + String.fromCharCode(parseInt(card.substring(2, 4), 16)) + String.fromCharCode(parseInt(card.substring(4, 6), 16));
        return pre == 'car';
    },

    isDriverCard: function (card) {
        var pre = String.fromCharCode(parseInt(card.substring(0, 2), 16)) + String.fromCharCode(parseInt(card.substring(2, 4), 16)) + String.fromCharCode(parseInt(card.substring(4, 6), 16));
        return pre == 'drv';
    },

    parseTrunkId: function (card) {
        var b3 = parseInt(card.substring(8, 10), 16);
        var b2 = parseInt(card.substring(10, 12), 16);
        var b1 = parseInt(card.substring(12, 14), 16);
        var b0 = parseInt(card.substring(14, 16), 16);

        var keyid = (b3 << 24) + (b2 << 16) + (b1 << 8) + b0;

        return keyid;
    },

    parseDriverId: function (card) {
        var b3 = parseInt(card.substring(8, 10), 16);
        var b2 = parseInt(card.substring(10, 12), 16);
        var b1 = parseInt(card.substring(12, 14), 16);
        var b0 = parseInt(card.substring(14, 16), 16);

        var keyid = (b3 << 24) + (b2 << 16) + (b1 << 8) + b0;

        return keyid;
    },

    parseDispatchId:function(card){
        var b3 = parseInt(card.substring(16, 18), 16);
        var b2 = parseInt(card.substring(18, 20), 16);
        var b1 = parseInt(card.substring(20, 22), 16);
        var b0 = parseInt(card.substring(22, 24), 16);

        var keyid = (b3 << 24) + (b2 << 16) + (b1 << 8) + b0;

        return keyid;
    },

    makeDriverCard: function (rid) {
        var data = 'drv\0';
        var b0 = rid & 0xFF;
        var b1 = (rid >> 8) & 0xFF;
        var b2 = (rid >> 16) & 0xFF;
        var b3 = (rid >> 24) & 0xFF;

        data += String.fromCharCode(b3) + String.fromCharCode(b2) + String.fromCharCode(b1) + String.fromCharCode(b0) + '\0\0\0\0';

        return data;
    },

    makeTrunkCard: function (rid) {
        var data = 'car\0';
        var b0 = rid & 0xFF;
        var b1 = (rid >> 8) & 0xFF;
        var b2 = (rid >> 16) & 0xFF;
        var b3 = (rid >> 24) & 0xFF;

        data += String.fromCharCode(b3) + String.fromCharCode(b2) + String.fromCharCode(b1) + String.fromCharCode(b0) + '\0\0\0\0';

        return data;
    },

    makeDispatchCard: function (did, rid) {
        var data = 'drv\0';
        var b0 = did & 0xFF;
        var b1 = (did >> 8) & 0xFF;
        var b2 = (did >> 16) & 0xFF;
        var b3 = (did >> 24) & 0xFF;

        data += String.fromCharCode(b3) + String.fromCharCode(b2) + String.fromCharCode(b1) + String.fromCharCode(b0);

        b0 = rid & 0xFF;
        b1 = (rid >> 8) & 0xFF;
        b2 = (rid >> 16) & 0xFF;
        b3 = (rid >> 24) & 0xFF;

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