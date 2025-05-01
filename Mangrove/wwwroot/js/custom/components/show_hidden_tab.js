// Theo dõi khi click vào tab giai đoạn của mỗi cây
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


// Theo dõi click vào tab khi chỉinh sửa nội dung trnag chung
function clickInfoOverview() {
	try {
		const optionEditor = document.querySelector(".option_editor");
		const options = optionEditor.querySelectorAll(".option");

		const frameContent = optionEditor.closest(".frame_content");
		const inforEditors = frameContent.querySelectorAll(".infor_editor")

		options.forEach((option) => {
			option.addEventListener("click", function () {
				// Thay đổi tab
				options.forEach((item) => item.classList.remove("active"));
				this.classList.add("active");

				// Thay đổi cờ quốc gia
				const frame = this.closest(".frame_editor");
				const img = frame.querySelector(".title img")
				const getImg = this.querySelector("img");
				img.src = getImg.src;

				// Thay đổi trình soạn thảo
				inforEditors.forEach((item) => item.classList.add("d-none"));
				const title = option.getAttribute("data");
				document.querySelector(`.${title}`).classList.remove("d-none");
			});
		});
	}
	catch { }
}
clickInfoOverview();