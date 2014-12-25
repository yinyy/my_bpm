var actionURL = '/Sanitation/ashx/SanitationDetailHandler.ashx';
var formurl   = '/Sanitation/html/SanitationDetail.html';

$(function () {

    autoResize({ dataGrid: '#list', gridType: 'datagrid', callback: grid.bind, height: 0 });

    $('#a_export').click(CRUD.export0);
//高级查询
    $('#a_search').click(CRUD.search);
    $('#a_refresh').click(function () {
        grid.reload();
    });
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
		        title: '时间', field: 'Time', width: 150, align: 'center', formatter: function (v, r, i) {
		            return v.substring(0, 16);
		        }
		    },
		    { title: '地点', field: 'Title', width: 150 },
            {
                title: '姓名', field: 'Name', width: 120, formatter: function (v, r, i) {
                    return v + '[' + r.Code + ']';
                }
            },
		    { title: '车牌号', field: 'Plate', width: 120, align: 'center' },
		    {
		        title: '加注量', field: 'Volumn', width: 120, align: 'right', formatter: function (v, r, i) {
		            return v + ' m³';
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

var condition;//查询条件

var CRUD = {
    search: function () {
        var hDialog = top.jQuery.hDialog({
            title: '导出', width: 450, height: 302, href: formurl, iconCls: 'icon-add', submit: function () {
                condition = { groupOp: 'AND', Rules: [], Groups: [] };//{ groupOp: 'AND', Rules: [{ field: 'MONTH(TIME)', data: '11', op: 'eq' }], Groups: [] };

                var value;
                //先检查时间
                if (top.$('#rbDay').attr('checked') == 'checked') {
                    value = top.$('#txt_day').datebox('getValue');

                    condition.Rules[condition.Rules.length] = { field: 'convert(varchar(10), time, 20)', op: 'eq', data: value };
                } else if (top.$('#rbMonth').attr('checked') == 'checked') {
                    value = top.$('#txt_month').combobox('getValue');
                    if (value == '') {
                        alert('请选择月份。');
                        return false;
                    }

                    condition.Rules[condition.Rules.length] = { field: 'convert(varchar(7), time, 20)', op: 'eq', data: value };
                } else if (top.$('#rbQuarter').attr('checked') == 'checked') {
                    value = top.$('#txt_quarter').combobox('getValue');
                    if (value == '') {
                        alert('请选择季度。');
                        return false;
                    }

                    var ty = value.substring(0, 4);
                    var tq = value.substring(5);
                    
                    if (tq == 'Q1') {
                        condition.Groups[condition.Groups.length] = {
                            groupOp: 'OR', Rules: [
                                { field: 'convert(varchar(7), time, 20)', op: 'eq', data: ty + '-01' },
                                { field: 'convert(varchar(7), time, 20)', op: 'eq', data: ty + '-02' },
                                { field: 'convert(varchar(7), time, 20)', op: 'eq', data: ty + '-03' }
                            ], Groups: []
                        };
                    } else if (tq == 'Q2') {
                        condition.Groups[condition.Groups.length] = {
                            groupOp: 'OR', Rules: [
                                { field: 'convert(varchar(7), time, 20)', op: 'eq', data: ty + '-04' },
                                { field: 'convert(varchar(7), time, 20)', op: 'eq', data: ty + '-05' },
                                { field: 'convert(varchar(7), time, 20)', op: 'eq', data: ty + '-06' }
                            ], Groups: []
                        };
                    } else if (tq == 'Q3') {
                        condition.Groups[condition.Groups.length] = {
                            groupOp: 'OR', Rules: [
                                { field: 'convert(varchar(7), time, 20)', op: 'eq', data: ty + '-07' },
                                { field: 'convert(varchar(7), time, 20)', op: 'eq', data: ty + '-08' },
                                { field: 'convert(varchar(7), time, 20)', op: 'eq', data: ty + '-09' }
                            ], Groups: []
                        };
                    } else if (tq == 'Q4') {
                        condition.Groups[condition.Groups.length] = {
                            groupOp: 'OR', Rules: [
                                { field: 'convert(varchar(7), time, 20)', op: 'eq', data: ty + '-10' },
                                { field: 'convert(varchar(7), time, 20)', op: 'eq', data: ty + '-11' },
                                { field: 'convert(varchar(7), time, 20)', op: 'eq', data: ty + '-12' }
                            ], Groups: []
                        };
                    }
                } else if (top.$('#rbYear').attr('checked') == 'checked') {
                    value = top.$('#txt_year').combobox('getValue');
                    if (value == '') {
                        alert('请选择年。');
                        return false;
                    }

                    condition.Rules[condition.Rules.length] = { field: 'YEAR(time)', op: 'eq', data: value };
                }

                value = top.$('#txt_trunk').combobox('getValues');
                if (value.length>0) {
                    condition.Rules[condition.Rules.length] = {
                        field: 'TrunkId', op: 'in', data: value.join(',')
                    };
                }

                value = top.$('#txt_driver').combobox('getValues');
                if (value.length > 0) {
                    condition.Rules[condition.Rules.length] = {
                        field: 'DriverId', op: 'in', data: value.join(',')
                    };
                }

                value = top.$('#txt_address').combobox('getValues');
                if (value.length > 0) {
                    condition.Rules[condition.Rules.length] = {
                        field: 'Address', op: 'in', data: '"' + value.join('","') + '"'
                    };
                }
                $('#list').datagrid('clearSelections').datagrid('reload', { filter: JSON.stringify(condition) });
                return false;
            }, onLoad: function () {
                var n = new Date();
                top.$('#txt_day').datebox('setValue', n.getFullYear() + '-' + (n.getMonth() + 1) + '-' + n.getDate());
            }
        });
    },
    export0: function () {
        var o = {};
        o.jsonEntity = JSON.stringify(condition);
        o.action = 'export';
        var query = "json=" + JSON.stringify(o);

        window.open(actionURL+'?'+query);
    }
};

