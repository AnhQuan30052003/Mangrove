// Theo dõi cuộn màn hình nút quay lên #scrollUp
document.addEventListener("scroll", function () {
    const button = document.querySelector("#scrollUp");
    if (window.scrollY > window.innerHeight * 1 / 8) {
        button.classList.add("opacity")
    } else {
        button.classList.remove("opacity")
    }
});
