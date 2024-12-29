$(document).ready(function () {

    $(".maintool").on("hover", function (e) {
        window.location.reload()
        //$('[data-toggle="tooltip"]').tooltip()
    })
    //$(function () {
    //    $('[data-toggle="tooltip"]').tooltip()
    //})

    toastr.options = {
        "closeButton": false,
        "debug": false,
        "newestOnTop": false,
        "progressBar": false,
        "positionClass": "toast-top-right",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "2000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }

    if ($("#toast-message").length) {

        if ($("#toast-message").attr("data-success") == "true") {
            toastr.success($("#toast-message").attr("data-text"))
        }
        else {
            toastr.error($("#toast-message").attr("data-text"))
        }

    }

    $(".order-accept").on("click", function (e) {
        e.preventDefault();
        let note = $("#note").val()
        let url = $(this).attr("href") + "&note=" + note;
        fetch(url)
            .then(response => response.json())
            .then(data => {
                Swal.fire({
                    title: "Are you sure?",
                    text: "You won't be able to revert this!",
                    icon: "warning",
                    showCancelButton: true,
                    confirmButtonColor: "#3085d6",
                    cancelButtonColor: "#d33",
                    confirmButtonText: "Yes, accept this order!"
                }).then((result) => {
                    if (result.isConfirmed) {
                        if (data.status == 200) {
                            //location.reload(true)
                            window.location.href = "https://localhost:44390/manage/Order/index"
                        }
                        else {
                            Swal.fire({
                                title: "Not Found!",
                                text: "Your file has not been found.",
                                icon: "success"
                            });
                        }

                    }
                });
            })
    })

    //$(".item-delete").on("click", e => {
    //    e.preventDefault();
    //    Swal.fire({
    //        title: "Are you sure?",
    //        text: "You won't be able to revert this!",
    //        icon: "warning",
    //        showCancelButton: true,
    //        confirmButtonColor: "#3085d6",
    //        cancelButtonColor: "#d33",
    //        confirmButtonText: "Yes, delete it!"
    //    }).then((result) => {
    //        if (result.isConfirmed) {

    //            var url = $(this).attr("href")
    //            fetch(url)
    //                .then(response => response.json())    
    //                .then(data => {
    //                    if (data.status == 200) {
    //                        location.reload(true)

    //                    }
    //                    else {
    //                        Swal.fire({
    //                            title: "Not Found!",
    //                            text: "Your file has not been found.",
    //                            icon: "error"
    //                        });
    //                    }
    //                })




    //        }
    //    });
    //})

})