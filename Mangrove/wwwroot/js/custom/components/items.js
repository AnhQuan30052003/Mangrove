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

					const reader = new FileReader();
					reader.onload = function (e) {
						const img = previewImg.querySelector(".show_temp");
						img.src = e.target.result;
					}
					reader.readAsDataURL(file.files[0]);


					inputImg.classList.add("d-none");
					previewImg.classList.remove("d-none");
				});
			}
		}

		// Khi click vào nút xoá ảnh
		if (clicked.matches(".btn_remove_preview_img") || clicked.matches(".icon_remove")) {
			const addImg = clicked.closest(".add_img");
			const inputImg = addImg.querySelector(".input_img");
			const previewImg = addImg.querySelector(".preview_img");
			const file = addImg.querySelector(".file");

			inputImg.classList.remove("d-none");
			previewImg.classList.add("d-none");
			file.value = "";
		}

		// Khi click vào xoá item
		if (clicked.matches(".btn_remove_item") || clicked.matches(".icon_remove_item")) {
			const addItem = clicked.closest(".add_item");
			const file = addItem.querySelector(".file");
			file.value = "";
			addItem.remove();

			const quantity = document.querySelector(".quantity_item");
			quantity.innerHTML = parseInt(quantity.innerText) - 1;
		}
	});
}
addImageToItem();

// Khi click vào nút thêm item
export function addItem() {
	const items = document.querySelector(".items");
	const addItemFind = items.querySelectorAll(".add_item");
	if (addItemFind.length == 10) return;

	const btn = document.querySelector(".btn_add_item");
	const att1 = btn.getAttribute("att1");
	const att2 = btn.getAttribute("att2");

	const addItem = createDivAddItem(addItemFind.length, att1, att2);
	items.appendChild(addItem);

	const quantity = document.querySelector(".quantity_item");
	quantity.innerHTML = addItemFind.length + 1;
}

function createDivAddItem(index, attribute1, attribute2) {
	const addItem = document.createElement("div");
	addItem.className = "add_item mb-3 rounded-1 d-flex flex-wrap gap-1 gap-lg-0 position-relative";

	const btnRemoveItem = document.createElement("button");
	btnRemoveItem.className = "btn_remove_item";
	btnRemoveItem.type = "button";

	const iconRemoveItem = document.createElement("i");
	iconRemoveItem.className = "icon_remove_item fa-solid fa-xmark";
	btnRemoveItem.appendChild(iconRemoveItem);

	const addImg = document.createElement("div");
	addImg.className = "add_img col-12 col-lg-6 add_img_height rounded-1 p-2";

	const photo = document.createElement("p");
	photo.className = "text-center";
	photo.innerText = document.querySelector("#photo").value;

	const inputImg = document.createElement("div");
	inputImg.className = "input_img bg-white d-flex justify-content-center align-items-center";
	inputImg.style.height = "calc(100% - 27px)";

	const btnAddImg = document.createElement("button");
	btnAddImg.className = "btn_add_img outline-none color-tree bg-transparent fs-1 px-4 py-2 border rounded-1";
	btnAddImg.type = "button";

	const iconAdd = document.createElement("i");
	iconAdd.className = "icon_add fa-solid fa-plus";

	const inputFile = document.createElement("input");
	inputFile.className = "file";
	inputFile.type = "file";
	inputFile.hidden = true;
	inputFile.name = "ImageFile"

	btnAddImg.appendChild(iconAdd);
	inputImg.appendChild(btnAddImg);
	inputImg.appendChild(inputFile);

	const previewImg = document.createElement("div");
	previewImg.className = "preview_img position-relative d-none";
	previewImg.style.height = "calc(100% - 27px)";

	const img = document.createElement("img");
	img.className = "show_temp object-fit-cover mx-auto d-block rounded-1 click_show_image";
	img.setAttribute("name", "ImageName");
	img.style.maxHeight = "200px";

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
	addContent.className = "add_content col-12 col-lg-6 d-lg-flex flex-lg-wrap align-items-lg-center p-2";

	const description = document.createElement("p");
	description.className = "text-center col-12 align-self-start";
	description.innerText = document.querySelector("#description").value;

	const addContentItem = document.createElement("div");
	addContentItem.className = "add_content_item mt-1 m-lg-0 col-12";

	const smallTite = document.createElement("small");
	smallTite.className = "mb-1 d-block font-small";

	const inputText = document.createElement("input");
	inputText.className = "w-100 border-none bg-white green_effect px-2 py-1 rounded-1";
	inputText.type = "text";
	inputText.name = "";

	const inputTextEN = inputText.cloneNode(false);
	inputTextEN.name = `[${index}].${attribute1}`;

	const inputTextVI = inputText.cloneNode(false);
	inputTextVI.name = `[${index}].${attribute2}`;

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
