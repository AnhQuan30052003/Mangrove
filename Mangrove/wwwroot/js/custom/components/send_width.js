function sendWidthScreen() {
	window.addEventListener("resize", function () {
		sendWidth();
	});
}
sendWidthScreen();

function sendWidth() {
	const width = screen.width;
	document.cookie = `WidthScreen=${width}; path=/; max-age=86400`;
}
sendWidth();