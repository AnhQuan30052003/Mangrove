// Theo dõi hiển thị page_hidden
function togglePageHidden() {
	document.addEventListener("click", function (e) {
		const toggle = document.querySelector(".toggle_page_hidden");
		if (toggle) {
			if (toggle.contains(e.target)) {
				const pageHidden = toggle.closest(".pagination").querySelector(".page_hidden");
				if (pageHidden) pageHidden.classList.toggle("d-none");
			}
		}
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

				const theads = document.querySelectorAll(".head_sort");
				const theadLast = theads[theads.length - 1];

				listOption.style.right = theadLast.clientWidth + "px";

				if (i > options.length / 2) {
					listOption.style.top = "-100px";
				}
				listOption.classList.toggle("d-none");
			}
		});
	}
	catch { }
}
whenClickOption();

