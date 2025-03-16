// Request with AJAX
function requestAjax(url, result = null, changeIcon = true, createItemPage = false) {
	try {
		const xhr = new XMLHttpRequest();
		xhr.open("get", url);
		xhr.setRequestHeader("REQUESTED", "AJAX");
		xhr.onload = function () {
			if (xhr.status == 200) {
				if (result != null) result.innerHTML = xhr.responseText;
				if (changeIcon) changeIconSearchOrWait();
				if (createItemPage) createItemsPage();
			}
		}
		xhr.send();
	}
	catch {
		console.log("Lỗi request AJAX tới URL: " + url);
	}
}

// Thay đổi biểu tượng trong quá trình tìm kiếm
function changeIconSearchOrWait() {
	const iconS = document.querySelector("#icon-s");
	const iconW = document.querySelector("#icon-w");

	if (iconS) iconS.classList.toggle("d-none");
	if (iconW) iconW.classList.toggle("d-none");
}

// Theo dõi việc tìm kiếm các cá thể trong một cây
function searchInvidiual() {
	const searchInvidiual = document.querySelector("#searchInvidiual");
	if (!searchInvidiual) return;

	let timer;
	searchInvidiual.addEventListener("input", function (e) {
		clearTimeout(timer);

		let timeWait = 300;
		if (this.value.length == 0) timeWait = 0;

		timer = setTimeout(() => {
			const value = this.value;
			const id = this.getAttribute("id-mangrove");
			const url = `/Home/Page_Result?id=${id}&searchIndividual=${value}`;
			const result = document.querySelector(".list_individuals");

			if (value.length == 0) {
				requestAjax(url, result, false);
			}
			else {
				changeIconSearchOrWait();
				requestAjax(url, result);
			}
		}, timeWait);
	});
}
searchInvidiual();

// Theo dõi việc tìm kiếm các cây trong thành phân loài
function searchMangroveUser() {
	const search = document.querySelector("#search_mangrove");
	if (!search) return;

	let timer;
	search.addEventListener("input", function (e) {
		clearTimeout(timer);

		let timeWait = 300;
		if (this.value.length == 0) timeWait = 0;

		timer = setTimeout(() => {
			const value = this.value;
			const url = `/Home/Page_SpeciesComposition?search=${value}`;
			const result = document.querySelector(".list_mangrove");

			if (value.length == 0) {
				requestAjax(url, result, false);
			}
			else {
				changeIconSearchOrWait();
				requestAjax(url, result);
			}
		}, timeWait);
	});
}
searchMangroveUser();

// Theo dõi việc tìm kiếm với các cây trong thành phân loài bên admin
function searchTreeAdmin() {
	const forms = document.querySelectorAll(".form_paginate");
	if (forms.length == 0) return;

	forms.forEach((form) => {
		const search = form.querySelector(".search_ajax");
		const controller = form.getAttribute("page");

		let timer;
		search.addEventListener("input", function (e) {
			clearTimeout(timer);

			let timeWait = 300;
			if (this.value.length == 0) timeWait = 0;

			timer = setTimeout(() => {
				const value = this.value;
				const pageSize = form.querySelector(".page_size").value;
				const url = `/${controller}/Page_Index?search=${value}&pageSize=${pageSize}`;
				const result = document.querySelector("#result_search_ajax");

				if (value.length == 0) {
					requestAjax(url, result, false, true);
				}
				else {
					changeIconSearchOrWait();
					requestAjax(url, result, true, true);
				}
			}, timeWait);
		});
	});
}
searchTreeAdmin();

// Tìm kiếm trang web khi thay đổi số lượng page_size
function submitWhenChangePageSize() {
	try {
		const forms = document.querySelectorAll(".form_paginate");
		if (forms.length == 0) return;

		forms.forEach((form) => {
			const select = form.querySelector(".page_size");
			if (select) {
				select.addEventListener("change", function () {
					const pageSize = this.value;
					const findText = form.querySelector(".search_ajax").value;
					const controller = form.getAttribute("page");

					const url = `/${controller}/Page_Index?search=${findText}&currentPage=1&pageSize=${pageSize}`;
					const result = document.querySelector("#result_search_ajax");

					changeIconSearchOrWait();
					requestAjax(url, result, true, true);
				});
			}
		});
	} catch { }
}
submitWhenChangePageSize();

