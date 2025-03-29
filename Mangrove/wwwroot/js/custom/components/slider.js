// Theo dõi các slick slider
function loadSlider() {
	// Lấy - khởi tạo tham chiếu
	const slickSliders = document.querySelectorAll(".slick_slider");
	slickSliders.forEach((slickSlider) => {
		const slides = slickSlider.querySelector(".slides");
		let slideItem = slides.querySelectorAll(".slide_item");
		const description = document.querySelector(".show_description_photo p");		

		// Nếu không có hoặc chỉ 1 slide_item thì không cần tạo Slick Slider
		if (slideItem.length <= 1) {
			description.innerHTML = document.querySelector(".slide_item").getAttribute("note");
			return;
		}
		let loadSuccess = false;

		// Tạo node đầu và cuối slide
		const firstSlide = slideItem[0].cloneNode(true);
		const lastSlide = slideItem[slideItem.length - 1].cloneNode(true);
		slides.insertBefore(lastSlide, slideItem[0]);
		slides.appendChild(firstSlide);
		slideItem = slides.querySelectorAll(".slide_item");

		// Tạo 2 nút prev và next
		const btnPrev = document.createElement("button");
		const btnNext = document.createElement("button");
		btnPrev.classList.add("btn_prev");
		btnNext.classList.add("btn_next");
		btnPrev.innerHTML = "&#10094;"
		btnNext.innerHTML = "&#10095;"
		slickSlider.appendChild(btnPrev);
		slickSlider.appendChild(btnNext);

		// Xử lý khi click
		let index = 1;

		function updateSlide(loadSlide = true) {
			if (loadSlide) slides.style.transition = "all 0.5s ease-in-out";
			else slides.style.transition = "none";

			slides.style.transform = `translateX(-${index * 100}%)`;

			if (index == 0 || index == slideItem.length - 1) {
				index = (index == 0 ? slideItem.length - 2 : 1);
				setTimeout(() => updateSlide(false), 500);
			}

			const note = slideItem[index].getAttribute("note");
			description.innerHTML = note;
		}

		function prevSlide() {
			index -= 1;
			updateSlide();
		}

		function nextSlide() {
			index += 1;
			updateSlide();
		}

		if (!loadSuccess) {
			updateSlide(false);
			loadSuccess = true;
		}

		btnPrev.addEventListener("click", function () {
			prevSlide();
			clearInterval(autoPlaySlide);
			autoPlaySlide = setInterval(() => nextSlide(), 5000);
		});
		btnNext.addEventListener("click", function () {
			nextSlide();
			clearInterval(autoPlaySlide);
			autoPlaySlide = setInterval(() => nextSlide(), 5000);
		});

		let autoPlaySlide = setInterval(() => nextSlide(), 5000);
	});
}
loadSlider();
