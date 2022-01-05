/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

function changeQuantity(id, child) {

    $("#" + id + "*").val(child.html());

//    document.getElementById(id).value = child.html();
//    console.log($("#" + id ).val());
}
$(document).ready(function () {
    $(".qt-plus").click(function () {
        var id = $(this).parent().children(".qt").prop("id");
        var child = $(this).parent().children(".qt");
        $(this).parent().children(".qt").html(parseInt($(this).parent().children(".qt").html()) + 1);
        changeQuantity(id, child);
        

    });
    $(".qt-minus").click(function () {
        child = $(this).parent().children(".qt");
        if (parseInt(child.html()) > 1) {
            child.html(parseInt(child.html()) - 1);
            var id = $(this).parent().children(".qt").prop("id");           
            changeQuantity(id, child);
        }
    });
});

