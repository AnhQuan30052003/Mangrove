function changeTabStatisticalAdmin() {
	const mod = document.querySelector('.mod_statistical');
	const btnOptions = mod.querySelectorAll(".statistical_option .btn_option");
	const displayOptions = mod.querySelectorAll('.statistical_display_item');

	for (let i = 0; i < btnOptions.length; i++) {
		const btn = btnOptions[i];
		btn.addEventListener("click", function () {
			btnOptions.forEach((item) => item.classList.remove("active"));
			displayOptions.forEach((item) => item.classList.add("d-none"));
			this.classList.add("active");
			displayOptions[i].classList.remove("d-none");
		});
	}
}
changeTabStatisticalAdmin();
