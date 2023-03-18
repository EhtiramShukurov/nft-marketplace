//$(document).ready(function () {
//    $('#item-properties').click(function () {
//        $('#propertiesModal').modal('show');
//    });


//    $('#propertyform').submit(function (event) {
//        event.preventDefault();

//        var formData = $(this).serialize();

//        $.ajax({
//            url: '/Property/Create',
//            type: 'POST',
//            data: formData,
//            success: function (response) {
//                if (response.success) {
//                    $('#propertiesModal').modal('hide');
//                    propertyIds.push(response.Id);
//                    $('#property-ids-input').val(propertyIds.join(','));
//                } else {
//                    alert(response.message);
//                }
//            },
//            error: function (xhr) {
//                alert('An error occurred while submitting the form');
//            }
//        });
//        $(this)[0].reset();
//    });
//});