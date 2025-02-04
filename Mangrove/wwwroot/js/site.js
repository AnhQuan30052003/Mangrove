document.querySelector("#searchIcon").addEventListener("click", function () {
    document.querySelector(".search-form").classList.toggle("active");
});

document.querySelector(".closeIcon").addEventListener("click", function () {
    document.querySelector(".search-form").classList.remove("active");
});
