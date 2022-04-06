$(function () {
    var picker = $('#picker');
    if (picker.length) {
        picker.farbtastic('#ChatNameColor');
    }

    $('div > #UpdateDisplayNameButton')
           .on('click',
               function () {
                   var userId = $(this).data('id');
                   var currentName = $(this).data('current');

                   $('#Id').val(userId);
                   $('#CurrentDisplayName').val(currentName);
               });
});