// Theo dõ đóng mắt mắt mật khẩu
function togglePassword() {
    const eyes = document.querySelectorAll(".icon_eye");
    if (!eyes) return;

    eyes.forEach((eye) => {
        eye.addEventListener("click", function (e) {
            const frame = this.closest(".frame_password");
            const input = frame.querySelector("input");
            const open = frame.querySelector(".icon_eye_open");
            const close = frame.querySelector(".icon_eye_close");
            open.classList.toggle("d-none");
            close.classList.toggle("d-none");
            input.setAttribute("type", input.getAttribute("type") == "password" ? "text" : "password");
        });
    });
}
togglePassword();