// Thêm ảnh vào item 
function addImageToItem() {
	document.addEventListener("click", function (e) {
		const clicked = e.target;

		// Khi click vào nút thêm ảnh
		if (clicked.matches(".btn_add_img") || clicked.matches(".icon_add")) {
			const addImg = clicked.closest(".add_img");
			const inputImg = addImg.querySelector(".input_img");
			const previewImg = addImg.querySelector(".preview_img");
			const file = inputImg.querySelector(".file");

			if (file) {
				file.click();
				file.addEventListener("change", function () {
					if (!file.files) return;

					const selected = file.files[0];
					const reader = new FileReader();

					const img = previewImg.querySelector(".show_temp");
					const imageType = inputImg.querySelector(".image_type");
					const imageData = inputImg.querySelector(".image_data");
					reader.onload = function (e) {
						img.src = e.target.result;
						imageType.value = selected.type;
						imageData.value = e.target.result;

						inputImg.classList.add("d-none");
						previewImg.classList.remove("d-none");
					}
					reader.readAsDataURL(selected);
				});
			}
		}

		// Khi click vào nút xoá ảnh
		if (clicked.matches(".btn_remove_preview_img") || clicked.matches(".icon_remove")) {
			const addImg = clicked.closest(".add_img");
			const inputImg = addImg.querySelector(".input_img");
			const previewImg = addImg.querySelector(".preview_img");

			const inputFile = inputImg.querySelector("input");
			const imageType = inputImg.querySelector(".image_type");
			const imageData = inputImg.querySelector(".image_data");

			inputImg.classList.remove("d-none");
			previewImg.classList.add("d-none");
			inputFile.value = imageData.value = imageType.value = "";
		}

		// Khi click vào xoá item
		if (clicked.matches(".btn_remove_item") || clicked.matches(".icon_remove_item")) {
			const addItem = clicked.closest(".add_item");
			addItem.remove();

			const quantity = document.querySelector(".quantity_item");
			quantity.innerHTML = parseInt(quantity.innerText) - 1;

			const inputFile = addItem.querySelector("input");
			const imageType = addItem.querySelector(".image_type");
			const imageData = addItem.querySelector(".image_data");
			inputFile.value = imageData.value = imageType.value = "";
		}
	});
}
addImageToItem();

// Khi click vào nút thêm item
export function addItem(idButtonClick, maxItem = null) {
	const clicked = document.querySelector(idButtonClick)
	const frameItems = clicked.closest(".frame_items");
	const items = frameItems.querySelector(".items");
	const addItemFind = items.querySelectorAll(".add_item");

	// Xử lý giới hạn item
	maxItem = maxItem ?? 10;
	if (addItemFind.length == maxItem) return;

	const addItem = createDivAddItem();
	items.appendChild(addItem);

	const quantity = frameItems.querySelector(".quantity_item");
	quantity.innerHTML = addItemFind.length + 1;
}

