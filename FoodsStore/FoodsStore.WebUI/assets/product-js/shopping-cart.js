/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */


var check = false;

function changeVal(el) {
    var qt = parseFloat(el.parent().children(".qt").html());
    var price = parseFloat(el.parent().children(".price").html());
    var eq = Math.round(price * qt * 100) / 100;
    el.parent().children(".full-price").html(eq + "$");
    changeTotal();
}

function changeTotal() {
    var price = 0;
    $(".full-price").each(function (index) {
        price += parseFloat($(".full-price").eq(index).html());
    });

    price = Math.round(price * 100) / 100;

    var tax = Math.round(price * 0.1 * 100) / 100;

    var fullPrice = Math.round((price + tax) * 100) / 100;

    if (price === 0) {
        fullPrice = 0;
    }

    $(".subtotal span").html(price);
    $(".tax span").html(tax);
    $(".total span").html(fullPrice);
}

function changeQuantity(id, child) {

    $("#" + id + "*").val(id + "-" + child.html());

//    document.getElementById(id).value = child.html();
//    console.log($("#" + id ).val());
}

$(document).ready(function () {
   // $(".cd-yes").click(function () {

 //       var el = $(this);
 //       el.parent().parent().parent().parent().parent().parent().addClass("removed");
//        window.setTimeout(
//                function () {
//                el.parent().parent().parent().parent().parent().parent().slideUp('slow', function () {
 //                   el.parent().parent().parent().parent().parent().parent().remove();
 //                   if ($(".product").length === 0) {
  //                      if (check) {
  //                          $("#cart").html("!!!");
 //                       }
 //                       else {
 //                           $("#cart").html("<h1>You have no products!</h1>");
  //                          $("#go-to-pay").remove();
  //                       }
  //                  }
  //                  changeTotal();
  //              });
  //              }, 500);
  //  });

    $(".qt-plus").click(function () {
        var id = $(this).parent().children(".qt").prop("id");
        var child = $(this).parent().children(".qt");
//        console.log(id);
        var maxQuantity = $(this).parent().children(".maxQantity").val();
        var currentQuantity = parseInt(child.html()) + 1;
        console.log(currentQuantity);
//        if (currentQuantity > maxQuantity) {
//            console.log(currentQuantity + ">" + maxQuantity);
//            $(this).parent().children(".message-popup").addClass("is-visible");
//            $(".message-popup-container").children(".message-display").replaceWith("<p class='message-display'>This quantity is out of stock !!</p>");
 //       } else {
        $(this).parent().children(".qt").html(parseInt($(this).parent().children(".qt").html()) + 1);
        changeQuantity(id, child);
//               }
//        console.log(maxQuantity);
//        document.getElementsByClassName(id).value=child.html();
//        console.log(document.getElementsByClassName(id).value);


//
        $(this).parent().children(".full-price").addClass("added");
        var el = $(this);
        window.setTimeout(function () {
            el.parent().children(".full-price").removeClass("added");
            changeVal(el);
        }, 150);
    });

    $(".qt-minus").click(function () {
        child = $(this).parent().children(".qt");
        if (parseInt(child.html()) > 1) {
            child.html(parseInt(child.html()) - 1);
  //          var id = $(this).parent().children(".qt").prop("id");
  //          console.log(id);
  //          console.log(child.html());
        $(this).closest("input:hidden").val(child.html);
 //       console.log($(this).closest("input:hidden").val());
 //           changeQuantity(id, child );
            $(this).parent().children(".full-price").addClass("minused");
            var el = $(this);
            window.setTimeout(function () {
                el.parent().children(".full-price").removeClass("minused");
                changeVal(el);
            }, 150);
        } else {
            $('.cd-popup-container').children('p').replaceWith('<p class="message-display">Are you sure you want to remove this product from your cart?</p>');
            $(".cd-popup").addClass("is-visible");
            var productID = $(this).parent().children('#idProdHere').val();
            var IDBill = $(this).parent().children('#idBillHere').val();
            $('#prodID').val(productID);
            $('#idBill').val(IDBill);
            var liYes = '<li><a href="#0" class="cd-yes" style="background-color: crimson;"' +
                'onclick="removeProduct();">Yes</a></li> ';
            var liNo = '<li><a href="#0" class="cd-no">No</a></li>';
            $('.cd-buttons li:eq(2)').replaceWith(liYes);
            $('.cd-buttons li:eq(3)').replaceWith(liNo);
        }
    });

    window.setTimeout(function () {
        $(".is-open").removeClass("is-open");
    }, 1200);

    //$(".btn").click(function () {
    //    check = true;
   //     $(".remove").click();
   // });
});

