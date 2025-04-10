// Xử lý khi click vào ảnh phần thông tin cây và hiển thị nó to hơn
function imageToShow() {
	const body = document.body;
	body.addEventListener("click", function (event) {
		const clicked = event.target;
		let scale;

		if (clicked.matches(".click_show_image")) {
			try {
				const src = clicked.getAttribute("src");
				const showImage = document.querySelector("#click_show_image");
				const img = showImage.querySelector(".box_show_img img");
				img.src = src;
				showImage.classList.remove("d-none");
				document.body.style.overflow = "hidden";
				scale = 1;
				img.style.transform = `scale(${scale})`;

				// Zoom ảnh khi cuộn chuột
				showImage.addEventListener("wheel", function (e) {
					if (e.deltaY < 0) {
						scale += 0.1;
						if (scale >= 3) scale = 3;
					} else {
						scale -= 0.1;
						if (scale < 0.5) scale = 0.5;
					}
					img.style.transform = `scale(${scale})`;
				});
			}
			catch { }
		}
	});
}
imageToShow();

// Theo dõi việc toggle QR img
function clickButtonQR() {
	const listIndividual = document.querySelector(".list_individuals");
	if (!listIndividual) return;

	listIndividual.addEventListener("click", function (event) {
		const clicked = event.target;
		if (clicked.matches(".toggleQR")) {
			const frameQR = clicked.closest(".individual_item").querySelector(".frame_qr");
			if (frameQR) frameQR.classList.toggle("height-222");
		}
	});
}
clickButtonQR();

// Theo dõi click hiện thông tin QR
function clickShowQR() {
	try {
		const qrImgs = document.querySelectorAll(".click_show_qr");
		qrImgs.forEach((item) => {
			item.addEventListener("click", function () {
				const infoQR = document.querySelector(".info_qr");
				const qrPos = infoQR.querySelector(".qr_pos");
				const qrName = document.querySelector(".qr_name_mangrove");
				const qrLongitude = infoQR.querySelector(".qrLongitude");
				const qrLatitude = infoQR.querySelector(".qrLatitude");

				const showQRCode = document.querySelector("#show_qr_code");
				showQRCode.querySelector(".qr_name").innerHTML = qrName.value;
				showQRCode.querySelector(".qr_pos").innerHTML = qrPos.textContent;
				showQRCode.querySelector(".qr_longitude").innerHTML = qrLongitude.textContent;
				showQRCode.querySelector(".qr_latitude").innerHTML = qrLatitude.textContent;
				showQRCode.querySelector(".qr_img").src = item.getAttribute("src");
				showQRCode.classList.remove("d-none");
			});
		});
	}
	catch { }
}
clickShowQR();

