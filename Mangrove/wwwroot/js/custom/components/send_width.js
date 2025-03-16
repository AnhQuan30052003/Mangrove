//// Theo dõi thay đổi cửa số
//function listenrChangeWidthScreen() {
//	window.addEventListener("resize", function () {
//		console.log("Width change is " + screen.width);
//		const havePaginate = document.querySelector(".have_paginate");
//		if (havePaginate) {
//			setTimeout(() => {
//				location.reload();
//			}, 1000);
//		}
//	});
//}
//listenrChangeWidthScreen();

//// Gửi width từ js qua C#
//function sendWidth() {
//	const width = screen.width;
//	console.log("Width gửi: " + width);
//	document.cookie = `WidthScreen=${width}; path=/; max-age=86400`;
//}
//sendWidth();