function removeProdInCart(IDBill, productID) {   
    $('.message-popup').removeClass('is-visible');    
    var insideMes =
        '<div class="message-popup-container">' +
        '<p class="message-display">Wait us a second to update your cart !!</p>' +
        '</div>';
    $('.message-popup').html(insideMes);
    $('.message-popup').addClass('is-visible');
    $.ajax({
        url: "/Cart/RemoveAProductInCart?IDBill=" + IDBill + "&productID=" + productID,
        type: "POST",
        //data: '{IDBill: ' + IDBill + ',productID:' + productID + '}',
        success: function (result) {
            $('.message-popup').removeClass('is-visible');
            $("#" + productID).addClass("removed");
            window.setTimeout(
                function () {
                    $("#" + productID).slideUp('slow', function () {
                        $("#" + productID).remove();
                        if ($(".product").length === 0) {
                            if (check) {
                                $("#cart").html("!!!");
                            }
                            else {
                                $("#cart").html("<h1>You have no products!</h1>");
                                $("#go-to-pay").remove();
                            }
                        }
                        changeTotal();
                    });
                }, 500);
            $('.message-popup').addClass('is-visible');
            var insideMes = '<div class="message-popup-container">' +
                '<p class="message-display">Remove product ' + result + ' success !!</p>' +
                '</div>';
            $('.message-popup').html(insideMes);
            window.setTimeout(function () {
                $('.message-popup').removeClass('is-visible');
            }, 1300);

        },
        error: function (errormessage) {
            alert("error!!");
            alert(errormessage.responseText);
        }
    });
}
function removeProduct() {
    var IDBill = $('#idBill').val();
    var productID = $('#prodID').val();
    removeProdInCart(IDBill, productID);
}

