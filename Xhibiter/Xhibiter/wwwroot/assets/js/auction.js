$(document).ready(function () {
    // Create a connection to the hub
    var connection = new signalR.HubConnectionBuilder()
        .withUrl('/auctionHub')
        .build();

    // Start the connection
    connection.start()
        .then(function () {
            console.log('SignalR connection started');
        })
        .catch(function (error) {
            console.error(error);
        });

    $('.bidbutton').on('click', function (a) {
        var auctionId = $(this).data('auctionid');
        var username = $(this).data('username');
        a.preventDefault();
        $.ajax({
            type: "POST",
            url: "Payment/PlaceBid",
            data: {
                "auctionId": auctionId,
                "username": username,
            },
            success: function (res) {
                $("#placeBidModal").find(".modal-body").html(res);
            },
            error: function (err) {
                console.log(err)
            }
        });
    });
    $('#biddingform').submit(function (event) {
        event.preventDefault();
        var formData = $(this).serialize();

        $.ajax({
            url: '/Bid/PlaceBid',
            type: 'POST',
            data: formData,
            success: function (response) {
                if (response.success) {
                    $('#placeBidModal').modal('hide');
                    alert(response.message);
                    var auctionId = $('#auctionId').val();
                    connection.invoke('UpdateBid', auctionId)
                        .catch(function (error) {
                            console.error(error);
                        });
                } else {
                    // If the operation was not successful, show an error message to the user
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

    // Register a client method to handle bid updates
    connection.on('bidUpdated', function (auctionId, bidAmount, bidderName) {
        var id = auctionId;
        // Update the UI to show the new highest bidder
        $('#highestbidder').html(`
            <span  class="text-sm text-jacarta-400 dark:text-jacarta-300">Highest bid by </span>
            <a href="/Profile?username=${bidderName}" class="text-sm font-bold text-accent"
                        >${bidderName}</a>
                                    `);
        $('.highestt-' + id).text(bidAmount + ' ETH');
    
        // Update the UI to show the new bid amount and bidder name
    });
});
