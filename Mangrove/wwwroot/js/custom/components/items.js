// Thêm ảnh vào item (kéo thả)
function listenrDragover() {
	document.addEventListener("dragover", function (e) {
		const elementDragover = e.target;
		if (elementDragover.matches(".drop-zone") || elementDragover.matches(".btn_add_imge") || elementDragover.matches(".icon_add")) {
			// Ngăn chặn hành vi mặc định của trình duyệt
			['dragenter', 'dragover', 'dragleave', 'drop'].forEach((eventName) => {
				elementDragover.addEventListener(eventName, (e) => e.preventDefault());
			});

			elementDragover.addEventListener('dragover', (e) => elementDragover.classList.add('dragover'));
			elementDragover.addEventListener('dragleave', (e) => elementDragover.classList.remove('dragover'));

			elementDragover.addEventListener('drop', (e) => {

				const addImg = elementDragover.closest(".add_img");
				const inputImg = addImg.querySelector(".input_img");
				const previewImg = addImg.querySelector(".preview_img");

				elementDragover.classList.remove('dragover');
				const file = e.dataTransfer.files[0];
				if (file) {
					// Định dạng cho phép
					const allowedTypes = ['image/jpeg', 'image/png'];
					const maxSizeMB = parseInt(document.querySelector("#maxSizeMB").value);

					// Kiểm tra định dạng
					if (!allowedTypes.includes(file.type)) {
						const text = document.querySelector("#textImageNotAllow").value;
						alert(text);
						file.value = "";
						return;
					}

					// Kiểm tra dung lượng
					if (file.size > maxSizeMB * 1024 * 1024) {
						const text = document.querySelector("#textMaxSize").value;
						alert(`${text} ${maxSizeMB}MB !`);
						file.value = "";
						return;
					}

					// Nếu file hợp lệ, hiển thị ảnh
					const reader = new FileReader();

					const img = previewImg.querySelector(".show_temp");
					const imageType = inputImg.querySelector(".image_type");
					const imageData = inputImg.querySelector(".image_data");
					reader.onload = () => {
						img.src = reader.result;
						imageType.value = file.type;
						imageData.value = reader.result;

						inputImg.classList.add("d-none");
						previewImg.classList.remove("d-none");
					};
					reader.readAsDataURL(file);
				}
			});
		}
	});
}
listenrDragover();

// Thêm ảnh vào item (click chọn)
function listenerAddImageToItem() {
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

					// Định dạng cho phép
					const allowedTypes = ['image/jpeg', 'image/png'];
					const maxSizeMB = parseInt(document.querySelector("#maxSizeMB").value);

					// Kiểm tra định dạng
					if (!allowedTypes.includes(selected.type)) {
						const text = document.querySelector("#textImageNotAllow").value;
						alert(text);
						this.value = '';
						return;
					}

					// Kiểm tra dung lượng
					if (selected.size > maxSizeMB * 1024 * 1024) {
						const text = document.querySelector("#textMaxSize").value;
						alert(`${text} ${maxSizeMB}MB !`);
						this.value = '';
						return;
					}

					// Nếu file hợp lệ, hiển thị ảnh
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

			const frameItems = addItem.closest(".frame_items");
			const quantity = frameItems.querySelector(".quantity_item");
			if (quantity) {
				let _value = parseInt(quantity.innerText) - 1;
				quantity.innerHTML = _value > 0 ? _value : 0;
			}

			const inputFile = addItem.querySelector("input");
			const imageType = addItem.querySelector(".image_type");
			const imageData = addItem.querySelector(".image_data");
			inputFile.value = imageData.value = imageType.value = "";

			// Thêm só lượng item photo bên manager (create, edit) individual
			const itemPhotoOfStages = frameItems.querySelector(".itemPhotoOfStages");
			if (itemPhotoOfStages) {
				itemPhotoOfStages.value = parseInt(itemPhotoOfStages.value) - 1;
			}
			addItem.remove();
		}

		// Khi lick xoá giai đoạn
		if (clicked.matches(".remove_tab") || clicked.matches(".icon_remove_tab")) {
			const tabItem = clicked.closest(".tab_item");
			const dataTabDelete = tabItem.getAttribute("data-tab");

			// Xoá display item của stage
			const displayItems = document.querySelectorAll(".display_item");
			displayItems.forEach((item) => {
				if (item.getAttribute("data-tab") == dataTabDelete) {
					item.remove();
					return;
				}
			});

			tabItem.remove();
		}
	});
}
listenerAddImageToItem();

