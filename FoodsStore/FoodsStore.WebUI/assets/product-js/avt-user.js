/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */


$(document).ready(function () {
    var readURL = function (input) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                $('.profile-pic').attr('src', e.target.result);
            }

            reader.readAsDataURL(input.files[0]);
        }
    }

    $(".file-upload").on('change', function () {
        readURL(this);
    });

    $(".upload-button").on('click', function () {
        $(".file-upload").click();
    });
});


function readURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $uploadedImg[0].style.backgroundImage = 'url(' + e.target.result + ')';
        };
        reader.readAsDataURL(input.files[0]);
    }
}

var $div = $("#imageUploadForm"),
        $file = $("#fileUpdate"),
        $uploadedImg = $("#uploadedImg"),
        $helpText = $("#helpText")
        ;
$file.on('change', function () {
    $div.removeClass('loading');
    readURL(this);
    $div.addClass('loading');

});
if ($file.val() !== null) {
    readURL($file);
}