function createItemsPage() {
	try {
		const pagination = document.querySelector(".pagination");

		const totalPages = parseInt(pagination.getAttribute("totalPages"));
		const currentPage = parseInt(pagination.getAttribute("currentPage"));
		const pageSize = parseInt(pagination.getAttribute("pageSize"));
		const search = pagination.getAttribute("search");
		const controller = pagination.getAttribute("controller");
		const action = pagination.getAttribute("action");
		const sortType = pagination.getAttribute("sortType");
		const sortFollow = pagination.getAttribute("sortFollow");

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
			aLeft.className = `arrow mx-1 btn_loading ${currentPage > 1 ? "" : "invisible"}`;
			aLeft.innerHTML = "&#10094";
			groupPage.appendChild(aLeft);

			// Mũi tên phải
			const aRight = document.createElement("a");
			aRight.href = `/${controller}/${action}?search=${search}&currentPage=${currentPage + 1}&pageSize=${pageSize}`;
			aRight.className = `arrow mx-1 btn_loading ${currentPage < totalPages ? "" : "invisible"}`;
			aRight.innerHTML = "&#10095";

			// Tạo các phần tử khác (item)
			if (totalPages <= 10) {
				for (let i = 1; i <= totalPages; i++) {
					const a = createLink(controller, action, search, i, pageSize, currentPage, sortType, sortFollow);
					groupPage.appendChild(a);
				}
			}
			else {
				morePage = true;
				for (let i = 1; i <= itemShow; i++) {
					const a = createLink(controller, action, search, i, pageSize, currentPage, sortType, sortFollow);
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
					const a = createLink(controller, action, search, i, pageSize, currentPage, sortType, sortFollow);
					groupPage.appendChild(a);
				}

			}

			groupPage.appendChild(aRight);
		}

		if (morePage) {
			for (let i = itemShow + 1; i <= totalPages - itemShow; i++) {
				const a = createLink(controller, action, search, i, pageSize, currentPage, sortType, sortFollow);
				pageHidden.appendChild(a);
			}
		}
	}
	catch { }
}
createItemsPage();

// Tạo thẻ a - link (item page)
function createLink(controller, action, search, i, pageSize, currentPage, sortType, sortFollow) {
	const a = document.createElement("a");
	a.innerHTML = i;
	a.className = "page_number";

	if (i == currentPage) a.classList.add("active");
	else {
		a.href = `/${controller}/${action}?search=${search}&currentPage=${i}&pageSize=${pageSize}&sortType=${sortType}&sortFollow=${sortFollow}`;
		a.classList.add("btn_loading");
	}

	return a;
}

// Theo dõi width screen và thay đổi item cho phù hợp
window.addEventListener("resize", function () {
	createItemsPage();
});

// Listener change select item paginate 
function ChangeSelectItemPaginate() {
	const controller = document.querySelector("#controller").value;
	const action = document.querySelector("#action").value;

	const fromDate = document.querySelector("#choose_fromDate").value;
	const toDate = document.querySelector("#choose_toDate").value;
	const chooseData = document.querySelector("#chooseData").value;

	const pageSizeMangrove = document.querySelector("#pageSizeMangrove").value;
	const currentPageMangrove = document.querySelector("#currentPageMangrove").value;
	const sortTypeMangrove = document.querySelector("#sortTypeMangrove").value;
	const sortFollowMangrove = document.querySelector("#sortFollowMangrove").value;

	const pageSizeIndividual = document.querySelector("#pageSizeIndividual").value;
	const currentPageIndividual = document.querySelector("#currentPageIndividual").value;
	const sortTypeIndividual = document.querySelector("#sortTypeIndividual").value;
	const sortFollowIndividual = document.querySelector("#sortFollowIndividual").value;

	const changeSelects = document.querySelectorAll(".changeSelect");
	changeSelects.forEach((select) => {
		select.addEventListener("change", function () {
			let setCurrentePageMangrove = currentPageMangrove, setCurrentPageIndividual = 1;
			let setPageSizeMangrove = pageSizeMangrove, setPageSizeIndividual = select.value;
			if (select.classList.contains("mangrove")) {
				setCurrentePageMangrove = 1;
				setCurrentPageIndividual = currentPageIndividual;

				setPageSizeMangrove = select.value;
				setPageSizeIndividual = pageSizeIndividual;
			}

			const url = `/${controller}/${action}?fromDate=${fromDate}&toDate=${toDate}&chooseTable=${chooseTable}&pageSizeMangrove=${setPageSizeMangrove}&currentPageMangrove=${setCurrentePageMangrove}&sortTypeMangrove=${sortTypeMangrove}&sortFollowMangrove=${sortFollowMangrove}&pageSizeIndividual=${setPageSizeIndividual}&currentPageIndividual=${setCurrentPageIndividual}&sortTypeIndividual=${sortTypeIndividual}&sortFollowIndividual=${sortFollowIndividual}`;
			window.location.href = url;
		});
	});
}
ChangeSelectItemPaginate();