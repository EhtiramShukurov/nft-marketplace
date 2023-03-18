$(document).ready(function () {

$('#buyingform').submit(function (event) {
    event.preventDefault();
    var formData = $(this).serialize();
    $.ajax({
        url: '/Payment/BuyNFT',
        type: 'POST',
        data: formData,
        success: function (response) {
            if (response.success) {
                setTimeout(function () {
                    $('#buyNowModal').modal('hide');
                    alert(response.message);
                    location.reload();
                }, 1000);
            } else {
                alert(response.message);
            }
        },
        error: function (xhr) {
            // If there's an error, show an error message to the user
            alert('An error occurred while submitting the form');
        }
    });

    $(this)[0].reset();
});
});