function updateProductInCart(IDBill, productID) {
    
    var quantity = parseInt($('#qua-' + productID).html());
    getUpdateProduct(IDBill, productID, quantity);
}
function getUpdateProduct(IDBill,productID,quantity) {
    $('.message-popup').addClass('is-visible');
    var insideMes =
        '<div class="message-popup-container">' +
        '<p class="message-display">Wait us a second to check quantity in stock and update your cart !!</p>' +
        '</div>';
    $('.message-popup').html(insideMes);
    $.ajax({
        url: "/Cart/GetQuantityOfProduct?productID=" + productID,
        type: "POST",
        success: function (result) {
            var prodName = $('#prodName-' + productID).html();
            if (result == -1) {                    
                $('.message-popup').removeClass('is-visible');               
                var insideMesResult =
                    '<p class="message-display">Product ' + prodName + ' is stop selling!! Let Hana Shop remove it from your cart!</p>' +
                    '<a href="#0" class="message-buttons" onclick="removeProdInCart(' + IDBill + ',' + productID + ');" style="color: #000; font-size: 20px;">OK</a>';
                $('.message-popup-container').html(insideMesResult);
                $('.message-popup').addClass('is-visible');
            } else if (result == 0) {
                $('.message-popup').removeClass('is-visible');                
                var insideMesResult =
                    '<p class="message-display">Product ' + prodName + ' is out of stock!! Let Hana Shop remove it from your cart!</p>' +
                    '<a href="#0" class="message-buttons" onclick="removeProdInCart(' + IDBill + ',' + productID + ');"' +
                    'style="color: #000; font-size: 20px;">OK</a>';
                $('.message-popup-container').html(insideMesResult);
                $('.message-popup').addClass('is-visible');
            }
            else {
                if (quantity <= result) {
                    $.ajax({
                        url: "/Cart/UpdateCart?IDBill=" + IDBill + "&productID=" + productID + "&quantity=" + quantity,
                        type: "POST",
                        //data: '{IDBill: ' + IDBill + ',productID:' + productID + '}',
                        success: function (result) {
                            $('.message-popup').removeClass('is-visible');
                            $('#qua-' + productID).parent().children(".full-price").addClass("added");
                            $('#qua-' + productID).html(quantity);
                            var el = $('#qua-' + productID);
                            window.setTimeout(function () {
                                el.parent().children(".full-price").removeClass("added");
                                changeVal(el);
                            }, 150);
                        },
                        error: function (errormessage) {
                            alert("error!!");
                            alert(errormessage.responseText);
                        }
                    });
                }
                else {
                    $('.message-popup').removeClass('is-visible');
                    $('.cd-popup').addClass('is-visible');
                    $('#prodID').val(productID);
                    $('#idBill').val(IDBill);
                    var insideMes = '<p class="message-display">Product ' + prodName+' is out of stock !! Only ' + result + ' left on stock!!' +
                        'What do you want to do ?</p >';
                    $('.cd-popup-container').children('p').replaceWith(insideMes);
                    var liGetMax = '<li><a href="#0" class="cd-yes" style="background-color: crimson;"' +
                        'onclick="getUpdateProduct(' + IDBill + ',' + productID + ',' + result + ');">Take me ' + result + '</a></li> ';
                    var liRemove = '<li><a href="#0" class="cd-no" onclick="removeProduct();">Remove</a></li>';
                    $('.cd-buttons li:eq(2)').replaceWith(liGetMax);
                    $('.cd-buttons li:eq(3)').replaceWith(liRemove);
                }
            }
        }
    });
}

