// Theo dõi hiển thị page_hidden
function togglePageHidden() {
	document.addEventListener("click", function (e) {
		const toggles = document.querySelectorAll(".toggle_page_hidden");
		toggles.forEach((toggle) => {
			if (toggle.contains(e.target)) {
				const pageHidden = toggle.closest(".pagination").querySelector(".page_hidden");
				if (pageHidden) pageHidden.classList.toggle("d-none");
			}
		});
	});
}
togglePageHidden();

// Theo dõi click ... tuỳ chọn
function whenClickOption() {
	try {
		document.addEventListener("click", function (e) {
			const options = document.querySelectorAll(".btn_toggle_option");
			for (let i = 0; i < options.length; i++) {
				const item = options[i];
				const listOption = item.closest("td").querySelector(".list_option");
				if (!item.contains(e.target)) {
					listOption.classList.add("d-none");
					continue;
				}

				const posItem = item.getBoundingClientRect();
				const td = item.closest("td");
				const posTd = td.getBoundingClientRect();

				listOption.style.top = posItem.top + "px";
				listOption.style.right = posTd.width + "px";
				listOption.classList.toggle("d-none");
			}
		});
	}
	catch { }
}
whenClickOption();

// Ẩn list_option khi click ra ngoài hoặc là scroll page
document.addEventListener("scroll", function () {
	const listOptions = document.querySelectorAll(".list_option");
	listOptions.forEach((listOption) => {
		listOption.classList.add("d-none");
	});
});