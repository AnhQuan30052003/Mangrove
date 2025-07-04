﻿// Xử lý khi click vào ảnh phần thông tin cây và hiển thị nó to hơn
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
		document.addEventListener("click", function (e) {
			if (e.target.matches(".click_show_qr") || e.target.matches(".fa-qrcode")) {
				const item = e.target;
				const frameQR = item.closest(".frame_qr");
				const showQR = frameQR.querySelector(".click_show_qr");

				const qrName = frameQR.querySelector(".get_qr_name");
				const qrPos = frameQR.querySelector(".get_qr_pos");
				const qrLongitude = frameQR.querySelector(".get_qr_longitude");
				const qrLatitude = frameQR.querySelector(".get_qr_latitude");

				const showQRCode = document.querySelector("#show_qr_code");
				showQRCode.querySelector(".qr_name").innerHTML = qrName.value;
				showQRCode.querySelector(".qr_pos").innerHTML = qrPos.value;

				const setQRLongitude = showQRCode.querySelector(".qr_longitude");
				if (qrLongitude.value != "") {
					setQRLongitude.innerHTML = qrLongitude.value;
				}
				else {
					const p = setQRLongitude.closest("p");
					p.classList.add("d-none");
				}

				const setQRLatitude = showQRCode.querySelector(".qr_latitude");
				if (qrLatitude.value != "") {
					setQRLatitude.innerHTML = qrLatitude.value;
				}
				else {
					const p = setQRLatitude.closest("p");
					p.classList.add("d-none");
				}

				showQRCode.querySelector(".qr_img").src = showQR.getAttribute("src");
				showQRCode.classList.remove("d-none");
			}
		});
	}
	catch { }
}
clickShowQR();

