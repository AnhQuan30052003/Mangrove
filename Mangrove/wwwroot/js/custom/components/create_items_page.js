function createItemsPage() {
	const pagination = document.querySelector(".pagination");

	const totalPages = parseInt(pagination.getAttribute("totalPages"));
	const currentPage = parseInt(pagination.getAttribute("currentPage"));
	const pageSize = parseInt(pagination.getAttribute("pageSize"));
	const search = pagination.getAttribute("search");
	const controller = pagination.getAttribute("controller");
	const action = pagination.getAttribute("action");

	const groupPage = pagination.querySelector(".group_page");
	const pageHidden = pagination.querySelector(".page_hidden");

	// clear items
	groupPage.innerHTML = pageHidden.innerHTML = "";

	let morePage = false;								
	const width = screen.width;
	let itemShow = 0;

	if (width <= 414) itemShow = 2;
	else if (width <= 767) itemShow = 3;
	else if (width <= 992) itemShow = 6;
	else if (width <= 1200) itemShow = 7;
	else itemShow = 12;

	if (totalPages == 1) {
		const a = createLink(controller, action, search, totalPages, pageSize, currentPage);
		groupPage.appendChild(a);
	}
	else {
		// Mũi tên trái
		const aLeft = document.createElement("a");
		aLeft.href = `/${controller}/${action}?search=${search}&currentPage=${currentPage - 1}&pageSize=${pageSize}`;
		aLeft.className = `arrow mx-1 ${currentPage > 1 ? "" : "invisible"}`;
		aLeft.innerHTML = "&#10094";
		groupPage.appendChild(aLeft);

		// Mũi tên phải
		const aRight = document.createElement("a");
		aRight.href = `/${controller}/${action}?search=${search}&currentPage=${currentPage + 1}&pageSize=${pageSize}`;
		aRight.className = `arrow mx-1 ${currentPage < totalPages ? "" : "invisible"}`;
		aRight.innerHTML = "&#10095";

		// Tạo các phần tử khác (item)
		if (totalPages <= 10) {
			for (let i = 1; i <= totalPages; i++) {
				const a = createLink(controller, action, search, i, pageSize, currentPage);
				groupPage.appendChild(a);
			}
		}
		else {
			morePage = true;
			for (let i = 1; i <= itemShow; i++) {
				const a = createLink(controller, action, search, i, pageSize, currentPage);
				groupPage.appendChild(a);
			}

			const a = document.createElement("a");
			const i = document.createElement("i");
			a.className = "page_number cursor-pointer toggle_page_hidden";
			i.className = "fas fa-ellipsis-h";

			if (currentPage > itemShow && currentPage < (totalPages - itemShow + 1)) {
				console.log("bé hơn");
				a.classList.add("bg-black", "text-white");
			}

			a.appendChild(i);
			groupPage.appendChild(a);

			for (let i = totalPages - itemShow + 1; i <= totalPages; i++) {
				const a = createLink(controller, action, search, i, pageSize, currentPage);
				groupPage.appendChild(a);
			}

		}

		groupPage.appendChild(aRight);
	}

	if (morePage) {
		for (let i = itemShow + 1; i <= totalPages - itemShow; i++) {
			const a = createLink(controller, action, search, i, pageSize, currentPage);
			pageHidden.appendChild(a);
		}
	}
}
createItemsPage();

// Tạo thẻ a - link (item page)
function createLink(controller, action, search, i, pageSize, currentPage) {
	const a = document.createElement("a");
	a.innerHTML = i;
	a.className = "page_number";

	if (i == currentPage) a.classList.add("active");
	else a.href = `/${controller}/${action}?search=${search}&currentPage=${i}&pageSize=${pageSize}`;

	return a;
}

// Theo dõi width screen và thay đổi item cho phù hợp
window.addEventListener("resize", function () {
	createItemsPage();
});