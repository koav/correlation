function initDataTable(table_id, ajaxSource, fnDataLoaded) {
    $(table_id).dataTable({
        "bProcessing": true,
        "bPaginate": false,
        "sScrollY": "400px",
        //        "bScrollCollapse": true,
        "bFilter": true,
        //        "bInfo": true,
        "bSort": true,
        //"bServerSide": true,
        "fnServerData": function (sSource, aoData, fnCallback) {
            $.ajax({
                "dataType": 'json',
                "type": 'post',
                "url": sSource,
                "data": aoData,
                "success": function (result) {
                    fnCallback(result);
                    if (fnDataLoaded != null) {
                        fnDataLoaded();
                    }
                }
            });
        },

        "oLanguage": {
            "sSearch": "搜索",
            "sLengthMenu": "每页显示 _MENU_ 条记录",
            "sZeroRecords": "抱歉， 没有找到",
            "sInfo": "从 _START_ 到 _END_ /共 _TOTAL_ 条数据",
            "sInfoEmpty": "没有数据",
            "sInfoFiltered": "(从 _MAX_ 条数据中检索)",
            "sProcessing": "正在处理……",
            "oPaginate": {
                "sFirst": "首页",
                "sPrevious": "上一页",
                "sNext": "下一页",
                "sLast": "尾页"
            }
        }

    });
}

function textValue(id) {
    var result = $(id).val();
    if (result == null || result == "") {
        return 0;
    }
    return result;
}

function ajaxUrl(url, loadData) {
    var result = url + "?" +
                            "load=" + loadData + "&" +
                            "attr=" + textValue('#selAttr') + "&" +
 							"support=" + textValue('#txtSupport') + "&" +
							"confidence=" + textValue('#txtConfidence') + "&" + 
                            "filter=" + textValue('#txtFilter');

    var time = new Date().getTime();
    result = result + "&_=" + time;

    return result;
}

jQuery.extend(jQuery.fn.dataTableExt.oSort, {
    "percent-pre": function (a) {
        var x = (a == "-") ? 0 : a.replace(/%/, "");
        return parseFloat(x);
    },

    "percent-asc": function (a, b) {
        return ((a < b) ? -1 : ((a > b) ? 1 : 0));
    },

    "percent-desc": function (a, b) {
        return ((a < b) ? 1 : ((a > b) ? -1 : 0));
    }
});

$(document).ready(function () {
    initDataTable('#correlation_result');

    $('#btnDig').click(function () {
        $('#correlation_result').dataTable().fnReloadAjax(ajaxUrl("../Processor.aspx", true));
    });
});