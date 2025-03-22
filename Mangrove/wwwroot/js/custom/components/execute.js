// Save danh sách của đối tượng
function saveIndex() {
	const url = location.href;
	localStorage.setItem("urlIndex", url);
}

// Lấy ra url index
function getUrlIndex() {
	const url = localStorage.getItem("urlIndex");
	location.href = url;
}