

function _getCategories(kindId) {

    $.ajax({
        url: "/Category/GetCategories" + "?kindId=" + kindId,
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            var html = '';
            var ulli = '';
            html += '<option value="">';
            html += "Choose one";
            html += '</option>';
            ulli += '<li data-value class="option selected focus">Choose one</li>';           
            $.each(result, function (key, item) {
                html += '<option value="' + item.CategoryID + '">' + item.CategoryName + '</option>';
                ulli += '<li data-value="' + item.CategoryID + '" class="option">' + item.CategoryName +'</li>';
            });
            $('#categoryId').html(html);
            $('#categoryId').parent().children('div').children('span').text('Choose one');
            $('#categoryId').parent().children('div').children('ul').html(ulli);
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}

function _getCategoryWithSelectTag(obj) {
    var value = obj.value;
    if (value != "") {
        _getCategories(value);
    } else {
        var html = '';
        var ulli = '';
        html += '<option selected value="">';
        html += "Choose one";
        html += '</option>';
        ulli += '<li data-value class="option selected focus">Choose one</li>';           
        var ulli = '<ul class="list">';
        ulli += '<li data-value selected class="option selected focus">Choose one</li>';
        ulli += '</ul>';
        $('#categoryId').html(html);
        $('#categoryId').parent().children('div').children('span').text('Choose one');
        $('#categoryId').parent().children('div').children('ul').html(ulli);

    }
}