/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */


jQuery(document).ready(function ($) {
    //close popup
    $('.message-popup').on('click', function (event) {
        if ($(event.target).is('.message-popup-close') || $(event.target).is('.message-popup') || $(event.target).is('.message-buttons')) {
            event.preventDefault();
            $(this).removeClass('is-visible');
        }
    });
    //close popup when clicking the esc keyboard button
    $(document).keyup(function (event) {
        if (event.which == '27') {
            $('.message-popup').removeClass('is-visible');
            $('.message-popup').children('.message-popup-container').remove('.message-display');
        }
    });
});