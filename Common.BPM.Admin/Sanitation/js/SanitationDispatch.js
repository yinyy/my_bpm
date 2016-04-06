var actionURL = '/Sanitation/ashx/SanitationDispatchHandler.ashx';
var formurl = '/Sanitation/html/SanitationDispatch.html';
var formediturl = '/Sanitation/html/SanitationDispatchEdit.html';
var searchurl = '/Sanitation/html/SanitationDispatchSearch.html';
var map;


$(function () {

    autoResize({ dataGrid: '#list', gridType: 'treegrid', callback: grid.bind, height: 0 });

    //高级查询
    $('#a_search').click(function () {
        var hDialog = top.jQuery.hDialog({
            title: '查询', width: 350, height: 185, href: searchurl, iconCls: 'icon-search', buttons: [
                {
                    text: '确定', iconCls: 'icon-ok', handler: function () {
                        if (top.$('#uiform').form('validate')) {
                            var filter = '';
                            if (top.$('#txt_Condition').val() == '') {
                                filter = '{"groupOp":"AND","rules":[{"field":"Time","op":"ge","data":"' +
                                top.$('#txt_Time_Start').datebox('getValue') + '"}, {"field":"Time","op":"le","data":"' +
                                top.$('#txt_Time_End').datebox('getValue') + '"}],"groups":[]}';
                            } else {
                                filter = '{"groupOp":"AND","rules":[{"field":"Time","op":"ge","data":"' +
                                top.$('#txt_Time_Start').datebox('getValue') + '"}, {"field":"Time","op":"le","data":"' +
                                top.$('#txt_Time_End').datebox('getValue') + '"}],"groups":[{"groupOp":"OR","rules":[{"field":"Plate","op":"cn","data":"' +
                                top.$('#txt_Condition').val() + '"},{"field":"Name","op":"cn","data":"' +
                                top.$('#txt_Condition').val() + '"}],"groups":[]}]}';
                            }

                            $('#list').datagrid('reload', { filter: filter });
                            $('body').data('where', filter);//.replace('Time', '加水时间').replace('Name', '姓名').replace('Plate', '车牌号')
                            hDialog.dialog('close');
                        }
                        return false;
                    }},
                {
                    text: '清空', iconCls: 'icon-clear', handler: function () {
                        $('#list').datagrid('reload', { filter: '' });
                        $('body').data('where', '');
                        hDialog.dialog('close');
                    }},
                {
                    text: '关闭', iconCls: 'icon-cancel', handler: function () {
                        hDialog.dialog('close');
                    }}
            ]
        });

        top.$('#uiform').validate();
    });
    $('#a_export').click(function () {
        var ee = new ExportExcel('list', "V_Dispatch2",
            [
            { title: '姓名', field: 'Name' },
            { title: '编号', field: 'Code' },
            { title: '车牌号', field: 'Plate' },
            { title: '加水时间', field: 'Time' },
            { title: '加水地点', field: 'Address' },
            { title: '类型', field: 'Kind' },
            { title: '容积', field: 'Volumn' },
            { title: '浓度', field: 'Potency' },
            { title: '工作状态', field: 'Status' },
            { title: '签到时间', field: 'Signed' },
            { title: '管子', field: 'Working' },
            { title: '坐标', field: 'Destination' },
            { title: '区域', field: 'Region' }
            ]);
        ee.go();
    });

    map = new BMap.Map('dd');          // 创建地图实例
    map.addControl(new BMap.NavigationControl());
    map.enableScrollWheelZoom(true);
});