// Khi click vào nút thêm item
export function addItem(idButtonClick, haveFocus = false) {
	const clicked = document.querySelector(idButtonClick);
	const frameItems = clicked.closest(".frame_items");
	const items = frameItems.querySelector(".items");
	const addItemFind = items.querySelectorAll(".add_item");

	// Xử lý giới hạn item
	const maxItem = document.querySelector("#maxItem");
	if (maxItem && addItemFind.length == maxItem.value) return;

	const addItem = createDivAddItem(haveFocus);
	items.appendChild(addItem);

	const quantity = frameItems.querySelector(".quantity_item");
	if (quantity) {
		quantity.innerHTML = addItemFind.length + 1;
	}

	// Thêm só lượng item photo bên manager (create, edit) individual
	const itemPhotoOfStages = frameItems.querySelector(".itemPhotoOfStages");
	if (itemPhotoOfStages) {
		itemPhotoOfStages.value = parseInt(itemPhotoOfStages.value) + 1;
	}
}

function createDivAddItem(haveFocus = false) {
	const htmlFocus = haveFocus ? `<small class="text-danger">✶</small>` : ``;

	const labelPhoto = document.querySelector("#photo").value;
	const labelDescription = document.querySelector("#description").value;
	const labelEnglish = document.querySelector("#english").value;
	const labelVietnamese = document.querySelector("#vietnamese").value;

	const addItem = document.createElement("div");
	addItem.className = "add_item p-2 mb-3 rounded-1 d-flex flex-wrap gap-1 gap-lg-0 position-relative border_detail";
	addItem.innerHTML = `
		<button class="btn_remove_item" type="button">
			<i class="icon_remove_item fa-solid fa-xmark"></i>
		</button>

		<div class="add_img col-12 col-lg-6 rounded-1 d-flex flex-column">
			<p class="catch_to_move text-center text-black"><small class="text-danger" >✶</small>${labelPhoto}</p>

			<div class="drop-zone input_img bg-white d-flex justify-content-center align-items-center min_height_input_img rounded-1 green_effect flex-grow-1">
				<button class="btn_add_img outline-none color-tree bg-transparent fs-1 px-4 py-2 border rounded-1 green_effect"
						type="button">
					<i class="icon_add fa-solid fa-plus"></i>
				</button>
				<input class="file" name="ImageFile" type="file" hidden />
				<div class="overflow-hidden w-h-0">
					<input class="image_type" name="dataTypes" />
					<input class="image_data" name="dataBase64s" />
				</div>
			</div>

			<div class="preview_img position-relative d-none">
				<img class="show_temp object-fit-cover mx-auto d-block rounded-1 click_show_image img_max_height"
						alt="" />

				<button class="btn_remove_preview_img"
						type="button">
					<i class="icon_remove fa-solid fa-xmark"></i>
				</button>
			</div>
		</div>

		<div class="add_content col-12 col-lg-6 d-lg-flex flex-lg-wrap p-2">
			<p class="text-center col-12 align-self-start text-black">${labelDescription}</p>

			<div class="add_content_item py-2 col-12">
				<small class="mb-1 d-block font-small">${htmlFocus}${labelEnglish}</small>
				<input type="text" name="noteENs" class="w-100 border-none bg-white green_effect px-2 py-1 rounded-1 text_body" />

			</div>

			<div class="add_content_item py-2 col-12">
				<small class="mb-1 d-block font-small">${htmlFocus}${labelVietnamese}</small>
				<input type="text" name="noteVIs" class="w-100 border-none bg-white green_effect px-2 py-1 rounded-1 text_body" />
			</div>
		</div>
	`;

	return addItem;
}

// Theo dõi khi click và tab giai đoạn của mỗi cây bên page admin
function clickChangeStageTab() {
	document.addEventListener("click", function (e) {
		//try {
		const tabItems = document.querySelectorAll(".tab_item");
		tabItems.forEach((tabItem) => {
			if (tabItem.contains(e.target)) {
				tabItems.forEach((item) => {
					item.classList.remove("active");
					const activeStages = item.querySelector(".activeStages");
					if (activeStages) {
						activeStages.value = "";
					}
				});

				tabItem.classList.add("active");
				const activeStages = tabItem.querySelector(".activeStages");
				if (activeStages) {
					activeStages.value = "active";
				}

				// Run fun thay đổi display_item
				const indexTab = parseInt(tabItem.innerText) - 1;
				const displayItems = document.querySelectorAll(".display_item");
				for (let i = 0; i < displayItems.length; i++) {
					var item = displayItems[i];
					if (i == indexTab) {
						item.classList.remove("d-none");
					}
					else {
						item.classList.add("d-none");
					}
				}
			}
		});
		//}
		//catch { }
	});
}
clickChangeStageTab();

