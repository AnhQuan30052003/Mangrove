// Theo dõi khi click và tab giai đoạn của mỗi cây
function clickPreviodOfTree() {
	try {
		const displayTabStage = document.querySelector(".display_tab_stage");
		const tabs = displayTabStage.querySelectorAll(".tab")
		const displayItems = displayTabStage.querySelectorAll(".display_item");

		tabs.forEach((tab) => {
			tab.addEventListener("click", function (e) {
				tabs.forEach((item) => item.classList.remove("active"));
				this.classList.add("active");

				const idTab = this.getAttribute("data-tab");
				displayItems.forEach((item) => item.classList.add("d-none"));
				displayItems[idTab-1].classList.remove("d-none");
			});
		})
	}
	catch { }
}
clickPreviodOfTree();