var grid = {
    bind: function (winSize) {
        $('#list').datagrid({
            url: actionURL,
            toolbar: '#toolbar',
            title: "数据列表",
            iconCls: 'icon icon-list',
            width: winSize.width,
            height: winSize.height,
            nowrap: false, //折行
            rownumbers: true, //行号
            striped: true, //隔行变色
            idField: 'KeyId',//主键
            singleSelect: true, //单选
            frozenColumns: [[]],
            columns: [[
		    {
		        title: '日期', field: 'Time', width: 160, align: 'center', formatter: function (v, r, i) {
		            return v;
		        }
		    },
		    {
		        title: '姓名', field: 'Name', width: 120, align: 'center',formatter: function (v, r, i) {
		            return v + '[' + r.Code + ']';
		        }
		    },
		    {
		        title: '车辆', field: 'Plate', width: 120, align: 'center', formatter: function (v, r, i) {
		            return '鲁' + v;
		        }
		    },
            {
                title: '加注地点', field: 'Address', width: 120, align: 'center'
            },
		    {
		        title: '加注类型', field: 'Kind', width: 120, align: 'center', formatter: function (v, r, i) {
		            if (v == 0) {
		                return "纯水";
		            } else {
		                return "肥水";
		            }
		        }
		    },
		    {
		        title: '加注浓度', field: 'Potency', width: 120, align: 'right', formatter: function (v, r, i) {
		            return v + '‰';
		        }
		    },
            {
                title: '加注量', field: 'Volumn', width: 120, align: 'right', formatter: function (v, r, i) {
                    return v + '方';
                }
            },
            {
                title: '当前状态', field: 'Status', width: 100, align: 'center', formatter: function (v, r, i) {
                    if (v == 0) {
                        return '工作中';
                    }else if (v == 1) {
                        return '完成';
                    } else {
                        return '未知';
                    }
                }
            }, {
                title: '管子类型', field: 'Working', width: 100,align:'center', formatter: function (v, r, i) {
                    if (r.Status == 1) {
                        if (v == 0) {
                            return "粗管";
                        } else if (v == 1) {
                            return "细管";
                        }
                    } else {
                        return '';
                    }
                }
            },
		    {
		        title: '签到时间', field: 'Signed', width: 200, align: 'center', formatter: function (v, r, i) {
		            if (r.Status == 1) {
		                return v;
		            } else {
		                return '';
		            }
		        }},
            {
                title: '签到地点', field: 'Destination', width: 80, align: 'center', formatter: function (v, r, i) {
                    if (r.Status == 1) {
                        var ss = v.split(',');
                        return '<a href="javascript:showMap(' + ss[0] + ', ' + ss[1] + ');">查看</a>';
                    } else {
                        return '';
                    }
                }
            },
            {
                title: '签到区域', field: 'Region', width: 80, align: 'center', formatter: function (v, r, i) {
                    if (r.Status == 1) {
                        if (v == 1) {
                            return "内部";
                        } else if (v == 0) {
                            return "外部";
                        }
                    } else {
                        return "";
                    }
                }
            }
            ]],
            pagination: true,
            pageSize: PAGESIZE,
            pageList: [20, 40, 50]
        });
    },
    getSelectedRow: function () {
        return $('#list').datagrid('getSelected');
    },
    reload: function () {
        $('#list').datagrid('clearSelections').datagrid('reload', { filter: '' });
    }
};

function createParam(action, keyid) {
    var o = {};
    var query = top.$('#uiform').serializeArray();
    query = convertArray(query);
    o.jsonEntity = JSON.stringify(query);
    o.action = action;
    o.keyid = keyid;
    return "json=" + JSON.stringify(o);
}

function convertGpgga(v) {
    var a = parseInt(v);
    var b = (v - a) * 100.0 / 60.0;

    return a + b;
}

function showMap(lng, lat) {
    //var hDialog = top.jQuery.hDialog({
    //    title: '地图', width: 400, height: 463, content: '<div id="map_container"></div>', iconCls: 'icon-add'
    //});

    $('#dd').dialog({
        title: '地图',
        width: 800,
        height: 550,
        closed: false, 
        cache: false,
        modal: false
    }).dialog('maximize');

    var xx = convertGpgga(lng);
    var yy = convertGpgga(lat);
    var gpsPoint = new BMap.Point(xx, yy);

    //alert(xx + "," + yy);

    //map.centerAndZoom("东营市", 12);                 // 初始化地图，设置中心点坐标和地图级别
    map.clearOverlays();

    BMap.Convertor.translate(gpsPoint, 0, function (point) {
        var marker = new BMap.Marker(point);
        map.addOverlay(marker);
        var label = new BMap.Label("签到地点", { offset: new BMap.Size(20, -10) });
        marker.setLabel(label); //添加百度label
        map.centerAndZoom(point, 15);
    });     //真实经纬度转成百度坐标

    //绘制工作区域
    var wrp = [];
    for (var i = 0; i < workRegionPoints.length; i++) {
        wrp[wrp.length] = new BMap.Point(workRegionPoints[i].x, workRegionPoints[i].y);
    }

    polygon = new BMap.Polygon(wrp, { strokeColor: "blue", strokeWeight: 2, strokeOpacity: 0.5, fillColor: 'red', fillOpacity: 0.05 });  //创建多边形
    map.addOverlay(polygon);   //增加多边形
    //polygon.enableEditing();
}