// Add item stage 
function addStage() {
	try {
		const btn = document.querySelector(".btn_add_stage");
		btn.addEventListener("click", function () {
			// Thiết lập số lượng max tạo ra
			const maxStage = document.querySelector("#maxStage").value;

			const frameTab = this.closest(".frame_tab");
			const tabs = frameTab.querySelector(".tabs");

			// Ẩn các tab item kia và lấy chỉ số index - data tab
			let listIndex = [];
			const tabItems = tabs.querySelectorAll(".tab_item");
			if (tabItems.length >= maxStage) return;
			tabItems.forEach((item) => {
				const getDataTab = item.getAttribute("data-tab");
				listIndex.push(parseInt(getDataTab));
				item.classList.remove("active");
				const activeStages = item.querySelector(".activeStages");
				activeStages.value = "";
			});

			// Xử lý chỉ số index
			let index = -1;
			listIndex.sort();
			for (let i = 0; i < listIndex.length; i++) {
				if (listIndex[i] != i + 1) {
					index = i + 1;
					break;
				}
			}

			if (index == -1) index = tabItems.length + 1;

			// Tạo tab item mới
			const newTabItem = document.createElement("span");
			newTabItem.setAttribute("data-tab", index);
			newTabItem.className = "tab_item change_tab stage_individual py-2 px-3 bg-white rounded-circle cursor-pointer position-relative active";
			newTabItem.innerHTML = `
				<span class="value_tab">${index}</span>
				<span class="remove_tab text-danger position_remove_tab">
					<i class="icon_remove_tab fa-solid fa-xmark"></i>
				</span>

				<input class="indexStages" type="text" name="indexStages" value="${index}" hidden />
				<input class="activeStages" type="text" name="activeStages" value="active" hidden />
				<input class="idStages" type="text" name="idStages" value="" hidden />
			`;
			tabs.appendChild(newTabItem);

			// Add frame input stage
			const displayInfoStage = document.querySelector(".display_info_stage");
			const displayItems = displayInfoStage.querySelectorAll(".display_item");
			displayItems.forEach((item) => item.classList.add("d-none"));

			const newDisplayItem = createDisplayItem(index);
			displayInfoStage.appendChild(newDisplayItem);
		});
	}
	catch { }
}
addStage();

