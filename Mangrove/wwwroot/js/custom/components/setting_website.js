function changeOptionDarkBackgroundFooter() {
	try {
		const optionDarkBackground = document.querySelector(".optionDarkBackground");
		optionDarkBackground.addEventListener("change", function () {
			const value = optionDarkBackground.value;
			const background = document.querySelector(".background");

			if (value == "True") {
				background.classList.add("darkBackground");
			}
			else {
				background.classList.remove("darkBackground");
			}
		});
	}
	catch { }
}
changeOptionDarkBackgroundFooter();
