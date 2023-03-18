$(function () {
    // Get all filter items
    var filterItems = document.querySelectorAll('.category-link');

    // Add click event listener to each filter item
    filterItems.forEach(function (item) {
        item.addEventListener('click', function () {
            // Remove active class from all filter items
            filterItems.forEach(function (item) {
                item.classList.remove('active');
            });

            // Add active class to clicked filter item
            this.classList.add('active');
        });
    });

    $('a.category-link').click(function (e) {
        e.preventDefault();

        var categoryName = $(this).text().trim();

        $.ajax({
            url: '/NFT/FilterbyCategory',
            type: 'GET',
            data: { 'categoryName': categoryName },
            success: function (result) {
                $('#trending-categories').html(result);
            }
        });
    });
});


