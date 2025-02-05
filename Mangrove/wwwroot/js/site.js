// Theo dõi cuộn màn hình nút quay lên #scrollUp
document.addEventListener("scroll", function () {
    const button = document.querySelector("#scrollUp");
    if (window.scrollY > window.innerHeight * 1 / 8) {
        button.classList.add("opacity")
    } else {
        button.classList.remove("opacity")
    }
});

// Click icon search
document.querySelector("#searchIcon").addEventListener("click", function () {
    document.querySelector("#search_form_user").classList.toggle("toggle_search_frame");
});

// Theo dõi chuyển tab của btn-option search-form home
document.querySelector(".search-form .options").addEventListener("click", function (e) {
    const itemClick = e.target;
    if (itemClick.matches(".btn-option")) {
        const div_options = document.querySelectorAll(".search-form .options .btn-option");
        const results = document.querySelectorAll(".search_frame .result_search");

        for (let i = 0; i < div_options.length; i++) {
            div_options[i].classList.remove("active")
            results[i].classList.add("d-none")
        }

        itemClick.classList.add("active");
        document.querySelector(itemClick.classList.contains("btn_search_keyword") ? ".search_keyword" : ".search_advance").classList.remove("d-none")
    }
});

// document.querySelector("body").addEventListener("click", function (e) {
//     const iconSearch = e.target.matches("#searchIcon");
//     const formSearch = document.querySelector("#search_form_user .search-form");
//     if (!iconSearch && formSearch.classList.contains("toggle_search_frame")) {
//         formSearch.classList.toggle("toggle_search_frame");
//     }
// });