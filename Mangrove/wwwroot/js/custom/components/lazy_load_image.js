﻿window.addEventListener("load", function () {
	const lazyImages = document.querySelectorAll(".lazy-load");

	// Cho trình duyệt mới
	if ('IntersectionObserver' in window) {
		const observer = new IntersectionObserver((entries, observer) => {
			entries.forEach(entry => {
				if (entry.isIntersecting) {
					const img = entry.target;
					img.src = img.dataset.src;

					// Đợi tải xong mới hiện
					img.onload = () => img.classList.add("loaded");
					observer.unobserve(img);

					if (img.classList.contains("scale_hover")) {
						setTimeout(() => {
							img.classList.remove("lazy-load");
						}, 1000);
					}

					const minHeightLazy = img.closest(".min-height-lazy");
					if (minHeightLazy) {
						minHeightLazy.classList.remove("min-height-lazy");
					}
				}
			});
		});

		lazyImages.forEach(img => observer.observe(img));
	}
	else {
		// Cho trình duyệt cũ
		lazyImages.forEach(img => img.src = img.dataset.src);
	}
});
