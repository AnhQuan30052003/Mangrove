// Toggle info title
function toggleInfoTitle() {
	const toggleTitles = document.querySelectorAll(".toggle_title");
	toggleTitles.forEach((item) => {
		item.addEventListener("click", function () {
			const info = item.closest(".box").querySelector(".info");
			if (info) info.classList.toggle("d-none");
		});
	});
}
toggleInfoTitle();

// Theo dõi mở rộng/thu gọn nội dung trong result
function toggleExpandOrCollapseContentResult() {
	const selects = document.querySelectorAll(".select_ex_co");
	selects.forEach((item) => {
		item.addEventListener("change", function () {
			// Đồng bộ 2 select mở rộng/thu gọn
			selects.forEach((select) => {
				if (select !== item) {
					select.value = item.value;
				}
			});

			// Xử lý sự kiện
			const infos = document.querySelectorAll(".box .info")
			if (item.value == "ex") {
				infos.forEach((info) => {
					//info.classList.remove("box_collapse");
					info.classList.remove("d-none");
				});
			}
			else {
				infos.forEach((info) => {
					//info.classList.add("box_collapse");
					info.classList.add("d-none");
				});
			}
		});
	});
}
toggleExpandOrCollapseContentResult();