function createDivAddItem() {
	const addItem = document.createElement("div");
	addItem.className = "add_item p-2 mb-3 rounded-1 d-flex flex-wrap gap-1 gap-lg-0 position-relative border_detail";

	const btnRemoveItem = document.createElement("button");
	btnRemoveItem.className = "btn_remove_item";
	btnRemoveItem.type = "button";

	const iconRemoveItem = document.createElement("i");
	iconRemoveItem.className = "icon_remove_item fa-solid fa-xmark";
	btnRemoveItem.appendChild(iconRemoveItem);

	const addImg = document.createElement("div");
	addImg.className = "add_img col-12 col-lg-6 add_img_min_height rounded-1";

	const photo = document.createElement("p");
	photo.className = "text-center text-black";
	photo.innerText = document.querySelector("#photo").value;

	const inputImg = document.createElement("div");
	inputImg.className = "input_img bg-white d-flex justify-content-center align-items-center min_height_input_img rounded-1 green_effect";

	const btnAddImg = document.createElement("button");
	btnAddImg.className = "btn_add_img outline-none color-tree bg-transparent fs-1 px-4 py-2 border rounded-1 green_effect";
	btnAddImg.type = "button";

	const iconAdd = document.createElement("i");
	iconAdd.className = "icon_add fa-solid fa-plus";

	const inputFile = document.createElement("input");
	inputFile.className = "file";
	inputFile.type = "file";
	inputFile.hidden = true;
	inputFile.name = "ImageFile"

	const div = document.createElement("div");
	div.className = "overflow-hidden w-h-0";

	const image_type = document.createElement("input");
	image_type.className = "image_type";
	image_type.name = "dataTypes";

	const image_data = document.createElement("input");
	image_data.className = "image_data";
	image_data.name = "dataBase64s";

	div.appendChild(image_type);
	div.appendChild(image_data);

	btnAddImg.appendChild(iconAdd);

	inputImg.appendChild(btnAddImg);
	inputImg.appendChild(inputFile);
	inputImg.appendChild(div);

	const previewImg = document.createElement("div");
	previewImg.className = "preview_img position-relative d-none";

	const img = document.createElement("img");
	img.className = "show_temp object-fit-cover mx-auto d-block rounded-1 click_show_image img_max_height";
	img.setAttribute("name", "ImageName");

	const btnRemovvPreviewImg = document.createElement("button")
	btnRemovvPreviewImg.className = "btn_remove_preview_img";
	btnRemovvPreviewImg.type = "button";

	const iconRemove = document.createElement("I");
	iconRemove.className = "icon_remove fa-solid fa-xmark";

	btnRemovvPreviewImg.appendChild(iconRemove);
	previewImg.appendChild(img);
	previewImg.appendChild(btnRemovvPreviewImg);

	addImg.appendChild(photo);
	addImg.appendChild(inputImg);
	addImg.appendChild(previewImg);

	const addContent = document.createElement("div");
	addContent.className = "add_content col-12 col-lg-6 d-lg-flex flex-lg-wrap p-2";

	const description = document.createElement("p");
	description.className = "text-center col-12 align-self-start text-black";
	description.innerText = document.querySelector("#description").value;

	const addContentItem = document.createElement("div");
	addContentItem.className = "add_content_item mt-1 col-12";

	const smallTite = document.createElement("small");
	smallTite.className = "mb-1 d-block font-small";

	const inputText = document.createElement("input");
	inputText.className = "w-100 border-none bg-white green_effect px-2 py-1 rounded-1 text_body";
	inputText.type = "text";

	const inputTextEN = inputText.cloneNode(false);
	inputTextEN.name = "noteENs";

	const inputTextVI = inputText.cloneNode(false);
	inputTextVI.name = "noteVIs";

	const smallEN = smallTite.cloneNode(false);
	smallEN.textContent = document.querySelector("#english").value;

	const smallVI = smallTite.cloneNode(false);
	smallVI.textContent = document.querySelector("#vietnamese").value;

	const addContentItemEN = addContentItem.cloneNode(false);
	addContentItemEN.appendChild(smallEN);
	addContentItemEN.appendChild(inputTextEN);

	const addContentItemVI = addContentItem.cloneNode(false);
	addContentItemVI.appendChild(smallVI);
	addContentItemVI.appendChild(inputTextVI);

	addContent.appendChild(description);
	addContent.appendChild(addContentItemEN);
	addContent.appendChild(addContentItemVI);

	addItem.appendChild(btnRemoveItem);
	addItem.appendChild(addImg);
	addItem.appendChild(addContent);

	return addItem;
}

// Theo dõi khi click và tab giai đoạn của mỗi cây bên page admin
function clickChangeStageTab() {
	document.addEventListener("click", function (e) {
		const clicked = e.target;
		if (clicked.matches(".change_tab")) {
			const tabs = clicked.closest(".tabs");
			const changeTabs = tabs.querySelectorAll(".change_tab");			

			// Highlight tab click
			changeTabs.forEach((item) => item.classList.remove("active"));
			clicked.classList.add("active");

			// Run fun thay đổi display_item
		}
	});
}
clickChangeStageTab();

// Add item stage 
function addStage() {
	const btn = document.querySelector(".btn_add_stage");
	btn.addEventListener("click", function () {
		const frameTab = this.closest(".frame_tab");
		const tabs = frameTab.querySelector(".tabs");
		const tabItemSpan = tabs.querySelectorAll(".tab_item");

		const maxStage = btn.getAttribute("max-stage");
		if (tabItemSpan.length >= maxStage) return;

		const lastTab = tabItemSpan[tabItemSpan.length - 1];
		let index = parseInt(lastTab.getAttribute("data-tab")) + 1;

		// Tạo item stage - span
		const span = document.createElement("span");
		span.className = "tab_item change_tab stage_individual py-2 px-3 bg-white rounded-circle cursor-pointer";
		span.setAttribute("data-tab", index);
		span.innerHTML = index;

		tabs.appendChild(span);
	});
}
addStage();