function checkCartIsValid(BillID) {
    $('.message-popup').addClass('is-visible');
    var insideMes =
        '<div class="message-popup-container">' +
        '<p class="message-display">Wait us a second to check quantity in stock !!</p>' +
        '</div>';
    $('.message-popup').html(insideMes);
    $.ajax({
        url: "/Cart/CheckCartIsValidToCheckOut?IDBill=" + BillID,
        type: "GET",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            if (result.ProductID != null) {
                $('.message-popup').removeClass('is-visible');
                var problem = result.Problem;
                if (problem.localeCompare("STOP") == 0) {
                    $('.message-popup').addClass('is-visible');
                    var insideMes =
                        '<p class="message-display">Product ' + result.ProductName + ' is stop selling!! Let Hana Shop remove it from your cart!</p>' +
                        '<a href="#0" class="message-buttons" onclick="removeProdInCart(' + BillID + ',' + result.ProductID + ');"' +
                        'style="color: #000; font-size: 20px;">OK</a>';
                    $('.message-popup-container').html(insideMes);
                } else if (problem.localeCompare("OUTS") == 0) {
                    $('.message-popup').addClass('is-visible');
                    var insideMes =
                        '<p class="message-display">Product ' + result.ProductName + ' is out of stock!! Let Hana Shop remove it from your cart!</p>' +
                        '<a href="#0" class="message-buttons" onclick="removeProdInCart(' + BillID + ',' + result.ProductID + ');"' +
                        'style="color: #000; font-size: 20px;">OK</a>';
                    $('.message-popup-container').html(insideMes);
                } else if (problem.localeCompare("HIGHER") == 0) {
                    $('.cd-popup').addClass('is-visible');
                    $('#prodID').val(productID);
                    $('#idBill').val(IDBill);
                    var insideMes = '<p class="message-display">Product ' + prodName + ' is out of stock !! Only ' + result + ' left on stock!!' +
                        'What do you want to do ?</p>';
                    $('.cd-popup-container').children('p').replaceWith(insideMes);
                    var liGetMax = '<li><a href="#0" class="cd-yes" style="background-color: crimson;"' +
                        'onclick="getUpdateProduct(' + IDBill + ',' + productID + ',' + result + ');">Take me ' + result + '</a></li> ';
                    var liRemove = '<li><a href="#0" class="cd-no" onclick="removeProduct();">Remove</a></li>';
                    $('.cd-buttons li:eq(2)').replaceWith(liGetMax);
                    $('.cd-buttons li:eq(3)').replaceWith(liRemove);
                } 
            } else {
                obj.closest('form').submit();
            }
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}

function goToCheckOut(BillID, obj) {
    $('.message-popup').addClass('is-visible');
    var insideMes =
        '<div class="message-popup-container">' +
        '<p class="message-display">Wait us a second to check quantity in stock !!</p>' +
        '</div>';
    $('.message-popup').html(insideMes);
    $.ajax({
        url: "/Cart/CheckCartIsValidToCheckOut?IDBill=" + BillID,
        type: "GET",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            if (result.ProductID != 0) {
                alert(result.ProductID);
                $('.message-popup').removeClass('is-visible');
                var problem = result.Problem;
                if (problem.localeCompare("STOP") == 0) {
                    $('.message-popup').addClass('is-visible');
                    var insideMes =
                        '<p class="message-display">Product ' + result.ProductName + ' is stop selling!! Let Hana Shop remove it from your cart!</p>' +
                        '<a href="#0" class="message-buttons" onclick="removeProdInCart(' + BillID + ',' + result.ProductID + ');"' +
                        'style="color: #000; font-size: 20px;">OK</a>';
                    $('.message-popup-container').html(insideMes);
                } else if (problem.localeCompare("OUTS") == 0) {
                    $('.message-popup').addClass('is-visible');
                    var insideMes =
                        '<p class="message-display">Product ' + result.ProductName + ' is out of stock!! Let Hana Shop remove it from your cart!</p>' +
                        '<a href="#0" class="message-buttons" onclick="removeProdInCart(' + BillID + ',' + result.ProductID + ');"' +
                        'style="color: #000; font-size: 20px;">OK</a>';
                    $('.message-popup-container').html(insideMes);
                } else if (problem.localeCompare("HIGHER") == 0) {
                    $('.cd-popup').addClass('is-visible');
                    $('#prodID').val(productID);
                    $('#idBill').val(IDBill);
                    var insideMes = '<p class="message-display">Product ' + prodName + ' is out of stock !! Only ' + result + ' left on stock!!' +
                        'What do you want to do ?</p>';
                    $('.cd-popup-container').children('p').replaceWith(insideMes);
                    var liGetMax = '<li><a href="#0" class="cd-yes" style="background-color: crimson;"' +
                        'onclick="getUpdateProduct(' + IDBill + ',' + productID + ',' + result + ');">Take me ' + result + '</a></li> ';
                    var liRemove = '<li><a href="#0" class="cd-no" onclick="removeProduct();">Remove</a></li>';
                    $('.cd-buttons li:eq(2)').replaceWith(liGetMax);
                    $('.cd-buttons li:eq(3)').replaceWith(liRemove);
                } else if (problem.localeCompare("CHANGEPRICE") == 0) {
                    
                    $('.message-popup').addClass('is-visible');
                    var insideMes =
                        '<p class="message-display">Product ' + result.ProductName + ' has price is changed !! Let check again from your cart!' +
                        '<br>' +                        
                        ' New price: ' + result.NewPrice + '| Old price: ' + result.OldPrice + '<br>' + 
                        'Click OK or F5 to reload your Cart'+
                        '</p>' +
                        '<a href="#0" class="message-buttons" ' +
                        'style="color: #000; font-size: 20px;" onclick="reloadCart();">OK</a>';
                    $('.message-popup-container').html(insideMes);
                }
            } else {
                obj.closest('form').submit();
            }
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}
function reloadCart() {
    location.reload(true);
}

