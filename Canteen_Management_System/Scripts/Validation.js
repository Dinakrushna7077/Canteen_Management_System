$(document).ready(function () {
    // Display success message for 5 seconds
    if ($("#successMessage").text().trim() !== "") {
        $("#successMessage").fadeIn();
        setTimeout(function () {
            $("#successMessage").fadeOut();
        }, 5000); // Hide after 5 seconds
    }

    // Display warning message for 5 seconds
    if ($("#warningMessage").text().trim() !== "") {
        $("#warningMessage").fadeIn();
        setTimeout(function () {
            $("#warningMessage").fadeOut();
        }, 5000); // Hide after 5 seconds
    }
});

/*function showMessage(messageId) {
    var messageText = $(messageId).text().trim();
    if (messageText !== "") {
        $(messageId).fadeIn();
        setTimeout(function () {
            $(messageId).fadeOut();
        }, 5000); // Hide after 5 seconds
    }
}*/

function showMessage(messageId, messageText) {
    $(messageId).text(messageText).fadeIn();
    setTimeout(function () {
        $(messageId).fadeOut();
    }, 5000); // Hide after 5 seconds
}

//Profile ----------------------
$(document).ready(function () {
    // When the profile link is clicked
    $('#profile').click(function (e) {
        e.preventDefault(); // Prevent the default anchor behavior

        // Toggle the visibility of the dropdown menu
        $('#profileDropdown').toggleClass('show'); // Use 'show' class to control visibility
    });

    // Optionally, hide the dropdown if clicked outside
    $(document).click(function (e) {
        // If the clicked target is not inside the profile or the dropdown, hide the dropdown
        if (!$(e.target).closest('#prof').length) {
            $('#profileDropdown').removeClass('show');
        }
    });
});

