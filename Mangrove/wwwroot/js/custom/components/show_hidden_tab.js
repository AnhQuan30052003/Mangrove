// Theo dõi khi click và tab giai đoạn của mỗi cây
function clickPreviodOfTree() {
	try {
		const individualDisplay = document.querySelector(".individual_display");
		const tabs = individualDisplay.querySelectorAll(".tab")
		const displayItems = individualDisplay.querySelectorAll(".display_item");

		tabs.forEach((tab) => {
			tab.addEventListener("click", function (e) {
				tabs.forEach((item) => item.classList.remove("active"));
				this.classList.add("active");

				const idTab = this.getAttribute("tab");
				displayItems.forEach((item) => item.classList.add("d-none"));
				displayItems[idTab].classList.remove("d-none");
			});
		})
	}
	catch { }
}
clickPreviodOfTree();

