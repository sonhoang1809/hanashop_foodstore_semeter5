
function addToCart(productID, quantity) {
    $.ajax({
        url: "/Cart/GetMaxQuantityCanAddToCart?productID=" + productID,
        type: "POST",
        success: function (result) {           
            if (quantity <= result) {
                $.ajax({
                    url: "/Cart/AddToCart",
                    type: "POST",
                    data: '{productID: "' + productID + '",quantity:"' + quantity + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (result) {
                        $('#UserbillId').val(result.BillID);
                        $('#num-prod-in-bill').html('<i class="fas fa-shopping-cart"></i>' + result.QuantityProduct);                       
                        var insideSpCart =
                            '<li>' +
                            '<div>' +
                            '<header id="site-header">' +
                            '<div class="container">' +
                            '<h3> Your shopping cart</h3>' +
                            '</div>' +
                            '</header>' +
                            '<div class="container">' +
                            '<section id="cart">';
                        $.each(result.ProductsInCart, function (i, item) {
                            insideSpCart = insideSpCart +
                                '<article class="product">' +
                                ' <header>' +
                                '<img src="/Product/GetImageProduct?productId=' + item.ProductID + '" alt="">' +
                                '</header>' +
                                '<div class="content">' +
                                '<h5>' + item.ProductName + '</h5>' + item.ShortDescription +
                                '</div>' +
                                '<footer class="content">' +
                                '<span class="qt">Kind: ' + item.Category + '</span>' +
                                '<span class="qt">Category:' + item.Kind + '</span>' +
                                '<span class="qt">Quantity:' + item.Quantity + '</span>';
                            if (item.StatusCode.localeCompare("OUTS") == 0) {
                                insideSpCart = insideSpCart +
                                    '<span class="qt" style="background-color: red;">Out of Stock</span>';
                            }
                            else if (item.StatusCode.localeCompare("STOP") == 0) {
                                insideSpCart = insideSpCart + '<span class="qt" style="background-color: red;">Stop selling</span>';
                            }
                            insideSpCart = insideSpCart +

                                '<h5 class="full-price">' + item.Total + '$</h5>' +
                                '<h5 class="price">' + item.Price + '$</h5>' +
                                '</footer>' +
                                '</article>';
                        });
                        insideSpCart = insideSpCart +
                            '</section>' +
                            '</div>' +
                            '<footer id="site-footer">' +
                            '<div class="container clearfix">' +
                            '<div class="left">' +
                            '<h5 class="subtotal">Subtotal: <span>' + result.SubTotal + '</span>$</h5>' +
                            '<h5 class="tax">Taxes (10%): <span>' + result.Tax + '</span>$</h5>' +
                            '<h5>Last time change: ' + result.LastTimeChange + '</h5>' +
                            '</div>' +
                            '<div class="right">' +
                            '<h3 class="total">Total: <span>' + result.Total + '</span>$</h3>' +
                            '<form action="/Pay/Index" method="POST">' +
                            '<input type="hidden" name="IDBill" value="' + result.BillID + '">' +
                            '<a class="btn-sp-cart" onclick="$(this).closest("form").submit();" style="color: #fff;">Checkout</a>' +
                            '</form>' +
                            ' </div>' +
                            '</div>' +
                            '</footer>' +
                            '</div>' +
                            '</li>';
                        $('#sp-cart-user').html(insideSpCart);
                    },
                    error: function (errormessage) {
                        alert("error!!");
                        alert(errormessage.responseText);
                    }
                });
            }
            else {
                $(".message-popup").addClass("is-visible");
                var insideMess = '<p class="message-display">This product is out of stock!! Only ' + result + ' left on stock!!</p>' +
                    '<a href="#0" class="message-buttons" style="font-size: 20px; color: #000;">OK</a>' +
                    '<a href="#0" class="message-popup-close img-replace">Close</a>';
                $(".message-popup-container").html(insideMess);
            }
        }, error: function (errormessage) {
            alert("error!!");
            alert(errormessage.responseText);
        }
    });
}

jQuery(document).ready(function ($) {
    $('#addcart*').click(function () {
        event.preventDefault();
        var productID = $(this).parent().children('#productID').val();        
        var quantity = $(this).parent().children('#quantity').val();
        $(this).closest('.cd-popup').removeClass('is-visible');
        addToCart(productID, quantity);
    });
});