//const { data } = require("jquery");

$(document).ready(function () {
    $(".show-book-modal").on("click", function (e) {
        e.preventDefault();
        const url = $(this).attr("href")
        fetch(url)
            .then(response => response.text())//oartial view
            .then(data => {
                console.log(data)
         
                $("#quickModal .product-details-modal").html(data);
            });
            $("#quickModal").modal("show")
    })
    //$(".addtobasket").on("click", e => {
    //    e.preventDefault();
    //    let url = $(this).attr("href")
    //    console.log(url)
    //    fetch(url)
    //        .then(response => response.text())
    //        .then(data => {
    //            console.log(data)
    //            $(".mybasket").html(data)
    //        })
    //})


    //$("#loadcomment").on("click", e => {
    //    e.preventDefault()
        
    //    let page = $(this).attr("dataPage")
    //    let url = $(this).attr("href") + "&page=" + page
    //    fetch(url)
    //        .then(response => response.text())
    //        .then(data => {
    //            $("#comments").html(data)
    //        })
    //    const maxPage=$(this).attr("max-Page")
    //    if (maxPage==page) {
    //        $(this).remove()
    //    }
    //    page = +page + 1


    //    $(this).attr("dataPage",page)




    //})


    //$(document).on("click", "show-book-modal", function (e) {
    //    e.preventDefault();

    //    var url=$(this).attr("href")

    //    fetch(url)
    //        .then(response => response.json())
    //        .then(data => console.log(data));

    //    //console.log("clicked")
    //    //$("#quickModal").modal("show")
    //})


    //$(document).on("click", "#loadcomment", function (e) {
    //    e.preventDefault();
    //    var url = $(this).attr("href") + "&page" + $(this).attr("nextPage");
    //    fetch(url)
    //        .then(response => response.json())
    //        .then(data => console.log(data));
    //    //$(this).attr("href")
    //})

    //$(document).on("click", "#loadcomment", function (e) {
    //    e.preventDefault();
    //    var nextPage = $(this).attr("dataPage");
    //    var url = $(this).attr("href") + "&page=" + nextPage;
    //    console.log(nextPage)
    //    fetch(url)
    //        .then(response => response.json())
    //        .then(data => {
    //            for (var i = 0; i < data.data.length; i++) {
    //                console.log(data.data[i].text)
    //            }
    //        })
    //    nextPage = +nextPage + 1
    //    $(this).attr("dataPage", nextPage)
    //    //console.log(url)
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
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }
    if ($("#order-toast-message").length) {
        if ($("#order-toast-message").attr("data-succeded") == "true") {
            toastr.success($("#order-toast-message").attr("data-text"),"Success")
            // toastr["success"]($("#order-toast-message").attr("data-text"))
            let l = $("#order-toast-message").length;
            console.log(l)
        }
        else
        {
            toastr.error($("#order-toast-message").attr("data-text"),"Error")
            //toastr["error"]($("#order-toast-message").attr("data-text"))
        }

    }

   
})

