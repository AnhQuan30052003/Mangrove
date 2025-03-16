// Theo dõi huỷ khi click ngoài đối tượng
function cancelActionWhenClickOut() {
	// Event Click
	document.addEventListener("click", function (event) {
		// Huỷ khi mở phóng to ảnh
		try {
			const showImg = document.querySelector("#click_show_image");
			const img = showImg.querySelector(".box_show_img .img");
			const btnShowImgCancel = showImg.querySelector(".btnShowImgCancel");
			if (!showImg.classList.contains("d-none") && showImg.contains(event.target) && (!img.contains(event.target) || btnShowImgCancel.contains(event.target))) {
				img.style.cursor = "default";
				showImg.classList.add("d-none");
				document.body.style.overflow = "auto";
			}
		}
		catch { }

		// Huỷ mở menu hamburger
		try {
			const btnMenu = document.querySelector(".btn_menu");
			const iconS = btnMenu.querySelector(".icon-s");
			const listMenu = document.querySelector(".list_menu");

			if (!listMenu.contains(event.target) && !btnMenu.contains(event.target) && iconS.classList.contains("d-none")) {
				iconS.classList.remove("d-none");
				btnMenu.querySelector(".icon-x").classList.add("d-none");
				listMenu.classList.remove("right-0");
				document.body.style.overflow = "auto";

				const bg = document.querySelector("#background");
				bg.classList.remove("w-100");
			}
		}
		catch { }

		// Huỷ mở QR code
		try {
			const showQRCode = document.querySelector("#show_qr_code");
			const wrapper = showQRCode.querySelector(".wrapper");
			const btnQRCancel = showQRCode.querySelector(".btnQR_cancel");

			if (!showQRCode.classList.contains("d-none") && showQRCode.contains(event.target) && (!wrapper.contains(event.target) || btnQRCancel.contains(event.target))) {
				showQRCode.classList.add("d-none");
			}
		}
		catch { }
	});

	// Event Keydown
	document.addEventListener("keydown", function (e) {
		try {
			if (e.key == "Escape") {
				const showImg = document.querySelector("#click_show_image");
				const img = showImg.querySelector(".box_show_img .img");
				if (!showImg.classList.contains("d-none")) {
					img.style.cursor = "default";
					showImg.classList.add("d-none");
					document.body.style.overflow = "auto";
				}

				const showQRCode = document.querySelector("#show_qr_code");
				if (!showQRCode.classList.contains("d-none")) {
					showQRCode.classList.add("d-none");
				}
			}
		}
		catch { }
	});
}
cancelActionWhenClickOut();