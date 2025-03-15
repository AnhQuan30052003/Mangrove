// Theo dõi hiển thị page_hidden
function togglePageHidden() {
	document.addEventListener("click", function (e) {
		const toggle = document.querySelector(".toggle_page_hidden");
		if (toggle) {
			if (toggle.contains(e.target)) {
				const pageHidden = document.querySelector(".page_hidden");
				if (pageHidden) pageHidden.classList.toggle("d-none");
			}
		}
	});
}
togglePageHidden();

// Theo dõi click ... tuỳ chọn
function whenClickOption() {
	try {
		const options = document.querySelectorAll(".btn_toggle_option");
		options.forEach((item) => {
			item.addEventListener("click", function () {
				const listOption = item.closest("td").querySelector(".list_option");
				listOption.classList.toggle("d-none");

				options.forEach((option) => {
					if (option != item) {
						const listOption = option.closest("td").querySelector(".list_option");
						listOption.classList.add("d-none");
					}
				});
			});
		});
	}
	catch { }
}
whenClickOption();

