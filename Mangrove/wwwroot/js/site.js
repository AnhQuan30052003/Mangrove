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
const searchIcon = document.querySelector("#searchIcon");
searchIcon.addEventListener("click", function () {
    const searchFormUser = document.querySelector("#search_form_user");
    searchFormUser.classList.toggle("toggle_search_frame");

    const icon = searchIcon.querySelector(".icon");
    console.log(icon);
    if (icon.classList.contains("no-click")) {
        icon.classList.remove("no-click", "fa", "fa-search");
        icon.classList.add("fa-solid", "fa-x");
    }
    else {
        icon.classList.add("no-click", "fa", "fa-search");
        icon.classList.remove("fa-solid", "fa-x");
    }
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

// Theo dõi việc chọn ngôn ngữ và lưu lại 
// function listenerChangeLanguage() {
//     const selected = document.querySelector(".selected");
//     const options = document.querySelectorAll(".dropdown-menu .dropdown-item");

//     // Khi click vào một option
//     options.forEach(option => {
//         option.addEventListener("click", function () {
//             let value = this.getAttribute("data-value");
//             let text = this.textContent;
//             selected.textContent = text;

//             // Log change after click
//             console.log(`Ngôn ngữ: ${text}`);

//             // Gửi tới db để thay đổi trong phiên làm việc của người dùng
//             const xhr = new XMLHttpRequest();
//             const url = `/Home/SaveLanguageWebsite?lang=${value}`;
//             xhr.open("post", url, true);
//             xhr.setRequestHeader("REQUESTED", "AJAX");
//             xhr.onload = function () {
//                 if (xhr.status == 200) {
//                     location.reload();
//                 }
//             }
//             xhr.send();
//         });
//     });
// }

// listenerChangeLanguage();

// Ẩn mấy cái UI hiển thị dịch của google

const div = document.querySelector("#google_translate_element .skiptranslate");

console.log(div);
// Lọc bỏ các ký tự text, chỉ giữ lại các thẻ HTML
div.innerHTML = [...div.children].map(el => el.outerHTML).join('');
console.log(div);

// --
console.log("Run file site.js");

