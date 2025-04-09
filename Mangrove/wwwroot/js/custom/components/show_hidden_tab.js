// Theo dõi khi click và tab giai đoạn của mỗi cây
function clickPreviodOfTree() {
	try {
		const infoStage = document.querySelector(".infor_stage");
		const tabItems = infoStage.querySelectorAll(".tab_item");
		const displayItems = infoStage.querySelectorAll(".display_item");

		for (let i = 0; i < tabItems.length; i++) {
			let tabItem = tabItems[i];
			tabItem.addEventListener("click", function () {
				tabItems.forEach((item) => item.classList.remove("active"));
				displayItems.forEach((item) => item.classList.add("d-none"));

				this.classList.add("active");
				displayItems[i].classList.remove("d-none");
			});
		}
	}
	catch { }
}
clickPreviodOfTree();