function createDisplayItem(index) {
	const detailImageStage = document.querySelector("#detailImageStage").value;
	const labelPhoto = document.querySelector("#photo").value;
	const labelBtnAddPhoto = document.querySelector("#btnAddPhoto").value;
	const labelDescription = document.querySelector("#description").value;
	const labelEnglish = document.querySelector("#english").value;
	const labelVietnamese = document.querySelector("#vietnamese").value;
	const maxItem = document.querySelector("#maxItem").value;
	const surveyDay = document.querySelector("#surveyDay").value;
	const stageName = document.querySelector("#stageName").value;
	const weather = document.querySelector("#weather").value;
	const height = document.querySelector("#height").value;
	const perimeter = document.querySelector("#perimeter").value;

	// Create today
	const today = new Date();
	const year = today.getFullYear();
	const month = String(today.getMonth() + 1).padStart(2, "0");
	const day = String(today.getDate()).padStart(2, "0");

	const dodayStr = `${year}-${month}-${day}`;

	const displayItem = document.createElement("div");
	displayItem.className = "display_item box";
	displayItem.setAttribute("data-tab", index);

	const frameInfoAndPosition = document.createElement("div");
	frameInfoAndPosition.className = "rame_info_and_position d-sm-flex";
	frameInfoAndPosition.innerHTML = `
		<div class="top col-sm-6 p-2 d-flex flex-column justify-content-between">
			<div class="py-2 d-flex flex-column flex-grow-1">
				<small class="mb-1 d-block font-small text-center"><small class="text-danger">✶</small>${labelPhoto}</small>
				<div class="w-100 flex-grow-1">
					<div class="add_img rounded-1 w-100 h-100">
						<div class="drop-zone input_img bg-white d-flex justify-content-center align-items-center w-100 h-100 min_height_input_img rounded-1 green_effect">
							<button class="btn_add_img outline-none color-tree bg-transparent fs-1 px-4 py-2 border rounded-1 green_effect"
									type="button">
								<i class="icon_add fa-solid fa-plus"></i>
							</button>
							<input class="file" name="ImageFile" type="file" hidden />
							<div class="overflow-hidden w-h-0">
								<input class="image_type" name="dataTypes" />
								<input class="image_data" name="dataBase64s" />
							</div>
						</div>

						<div class="preview_img position-relative d-none overflow-hidden h-100 d-flex align-items-center">
							<img class="show_temp object-fit-cover mx-auto d-block rounded-1 click_show_image img_max_height"
									alt="" />

							<button class="btn_remove_preview_img"
									type="button">
								<i class="icon_remove fa-solid fa-xmark"></i>
							</button>
						</div>
					</div>
				</div>
			</div>
			<div class="py-2">
				<small class="mb-1 d-block font-small"><small class="text-danger">✶</small>${surveyDay}</small>
				<input type="date" name="surveyDates" class="w-100 border-none bg-white green_effect px-2 py-1 rounded-1 text_body" value="${dodayStr}" />
			</div>
		</div>

		<div class="bottom col-sm-6 p-2">
			<div class="py-2">
				<small class="mb-1 d-block font-small">${stageName} (${labelEnglish})</small>
				<input type="text" name="stageNameENs" class="w-100 border-none bg-white green_effect px-2 py-1 rounded-1 text_body" />
			</div>

			<div class="py-2">
				<small class="mb-1 d-block font-small">${stageName} (${labelVietnamese})</small>
				<input type="text" name="stageNameVIs" class="w-100 border-none bg-white green_effect px-2 py-1 rounded-1 text_body" />
			</div>

			<div class="py-2">
				<small class="mb-1 d-block font-small">${weather} (${labelEnglish})</small>
				<input type="text" name="weatherENs" class="w-100 border-none bg-white green_effect px-2 py-1 rounded-1 text_body" />
			</div>

			<div class="py-2">
				<small class="mb-1 d-block font-small">${weather} (${labelVietnamese})</small>
				<input type="text" name="weatherVIs" class="w-100 border-none bg-white green_effect px-2 py-1 rounded-1 text_body" />
			</div>

			<div class="py-2">
				<small class="mb-1 d-block font-small">${height}</small>
				<input type="text" name="heights" class="w-100 border-none bg-white green_effect px-2 py-1 rounded-1 text_body" />
			</div>

			<div class="py-2">
				<small class="mb-1 d-block font-small">${perimeter}</small>
				<input type="text" name="perimeters" class="w-100 border-none bg-white green_effect px-2 py-1 rounded-1 text_body" />
			</div>
		</div>
	`;

	const frameItems = document.createElement("div");
	frameItems.className = "frame_items";
	frameItems.innerHTML = `
		<div class="toggle_title cursor-pointer text-center my-2">
			<h6 class="py-2 bg-black text-white rounded-1">
				${detailImageStage} ${index} (<span class="quantity_item">1</span>/${maxItem})
			</h6>
			<input class="itemPhotoOfStages" name="itemPhotoOfStages" type="text" value="1" hidden />
		</div>

		<div class="info">
			<div class="items bg-form have_move">
				<div class="add_item p-2 mb-3 rounded-1 d-flex flex-wrap gap-1 gap-lg-0 position-relative border_detail">
					<button class="btn_remove_item" type="button">
						<i class="icon_remove_item fa-solid fa-xmark"></i>
					</button>

					<div class="add_img col-12 col-lg-6 rounded-1 d-flex flex-column">
						<p class="catch_to_move text-center text-black"><small class="text-danger">✶</small>${labelPhoto}</p>

						<div class="drop-zone input_img bg-white d-flex justify-content-center align-items-center min_height_input_img rounded-1 green_effect flex-grow-1">
							<button class="btn_add_img outline-none color-tree bg-transparent fs-1 px-4 py-2 border rounded-1 green_effect"
									type="button">
								<i class="icon_add fa-solid fa-plus"></i>
							</button>
							<input class="file" name="ImageFile" type="file" hidden />
							<div class="overflow-hidden w-h-0">
								<input class="image_type" name="dataTypes" />
								<input class="image_data" name="dataBase64s" />
							</div>
						</div>

						<div class="preview_img position-relative d-none">
							<img class="show_temp object-fit-cover mx-auto d-block rounded-1 click_show_image img_max_height"
									alt="" />

							<button class="btn_remove_preview_img"
									type="button">
								<i class="icon_remove fa-solid fa-xmark"></i>
							</button>
						</div>
					</div>

					<div class="add_content col-12 col-lg-6 d-lg-flex flex-lg-wrap p-2">
						<p class="text-center col-12 align-self-start text-black">${labelDescription}</p>

						<div class="add_content_item py-2 col-12">
							<small class="mb-1 d-block font-small">${labelEnglish}</small>
							<input type="text" name="noteENs" class="w-100 border-none bg-white green_effect px-2 py-1 rounded-1 text_body" />

						</div>

						<div class="add_content_item py-2 col-12">
							<small class="mb-1 d-block font-small">${labelVietnamese}</small>
							<input type="text" name="noteVIs" class="w-100 border-none bg-white green_effect px-2 py-1 rounded-1 text_body" />
						</div>
					</div>
				</div>
			</div>

			<div class="text-center py-2">
				<button id="btn_add_item_${index}"
						class="btn_add_item bg-black text-white rounded-1 border-none px-3 py-2"
						type="button"
						onclick="item.addItem('#btn_add_item_${index}');">
					<i class="fa-solid fa-plus"></i>
					${labelBtnAddPhoto}
				</button>
			</div>
		</div>
	`;

	// Thêm hết vào díplay item
	displayItem.appendChild(frameInfoAndPosition);
	displayItem.appendChild(frameItems);

	return displayItem;
}