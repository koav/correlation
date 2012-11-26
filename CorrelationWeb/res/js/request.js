function textValue(id) {
    var result = $(id).val();
    if (result == null || result == "") {
        return 0;
    }
    return result;
}

function ajaxUrl(url) {
    var result = url + "?" +
                            "attr=" + textValue('#selAttr') + "&" +
 							"support=" + textValue('#txtSupport') + "&" +
							"confidence=" + textValue('#txtConfidence') + "&" +
                            "filter=" + textValue('#txtFilter');

    var time = new Date().getTime();
    result = result + "&_=" + time;

    return result;
}