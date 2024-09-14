function ConfirmationAlert(deleteUrl) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            // If user confirms deletion, call the DeleteConfirmed action via AJAX
            $.ajax({
                url: deleteUrl,
                method: 'POST',
                data: { __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val() },
                dataType: 'json',
                success: function (data) {
                    if (data.success) {
                        // Show success message and reload the page after user clicks OK
                        Swal.fire({
                            title: 'Success',
                            text: data.message,
                            icon: 'success'
                        }).then(() => {
                            location.reload();
                        });
                    } else {
                        // Optionally handle error scenario
                        Swal.fire({
                            title: 'Error',
                            text: data.message,
                            icon: 'error'
                        });
                    }
                },
                error: function (xhr, status, error) {
                    Swal.fire({
                        title: 'Error',
                        text: 'Failed to delete user.',
                        icon: 'error'
                    });
                }
            });
        }
    });
}
